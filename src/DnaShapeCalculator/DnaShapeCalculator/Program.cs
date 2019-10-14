using DnaShapeCalculator.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DnaShapeCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
			var pdbMapTask = Task.Run(() =>
			{
				var file = File.ReadAllLines("pdbmap");
				return PdbDataFactory.CreatePdbMapRecords(file);
			});

			var pdbPfamMappingTask = Task.Run(() =>
			{
				var file = File.ReadAllLines("pdb_pfam_mapping.txt");
				return PdbDataFactory.CreatePdbPfamMapRecords(file);
			});

			Task.WhenAll(pdbMapTask, pdbPfamMappingTask).Wait();

			Console.WriteLine("Hello World!");
        }
    }
}
