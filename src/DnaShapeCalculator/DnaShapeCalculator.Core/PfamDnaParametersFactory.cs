using DnaShapeCalculator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DnaShapeCalculator.Core
{
	public static class PfamDnaParametersFactory
	{
		private static readonly Regex parameterStringRegex = new Regex(@"^\s*[0-9]+\)", RegexOptions.Compiled);

		public static PfamDnaParameters CreatePfamDnaParameters(PfamFile pfamFile, string resultFilename)
		{
			if (string.IsNullOrEmpty(resultFilename))
			{
				return null;
			}

			var file = File.ReadAllLines(resultFilename);

			var parametersAIndex = Array.FindIndex(file, str => str.Contains("(A) BP-Axis", StringComparison.OrdinalIgnoreCase));
			var parametersBIndex = Array.FindIndex(file, str => str.Contains("(B) Intra-BP parameters", StringComparison.OrdinalIgnoreCase));
			var parametersCIndex = Array.FindIndex(file, str => str.Contains("(C) Inter-BP", StringComparison.OrdinalIgnoreCase));
			var parametersDIndex = Array.FindIndex(file, str => str.Contains("(D) Backbone Parameters", StringComparison.OrdinalIgnoreCase));

			if (parametersAIndex == -1 || parametersBIndex == -1 || parametersCIndex == -1 ||
				parametersAIndex >= parametersBIndex || parametersBIndex >= parametersCIndex || parametersCIndex >= parametersDIndex)
			{
				return null;
			}

			var parmAList = new List<PfamDnaParametersA>(parametersBIndex - parametersAIndex);
			var parmBList = new List<PfamDnaParametersB>(parametersCIndex - parametersBIndex);
			var parmCList = new List<PfamDnaParametersС>(parametersDIndex - parametersCIndex);

			for (int i = parametersAIndex; i < parametersBIndex; ++i)
			{
				if (parameterStringRegex.IsMatch(file[i]))
				{
					parmAList.Add(CreatePfamDnaParametersA(pfamFile.Positions, file[i]));
				}
			}

			for (int i = parametersBIndex; i < parametersCIndex; ++i)
			{
				if (parameterStringRegex.IsMatch(file[i]))
				{
					parmBList.Add(CreatePfamDnaParametersB(pfamFile.Positions, file[i]));
				}
			}

			for (int i = parametersCIndex; i < parametersDIndex; ++i)
			{
				if (parameterStringRegex.IsMatch(file[i]))
				{
					parmCList.Add(CreatePfamDnaParametersC(pfamFile.Positions, file[i]));
				}
			}

			var averageA = parmAList.Where(a => a != null).Average();
			var averageB = parmBList.Where(b => b != null).Average();
			var averageC = parmCList.Where(c => c != null).Average();

			return new PfamDnaParameters(pfamFile, averageA, averageB, averageC);
		}

		public static PfamDnaParameters GetAverageForFamily(IEnumerable<PfamDnaParameters> family)
		{
			if (!family.Any())
			{
				return null;
			}

			var firstPfam = family.First().PfamFile;
			var averageA = family.Select(f => f.DnaParametersA).Average();
			var averageB = family.Select(f => f.DnaParametersB).Average();
			var averageC = family.Select(f => f.DnaParametersС).Average();

			return new PfamDnaParameters(firstPfam, averageA, averageB, averageC);
		}

		private static PfamDnaParametersA CreatePfamDnaParametersA(HashSet<int> positions, string parseString)
		{
			(var firstIndex, var secondIndex) = GetIndexes(parseString, '-');

			if (positions.Contains(firstIndex) && positions.Contains(secondIndex))
			{
				var splitted = parseString.Substring(21).Split(' ', StringSplitOptions.RemoveEmptyEntries);

				float xdisp = float.Parse(splitted[0], CultureInfo.InvariantCulture);
				float ydisp = float.Parse(splitted[1], CultureInfo.InvariantCulture);
				float inclin = float.Parse(splitted[2], CultureInfo.InvariantCulture);
				float tip = float.Parse(splitted[3], CultureInfo.InvariantCulture);

				float? axBend = null;
				if (float.TryParse(splitted[4], NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
				{
					axBend = result;
				}

				return new PfamDnaParametersA(xdisp, ydisp, inclin, tip, axBend);
			}
			else
			{
				return null;
			}
		}

		private static PfamDnaParametersB CreatePfamDnaParametersB(HashSet<int> positions, string parseString)
		{
			(var firstIndex, var secondIndex) = GetIndexes(parseString, '-');

			if (positions.Contains(firstIndex) && positions.Contains(secondIndex))
			{
				var splitted = parseString.Substring(21).Split(' ', StringSplitOptions.RemoveEmptyEntries);

				float shear = float.Parse(splitted[0], CultureInfo.InvariantCulture);
				float stretch = float.Parse(splitted[1], CultureInfo.InvariantCulture);
				float stagger = float.Parse(splitted[2], CultureInfo.InvariantCulture);
				float buckle = float.Parse(splitted[3], CultureInfo.InvariantCulture);
				float propel = float.Parse(splitted[4], CultureInfo.InvariantCulture);
				float opening = float.Parse(splitted[5], CultureInfo.InvariantCulture);

				return new PfamDnaParametersB(shear, stretch, stagger, buckle, propel, opening);
			}
			else
			{
				return null;
			}
		}

		private static PfamDnaParametersС CreatePfamDnaParametersC(HashSet<int> positions, string parseString)
		{
			(var firstIndex, var secondIndex) = GetIndexes(parseString, '/');

			if (positions.Contains(firstIndex) && positions.Contains(secondIndex))
			{
				var splitted = parseString.Substring(21).Split(' ', StringSplitOptions.RemoveEmptyEntries);

				float shift = float.Parse(splitted[0], CultureInfo.InvariantCulture);
				float slide = float.Parse(splitted[1], CultureInfo.InvariantCulture);
				float rise = float.Parse(splitted[2], CultureInfo.InvariantCulture);
				float tilt = float.Parse(splitted[3], CultureInfo.InvariantCulture);
				float roll = float.Parse(splitted[4], CultureInfo.InvariantCulture);
				float twist = float.Parse(splitted[5], CultureInfo.InvariantCulture);

				return new PfamDnaParametersС(shift, slide, rise, tilt, roll, twist);
			}
			else
			{
				return null;
			}
		}

		private static (int, int) GetIndexes(string searchString, char delimiter)
		{
			int firstIndex = int.Parse(searchString.Substring(9).Split(new char[] { delimiter, ' '}, StringSplitOptions.RemoveEmptyEntries)[0]);
			int secondIndex = int.Parse(searchString.Substring(15).Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);

			return (firstIndex, secondIndex);
		}

	}
}
