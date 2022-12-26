using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StormLoader.Themes
{
    [Serializable]
    public class ThemeColor
    {
        public byte r;
        public byte g;
        public byte b;

        public ThemeColor(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public SolidColorBrush ToBrush()
        {
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
        public static ThemeColor FromBrush(SolidColorBrush brush)
        {
            return new ThemeColor(brush.Color.R, brush.Color.G, brush.Color.B);
        }
    }
}
