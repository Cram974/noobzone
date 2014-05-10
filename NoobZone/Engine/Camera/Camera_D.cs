using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Tao.OpenGl;

namespace NoobZone
{
    class Camera_D : IRenderable
    {
        Spline3D m_eye;
        Spline3D m_to;
        float m_speed;
        float m_time;

        public Camera_D(string name)
        {
            m_eye = new Spline3D();
            m_to = new Spline3D();

            FileStream fs = new FileStream(name, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(' ');
                if (line.Contains("SPEED"))
                {
                    m_speed = floatFromString(data[1]);
                }
                if (line.Contains("EYE"))
                {
                    m_eye.AddPoint(new Vector3(floatFromString(data[1]),
                                             floatFromString(data[2]),
                                             floatFromString(data[3])));
                }
                if (line.Contains("LOOKAT"))
                {
                    m_to.AddPoint(new Vector3(floatFromString(data[1]),
                                            floatFromString(data[2]),
                                            floatFromString(data[3])));
                }
            }
            sr.Close();
            fs.Close();

            m_eye.BuildSplines();
            m_to.BuildSplines();
        }
        public void Save(string name)
        {
            FileStream fs = new FileStream(name, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("SPEED " + m_speed);
            for (int i = 0; i < m_eye.ControlPoints.Count; i++)
            {
                string eyeString = "EYE " + m_eye.ControlPoints[i].X + " "
                                          + m_eye.ControlPoints[i].Y + " " 
                                          + m_eye.ControlPoints[i].Z;
                string lookAtString = "LOOKAT " + m_to.ControlPoints[i].X + " "
                                                + m_to.ControlPoints[i].Y + " "
                                                + m_to.ControlPoints[i].Z;
                sw.WriteLine(eyeString);
                sw.WriteLine(lookAtString);
                sw.WriteLine();
            }

            sw.Close();
            fs.Close();
        }
        float floatFromString(string s)
        {
            return Convert.ToSingle(s.Replace('.', ','));
        }

        public void Update()
        {
            m_time += World.dt * m_speed;
            if (m_time > 1f)
            {
                m_time = 0;
            }
            Camera.Position = m_eye.GetPoint(m_time);
            Camera.Forward = m_to.GetPoint(m_time) - m_eye.GetPoint(m_time);
        }

        public override void Render()
        {
            //Rendering Splines
        }
    }
}
