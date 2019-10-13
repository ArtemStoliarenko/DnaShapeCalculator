using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	sealed class PdbPfamMappingRecord
	{
		public string PdbCode { get; }

		public string Strand { get; }

		public string Family { get; }

		public int PdbStartCoordinate { get; }

		public int PdbEndCoordinate { get; }
	}
}
