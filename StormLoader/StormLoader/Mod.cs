using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormLoader
{
    // stores data about mods for use in install queues
    public class Mod
    {
        public string path;
        public string name;

        public Mod(string name, string path)
        {
            this.path = path;
            this.name = name;
        }
    }
}
