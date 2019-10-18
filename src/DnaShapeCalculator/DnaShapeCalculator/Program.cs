﻿using DnaShapeCalculator.Core;
using DnaShapeCalculator.Core.Entities;
using DnaShapeCalculator.PdbMapRecordFilters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DnaShapeCalculator
{
	class Program
	{
		private const ExperimentType allowedExperimentType = ExperimentType.Xray;
		private const float resolutionThreshold = 3.5F;

		private const int domainCountThreshold = 3;

		private const string dnaFolderName = "dna";
		private const string pfamFolderName = "pfam/";
		private const string pdbMapName = "pdbmap";
		private const string pdbPfamMappingName = "pdb_pfam_mapping.txt";
		private const string pdbFileMask = "*.pdb";
		private const string pdbResultFolderName = "results_pdb";
		private const string resultFileExtension = "*.lis";

		static void Main(string[] args)
		{
			var allowedPdbs = Task.Run(() => GetAllowedPdbCodes(dnaFolderName));

			var pdbMapRecords = GetPdbMapRecords(pdbMapName);

			pdbMapRecords = new PdbMapRecordFilter(pdbMapRecords, allowedPdbs.Result, domainCountThreshold).Filter();

			var pfamRecords = new PfamRecordFactory(pdbMapRecords).CreatePfamFileHandles(new DirectoryInfo(pfamFolderName).GetFiles(pdbFileMask), true);

			pfamRecords = pfamRecords.GroupBy(pf => pf.Family)
				.Where(pf => pf.GroupBy(domainGroup => domainGroup.Domain).Count() >= domainCountThreshold)
				.SelectMany(pf => pf)
				.ToArray();

			var pdbFileInfo = GetPdbFileList(pfamRecords);

			Console.WriteLine($"PDB index built; {pdbFileInfo.Length} PDBs to process; {pfamRecords.Length} PFAM patterns.");

			if (!Directory.Exists(pdbResultFolderName))
			{
				Directory.CreateDirectory(pdbResultFolderName);
			}

			var results = pdbFileInfo.AsParallel()
				.Select(pdb => new EmptyPdbFileProcessor().ProcessFile())
				.ToArray();

			var failues = results.Count(r => !r);
			var succeeded = results.Length - failues;

			Console.WriteLine($"Dna shape extraction is complete; {succeeded} files processed; {failues} files failed to process!");

			var resultPdbCodes = GetResultPdbCodes();

			var resultGroups = pfamRecords.Where(pf => resultPdbCodes.Contains(pf.PdbCode))
				.GroupBy(pf => pf.Family)
				.Where(group => group.GroupBy(innerGroup => innerGroup.Domain).Count() >= domainCountThreshold);

			var familyCount = resultGroups.Count();
			pfamRecords = resultGroups.SelectMany(group => group)
			   .ToArray();

			Console.WriteLine($"After running Curves+ there are {pfamRecords.Length} PFAM records and {familyCount} families!");

			Console.ReadLine();
		}

		private static PdbMapRecord[] GetPdbMapRecords(string filename)
		{
			var file = File.ReadAllLines(filename);

			return PdbMappingDataFactory.CreatePdbMapRecords(file);
		}

		private static PdbPfamMapRecord[] GetPdbPfamMapRecords(string filename)
		{
			var file = File.ReadAllLines(filename);

			return PdbMappingDataFactory.CreatePdbPfamMapRecords(file);
		}

		private static FileInfo[] GetPdbFileList(PfamFile[] pfamRecords)
		{
			var pdbCodesToRun = new HashSet<string>(
				pfamRecords.GroupBy(group => group.PdbCode)
				.Select(group => group.Key), StringComparer.OrdinalIgnoreCase);

			return new DirectoryInfo("dna").GetFiles("*.pdb")
				.Where(file => pdbCodesToRun.Contains(GetPdbCodeFromFileInfo(file)))
				.ToArray();
		}

		private static HashSet<string> GetAllowedPdbCodes(string directoryName)
		{
			var records = PdbRecordFactory.GetRecords(directoryName)
				.Where(IsAllowedPdbFile)
				.Select(r => r.PdbCode);

			return new HashSet<string>(records);
		}

		private static HashSet<string> GetResultPdbCodes()
		{
			return new HashSet<string>(new DirectoryInfo(pdbResultFolderName).GetFiles(resultFileExtension)
				.Select(GetPdbCodeFromFileInfo));
		}

		private static string GetPdbCodeFromFileInfo(FileInfo pdbFileInfo) => pdbFileInfo.Name.Substring(3, 4).ToUpperInvariant();

		private static bool IsAllowedPdbFile(PdbRecord pdbRecord)
		{
			return pdbRecord.ExperimentType == allowedExperimentType && pdbRecord.Resolution.HasValue && pdbRecord.Resolution < resolutionThreshold;
		}
	}
}
