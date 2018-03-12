using Newtonsoft.Json;

namespace Collector.Common.Validation.AccountNumber.Models
{
    public class AccountValidationModel
    {
        [JsonRequired]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonRequired]
        [JsonProperty("clearingRegex")]
        public string ClearingRegex { get; set; }

        [JsonRequired]
        [JsonProperty("numberRegex")]
        public string NumberRegex { get; set; }

        [JsonRequired]
        [JsonProperty("modulo")]
        public int Modulo { get; set; }

        [JsonRequired]
        [JsonProperty("clearing")]
        public int Clearing { get; set; }

        [JsonRequired]
        [JsonProperty("account")]
        public int Account { get; set; }

        [JsonRequired]
        [JsonProperty("control")]
        public int Control { get; set; }
    }
}