/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Tools.Visualizer
{
	public class VisualizerPresenter
	{
		private IVisualizerModel model;
		private IVisualizerView view;

		public VisualizerPresenter(IVisualizerModel model, IVisualizerView view)
		{
			this.model = model;
			this.view = view;
			view.OnOpenConfigFile += OpenConfigFile;
			view.OnVisualizationActivate += UseVisualization;
			view.OnChengeCleanUpOption += x => model.AutomaticallyCleanUp = x;
			
			Title = string.Empty;
		}
		public void Run()
		{
			view.Show();
		}
		public string Title
		{
			get { return view.Title; }
			set
			{
				view.Title = "Visualizer";
				if ((value != null) && (value != string.Empty))
				{
					view.Title += " - " + value;
				}
			}
		}
		private void ReadOptions()
		{
			view.SetVisualizationList(model.Visualizations.Select(x => x.Title));
			view.AutomaticallyCleanUp = model.AutomaticallyCleanUp;
		}
		private void OpenConfigFile(string fileName)
		{
			try
			{
				model.OpenConfig(fileName);
				ReadOptions();
			}
			catch (Exception e)
			{
				view.ShowError(e.Message);
			}
		}
		private void UseVisualization(int number)
		{
			IVisualization visualization = model.Visualizations[number];

			if (! visualization.Configurable || view.ConfigureVisualization(visualization))
			{
				if (model.AutomaticallyCleanUp)
				{
					view.Graph.CleanUp();
				}

				model.CalcVisualization(visualization);
				visualization.Draw(view.Graph);
				view.Status = model.LastVisualizationProfiling;
			}
		}
	}
}
