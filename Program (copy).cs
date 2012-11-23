//----------------------------------------------------------------------------
//  Copyright (C) 2004-2012 by EMGU. All rights reserved.       
//----------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using Gtk;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

namespace HelloWorld
{
	class Program
	{
		static Capture _capture = null;
		static int counter = 0;
		static bool refframe = true;
		static Image<Bgr, Byte> reff;
		static String win1 = "Test Window"; //The name of the window
		static	String win2 = "Window 2";
		static	String win3 = "Window 3";
		static	String win4 = "Window 4";

		//static bool _captureInProgress;
		static void Main (string[] args)
		{
			if (!IsPlaformCompatable ())
				return;
			Application.Run (new Form1());
			return;
			//Application.Init ();
			
			//Window1 window = new Window1();
			//window.Show ();
			//Application.Run ();
			//Application.Run (new Window1());
			//CvInvoke.cvNamedWindow (win1); //Create the window using the specific nam			{
			//CvInvoke.cvNamedWindow (win2); //Create the window using the specific nam			{
			//CvInvoke.cvNamedWindow (win3); //Create the window using the specific nam			{
			CvInvoke.cvNamedWindow (win4); //Create the window using the specific nam			{
			
			try {
				//Console.Write ("choose capture channel (0~3)");
				//string key = Console.ReadLine();
				//char charz = key.KeyChar;
				//int integer = Convert.ToInt32 (charz);
				_capture = new Capture (3);
				_capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_AUTO_EXPOSURE, 0);
				//_capture.SetCaptureProperty(CAP_PROP.
				_capture.ImageGrabbed += ProcessFrame;


				//Image<Bgr, Byte> img = new Image<Bgr, byte>(400, 200, new Bgr(255, 0, 0)); //Create an image of 400x200 of Blue color
				// MCvFont f = new MCv
				_capture.Start ();
			} catch (NullReferenceException excpt) {
				//MessageBox.Show(excpt.Message);
				
				Console.Out.WriteLine (excpt.Message);
				_capture = new Capture (1);
				_capture.ImageGrabbed += ProcessFrame;

				_capture.Start ();
			}

       
			CvInvoke.cvDestroyWindow (win1); //Destory the window
			//	CvInvoke.cvReleaseCapture(_capture);
		}

		static void ProcessFrame (object sender, EventArgs arg)
		{
			//Console.WriteLine("captureX");
			Image<Bgr, Byte> frame = _capture.RetrieveBgrFrame ();
			//if (frame.Equals(null)) return;
			
			counter ++;
			int circlecount = 0;
			if (counter == 900)
				Console.Out.WriteLine ("900!");
		
			if (refframe) {
				refframe = false;
				reff = frame.Clone ();
			}
			Image<Bgr, Byte> thresh2 = frame.Clone ();

			Image<Bgr, Byte> dif = reff.AbsDiff (frame);
			//Image<Bgr, Byte> thresh = dif.Clone ().ThresholdBinary (new Bgr (200, 200, 200), new Bgr (255, 255, 255));
			
		   thresh2 = frame.Clone ().ThresholdBinary(new Bgr (200,100,200), new Bgr(255,255,255));
			//thresh2 = frame.Clone ().ThresholdBinary(new Bgr (0,0,0), new Bgr(200,100,200));
			//thresh2 = thresh2.ThresholdBinary (new Bgr (200, 200, 200), new Bgr (255, 255, 255));
			//captureImageBox.Image = frame;
			
			//Image<Gray, Byte> gray = frame.InRange (new Bgr(0,0,175), new Bgr(100,100,256)).Convert<Gray, Byte> ().PyrDown ().PyrUp ();



			#region blob detection
			int maxArea = 100;
			
			PointF center;
			Contour<Point> biggestContour = null;// = new Contour<Point>();
			using (MemStorage storage = new MemStorage()) {
				//thresh2.Convert<Gray, byte> ().FindContours ();

				for (Contour<Point> contours = thresh2.Convert<Gray, byte>().FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext) {
					Contour<Point> currentContour = contours.ApproxPoly (contours.Perimeter * 0.0005, storage);
//find biggest blob
					//BiggestContour = currentContour;
					if (biggestContour == null)
					{
						biggestContour = currentContour;
					}
					if (biggestContour.Area < currentContour.Area)
					{
						biggestContour = currentContour;
					}
					// Alright this bit has been redone to be extra careful with the values it processes, so if things go wrong
					// it's easier to debug
					
					int blax = currentContour.BoundingRectangle.Width;
					double dx = Double.Parse (blax.ToString ());
					//Console.WriteLine ("doublex: " + dx);
					dx = dx * 0.5;
			
					dx = dx + currentContour.BoundingRectangle.X;
					int y = currentContour.BoundingRectangle.Height;
					double dy = Convert.ToDouble (y);
					dy = dy * 0.5;
					dy = dy + currentContour.BoundingRectangle.Y;
					if (dy == 240) {
						// this usually happens when the variables aren't being refreshed properly, this happened with internal cam on laptop
						Console.WriteLine ("oops, this shouldnt occur often with the ps eye cam, you using the right one?");
						//break;
				
					}
					float xf;
					float yf;
					
				
					//Console.WriteLine ("doubley: " + dy);
					float.TryParse (dx.ToString (), out xf);
					float.TryParse (dy.ToString (), out yf);
					if (currentContour == biggestContour)
					{
					center = new PointF (xf, yf);
					}
					circlecount++;
					
//					if (currentContour.Area < maxArea) {
//						Console.WriteLine ("Area:"+ maxArea);
//						maxArea = (int)currentContour.Area;
//						BiggestContour = currentContour;

					//}
					
					if (currentContour.Area > 1000 && currentContour.Area < 20000)
					{
						PointF center2 = new PointF (xf, yf);
						//CircleF circle = new CircleF (center2, float.Parse (currentContour.Area.ToString ()));
						CircleF circle = new CircleF(center2,20f);
						thresh2.Draw (currentContour, new Bgr (Color.Blue), 5);
						thresh2.Draw (circle, new Bgr (Color.Purple), 5);
					}
				}
				//if (xf > 0 && yf > 0) { 
				if (biggestContour != null)
				{
				CircleF circle = new CircleF (center, float.Parse (biggestContour.Area.ToString ()));

						//thresh2.Draw (currentContour,new Bgr(Color.Red), 1);
						thresh2.Draw (biggestContour, new Bgr (Color.Black), 5);
						//thresh2.Draw (circle, new Bgr (Color.Green), 1);
					
				CvInvoke.cvWaitKey (1);
				}

				//storage.CreateChildMemStorage();
			}
			//BiggestContour.BoundingRectangle
#endregion



			#region circle detection
			//CircleF[] circles = null;
			//gray = gray.SmoothGaussian(9);
		double cannyThreshold = 120.0;
			//double circleAccumulatorThreshold = 120;
//			try {
//			//Stopwatch watch = Stopwatch.StartNew();
//	
//			circles = gray.HoughCircles(
//				new Gray(100),
//				new Gray(50),
//				2.0, //Resolution of the accumulator used to detect centers of the circles
//				20.0, //min distance
//				1, //min radius
//				400 //max radius
//				)[0]; //Get the circles from the first channel
//			}
//			catch (Exception x)
//			{
//				
//			}
//			watch.Stop();
//			msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
#endregion
			

			#region Canny and edge detection
			//watch.Reset(); watch.Start();
			//double cannyThresholdLinking = 120.0;
			//Image<Gray, Byte> cannyEdges = gray.Canny (cannyThreshold, cannyThresholdLinking);
			try {
//				LineSegment2D[] lines = cannyEdges.HoughLinesBinary (
//				1, //Distance resolution in pixel-related units
//				Math.PI / 45.0, //Angle resolution measured in radians.
//				20, //threshold
//				30, //min Line width
//				10 //gap between lines
//				) [0]; //Get the lines from the first channel
//				//watch.Stop();
//				//msgBuilder.Append(String.Format("Canny & Hough lines - {0} ms; ", watch.ElapsedMilliseconds));
#endregion
//				Image<Bgr, Byte> lineImage = reff.Clone ();
//				//foreach
//				if (circles != null)
//				{
//				foreach (CircleF circle in circles)
//					lineImage.Draw (circle, new Bgr (Color.Red), 10);
//				}
				//captureImageBox.Image = lineImage;
				//CvInvoke.cvShowImage (win1, lineImage);
				//CvInvoke.cvShowImage (win2, gray);
				//CvInvoke.cvShowImage (win3, dif);
				CvInvoke.cvShowImage (win4, thresh2);
				CvInvoke.cvWaitKey (1);
			} catch (Exception ez) {
				Console.WriteLine ("nothing found");
			}
			//watch.Stop();
			//msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
            

			
		}

		/// <summary>
		/// Check if both the managed and unmanaged code are compiled for the same architecture
		/// </summary>
		/// <returns>Returns true if both the managed and unmanaged code are compiled for the same architecture</returns>
		static bool IsPlaformCompatable ()
		{
			int clrBitness = Marshal.SizeOf (typeof(IntPtr)) * 8;
			if (clrBitness != CvInvoke.UnmanagedCodeBitness) {
				MessageBox.Show (String.Format ("Platform mismatched: CLR is {0} bit, C++ code is {1} bit."
               + " Please consider recompiling the executable with the same platform target as C++ code.",
               clrBitness, CvInvoke.UnmanagedCodeBitness));
				return false;
			}
			return true;
		}
	}
}
