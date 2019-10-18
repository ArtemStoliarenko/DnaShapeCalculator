using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DnaShapeCalculator.Core.Entities
{
	public sealed class PfamRecordFactory
	{
		private const int minimumFilenameSections = 4;
		private const int pdbCodeIndex = 0;
		private const int strandIndex = 1;
		private const int startCoordinateIndex = 2;
		private const int endCoordinateIndex = 3;

		private const int minPdbLength = 6;
		private const int pdbFieldIdIndex = 0;
		private const int pdbChemicalElementId = 2;
		private const int pdbStructuralId = 3;
		private const int pdbStrandId = 4;
		private const int proteinSequenceId = 5;

		private static readonly char[] filenameSeparators = { '.', '_', '-' };
		private const char pdbSeparator = ' ';
		private const string atomFieldType = "ATOM";
		private const string phosporusName = "P";
		private static readonly Regex isDnaRegex = new Regex("D(A|T|G|C)", RegexOptions.Compiled);

		private readonly PdbMap pdbMap;

		public PfamRecordFactory(PdbMapRecord[] records)
		{
			if (records == null)
			{
				throw new ArgumentNullException(nameof(records));
			}
			if (records.Length == 0)
			{
				throw new ArgumentException(nameof(records));
			}

			this.pdbMap = new PdbMap(records);
		}

		public PfamFile[] CreatePfamFileHandles(FileInfo[] files, bool asParallel = false)
		{
			if (asParallel)
			{
				return files.AsParallel()
					.Select(GetPfamFileHandle)
					.Where(handle => handle != null)
					.ToArray();
			}
			else
			{
				return files
					.Select(GetPfamFileHandle)
					.Where(handle => handle != null)
					.ToArray();
			}
		}

		private PfamFile GetPfamFileHandle(FileInfo fileInfo)
		{
			(var pdbCode, var strand, var startCoordinate, var endCoordinate) = ParseFilename(fileInfo.Name);

			if (!ValidateFilenameValues(pdbCode, strand, startCoordinate, endCoordinate))
			{
				return null;
			}

			var record = pdbMap.GetPdbMapRecord(pdbCode, strand, startCoordinate, endCoordinate);
			if (record == null)
			{
				return null;
			}

			var file = File.ReadAllLines(fileInfo.FullName);
			var positions = new HashSet<int>
				(file.Select(GetPositionFromLine)
				.Where(pos => pos.HasValue)
				.Select(pos => pos.Value));
			
			if (!ValidatePositions(positions))
			{
				return null;
			}

			return new PfamFile(fileInfo.FullName, record, positions);
		}


		private static (string pdbCode, string strand, int startCoordinate, int EndCoordinate) ParseFilename(string name)
		{
			var toParse = Path.GetFileNameWithoutExtension(name);
			var splitted = toParse.Split(filenameSeparators, StringSplitOptions.RemoveEmptyEntries);

			if (splitted.Length < minimumFilenameSections)
			{
				return (null, null, 0, 0);
			}

			var pdbCode = splitted[pdbCodeIndex].ToUpperInvariant();
			var strand = splitted[strandIndex].ToUpperInvariant();
			int.TryParse(splitted[startCoordinateIndex], out var startCoordinate);
			int.TryParse(splitted[endCoordinateIndex], out var endCoordinate);

			return (pdbCode, strand, startCoordinate, endCoordinate);
		}

		private static int? GetPositionFromLine(string line)
		{
			line = line.TrimStart();

			if (!string.IsNullOrEmpty(line) && line.StartsWith(atomFieldType))
			{
				var extracted = ExtractColumnsFromPdb(line);
				if (phosporusName.Equals(extracted.atom, StringComparison.OrdinalIgnoreCase) && isDnaRegex.IsMatch(extracted.residue))
				{
					return int.Parse(extracted.coordinate);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		private static (string atom, string residue, string coordinate) ExtractColumnsFromPdb(string line)
		{
			var atom = line.Substring(13, 4).Trim();
			var residue = line.Substring(17, 3).Trim();
			var coordinate = line.Substring(23, 3).Trim();

			return (atom, residue, coordinate);
		}

		private static bool ValidateFilenameValues(string pdbCode, string strand, int startCoordinate, int endCoordinate) =>
			!string.IsNullOrEmpty(pdbCode) && !string.IsNullOrEmpty(strand) && startCoordinate >= 0 && endCoordinate >= startCoordinate;

		private bool ValidatePositions(HashSet<int> positions) => positions != null && positions.Count != 0;
	}
}
