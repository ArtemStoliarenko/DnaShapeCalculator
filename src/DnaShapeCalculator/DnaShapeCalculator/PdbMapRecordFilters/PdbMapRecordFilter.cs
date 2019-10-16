using DnaShapeCalculator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DnaShapeCalculator.PdbMapRecordFilters
{
	internal class PdbMapRecordFilter
	{
		protected readonly PdbMapRecord[] records;

		protected readonly HashSet<string> allowedPdbCodes;

		protected readonly int minDomains;

		private readonly Lazy<PdbMapRecord[]> lazyResult;

		public PdbMapRecordFilter(PdbMapRecord[] records, HashSet<string> allowedPdbCodes, int minDomains)
		{
			if (records == null)
			{
				throw new ArgumentNullException(nameof(records));
			}
			if (records.Length == 0)
			{
				throw new ArgumentException(nameof(records));
			}
			if (allowedPdbCodes == null)
			{
				throw new ArgumentNullException(nameof(allowedPdbCodes));
			}
			if (allowedPdbCodes.Count == 0)
			{
				throw new ArgumentException(nameof(allowedPdbCodes));
			}
			if (minDomains <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(minDomains));
			}

			this.records = records;
			this.allowedPdbCodes = allowedPdbCodes;
			this.minDomains = minDomains;

			lazyResult = new Lazy<PdbMapRecord[]>(FilterInner);
		}

		public PdbMapRecord[] Filter() => lazyResult.Value;

		protected virtual PdbMapRecord[] FilterInner()
		{
			var allowedPdbRecords = FilterAllowedPdbCodes();
			var allowedFamilies = GetFamiliesWithCorrectDomainCount(allowedPdbRecords);

			return allowedPdbRecords.Where(r => allowedFamilies.Contains(r.Family)).ToArray();
		}

		protected IEnumerable<PdbMapRecord> FilterAllowedPdbCodes() => records.Where(r => allowedPdbCodes.Contains(r.PdbCode));

		protected HashSet<string> GetFamiliesWithCorrectDomainCount(IEnumerable<PdbMapRecord> records)
		{
			var allowedFamilies = records.GroupBy(r => r.Family)
				.Select(group => (group.Key, group.GroupBy(innerGroup => innerGroup.Domain).Count()))
				.Where(family => family.Item2 >= minDomains)
				.Select(family => family.Key);

			return new HashSet<string>(allowedFamilies);
		}
	}
}
