using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace CountdownWPF.Utils
{
    public class ChromeUtils
    {
        private const int WM_GETTEXTLENGTH = 0Xe;
        private const int WM_GETTEXT = 0Xd;

        [DllImport("user32.dll")]
        private extern static int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private extern static int SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private extern static IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        public static string GetChromeUrl(IntPtr windowHandle)
        {
            string browserUrl = null;
            IntPtr urlHandle = FindWindowEx(windowHandle, IntPtr.Zero, "Chrome_AutocompleteEditView", null);
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            int length = SendMessage(urlHandle, WM_GETTEXTLENGTH, 0, 0);
            if (length > 0)
            {
                SendMessage(urlHandle, WM_GETTEXT, nChars, Buff);
                browserUrl = Buff.ToString();

                return browserUrl;
            }
            else
            {
                return browserUrl;
            }
        }

        public static string GetChromeUrl(Process proc)
        {
            if (proc.MainWindowHandle == IntPtr.Zero) return null;

            AutomationElement element = AutomationElement.FromHandle(proc.MainWindowHandle);
            if (element == null) return null;

            Condition conditions = new AndCondition(
                new PropertyCondition(AutomationElement.ProcessIdProperty, proc.Id),
                new PropertyCondition(AutomationElement.IsControlElementProperty, true),
                new PropertyCondition(AutomationElement.IsContentElementProperty, true),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

            AutomationElement elementx = element.FindFirst(TreeScope.Descendants, conditions);
            var url = ((ValuePattern)elementx.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;

            return url;
        }
    }
}
