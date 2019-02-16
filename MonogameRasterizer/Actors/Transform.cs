using Microsoft.Xna.Framework;
using MonogameRasterizer.Utils;

namespace MonogameRasterizer.Actors
{
	public sealed class Transform
	{
		public Vector3 Position { get; set; }

		public Vector3 Scale { get; set; }

		public Quaternion Rotation { get; set; }

		public Matrix Matrix
		{
			get { return MatrixUtils.Trs(Position, Rotation, Scale); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Transform()
		{
			Position = Vector3.Zero;
			Scale = Vector3.One;
			Rotation = Quaternion.Identity;
		}
	}
}