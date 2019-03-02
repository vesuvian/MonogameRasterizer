using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonogameRasterizer.Utils;

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
				A = VectorUtils.MultiplyPointMatrix(A, transform),
				B = VectorUtils.MultiplyPointMatrix(B, transform),
				C = VectorUtils.MultiplyPointMatrix(C, transform)
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

		/// <summary>
		/// Returns a new triangle with the same points such that A is the topmost vertex and C is the bottom.
		/// </summary>
		/// <param name="triangle"></param>
		/// <returns></returns>
		public static Triangle SortVertexIndexingByY(Triangle triangle)
		{
			// A comes before B
			if (triangle.A.Y <= triangle.B.Y)
			{
				// ABC
				if (triangle.B.Y <= triangle.C.Y)
					return triangle;

				// CAB
				if (triangle.C.Y <= triangle.A.Y)
					return new Triangle(triangle.C, triangle.A, triangle.B);

				// ACB
				return new Triangle(triangle.A, triangle.C, triangle.B);
			}

			// A comes after B and before C
			if (triangle.A.Y <= triangle.C.Y)
				// BAC
				return new Triangle(triangle.B, triangle.A, triangle.C);

			// A comes last
			// BCA
			if (triangle.B.Y <= triangle.C.Y)
				return new Triangle(triangle.B, triangle.C, triangle.A);

			// CBA
			return new Triangle(triangle.C, triangle.B, triangle.A);
		}
	}
}
