using DnaShapeCalculator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DnaShapeCalculator.Core
{
	public static class PdbMapFactory
	{
		private const int pdbMapRecordLength = 6;
		private const int pdbCodeLength = 4;

		private static readonly char[] pdbMapSeparator = { ';', ' ', '\t' };
		private static readonly Regex pdbMapCoordinatesRegex = new Regex("[0-9]{1,3}-[0-9]{1,3}", RegexOptions.Compiled);

		public static PdbMap CreatePdbMap(string[] records)
		{
			var pdbMapRecords = records.Select(CreatePdbMapRecord)
				.Where(record => record != null).ToList();
			return new PdbMap(pdbMapRecords);
		}

		private static PdbMapRecord CreatePdbMapRecord(string record)
		{
			var pdbMapData = record.Split(pdbMapSeparator, StringSplitOptions.RemoveEmptyEntries);

			if (!ValidatePdbMapData(pdbMapData))
			{
				return null;
			}

			var coordinates = pdbMapData[5].Split('-', StringSplitOptions.RemoveEmptyEntries);
			return new PdbMapRecord(pdbMapData[0], pdbMapData[1], pdbMapData[3], pdbMapData[4], int.Parse(coordinates[0]), int.Parse(coordinates[1]));
		}

		private static bool ValidatePdbMapData(string[] pdbMapData)
		{
			if (pdbMapData.Length != pdbMapRecordLength)
			{
				return false;
			}

			if (pdbMapData[0].Length != pdbCodeLength)
			{
				return false;
			}

			if (pdbMapData[3].Length == 0)
			{
				return false;
			}

			if (pdbMapData[4].Length == 0)
			{
				return false;
			}

			if (!pdbMapCoordinatesRegex.IsMatch(pdbMapData[5]))
			{
				return false;
			}

			return true;
		}
	}
}
