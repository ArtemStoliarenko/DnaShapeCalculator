using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core.Entities
{
	public interface IFamilyMappingRecord
	{
		string PdbCode { get; }

		string Family { get; }
	}
}
