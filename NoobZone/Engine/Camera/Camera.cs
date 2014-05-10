using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    class Camera
    {
        static Vector3 v_position;
        static Vector3 v_forward;
        static Vector3 v_left;

        static float f_fovy;
        static float f_near;
        static float f_far;

        static bool b_init = false;

        static Type t_cameratype;
        static Camera_FF c_freefly;
        static Camera_D c_driven;
        static Camera_FPS c_fps;
        static Frustrum f_frustrum;

        public enum Type
        {
            FreeFly,
            Driven,
            FPS
        }
        public static Type CameraType
        {
            get { return t_cameratype; }
        }
        public static Vector3 Position
        {
            get { return v_position; }
            set { v_position = value; }
        }
        public static Vector3 Forward
        {
            get { return v_forward; }
            set { v_forward = value; }
        }
        public static Vector3 Left
        {
            get { return v_left; }
            set { v_left = value; }
        }
        public static float FoVy
        {
            get { return f_fovy; }
        }
        public static float Near
        {
            get { return f_near; }
            set { f_near = value; }
        }
        public static float Far
        {
            get { return f_far; }
            set { f_far = value; } 
        }
        public static bool IsInit
        {
            get { return b_init; }
        }
        public static Camera_D DrivenCam
        {
            get { return c_driven; }
            set { c_driven = value; }
        }
        public static Camera_FPS FPSCam
        {
            get { return c_fps; }
            set { c_fps = value; }
        }
        public static Frustrum Frustrum
        {
            get { return f_frustrum; }
        }
        
        public static void Init()
        {
            if (!IsInit)
            {
                v_position = new Vector3();
                v_forward = new Vector3(0, 1, 0);
                v_left = new Vector3();
                f_fovy = 70f;
                f_near = 1f;
                f_far = 10000f;
                c_freefly = new Camera_FF();
                f_frustrum = new Frustrum();
                b_init = true;
                
                Update();
            }
        }

        public static void SetFreeFly()
        {
            t_cameratype = Type.FreeFly;
        }
        public static void SetFPS()
        {
            t_cameratype = Type.FPS;
        }
        public static void SetDriven()
        {
            t_cameratype = Type.Driven;
            
        }

        public static void Update()
        {
            switch(t_cameratype)
            {
                case Type.FreeFly:
                    c_freefly.Update();
                    break;
                case Type.Driven:
                    c_driven.Update();
                    break;
                case Type.FPS:
                    c_fps.Update();
                    break;
            }  
            v_left = World.Up.Cross(v_forward);

            v_forward = v_forward.Normalized;
            v_left = v_left.Normalized;

            f_frustrum.UpdateFrustrum();
        }
        public static void LookAt()
        {
            Vector3 to = v_forward + v_position;

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Glu.gluPerspective(f_fovy, (double)VarMgr.Getf("WindowRatio"), f_near, f_far);
            Glu.gluLookAt(v_position.X, v_position.Y, v_position.Z, to.X, to.Y, to.Z, World.Up.X, World.Up.Y, World.Up.Z);
        }
    }
}
