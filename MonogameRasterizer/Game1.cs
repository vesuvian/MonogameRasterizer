using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonogameRasterizer
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private readonly GraphicsDeviceManager m_Graphics;

		private SpriteBatch m_SpriteBatch;
		private Texture2D m_Canvas;
		private Buffer m_Buffer;
		private Scene m_Scene;

		public Game1()
		{
			m_Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			m_Scene = new Scene();

			Rectangle tracedSize = GraphicsDevice.PresentationParameters.Bounds;
			m_Canvas = new Texture2D(GraphicsDevice, tracedSize.Width, tracedSize.Height, false, SurfaceFormat.Color);
			m_Buffer = new Buffer
			{
				Bounds = tracedSize,
				Pixels = new uint[tracedSize.Width * tracedSize.Height]
			};

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			m_SpriteBatch = new SpriteBatch(GraphicsDevice);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			Window.Title = string.Format("{0:0.00} fps", 1.0f / gameTime.ElapsedGameTime.TotalSeconds);

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
			    Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			m_Scene.Update(gameTime);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			GraphicsDevice.Textures[0] = null;

			m_Buffer.Clear();

			m_Scene.Render(m_Buffer, gameTime);
			m_Canvas.SetData(m_Buffer.Pixels);

			m_SpriteBatch.Begin();
			m_SpriteBatch.Draw(m_Canvas, new Rectangle(0, 0, m_Buffer.Bounds.Width, m_Buffer.Bounds.Height), Color.White);
			m_SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
