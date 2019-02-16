using Microsoft.Xna.Framework;
using MonogameRasterizer.Actors;

namespace MonogameRasterizer
{
	public sealed class Primitives
	{
		public static Mesh Cube()
		{
			Vector3[] verts =
			{
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, -0.5f),

				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f)
			};

			int[] triangles =
			{
				// Top
				0, 2, 1,
				1, 2, 3,

				// Bottom
				4, 5, 6,
				7, 6, 5,

				// Left
				6, 3, 2,
				3, 6, 7,

				// Right
				0, 1, 4,
				1, 5, 4,

				// Front
				0, 4, 6,
				0, 6, 2,

				// Back
				1, 7, 5,
				1, 3, 7
			};

			return new Mesh
			{
				Vertices = verts,
				Triangles = triangles
			};
		}
	}
}
