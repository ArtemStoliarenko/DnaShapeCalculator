using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
    class PdbMapRecord
    {
        public string PdbCode { get; }

        public char Strand { get; }

        public string Family { get; }

        public string Domain { get; }

        public int StartCoordinate { get; }

        public int EndCoordinate { get; }

        internal PdbMapRecord(string pdbCode, char strand, string family, string domain, int startCoordinate, int endCoordinate)
        {
            if (string.IsNullOrWhiteSpace(pdbCode))
            {
                throw new ArgumentNullException(nameof(pdbCode));
            }
            if (string.IsNullOrEmpty(family))
            {
                throw new ArgumentNullException(nameof(family));
            }
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException(nameof(domain));
            }
            if (startCoordinate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startCoordinate));
            }
            if (endCoordinate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(endCoordinate));
            }

            this.PdbCode = pdbCode;
            this.Strand = strand;
            this.Family = family;
            this.Domain = domain;
            this.StartCoordinate = startCoordinate;
            this.EndCoordinate = endCoordinate;
        }
    }
}
