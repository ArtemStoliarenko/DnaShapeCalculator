using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
    public sealed class PdbMapRecord
    {
        public string PdbCode { get; }

        public string Strand { get; }

        public string Family { get; }

        public string Domain { get; }

        public int ProteinStartCoordinate { get; }

        public int ProteinEndCoordinate { get; }

        internal PdbMapRecord(string pdbCode, string strand, string family, string domain, int proteinStartCoordinate, int proteinEndCoordinate)
        {
            if (string.IsNullOrWhiteSpace(pdbCode))
            {
                throw new ArgumentNullException(nameof(pdbCode));
            }
			if  (string.IsNullOrEmpty(strand))
			{
				throw new ArgumentNullException(nameof(strand));
			}
            if (string.IsNullOrEmpty(family))
            {
                throw new ArgumentNullException(nameof(family));
            }
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException(nameof(domain));
            }
            if (proteinStartCoordinate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(proteinStartCoordinate));
            }
            if (proteinEndCoordinate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(proteinEndCoordinate));
            }
			if (proteinStartCoordinate > proteinEndCoordinate)
			{
				throw new ArgumentOutOfRangeException($"{nameof(proteinStartCoordinate)} {nameof(proteinEndCoordinate)}");
			}

			this.PdbCode = pdbCode;
            this.Strand = strand;
            this.Family = family;
            this.Domain = domain;
            this.ProteinStartCoordinate = proteinStartCoordinate;
            this.ProteinEndCoordinate = proteinEndCoordinate;
        }
    }
}
