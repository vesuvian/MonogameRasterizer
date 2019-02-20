using Microsoft.Xna.Framework;

namespace MonogameRasterizer.Utils
{
	public static class VectorUtils
	{
		/// <summary>
		/// Transforms the point correcting for W.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="transform"></param>
		/// <returns></returns>
		public static Vector3 MultiplyPointMatrix(Vector3 point, Matrix transform)
		{
			Vector4 result = Vector4.Transform(point, transform);
			result /= result.W;
			return new Vector3(result.X, result.Y, result.Z);
		}
	}
}
