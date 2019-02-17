using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonogameRasterizer.Actors;

namespace MonogameRasterizer
{
	public sealed class Scene
	{
		private readonly Camera m_Camera;

		public List<MeshActor> Geometry { get; set; }

		public Scene()
		{
			m_Camera = new Camera();
			m_Camera.Transform.Position = Vector3.Forward * 3 + Vector3.Up + Vector3.Right;

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
			foreach (var item in Geometry)
			{
				item.Transform.Scale = Vector3.One * (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds);
				item.Transform.Rotation *= Quaternion.CreateFromYawPitchRoll((float)gameTime.ElapsedGameTime.TotalSeconds,
				                                                             (float)gameTime.ElapsedGameTime.TotalSeconds,
				                                                             (float)gameTime.ElapsedGameTime.TotalSeconds);
			}

			KeyboardState keyState = Keyboard.GetState();

			Vector3 move = Vector3.Zero;

			if (keyState.IsKeyDown(Keys.E))
				move += Vector3.Up;

			if (keyState.IsKeyDown(Keys.Q))
				move += Vector3.Down;

			if (keyState.IsKeyDown(Keys.W))
				move += Vector3.Forward;

			if (keyState.IsKeyDown(Keys.S))
				move += Vector3.Backward;

			if (keyState.IsKeyDown(Keys.A))
				move += Vector3.Left;

			if (keyState.IsKeyDown(Keys.D))
				move += Vector3.Right;

			move *= (float)gameTime.ElapsedGameTime.TotalSeconds;

			m_Camera.Transform.Position += move;
		}
	}
}
