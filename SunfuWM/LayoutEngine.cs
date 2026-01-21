using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SunfuWM
{
    public class LayoutEngine
    {
        public List<WindowRect> GetLayoutRects(string layoutName, List<WindowInfo> windows)
        {
            var rects = new List<WindowRect>();
            if (windows == null || windows.Count == 0) return rects;

            // Use WPF's SystemParameters to get safe work area (excludes Taskbar)
            var workArea = SystemParameters.WorkArea;
            int screenX = (int)workArea.X;
            int screenY = (int)workArea.Y;
            int screenW = (int)workArea.Width;
            int screenH = (int)workArea.Height;

            if (layoutName == "Full")
            {
                // Full layout: only the focused window (or the first one if none focused) fills the screen
                var target = windows.FirstOrDefault(w => w.IsFocused) ?? windows.First();
                rects.Add(new WindowRect 
                { 
                    Handle = target.Handle, 
                    X = screenX, 
                    Y = screenY, 
                    Width = screenW, 
                    Height = screenH 
                });
            }
            else if (layoutName == "Tall")
            {
                int masterWidth = (int)(screenW * 0.6);
                int stackWidth = screenW - masterWidth;
                int stackCount = windows.Count - 1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (i == 0) // Master
                    {
                        rects.Add(new WindowRect
                        {
                            Handle = windows[i].Handle,
                            X = screenX,
                            Y = screenY,
                            Width = (windows.Count == 1) ? screenW : masterWidth,
                            Height = screenH
                        });
                    }
                    else // Stack
                    {
                        int stackItemHeight = screenH / stackCount;
                        int yPos = screenY + ((i - 1) * stackItemHeight);
                        rects.Add(new WindowRect
                        {
                            Handle = windows[i].Handle,
                            X = screenX + masterWidth,
                            Y = yPos,
                            Width = stackWidth,
                            Height = stackItemHeight
                        });
                    }
                }
            }
            return rects;
        }
    }
}
