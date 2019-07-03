using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
    public sealed class PdbMap
    {
        private readonly List<PdbMapRecord> records;

        internal PdbMap(IEnumerable<PdbMapRecord> records)
            : this(records.ToList())
        {

        }

        internal PdbMap(List<PdbMapRecord> records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
        }

        public string GetDomain(string pdbCode, char pdbStrand) => records.FirstOrDefault(r => r.MatchPdb(pdbCode, pdbStrand))?.Domain;

        public string GetFamily(string pdbCode, char pdbStrand, int startCoordinate, int endCoordinate) =>
            records.Single(r => r.MatchPdb(pdbCode, pdbStrand, startCoordinate, endCoordinate)).Family;
    }
}
