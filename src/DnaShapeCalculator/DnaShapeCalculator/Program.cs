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

		static void Main(string[] args)
		{
			var allowedPdbs = Task.Run(() => GetAllowedPdbCodes("dna"));

			var pdbMapRecords = GetPdbMapRecords("pdbmap");

			pdbMapRecords = new PdbMapRecordFilter(pdbMapRecords, allowedPdbs.Result, domainCountThreshold).Filter();

			var pfamRecords = new PfamRecordFactory(pdbMapRecords).CreatePfamFileHandles(new DirectoryInfo("pfam/").GetFiles("*.pdb"), true);

			pfamRecords = pfamRecords.GroupBy(pf => pf.Family)
				.Where(pf => pf.GroupBy(domainGroup => domainGroup.Domain).Count() >= domainCountThreshold)
				.SelectMany(pf => pf)
				.ToArray();

			var pdbFileInfo = GetPdbFileList(pfamRecords);

			Console.WriteLine($"PDB index built; {pdbFileInfo.Length} PDBs to process; {pfamRecords.Length} PFAM patterns.");

			if (!Directory.Exists("results_pdb"))
			{
				Directory.CreateDirectory("results_pdb");
			}

			var results = pdbFileInfo.AsParallel()
				.Select(pdb => new PdbFileProcessor(pdb, "results_pdb").ProcessFile());

			var failues = results.Count(r => !r);

			Console.WriteLine($"Dna shape extraction is complete; {failues} files unprocessed!");
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
				.Where(file => pdbCodesToRun.Contains(file.Name.Substring(3, 4)))
				.ToArray();
		}

		private static HashSet<string> GetAllowedPdbCodes(string directoryName)
		{
			var records = PdbRecordFactory.GetRecords(directoryName)
				.Where(IsAllowedPdbFile)
				.Select(r => r.PdbCode);

			return new HashSet<string>(records);
		}

		private static bool IsAllowedPdbFile(PdbRecord pdbRecord)
		{
			return pdbRecord.ExperimentType == allowedExperimentType && pdbRecord.Resolution.HasValue && pdbRecord.Resolution < resolutionThreshold;
		}
	}
}
