using StormLoader.Profiles;
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
        public ModPack pack;
        public string path;
        public string name;
        public bool active;

        public Mod(ModPack pack)
        {
            this.pack = pack;
        }
    }
}
