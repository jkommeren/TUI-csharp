using System;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Threading;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TangibleUISharp
{
	class MainClass
	{

		static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer (); // timer for Animation thread
		static bool backwards = false ; // For display animation: determines which way the circle is moving
		static float xfCirc = 0f; // x of animation circle
		static float yfCirc = 0f; // y of animation circle

		static Stopwatch watch; // stopwatch for framerate counter
		static int framecounter = 0; // counter for framerate

		static Capture _capture = null; // Webcam!

		public static void Main (string[] args)
		{
			Console.WriteLine ("Welcome to TUI!");


			// create second thread with display animation
			Thread t2 = new Thread (new ThreadStart (SecondThread));
			t2.Priority = System.Threading.ThreadPriority.Highest; 
			t2.Start ();
			StartCapture ();

		}
		 
		static public void StartCapture ()
		{
			CvInvoke.cvNamedWindow("Capture");
			try {
				_capture = new Capture (1);
			

			} catch (NullReferenceException excpt) {	
				Console.Out.WriteLine (excpt.Message);
				_capture = new Capture (0);

			} finally {
				watch = Stopwatch.StartNew ();
				Application.Idle += ProcessFrame;
				_capture.Start ();

			}
		}

		// this is for object detection
		private static void ProcessFrame (object sender, EventArgs arg)
		{
			Image<Bgr, Byte> frame = null; // the frame retrieved from camera
			bool showWindow = true; // toggle whether or not to show the live feed (performance hit!)

			// variables you'll want to calibrate
			
			int bluerange = 200;
			int redrange = 200;
			int greenrange = 100;
			double ContourAccuracy = 0.0005;
			int minAreaSize = 2000;
			int maxAreaSize = 20000;

			try {
				frame = _capture.RetrieveBgrFrame ();
			} catch (Exception ax) {
				Console.WriteLine ("Image retrieval failed! quiting: " + ax.ToString ());
				return;
			}
		
			framecounter ++; // counter for framerate

			// 30 frames have passed, time to print framerate!
			if (framecounter == 30) {
				//watch.Stop();
				double framerate = 30.0 / (Convert.ToDouble (watch.ElapsedMilliseconds) / 1000);
				Console.WriteLine (framerate);
				framecounter = 0;
			
				watch.Stop ();
				watch.Reset ();
				watch.Start ();
				framecounter = 0;
			}


			Image<Bgr, Byte> processedFrame = frame.Clone ().ThresholdBinary (new Bgr (redrange, greenrange, bluerange), new Bgr (255, 255, 255));

			#region blob detection
			
			PointF center;
			Contour<Point> biggestContour = null;
			using (MemStorage storage = new MemStorage()) {
				// get all contours, and process each 
				for (Contour<Point> contours = processedFrame.Convert<Gray, byte>().FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext) {
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
					
						int boundingWidth = currentContour.BoundingRectangle.Width;
						double dx = Double.Parse (boundingWidth.ToString ());

						// horizontal coordinates of center point of area
						dx = dx * 0.5;
						dx = dx + currentContour.BoundingRectangle.X;
						int boundingHeight = currentContour.BoundingRectangle.Height;
						bool circ = false;

						string strheight = currentContour.GetMinAreaRect ().size.Height.ToString ();
						string strwidth = currentContour.GetMinAreaRect ().size.Width.ToString ();
						
						// set accuracy for shape detection, since it depends on contouracc, we'll do it as follows
						int boundaries = 6;
						if (ContourAccuracy >= 0.005) {
							boundaries = 8;
						}
						
						
						// Example: the bounding box can be 90 degrees rotated from the bounding area square
						// to make sure the right edges are compared the ratio is used to widen the range when needed
						// the following values resulted in a 99.9% true negative when fully detected, and some colours 100% true positive
						double ratio = double.Parse (strheight) / double.Parse (strwidth);
						if (ratio < 1)
							ratio = 1 / ratio;
						ratio = ratio * ratio * 1.05;

							
						// old: if (double.Parse(strheight) <= currentContour.BoundingRectangle.Height + 2 && int.Parse(strheight) >= currentContour.BoundingRectangle.Height - boundaries)
						if (double.Parse (strheight) <= double.Parse (currentContour.BoundingRectangle.Height.ToString ()) * ratio && double.Parse (strheight) >= double.Parse (currentContour.BoundingRectangle.Height.ToString ()) / ratio) {
							if (double.Parse (strwidth) <= double.Parse (currentContour.BoundingRectangle.Width.ToString ()) * ratio && double.Parse (strwidth) >= double.Parse (currentContour.BoundingRectangle.Width.ToString ()) / ratio) {
//								// is it non-elliptical?


								if (double.Parse (strwidth) * 0.9 < double.Parse (strheight) && (double.Parse (strwidth) * 1.1 > double.Parse (strheight))) {
									circ = true;
									//Console.WriteLine (currentContour.GetMinAreaRect ().size.Height.ToString ());
								} else {
									//Console.WriteLine ("elliptical");
								}
							} else {
								//	Console.WriteLine ("width not a circle? float:" + currentContour.GetMinAreaRect ().size.Width.ToString () + " int: " + strwidth + " exp: " + currentContour.BoundingRectangle.Width.ToString ());
							                   
							}
						} else {
							//	Console.WriteLine ("height not a circle? float:" + currentContour.GetMinAreaRect ().size.Height.ToString () + " int: " + strheight + " exp: " + currentContour.BoundingRectangle.Height.ToString ());
							                   
						}


						// find center
						double dy = Convert.ToDouble (boundingHeight);
						dy = dy * 0.5;
						dy = dy + currentContour.BoundingRectangle.Y; 


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
						float.TryParse (dx.ToString (), out xf);
						float.TryParse (dy.ToString (), out yf);
						if (currentContour == biggestContour) {
							center = new PointF (xf, yf);
						}
						PointF center2 = new PointF (xf, yf);
						//CircleF circle = new CircleF (center2, float.Parse (currentContour.Area.ToString ()));
						CircleF circle = new CircleF (center2, 20f);
						int avgr = (currentContour.BoundingRectangle.Width + currentContour.BoundingRectangle.Height) / 4;
						double total = Math.PI * Convert.ToDouble (avgr) * Convert.ToDouble (avgr);
						
						//Console.WriteLine ("circle totalarea" + currentContour.Area.ToString ()+ " calculated area " + total.ToString());
						double area = Convert.ToDouble (currentContour.Area);
						double mintotalarea = (total * 0.9);
						double maxtotalarea = (total * 1.05);
					
						if (area > total * 1.02) {
							circ = false;
							// I'm actually a square after all! just nearly vertical
						}
		
						processedFrame.Draw (currentContour, new Bgr (Color.Blue), 5);
						if (!circ)
							// this object is probably not a circle / round shape
							processedFrame.Draw (circle, new Bgr (Color.Purple), 5);
						if (circ)
							// this object should be a circle
							processedFrame.Draw (circle, new Bgr (Color.White), 5);
					}
				}

			}
#endregion



			#region display
			
			try {
				if (showWindow) {
					CvInvoke.cvShowImage ("Capture", processedFrame);
					CvInvoke.cvWaitKey (1);
				}
			} catch (Exception ez) {
				Console.WriteLine ("nothing found");
			}
			#endregion
		}
		// Thread for display animation
		public static void SecondThread ()
		{
			CvInvoke.cvNamedWindow ("Animation");
			timer.Interval = 20; // set it to 50Hz for now
			timer.Tick += Animation_tick;
		}

		private static void Animation_tick (object sender, EventArgs e)
		{

			// primitive for now
			// creates 200 x 200 pixel picture, with a circle moving across it
			Image<Bgr, byte> img = new Image<Bgr, byte> (200, 200, new Bgr (255, 0, 0));

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
			} else if (xfCirc < 10.0f) {
				// reached the top!;
				backwards = false;
			}
			PointF circleCenter = new PointF (xfCirc, yfCirc);
			CircleF circle = new CircleF (circleCenter, 20f);
			img.Draw (circle, new Bgr (255, 255, 0), 10);
			CvInvoke.cvShowImage ("Animation", img);
			CvInvoke.cvWaitKey (1);

		}
	}
}
