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
//using Emgu.CV.UI;
using System.Diagnostics;
using System.Threading;

namespace TangibleUISharp
{
    public partial class AppX2 : Form
    {

        bool backwards = false; // For display animation: determines which way the circle is moving
        float xfCirc = 0f; // x of animation circle
        float yfCirc = 0f; // y of animation circle

        Stopwatch watch; // stopwatch for framerate counter
        int framecounter = 0;
        private TrackBar trackHueMin;
        private TrackBar trackSatMin;
        private TrackBar trackValMin;
        private TrackBar trackHueMax;
        private TrackBar trackSatMax;
        private TrackBar trackValMax;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TrackBar trackSizeMin;
        private TrackBar trackSizeMax; // counter for framerate



        Capture _capture = null; // Webcam!

        bool IsPlaformCompatable()
        {
            int clrBitness = Marshal.SizeOf(typeof(IntPtr)) * 8;
            if (clrBitness != CvInvoke.UnmanagedCodeBitness)
            {
                MessageBox.Show(String.Format("Platform mismatched: CLR is {0} bit, C++ code is {1} bit."
                    + " Please consider recompiling the executable with the same platform target as C++ code.",
               clrBitness, CvInvoke.UnmanagedCodeBitness)
                );
                return false;
            }
            return true;

        }

        public AppX2()
        {
            Console.WriteLine("Welcome to TUI!");
            //Application.Init ();

            // create second thread with display animation

            if (IsPlaformCompatable())
            {
                Thread t2 = new Thread(new ThreadStart(SecondThread));
                t2.Priority = System.Threading.ThreadPriority.Highest;
                t2.Start();
                StartCapture();

            }

        }

        public void StartCapture()
        {
            //CvInvoke.cvNamedWindow("Capture");
            try
            {
                _capture = new Capture(1);


            }
            catch (NullReferenceException excpt)
            {
                Console.Out.WriteLine(excpt.Message);
                _capture = new Capture(0);

            }
            finally
            {
                watch = Stopwatch.StartNew();
                _capture.Start();
                Application.Idle += ProcessFrame;


            }
        }

        // this is for object detection
        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> frame = null; // the frame retrieved from camera
            bool showWindow = true; // toggle whether or not to show the live feed (performance hit!)

            // variables you'll want to calibrate

            //int bluerange = 200;
            //int redrange = 200;
            //int greenrange = 100;

            int hue = 0;
            int sat = 0;
            int val = 0;
            int maxHue = 255;
            int maxSat = 255;
            int maxVal = 255;

            double ContourAccuracy = 0.0005;
            int minAreaSize = 2000;
            int maxAreaSize = 20000;

            try
            {
                frame = _capture.RetrieveBgrFrame();
            }
            catch (Exception ax)
            {
                Console.WriteLine("Image retrieval failed! quiting: " + ax.ToString());
                return;
            }

            framecounter++; // counter for framerate

            // 30 frames have passed, time to print framerate!
            if (framecounter == 30)
            {
                //watch.Stop();
                double framerate = 30.0 / (Convert.ToDouble(watch.ElapsedMilliseconds) / 1000);
                Console.WriteLine(framerate);
                framecounter = 0;

                watch.Stop();
                watch.Reset();
                watch.Start();
                framecounter = 0;
            }

            Image<Hsv, Byte> frame_HSV = frame.Convert<Hsv, Byte>();
            Image<Gray, Byte> frame_thresh;
            frame_thresh = frame_HSV.InRange(new Hsv(hue, sat, val), new Hsv(maxHue, maxSat, maxVal));

            //processedFrame = frame.Clone ().ThresholdBinary (new Bgr (redrange, greenrange, bluerange), new Bgr (255, 255, 255));

            using (MemStorage storage = new MemStorage())
            {
                List<Contour<Point>> unidentifiedObjects = this.DetectObjects(storage, frame_thresh, ContourAccuracy, minAreaSize, maxAreaSize);

                List<IdentifiedObject> identifiedObjects = new List<IdentifiedObject>();
                foreach (Contour<Point> contour in unidentifiedObjects)
                {
                    identifiedObjects.Add(this.IdentifyObject(contour, ContourAccuracy));
                }
            }
            if (showWindow)
            {
                CvInvoke.cvShowImage("Capture", frame);
                //cvlib.
                CvInvoke.cvWaitKey(1);
            }


        }

        private List<Contour<Point>> DetectObjects(MemStorage storage, Image<Gray, Byte> processedFrame, double ContourAccuracy, int minAreaSize, int maxAreaSize)
        {
            Contour<Point> biggestContour = null;

            // clear filtered list
            List<Contour<Point>> filteredUnidentifiedObjects = new List<Contour<Point>>();


            // get all contours, and process each 
            //for (Contour<Point> contours = processedFrame.Convert<Gray, byte>().FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext) {


            for (Contour<Point> contours = processedFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext)
            {
                Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * ContourAccuracy, storage);
                //find biggest blob
                if (biggestContour == null)
                {
                    biggestContour = currentContour;
                }
                else if (biggestContour.Area < currentContour.Area)
                {
                    biggestContour = currentContour;
                }
                // Alright this bit has been redone to be extra careful with the values it processes, so if things go wrong
                // it's easier to debug

                // if detected size is within limits
                if (currentContour.Area > minAreaSize && currentContour.Area < maxAreaSize)
                {

                    filteredUnidentifiedObjects.Add(currentContour);

                }
            }
            return filteredUnidentifiedObjects;
        }

        private IdentifiedObject IdentifyObject(Contour<Point> contour, double ContourAccuracy)
        {
            //PointF center;

            //int boundingWidth = contour.BoundingRectangle.Width;
            //double dx = Double.Parse (boundingWidth.ToString ());

            //			// horizontal coordinates of center point of area
            //			dx = dx * 0.5;
            //			dx = dx + contour.BoundingRectangle.X;
            //int boundingHeight = contour.BoundingRectangle.Height;
            bool circ = false;

            string strheight = contour.GetMinAreaRect().size.Height.ToString();
            string strwidth = contour.GetMinAreaRect().size.Width.ToString();

            // set accuracy for shape detection, since it depends on contouracc, we'll do it as follows
            //int boundaries = 6;
            //			if (ContourAccuracy >= 0.005) {
            //				boundaries = 8;
            //			}


            // Example: the bounding box can be 90 degrees rotated from the bounding area square
            // to make sure the right edges are compared the ratio is used to widen the range when needed
            // the following values resulted in a 99.9% true negative when fully detected, and some colours 100% true positive
            double ratio = double.Parse(strheight) / double.Parse(strwidth);
            if (ratio < 1)
                ratio = 1 / ratio;
            ratio = ratio * ratio * 1.05;
            double rectHeight = (double)contour.BoundingRectangle.Height;
            double rectWidth = (double)contour.BoundingRectangle.Width;

            // old: if (double.Parse(strheight) <= contour.BoundingRectangle.Height + 2 && int.Parse(strheight) >= contour.BoundingRectangle.Height - boundaries)
            if (double.Parse(strheight) <= rectHeight * ratio && double.Parse(strheight) >= rectHeight / ratio)
            {
                if (double.Parse(strwidth) <= rectWidth * ratio && double.Parse(strwidth) >= rectWidth / ratio)
                {
                    //								// is it non-elliptical?
                    if (double.Parse(strwidth) * 0.9 < double.Parse(strheight) && (double.Parse(strwidth) * 1.1 > double.Parse(strheight)))
                    {
                        circ = true;
                        //Console.WriteLine (contour.GetMinAreaRect ().size.Height.ToString ());
                    }
                    else
                    {
                        //Console.WriteLine ("elliptical");
                    }
                }
                else
                {
                    //	Console.WriteLine ("width not a circle? float:" + currentContour.GetMinAreaRect ().size.Width.ToString () + " int: " + strwidth + " exp: " + currentContour.BoundingRectangle.Width.ToString ());						                   
                }
            }
            else
            {
                //	Console.WriteLine ("height not a circle? float:" + currentContour.GetMinAreaRect ().size.Height.ToString () + " int: " + strheight + " exp: " + currentContour.BoundingRectangle.Height.ToString ());

            }


            // find center
            double dy = 0.5 * rectHeight + contour.BoundingRectangle.Y;
            double dx = 0.5 * rectWidth + +contour.BoundingRectangle.X;

            // sometimes pc would act up
            if (dy == 240)
            {
                // this usually happens when the variables aren't being refreshed properly, this happened with internal cam on laptop
                Console.WriteLine("oops, this shouldnt occur often with the ps eye cam, you using the right one?");
                //break;

            }
            // float coordinates of center of area
            float xf;
            float yf;

            //Console.WriteLine ("doubley: " + dy);

            // convert values to floats
            float.TryParse(dx.ToString(), out xf);
            float.TryParse(dy.ToString(), out yf);
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
            double area = Convert.ToDouble(contour.Area);

            if (area > total * 1.02)
            {
                circ = false;
                // I'm actually a square after all! just nearly vertical
            }
            IdentifiedObject io = new IdentifiedObject(circ, dx, dy, Convert.ToDouble(contour.Area));
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
        public void SecondThread()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer(); // timer for Animation thread
            CvInvoke.cvNamedWindow("Animation");

            timer.Interval = 20; // set it to 50Hz for now
            timer.Tick += Animation_tick;
            animation = new Image<Gray, byte>(200, 200, new Gray(0));
            blank_animation = animation.Clone();
            timer.Start();
        }

        private void Animation_tick(object sender, EventArgs e)
        {

            // primitive for now
            // creates 200 x 200 pixel picture, with a circle moving across it

            // circle has touched top edge last, going downwards
            if (!backwards)
            {
                xfCirc = xfCirc + 1.0f;
                yfCirc = yfCirc + 1.0f;
            }
            // circle has last touched the bottom edge, going upwards
            else
            {
                xfCirc -= 1.0f;
                yfCirc -= 1.0f;
            }



            if (xfCirc > 190f)
            {
                // reached the bottom!
                backwards = true;
                animation = blank_animation.Clone();
            }
            else if (xfCirc < 10.0f)
            {
                // reached the top!;
                backwards = false;
                animation = blank_animation.Clone();
            }
            PointF circleCenter = new PointF(xfCirc, yfCirc);
            CircleF circle = new CircleF(circleCenter, 20f);
            animation.Draw(circle, new Gray(255), 1);
            //Console.WriteLine(" circle drawn ");

            CvInvoke.cvShowImage("Animation", animation);
            //Console.WriteLine(" hi ");
            ///Thread.Sleep(10);
            int key = CvInvoke.cvWaitKey(1);

            //Console.WriteLine(" lo ");
        }

        private void InitializeComponent()
        {
            
            // 
            // trackHueMin
            // 
            this.trackHueMin.Location = new System.Drawing.Point(12, 12);
            this.trackHueMin.Name = "trackHueMin";
            this.trackHueMin.Size = new System.Drawing.Size(104, 45);
            this.trackHueMin.TabIndex = 0;
            this.trackHueMin.Scroll += new System.EventHandler(this.trackHueMin_Scroll);
            // 
            // trackSatMin
            // 
            this.trackSatMin.Location = new System.Drawing.Point(12, 47);
            this.trackSatMin.Name = "trackSatMin";
            this.trackSatMin.Size = new System.Drawing.Size(104, 45);
            this.trackSatMin.TabIndex = 1;
            this.trackSatMin.Scroll += new System.EventHandler(this.trackSatMin_Scroll);
            // 
            // trackValMin
            // 
            this.trackValMin.Location = new System.Drawing.Point(12, 80);
            this.trackValMin.Name = "trackValMin";
            this.trackValMin.Size = new System.Drawing.Size(104, 45);
            this.trackValMin.TabIndex = 2;
            this.trackValMin.Scroll += new System.EventHandler(this.trackValMin_Scroll);
            // 
            // trackHueMax
            // 
            this.trackHueMax.Location = new System.Drawing.Point(190, 12);
            this.trackHueMax.Name = "trackHueMax";
            this.trackHueMax.Size = new System.Drawing.Size(104, 45);
            this.trackHueMax.TabIndex = 3;
            this.trackHueMax.Scroll += new System.EventHandler(this.trackHueMax_Scroll);
            // 
            // trackSatMax
            // 
            this.trackSatMax.Location = new System.Drawing.Point(190, 44);
            this.trackSatMax.Name = "trackSatMax";
            this.trackSatMax.Size = new System.Drawing.Size(104, 45);
            this.trackSatMax.TabIndex = 4;
            this.trackSatMax.Scroll += new System.EventHandler(this.trackSatMax_Scroll);
            // 
            // trackValMax
            // 
            this.trackValMax.Location = new System.Drawing.Point(190, 80);
            this.trackValMax.Name = "trackValMax";
            this.trackValMax.Size = new System.Drawing.Size(104, 45);
            this.trackValMax.TabIndex = 5;
            this.trackValMax.Scroll += new System.EventHandler(this.trackValMax_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(116, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Hue";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(116, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Saturation";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(116, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Value";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(116, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Obj Size";
            // 
            // trackSizeMin
            // 
            this.trackSizeMin.Location = new System.Drawing.Point(12, 117);
            this.trackSizeMin.Name = "trackSizeMin";
            this.trackSizeMin.Size = new System.Drawing.Size(104, 45);
            this.trackSizeMin.TabIndex = 10;
            this.trackSizeMin.Scroll += new System.EventHandler(this.trackSizeMin_Scroll);
            // 
            // trackSizeMax
            // 
            this.trackSizeMax.Location = new System.Drawing.Point(190, 117);
            this.trackSizeMax.Name = "trackSizeMax";
            this.trackSizeMax.Size = new System.Drawing.Size(104, 45);
            this.trackSizeMax.TabIndex = 11;
            this.trackSizeMax.Scroll += new System.EventHandler(this.trackSizeMax_Scroll);
            // 
            // AppX
            // 
            this.ClientSize = new System.Drawing.Size(387, 262);
            this.Controls.Add(this.trackSizeMax);
            this.Controls.Add(this.trackSizeMin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackValMax);
            this.Controls.Add(this.trackSatMax);
            this.Controls.Add(this.trackHueMax);
            this.Controls.Add(this.trackValMin);
            this.Controls.Add(this.trackSatMin);
            this.Controls.Add(this.trackHueMin);
            this.Name = "AppX";
            ((System.ComponentModel.ISupportInitialize)(this.trackHueMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSatMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackValMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHueMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSatMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackValMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSizeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSizeMax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void trackSizeMax_Scroll(object sender, EventArgs e)
        {

        }

        private void trackHueMin_Scroll(object sender, EventArgs e)
        {

        }

        private void trackSatMin_Scroll(object sender, EventArgs e)
        {

        }

        private void trackValMin_Scroll(object sender, EventArgs e)
        {

        }

        private void trackSizeMin_Scroll(object sender, EventArgs e)
        {

        }

        private void trackSatMax_Scroll(object sender, EventArgs e)
        {

        }

        private void trackHueMax_Scroll(object sender, EventArgs e)
        {

        }

        private void trackValMax_Scroll(object sender, EventArgs e)
        {

        }
    }
}
