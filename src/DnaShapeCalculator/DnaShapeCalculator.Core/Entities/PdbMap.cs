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
        private readonly Dictionary<(string pdbCode, string pdbStrand), string> domainMap;

        private readonly Dictionary<(string pdbCode, string pdbStrand, int startCoordinate), string> familyMap;

        internal PdbMap(List<PdbMapRecord> records)
        {
            var domainMap = new Dictionary<(string, string), string>(records.Count);
            var familyMap = new Dictionary<(string, string, int), string>(records.Count);

            foreach (var record in records)
            {
                if (!domainMap.ContainsKey((record.PdbCode, record.Strand)))
                {
                    domainMap.Add((record.PdbCode, record.Strand), record.Domain);
                }

				if (!familyMap.ContainsKey((record.PdbCode, record.Strand, record.ProteinStartCoordinate)))
				{
					familyMap.Add((record.PdbCode, record.Strand, record.ProteinStartCoordinate), record.Family);
				}
            }

            this.domainMap = domainMap;
            this.familyMap = familyMap;
        }

        public string GetDomain(string pdbCode, string pdbStrand) =>
            domainMap.TryGetValue((pdbCode, pdbStrand), out var domain) ? domain : null;

        public string GetFamily(string pdbCode, string pdbStrand, int startCoordinate, int endCoordinate) =>
            familyMap.TryGetValue((pdbCode, pdbStrand, startCoordinate), out var family) ? family : null;
    }
}
