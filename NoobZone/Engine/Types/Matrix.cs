using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    class Matrix
    {
        float[] mat;
        public Matrix()
        {
            mat = new float[16];
            mat[0] = 1;
            mat[5] = 1;
            mat[10] = 1;
            mat[15] = 1;
        }
        public Matrix(float[] tab)
        {
            mat = tab;
        }

        public float[] Mat
        {
            get { return mat; }
        }
        public float this[int i]
        {
            get { return mat[i]; }
        }
        public static Vector3 operator *(Vector3 v, Matrix m)
        {
            return new Vector3(v.X * m[0] + v.Y * m[4] + v.Z * m[8] + m[12],
                               v.X * m[1] + v.Y * m[5] + v.Z * m[9] + m[13],
                               v.X * m[2] + v.Y * m[6] + v.Z * m[10] + m[14]);
        }
        public static Vector3 operator *(Matrix m, Vector3 v)
        {
            return new Vector3(v.X * m[0] + v.Y * m[4] + v.Z * m[8] + m[12],
                               v.X * m[1] + v.Y * m[5] + v.Z * m[9] + m[13],
                               v.X * m[2] + v.Y * m[6] + v.Z * m[10] + m[14]);
        }
        public static Matrix GetGLModelView()
        {
            float[] tab = new float[16];
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, tab);
            return new Matrix(tab);
        }
    }
}
