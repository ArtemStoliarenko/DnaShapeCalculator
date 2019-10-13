using DnaShapeCalculator.Core;
using System;
using System.IO;

namespace DnaShapeCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
			var file = File.ReadAllLines("pdbmap");
			var pdbMap = PdbMapFactory.CreatePdbMap(file);

			Console.WriteLine("Hello World!");
        }
    }
}
