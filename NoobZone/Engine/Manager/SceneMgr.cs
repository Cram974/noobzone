using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    class SceneMgr
    {
        static List<IRenderable> Objects = new List<IRenderable>();
        public static void Clear()
        {
            Objects.Clear();
        }
        public static void Add(IRenderable obj)
        {
            Objects.Add(obj);
        }
        public static void Render()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Render();
            }

        }
    }
}
