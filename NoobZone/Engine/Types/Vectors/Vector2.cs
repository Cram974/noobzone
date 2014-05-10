using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    class Vector2
    {
        //Attribut
        float[] v;

        //Constructeurs
        public Vector2()
        {
            v = new float[2];
            v[0] = 0f;
            v[1] = 0f;
        }
        public Vector2(float x, float y)
        {
            v = new float[2];
            v[0] = x;
            v[1] = y;
        }
        public Vector2(Vector2 w)
        {
            v = new float[2];
            v[0] = w.X;
            v[1] = w.Y;
        }
        public Vector2(float[] v2)
        {
            if (v2.Length == 2)
                v = v2;
        }

        //Accesseurs
        public float X
        {
            get { return v[0]; }
            set { v[0] = value; }
        }
        public float Y
        {
            get { return v[1]; }
            set { v[1] = value; }
        }
        public float[] V
        {
            get { return v; }
            set { v = value; }
        }

        public float Length
        {
            get { return (float)Math.Sqrt(v[0] * v[0] + v[1] * v[1]); }
        }
        public Vector2 Normalized
        {

            get
            {
                if (Length != 0)
                {
                    return new Vector2(X / Length, Y / Length);
                }
                else
                    return new Vector2();
            }
        }


        //Operateurs
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator *(Vector2 v, float f)
        {
            return new Vector2(v.X * f, v.Y * f);
        }
        public static Vector2 operator *(float f, Vector2 v)
        {
            return new Vector2(v.X * f, v.Y * f);
        }
        public static Vector2 operator /(Vector2 v, float f)
        {
            return new Vector2(v.X / f, v.Y / f);
        }

        //Methodes
        public float dotProduct(Vector2 v)
        {
            return (v.X * this.X + v.Y * this.Y);
        }

    }
}
