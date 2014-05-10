using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    class TargetMgr
    {

        public static bool hasloose = false;
        public static bool hasStarted;
        public static float nextTimeup = 60;
        const float startimeup = 60;
        static Random rnd = new Random();
        public static List<Target> Targets = new List<Target>();
        public static int ActiveTarget = -1;
        public static void ChooseTarget()
        {
            if (Targets.Count > 0)
                ActiveTarget = rnd.Next(0, Targets.Count);
        }

        public static void Start()
        {
            ChooseTarget();
            hasStarted = true;
            hasloose = false;
            nextTimeup = startimeup;
        }
        public static void Stop()
        {
            ActiveTarget = -1;
            hasStarted = false;
        }
        public static void Update()
        {
            if (hasStarted)
                nextTimeup -= World.dt;
            if (nextTimeup <= 0)
            {
                hasloose = true;
                nextTimeup = 0;
                hasStarted = false;
                ActiveTarget = -1;
                Camera.SetDriven();
            }
        }
    }
}
