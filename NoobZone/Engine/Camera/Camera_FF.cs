using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    class Camera_FF
    {
        float f_teta;
        float f_phi;
        float f_speed;
        float f_sens;

        public Camera_FF()
        {
            f_teta = 0;
            f_phi = 0;
            f_speed = 30f*(1/0.1f);
            f_sens = 0.3f;
        }

        public void Update()
        {
            Vector3 translate = new Vector3();

            updateAngles();

            if (EventMgr.Keyboard['w'])
                translate += Camera.Forward;
            if (EventMgr.Keyboard['s'])
                translate -= Camera.Forward;
            if (EventMgr.Keyboard['a'])
                translate += Camera.Left;
            if (EventMgr.Keyboard['d'])
                translate -= Camera.Left;

            if (translate.Length != 0)
            {
                translate = translate.Normalized;
                Camera.Position = new Vector3(Camera.Position + translate * (f_speed * World.dt));
            }
        }
        void updateAngles()
        {
            //SetAngles
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
    }
}
