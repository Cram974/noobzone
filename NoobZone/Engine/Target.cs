using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    class Target : IRenderable
    {
        public Vector3 position;
        Vector3[] sommets;
        Texture2D tex;
        public Target(Vector3 pos)
        {
            position = pos;
            sommets = new Vector3[4];
            sommets[0] = new Vector3(0, -1, 1);
            sommets[1] = new Vector3(0, 1, 1);
            sommets[2] = new Vector3(0, 1, -1);
            sommets[3] = new Vector3(0, -1, -1);
            tex = TextureMgr.Load("Textures/target.jpg");
        }

        public override void Render()
        {
            Gl.glPushMatrix();
            Vector3 trans = Camera.Position;
            Gl.glTranslatef(position.X, position.Y, position.Z);
            Gl.glRotatef(Camera.FPSCam.Teta, 0, 0, 1);
            Gl.glScalef(30, 30, 50);
            tex.Bind();
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3fv(sommets[0].V);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3fv(sommets[1].V);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3fv(sommets[2].V);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3fv(sommets[3].V);
            Gl.glEnd();
            tex.Unbind();

            Gl.glPopMatrix();
        }

        public Vector3[] Poly
        {
            get 
            {
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glPushMatrix();
                Gl.glTranslatef(position.X, position.Y, position.Z);
                Gl.glRotatef(Camera.FPSCam.Teta, 0, 0, 1);
                Gl.glScalef(30, 30, 50);
                Matrix m = Matrix.GetGLModelView();
                Gl.glPopMatrix();

                Vector3[] ret = new Vector3[4];
                for (int i = 0; i < 4; i++)
                {
                    ret[i] = m * sommets[i];
                }

                return ret;

            }
        }
    }
}
