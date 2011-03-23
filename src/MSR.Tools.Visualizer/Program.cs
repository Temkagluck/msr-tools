﻿/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			VisualizerPresenter visualizer = new VisualizerPresenter(
				new VisualizerModel(),
				new VisualizerView()
			);

			visualizer.Run();
		}
	}
}
