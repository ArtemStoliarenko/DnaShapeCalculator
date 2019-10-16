using DnaShapeCalculator.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DnaShapeCalculator.Core
{
	public static class PdbRecordFactory
	{
		private const string pdbSearchPattern = "*.pdb";

		private const string expdataGroupName = "EXPDTA";
		private const string remarksGroupName = "REMARK";
		private const string resolutionFieldName = "RESOLUTION.";

		private const string XrayExperimentType = "X-RAY";
		private const string NmrExperimentType = "NMR";

		private const char pdbSeparator = ' ';
		private const int resolutionFieldPosition = 3;
		private const int pdbCodeLength = 4;

		public static PdbRecord[] GetRecords(string directoryPath) => GetRecords(new DirectoryInfo(directoryPath));

		public static PdbRecord[] GetRecords(DirectoryInfo directory)
		{
			return directory.GetFiles(pdbSearchPattern)
				.Select(GetPdbRecord)
				.ToArray();
		}

		private static PdbRecord GetPdbRecord(FileInfo fileInfo)
		{
			var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
			string pdbCode = filenameWithoutExtension.Substring(filenameWithoutExtension.Length - pdbCodeLength, pdbCodeLength);

			ExperimentType experimentType = ExperimentType.None;
			float? resolution = null;

			using (var sr = fileInfo.OpenText())
			{
				string line;
				while (ShouldContinueReading(experimentType, in resolution) && (line = sr.ReadLine()) != null)
				{
					if (line.Contains(expdataGroupName, StringComparison.OrdinalIgnoreCase))
					{
						experimentType = GetExperimentType(line);
						if (experimentType == ExperimentType.Nmr)
						{
							return new PdbRecord(pdbCode, ExperimentType.Nmr);
						}
					}

					if (IsResolutionLine(line))
					{
						resolution = GetResolution(line);
					}
				}

				return new PdbRecord(pdbCode, experimentType, resolution);
			}
		}

		private static ExperimentType GetExperimentType(string expdataString)
		{
			var splittedExpData = expdataString.Split(pdbSeparator, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 1; i < splittedExpData.Length; ++i)
			{
				if (splittedExpData[i].Equals(XrayExperimentType, StringComparison.OrdinalIgnoreCase))
				{
					return ExperimentType.Xray;
				}

				if (splittedExpData[i].Equals(NmrExperimentType, StringComparison.OrdinalIgnoreCase))
				{
					return ExperimentType.Nmr;
				}
			}

			return ExperimentType.None;
		}

		private static float? GetResolution(string remarkResolutionString)
		{
			var splittedString = remarkResolutionString.Split(pdbSeparator);
			float.TryParse(splittedString[resolutionFieldPosition], out var result);
			return result;
		}

		private static bool ShouldContinueReading(ExperimentType experimentType, in float? resolution)
		{
			return experimentType == ExperimentType.None || !resolution.HasValue;
		}

		private static bool IsResolutionLine(string line)
		{
			return line.Contains(remarksGroupName, StringComparison.OrdinalIgnoreCase) && line.Contains(resolutionFieldName);
		}
	}
}
