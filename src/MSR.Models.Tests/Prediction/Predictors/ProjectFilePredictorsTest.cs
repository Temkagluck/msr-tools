/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Models.Prediction.Predictors
{
	[TestFixture]
	public class ProjectFilePredictorsTest : BaseRepositoryTest
	{
		private Prediction p;
		private PredictorContext context;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			context = new PredictorContext(this);
			p = new Prediction(this)
				.AddFileTouchCountInRevisionsPredictor();
		}
		[Test]
		public void Should_count_number_of_touches_for_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
						.Code(10)
					.AddFile("file2").Modified()
						.Code(50)
			.Submit();
			
			context
				.SetValue("file_id", selectionDSL.Files().PathIs("file1").Single().ID)
				.SetValue("after_revision", null)
				.SetValue("till_revision", "2");
			
			PredictorValue()
				.Should().Be(2);

			context.SetValue("after_revision", "1");

			PredictorValue()
				.Should().Be(1);
		}
		private double PredictorValue()
		{
			return p.GetPredictorValuesFor(context).Single();
		}
	}
}
