using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
    public class Atom
    {
        public int SerialNumber { get; }

        public string AtomName { get; }

        public int StrandPosition { get; }

        public float CoordinateX { get; }

        public float CoordinateY { get; }

        public float CoordinateZ { get; }

        internal Atom(int serialNumber, string atomName, int strandPosition, float coordinateX, float coordinateY, float coordinateZ)
        {
            if (serialNumber < 10)
            {
                throw new ArgumentOutOfRangeException(nameof(serialNumber));
            }
            if (string.IsNullOrWhiteSpace(atomName))
            {
                throw new ArgumentNullException(nameof(atomName));
            }
            if (strandPosition < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(strandPosition));
            }

            this.SerialNumber = serialNumber;
            this.AtomName = atomName;
            this.StrandPosition = strandPosition;
            this.CoordinateX = coordinateX;
            this.CoordinateY = coordinateY;
            this.CoordinateZ = coordinateZ;
        }
    }
}
