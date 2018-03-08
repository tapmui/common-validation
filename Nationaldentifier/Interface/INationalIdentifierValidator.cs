namespace Collector.Common.Validation.NationalIdentifier.Interface
{
    public interface INationalIdentifierValidator
    {
        bool   IsValid(string nationalIdentifier);
        bool   IsValid(int nationalIdentifier);
        string Normalize(string nationalIdentifier);
        int    Normalize(int nationalIdentifier);
    }
}