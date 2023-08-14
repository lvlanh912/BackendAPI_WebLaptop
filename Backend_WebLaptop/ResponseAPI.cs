using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Backend_WebLaptop
{
    public class ResponseAPI<T>
    {
        public string Status { get; set; } = "Sucess";
        public T? Result { get; set; }
        public string? Message { get; set; }
    public string Format()
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling= NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = true,
                IgnoreSerializableInterface = true,
                IgnoreShouldSerializeMembers = true
            }
        };
        return JsonConvert.SerializeObject(this, settings).ToString();
    }
    }
}
