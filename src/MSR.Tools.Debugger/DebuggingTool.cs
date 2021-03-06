/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Data.Persistent;
using MSR.Data.VersionControl;
using MSR.Models.Prediction;
using MSR.Models.Prediction.PostReleaseDefectFiles;
using MSR.Models.Prediction.Predictors;

namespace MSR.Tools.Debugger
{
	public class DebuggingTool : Tool
	{
		private IScmData scmDataWithoutCache;
		
		public DebuggingTool(string configFile)
			: base(configFile, "mappingtool", "generatingtool")
		{
			scmDataWithoutCache = GetScmDataWithoutCache();
		}
		public void FindDiffError(int startRevision, string path)
		{
			int revisionNumber = startRevision;
			string revision = scmDataWithoutCache.RevisionByNumber(revisionNumber);
			
			do
			{
				Console.WriteLine("Searching for diff errors in commit {0}({1})...", revision, revisionNumber);
				
				List<string> fileErrors = new List<string>();
				foreach (var file in scmDataWithoutCache.Log(revision).TouchedFiles.Select(x => x.Path))
				{
					if ((path != null) && (path != file))
					{
						continue;
					}
					var diff = scmDataWithoutCache.Diff(revision, file);
					if (diff.AddedLines.Count() > 0)
					{
						var blame = scmDataWithoutCache.Blame(revision, file);
						
						foreach (var line in diff.AddedLines)
						{
							if (blame[line] != revision)
							{
								fileErrors.Add(string.Format(
									"Line {0}: added in diff, but blame say otherwise.", line
								));
							}
						}
						foreach (var line in blame.Where(x => x.Value == revision).Select(x => x.Key))
						{
							if (! diff.AddedLines.Contains(line))
							{
								fileErrors.Add(string.Format(
									"Line {0}: not added in diff, but blame say otherwise.", line
								));
							}
						}
					}
					if (fileErrors.Count > 0)
					{
						Console.WriteLine("Diff error in revision {0} in file {1}.", revision, file);
						foreach (var error in fileErrors)
						{
							Console.WriteLine(error);
						}
						return;
					}
				}
				
				revisionNumber++;
				revision = scmDataWithoutCache.RevisionByNumber(revisionNumber);
			} while (revision != null);
			
			Console.WriteLine("No diff errors.");
		}
		public void QueryUnderProfiler()
		{
			PersistentDataStoreProfiler prof = new PersistentDataStoreProfiler(data);

			using (var s = data.OpenSession())
			{
				
			}

			Console.WriteLine("{0}", prof);
		}
		public void Blame(string path, string revision)
		{
			var blame = scmDataWithoutCache.Blame(revision, path);
			
			SmartDictionary<string, IEnumerable<int>> output =
				new SmartDictionary<string,IEnumerable<int>>((x) => new List<int>());
			for (int i = 1; i <= blame.Count; i++)
			{
				(output[blame[i]] as List<int>).Add(i);
			}
			foreach (var rev in output.OrderBy(x => x.Key))
			{
				Console.WriteLine("{0} {1}", rev.Key, rev.Value.Count());
			}
		}
	}
}