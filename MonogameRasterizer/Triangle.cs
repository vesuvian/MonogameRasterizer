using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonogameRasterizer
{
	public struct Triangle : IEnumerable<Vector3>
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

		public Vector3 Centroid
		{
			get { return (A + B + C) / 3.0f; }
		}

		public Triangle(Vector3 a, Vector3 b, Vector3 c)
		{
			A = a;
			B = b;
			C = c;
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

		public IEnumerator<Vector3> GetEnumerator()
		{
			return Vertices.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
