using System.Text.Json.Serialization;

namespace SunfuWM
{
    public class AppConfig
    {
        public int BarHeight { get; set; } = 30;
        public string BarBackgroundColor { get; set; } = "#333333";
        public string BarTextColor { get; set; } = "#EEEEEE";
    }
}
