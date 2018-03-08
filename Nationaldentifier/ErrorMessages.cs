namespace Collector.Common.Validation.NationalIdentifier
{
    public static class ErrorMessages
    {
        public static string GetInvalidIdentifierMessage(string nationalIdentifier ,NationalIdentifierCountry nationalCountry) =>
            $"The given national Identifier '{nationalIdentifier}' is not valid for the country {nationalCountry}";
    }
}