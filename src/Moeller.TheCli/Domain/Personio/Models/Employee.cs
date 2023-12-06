using Moeller.TheCli.Domain.Personio.Util;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models
{
    public class Employee
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "position")]
        public string Position { get; set; }

        [JsonProperty(PropertyName = "subcompany")]
        public string Subcompany { get; set; }

        [JsonProperty(PropertyName = "department")]
        public string Department { get; set; }

        [JsonProperty(PropertyName = "office")]
        public string Office { get; set; }

        [JsonProperty(PropertyName = "hire_date"), JsonConverter(typeof(DateFormatConverter))]
        public string HireDate { get; set; }

        [JsonProperty(PropertyName = "weekly_working_hours")]
        public decimal WeeklyWorkingHours { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "supervisor_id")]
        public int SupervisorId { get; set; }

        [JsonProperty(PropertyName = "custom_attributes")]
        public Dictionary<string, string> CustomAttributes { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}, {Id}";
        }
    }
}
