using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
    public class PdbRecord
    {
		public string PdbCode { get; }

        public ExperimentType ExperimentType { get; }

        public float? Resolution { get; }

		internal PdbRecord(string pdbCode, ExperimentType experimentType)
			: this(pdbCode, experimentType, null)
		{
		}

		internal PdbRecord(string pdbCode, ExperimentType experimentType, float? resolution)
		{
			if (string.IsNullOrEmpty(pdbCode))
			{
				throw new ArgumentNullException(nameof(pdbCode));
			}

			this.PdbCode = pdbCode.ToUpperInvariant();
			this.ExperimentType = experimentType;
			this.Resolution = resolution;
		}
    }
}
