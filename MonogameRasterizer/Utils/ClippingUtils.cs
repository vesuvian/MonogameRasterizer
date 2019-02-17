using System;
using Microsoft.Xna.Framework;

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
		/// Compute the bit code for a point (x, y) using the clip rectangle
		/// bounded diagonally by (xmin, ymin), and (xmax, ymax)
		/// ASSUME THAT xmax , xmin , ymax and ymin are global constants.
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
		/// Cohen–Sutherland clipping algorithm clips a line from
		/// P0 = (x0, y0) to P1 = (x1, y1) against a rectangle with
		/// diagonal from (xmin, ymin) to (xmax, ymax).
		/// </summary>
		/// <param name="extents"></param>
		/// <param name="p0"></param>
		/// <param name="p1"></param>
		/// <returns>a list of two points in the resulting clipped line, or zero</returns>
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
				float x, y, z;

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
					p0 = new Vector3(x, y, z);
					outcode0 = CohenSutherlandOutCode(extents, p0);
				}
				else
				{
					p1 = new Vector3(x, y, z);
					outcode1 = CohenSutherlandOutCode(extents, p1);
				}
			}
		}
	}
}
