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
            mods = new List<ModPack>();
        }

        // yeah.. i did initially check it contained the instance... wouldnt work
        // maybe update to add a version check, though in reality i should only check on the name
        public bool IsModInstalled(ModPack mp)
        {
            foreach (var mod in mods)
            {
                if (mod.name == mp.name)
                {
                    return true;
                    
                }
            }
            return false;
        }

        public bool AddMod(ModPack mp)
        {
            if (IsModInstalled(mp))
            {
                return false;
            }
            mods.Add(mp);
            return true;
        }

       


    }
}
