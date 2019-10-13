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
        private readonly Dictionary<(string pdbCode, char pdbStrand), string> domainMap;

        private readonly Dictionary<(string pdbCode, char pdbStrand, int startCoordinate, int endCoordinate), string> familyMap;

        internal PdbMap(List<PdbMapRecord> records)
        {
            var domainMap = new Dictionary<(string, char), string>(records.Count);
            var familyMap = new Dictionary<(string, char, int, int), string>(records.Count);

            foreach (var record in records)
            {
                if (!domainMap.ContainsKey((record.PdbCode, record.Strand)))
                {
                    domainMap.Add((record.PdbCode, record.Strand), record.Domain);
                }

				if (!familyMap.ContainsKey((record.PdbCode, record.Strand, record.StartCoordinate, record.EndCoordinate)))
				{
					familyMap.Add((record.PdbCode, record.Strand, record.StartCoordinate, record.EndCoordinate), record.Family);
				}
            }

            this.domainMap = domainMap;
            this.familyMap = familyMap;
        }

        public string GetDomain(string pdbCode, char pdbStrand) =>
            domainMap.TryGetValue((pdbCode, pdbStrand), out var domain) ? domain : null;

        public string GetFamily(string pdbCode, char pdbStrand, int startCoordinate, int endCoordinate) =>
            familyMap.TryGetValue((pdbCode, pdbStrand, startCoordinate, endCoordinate), out var family) ? family : null;
    }
}
