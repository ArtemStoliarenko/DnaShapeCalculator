using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	public sealed class PfamDnaParametersA
	{
		public float Xdisp { get; }

		public float Ydisp { get; }

		public float Inclin { get; }

		public float Tip { get; }

		public float? AxBend { get; }

		private PfamDnaParametersA(float xdisp, float ydisp, float inclin, float tip, float? axBend)
		{
			this.Xdisp = xdisp;
			this.Ydisp = ydisp;
			this.Inclin = inclin;
			this.Tip = tip;
			this.AxBend = axBend;
		}

		public static PfamDnaParametersA Parse(ReadOnlySpan<string> values)
		{
			var xdisp = float.Parse(values[0]);
			var ydisp = float.Parse(values[1]);
			var inclin = float.Parse(values[2]);
			var tip = float.Parse(values[3]);
			float? axBend = float.TryParse(values[4], out var temp) ? (float?)temp : null;

			return new PfamDnaParametersA(xdisp, ydisp, inclin, tip, axBend);
		}
	}
}
