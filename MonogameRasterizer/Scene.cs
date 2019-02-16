using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonogameRasterizer.Actors;

namespace MonogameRasterizer
{
	public sealed class Scene
	{
		Random random = new Random();

		private readonly Camera m_Camera;

		public List<MeshActor> Geometry { get; set; }

		public Scene()
		{
			m_Camera = new Camera();
			m_Camera.Transform.Position = Vector3.Backward * 3;

			Geometry = new List<MeshActor>();

			MeshActor origin = new MeshActor { Mesh = Primitives.Cube() };

			MeshActor forward = new MeshActor { Mesh = Primitives.Cube() };
			forward.Transform.Position += Vector3.Forward * 1f;

			MeshActor right = new MeshActor { Mesh = Primitives.Cube() };
			right.Transform.Position += Vector3.Right * 1f;

			MeshActor up = new MeshActor { Mesh = Primitives.Cube() };
			up.Transform.Position += Vector3.Up * 1f;

			Geometry.Add(origin);
			Geometry.Add(forward);
			Geometry.Add(right);
			Geometry.Add(up);
		}

		public void Render(Buffer buffer, GameTime gameTime)
		{
			m_Camera.SetAspectRatio(buffer.Bounds);

			m_Camera.Render(buffer, gameTime, this);
		}

		public void Update(GameTime gameTime)
		{
		}
	}
}
