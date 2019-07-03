using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
    public class PdbRecord
    {
        public float Resolution { get; }

        public ReadOnlyCollection<Atom> Atoms { get; }


    }
}
