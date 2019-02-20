using Microsoft.Xna.Framework;
using MonogameRasterizer.Extensions;
using MonogameRasterizer.Utils;

namespace MonogameRasterizer.Actors
{
	public sealed class Camera : AbstractActor
	{
		public float NearClipPlane { get; set; }

		public float FarClipPlane { get; set; }

		public float FovRadians { get; set; }

		public float AspectRatio { get; set; }

		public float CanvasWidth { get; set; }

		public float CanvasHeight { get; set; }

		public Matrix Projection
		{
			get { return Matrix.CreatePerspectiveFieldOfView(FovRadians, AspectRatio, NearClipPlane, FarClipPlane); }
		}

		public Camera()
		{
			NearClipPlane = 1.0f;
			FarClipPlane = 200.0f;
			FovRadians = MathHelper.PiOver2;
			AspectRatio = 1.0f;
			CanvasWidth = 1.0f;
			CanvasHeight = 1.0f;
		}

		public void SetAspectRatio(Rectangle bufferBounds)
		{
			AspectRatio = (float)bufferBounds.Width / bufferBounds.Height;
		}

		public void Render(Buffer buffer, GameTime gameTime, Scene scene)
		{
			Matrix worldToCanvas = Transform.Matrix.Inverse() * Projection;

			DrawGrid(buffer, worldToCanvas);
			DrawAxis(buffer, worldToCanvas);

			foreach (MeshActor item in scene.Geometry)
				DrawWireframe(buffer, worldToCanvas, item);
		}

		private void DrawGrid(Buffer buffer, Matrix worldToCanvas)
		{
			const int lines = 11;
			const float half = (lines - 1) / 2.0f;

			for (int x = 0; x < lines; x++)
			{
				Vector3 start = new Vector3(-half + x, 0, -half);
				Vector3 end = new Vector3(-half + x, 0, half);

				DrawLine(buffer, worldToCanvas, start, end, Color.Gray);
			}

			for (int z = 0; z < lines; z++)
			{
				Vector3 start = new Vector3(-half, 0, -half + z);
				Vector3 end = new Vector3(half, 0, -half + z);

				DrawLine(buffer, worldToCanvas, start, end, Color.Gray);
			}
		}

		private void DrawAxis(Buffer buffer, Matrix worldToCanvas)
		{
			DrawLine(buffer, worldToCanvas, Vector3.Zero, Vector3.Right, Color.Red);
			DrawLine(buffer, worldToCanvas, Vector3.Zero, Vector3.Up, Color.Green);
			DrawLine(buffer, worldToCanvas, Vector3.Zero, Vector3.Forward, Color.Blue);
		}

		private void DrawWireframe(Buffer buffer, Matrix worldToCanvas, MeshActor geometry)
		{
			foreach (Triangle triangle in geometry.Mesh.GetTriangles())
			{
				Triangle world = triangle.Transform(geometry.Transform.Matrix);

				DrawLine(buffer, worldToCanvas, world.A, world.B, Color.Red);
				DrawLine(buffer, worldToCanvas, world.B, world.C, Color.Red);
				DrawLine(buffer, worldToCanvas, world.C, world.A, Color.Red);
			}
		}

		private void DrawLine(Buffer buffer, Matrix worldToCanvas, Vector3 worldA, Vector3 worldB, Color color)
		{
			Vector3 canvasA = VectorUtils.MultiplyPointMatrix(worldA, worldToCanvas);
			Vector3 canvasB = VectorUtils.MultiplyPointMatrix(worldB, worldToCanvas);

			BoundingBox extents = new BoundingBox(new Vector3(-1.0f, -1.0f, NearClipPlane),
			                                      new Vector3(1.0f, 1.0f, FarClipPlane));

			if (!ClippingUtils.CohenSutherlandLineClip(extents, ref canvasA, ref canvasB))
				return;

			Vector3 screenA = buffer.CanvasToScreen(canvasA);
			Vector3 screenB = buffer.CanvasToScreen(canvasB);

			Vector2 rasterA = buffer.ScreenToRaster(CanvasWidth, CanvasHeight, screenA);
			Vector2 rasterB = buffer.ScreenToRaster(CanvasWidth, CanvasHeight, screenB);

			buffer.DrawLine(rasterA, rasterB, color);
		}
	}
}
