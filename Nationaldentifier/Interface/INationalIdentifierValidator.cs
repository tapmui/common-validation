using System;

namespace Collector.Common.Validation.NationalIdentifier.Interface
{
    public interface INationalIdentifierValidator
    {
        bool IsValid(string nationalIdentifier);
		ParsedNationalIdentifierData Parse(string nationalIdentifier);
		string Normalize(string nationalIdentifier);

	}
	public class ParsedNationalIdentifierData 
	{
		public bool Valid { get; set; }
		public Gender Gender { get; set; }
		public int? AgeInYears { get; set; }
		public DateTime DateOfBirth { get; set; }

	}
	public enum Gender
	{
		UNKNOWN,
		FEMALE,
		MALE
	}
	
}