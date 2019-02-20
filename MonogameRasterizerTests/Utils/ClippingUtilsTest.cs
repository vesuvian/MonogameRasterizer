using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonogameRasterizer.Utils;
using NUnit.Framework;

namespace MonogameRasterizerTests.Utils
{
	[TestFixture]
	public sealed class ClippingUtilsTest
	{
		public sealed class CohenSutherlandOutCodeTestCase
		{
			public BoundingBox Extents { get; set; }
			public Vector3 Point { get; set; }
			public ClippingUtils.eCohenSutherlandCode Expected { get; set; }
		}

		private static IEnumerable<CohenSutherlandOutCodeTestCase> CohenSutherlandOutCodeTestCases
		{
			get
			{
				yield return new CohenSutherlandOutCodeTestCase
				{
					Extents = new BoundingBox(Vector3.Zero, Vector3.One),
					Point = Vector3.Zero,
					Expected = ClippingUtils.eCohenSutherlandCode.Inside
				};

				yield return new CohenSutherlandOutCodeTestCase
				{
					Extents = new BoundingBox(Vector3.Zero, Vector3.One),
					Point = Vector3.Left,
					Expected = ClippingUtils.eCohenSutherlandCode.Left
				};

				yield return new CohenSutherlandOutCodeTestCase
				{
					Extents = new BoundingBox(Vector3.Zero, Vector3.One),
					Point = Vector3.Right * 2,
					Expected = ClippingUtils.eCohenSutherlandCode.Right
				};

				yield return new CohenSutherlandOutCodeTestCase
				{
					Extents = new BoundingBox(Vector3.Zero, Vector3.One),
					Point = Vector3.Up * 2,
					Expected = ClippingUtils.eCohenSutherlandCode.Top
				};

				yield return new CohenSutherlandOutCodeTestCase
				{
					Extents = new BoundingBox(Vector3.Zero, Vector3.One),
					Point = Vector3.Down,
					Expected = ClippingUtils.eCohenSutherlandCode.Bottom
				};

				yield return new CohenSutherlandOutCodeTestCase
				{
					Extents = new BoundingBox(Vector3.Zero, Vector3.One),
					Point = Vector3.Forward,
					Expected = ClippingUtils.eCohenSutherlandCode.Near
				};

				yield return new CohenSutherlandOutCodeTestCase
				{
					Extents = new BoundingBox(Vector3.Zero, Vector3.One),
					Point = Vector3.Backward * 2,
					Expected = ClippingUtils.eCohenSutherlandCode.Far
				};
			}
		}

		[Test, TestCaseSource("CohenSutherlandOutCodeTestCases")]
		public static void CohenSutherlandOutCodeTest(CohenSutherlandOutCodeTestCase testCase)
		{
			Assert.AreEqual(testCase.Expected, ClippingUtils.CohenSutherlandOutCode(testCase.Extents, testCase.Point));
		}
	}
}
