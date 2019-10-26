using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	public class PfamDnaParameters
	{
		public PfamFile PfamFile { get; }

		public PfamDnaParametersA DnaParametersA { get; }

		public PfamDnaParametersB DnaParametersB { get; }

		public PfamDnaParametersС DnaParametersС { get; }

		internal PfamDnaParameters(PfamFile pfamFile, PfamDnaParametersA dnaParametersA, PfamDnaParametersB dnaParametersB, PfamDnaParametersС dnaParametersС)
		{
			if (pfamFile == null)
			{
				throw new ArgumentNullException(nameof(pfamFile));
			}
			if (dnaParametersA == null)
			{
				throw new ArgumentNullException(nameof(dnaParametersA));
			}
			if (dnaParametersB == null)
			{
				throw new ArgumentNullException(nameof(dnaParametersB));
			}
			if (dnaParametersС == null)
			{
				throw new ArgumentNullException(nameof(dnaParametersС));
			}

			this.PfamFile = pfamFile;
			this.DnaParametersA = dnaParametersA;
			this.DnaParametersB = dnaParametersB;
			this.DnaParametersС = dnaParametersС;
		}

		public override string ToString() => string.Join(';', this.PfamFile, this.DnaParametersA, this.DnaParametersB, this.DnaParametersС);
	}
}
