using Microsoft.Xna.Framework;

namespace MonogameRasterizer.Extensions
{
	public static class MatrixExtensions
	{
		public static Matrix Inverse(this Matrix extends)
		{
			Matrix output = Matrix.Invert(extends);
			return output;
		}
	}
}
