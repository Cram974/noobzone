using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.Sdl;

namespace NoobZone
{
    class World
    {
        static Timer timesincelastframe = new Timer();
        static float m_dt = 0;
        static float m_elapsedtime = 0;
        static Vector3 m_up = new Vector3(0, 0, 1);
        static float m_gravity = 9.8f;
        static float m_jumpAcceleration = 4;
        public static Map Map;
        public static float dt
        { 
            get { return m_dt; }
        }
        public static float ElapsedTime
        {
            get { return m_elapsedtime; }
        }
        public static Vector3 Up
        {
            get { return m_up; }
            set { m_up = value; }
        }
        public static float g
        {
            get { return m_gravity; }
            set { m_gravity = value; }
        }
        public static float JumpAcc
        {
            get { return m_jumpAcceleration; }
            set { m_jumpAcceleration = value; }
        }

        public static void Update()
        {
            timesincelastframe.Stop();
            m_dt = timesincelastframe.Elapsed;
            m_elapsedtime += m_dt;
            timesincelastframe.Start();
        }
    }
}
