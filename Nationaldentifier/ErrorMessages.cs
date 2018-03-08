namespace Collector.Common.Validation.NationalIdentifier
{
    public static class ErrorMessages
    {
        public static string GetInvalidIdentifierMessage(NationalIdentifierCountry nationalCountry) =>
            $"The given national Identifier is not valid for the country {nationalCountry}";
    }
}