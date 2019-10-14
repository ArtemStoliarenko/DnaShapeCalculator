using DnaShapeCalculator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DnaShapeCalculator.Core
{
	public static class PdbDataFactory
	{
		private const int pdbMapRecordLength = 6;
		private const int pdbCodeLength = 4;

		private const int pdbPfamMapRecordLength = 5;

		private static readonly char[] pdbMapSeparators = { ';', ' ', '\t' };
		private static readonly Regex pdbMapCoordinatesRegex = new Regex("[0-9]{1,3}-[0-9]{1,3}", RegexOptions.Compiled);

		private static readonly char[] pdbPfamMappingSeparators = { '\t', '.' };

		public static PdbMapRecord[] CreatePdbMapRecords(string[] lines) => lines.Select(CreatePdbMapRecord).Where(record => record != null).ToArray();

		public static PdbPfamMapRecord[] CreatePdbPfamMapRecords(string[] lines) => lines.Skip(1)
			.Select(CreatePdbPfamMapRecord)
			.Where(record => record != null)
			.ToArray();

		private static PdbMapRecord CreatePdbMapRecord(string record)
		{
			var pdbMapData = record.Split(pdbMapSeparators, StringSplitOptions.RemoveEmptyEntries);

			if (!ValidatePdbMapData(pdbMapData))
			{
				return null;
			}

			var coordinates = pdbMapData[5].Split('-', StringSplitOptions.RemoveEmptyEntries);
			return new PdbMapRecord(pdbMapData[0], pdbMapData[1], pdbMapData[3], pdbMapData[4], int.Parse(coordinates[0]), int.Parse(coordinates[1]));
		}

		private static PdbPfamMapRecord CreatePdbPfamMapRecord(string record)
		{
			var pdbPfamMapData = record.Split(pdbPfamMappingSeparators, StringSplitOptions.RemoveEmptyEntries);
			return ValudatePdbPfamMapData(pdbPfamMapData) ?
				new PdbPfamMapRecord(pdbPfamMapData[0], pdbPfamMapData[1], pdbPfamMapData[4], pdbPfamMapData[2], pdbPfamMapData[3])
				: null;
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
			if (string.IsNullOrEmpty(pdbMapData[1]))
			{
				return false;
			}
			if (string.IsNullOrEmpty(pdbMapData[3]))
			{
				return false;
			}
			if (string.IsNullOrEmpty(pdbMapData[4]))
			{
				return false;
			}
			if (!pdbMapCoordinatesRegex.IsMatch(pdbMapData[5]))
			{
				return false;
			}

			return true;
		}

		private static bool ValudatePdbPfamMapData(string[] pdbPfamMapData)
		{
			if (pdbPfamMapData.Length < pdbPfamMapRecordLength)
			{
				return false;
			}
			if (pdbPfamMapData[0].Length != pdbCodeLength)
			{
				return false;
			}
			if (string.IsNullOrEmpty(pdbPfamMapData[1]))
			{
				return false;
			}
			if (string.IsNullOrEmpty(pdbPfamMapData[2]))
			{
				return false;
			}
			if (string.IsNullOrEmpty(pdbPfamMapData[3]))
			{
				return false;
			}
			if (string.IsNullOrEmpty(pdbPfamMapData[4]))
			{
				return false;
			}

			return true;
		}
	}
}
