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
        public Action<string> OnRequestShutdown;
        public Action<string> OnRequestRestart;
        public Action OnRequestBypassCurfew;

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
                WriteLog("Remote Service is disabled in settings.");
                OnStatusChanged?.Invoke("Disabled");
                return;
            }

            _cts = new CancellationTokenSource();
            // Run the main connection loop in a long-running background thread
            Task.Factory.StartNew(() => ConnectionLoop(_cts.Token), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            try
            {
                _cts?.Cancel();
                _webSocket?.Abort();
                _webSocket?.Dispose();
                _webSocket = null;
            }
            catch { }
        }

        private async Task ConnectionLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    OnStatusChanged?.Invoke("Connecting...");
                    _webSocket = new ClientWebSocket();

                    // Set Internal Keep-Alive to help with Stealth Mode stability
                    _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);

                    Uri serverUri = new Uri(_activeSettings.WebSocketUrl);
                    await _webSocket.ConnectAsync(serverUri, token);

                    OnStatusChanged?.Invoke("Connected");
                    WriteLog("Connected to server: " + _activeSettings.WebSocketUrl);

                    await SendIdentity();

                    // Start a dedicated Heartbeat task for this specific connection
                    _ = RunHeartbeat(token);

                    // Start receiving messages
                    await ReceiveMessages(token);
                }
                catch (Exception ex)
                {
                    if (token.IsCancellationRequested) break;
                    OnStatusChanged?.Invoke("Disconnected");
                    WriteLog("Connection Lost: " + ex.Message + ". Reconnecting in 10s...");
                    await Task.Delay(10000, token); // Wait before retrying
                }
            }
        }

        private async Task RunHeartbeat(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _webSocket?.State == WebSocketState.Open)
            {
                try
                {
                    // Every 10 seconds is the industry standard for stable connections
                    await Task.Delay(10000, token);

                    // We check uptime and lock state here internally
                    TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
                    string uptimeText = uptime.TotalDays >= 1
                        ? $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m"
                        : $"{uptime.Hours:D2}h {uptime.Minutes:D2}m {uptime.Seconds:D2}s";

                    // Detect lock state (Search for LimitClosedForm)
                    bool isLocked = Application.OpenForms.Cast<Form>().Any(f => f.Name == "LimitClosedForm");

                    await SendStatusUpdate("Service Active", uptimeText, isLocked);
                }
                catch { break; }
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
            await SendString(JsonSerializer.Serialize(identity));
            WriteLog("Identity handshake sent.");
        }

        public async Task SendStatusUpdate(string countdownText, string uptimeText, bool isLocked)
        {
            if (_webSocket?.State != WebSocketState.Open) return;
            var status = new
            {
                type = "STATUS_UPDATE",
                pc_name = Environment.MachineName,
                countdown = countdownText,
                uptime = uptimeText,
                isLocked = isLocked
            };
            await SendString(JsonSerializer.Serialize(status));
        }

        private async Task ReceiveMessages(CancellationToken token)
        {
            var buffer = new byte[1024 * 8];
            while (_webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    WriteLog("Server requested closure.");
                    break;
                }

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
                        case "SCREENSHOT": await CaptureAndSendScreenshot(); break;
                        case "SHUTDOWN": OnRequestShutdown?.Invoke("Remote Web Command"); break;
                        case "RESTART": OnRequestRestart?.Invoke("Remote Web Command"); break;
                        case "BYPASS_CURFEW": OnRequestBypassCurfew?.Invoke(); break;
                        case "NAVIGATE":
                            if (root.TryGetProperty("content", out JsonElement urlElement))
                                NavigateToUrl(urlElement.GetString());
                            break;
                        case "MESSAGE":
                            if (root.TryGetProperty("content", out JsonElement msgElement))
                                ShowRemoteMessage(msgElement.GetString());
                            break;
                    }
                }
            }
            catch (Exception ex) { WriteLog("Command Error: " + ex.Message); }
        }

        private void NavigateToUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                WriteLog("Remote Nav: " + url);
            }
            catch { }
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
                        ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().First(x => x.FormatID == ImageFormat.Jpeg.Guid);
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
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch { }
        }

        private void ShowRemoteMessage(string msg)
        {
            Task.Run(() => {
                try
                {
                    Form syncTarget = null;
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f.IsHandleCreated && !f.IsDisposed)
                        {
                            syncTarget = f;
                            break;
                        }
                    }

                    if (syncTarget != null)
                    {
                        syncTarget.BeginInvoke(new Action(() => {
                            RemoteMessageForm popup = new RemoteMessageForm(msg);
                            popup.Show();

                            // We added this in the previous fix, keep it to ensure Z-order
                            popup.BringToFront();
                        }));
                    }
                    else
                    {
                        Thread messageThread = new Thread(() => {
                            RemoteMessageForm popup = new RemoteMessageForm(msg);
                            Application.Run(popup);
                        });
                        messageThread.SetApartmentState(ApartmentState.STA);
                        messageThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    WriteLog("Failed to show remote message: " + ex.Message);
                }
            });
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
                File.AppendAllText(_logPath, logEntry);

                // Keep log file small (Trim if over 1MB)
                FileInfo fi = new FileInfo(_logPath);
                if (fi.Length > 1024 * 1024)
                {
                    var lines = File.ReadAllLines(_logPath).Skip(500);
                    File.WriteAllLines(_logPath, lines);
                }
            }
            catch { }
        }
    }
}