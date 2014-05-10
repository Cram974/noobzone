using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    class Camera_FPS
    {
        protected float f_teta;
        protected float f_phi;
        protected float f_speed;
        protected float f_sens;
        protected Vector3 vVelocity;

        public float Phi
        {
            get { return f_phi; }
        }
        public float Teta
        {
            get { return f_teta; }
        }

        public Camera_FPS()
        {
            f_teta = 0;
            f_phi = 0;
            f_speed = 300f;
            f_sens = 0.3f;
            vVelocity = new Vector3();
        }

        public virtual void Update()
        {
            Camera.Forward = Camera.Forward.Normalized;
            Vector3 vCross = Camera.Forward.Cross(World.Up);

            // Normalize the strafe vector
            Camera.Left = vCross.Normalized;

            // Move the camera's view by the mouse
            SetViewByMouse();

            // This checks to see if the keyboard was pressed
            CheckForMovement();
        }
        void SetViewByMouse()
        {
            if (EventMgr.Mouse.Moved)
            {
                f_teta -= (float)EventMgr.Mouse.Xrel * f_sens;
                f_teta %= 360;
                f_phi -= (float)EventMgr.Mouse.Yrel * f_sens;
                if (f_phi > 89)
                    f_phi = 89;
                if (f_phi < -89)
                    f_phi = -89;

                float r = (float)Math.Cos(f_phi * Math.PI / 180);

                Camera.Forward = new Vector3(r * (float)Math.Cos(f_teta * Math.PI / 180),
                                             r * (float)Math.Sin(f_teta * Math.PI / 180),
                                                 (float)Math.Sin(f_phi * Math.PI / 180));
            }

        }
        public void UpdateAngles()
        {
                float r = (float)Math.Cos(f_phi * Math.PI / 180);

                Camera.Forward = new Vector3(r * (float)Math.Cos(f_teta * Math.PI / 180),
                                             r * (float)Math.Sin(f_teta * Math.PI / 180),
                                                 (float)Math.Sin(f_phi * Math.PI / 180));
        }
        void RotateForward(float angle, float x, float y, float z)
        {
	        // Get the view vector (The direction we are facing)
	        Vector3 vView = Camera.Forward;		

	        // Calculate the sine and cosine of the angle once
	        float cosTheta = (float)Math.Cos(angle);
	        float sinTheta = (float)Math.Sin(angle);

            Vector3 vNewView = new Vector3();
	        // Find the new x position for the new rotated point
	        vNewView.X  = (cosTheta + (1 - cosTheta) * x * x)		* vView.X;
	        vNewView.X += ((1 - cosTheta) * x * y - z * sinTheta)	* vView.Y;
	        vNewView.X += ((1 - cosTheta) * x * z + y * sinTheta)	* vView.Z;

	        // Find the new y position for the new rotated point
	        vNewView.Y  = ((1 - cosTheta) * x * y + z * sinTheta)	* vView.X;
	        vNewView.Y += (cosTheta + (1 - cosTheta) * y * y)		* vView.Y;
	        vNewView.Y += ((1 - cosTheta) * y * z - x * sinTheta)	* vView.Z;

	        // Find the new z position for the new rotated point
	        vNewView.Z  = ((1 - cosTheta) * x * z - y * sinTheta)	* vView.X;
	        vNewView.Z += ((1 - cosTheta) * y * z + x * sinTheta)	* vView.Y;
	        vNewView.Z += (cosTheta + (1 - cosTheta) * z * z)		* vView.Z;

	        // Now we just add the newly rotated vector to our position to set
	        // our new rotated view of our camera.
            Camera.Forward = vNewView;
        }
        void CheckForMovement()
        {
            Vector3 trans = new Vector3();

            if (EventMgr.Keyboard['w'])
            {
                trans += Camera.Forward;
            }
            if (EventMgr.Keyboard['s'])
            {
                trans -= Camera.Forward;
            }
            if (EventMgr.Keyboard['a'])
            {
                trans -= Camera.Left;
            }
            if (EventMgr.Keyboard['d'])
            {
                trans += Camera.Left;
            }

            trans = trans.Normalized;

            vVelocity -= World.g * World.dt * World.Up;
            Vector3 newPos = Camera.Position + trans * f_speed * World.dt;

            Vector3 transX = newPos - Camera.Position;
            transX.Z = 0;
            transX.Y = 0;

            Vector3 transZ = newPos - Camera.Position;
            transZ.X = 0;
            transZ.Y = 0;

            Vector3 transY = newPos - Camera.Position;
            transY.Z = 0;
            transY.X = 0;


            Vector3 stepup = new Vector3();

            AABB cambox = new AABB();

            cambox.AddVertex(new Vector3(-10, -10, -60));
            cambox.AddVertex(new Vector3(10, 10, 20));

            Q3CollisionData coldata = World.Map.Level.TraceBox(Camera.Position, Camera.Position + transX, cambox);
            if (coldata.collisionPoint != coldata.endPosition)
            {
                // try to step
                Vector3 currstep = new Vector3();
                for (float step = 1f; coldata.collisionPoint != coldata.endPosition && step < 10; step++)
                {
                    currstep = new Vector3(0, 0, step);
                    coldata = World.Map.Level.TraceBox(Camera.Position + currstep, Camera.Position + currstep + transX, cambox);
                }
                if (coldata.collisionPoint != coldata.endPosition)
                    transX.X = 0;
                else
                    stepup = currstep;
            }

            coldata = World.Map.Level.TraceBox(Camera.Position, Camera.Position + transY, cambox);
            if (coldata.collisionPoint != coldata.endPosition)
            {
                Vector3 currstep = new Vector3();
                for (float step = 1f; coldata.collisionPoint != coldata.endPosition && step < 10; step++)
                {
                    currstep = new Vector3(0, 0, step);
                    coldata = World.Map.Level.TraceBox(Camera.Position + currstep, Camera.Position + currstep + transY, cambox);
                }
                if (coldata.collisionPoint != coldata.endPosition)
                    transY.Y = 0;
                else
                    stepup.Z = Math.Max(currstep.Z,stepup.Z);
            }

            Camera.Position += transX + transY + stepup;

            coldata = World.Map.Level.TraceBox(Camera.Position, Camera.Position + vVelocity, cambox);
            if (coldata.collisionPoint != coldata.endPosition)
            {
                vVelocity = new Vector3();
                if (EventMgr.Keyboard[' '])
                {
                    vVelocity.Z = World.JumpAcc;

                    coldata = World.Map.Level.TraceBox(Camera.Position, Camera.Position + vVelocity, cambox);
                    if (coldata.collisionPoint != coldata.endPosition)
                        vVelocity.Z = 0;
                }
                else if(transX.X != 0 || transY.Y != 0)
                {
                    vVelocity.Z = 1f;

                    coldata = World.Map.Level.TraceBox(Camera.Position, Camera.Position + vVelocity, cambox);
                    if (coldata.collisionPoint != coldata.endPosition)
                        vVelocity.Z = 0;
                }
            }

            Camera.Position += vVelocity;

        }
    }
}
