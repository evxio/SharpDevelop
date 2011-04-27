﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class PropertyTests
	{
		Properties properties;
		TestableDTEProject project;
		TestableProject msbuildProject;
		
		void CreateProperties()
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			properties = new Properties(project);
		}
		
		[Test]
		public void Value_GetPostBuildEvent_ReturnsProjectsPostBuildEvent()
		{
			CreateProperties();
			msbuildProject.SetProperty("PostBuildEvent", "Test");
			var postBuildEventProperty = properties.Item("PostBuildEvent").Value;
			
			Assert.AreEqual("Test", postBuildEventProperty);
		}
		
		[Test]
		public void Value_GetPostBuildEvent_ReturnsUnevaluatedPostBuildEvent()
		{
			CreateProperties();
			msbuildProject.SetProperty("PostBuildEvent", "$(SolutionDir)", false);
			var postBuildEventProperty = properties.Item("PostBuildEvent").Value;
			
			Assert.AreEqual("$(SolutionDir)", postBuildEventProperty);
		}
		
		[Test]
		public void Value_GetNullProperty_ReturnsEmptyString()
		{
			CreateProperties();
			var property = properties.Item("TestTestTest").Value;
			
			Assert.AreEqual(String.Empty, property);
		}
		
		[Test]
		public void Value_SetPostBuildEvent_UpdatesProjectsPostBuildEvent()
		{
			CreateProperties();
			properties.Item("PostBuildEvent").Value = "Test";
			
			string postBuildEventProperty = msbuildProject.GetEvaluatedProperty("PostBuildEvent");
			
			Assert.AreEqual("Test", postBuildEventProperty);
		}
		
		[Test]
		public void Value_SetPostBuildEvent_DoesNotEscapeText()
		{
			CreateProperties();
			properties.Item("PostBuildEvent").Value = "$(SolutionDir)";
			
			string postBuildEventProperty = msbuildProject.GetUnevalatedProperty("PostBuildEvent");
			
			Assert.AreEqual("$(SolutionDir)", postBuildEventProperty);
		}
		
		[Test]
		public void ItemValue_SetPostBuildEvent_MSBuildProjectIsSaved()
		{
			CreateProperties();
			properties.Item("PostBuildEvent").Value = "test";
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void ItemValue_GetTargetFrameworkMoniker_ReturnsNet40ClientProfile()
		{
			CreateProperties();
			msbuildProject.SetProperty("TargetFrameworkVersion", "4.0");
			msbuildProject.SetProperty("TargetFrameworkProfile", "Client");
			
			string targetFrameworkMoniker = properties.Item("TargetFrameworkMoniker").Value as string;
			
			string expectedTargetFrameworkMoniker = ".NETFramework,Version=v4.0,Profile=Client";
			
			Assert.AreEqual(expectedTargetFrameworkMoniker, targetFrameworkMoniker);
		}
		
		[Test]
		public void ItemValue_GetTargetFrameworkMonikerUsingIncorrectCaseAndFrameworkIdentifierIsSilverlight_ReturnsNet35Silverlight()
		{
			CreateProperties();
			msbuildProject.SetProperty("TargetFrameworkIdentifier", "Silverlight");
			msbuildProject.SetProperty("TargetFrameworkVersion", "3.5");
			msbuildProject.SetProperty("TargetFrameworkProfile", "Full");
			
			string targetFrameworkMoniker = properties.Item("targetframeworkmoniker").Value as string;
			
			string expectedTargetFrameworkMoniker = "Silverlight,Version=v3.5,Profile=Full";
			
			Assert.AreEqual(expectedTargetFrameworkMoniker, targetFrameworkMoniker);
		}
	}
}
