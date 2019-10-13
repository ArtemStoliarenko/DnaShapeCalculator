using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	sealed class PdbPfamMappingRecord
	{
		private const char revisionDelimiter = '.';

		public string PdbCode { get; }

		public string Strand { get; }

		public string Family { get; }

		public int PdbStartCoordinate { get; }

		public int PdbEndCoordinate { get; }

		public PdbPfamMappingRecord(string pdbCode, string strand, string family, int pdbStartCoordinate, int pdbEndCoordinate)
		{
			if (string.IsNullOrWhiteSpace(pdbCode))
			{
				throw new ArgumentNullException(nameof(pdbCode));
			}
			if (string.IsNullOrEmpty(strand))
			{
				throw new ArgumentNullException(nameof(strand));
			}
			if (string.IsNullOrEmpty(family))
			{
				throw new ArgumentNullException(nameof(family));
			}
			if (family.Contains(revisionDelimiter))
			{
				throw new ArgumentOutOfRangeException(nameof(family));
			}
			if (pdbStartCoordinate < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(pdbStartCoordinate));
			}
			if (pdbEndCoordinate < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(pdbEndCoordinate));
			}
			if (pdbStartCoordinate < pdbEndCoordinate)
			{
				throw new ArgumentOutOfRangeException($"{nameof(pdbStartCoordinate)} {nameof(pdbEndCoordinate)}");
			}

			this.PdbCode = pdbCode;
			this.Strand = strand;
			this.Family = family;
			this.PdbStartCoordinate = pdbStartCoordinate;
			this.PdbEndCoordinate = pdbEndCoordinate;
		}
	}
}
