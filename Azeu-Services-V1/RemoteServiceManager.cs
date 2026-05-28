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

namespace AzeuServices_V1
{
    public class RemoteServiceManager
    {
        private static RemoteServiceManager _instance;
        public static RemoteServiceManager Instance => _instance ??= new RemoteServiceManager();

        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cts;
        private AppSettings _settings;
        private string _cachePath;
        private string _logPath;

        public event Action<string> OnStatusChanged;

        private RemoteServiceManager()
        {
            _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "websocket-cache");
            _logPath = Path.Combine(_cachePath, "websocket.log");
            if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);
        }

        public void Start()
        {
            Stop(); // Reset if already running
            _settings = AppSettings.Load();

            if (!_settings.EnableRemoteService || string.IsNullOrEmpty(_settings.WebSocketUrl))
            {
                WriteLog("Remote Service disabled or URL missing.");
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
                    Uri serverUri = new Uri(_settings.WebSocketUrl);

                    await _webSocket.ConnectAsync(serverUri, token);
                    OnStatusChanged?.Invoke("Connected");
                    WriteLog("Connected to server: " + _settings.WebSocketUrl);

                    // 1. Send Identity Handshake (Option A)
                    await SendIdentity();

                    // 2. Start receiving messages
                    await ReceiveMessages(token);
                }
                catch (Exception ex)
                {
                    OnStatusChanged?.Invoke("Disconnected");
                    WriteLog("Connection Error: " + ex.Message);
                    await Task.Delay(30000, token); // Retry every 30 seconds
                }
            }
        }

        private async Task SendIdentity()
        {
            var identity = new
            {
                type = "IDENTITY",
                pc_name = Environment.MachineName,
                token = _settings.WebSocketToken,
                status = "Online"
            };

            string json = JsonSerializer.Serialize(identity);
            await SendString(json);
        }

        private async Task ReceiveMessages(CancellationToken token)
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                if (result.MessageType == WebSocketMessageType.Close) break;

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                WriteLog("Received Command: " + message);
                await HandleCommand(message);
            }
        }

        private async Task HandleCommand(string json)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                string command = doc.RootElement.GetProperty("command").GetString().ToUpper();

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
                        string content = doc.RootElement.GetProperty("content").GetString();
                        ShowRemoteMessage(content);
                        break;
                }
            }
            catch (Exception ex) { WriteLog("Command Handling Error: " + ex.Message); }
        }

        private async Task CaptureAndSendScreenshot()
        {
            try
            {
                string fileName = $"shot_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string fullPath = Path.Combine(_cachePath, fileName);

                // Perform capture on UI thread or via Screen bounds
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    // Efficiency: Downscale to 720p for pisonet performance
                    using (Bitmap resized = new Bitmap(bitmap, new Size(1280, 720)))
                    {
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);

                        resized.Save(fullPath, jpgEncoder, encoderParameters);
                    }
                }

                // Convert to Base64 and send
                byte[] imageBytes = File.ReadAllBytes(fullPath);
                string base64Image = Convert.ToBase64String(imageBytes);

                var response = new
                {
                    type = "SCREENSHOT_DATA",
                    pc_name = Environment.MachineName,
                    image = "data:image/jpeg;base64," + base64Image
                };

                await SendString(JsonSerializer.Serialize(response));
                WriteLog("Screenshot sent and deleted.");

                // Failsafe deletion
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception ex) { WriteLog("Screenshot Error: " + ex.Message); }
        }

        private void ShowRemoteMessage(string msg)
        {
            // Use Task.Run and Invoke to ensure we don't block the WebSocket thread
            // and that the UI form opens on the Main thread.
            Task.Run(() => {
                // Find the active settings form or main form to invoke upon
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

        private void WriteLog(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";

                // Ensure the directory exists before writing
                if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);

                // Rolling Log Logic (Prune if file size is > 2MB)
                FileInfo fi = new FileInfo(_logPath);
                if (fi.Exists && fi.Length > 2 * 1024 * 1024)
                {
                    // Read all lines from the log
                    string[] lines = File.ReadAllLines(_logPath);

                    // Logic: Skip the first 50% of the lines to prune the oldest data
                    // We use System.Linq.Enumerable.Skip to ensure compatibility
                    var remainingLines = System.Linq.Enumerable.Skip(lines, lines.Length / 2);

                    // Overwrite the log file with the remaining (newer) lines
                    File.WriteAllLines(_logPath, remainingLines);

                    // Add a separator indicating a prune happened
                    File.AppendAllText(_logPath, $"--- Log Pruned at {DateTime.Now} to save space ---{Environment.NewLine}");
                }

                // Append the new log entry
                File.AppendAllText(_logPath, logEntry);

                // Also output to the Debug console for development
                Debug.WriteLine("WS_LOG: " + message);
            }
            catch (Exception ex)
            {
                // Fail silently to prevent crashing the main app if disk is full/locked
                Debug.WriteLine("CRITICAL: Could not write to log: " + ex.Message);
            }
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