using System;
using System.Runtime.InteropServices;

/// Gets the time of the last user input (in ms since the system started)
/// Notes:
/// Very useful to detect user-idle state of an application.
/// Minimum operating systems: Windows 2000
/// Tips & Tricks:
/// Compare to Environment.TickCount to get the time since the last user input.
namespace CountdownWPF.Utils
{
    public class UserInputInfoUtils
    {
        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// This function retrieves the time in seconds since last user input
        /// </summary>
        /// <returns></returns>
        public static uint GetLastInputTime()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : 0);
        }
    }
}
