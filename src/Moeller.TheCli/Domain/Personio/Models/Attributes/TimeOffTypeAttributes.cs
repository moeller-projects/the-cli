using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Attributes
{
    public class TimeOffTypeAttributes
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "certificationRequired")]
        public bool CertificationRequired { get; set; }

        [JsonProperty(PropertyName = "certificationSubmissionTimeframe")]
        public int CertificationSubmissionTimeframe { get; set; }

        [JsonProperty(PropertyName = "halfDayRequestsEnabled")]
        public bool HalfDayRequestsEnabled { get; set; }

        [JsonProperty(PropertyName = "legacyCategory")]
        public string LegacyCategory { get; set; }

        [JsonProperty(PropertyName = "substituteOption")]
        public string SubstituteOption { get; set; }

        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }

        public static implicit operator TimeOffType(TimeOffTypeAttributes x)
            => new TimeOffType()
            {
                Category = x.Category,
                CertificationRequired = x.CertificationRequired,
                CertificationSubmissionTimeframe = x.CertificationSubmissionTimeframe,
                HalfDayRequestsEnabled = x.HalfDayRequestsEnabled,
                Id = x.Id,
                LegacyCategory = x.LegacyCategory,
                Name = x.Name,
                SubstituteOption = x.SubstituteOption,
                Unit = x.Unit
            };

        public TimeOffType ToTimeOffType() => (TimeOffType)this;
    }
}
