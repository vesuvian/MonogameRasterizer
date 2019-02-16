using Microsoft.Xna.Framework;
using MonogameRasterizer.Extensions;

namespace MonogameRasterizer.Actors
{
	public sealed class Camera : AbstractActor
	{
		public float NearClipPlane { get; set; }

		public float FarClipPlane { get; set; }

		public float FovRadians { get; set; }

		public float AspectRatio { get; set; }

		public Matrix Projection
		{
			get { return Matrix.CreatePerspectiveFieldOfView(FovRadians, AspectRatio, NearClipPlane, FarClipPlane); }
		}

		public Camera()
		{
			NearClipPlane = 1.0f;
			FarClipPlane = 200.0f;
			FovRadians = MathHelper.PiOver4;
			AspectRatio = 1.0f;
		}

		public void Render(Buffer buffer, GameTime gameTime, Scene scene)
		{
			foreach (var item in scene.Geometry)
				RenderGeometry(buffer, item);
		}

		private void RenderGeometry(Buffer buffer, MeshActor geometry)
		{
			foreach (Triangle triangle in geometry.Mesh.GetTriangles())
			{
				Triangle world = triangle.Transform(geometry.Transform.Matrix);
				Triangle view = world.Transform(Transform.Matrix);
				Triangle screen = view.Transform(Projection);

				RenderScreenTriangle(buffer, screen);
			}
		}

		private void RenderScreenTriangle(Buffer buffer, Triangle triangle)
		{
			Vector2 a = buffer.CameraToRaster(triangle.A);
			Vector2 b = buffer.CameraToRaster(triangle.B);
			Vector2 c = buffer.CameraToRaster(triangle.C);

			buffer.DrawLine(a, b, Color.Red);
			buffer.DrawLine(b, c, Color.Red);
			buffer.DrawLine(c, a, Color.Red);
		}

		public void SetAspectRatio(Rectangle bufferBounds)
		{
			AspectRatio = (float)bufferBounds.Width / bufferBounds.Height;
		}
	}
}
