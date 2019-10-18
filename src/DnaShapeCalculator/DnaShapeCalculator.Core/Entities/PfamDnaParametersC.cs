using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	namespace DnaShapeCalculator.Core.Entities
	{
		public sealed class PfamDnaParametersС
		{
			public float Shift { get; }

			public float Slide { get; }

			public float Rise { get; }

			public float Tilt { get; }

			public float Roll { get; }

			public float Twist { get; }

			private PfamDnaParametersС(float shift, float slide, float rise, float tilt, float roll, float twist)
			{
				this.Shift = shift;
				this.Slide = slide;
				this.Rise = rise;
				this.Tilt = tilt;
				this.Roll = roll;
				this.Twist = twist;
			}

			public static PfamDnaParametersС Parse(ReadOnlySpan<string> values)
			{
				var shift = float.Parse(values[0]);
				var slide = float.Parse(values[1]);
				var rise = float.Parse(values[2]);
				var tilt = float.Parse(values[3]);
				var roll = float.Parse(values[4]);
				var twist = float.Parse(values[5]);

				return new PfamDnaParametersС(shift, slide, rise, tilt, roll, twist);
			}
		}
	}

}
