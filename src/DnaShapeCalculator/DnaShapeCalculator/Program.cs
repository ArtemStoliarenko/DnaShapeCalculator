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
		private const double resolutionThreshold = 3.5;
		private const int domainCountThreshold = 3;

		static void Main(string[] args)
		{
			var allowedPdbs = Task.Run(() => GetAllowedPdbCodes("dna"));

			var pdbMapRecords = GetPdbMapRecords("pdbmap");
			var pdbPfamMapRecords = GetPdbPfamMapRecords("pdb_pfam_mapping.txt");

			pdbMapRecords = pdbMapRecords.Where(r => allowedPdbs.Result.Contains(r.PdbCode)).ToArray();
			pdbPfamMapRecords = pdbPfamMapRecords.Where(r => allowedPdbs.Result.Contains(r.PdbCode)).ToArray();

			var pdbMapStructuresCounts = GetStructuresPerFamily(pdbMapRecords);
			var pdbPfaMapStructuresCounts = GetStructuresPerFamily(pdbPfamMapRecords);

			var pdbAllowedFamilies = new HashSet<string>
				(GetDomainsPerFamily(pdbMapRecords)
				.Where(family => IsAllowedFamily(family.Family, family.DomainCount, pdbMapStructuresCounts, pdbPfaMapStructuresCounts))
				.Select(family => family.Family));

			var pdbMapRecords2 = new PdbMapRecordFilter(pdbMapRecords, allowedPdbs.Result, domainCountThreshold).Filter();
			pdbMapRecords = pdbMapRecords.Where(r => pdbAllowedFamilies.Contains(r.Family)).ToArray();
			int count = pdbMapRecords.GroupBy(p => p.PdbCode).Count();

			Console.WriteLine("Hello World!");
		}

		private static Dictionary<string, int> GetStructuresPerFamily(IFamilyMappingRecord[] records) =>
			records.GroupBy(r => r.Family)
			.ToDictionary(group => group.Key, group => group.Count());

		private static IEnumerable<(string Family, int DomainCount)> GetDomainsPerFamily(PdbMapRecord[] records) =>
			records.GroupBy(r => r.Family)
			.Select(group => (group.Key, group.GroupBy(g => g.Domain).Count()));

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

		private static bool IsAllowedFamily(string familyName, int domainCount, IDictionary<string, int> pdbMapStructuresCount, IDictionary<string, int> pdbPfamMapStructuresCount)
		{
			if (domainCount < domainCountThreshold)
			{
				return false;
			}

			if (!pdbMapStructuresCount.TryGetValue(familyName, out var pdbMapFamilyCount))
			{
				return false;
			}

			if (!pdbPfamMapStructuresCount.TryGetValue(familyName, out var pdbPfamMapFamilyCount))
			{
				return false;
			}

			if (pdbMapFamilyCount != pdbPfamMapFamilyCount)
			{
				return false;
			}

			return true;
		}
	}
}
