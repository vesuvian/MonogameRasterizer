using System;
using Microsoft.Xna.Framework;

namespace MonogameRasterizer.Utils
{
	public static class QuaternionUtils
	{
		/// <summary>
		/// Evaluates a rotation needed to be applied to an object positioned at sourcePoint to face destPoint
		/// </summary>
		/// <param name="sourcePoint">Coordinates of source point</param>
		/// <param name="destPoint">Coordinates of destination point</param>
		/// <returns></returns>
		public static Quaternion LookAt(Vector3 sourcePoint, Vector3 destPoint)
		{
			Vector3 forwardVector = Vector3.Normalize(destPoint - sourcePoint);

			float dot = Vector3.Dot(Vector3.Forward, forwardVector);

			if (Math.Abs(dot + 1.0f) < 0.000001f)
				return new Quaternion(Vector3.Up.X, Vector3.Up.Y, Vector3.Up.Z, MathHelper.Pi);

			if (Math.Abs(dot - 1.0f) < 0.000001f)
				return Quaternion.Identity;

			float rotAngle = (float)Math.Acos(dot);
			Vector3 rotAxis = Vector3.Cross(Vector3.Forward, forwardVector);
			rotAxis = Vector3.Normalize(rotAxis);

			return Quaternion.CreateFromAxisAngle(rotAxis, rotAngle);
		}
	}
}
