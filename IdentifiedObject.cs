using System;
using Emgu.CV.Structure;
using System.Drawing;

namespace TangibleUISharp
{
	public class IdentifiedObject
	{
		bool _circle;
		bool _square;
		double size;
		int locX;
		int locY;
		int ID = 0;
		bool[] circleOccurences = new bool[50];
		bool[] squareOccurences = new bool[50];
		double[] measuredAreas = new double[50];
		int arrayPosition = 0;
		long lastSeen = 0;
		bool isnew = true;
		long timeRenewed = -1;
		double sizeAcc = 0.80;
	Bgr color = new Bgr(0,0,0);
		MCvBox2D boundingBox;

		public IdentifiedObject (int x, int y, double areaSize, bool isCircle, bool isSquare, int ID, long curTime)
		{
			timeRenewed = curTime;
			lastSeen = curTime;
			this._circle = isCircle;
			this._square = isSquare;
			this.locX = x;
			this.locY = y;
			this.size = areaSize;
			this.ID = ID;
			// generate random color

			Random random = new Random();
			int maxValue = 255;
			this.color = new Bgr(random.Next (maxValue),random.Next (maxValue),random.Next (maxValue));
		}

		public bool isNew ()
		{
			// if true is returned, processing will have to continue, and do an "upgrade shape" afterwards
			return isnew;
		}

		public bool liesWithin (int x, int y, bool recentlyChanged, int accuracy, long curTime, double curSize)
		{
			if (recentlyChanged || isnew) {

				//be alot less sensitive with x and y
				// do a size check only when this is not the case

				if (locX - (2 * accuracy) < x && locX + (accuracy * 2) > x) {
					if (locY - (2 * accuracy) < y && locY + (accuracy * 2) > y) {
						// rough method now, perhaps we can smooth this out in the future
						locX = x;
						locY = y;
						lastSeen = curTime;
						size = curSize;
						// if x and y are within area
						// adjust x and y
						// update lastSeen
						if (!isnew) {
					// wipe data
					isnew = true;
							// make sure this part is triggered for the next couple of frames
					timeRenewed = curTime;
				}

						return  true;
					}
					return false;

				} else {
					return false;
				}

			} else if (locX - accuracy < x && locX + accuracy > x) {
				if (locY - accuracy < y && locY + accuracy > y) {
					if (curSize > sizeAcc * size && curSize < size / sizeAcc)
					{
					// rough method now, perhaps we can smooth this out in the future
					locX = x;
					locY = y;
					lastSeen = curTime;
					// if x and y are within area
					// adjust x and y
					// update lastSeen
					return true;
					}
				}
				return false;
			} else {
				return false;
			}
		}

		private bool mostOccurred (bool[] array)
		{
			int trueCounter = 0;
			int falseCounter = 0;
			int i = 0;
			while (i < 50) {
				if (array [i] == null)
					break;
				else if (array [i]) {
					trueCounter ++;
				} else {
					falseCounter ++;
				}

			}

			return (trueCounter > falseCounter);
		}

		public bool updateShape (double areaSize, bool isCircle, bool isSquare)
		{
			if (arrayPosition < 50) {
				measuredAreas [arrayPosition] = areaSize;
				circleOccurences [arrayPosition] = isCircle;
				squareOccurences [arrayPosition] = isSquare;

				if (this.mostOccurred (circleOccurences))
					this._circle = true;
				else if (this.mostOccurred (squareOccurences))
					this._square = true;
				int j = 0;
				double totalArea = 0;
				while (j < arrayPosition) {
					j++;
					totalArea += areaSize;
				}
				if (arrayPosition != 0)
					this.size = totalArea / arrayPosition;
				else
					this.size = areaSize;
			
				// calculate median etc.
				arrayPosition++;
				return true;
			}
			// got plenty of data already! no need to update position
			else
				return false;
		}

		public bool updateBoundingBox (MCvBox2D box)
		{
			// this is for determining orientation of square objects
			// we'll get this done in M5 or later
			this.boundingBox = box;
			return true;
		}

		public MCvBox2D getBoundingBox ()
		{
			// we'll get this done in M5 or later
			return boundingBox;
		}

		public Bgr getColor()
		{
			return color;
		}

		public Point getPosition (long curTime)
		{
			if (curTime - timeRenewed > 1000) {
				isnew = false;
			}
			// if seen in the last 1 second
			if (curTime - lastSeen < 1000) {
				return new Point (locX, locY);
			} else {
				//Console.WriteLine ("(internal) marked for removal");
				return new Point(-1,-1);
			}
		}

		public override string ToString ()
		{
			return (locX + "=x," + locY + "=y,circle=" + _circle + ", square=" +_square);
		}

		public string getShape ()
		{
			if (_circle)
				return "circle";
			else if (_square)
				return "square";
			else
				return "different kind of object";
		}
	}
}

