using System;

namespace SunfuWM
{
    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public bool IsFocused { get; set; }

        public override string ToString()
        {
            return ProcessName;
        }
    }
}
