using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonogameRasterizer
{
	public struct Triangle
	{
		public Vector3 A { get; set; }

		public Vector3 B { get; set; }

		public Vector3 C { get; set; }

		public IEnumerable<Vector3> Vertices { get { return new[] {A, B, C}; } }

		public Vector3 Normal
		{
			get
			{
				return Vector3.Normalize(Vector3.Cross(C - B, A - B));
			}
		}

		public Triangle Transform(Matrix transform)
		{
			return new Triangle
			{
				A = Vector3.Transform(A, transform),
				B = Vector3.Transform(B, transform),
				C = Vector3.Transform(C, transform)
			};
		}
	}
}
