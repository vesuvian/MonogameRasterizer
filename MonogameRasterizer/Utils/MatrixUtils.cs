using System;
using Microsoft.Xna.Framework;

namespace MonogameRasterizer.Utils
{
	public static class MatrixUtils
	{
		public static Matrix Trs(Vector3 translation, Quaternion rotation, Vector3 scale)
		{
			Matrix tMat = Matrix.CreateTranslation(translation);
			Matrix rMat = Matrix.CreateFromQuaternion(rotation);
			Matrix sMat = Matrix.CreateScale(scale);

			return tMat * rMat * sMat;
		}
	}
}
