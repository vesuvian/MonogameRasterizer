using System;
using Microsoft.Xna.Framework;
using MonogameRasterizer.Utils;

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

		public void DrawLine(Vector2 a, Vector2 b, Color color)
		{
			DrawLine((int)a.X, (int)a.Y, (int)b.X, (int)b.Y, color);
		}

		public void DrawLine(int x0, int y0, int x1, int y1, Color color)
		{
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

		public void DrawPixel(Vector2 point, Color color)
		{
			DrawPixel((int)point.X, (int)point.Y, color);
		}

		public void DrawPixel(int x, int y, Color color)
		{
			if (x < 0 || y < 0 || x >= Bounds.Width || y >= Bounds.Height)
				return;

			int index = y * Bounds.Width + x;
			Pixels[index] = color.PackedValue;
		}

		public Vector2 CanvasToScreen(Vector3 point)
		{
			return new Vector2(point.X / -point.Z,
			                   point.Y / -point.Z);
		}

		public Vector2 ScreenToRaster(float canvasWidth, float canvasHeight, Vector2 screen)
		{
			Vector2 ndc = ScreenToNdc(canvasWidth, canvasHeight, screen);
			return NdcToRaster(ndc);
		}

		public Vector2 ScreenToNdc(float canvasWidth, float canvasHeight, Vector2 screen)
		{
			return new Vector2((screen.X + canvasWidth / 2.0f) / canvasWidth,
			                   (screen.Y + canvasHeight / 2.0f) / canvasHeight);
		}

		public Vector2 NdcToRaster(Vector2 ndc)
		{
			return new Vector2(ndc.X * Bounds.Width,
			                   (1 - ndc.Y) * Bounds.Height);
		}
	}
}
