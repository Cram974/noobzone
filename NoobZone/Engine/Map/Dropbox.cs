using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    class Dropbox : IRenderable
    {
        public static List<Dropbox> BoxList= new List<Dropbox>();

        public enum Type
        {
            Munition,
            Health
        }
        public Dropbox.Type type;
        MD2 skin;
        Vector3 position;
        public AABB box;
        public bool hidden;
        float reappeartime;
        float teta;
        float currframe;


        public Dropbox(Type type, Vector3 pos)
        {
            if (type == Type.Munition)
            {
                skin = new MD2("Models/munition.md2", "Textures/munition.jpg");
            }
            else if (type == Type.Health)
            {
                skin = new MD2("Models/health.md2", "Textures/health.jpg");
            }
            this.type = type;
            teta = 0;
            reappeartime = 60;
            currframe = 0;
            hidden = false;
            position = pos;

            Update();
        }

        public override void Render()
        {
            if (!hidden && Camera.Frustrum.BoxInFrustrum(skin.getAABB(0)))
                skin.Render(0);
        }

        public void Update()
        {
            currframe += World.dt*4;
            teta += World.dt*2;
            if (teta >= 360) teta = 0;
            if (currframe >= 2 * Math.PI) currframe = 0;
            if (hidden)
                reappeartime -= World.dt;
            if (reappeartime < 0)
            {
                hidden = false;
                reappeartime = 60;
            }


            if (currframe >= Math.PI * 2)
                currframe = 0;
            if (teta >= Math.PI * 2)
                teta = 0;
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glTranslatef(position.X, position.Y, position.Z - 30f - 0.3f + (float)Math.Sin(currframe) * 5f);
            Gl.glRotated(((teta * 180) / Math.PI), 0, 0, 1);
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, skin.Matrice.Mat);
            Gl.glPopMatrix();

            box = skin.getAABB(0);
        }
    }
}
