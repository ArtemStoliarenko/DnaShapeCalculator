using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
    internal sealed class PdbMap
    {
        private readonly Dictionary<(string pdbCode, string pdbStrand, int startCoordinate, int endCoordinate), PdbMapRecord> pdbMap;

        public PdbMap(PdbMapRecord[] records)
        {
            var pdbMap = new Dictionary<(string, string, int, int), PdbMapRecord>(records.Length);

            foreach (var record in records)
            {
				if (!pdbMap.ContainsKey((record.PdbCode, record.Strand, record.ProteinStartCoordinate, record.ProteinEndCoordinate)))
				{
					pdbMap.Add((record.PdbCode, record.Strand, record.ProteinStartCoordinate, record.ProteinEndCoordinate), record);
				}
            }

            this.pdbMap = pdbMap;
        }

		public PdbMapRecord GetPdbMapRecord(string pdbCode, string strand, int startCoordinate, int endCoordinate) =>
			pdbMap.TryGetValue((pdbCode, strand, startCoordinate, endCoordinate), out var pdbMapRecord) ? pdbMapRecord : null;
    }
}
