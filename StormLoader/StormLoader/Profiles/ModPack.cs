using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormLoader.Profiles
{
    [Serializable]
    public class ModPack
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public DateTime InstalledOn { get; set; }
        public bool SteamMod { get; set; }
        public string steamID { get; set; }
        public string ContentPath { get; set; }
        public bool Active { get; set; }

        // enable extra data of generic types (remember casts)
        public Dictionary<String, Object> ExtraData { get; set; }



    }
}
