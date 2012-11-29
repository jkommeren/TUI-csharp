//using System;
//using System.Diagnostics;
//using Emgu.CV;
//using Emgu.CV.CvEnum;
//using Emgu.CV.Structure;
//using System.Threading;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;
using System.Runtime.InteropServices;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Threading;

namespace TangibleUISharp
{
	public partial class AppX3 : Form
	{
		int hue = 0;
		int sat = 0;
		int val = 0;
		int maxHue = 255;
		int maxSat = 255;
		int maxVal = 255;
		double ContourAccuracy = 0.0005;
		int minAreaSize = 2000;
		int maxAreaSize = 20000;
		int IDcounter = 0;
		Rectangle projectionArea;
		List<IdentifiedObject> identifiedObjects;
		bool recentlyChanged = false;
		bool showWindow = true;
		bool calibrateSwitch = false;
		int maxPixTravel = 20; // higher value: less accurate, but more likely to pick up faster moving objects
		bool backwards = false; // For display animation: determines which way the circle is moving
		float xfCirc = 0f; // x of animation circle
		float yfCirc = 0f; // y of animation circle

		Stopwatch watch; // stopwatch for framerate counter
		int framecounter = 0;
		// counter for framerate



		Capture _capture = null; // Webcam!

		bool IsPlaformCompatable ()
		{
			int clrBitness = Marshal.SizeOf (typeof(IntPtr)) * 8;
			if (clrBitness != CvInvoke.UnmanagedCodeBitness) {
				MessageBox.Show (String.Format ("Platform mismatched: CLR is {0} bit, C++ code is {1} bit."
					+ " Please consider recompiling the executable with the same platform target as C++ code.",
               clrBitness, CvInvoke.UnmanagedCodeBitness)
				);
				return false;
			}
			return true;

		}

		public AppX3 ()
		{
			Console.WriteLine ("Welcome to TUI!");
			InitializeComponent ();
			identifiedObjects = new List<IdentifiedObject> ();
			// create second thread with display animation

			if (IsPlaformCompatable ()) {
				Thread t2 = new Thread (new ThreadStart (SecondThread));
				t2.Priority = System.Threading.ThreadPriority.Highest;
				t2.Start ();
				StartCapture ();

			}
            hueMin.Text = trackHueMin.Value.ToString();
            satMin.Text = trackSatMin.Value.ToString();
            sizeMax.Text = trackSizeMax.Value.ToString();
            valMax.Text = trackValMax.Value.ToString();
            hueMax.Text = trackHueMax.Value.ToString();
            satMax.Text = trackSatMax.Value.ToString();
            sizeMin.Text = trackSizeMin.Value.ToString();
            valMin.Text = trackValMin.Value.ToString();


		}

		public void StartCapture ()
		{
			//CvInvoke.cvNamedWindow("Capture");
			try {
				_capture = new Capture (1);


			} catch (NullReferenceException excpt) {
				Console.Out.WriteLine (excpt.Message);
				_capture = new Capture (0);

			} finally {
				watch = Stopwatch.StartNew ();
				_capture.Start ();
				Application.Idle += ProcessFrame;


			}
		}

		// this is for object detection
		private void ProcessFrame (object sender, EventArgs arg)
		{
			Image<Bgr, Byte> frame = null; // the frame retrieved from camera
			//bool showWindow = true; // toggle whether or not to show the live feed (performance hit!)

			// variables you'll want to calibrate

			//int bluerange = 200;
			//int redrange = 200;
			//int greenrange = 100;

           

            

			try {
				frame = _capture.RetrieveBgrFrame ();
			} catch (Exception ax) {
				Console.WriteLine ("Image retrieval failed! quiting: " + ax.ToString ());
				return;
			}

			framecounter++; // counter for framerate

			// 30 frames have passed, time to print framerate!
			if (framecounter == 30) {
				//watch.Stop();
				double framerate = 30.0 / (Convert.ToDouble (watch.ElapsedMilliseconds) / 1000);
				if (showWindow)
					Console.WriteLine (framerate);
				framecounter = 0;

				watch.Stop ();
				watch.Reset ();
				watch.Start ();
				framecounter = 0;
			}

			Image<Hsv, Byte> frame_HSV = frame.Convert<Hsv, Byte> ();
			Image<Gray, Byte> frame_thresh;
			frame_thresh = frame_HSV.InRange (new Hsv (hue, sat, val), new Hsv (maxHue, maxSat, maxVal));

			//processedFrame = frame.Clone ().ThresholdBinary (new Bgr (redrange, greenrange, bluerange), new Bgr (255, 255, 255));

			using (MemStorage storage = new MemStorage()) {
				List<Contour<Point>> unidentifiedObjects = this.DetectObjects (storage, frame_thresh, ContourAccuracy, minAreaSize, maxAreaSize);
				//if (unidentifiedObjects.Count == 0) Console.WriteLine ("no objects detected");
				foreach (Contour<Point> contour in unidentifiedObjects) {
					long timeinms = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					bool found = false;
					// calculate center
					double rectHeight = (double)contour.BoundingRectangle.Height;
					double rectWidth = (double)contour.BoundingRectangle.Width;
										
					// find center
					double dy = 0.5 * rectHeight + contour.BoundingRectangle.Y;
					double dx = 0.5 * rectWidth + contour.BoundingRectangle.X;
				
					if (identifiedObjects.Count > 0) 
					{ 
						//Console.WriteLine ("check 1a");
						foreach (IdentifiedObject io in identifiedObjects) {
						// calculate center point
						// accuracy is in pixels, 10 for now
						if (io.liesWithin ((int)dx, (int)dy, recentlyChanged, maxPixTravel, timeinms, contour.Area)) {
							found = true;
							//if (showWindow) Console.WriteLine("existing object");
							break;
						}
						}
					}
					//Console.WriteLine("check 2");
					// if not found, add object!
					if (!found){

						IdentifiedObject iox = this.IdentifyObject (contour, ContourAccuracy,timeinms);
						if (iox != null) 
						{
							if (showWindow) Console.WriteLine("new object detected");
							identifiedObjects.Add(iox);
						}
					}
					//else {if (showWindow)Console.WriteLine("existing object");}
				}
				if (recentlyChanged) recentlyChanged = false;
			}

			if (showWindow) {
				List<IdentifiedObject> toRemove = new List<IdentifiedObject>();
				foreach (IdentifiedObject io in identifiedObjects)
				{
					long timeinms = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					//Console.WriteLine (io.ToString());
					Point center = io.getPosition(timeinms);

					if(center.X == -1) 
					{
						toRemove.Add(io);
						//Console.WriteLine("ready for removal");
					}
					else {
					PointF centerf = new PointF((float) center.X, (float) center.Y);
					CircleF circlef = new CircleF(centerf,5);
					if ( io.getShape() == "circle") frame.Draw(circlef,io.getColor(),5);
					else if (io.getShape() == "square") frame.Draw (circlef, io.getColor(),10);
						else  frame.Draw (circlef,  io.getColor(), 1);
					}
				}
				foreach (IdentifiedObject io in toRemove)
				{
					identifiedObjects.Remove (io);
				}
				CvInvoke.cvShowImage ("Capture", frame);
				CvInvoke.cvWaitKey (1);
			}


		}

		private List<Contour<Point>> DetectObjects (MemStorage storage, Image<Gray, Byte> processedFrame, double ContourAccuracy, int minAreaSize, int maxAreaSize)
		{
			Contour<Point> biggestContour = null;

			// clear filtered list
			List<Contour<Point>> filteredUnidentifiedObjects = new List<Contour<Point>> ();


			// get all contours, and process each 
			//for (Contour<Point> contours = processedFrame.Convert<Gray, byte>().FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext) {


			for (Contour<Point> contours = processedFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext) {
				Contour<Point> currentContour = contours.ApproxPoly (contours.Perimeter * ContourAccuracy, storage);
				//find biggest blob
				if (biggestContour == null) {
					biggestContour = currentContour;
				} else if (biggestContour.Area < currentContour.Area) {
					biggestContour = currentContour;
				}
				// Alright this bit has been redone to be extra careful with the values it processes, so if things go wrong
				// it's easier to debug

				// if detected size is within limits
				if (currentContour.Area > minAreaSize && currentContour.Area < maxAreaSize) {

					filteredUnidentifiedObjects.Add (currentContour);

				}
			}
			if (calibrateSwitch) {
				if (biggestContour != null) {

					calibrateSwitch = false;
					projectionArea = biggestContour.BoundingRectangle;
					Console.WriteLine ("Calibrated to: " + projectionArea.Width + " " + projectionArea.Height + "! ");
				} else {
					Console.WriteLine ("No contour detected atm! Try again :D");
				}
			}
			return filteredUnidentifiedObjects;
		}

		private IdentifiedObject IdentifyObject (Contour<Point> contour, double ContourAccuracy, long timeinms)
		{
			//PointF center;

			//int boundingWidth = contour.BoundingRectangle.Width;
			//double dx = Double.Parse (boundingWidth.ToString ());

			//			// horizontal coordinates of center point of area
			//			dx = dx * 0.5;
			//			dx = dx + contour.BoundingRectangle.X;
			//int boundingHeight = contour.BoundingRectangle.Height;
			bool circ = false;
			bool rect = false;
			string strheight = contour.GetMinAreaRect ().size.Height.ToString ();
			string strwidth = contour.GetMinAreaRect ().size.Width.ToString ();

			// set accuracy for shape detection, since it depends on contouracc, we'll do it as follows
			//int boundaries = 6;
			//			if (ContourAccuracy >= 0.005) {
			//				boundaries = 8;
			//			}


			// Example: the bounding box can be 90 degrees rotated from the bounding area square
			// to make sure the right edges are compared the ratio is used to widen the range when needed
			// the following values resulted in a 99.9% true negative when fully detected, and some colours 100% true positive
			double ratio = double.Parse (strheight) / double.Parse (strwidth);
			if (ratio < 1)
				ratio = 1 / ratio;
			ratio = ratio * ratio * 1.05;
			double rectHeight = (double)contour.BoundingRectangle.Height;
			double rectWidth = (double)contour.BoundingRectangle.Width;
							
			// find center
			double dy = 0.5 * rectHeight + contour.BoundingRectangle.Y;
			double dx = 0.5 * rectWidth + contour.BoundingRectangle.X;

			// old: if (double.Parse(strheight) <= contour.BoundingRectangle.Height + 2 && int.Parse(strheight) >= contour.BoundingRectangle.Height - boundaries)
			if (double.Parse (strheight) <= rectHeight * ratio && double.Parse (strheight) >= rectHeight / ratio) {
				if (double.Parse (strwidth) <= rectWidth * ratio && double.Parse (strwidth) >= rectWidth / ratio) {
					//								// is it non-elliptical?
					if (double.Parse (strwidth) * 0.9 < double.Parse (strheight) && (double.Parse (strwidth) * 1.1 > double.Parse (strheight))) {
						circ = true;
						//Console.WriteLine (contour.GetMinAreaRect ().size.Height.ToString ());
					} else {
						return null;
						//Console.WriteLine ("elliptical");
					}
				} else {
					//	Console.WriteLine ("width not a circle? float:" + currentContour.GetMinAreaRect ().size.Width.ToString () + " int: " + strwidth + " exp: " + currentContour.BoundingRectangle.Width.ToString ());						                   
				}
			} else {
				//	Console.WriteLine ("height not a circle? float:" + currentContour.GetMinAreaRect ().size.Height.ToString () + " int: " + strheight + " exp: " + currentContour.BoundingRectangle.Height.ToString ());

			}



			// sometimes pc would act up
			if (dy == 240) {
				// this usually happens when the variables aren't being refreshed properly, this happened with internal cam on laptop
				Console.WriteLine ("oops, this shouldnt occur often with the ps eye cam, you using the right one?");
				//break;

			}
			// float coordinates of center of area
			float xf;
			float yf;

			//Console.WriteLine ("doubley: " + dy);

			// convert values to floats
			float.TryParse (dx.ToString (), out xf);
			float.TryParse (dy.ToString (), out yf);
			//						if (contour == biggestContour) {
			//							center = new PointF (xf, yf);
			//						}
			//PointF center = new PointF (xf, yf);
			//CircleF circle = new CircleF (center2, float.Parse (contour.Area.ToString ()));
			//CircleF circle = new CircleF (center, 20f);

			// compare square footing of expected circle with contour area
			// calc average radius
			double avgr = (rectWidth + rectHeight) / 4;
			double total = Math.PI * avgr * avgr;

			//Console.WriteLine ("circle totalarea" + contour.Area.ToString ()+ " calculated area " + total.ToString());
			double area = Convert.ToDouble (contour.Area);

			if (circ) if (area > total * 1.02) {
				circ = false;
				rect = true;
				// I'm actually a square after all! just nearly vertical
			}


			// if object doesn't already exist create new one


			IDcounter++;
			IdentifiedObject io = new IdentifiedObject ((int)dx, (int)dy, Convert.ToDouble (contour.Area), circ, rect, IDcounter, timeinms);
			return io;
		}



		//
		//			#region display
		//			
		//			try {
		//				if (showWindow) {
		//					CvInvoke.cvShowImage ("Capture", processedFrame);
		//					CvInvoke.cvWaitKey (1);
		//				}
		//			} catch (Exception ez) {
		//				Console.WriteLine ("nothing found");
		//			}
		//			#endregion
		//		}
		// Thread for display animation
		Image<Gray, byte> animation;
		Image<Gray, byte> blank_animation;
		//timer;
		public void SecondThread ()
		{
			System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer (); // timer for Animation thread
			CvInvoke.cvNamedWindow ("Animation");

			timer.Interval = 20; // set it to 50Hz for now
			timer.Tick += Animation_tick;
			animation = new Image<Gray, byte> (200, 200, new Gray (0));
			blank_animation = animation.Clone ();
			timer.Start ();
		}

		private void Animation_tick (object sender, EventArgs e)
		{

			// primitive for now
			// creates 200 x 200 pixel picture, with a circle moving across it

			// circle has touched top edge last, going downwards
			if (!backwards) {
				xfCirc = xfCirc + 1.0f;
				yfCirc = yfCirc + 1.0f;
			}
            // circle has last touched the bottom edge, going upwards
            else {
				xfCirc -= 1.0f;
				yfCirc -= 1.0f;
			}



			if (xfCirc > 190f) {
				// reached the bottom!
				backwards = true;
				animation = blank_animation.Clone ();
			} else if (xfCirc < 10.0f) {
				// reached the top!;
				backwards = false;
				animation = blank_animation.Clone ();
			}
			PointF circleCenter = new PointF (xfCirc, yfCirc);
			CircleF circle = new CircleF (circleCenter, 20f);
			animation.Draw (circle, new Gray (255), 1);
			//Console.WriteLine(" circle drawn ");

			CvInvoke.cvShowImage ("Animation", animation);
			//Console.WriteLine(" hi ");
			///Thread.Sleep(10);
			int key = CvInvoke.cvWaitKey (10);
			if (key == 99) {
				calibrateSwitch = true;
			}

			if (key == 119) {
				showWindow = false;
			}

			//Console.WriteLine(" lo ");
		}

		private void trackHueMin_Scroll (object sender, EventArgs e)
		{
			hueMin.Text = trackHueMin.Value.ToString ();
			hue = trackHueMin.Value;
			this.recentlyChanged = true;
		}

		private void trackSatMin_Scroll (object sender, EventArgs e)
		{
			satMin.Text = trackSatMin.Value.ToString ();
			sat = trackSatMin.Value;
			this.recentlyChanged = true;
		}

		private void trackValMin_Scroll (object sender, EventArgs e)
		{
			valMin.Text = trackValMin.Value.ToString ();
			val = trackValMin.Value;
			this.recentlyChanged = true;
		}

		private void trackSizeMin_Scroll (object sender, EventArgs e)
		{
			sizeMin.Text = trackSizeMin.Value.ToString ();
			minAreaSize = trackSizeMin.Value * 100;
			this.recentlyChanged = true;
		}

		private void trackHueMax_Scroll (object sender, EventArgs e)
		{
			hueMax.Text = trackHueMax.Value.ToString ();
			maxHue = trackHueMax.Value;
			this.recentlyChanged = true;
		}

		private void trackSatMax_Scroll (object sender, EventArgs e)
		{
			satMax.Text = trackSatMax.Value.ToString ();
			maxSat = trackSatMax.Value;
			this.recentlyChanged = true;
		}

      
		private void trackValMax_Scroll (object sender, EventArgs e)
		{
			valMax.Text = trackValMax.Value.ToString ();
			maxVal = trackValMax.Value;
			this.recentlyChanged = true;
		}

		private void trackSizeMax_Scroll (object sender, EventArgs e)
		{
			sizeMax.Text = trackSizeMax.Value.ToString ();
			maxAreaSize = trackSizeMax.Value * 1000;
			this.recentlyChanged = true;
		}

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
			this.maxPixTravel = trackBar1.Value * 10;

        }


	}
}
