using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DnaShapeCalculator.Core.Entities;

namespace DnaShapeCalculator.PdbMapRecordFilters
{
	internal sealed class PdbMapRecordFilterStrict : PdbMapRecordFilter
	{
		private readonly PdbPfamMapRecord[] pfamRecords;

		public PdbMapRecordFilterStrict(PdbMapRecord[] records, PdbPfamMapRecord[] pfamRecords, HashSet<string> allowedPdbCodes, int minDomains)
			: base(records, allowedPdbCodes, minDomains)
		{
			if (pfamRecords == null)
			{
				throw new ArgumentNullException(nameof(pfamRecords));
			}
			if (pfamRecords.Length == 0)
			{
				throw new ArgumentException(nameof(pfamRecords));
			}

			this.pfamRecords = pfamRecords;
		}

		protected override PdbMapRecord[] FilterInner()
		{
			var allowedPdbRecords = FilterAllowedPdbCodes();
			var allowedPdbPfamRecords = FilterAllowedPdbCodesForPdbPfamRecords();

			var pdbStructureCount = GetFamilyStructureCount(allowedPdbRecords);
			var pdbPfamStructureCount = GetFamilyStructureCount(allowedPdbPfamRecords);

			var allowedFamilies = GetFamiliesWithCorrectDomainCount(allowedPdbRecords);

			return allowedPdbRecords.Where(r => IsAllowedFamily(r, allowedFamilies, pdbStructureCount, pdbPfamStructureCount))
				.ToArray();
		}

		private static bool IsAllowedFamily(PdbMapRecord record, HashSet<string> allowedFamilies, IDictionary<string, int> pdbStructureCount, IDictionary<string, int> pdbPfamStructureCount)
		{
			if (!allowedFamilies.Contains(record.Family))
			{
				return false;
			}
			if (!pdbStructureCount.TryGetValue(record.Family, out var pdbCount))
			{
				return false;
			}
			if (!pdbPfamStructureCount.TryGetValue(record.Family, out var pdbPfamCount))
			{
				return false;
			}

			return pdbCount == pdbPfamCount;
		}

		private IEnumerable<PdbPfamMapRecord> FilterAllowedPdbCodesForPdbPfamRecords() =>
			pfamRecords.Where(r => allowedPdbCodes.Contains(r.PdbCode));

		private static Dictionary<string, int> GetFamilyStructureCount(IEnumerable<IFamilyMappingRecord> mappingRecords) =>
			mappingRecords.GroupBy(r => r.Family)
			.ToDictionary(group => group.Key, group => group.Count());
	}
}
