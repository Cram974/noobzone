using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace NoobZone
{
    class Color
    {
        float[] v_rgba;
        public Color()
        {
            v_rgba = new float[4];
            for (int i = 0; i < 4; i++)
                v_rgba[i] = 1;
        }
        public Color(float[] rgba)
        {
            v_rgba = rgba;
        }
        public Color(float r, float g, float b, float a)
        {
            v_rgba = new float[4];
            v_rgba[0] = r;
            v_rgba[1] = g;
            v_rgba[2] = b;
            v_rgba[3] = a;
        }
        public Color(float r, float g, float b)
        {
            v_rgba = new float[4];
            v_rgba[0] = r;
            v_rgba[1] = g;
            v_rgba[2] = b;
            v_rgba[3] = 0f;
        }
        public void normalize()
        {
            for(int i = 0;i<4;i++)
            {
                if (v_rgba[i] != 0)
                    v_rgba[i] /= 255;
            }
        }
        public float R
        {
            get { return v_rgba[0]; }
            set { v_rgba[0] = value; }
        }
        public float G
        {
            get { return v_rgba[1]; }
            set { v_rgba[1] = value; }
        }
        public float B
        {
            get { return v_rgba[2]; }
            set { v_rgba[2] = value; }
        }
        public float A
        {
            get { return v_rgba[3]; }
            set { v_rgba[3] = value; }
        }
        public float[] V
        {
            get { return v_rgba; }
            set { v_rgba = value; }
        }
     
    }
}
