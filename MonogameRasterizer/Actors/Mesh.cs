using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonogameRasterizer.Actors
{
	public sealed class Mesh
	{
		public Vector3[] Vertices { get; set; }

		public int[] Triangles { get; set; }

		public Mesh()
		{
			Vertices = new Vector3[0];
			Triangles = new int[0];
		}

		public IEnumerable<Triangle> GetTriangles()
		{
			for (int index = 0; index < Triangles.Length; index += 3)
			{
				yield return new Triangle
				{
					A = Vertices[Triangles[index]],
					B = Vertices[Triangles[index + 1]],
					C = Vertices[Triangles[index + 2]]
				};
			}
		}
	}
}

