using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Models.Response
{
    public class TypeAndAttributesObject<T>
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public T Attributes { get; set; }
    }

    public class AttributeObject<T>
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "value")]
        public T Value { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "universal_id")]
        public string UniversalId { get; set; }

        public static implicit operator T(AttributeObject<T> attributeObject) => attributeObject != null ? attributeObject.Value : default;
    }


}
