using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SunfuWM
{
    public static class Interop
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);
        
        [DllImport("user32.dll", EntryPoint="GetWindowLongPtr", SetLastError=true)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        // Helper to handle 32/64 bit compatibility for GetWindowLongPtr
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
             if (IntPtr.Size == 8)
                 return GetWindowLongPtr64(hWnd, nIndex);
             else
                 return GetWindowLong(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint="SetWindowLong", SetLastError=true)]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint="SetWindowLongPtr", SetLastError=true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_EX_TOOLWINDOW = 0x00000080;
        public const uint WS_EX_APPWINDOW = 0x00040000;

        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        public const uint GW_OWNER = 4;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError=true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out bool pvAttribute, int cbAttribute);

        public const int DWMWA_CLOAKED = 14;

        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const int SW_RESTORE = 9;
        public const int SW_MAXIMIZE = 3;
    }
}
