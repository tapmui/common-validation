namespace Collector.Common.Validation.NationalIdentifier
{
    public interface INationalIdentifierValidator
    {

        bool IsValid(string nationalIdentifier);
        bool IsValid(int nationalIdentifier);
        string Normalize(string nationalIdentifier);
        string Normalize(int nationalIdentifier);
    }
}