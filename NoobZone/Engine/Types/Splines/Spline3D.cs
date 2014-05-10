using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    class Spline3D
    {
        Spline m_pSplineX;
        Spline m_pSplineY;
        Spline m_pSplineZ;

        List<Vector3> m_tCtrlPts;
        bool m_bClosed;
        bool m_bBuilt;

        public List<Vector3> ControlPoints
        {
            get { return m_tCtrlPts; }
        }
        public bool IsClosed
        {
            get { return m_bClosed; }
        }
        public bool IsValid
        {
            get { return m_bBuilt && m_tCtrlPts.Count > 1; }
        }

        public Spline3D()
        {
            m_bClosed = false;
            m_bBuilt = false;
            m_tCtrlPts = new List<Vector3>();
        }

        public Vector3 GetPoint(float t)
        {
            if (m_bBuilt)
            {
                return new Vector3(m_pSplineX.GetPoint(t).Y,
                                   m_pSplineY.GetPoint(t).Y,
                                   m_pSplineZ.GetPoint(t).Y);
            }
            else
            {
                return new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        public bool AddPoint(Vector3 pt)
        {
            m_bBuilt = false;
            m_tCtrlPts.Add(pt);
            return true;
        }
        public bool DeleteLastPoint()
        {
            m_bBuilt = false;
            if (m_tCtrlPts.Count > 0)
            {
                m_tCtrlPts.RemoveAt(m_tCtrlPts.Count - 1);
            }
            return true;
        }
        public bool Clear()
        {
            m_bBuilt = false;
            m_tCtrlPts = new List<Vector3>();
            return true;
        }
        public void BuildSplines()
        {
            m_bBuilt = false;

            if (m_tCtrlPts.Count > 1)
            {
                List<Vector2> tab1 = new List<Vector2>();
                List<Vector2> tab2 = new List<Vector2>();
                List<Vector2> tab3 = new List<Vector2>();

                for (int t = 0; t < m_tCtrlPts.Count; t++)
                {
                    tab1.Add(new Vector2(t, m_tCtrlPts[t].X));
                    tab2.Add(new Vector2(t, m_tCtrlPts[t].Y));
                    tab3.Add(new Vector2(t, m_tCtrlPts[t].Z));
                }

                m_pSplineX = new Spline(tab1);
                m_pSplineY = new Spline(tab2);
                m_pSplineZ = new Spline(tab3);

                m_bBuilt = true;
            }
        }
        public bool Close()
        {
            m_bBuilt = false;
            m_bClosed = true;

            if (m_tCtrlPts.Count > 1)
            {
                AddPoint(m_tCtrlPts[0]);
                return true;
            }

            m_bClosed = false;
            return false;
        }
    }
}
