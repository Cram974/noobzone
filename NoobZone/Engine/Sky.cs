using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    class Sky : IRenderable
    {
        public static Sky Instance = null;
        enum Orient
        {
            NORTH = 0,
            EAST,
            SOUTH,
            WEST,
            UP,
            DOWN
        }

        string m_name;
        public Sky(string name, string ext)
        {
            m_name = name;
            Texture2D.EnableMipMaping = false;
            Texture2D[] cubemap = new Texture2D[6];
            for (int i = 0; i < 6; i++)
            {
                cubemap[i] = TextureMgr.Load("Sky/" + name + "/" + i + ext);
                cubemap[i].Bind();
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
                cubemap[i].Unbind();
            }
            Texture2D.EnableMipMaping = true;

            Instance = this;
        }

        public override void Render()
        {
            Texture2D[] cubemap = new Texture2D[6];
            for (int i = 0; i < 6; i++)
                cubemap[i] = TextureMgr.Load("Sky/" + m_name + "/" + i + ".bmp");

            Gl.glDepthMask(Gl.GL_FALSE);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glPushMatrix();
            
            Gl.glTranslatef(Camera.Position.X,
                            Camera.Position.Y,
                            Camera.Position.Z);

            Gl.glScalef(5, 5, 5);
            

            cubemap[(int)Orient.NORTH].Bind();
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2i(0, 0); Gl.glVertex3i(-1, -1, 1);
            Gl.glTexCoord2i(0, 1); Gl.glVertex3i(-1, -1, -1);
            Gl.glTexCoord2i(1, 1); Gl.glVertex3i(1, -1, -1);
            Gl.glTexCoord2i(1, 0); Gl.glVertex3i(1, -1, 1);
            Gl.glEnd();
            cubemap[(int)Orient.NORTH].Unbind();

            cubemap[(int)Orient.EAST].Bind();
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2i(0, 0); Gl.glVertex3i(1, -1, 1);
            Gl.glTexCoord2i(0, 1); Gl.glVertex3i(1, -1, -1);
            Gl.glTexCoord2i(1, 1); Gl.glVertex3i(1, 1, -1);
            Gl.glTexCoord2i(1, 0); Gl.glVertex3i(1, 1, 1);
            Gl.glEnd();
            cubemap[(int)Orient.EAST].Unbind();

            cubemap[(int)Orient.SOUTH].Bind();
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2i(0, 0); Gl.glVertex3i(1, 1, 1);
            Gl.glTexCoord2i(0, 1); Gl.glVertex3i(1, 1, -1);
            Gl.glTexCoord2i(1, 1); Gl.glVertex3i(-1, 1, -1);
            Gl.glTexCoord2i(1, 0); Gl.glVertex3i(-1, 1, 1);
            Gl.glEnd();
            cubemap[(int)Orient.SOUTH].Unbind();

            cubemap[(int)Orient.WEST].Bind();
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2i(0, 0); Gl.glVertex3i(-1, 1, 1);
            Gl.glTexCoord2i(0, 1); Gl.glVertex3i(-1, 1, -1);
            Gl.glTexCoord2i(1, 1); Gl.glVertex3i(-1, -1, -1);
            Gl.glTexCoord2i(1, 0); Gl.glVertex3i(-1, -1, 1);
            Gl.glEnd();
            cubemap[(int)Orient.WEST].Unbind();

            cubemap[(int)Orient.UP].Bind();
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2i(0, 0); Gl.glVertex3i(1, -1, 1);
            Gl.glTexCoord2i(0, 1); Gl.glVertex3i(1, 1, 1);
            Gl.glTexCoord2i(1, 1); Gl.glVertex3i(-1, 1, 1);
            Gl.glTexCoord2i(1, 0); Gl.glVertex3i(-1, -1, 1);
            Gl.glEnd();
            cubemap[(int)Orient.UP].Unbind();

            cubemap[(int)Orient.DOWN].Bind();
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2i(0, 0); Gl.glVertex3i(1, 1, -1);
            Gl.glTexCoord2i(0, 1); Gl.glVertex3i(1, -1, -1);
            Gl.glTexCoord2i(1, 1); Gl.glVertex3i(-1, -1, -1);
            Gl.glTexCoord2i(1, 0); Gl.glVertex3i(-1, 1, -1);
            Gl.glEnd();
            cubemap[(int)Orient.DOWN].Unbind();

            Gl.glPopMatrix();
            Gl.glShadeModel(Gl.GL_FLAT);
            Gl.glDepthMask(Gl.GL_TRUE);
        }
    }
}
