namespace Collector.Common.Validation.NationalIdentifier.Interface
{
    public interface INationalIdentifierValidator
    {
        bool   IsValid(string nationalIdentifier);
        bool   IsValid(long nationalIdentifier);
        string Normalize(string nationalIdentifier);
    }
}