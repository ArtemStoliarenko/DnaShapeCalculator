using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	static class PfamDnaParametersExtension
	{
		public static PfamDnaParametersA Average(this IEnumerable<PfamDnaParametersA> parameters)
		{
			int count = 0;
			int axBendCount = 0;

			float xDisp = 0;
			float yDisp = 0;
			float inclin = 0;
			float tip = 0;
			float axBend = 0;

			foreach (var parameter in parameters)
			{
				xDisp += ReplaceNaN(parameter.Xdisp);
				yDisp += ReplaceNaN(parameter.Ydisp);
				inclin += ReplaceNaN(parameter.Inclin);
				tip += ReplaceNaN(parameter.Tip);

				if (parameter.AxBend.HasValue)
				{
					axBend += ReplaceNaN(parameter.AxBend.Value);
					++axBendCount;
				}

				++count;
			}

			if (axBendCount == 0)
			{
				return new PfamDnaParametersA(xDisp / count, yDisp / count, inclin / count, tip / count, null);
			}
			else
			{
				return new PfamDnaParametersA(xDisp / count, yDisp / count, inclin / count, tip / count, axBend / axBendCount);
			}
		}

		public static PfamDnaParametersB Average(this IEnumerable<PfamDnaParametersB> parameters)
		{
			int count = 0;

			float shear = 0;
			float stretch = 0;
			float stagger = 0;
			float buckle = 0;
			float propel = 0;
			float opening = 0;

			foreach (var parameter in parameters)
			{
				shear += ReplaceNaN(parameter.Shear);
				stretch += ReplaceNaN(parameter.Stretch);
				stagger += ReplaceNaN(parameter.Stagger);
				buckle += ReplaceNaN(parameter.Buckle);
				propel += ReplaceNaN(parameter.Propel);
				opening += ReplaceNaN(parameter.Opening);

				++count;
			}

			return new PfamDnaParametersB(shear / count, stretch / count, stagger / count, buckle / count, propel / count, opening / count);
		}

		public static PfamDnaParametersС Average(this IEnumerable<PfamDnaParametersС> parameters)
		{
			int count = 0;

			float shift = 0;
			float slide = 0;
			float rise = 0;
			float tilt = 0;
			float roll = 0;
			float twist = 0;

			foreach (var parameter in parameters)
			{
				shift += ReplaceNaN(parameter.Shift);
				slide += ReplaceNaN(parameter.Slide);
				rise += ReplaceNaN(parameter.Rise);
				tilt += ReplaceNaN(parameter.Tilt);
				roll += ReplaceNaN(parameter.Roll);
				twist += ReplaceNaN(parameter.Twist);

				++count;
			}

			return new PfamDnaParametersС(shift / count, slide / count, rise / count, tilt / count, roll / count, twist / count);
		}

		private static float ReplaceNaN(float value) => float.IsNaN(value) ? 0 : value;

	}
}
