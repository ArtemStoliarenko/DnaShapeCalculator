using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core
{
	public class EmptyPdbFileProcessor : IPdbFileProcessor
	{
		public bool ProcessFile() => true;
	}
}
