using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	public sealed class PfamFile
	{
		public string FullName { get; }

		public string PdbCode { get; }

		public string Strand { get; }

		public int[] Positions { get; }

		public PfamFile(string fullName, string pdbCode, string strand, int[] positions)
		{
			if (string.IsNullOrEmpty(fullName))
			{
				throw new ArgumentNullException(nameof(fullName));
			}
			if (string.IsNullOrWhiteSpace(pdbCode))
			{
				throw new ArgumentNullException(nameof(pdbCode));
			}
			if (string.IsNullOrEmpty(strand))
			{
				throw new ArgumentNullException(nameof(strand));
			}
			if (positions == null)
			{
				throw new ArgumentNullException(nameof(positions));
			}
			if (positions.Length == 0)
			{
				throw new ArgumentException(nameof(positions));
			}

			this.FullName = fullName;
			this.PdbCode = pdbCode.ToUpperInvariant();
			this.Strand = strand.ToUpperInvariant();
			this.Positions = positions;
		}
	}
}
