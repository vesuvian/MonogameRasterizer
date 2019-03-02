using System;
using Microsoft.Xna.Framework;

namespace MonogameRasterizer
{
	public sealed class Buffer
	{
		public Rectangle Bounds { get; set; }
		public uint[] Pixels { get; set; }

		public void Clear()
		{
			Array.Clear(Pixels, 0, Pixels.Length);
		}

		private int GetIndex(int x, int y)
		{
			return y * Bounds.Width + x;
		}

		#region Draw

		public void DrawScanline(int x0, int x1, int y, Color color)
		{
			if (y < 0 || y >= Bounds.Height)
				return;

			if (x0 > x1)
			{
				int temp = x0;
				x0 = x1;
				x1 = temp;
			}

			if (x1 < 0 || x0 >= Bounds.Width)
				return;

			// Clamp to the buffer
			x0 = Math.Max(x0, 0);
			x1 = Math.Min(x1, Bounds.Width - 1);

			int indexStart = GetIndex(x0, y);
			int indexEnd = GetIndex(x1, y);

			uint colorValue = color.PackedValue;

			for (int index = indexStart; index <= indexEnd; index++)
				Pixels[index] = colorValue;
		}

		public void DrawLine(Vector3 a, Vector3 b, Color color)
		{
			DrawLine((int)a.X, (int)a.Y, (int)b.X, (int)b.Y, color);
		}

		public void DrawLine(int x0, int y0, int x1, int y1, Color color)
		{
			if (y0 == y1)
			{
				DrawScanline(x0, x1, y0, color);
				return;
			}

			int dx = Math.Abs(x1 - x0);
			int sx = x0 < x1 ? 1 : -1;

			int dy = Math.Abs(y1 - y0);
			int sy = y0 < y1 ? 1 : -1;

			int err = (dx > dy ? dx : -dy) / 2;

			for (;;)
			{
				DrawPixel(x0, y0, color);

				if (x0 == x1 && y0 == y1)
					break;

				var e2 = err;

				if (e2 > -dx)
				{
					err -= dy;
					x0 += sx;
				}

				if (e2 < dy)
				{
					err += dx;
					y0 += sy;
				}
			}
		}

		public void DrawPixel(Vector3 point, Color color)
		{
			DrawPixel((int)point.X, (int)point.Y, color);
		}

		public void DrawPixel(int x, int y, Color color)
		{
			if (x < 0 || y < 0 || x >= Bounds.Width || y >= Bounds.Height)
				return;

			int index = GetIndex(x, y);
			Pixels[index] = color.PackedValue;
		}

		public void DrawFilledTriangle(Triangle raster, Color color)
		{
			// Ensure A is at the top of the triangle
			raster = Triangle.SortVertexIndexingByY(raster);

			// Is the triangle bottom-flat?
			if (Math.Abs(raster.B.Y - raster.C.Y) < 0.5f)
			{
				FillBottomFlatTriangle(raster.A, raster.B, raster.C, color);
				return;
			}

			// Is the triangle top-flat?
			if (Math.Abs(raster.A.Y - raster.B.Y) < 0.5f)
			{
				FillTopFlatTriangle(raster.A, raster.B, raster.C, color);
				return;
			}

			// Bisect and draw the two halves
			Vector3 rasterD =
				new Vector3(raster.A.X + (raster.B.Y - raster.A.Y) / (raster.C.Y - raster.A.Y) * (raster.C.X - raster.A.X),
					        raster.B.Y,
					        raster.B.Z);

			FillBottomFlatTriangle(raster.A, raster.B, rasterD, color);
			FillTopFlatTriangle(raster.B, rasterD, raster.C, color);
		}

		private void FillTopFlatTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
		{
			float invslope1 = (v3.X - v1.X) / (v3.Y - v1.Y);
			float invslope2 = (v3.X - v2.X) / (v3.Y - v2.Y);

			float curx1 = v3.X;
			float curx2 = v3.X;

			for (int scanlineY = (int)v3.Y; scanlineY > v1.Y; scanlineY--)
			{
				DrawScanline((int)curx1, (int)curx2, scanlineY, color);
				curx1 -= invslope1;
				curx2 -= invslope2;
			}
		}

		private void FillBottomFlatTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
		{
			float invslope1 = (v2.X - v1.X) / (v2.Y - v1.Y);
			float invslope2 = (v3.X - v1.X) / (v3.Y - v1.Y);

			float curx1 = v1.X;
			float curx2 = v1.X;

			for (int scanlineY = (int)v1.Y; scanlineY <= v2.Y; scanlineY++)
			{
				DrawScanline((int)curx1, (int)curx2, scanlineY, color);
				curx1 += invslope1;
				curx2 += invslope2;
			}
		}

		#endregion

		#region Conversion

		public Vector3 CanvasToScreen(Vector3 point)
		{
			return new Vector3(point.X / -point.Z,
			                   point.Y / -point.Z,
			                   -point.Z);
		}

		public Vector3 ScreenToRaster(float canvasWidth, float canvasHeight, Vector3 screen)
		{
			Vector3 ndc = ScreenToNdc(canvasWidth, canvasHeight, screen);
			return NdcToRaster(ndc);
		}

		public Vector3 ScreenToNdc(float canvasWidth, float canvasHeight, Vector3 screen)
		{
			return new Vector3((screen.X + canvasWidth / 2.0f) / canvasWidth,
			                   (screen.Y + canvasHeight / 2.0f) / canvasHeight,
			                   screen.Z);
		}

		public Vector3 NdcToRaster(Vector3 ndc)
		{
			return new Vector3(ndc.X * Bounds.Width,
			                   (1 - ndc.Y) * Bounds.Height,
			                   ndc.Z);
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

		public Triangle ScreenToRaster(float canvasWidth, float canvasHeight, Triangle screen)
		{
			return new Triangle
			{
				A = ScreenToRaster(canvasWidth, canvasHeight, screen.A),
				B = ScreenToRaster(canvasWidth, canvasHeight, screen.B),
				C = ScreenToRaster(canvasWidth, canvasHeight, screen.C)
			};
		}

		#endregion
	}
}
