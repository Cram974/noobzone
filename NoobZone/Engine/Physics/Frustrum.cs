using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    public class Frustrum
    {
        public enum PlaneName
        {
            Near = 0, Far, Right, Left, Top, Bottom
        }

        public class Plane
        {
            public float A = 0.0f;
            public float B = 0.0f;
            public float C = 0.0f;
            public float D = 0.0f;

            public Plane()
            {
                //Do nothing
            }

            public Plane(float A, float B, float C, float D)
            {
                //Copy values
                this.A = A;
                this.B = B;
                this.C = C;
                this.D = D;
            }

            public void Normalize()
            {
                //Calculate magnitude
                float Mag = (float)Math.Sqrt((A * A) + (B * B) + (C * C));

                //Normalize
                A /= Mag;
                B /= Mag;
                C /= Mag;
                D /= Mag;
            }
        }

        public static readonly int NumPlanes = 6;

        public Plane[] Planes = new Plane[NumPlanes];

        public Frustrum()
        {
            //Allocate planes
            for (int i = 0; i < NumPlanes; i++)
            {
                Planes[i] = new Plane();
            }
        }

        public void UpdateFrustrum()
        {
            float[] Projection = new float[16];
            float[] ModelView = new float[16];
            float[] Clip = new float[16];

            //Load matrices
            Gl.glGetFloatv(Gl.GL_PROJECTION_MATRIX, Projection);
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, ModelView);

            //Multiply matrices
            Clip[0] = ModelView[0] * Projection[0] + ModelView[1] * Projection[4] + ModelView[2] * Projection[8] + ModelView[3] * Projection[12];
            Clip[1] = ModelView[0] * Projection[1] + ModelView[1] * Projection[5] + ModelView[2] * Projection[9] + ModelView[3] * Projection[13];
            Clip[2] = ModelView[0] * Projection[2] + ModelView[1] * Projection[6] + ModelView[2] * Projection[10] + ModelView[3] * Projection[14];
            Clip[3] = ModelView[0] * Projection[3] + ModelView[1] * Projection[7] + ModelView[2] * Projection[11] + ModelView[3] * Projection[15];

            Clip[4] = ModelView[4] * Projection[0] + ModelView[5] * Projection[4] + ModelView[6] * Projection[8] + ModelView[7] * Projection[12];
            Clip[5] = ModelView[4] * Projection[1] + ModelView[5] * Projection[5] + ModelView[6] * Projection[9] + ModelView[7] * Projection[13];
            Clip[6] = ModelView[4] * Projection[2] + ModelView[5] * Projection[6] + ModelView[6] * Projection[10] + ModelView[7] * Projection[14];
            Clip[7] = ModelView[4] * Projection[3] + ModelView[5] * Projection[7] + ModelView[6] * Projection[11] + ModelView[7] * Projection[15];

            Clip[8] = ModelView[8] * Projection[0] + ModelView[9] * Projection[4] + ModelView[10] * Projection[8] + ModelView[11] * Projection[12];
            Clip[9] = ModelView[8] * Projection[1] + ModelView[9] * Projection[5] + ModelView[10] * Projection[9] + ModelView[11] * Projection[13];
            Clip[10] = ModelView[8] * Projection[2] + ModelView[9] * Projection[6] + ModelView[10] * Projection[10] + ModelView[11] * Projection[14];
            Clip[11] = ModelView[8] * Projection[3] + ModelView[9] * Projection[7] + ModelView[10] * Projection[11] + ModelView[11] * Projection[15];

            Clip[12] = ModelView[12] * Projection[0] + ModelView[13] * Projection[4] + ModelView[14] * Projection[8] + ModelView[15] * Projection[12];
            Clip[13] = ModelView[12] * Projection[1] + ModelView[13] * Projection[5] + ModelView[14] * Projection[9] + ModelView[15] * Projection[13];
            Clip[14] = ModelView[12] * Projection[2] + ModelView[13] * Projection[6] + ModelView[14] * Projection[10] + ModelView[15] * Projection[14];
            Clip[15] = ModelView[12] * Projection[3] + ModelView[13] * Projection[7] + ModelView[14] * Projection[11] + ModelView[15] * Projection[15];

            //Extract right plane information
            Planes[(int)PlaneName.Right].A = Clip[3] - Clip[0];
            Planes[(int)PlaneName.Right].B = Clip[7] - Clip[4];
            Planes[(int)PlaneName.Right].C = Clip[11] - Clip[8];
            Planes[(int)PlaneName.Right].D = Clip[15] - Clip[12];

            //Extract left plane information
            Planes[(int)PlaneName.Left].A = Clip[3] + Clip[0];
            Planes[(int)PlaneName.Left].B = Clip[7] + Clip[4];
            Planes[(int)PlaneName.Left].C = Clip[11] + Clip[8];
            Planes[(int)PlaneName.Left].D = Clip[15] + Clip[12];

            //Extract bottom plane information
            Planes[(int)PlaneName.Bottom].A = Clip[3] + Clip[1];
            Planes[(int)PlaneName.Bottom].B = Clip[7] + Clip[5];
            Planes[(int)PlaneName.Bottom].C = Clip[11] + Clip[9];
            Planes[(int)PlaneName.Bottom].D = Clip[15] + Clip[13];

            //Extract top plane information
            Planes[(int)PlaneName.Top].A = Clip[3] - Clip[1];
            Planes[(int)PlaneName.Top].B = Clip[7] - Clip[5];
            Planes[(int)PlaneName.Top].C = Clip[11] - Clip[9];
            Planes[(int)PlaneName.Top].D = Clip[15] - Clip[13];

            //Extract far plane information
            Planes[(int)PlaneName.Far].A = Clip[3] - Clip[2];
            Planes[(int)PlaneName.Far].B = Clip[7] - Clip[6];
            Planes[(int)PlaneName.Far].C = Clip[11] - Clip[10];
            Planes[(int)PlaneName.Far].D = Clip[15] - Clip[14];

            //Extract near plane information
            Planes[(int)PlaneName.Near].A = Clip[3] + Clip[2];
            Planes[(int)PlaneName.Near].B = Clip[7] + Clip[6];
            Planes[(int)PlaneName.Near].C = Clip[11] + Clip[10];
            Planes[(int)PlaneName.Near].D = Clip[15] + Clip[14];

            //Normalize planes
            for (int i = 0; i < NumPlanes; i++)
            {
                Planes[i].Normalize();
            }
        }

        bool BoxInFrustrum(float X, float Y, float Z, float X2, float Y2, float Z2)
        {
            this.UpdateFrustrum();
            bool ReturnStatus = true;

            for (int i = 0; i < 6; i++)
            {
                Plane CP = Planes[i];

                if (((CP.A * X) + (CP.B * Y) + (CP.C * Z) + CP.D) > 0) continue;
                if (((CP.A * X2) + (CP.B * Y) + (CP.C * Z) + CP.D) > 0) continue;
                if (((CP.A * X) + (CP.B * Y2) + (CP.C * Z) + CP.D) > 0) continue;
                if (((CP.A * X2) + (CP.B * Y2) + (CP.C * Z) + CP.D) > 0) continue;
                if (((CP.A * X) + (CP.B * Y) + (CP.C * Z2) + CP.D) > 0) continue;
                if (((CP.A * X2) + (CP.B * Y) + (CP.C * Z2) + CP.D) > 0) continue;
                if (((CP.A * X) + (CP.B * Y2) + (CP.C * Z2) + CP.D) > 0) continue;
                if (((CP.A * X2) + (CP.B * Y2) + (CP.C * Z2) + CP.D) > 0) continue;

                ReturnStatus = false;
            }

            return (ReturnStatus);
        }
        
        public bool BoxInFrustrum(AABB Box)
        {
            return BoxInFrustrum(Box.Min.X, Box.Min.Y, Box.Min.Z, Box.Max.X, Box.Max.Y, Box.Max.Z);
        }
    }
}
