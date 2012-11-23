using System;

namespace TangibleUISharp
{
	public class IdentifiedObject
	{
		bool circle;
		double size;
		double locX;
		double locY;
		public IdentifiedObject (bool circle, double x, double y, double area)
		{
			this.circle=circle;
			this.locX=x;
			this.locY=y;
			this.size=area;
		}
	}
}

