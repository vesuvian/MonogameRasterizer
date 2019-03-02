using Microsoft.Xna.Framework;

namespace MonogameRasterizer.Extensions
{
	public static class PlaneExtensions
	{
		public static bool IsInFront(this Plane extends, Vector3 point)
		{
			return Vector3.Dot(point, extends.Normal) + extends.D > 0.0f;
		}
	}
}
