using Moeller.TheCli.Domain.Personio.Models.Response;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Attributes
{
    public class EmployeeAttributes
    {
        [JsonProperty(PropertyName = "id")]
        public AttributeObject<int> Id { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public AttributeObject<string> FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public AttributeObject<string> LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public AttributeObject<string> Email { get; set; }

        public static implicit operator Employee(EmployeeAttributes x)
            => x != null ? new Employee()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            } : null;

        public Employee ToEmployee() => (Employee)this;

    }
}
