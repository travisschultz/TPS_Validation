using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TPS_Validation
{
	public static class Xml
	{
		private static String _xmlFileLocation = @"\\10.71.248.61\va_data$\ESAPI\Test Files\EclipseUpgradeValidation.xml";
		private static XElement _root = XElement.Load(_xmlFileLocation);

		public static XElement GetOptions()
		{
			return _root.Elements().Where(e => e.Name == "Options").Single();
		}

		public static string GetCourseID()
		{
			return GetOptions().Elements().Where(e => e.Name == "Course").Select(e => e.Attribute("ID")).Single().Value;
		}

		public static string GetVersionNumber()
		{
			return GetOptions().Elements().Where(e => e.Name == "Version").Select(e => e.Attribute("Number")).Single().Value;
		}

		public static IEnumerable<string> GetPatientIDs()
		{
			return _root.Elements().Where(e => e.Name == "PatientList").Single().Elements().Select(e => e.Attribute("ID").Value);
		}
	}
}
