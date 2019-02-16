using System.Collections.Generic;

namespace MonogameRasterizer.Actors
{
	public interface IGeometry
	{
		IEnumerable<Triangle> GetTriangles();
	}
}