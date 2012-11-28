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

namespace TangibleUISharp
{
	class Program
	{

		//static bool _captureInProgress;
		static void Main (string[] args)
		{
			if (!IsPlaformCompatable ())
				return;
			Application.Run (new AppX3());
			return;
			//Application.Init ();


			
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
