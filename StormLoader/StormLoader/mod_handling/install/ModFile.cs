using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormLoader.mod_handling.install
{
    [Serializable]
    class ModFile
    {
        string name;
        public string installPath;
        public string contentPath;
        ModFile overwrites = null;
        ModFile overwritten = null;
        public bool active = true;

        public ModFile(string path)
        {
            this.installPath = path;
        }

        public bool IsOverwritten()
        {
            return overwritten != null;
        }

        public bool Overwrites()
        {
            return overwrites != null;
        }

        public ModFile GetOverwrites()
        {
            return overwrites != null ? overwrites : null;
        }

        public ModFile GetOverwrittenBy()
        {
            return overwritten != null? overwritten : null;
        }

        public void SetOverwrittenBy(ModFile file)
        {
            overwritten = file;
        }

        public void SetOverwrites(ModFile file)
        {
            overwrites = file;
        }


    }
}
