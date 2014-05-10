using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    class Spline
    {
        List<Vector2> ctrlpoints;
        float[] dersec;

        public Spline(List<Vector2> controlpoints)
        {
            int nbcontrolpoints = controlpoints.Count;
            ctrlpoints = new List<Vector2>(controlpoints);

            int nbctrlpoints = controlpoints.Count;

	        //Calcul des dérivés seconde
	        dersec = new float[nbctrlpoints];

	        float yp0=(ctrlpoints[1].Y-ctrlpoints[0].Y)
                     /(ctrlpoints[1].X-ctrlpoints[0].X);

	        float ypnm1=(ctrlpoints[nbctrlpoints-1].Y-ctrlpoints[nbctrlpoints-2].Y)
                       /(ctrlpoints[nbctrlpoints-1].X-ctrlpoints[nbctrlpoints-2].X);
        	
	        calculderivsecondes(yp0,ypnm1);
        }

        public Vector2 GetPoint(float t)
        {
            Vector2 ret = new Vector2();
            ret.X = ctrlpoints[0].X + t * (ctrlpoints[ctrlpoints.Count - 1].X - ctrlpoints[0].X);

            int i = 1;
            while (ctrlpoints[i].X < ret.X)
                i++;

            ret.Y = S(i, ret.X);

            return ret;
        }
        float S(int i, float x)
        {
            float xi=ctrlpoints[i-1].X;
	        float xip1=ctrlpoints[i].X;

	        float A=(xip1-x)/(xip1-xi);
	        float B=(x-xi)/(xip1-xi);

	        float C=((A*A*A-A)*((xip1-xi)*(xip1-xi)))/6.0f;
	        float D=((B*B*B-B)*((xip1-xi)*(xip1-xi)))/6.0f;

        	
	        return A*ctrlpoints[i-1].Y+B*ctrlpoints[i].Y+C*dersec[i-1]+D*dersec[i];
        }
        void calculderivsecondes(float yp0, float ypnm1)
        {
	        int nbctrlpoints = ctrlpoints.Count;
	        int 	i,k;
	        float p,qn,sig,un;

	        float[] u = new float[nbctrlpoints];

	        //Empeche une pente initiale trop importante
	        if (yp0 > 1e30)
		        dersec[0]=u[0]=0.0f;
	        else
	        {
		        dersec[0] = -0.5f;
		        u[0]=(3.0f/(float)(ctrlpoints[1].X-ctrlpoints[0].X))*((float)(ctrlpoints[1].Y-ctrlpoints[0].Y)/(float)(ctrlpoints[1].X-ctrlpoints[0].X)-yp0);
	        }
	        for (i=1;i<nbctrlpoints-1;i++)
	        {
		        sig=(float)(ctrlpoints[i].X-ctrlpoints[i-1].X)/(float)(ctrlpoints[i+1].X-ctrlpoints[i-1].X);
		        p=sig*dersec[i-1]+2.0f;
		        dersec[i]=(sig-1.0f)/p;
		        u[i]=(float)(ctrlpoints[i+1].Y-ctrlpoints[i].Y)/(float)(ctrlpoints[i+1].X-ctrlpoints[i].X) - (float)(ctrlpoints[i].Y-ctrlpoints[i-1].Y)/(float)(ctrlpoints[i].X-ctrlpoints[i-1].X);
		        u[i]=(6.0f*u[i]/(float)(ctrlpoints[i+1].X-ctrlpoints[i-1].X)-sig*u[i-1])/p;
	        }

	        //Empeche une pente finale trop importante
	        if (ypnm1 > 0.99e30)
		        qn=un=0.0f;
	        else {
		        qn=0.5f;
		        un=(3.0f/(float)(ctrlpoints[nbctrlpoints-1].X-ctrlpoints[nbctrlpoints-2].X))*(ypnm1-(float)(ctrlpoints[nbctrlpoints-1].Y-ctrlpoints[nbctrlpoints-2].Y)/(float)(ctrlpoints[nbctrlpoints-1].X-ctrlpoints[nbctrlpoints-2].X));
	        }
	        dersec[nbctrlpoints-1]=(un-qn*u[nbctrlpoints-2])/(qn*dersec[nbctrlpoints-2]+1.0f);
	        for (k=nbctrlpoints-2;k>=0;k--)		dersec[k]=dersec[k]*dersec[k+1]+u[k];
        }
    }
}
