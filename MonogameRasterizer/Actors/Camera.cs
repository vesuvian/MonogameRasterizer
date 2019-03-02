using System.Collections.Generic;
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
			NearClipPlane = 0.1f;
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
		
		public IEnumerable<Plane> GetLocalFrustumPlanes()
		{
			// Near and far
			yield return new Plane(new Vector3(0, 0, NearClipPlane), Vector3.Backward);
			yield return new Plane(new Vector3(0, 0, FarClipPlane), Vector3.Forward);

			// Left and right
			float halfFov = FovRadians / 2.0f;
			Quaternion yaw = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(90.0f) - halfFov, 0.0f, 0.0f);

			yield return new Plane(Vector3.Zero, Vector3.Transform(Vector3.Backward, yaw));
			yield return new Plane(Vector3.Zero, Vector3.Transform(Vector3.Backward, Quaternion.Inverse(yaw)));

			// Top and bottom
			halfFov /= AspectRatio;
			Quaternion pitch = Quaternion.CreateFromYawPitchRoll(0.0f, MathHelper.ToRadians(90.0f) - halfFov, 0.0f);

			yield return new Plane(Vector3.Zero, Vector3.Transform(Vector3.Backward, pitch));
			yield return new Plane(Vector3.Zero, Vector3.Transform(Vector3.Backward, Quaternion.Inverse(pitch)));
		}
		
		public void Render(Buffer buffer, GameTime gameTime, Scene scene)
		{
			DrawGrid(buffer);
			DrawAxis(buffer);

			foreach (MeshActor item in scene.Geometry)
				DrawWireframe(buffer, item);
		}

		private void DrawGrid(Buffer buffer)
		{
			const int lines = 11;
			const float half = (lines - 1) / 2.0f;
			
			for (int x = 0; x < lines; x++)
			{
				Vector3 start = new Vector3(-half + x, 0, -half);
				Vector3 end = new Vector3(-half + x, 0, half);

				DrawLine(buffer, start, end, Color.Gray);
			}

			for (int z = 0; z < lines; z++)
			{
				Vector3 start = new Vector3(-half, 0, -half + z);
				Vector3 end = new Vector3(half, 0, -half + z);

				DrawLine(buffer, start, end, Color.Gray);
			}
		}
		
		private void DrawAxis(Buffer buffer)
		{
			DrawLine(buffer, Vector3.Zero, Vector3.Right, Color.Red);
			DrawLine(buffer, Vector3.Zero, Vector3.Up, Color.Green);
			DrawLine(buffer, Vector3.Zero, Vector3.Forward, Color.Blue);
		}

		private void DrawWireframe(Buffer buffer, MeshActor geometry)
		{
			foreach (Triangle triangle in geometry.Mesh.GetTriangles())
			{
				Triangle world = triangle.Transform(geometry.Transform.Matrix);

				DrawLine(buffer, world.A, world.B, Color.Red);
				DrawLine(buffer, world.B, world.C, Color.Red);
				DrawLine(buffer, world.C, world.A, Color.Red);
			}
		}

		private void DrawLine(Buffer buffer, Vector3 worldA, Vector3 worldB, Color color)
		{
			Vector3 cameraA = Vector3.Transform(worldA, Transform.Matrix.Inverse());
			Vector3 cameraB = Vector3.Transform(worldB, Transform.Matrix.Inverse());

			foreach (Plane plane in GetLocalFrustumPlanes())
				if (!ClippingUtils.PlaneLineClip(plane, ref cameraA, ref cameraB))
					return;

			Vector3 canvasA = VectorUtils.MultiplyPointMatrix(cameraA, Projection);
			Vector3 canvasB = VectorUtils.MultiplyPointMatrix(cameraB, Projection);

			Vector2 screenA = buffer.CanvasToScreen(canvasA);
			Vector2 screenB = buffer.CanvasToScreen(canvasB);

			Vector2 rasterA = buffer.ScreenToRaster(CanvasWidth, CanvasHeight, screenA);
			Vector2 rasterB = buffer.ScreenToRaster(CanvasWidth, CanvasHeight, screenB);

			buffer.DrawLine(rasterA, rasterB, color);
		}
	}
}
