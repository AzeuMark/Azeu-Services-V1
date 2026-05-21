using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public class ActivityMonitor
    {
        public bool IsKbAfk { get; private set; }
        public bool IsMouseAfk { get; private set; }
        public bool IsKbSuspicious { get; private set; }
        public bool IsMouseClickSuspicious { get; private set; }

        public bool IsSuspicious => IsKbSuspicious || IsMouseClickSuspicious;

        private int kbSecs = 0;
        private int mouseSecs = 0;
        private int clickResetTimer = 0;

        private Point lastPos;
        private List<string> kbHistory = new List<string>();
        private List<int> mouseClickHistory = new List<int>();

        private IntPtr _kbHook, _mouseHook;
        private LowLevelKeyboardProc _kbDelegate;
        private LowLevelMouseProc _mouseDelegate;

        public ActivityMonitor()
        {
            _kbDelegate = KeyboardHookHandler;
            _mouseDelegate = MouseHookHandler;
            _kbHook = SetHook(13, _kbDelegate);
            _mouseHook = SetHook(14, _mouseDelegate);
            lastPos = Cursor.Position;
        }

        private IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)0x0100) // WM_KEYDOWN
            {
                kbSecs = 0;
                IsKbAfk = false;

                // FIX: If a key is pressed, the mouse cannot be considered "Click Suspicious"
                IsMouseClickSuspicious = false;
                mouseClickHistory.Clear();

                Keys key = (Keys)Marshal.ReadInt32(lParam);
                string keyStr = key.ToString();

                if (!keyStr.Contains("Shift") && !keyStr.Contains("Control") && !keyStr.Contains("Menu"))
                {
                    CheckKbPatterns(keyStr);
                }
            }
            return CallNextHookEx(_kbHook, nCode, wParam, lParam);
        }

        private IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int msg = (int)wParam;
            if (nCode >= 0 && (msg == 0x0201 || msg == 0x0204 || msg == 0x0207 || msg == 0x020A))
            {
                mouseSecs = 0;
                IsMouseAfk = false;
                TrackMouseClicks(msg);
            }
            return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
        }

        public void Update()
        {
            // 1. Mouse Movement Check
            if (Cursor.Position != lastPos)
            {
                mouseSecs = 0;
                IsMouseAfk = false;
                lastPos = Cursor.Position;

                // FIX: If the mouse moves, the keyboard is no longer "Suspicious" 
                // because a human is clearly interacting with the mouse.
                IsKbSuspicious = false;
                kbHistory.Clear();

                mouseClickHistory.Clear();
                IsMouseClickSuspicious = false;
            }
            else
            {
                mouseSecs++;
                if (mouseSecs >= 2) IsMouseAfk = true;
            }

            // 2. Keyboard Check
            kbSecs++;
            if (kbSecs >= 2)
            {
                IsKbAfk = true;
                IsKbSuspicious = false;
                kbHistory.Clear();
            }

            // 3. Click History Cleanup
            clickResetTimer++;
            if (clickResetTimer >= 5)
            {
                mouseClickHistory.Clear();
                IsMouseClickSuspicious = false;
                clickResetTimer = 0;
            }
        }

        private void CheckKbPatterns(string key)
        {
            kbHistory.Add(key);
            if (kbHistory.Count > 10) kbHistory.RemoveAt(0);
            if (kbHistory.Count < 6) return;

            string flat = string.Join("", kbHistory).ToLower();
            bool spam = kbHistory.Skip(kbHistory.Count - 5).All(x => x == kbHistory.Last());
            bool pat = flat.EndsWith(flat.Substring(flat.Length - 4, 2) + flat.Substring(flat.Length - 4, 2));

            IsKbSuspicious = spam || pat;
        }

        private void TrackMouseClicks(int msgCode)
        {
            clickResetTimer = 0;
            mouseClickHistory.Add(msgCode);
            if (mouseClickHistory.Count > 5) mouseClickHistory.RemoveAt(0);

            if (mouseClickHistory.Count == 5 && mouseClickHistory.All(x => x == mouseClickHistory[0]))
            {
                IsMouseClickSuspicious = true;
            }
            else
            {
                IsMouseClickSuspicious = false;
            }
        }

        public void Stop()
        {
            UnhookWindowsHookEx(_kbHook);
            UnhookWindowsHookEx(_mouseHook);
        }

        [DllImport("user32.dll")] private static extern IntPtr SetWindowsHookEx(int id, Delegate lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string lpModuleName);
        private delegate IntPtr LowLevelKeyboardProc(int n, IntPtr w, IntPtr l);
        private delegate IntPtr LowLevelMouseProc(int n, IntPtr w, IntPtr l);

        private IntPtr SetHook(int id, Delegate proc)
        {
            using (Process curP = Process.GetCurrentProcess())
            using (ProcessModule curM = curP.MainModule!)
                return SetWindowsHookEx(id, proc, GetModuleHandle(curM.ModuleName!), 0);
        }
    }
}