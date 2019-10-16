using DnaShapeCalculator.Core;
using DnaShapeCalculator.Core.Entities;
using DnaShapeCalculator.PdbMapRecordFilters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DnaShapeCalculator
{
	class Program
	{
		private const ExperimentType allowedExperimentType = ExperimentType.Xray;
		private const float resolutionThreshold = 3.5F;

		private const int domainCountThreshold = 3;

		static void Main(string[] args)
		{
			var allowedPdbs = Task.Run(() => GetAllowedPdbCodes("dna"));

			var pdbMapRecords = GetPdbMapRecords("pdbmap");

			var pdbMapRecords2 = new PdbMapRecordFilter(pdbMapRecords, allowedPdbs.Result, domainCountThreshold).Filter();

			Console.WriteLine("Hello World!");
		}


		private static PdbMapRecord[] GetPdbMapRecords(string filename)
		{
			var file = File.ReadAllLines(filename);

			return PdbMappingDataFactory.CreatePdbMapRecords(file);
		}

		private static PdbPfamMapRecord[] GetPdbPfamMapRecords(string filename)
		{
			var file = File.ReadAllLines(filename);

			return PdbMappingDataFactory.CreatePdbPfamMapRecords(file);
		}

		private static HashSet<string> GetAllowedPdbCodes(string directoryName)
		{
			var records = PdbRecordFactory.GetRecords(directoryName)
				.Where(IsAllowedPdbFile)
				.Select(r => r.PdbCode);

			return new HashSet<string>(records);
		}

		private static bool IsAllowedPdbFile(PdbRecord pdbRecord)
		{
			return pdbRecord.ExperimentType == allowedExperimentType && pdbRecord.Resolution.HasValue && pdbRecord.Resolution < resolutionThreshold;
		}
	}
}
