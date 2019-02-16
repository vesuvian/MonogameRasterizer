namespace MonogameRasterizer.Actors
{
	public abstract class AbstractActor : IActor
	{
		private readonly Transform m_Transform;

		public Transform Transform
		{
			get { return m_Transform; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractActor()
		{
			m_Transform = new Transform();
		}
	}
}