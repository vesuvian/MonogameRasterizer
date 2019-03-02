using Microsoft.Xna.Framework;

namespace MonogameRasterizer.Extensions
{
	public static class PlaneExtensions
	{
		public static bool IsInFront(this Plane extends, Vector3 point)
		{
			return extends.Distance(point) > 0.0f;
		}

		public static bool IsBehind(this Plane extends, Vector3 point)
		{
			return extends.Distance(point) < 0.0f;
		}

		public static float Distance(this Plane extends, Vector3 point)
		{
			return Vector3.Dot(extends.Normal, point) - extends.D;
		}
	}
}
