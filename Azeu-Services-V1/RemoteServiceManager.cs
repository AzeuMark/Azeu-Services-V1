using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace AzeuServices_V1
{
    public class RemoteServiceManager
    {
        private static RemoteServiceManager _instance;
        public static RemoteServiceManager Instance => _instance ??= new RemoteServiceManager();

        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cts;
        private AppSettings _activeSettings; // Store the settings currently in use
        private string _cachePath;
        private string _logPath;

        public event Action<string> OnStatusChanged;

        private RemoteServiceManager()
        {
            _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "websocket-cache");
            _logPath = Path.Combine(_cachePath, "websocket.log");
            if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);
        }

        // Updated Start to allow passing temporary test settings
        public void Start(AppSettings customSettings = null)
        {
            Stop();

            // Use provided settings (for testing) or load from disk (for normal run)
            _activeSettings = customSettings ?? AppSettings.Load();

            if (!_activeSettings.EnableRemoteService || string.IsNullOrEmpty(_activeSettings.WebSocketUrl))
            {
                WriteLog("Remote Service stopped: Disabled or URL empty.");
                return;
            }

            _cts = new CancellationTokenSource();
            Task.Run(() => ConnectionLoop(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            _webSocket?.Dispose();
            _webSocket = null;
        }

        private async Task ConnectionLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    OnStatusChanged?.Invoke("Connecting...");
                    _webSocket = new ClientWebSocket();
                    Uri serverUri = new Uri(_activeSettings.WebSocketUrl);

                    await _webSocket.ConnectAsync(serverUri, token);

                    OnStatusChanged?.Invoke("Connected");
                    WriteLog("Connected to server: " + _activeSettings.WebSocketUrl);

                    await SendIdentity();
                    await ReceiveMessages(token);
                }
                catch (Exception ex)
                {
                    if (token.IsCancellationRequested) break;

                    OnStatusChanged?.Invoke("Disconnected");
                    WriteLog("Connection Error: " + ex.Message);

                    await Task.Delay(30000, token);
                }
            }
        }

        private async Task SendIdentity()
        {
            var identity = new
            {
                type = "IDENTITY",
                pc_name = Environment.MachineName,
                token = _activeSettings.WebSocketToken,
                status = "Online"
            };

            string json = JsonSerializer.Serialize(identity);
            await SendString(json);
            WriteLog("Handshake sent to server.");
        }

        private async Task ReceiveMessages(CancellationToken token)
        {
            var buffer = new byte[1024 * 8];
            while (_webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                if (result.MessageType == WebSocketMessageType.Close) break;

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await HandleCommand(message);
            }
        }

        private async Task HandleCommand(string json)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("command", out JsonElement cmdElement))
                {
                    string command = cmdElement.GetString().ToUpper();
                    WriteLog("Received Command: " + command);

                    switch (command)
                    {
                        case "SCREENSHOT":
                            await CaptureAndSendScreenshot();
                            break;
                        case "SHUTDOWN":
                            WriteLog("Remote Shutdown triggered.");
                            ExecutePowerCommand("shutdown", "/s /f /t 0");
                            break;
                        case "RESTART":
                            WriteLog("Remote Restart triggered.");
                            ExecutePowerCommand("shutdown", "/r /f /t 0");
                            break;
                        case "MESSAGE":
                            if (root.TryGetProperty("content", out JsonElement msgElement))
                            {
                                ShowRemoteMessage(msgElement.GetString());
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Command Handling Error: " + ex.Message);
            }
        }

        private async Task CaptureAndSendScreenshot()
        {
            try
            {
                string fileName = $"shot_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string fullPath = Path.Combine(_cachePath, fileName);

                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    using (Bitmap resized = new Bitmap(bitmap, new Size(1280, 720)))
                    {
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);
                        resized.Save(fullPath, jpgEncoder, encoderParameters);
                    }
                }

                byte[] imageBytes = File.ReadAllBytes(fullPath);
                string base64Image = Convert.ToBase64String(imageBytes);

                var response = new
                {
                    type = "SCREENSHOT_DATA",
                    pc_name = Environment.MachineName,
                    image = "data:image/jpeg;base64," + base64Image
                };

                await SendString(JsonSerializer.Serialize(response));
                WriteLog("Screenshot sent.");

                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception ex) { WriteLog("Screenshot Error: " + ex.Message); }
        }

        private void ShowRemoteMessage(string msg)
        {
            Task.Run(() => {
                if (Application.OpenForms.Count > 0)
                {
                    Application.OpenForms[0].Invoke(new Action(() => {
                        RemoteMessageForm popup = new RemoteMessageForm(msg);
                        popup.Show();
                    }));
                }
            });
            WriteLog("Remote Message displayed: " + msg);
        }

        private async Task SendString(string data)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open) return;
            var bytes = Encoding.UTF8.GetBytes(data);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void ExecutePowerCommand(string cmd, string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo(cmd, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi);
        }

        public void WriteLog(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";

                FileInfo fi = new FileInfo(_logPath);
                if (fi.Exists && fi.Length > 2 * 1024 * 1024)
                {
                    string[] lines = File.ReadAllLines(_logPath);
                    var remainingLines = lines.Skip(lines.Length / 2);
                    File.WriteAllLines(_logPath, remainingLines);
                }

                File.AppendAllText(_logPath, logEntry);
                Debug.WriteLine("WS_LOG: " + message);
            }
            catch { }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid) return codec;
            return null;
        }
    }
}