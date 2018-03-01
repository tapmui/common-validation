namespace Collector.Common.Validation.NationalIdentifier
{
    public class SwedishNationalIdentifierValidator : NationalIdentifierValidator
    {
        public override NationalIdentifierCountry NationalCountry { get; }
        public override bool IsValid(string nationalIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsValid(int nationalIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public override string Normalize(string nationalIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public override string Normalize(int nationalIdentifier)
        {
            throw new System.NotImplementedException();
        }
    }
}