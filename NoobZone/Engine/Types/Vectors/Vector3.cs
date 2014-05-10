using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    public class Vector3
    {
        //Attribut
        float[] v;

        //Constructeurs
        public Vector3()
        {
            v = new float[3];
            v[0] = 0f;
            v[1] = 0f;
            v[2] = 0f;
        }
        public Vector3(float x, float y, float z)
        {
            v = new float[3];
            v[0] = x;
            v[1] = y;
            v[2] = z;
        }
        public Vector3(Vector3 w)
        {
            v = new float[3];
            v[0] = w.X;
            v[1] = w.Y;
            v[2] = w.Z;
        }
        public Vector3(float[] v3)
        {
            if (v3.Length == 3)
                v = v3;
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
        public float Z
        {
            get { return v[2]; }
            set { v[2] = value; }
        }
        public float[] V
        {
            get { return v; }
            set { v = value; }
        }

        public float Length
        {
            get { return (float)Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]); }
        }
        public float LengthSquared
        {
            get { return v[0] * v[0] + v[1] * v[1] + v[2] * v[2]; }
        }
        public Vector3 Normalized
        {

            get
            {
                if (Length != 0)
                {
                    return new Vector3(X / Length, Y / Length, Z / Length);
                }
                else
                    return new Vector3();
            }
        }


        //Operateurs
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator *(Vector3 v, float f)
        {
            return new Vector3(v.X * f, v.Y * f, v.Z * f);
        }
        public static Vector3 operator *(float f, Vector3 v)
        {
            return new Vector3(v.X * f, v.Y * f, v.Z * f);
        }
        public static Vector3 operator /(Vector3 v, float f)
        {
            return new Vector3(v.X / f, v.Y / f, v.Z / f);
        }

        //Methodes
        public Vector3 Cross(Vector3 v)
        {
            Vector3 w = new Vector3();
            w.X = this.Y * v.Z - this.Z * v.Y;
            w.Y = this.Z * v.X - this.X * v.Z;
            w.Z = this.X * v.Y - this.Y * v.X;
            return w;
        }
        public float Dot(Vector3 v)
        {
            return (v.X * this.X + v.Y * this.Y + v.Z * this.Z);
        }

    }
}
