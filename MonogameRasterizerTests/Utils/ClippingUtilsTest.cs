﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonogameRasterizer;
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

		[Test]
		public static void SutherlandHodgmanPolygonClipTest()
		{
			Triangle polygon = new Triangle(Vector3.Zero, Vector3.Up, Vector3.Right);

			List<Plane> planes = new List<Plane>
			{
				new Plane(Vector3.Up, 0.0f),
				new Plane(Vector3.Right, 0.0f),
				new Plane(Vector3.Down, 0.5f),
				new Plane(Vector3.Left, 0.5f)
			};

			List<Vector3> verts = ClippingUtils.SutherlandHodgmanPolygonClip(polygon, planes).ToList();

			List<Vector3> expected = new List<Vector3>
			{
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(0.0f, 0.5f, 0.0f),
				new Vector3(0.5f, 0.5f, 0.0f),
				new Vector3(0.5f, 0.0f, 0.0f),
			};

			//Assert.IsTrue(verts.SequenceEqual(expected));

			Assert.Inconclusive();
		}
	}
}
