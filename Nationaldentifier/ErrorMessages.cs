namespace Collector.Common.Validation.NationalIdentifier
{
    public static class ErrorMessages
    {
        public static string GetInvalidIdentifierMessage(string nationalIdentifier, CountryCode countryCode) =>
            $"The given national Identifier '{nationalIdentifier}' is not valid for the country {countryCode}";
    }
}