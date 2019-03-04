using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonogameRasterizer.Extensions;

namespace MonogameRasterizer.Utils
{
	public static class ClippingUtils
	{
		[Flags]
		public enum eCohenSutherlandCode
		{
			Inside = 0,
			Left = 1,
			Right = 2,
			Bottom = 4,
			Top = 8,
			Near = 16,
			Far = 32
		}

		/// <summary>
		/// Compute the bit code for a point against the given bounding box.
		/// </summary>
		/// <param name="extents"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		public static eCohenSutherlandCode CohenSutherlandOutCode(BoundingBox extents, Vector3 point)
		{
			eCohenSutherlandCode code = eCohenSutherlandCode.Inside;

			if (point.X < extents.Min.X)
				code |= eCohenSutherlandCode.Left;
			else if (point.X > extents.Max.X)
				code |= eCohenSutherlandCode.Right;

			if (point.Y < extents.Min.Y)
				code |= eCohenSutherlandCode.Bottom;
			else if (point.Y > extents.Max.Y)
				code |= eCohenSutherlandCode.Top;

			if (point.Z < extents.Min.Z)
				code |= eCohenSutherlandCode.Near;
			else if (point.Z > extents.Max.Z)
				code |= eCohenSutherlandCode.Far;

			return code;
		}

		/// <summary>
		/// Clips the line p0 to p1, modifying the points so they fit within the given bounding box.
		/// </summary>
		/// <param name="extents"></param>
		/// <param name="p0"></param>
		/// <param name="p1"></param>
		/// <returns>True if the line passes through the bounding box.</returns>
		public static bool CohenSutherlandLineClip(BoundingBox extents, ref Vector3 p0, ref Vector3 p1)
		{
			// Compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
			eCohenSutherlandCode outcode0 = CohenSutherlandOutCode(extents, p0);
			eCohenSutherlandCode outcode1 = CohenSutherlandOutCode(extents, p1);

			while (true)
			{
				// Bitwise OR is 0. Trivially accept and get out of loop
				if ((outcode0 | outcode1) == eCohenSutherlandCode.Inside)
					return true;

				// Bitwise AND is not 0. Trivially reject and get out of loop
				if ((outcode0 & outcode1) != eCohenSutherlandCode.Inside)
					return false;

				// failed both tests, so calculate the line segment to clip
				// from an outside point to an intersection with clip edge
				double x, y, z;

				// At least one endpoint is outside the clip rectangle; pick it.
				eCohenSutherlandCode outcodeOut = outcode0 == eCohenSutherlandCode.Inside ? outcode1 : outcode0;

				// Now find the intersection point;
				// use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
				if ((outcodeOut & eCohenSutherlandCode.Top) != 0)
				{
					x = p0.X + (p1.X - p0.X) * (extents.Max.Y - p0.Y) / (p1.Y - p0.Y);
					y = extents.Max.Y;
					z = p0.Z + (p1.Z - p0.Z) * (extents.Max.Y - p0.Y) / (p1.Y - p0.Y);
				}
				else if ((outcodeOut & eCohenSutherlandCode.Bottom) != 0)
				{
					x = p0.X + (p1.X - p0.X) * (extents.Min.Y - p0.Y) / (p1.Y - p0.Y);
					y = extents.Min.Y;
					z = p0.Z + (p1.Z - p0.Z) * (extents.Min.Y - p0.Y) / (p1.Y - p0.Y);
				}
				else if ((outcodeOut & eCohenSutherlandCode.Right) != 0)
				{
					x = extents.Max.X;
					y = p0.Y + (p1.Y - p0.Y) * (extents.Max.X - p0.X) / (p1.X - p0.X);
					z = p0.Z + (p1.Z - p0.Z) * (extents.Max.X - p0.X) / (p1.X - p0.X);
				}
				else if ((outcodeOut & eCohenSutherlandCode.Left) != 0)
				{
					x = extents.Min.X;
					y = p0.Y + (p1.Y - p0.Y) * (extents.Min.X - p0.X) / (p1.X - p0.X);
					z = p0.Z + (p1.Z - p0.Z) * (extents.Min.X - p0.X) / (p1.X - p0.X);
				}
				else if ((outcodeOut & eCohenSutherlandCode.Far) != 0)
				{
					x = p0.X + (p1.X - p0.X) * (extents.Max.Z - p0.Z) / (p1.Z - p0.Z);
					y = p0.Y + (p1.Y - p0.Y) * (extents.Max.Z - p0.Z) / (p1.Z - p0.Z);
					z = extents.Max.Z;
				}
				else if ((outcodeOut & eCohenSutherlandCode.Near) != 0)
				{
					x = p0.X + (p1.X - p0.X) * (extents.Min.Z - p0.Z) / (p1.Z - p0.Z);
					y = p0.Y + (p1.Y - p0.Y) * (extents.Min.Z - p0.Z) / (p1.Z - p0.Z);
					z = extents.Min.Z;
				}
				else
				{
					// Will never hit this
					return false;
				}

				// Now we move outside point to intersection point to clip
				// and get ready for next pass.
				if (outcodeOut == outcode0)
				{
					p0 = new Vector3((float)x, (float)y, (float)z);
					outcode0 = CohenSutherlandOutCode(extents, p0);
				}
				else
				{
					p1 = new Vector3((float)x, (float)y, (float)z);
					outcode1 = CohenSutherlandOutCode(extents, p1);
				}
			}
		}

		/// <summary>
		/// Clips the line p0 to p1, modifying the points so they fit in front of the plane.
		/// </summary>
		/// <param name="plane"></param>
		/// <param name="p0"></param>
		/// <param name="p1"></param>
		/// <returns>True if the line passes through the front of the plane.</returns>
		public static bool PlaneLineClip(Plane plane, ref Vector3 p0, ref Vector3 p1)
		{
			bool p0InFront = plane.IsInFront(p0);
			bool p1InFront = plane.IsInFront(p1);

			// Both points in front or behind plane
			if (!(p0InFront ^ p1InFront))
				return p0InFront;

			if (p0InFront)
			{
				Vector3 direction = Vector3.Normalize(p1 - p0);
				return PlaneRayClip(plane, new Ray(p0, direction), out p1);
			}
			else
			{
				Vector3 direction = Vector3.Normalize(p0 - p1);
				return PlaneRayClip(plane, new Ray(p1, direction), out p0);
			}
		}

		public static bool PlaneRayClip(Plane plane, Ray ray, out Vector3 intersection)
		{
			intersection = ray.Position;

			// Line and plane are parallel
			float dotDenominator = Vector3.Dot(ray.Direction, plane.Normal);
			if (Math.Abs(dotDenominator) < 0.00001f)
				return false;

			float dotNumerator = -plane.Distance(ray.Position);
			float length = dotNumerator / dotDenominator;

			if (Math.Abs(length) > 0.00001f)
				intersection += ray.Direction * length;

			return true;
		}

		/// <summary>
		/// Clips the given triangle polygon against the given clipping planes.
		/// Returns a fanned triangle mesh for the resulting polygon.
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="clippingPlanes"></param>
		/// <returns></returns>
		public static IEnumerable<Triangle> SutherlandHodgmanPolygonClip(Triangle polygon, IEnumerable<Plane> clippingPlanes)
		{
			IEnumerable<Vector3> verts = SutherlandHodgmanPolygonClip(polygon.Vertices, clippingPlanes);
			return BuildConvexTriangleFan(verts);
		}

		/// <summary>
		/// Clips the given polygon against the given clipping planes.
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="clippingPlanes"></param>
		/// <returns></returns>
		public static IEnumerable<Vector3> SutherlandHodgmanPolygonClip(IEnumerable<Vector3> polygon, IEnumerable<Plane> clippingPlanes)
		{
			List<Vector3> clipped = polygon.ToList();

			foreach (Plane clippingPlane in clippingPlanes)
			{
				if (clipped.Count == 0)
					break;

				List<Vector3> input = clipped;
				clipped = new List<Vector3>();

				for (int index = 0; index < input.Count; index++)
				{
					Vector3 a = input[index];
					Vector3 b = input[(index + 1) % input.Count];

					bool aInFront = !clippingPlane.IsBehind(a);
					bool bInFront = !clippingPlane.IsBehind(b);
					
					if (aInFront)
					{
						if (bInFront)
							clipped.Add(b);
						else
						{
							Vector3 intersection;
							PlaneRayClip(clippingPlane, new Ray(a, Vector3.Normalize(b - a)), out intersection);

							if (intersection != a)
								clipped.Add(intersection);
						}
					}
					else if (bInFront)
					{
						Vector3 intersection;
						PlaneRayClip(clippingPlane, new Ray(a, Vector3.Normalize(b - a)), out intersection);

						if (intersection != b)
							clipped.Add(intersection);

						clipped.Add(b);
					}
				}
			}

			return clipped;
		}

		/// <summary>
		/// Given a convex shape defined by the given sequence of vertices, returns a contiguous triangle fan. 
		/// </summary>
		/// <param name="verts"></param>
		/// <returns></returns>
		private static IEnumerable<Triangle> BuildConvexTriangleFan(IEnumerable<Vector3> verts)
		{
			Vector3 a = Vector3.Zero;
			Vector3 b = Vector3.Zero;
			Vector3 c = Vector3.Zero;

			int index = 0;

			foreach (Vector3 vert in verts)
			{
				if (index == 0)
					a = vert;

				if (index % 2 == 0)
					c = vert;
				else
					b = vert;

				if (index >= 2)
					yield return new Triangle(a, b, c);

				index++;
			}
		}
	}
}
