using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Countdown.WorkerService.Utilities
{
    public class ProcessUtils
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static Process GetActiveProcess()
        {
            IntPtr hwnd = GetForegroundWindow();

            return GetProcessByHandle(hwnd);
        }

        private static Process GetProcessByHandle(IntPtr hwnd)
        {
            try
            {
                uint processID;
                GetWindowThreadProcessId(hwnd, out processID);
                return Process.GetProcessById((int)processID);
            }
            catch
            {
                return null;
            }
        }
    }
}
