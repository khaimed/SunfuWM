using System;

namespace SunfuWM
{
    public static class LayoutApplier
    {
        public static void Apply(List<WindowRect> rects)
        {
            foreach (var rect in rects)
            {
                // Skip minimized windows
                if (Interop.IsIconic(rect.Handle))
                    continue;

                // Apply position without changing Z-order or activating
                Interop.SetWindowPos(
                    rect.Handle, 
                    IntPtr.Zero, 
                    rect.X, 
                    rect.Y, 
                    rect.Width, 
                    rect.Height, 
                    Interop.SWP_NOZORDER | Interop.SWP_NOACTIVATE
                );
            }
        }
    }

    public struct WindowRect
    {
        public IntPtr Handle;
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}
