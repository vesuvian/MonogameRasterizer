using System.Collections.Generic;
using System.Linq;
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
			yield return new Plane(Vector3.Backward, NearClipPlane);
			yield return new Plane(Vector3.Forward, -FarClipPlane);

			// Left and right
			float halfFov = FovRadians / 2.0f;
			halfFov -= MathHelper.ToRadians(5.0f);

			Quaternion yaw = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(90.0f) - halfFov, 0.0f, 0.0f);

			// TODO - Some over-clipping close to the camera?
			yield return new Plane(Vector3.Zero, Vector3.Transform(Vector3.Backward, yaw));
			yield return new Plane(Vector3.Zero, Vector3.Transform(Vector3.Backward, Quaternion.Inverse(yaw)));

			// Top and bottom
			halfFov /= AspectRatio; // TODO - Correct for aspect ratio
			Quaternion pitch = Quaternion.CreateFromYawPitchRoll(0.0f, MathHelper.ToRadians(90.0f) - halfFov, 0.0f);

			yield return new Plane(Vector3.Zero, Vector3.Transform(Vector3.Backward, pitch));
			yield return new Plane(Vector3.Zero, Vector3.Transform(Vector3.Backward, Quaternion.Inverse(pitch)));
		}

		public IEnumerable<Plane> GetWorldFrustumPlanes()
		{
			return GetLocalFrustumPlanes().Select(p => Plane.Transform(p, Transform.Matrix.Inverse()));
		}

		#region Drawing

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
			DrawLine(buffer, Vector3.Zero, Vector3.Backward, Color.Blue);
		}

		private void DrawWireframe(Buffer buffer, MeshActor geometry)
		{
			foreach (Triangle triangle in geometry.Mesh.GetTriangles())
			{
				Triangle world = triangle.Transform(geometry.Transform.Matrix);

				// Backface culling
				if (Vector3.Dot(world.Normal, Vector3.Normalize(world.Centroid - Transform.Position)) > 0)
					continue;

				// Temp
				foreach (Triangle t in ClippingUtils.SutherlandHodgmanPolygonClip(world, GetWorldFrustumPlanes()))
					DrawFilledTriangle(buffer, t, Color.Red);

				DrawLine(buffer, world.A, world.B, Color.Black);
				DrawLine(buffer, world.B, world.C, Color.Black);
				DrawLine(buffer, world.C, world.A, Color.Black);
			}
		}

		private void DrawFilledTriangle(Buffer buffer, Triangle world, Color color)
		{
			Triangle camera = world.Transform(Transform.Matrix.Inverse());
			Triangle canvas = camera.Transform(Projection);
			Triangle screen = CanvasToScreen(canvas);
			Triangle raster = buffer.ScreenToRaster(CanvasWidth, CanvasHeight, screen);

			buffer.DrawFilledTriangle(raster, color);
		}

		private void DrawLine(Buffer buffer, Vector3 worldA, Vector3 worldB, Color color)
		{
			Vector3 cameraA = WorldToCamera(worldA);
			Vector3 cameraB = WorldToCamera(worldB);

			foreach (Plane plane in GetLocalFrustumPlanes())
				if (!ClippingUtils.PlaneLineClip(plane, ref cameraA, ref cameraB))
					return;

			Vector3 canvasA = CameraToCanvas(cameraA);
			Vector3 canvasB = CameraToCanvas(cameraB);

			Vector3 screenA = CanvasToScreen(canvasA);
			Vector3 screenB = CanvasToScreen(canvasB);

			Vector3 rasterA = buffer.ScreenToRaster(CanvasWidth, CanvasHeight, screenA);
			Vector3 rasterB = buffer.ScreenToRaster(CanvasWidth, CanvasHeight, screenB);

			buffer.DrawLine(rasterA, rasterB, color);
		}

		#endregion

		#region Conversion

		public Vector3 WorldToCamera(Vector3 world)
		{
			return Vector3.Transform(world, Transform.Matrix.Inverse());
		}

		public Vector3 CameraToWorld(Vector3 camera)
		{
			return Vector3.Transform(camera, Transform.Matrix);
		}

		public Vector3 CameraToCanvas(Vector3 camera)
		{
			return VectorUtils.MultiplyPointMatrix(camera, Projection);
		}

		public Vector3 CanvasToCamera(Vector3 canvas)
		{
			return VectorUtils.MultiplyPointMatrix(canvas, Projection.Inverse());
		}

		public Vector3 CanvasToScreen(Vector3 point)
		{
			return new Vector3(point.X / -point.Z,
			                   point.Y / -point.Z,
			                   -point.Z);
		}

		public Triangle CanvasToScreen(Triangle canvas)
		{
			return new Triangle
			{
				A = CanvasToScreen(canvas.A),
				B = CanvasToScreen(canvas.B),
				C = CanvasToScreen(canvas.C)
			};
		}

		#endregion
	}
}
