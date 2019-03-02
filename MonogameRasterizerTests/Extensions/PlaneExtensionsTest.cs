using Microsoft.Xna.Framework;
using MonogameRasterizer.Extensions;
using NUnit.Framework;

namespace MonogameRasterizerTests.Extensions
{
	[TestFixture]
	public sealed class PlaneExtensionsTest
	{
		[Test]
		public static void IsInFrontTest()
		{
			Assert.IsTrue(new Plane(Vector3.Up, 0.0f).IsInFront(Vector3.Up));
			Assert.IsFalse(new Plane(Vector3.Up, 0.0f).IsInFront(Vector3.Down));
			Assert.IsFalse(new Plane(Vector3.Up, 0.5f).IsInFront(Vector3.Zero));
		}

		[Test]
		public static void IsBehindTest()
		{
			Assert.IsFalse(new Plane(Vector3.Up, 0.0f).IsBehind(Vector3.Up));
			Assert.IsTrue(new Plane(Vector3.Up, 0.0f).IsBehind(Vector3.Down));
			Assert.IsTrue(new Plane(Vector3.Up, 0.5f).IsBehind(Vector3.Zero));
		}
	}
}
