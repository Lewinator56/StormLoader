using StormLoader.mod_handling.install;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormLoader.mod_handling.install
{
    [Serializable]
    class ModPack
    {

        public List<ModFile> modFiles;

        public string name;
        public string path;

        public ModPack(string name, string path)
        {
            this.name = name;
            this.path = path;
            modFiles = new List<ModFile>();
        }

    }
}


