using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	public sealed class PfamFile
	{
		public string PdbCode { get; }

		public string Strand { get; }

		public string Family { get; }

		public string Domain { get; }

		public int ProteinStartCoordinate { get; }

		public int ProteinEndCoordinate { get; }

		public AtomIndex[] Positions { get; }

		public PfamFile(string fullName, PdbMapRecord record, AtomIndex[] positions)
		{
			if (positions == null)
			{
				throw new ArgumentNullException(nameof(positions));
			}
			if (positions.Length == 0)
			{
				throw new ArgumentException(nameof(positions));
			}

			this.PdbCode = record.PdbCode;
			this.Strand = record.Strand;
			this.Family = record.Family;
			this.Domain = record.Domain;
			this.ProteinStartCoordinate = record.ProteinStartCoordinate;
			this.ProteinEndCoordinate = record.ProteinEndCoordinate;
			this.Positions = positions;
		}
	}
}
