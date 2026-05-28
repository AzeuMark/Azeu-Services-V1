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
        private AppSettings _activeSettings;
        private string _cachePath;
        private string _logPath;

        public event Action<string> OnStatusChanged;

        // --- NEW: DELEGATES TO NOTIFY FORM1 ---
        public Action<string> OnRequestShutdown;
        public Action<string> OnRequestRestart;

        private RemoteServiceManager()
        {
            _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "websocket-cache");
            _logPath = Path.Combine(_cachePath, "websocket.log");
            if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);
        }

        public void Start(AppSettings customSettings = null)
        {
            Stop();
            _activeSettings = customSettings ?? AppSettings.Load();

            if (!_activeSettings.EnableRemoteService || string.IsNullOrEmpty(_activeSettings.WebSocketUrl))
            {
                WriteLog("Remote Service stopped.");
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
                    await Task.Delay(1000);
                    await CaptureAndSendScreenshot();

                    await ReceiveMessages(token);
                }
                catch (Exception ex)
                {
                    if (token.IsCancellationRequested) break;
                    OnStatusChanged?.Invoke("Disconnected");
                    WriteLog("Connection Error: " + ex.Message);
                    await Task.Delay(10000, token);
                }
            }
        }

        private async Task SendIdentity()
        {
            var identity = new { type = "IDENTITY", pc_name = Environment.MachineName, token = _activeSettings.WebSocketToken, status = "Online" };
            await SendString(JsonSerializer.Serialize(identity));
            WriteLog("Identity handshake sent.");
        }

        public async Task SendStatusUpdate(string countdownText)
        {
            if (_webSocket?.State != WebSocketState.Open) return;
            var status = new { type = "STATUS_UPDATE", pc_name = Environment.MachineName, countdown = countdownText };
            await SendString(JsonSerializer.Serialize(status));
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

                // Ensure we are looking for the "command" property
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
                            OnRequestShutdown?.Invoke("Remote Web Command");
                            break;
                        case "RESTART":
                            OnRequestRestart?.Invoke("Remote Web Command");
                            break;
                        case "MESSAGE":
                            // FIX: Ensure 'content' property exists and has text before showing popup
                            if (root.TryGetProperty("content", out JsonElement msgElement))
                            {
                                string messageText = msgElement.GetString();
                                if (!string.IsNullOrEmpty(messageText))
                                {
                                    ShowRemoteMessage(messageText);
                                }
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

        public async Task CaptureAndSendScreenshot()
        {
            try
            {
                string fullPath = Path.Combine(_cachePath, "temp_shot.jpg");
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap)) g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    using (Bitmap resized = new Bitmap(bitmap, new Size(1280, 720)))
                    {
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        EncoderParameters ep = new EncoderParameters(1);
                        ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);
                        resized.Save(fullPath, jpgEncoder, ep);
                    }
                }

                string base64Image = Convert.ToBase64String(File.ReadAllBytes(fullPath));

                var response = new
                {
                    type = "SCREENSHOT_DATA",
                    pc_name = Environment.MachineName,
                    image = "data:image/jpeg;base64," + base64Image,
                    date = DateTime.Now.ToString("MMMM dd, yyyy"),
                    time = DateTime.Now.ToString("hh:mm:ss tt")
                };

                await SendString(JsonSerializer.Serialize(response));
                WriteLog("Screenshot sent.");
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception ex) { WriteLog("Screenshot Error: " + ex.Message); }
        }

        private void ShowRemoteMessage(string msg)
        {
            // Use BeginInvoke on an existing form to ensure Thread Safety
            // This allows multiple messages to spawn without blocking the socket
            Task.Run(() => {
                if (Application.OpenForms.Count > 0)
                {
                    var targetForm = Application.OpenForms[0];
                    if (targetForm.IsHandleCreated)
                    {
                        targetForm.BeginInvoke(new Action(() => {
                            RemoteMessageForm popup = new RemoteMessageForm(msg);
                            popup.Show();
                        }));
                    }
                }
            });
            WriteLog("Remote Message processed: " + msg);
        }

        private async Task SendString(string data)
        {
            if (_webSocket?.State != WebSocketState.Open) return;
            var bytes = Encoding.UTF8.GetBytes(data);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
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
                    File.WriteAllLines(_logPath, lines.Skip(lines.Length / 2));
                }
                File.AppendAllText(_logPath, logEntry);
            }
            catch { }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(x => x.FormatID == format.Guid);
        }
    }
}