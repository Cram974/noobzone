using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    class DropboxMgr
    {
        static Son healthsound;
        static Son munitsound;

        public static List<Dropbox> BoxList = new List<Dropbox>();
        public static void Init()
        {
            healthsound = new Son("Sons/health_box.wav");
            munitsound = new Son("Sons/munit_box.wav");
        }
        public static void TryToDrop(AABB box, string player)
        {
            foreach (Dropbox b in BoxList)
            {
                if (Collision.IsColliding(b.box, box) && !b.hidden)
                {
                    if (((Player)Camera.FPSCam).Name == player)
                    {
                        if (b.type == Dropbox.Type.Health)
                        {
                            healthsound.Play();
                            TargetMgr.nextTimeup += 5;
                        }
                        if (b.type == Dropbox.Type.Munition)
                        {
                            munitsound.Play();
                            ((Player)Camera.FPSCam).ChargerCount = 10;
                            ((Player)Camera.FPSCam).MunitCount = Player.munitMax;
                        }
                    }
                    b.hidden = true;
                }
            }
        }
        public static void Update()
        {
            foreach (Dropbox b in BoxList)
                b.Update();
        }
    }
}
