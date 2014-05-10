using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    class Collision
    {
        public static bool IsColliding(AABB box1, AABB box2)
        {
            if (box1.Max.X < box2.Min.X || box1.Min.X > box2.Max.X)
                return false;
            if (box1.Max.Y < box2.Min.Y || box2.Max.Y < box1.Min.Y)
                return false;
            if (box1.Max.Z < box2.Min.Z || box2.Max.Z < box1.Min.Z)
                return false;

            return true;
        }
        public static bool IsColliding(Vector3[] poly, Vector3[] line, int vertcount)
        {
            return IntersectedPolygon(poly, line, vertcount);
        }
        static Vector3 IntersectionPoint(Vector3 vNormal, Vector3[] vLine, double distance)
        {
	        Vector3 vPoint = new Vector3();
            Vector3 vLineDir = new Vector3();
	        double Numerator = 0.0, Denominator = 0.0, dist = 0.0;


	        vLineDir = (vLine[1] - vLine[0]);
	        vLineDir = vLineDir.Normalized;

	        Numerator = - (vNormal.X * vLine[0].X +	
				           vNormal.Y * vLine[0].Y +
				           vNormal.Z * vLine[0].Z + distance);

	        Denominator = vNormal.Dot(vLineDir);

	        if( Denominator == 0.0)
		        return vLine[0];


	        dist = Numerator / Denominator;			

	        vPoint.X = (float)(vLine[0].X + (vLineDir.X * dist));
	        vPoint.Y = (float)(vLine[0].Y + (vLineDir.Y * dist));
	        vPoint.Z = (float)(vLine[0].Z + (vLineDir.Z * dist));

	        return vPoint;
        }
        static bool InsidePolygon(Vector3 vIntersection, Vector3[] Poly, long verticeCount)
        {
	        const double MATCH_FACTOR = 0.9999;
	        double Angle = 0.0;
	        Vector3 vA, vB;

	        for (int i = 0; i < verticeCount; i++)
	        {	
		        vA = Poly[i] - vIntersection;
		        vB = Poly[(i + 1) % verticeCount] - vIntersection;
        												
		        Angle += AngleBetweenVectors(vA, vB);
	        }
        												
	        if(Angle >= (MATCH_FACTOR * (2.0 * Math.PI)) )
		        return true;
        		
	        return false;
        }
        static bool IntersectedPolygon(Vector3[] vPoly, Vector3[] vLine, int verticeCount)
        {
	        Vector3 vNormal = new Vector3();
	        float originDistance = 0;

	        if(!IntersectedPlane(vPoly, vLine,   ref vNormal, ref  originDistance))
		        return false;

	        Vector3 vIntersection = IntersectionPoint(vNormal, vLine, originDistance);

	        if(InsidePolygon(vIntersection, vPoly, verticeCount))
		        return true;

	        return false;
        }
        static bool IntersectedPlane(Vector3[] vPoly, Vector3[] vLine, ref Vector3 vNormal, ref float originDistance)
        {
            float distance1=0, distance2=0;
            vNormal = Normal(vPoly);
            originDistance = PlaneDistance(vNormal, vPoly[0]);

            distance1 = ((vNormal.X * vLine[0].X)  +				
	                     (vNormal.Y * vLine[0].Y)  +					
			             (vNormal.Z * vLine[0].Z)) + originDistance;

            distance2 = ((vNormal.X * vLine[1].X) +				
                         (vNormal.Y * vLine[1].Y) +				
                         (vNormal.Z * vLine[1].Z)) + originDistance;

            if(distance1 * distance2 >= 0)
               return false;
        					
            return true;
        }
        static float PlaneDistance(Vector3 Normal, Vector3 Point)
        {
            float distance = 0;
            distance = -((Normal.X * Point.X) + (Normal.Y * Point.Y) + (Normal.Z * Point.Z));

            return distance;
        }
        static Vector3 Normal(Vector3[] vTriangle)
        {
	        Vector3 vVector1 = vTriangle[2] - vTriangle[0];
	        Vector3 vVector2 = vTriangle[1] - vTriangle[0];

	        Vector3 vNormal = vVector1.Cross(vVector2);

	        vNormal = vNormal.Normalized;
	        return vNormal;
        }
        static double AngleBetweenVectors(Vector3 Vector1, Vector3 Vector2)
        {
            float dotProduct = Vector1.Dot(Vector2);
            float vectorsMagnitude = Vector1.Length * Vector2.Length;
            double angle = Math.Acos(dotProduct / vectorsMagnitude);

            if (double.IsNaN(angle))
                return 0;
            return (angle);
        }
    }
}
