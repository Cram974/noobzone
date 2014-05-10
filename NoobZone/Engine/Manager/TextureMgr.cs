using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    class TextureMgr
    {
        static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Texture2D Load(string name)
        {
            Texture2D tex = null;
            if (!Textures.ContainsKey(name))
            {
                tex = new Texture2D(name);
                Textures.Add(name, tex);
            }
            else
                tex = Textures[name];
            return tex;
        }
        public static Texture2D Load(string name, float gamma)
        {
            Texture2D tex = null;
            if (!Textures.ContainsKey(name))
            {
                tex = new Texture2D(name,gamma);
                Textures.Add(name, tex);
            }
            else
                tex = Textures[name];
            return tex;
        }
        public static bool Unload(string name)
        {
            return Textures.Remove(name);
        }
    }
}
