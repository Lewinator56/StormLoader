using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace StormLoader.Profiles
{
    [Serializable]
    class Profile
    {
        public List<ModPack> ModPacks { get; set; }

        public string Name { get; set; }

        public Profile(string name)
        {
            Name = name;
            ModPacks = new List<ModPack>();
        }

        public Profile()
        {
            ModPacks = new List<ModPack>();
        }

        public void AddMod(ModPack mod)
        {
            ModPacks.Add(mod);
        }

        public void RemoveMod(ModPack Mod)
        {
            ModPacks.Remove(Mod);
        }

        public void Save(string path)
        {
            
            BinaryFormatter f = new BinaryFormatter();
            using (Stream s = File.OpenWrite(path))
            {
                f.Serialize(s, this);
            }
        }

        public static Profile Load(string path)
        {
            try
            {
                BinaryFormatter f = new BinaryFormatter();
                using (Stream s = File.OpenRead(path))
                {
                    return (Profile)f.Deserialize(s);
                }
            }
            catch
            {
                return new Profile();
            }
            
        }

        
    }
}
