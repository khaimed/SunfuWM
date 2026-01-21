using System;
using System.Collections.Generic;
using System.Text;

namespace SunfuWM
{
    public class WindowEnumerator
    {
        private IVirtualDesktopManager _virtualDesktopManager;

        public WindowEnumerator()
        {
            try
            {
                _virtualDesktopManager = (IVirtualDesktopManager)new VirtualDesktopManager();
            }
            catch { /* VM not supported or failed */ }
        }

        public List<WindowInfo> GetOpenWindows(IntPtr ignoreWindowHandle)
        {
            var windows = new List<WindowInfo>();
            
            // Get Shell Window to ignore it
            IntPtr shellWindow = Interop.GetShellWindow();

            Interop.EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                if (hWnd != ignoreWindowHandle && IsManageableWindow(hWnd, shellWindow))
                {
                    string title = "";
                    int length = Interop.GetWindowTextLength(hWnd);
                    if (length > 0)
                    {
                        StringBuilder sb = new StringBuilder(length + 1);
                        Interop.GetWindowText(hWnd, sb, sb.Capacity);
                        title = sb.ToString();
                    }

                    string processName = "Unknown";
                    uint pid;
                    Interop.GetWindowThreadProcessId(hWnd, out pid);
                    try
                    {
                        using var proc = System.Diagnostics.Process.GetProcessById((int)pid);
                        processName = proc.ProcessName;
                    }
                    catch { /* process might be gone */ }

                    windows.Add(new WindowInfo 
                    { 
                        Handle = hWnd, 
                        Title = string.IsNullOrEmpty(title) ? processName : title,
                        ProcessName = processName 
                    });
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);

            return windows;
        }

        private bool IsManageableWindow(IntPtr hWnd, IntPtr shellWindow)
        {
            if (hWnd == shellWindow) return false;
            
            // 1. Must be visible
            if (!Interop.IsWindowVisible(hWnd)) return false;

            // 2. Must not be cloaked (UWP)
            bool isCloaked;
            Interop.DwmGetWindowAttribute(hWnd, Interop.DWMWA_CLOAKED, out isCloaked, sizeof(bool));
            if (isCloaked) return false;

            // 3. Must not have an owner (ignore popups/dialogs)
            if (Interop.GetWindow(hWnd, Interop.GW_OWNER) != IntPtr.Zero) return false;

            // 4. Styles check
            IntPtr exStylePtr = Interop.GetWindowLongPtr(hWnd, Interop.GWL_EXSTYLE);
            long exStyle = exStylePtr.ToInt64();
            
            // Ignore ToolWindows unless they force AppWindow
            if ((exStyle & Interop.WS_EX_TOOLWINDOW) != 0 && (exStyle & Interop.WS_EX_APPWINDOW) == 0)
                return false;

            // 5. Exclude specific classes
            StringBuilder className = new StringBuilder(256);
            Interop.GetClassName(hWnd, className, className.Capacity);
            if (className.ToString() == "Shell_TrayWnd") return false;

            // Virtual Desktop Check
            if (_virtualDesktopManager != null)
            {
                try
                {
                    if (!_virtualDesktopManager.IsWindowOnCurrentVirtualDesktop(hWnd))
                        return false;
                }
                catch { /* Fail open */ }
            }
            
            return true;
        }
    }
}
