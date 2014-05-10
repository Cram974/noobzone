using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace NoobZone
{
    class Map
    {
        Q3BSP q3level;
        SpawnPoint[] spawnpoints;
        Sky skybox;
        public Q3BSP Level
        {
            get { return q3level; }
        }
        public SpawnPoint[] SpawnPoints
        {
            get { return spawnpoints; }
        }
        public Son drivsound;
        public Map(string name)
        {
            StreamReader sr = new StreamReader(new FileStream(name, FileMode.Open));

            q3level = new Q3BSP(sr.ReadLine());
            skybox = new Sky(sr.ReadLine(), ".bmp");
            Camera.DrivenCam = new Camera_D(sr.ReadLine());
            drivsound = new Son(sr.ReadLine());
            spawnpoints = new SpawnPoint[2];
            string[] buff = sr.ReadLine().Split(' ');
            spawnpoints[0] = new SpawnPoint();
            spawnpoints[0].position = new Vector3(Convert.ToSingle(buff[0]),
                                                  Convert.ToSingle(buff[1]),
                                                  Convert.ToSingle(buff[2]));
            spawnpoints[0].phi = Convert.ToSingle(buff[3]);
            spawnpoints[0].teta = Convert.ToSingle(buff[4]);

            buff = sr.ReadLine().Split(' ');
            spawnpoints[1] = new SpawnPoint();
            spawnpoints[1].position = new Vector3(Convert.ToSingle(buff[0]),
                                                  Convert.ToSingle(buff[1]),
                                                  Convert.ToSingle(buff[2]));
            spawnpoints[1].phi = Convert.ToSingle(buff[3]);
            spawnpoints[1].teta = Convert.ToSingle(buff[4]);

            World.g = Convert.ToSingle(sr.ReadLine());
            World.JumpAcc = Convert.ToSingle(sr.ReadLine());

            int nbmunitbox = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < nbmunitbox; i++)
            {
                buff = sr.ReadLine().Split(' ');
                Vector3 pos = new Vector3(Convert.ToSingle(buff[0]),
                                          Convert.ToSingle(buff[1]),
                                          Convert.ToSingle(buff[2]));
                Dropbox b = new Dropbox(Dropbox.Type.Munition, pos);
                DropboxMgr.BoxList.Add(b);
                SceneMgr.Add(b);
            }
            int nbhealthbox = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < nbhealthbox; i++)
            {
                buff = sr.ReadLine().Split(' ');
                Vector3 pos = new Vector3(Convert.ToSingle(buff[0]),
                                          Convert.ToSingle(buff[1]),
                                          Convert.ToSingle(buff[2]));
                Dropbox b = new Dropbox(Dropbox.Type.Health, pos);
                DropboxMgr.BoxList.Add(b);
                SceneMgr.Add(b);
            }

            int nbtarget = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < nbtarget; i++)
            {
                buff = sr.ReadLine().Split(' ');
                Vector3 pos = new Vector3(Convert.ToSingle(buff[0]),
                                          Convert.ToSingle(buff[1]),
                                          Convert.ToSingle(buff[2]));
                TargetMgr.Targets.Add(new Target(pos));
            }

            sr.Close();
        }

        public void Render()
        {
            skybox.Render();
            q3level.Render();
        }
    }

    class SpawnPoint
    {
        public float teta;
        public float phi;
        public Vector3 position;
    }

}
