using System.Windows.Media;

namespace SunfuWM
{
    public class WorkspaceInfo
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public Brush Background => IsActive ? new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD7)) : new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x44));
    }
}
