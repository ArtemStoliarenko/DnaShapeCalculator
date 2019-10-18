using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	public sealed class PfamDnaParametersB
	{
		public float Shear { get; }

		public float Stretch { get; }

		public float Stagger { get; }

		public float Buckle { get; }

		public float Propel { get; }

		public float Opening { get; }

		private PfamDnaParametersB(float shear, float stretch, float stagger, float buckle, float propel, float opening)
		{
			this.Shear = shear;
			this.Stretch = stretch;
			this.Stagger = stagger;
			this.Buckle = buckle;
			this.Propel = propel;
			this.Opening = opening;
		}

		public static PfamDnaParametersB Parse(ReadOnlySpan<string> values)
		{
			var shear = float.Parse(values[0]);
			var stretch = float.Parse(values[1]);
			var stagger = float.Parse(values[2]);
			var buckle = float.Parse(values[3]);
			var propel = float.Parse(values[4]);
			var opening = float.Parse(values[5]);

			return new PfamDnaParametersB(shear, stretch, stagger, buckle, propel, opening);
		}
	}
}
