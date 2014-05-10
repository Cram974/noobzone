using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    public class AABB
    {
        Vector3 v_min;
        Vector3 v_max;

        public void Render()
        {
            Gl.glColor3f(1f, 0f, 0f);

            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(v_max.X, v_max.Y, v_max.Z);
            Gl.glVertex3f(v_max.X, v_min.Y, v_max.Z);
            Gl.glVertex3f(v_min.X, v_min.Y, v_max.Z);
            Gl.glVertex3f(v_min.X, v_max.Y, v_max.Z);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(v_min.X, v_min.Y, v_min.Z);
            Gl.glVertex3f(v_min.X, v_max.Y, v_min.Z);
            Gl.glVertex3f(v_max.X, v_max.Y, v_min.Z);
            Gl.glVertex3f(v_max.X, v_min.Y, v_min.Z);
            Gl.glEnd();

            

            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(v_max.X, v_max.Y, v_min.Z);
            Gl.glVertex3f(v_max.X, v_max.Y, v_max.Z);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(v_min.X, v_max.Y, v_min.Z);
            Gl.glVertex3f(v_min.X, v_max.Y, v_max.Z);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(v_max.X, v_min.Y, v_min.Z);
            Gl.glVertex3f(v_max.X, v_min.Y, v_max.Z);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(v_min.X, v_min.Y, v_min.Z);
            Gl.glVertex3f(v_min.X, v_min.Y, v_max.Z);
            Gl.glEnd();

            Gl.glColor3f(1f, 1f, 1f);
        }

        public Vector3 Min
        {
            get { return v_min; }
            set { v_min = value; }
        }
        public Vector3 Max
        {
            get { return v_max; }
            set { v_max = value; }
        }

        public AABB()
        {
            v_min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            v_max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        }
        public AABB(float[] tab)
        {
            v_min = new Vector3(tab[0], tab[1], tab[2]);
            v_max = new Vector3(tab[3], tab[4], tab[5]);
        }
        public AABB(int[] tab)
        {
            v_min = new Vector3((float)tab[0], (float)tab[1], (float)tab[2]);
            v_max = new Vector3((float)tab[3], (float)tab[4], (float)tab[5]);
        }
        public AABB(Vector3 min, Vector3 max)
        {
            v_min = min;
            v_max = max;
        }

        public void AddVertex(Vector3 v)
        {
            if (v.X < v_min.X)
                v_min.X = v.X;
            if (v.Y < v_min.Y)
                v_min.Y = v.Y;
            if (v.Z < v_min.Z)
                v_min.Z = v.Z;

            if (v.X > v_max.X)
                v_max.X = v.X;
            if (v.Y > v_max.Y)
                v_max.Y = v.Y;
            if (v.Z > v_max.Z)
                v_max.Z = v.Z;
        }
    }
}
