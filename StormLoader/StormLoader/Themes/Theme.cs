using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StormLoader.Themes
{
    [Serializable]
    public class Theme
    {
        
        public Dictionary<string, ThemeColor> colors;

        public Theme()
        {
            colors = new Dictionary<string, ThemeColor>();
        }

        public void SetColor(string key, SolidColorBrush c)
        {
            if (!colors.ContainsKey(key))
            {
                colors.Add(key, ThemeColor.FromBrush(c));
            } else
            {
                colors[key] = ThemeColor.FromBrush(c);
            }
            
        }
        public SolidColorBrush GetColor(string key)
        {
            return colors[key].ToBrush();
        }
    }
}
