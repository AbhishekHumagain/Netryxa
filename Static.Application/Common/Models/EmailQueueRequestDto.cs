using Newtonsoft.Json;

namespace Static.Application.Common.Models
{
    public class EmailQueueRequestDto
    {
        [JsonProperty("emailTemplateName")]
        public string EmailTemplateName { get; set; }
        [JsonProperty("toAddress")]
        public string ToAddress { get; set; }
        [JsonProperty("emailFrom")]
        public string EmailFrom { get; set; }
        [JsonProperty("templateData")]
        public Dictionary<string, string> TemplateData { get; set; }
    }
}
