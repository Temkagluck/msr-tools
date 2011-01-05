/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.StatGenerator
{
	public class GeneratingTool : Tool
	{
		public GeneratingTool(string configFile)
			: base(configFile)
		{
			data.ReadOnly = true;
		}
		public void GenerateStat(string outputDir, string templateDir)
		{
			StatBuilder builder = GetConfiguredType<StatBuilder>();
			builder.GenerateStat(data, outputDir, templateDir);
		}
	}
}