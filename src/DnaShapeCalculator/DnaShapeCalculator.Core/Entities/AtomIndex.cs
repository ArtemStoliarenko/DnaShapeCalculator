using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	public sealed class AtomIndex
	{
		public string Strand { get; }

		public int Coordinate { get; }

		public AtomIndex(string strand, int coordinate)
		{
			if (string.IsNullOrEmpty(strand))
			{
				throw new ArgumentNullException(nameof(strand));
			}
			if (coordinate < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(coordinate));
			}

			this.Strand = strand;
			this.Coordinate = coordinate;
		}
	}
}
