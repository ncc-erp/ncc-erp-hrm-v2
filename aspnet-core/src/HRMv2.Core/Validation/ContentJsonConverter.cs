using HRMv2.Manager.Notifications.SendMezonDM.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Validation
{
    public class ContentJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ContentMezonDM content)
            {
                var contentJson = JsonConvert.SerializeObject(content);
                writer.WriteValue(contentJson);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.StartObject)
            {
                var jsonObject = JObject.Load(reader);
                return jsonObject.ToObject<ContentMezonDM>();
            }
            if (reader.TokenType == JsonToken.String)
            {
                var jsonString = reader.Value.ToString();
                return JsonConvert.DeserializeObject<ContentMezonDM>(jsonString);
            }
            throw new JsonSerializationException("Unexpected token type: " + reader.TokenType);

        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ContentMezonDM).IsAssignableFrom(objectType);
        }
    }
}
