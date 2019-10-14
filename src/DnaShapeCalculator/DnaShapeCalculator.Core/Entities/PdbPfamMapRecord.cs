using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	public sealed class PdbPfamMapRecord
	{
		public string PdbCode { get; }

		public string Strand { get; }

		public string Family { get; }

		public string PdbStartCoordinate { get; }

		public string PdbEndCoordinate { get; }

		public PdbPfamMapRecord(string pdbCode, string strand, string family, string pdbStartCoordinate, string pdbEndCoordinate)
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
			if (string.IsNullOrEmpty(pdbStartCoordinate))
			{
				throw new ArgumentNullException(nameof(pdbStartCoordinate));
			}
			if (string.IsNullOrEmpty(pdbEndCoordinate))
			{
				throw new ArgumentNullException(nameof(pdbEndCoordinate));
			}

			this.PdbCode = pdbCode;
			this.Strand = strand;
			this.Family = family;
			this.PdbStartCoordinate = pdbStartCoordinate;
			this.PdbEndCoordinate = pdbEndCoordinate;
		}
	}
}
