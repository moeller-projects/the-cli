using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models
{
    public class TimeOffType
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// family_care maternity_parental_leave child_care short_time_allowance 
        /// quarantine lockout irrevocable_exemption sick_leave voluntary_military_service 
        /// unlawful_strike lawful_strike paid_vacation unpaid_vacation unexcused_absence 
        /// offsite_work other undefined
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        /// <summary>
        /// family_care_sick_leave individual_prohibition_of_employment maternity_protection_period 
        /// other paid_vacation parental_leave sick_leave lawful_strike unlawful_strike treatment 
        /// unexcused_absence unpaid_vacation voluntary_military_service offsite_work 
        /// family_care_long_term paid_child_sick unpaid_child_sick undefined
        /// </summary>
        [JsonProperty(PropertyName = "legacy_category")]
        public string LegacyCategory { get; set; }

        /// <summary>
        /// day hour
        /// </summary>
        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }

        [JsonProperty(PropertyName = "half_day_requests_enabled")]
        public bool HalfDayRequestsEnabled { get; set; }

        [JsonProperty(PropertyName = "certification_required")]
        public bool CertificationRequired { get; set; }

        /// <summary>
        /// The timeframe in days under which the employee needs to submit the certification
        /// </summary>
        [JsonProperty(PropertyName = "certification_submission_timeframe")]
        public int CertificationSubmissionTimeframe { get; set; }

        /// <summary>
        /// disabled optional required
        /// </summary>
        [JsonProperty(PropertyName = "substitute_option")]
        public string SubstituteOption { get; set; }

        [JsonProperty(PropertyName = "approval_required")]
        public bool ApprovalRequired { get; set; }
    }
}
