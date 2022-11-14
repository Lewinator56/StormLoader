using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormLoader.mod_handling.install
{
    [Serializable]
    class InstallList
    {
        public List<ModPack> mods;

        public InstallList()
        {

        }

        public bool IsModInstalled(ModPack mp)
        {
            return mods.Contains(mp);
        }


    }
}
