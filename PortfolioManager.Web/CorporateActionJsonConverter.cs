using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PortfolioManager.RestApi.CorporateActions;

namespace PortfolioManager.Web
{
    public class CorporateActionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(CorporateAction));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            if (jsonObject.TryGetValue("type", out var jsonToken))
            {
                var corporateActionType = jsonToken.Value<string>();

                if (corporateActionType == "capitalreturn")
                    return jsonObject.ToObject<CapitalReturn>();
                else if (corporateActionType == "composite")
                    return jsonObject.ToObject<CompositeAction>();
                else if (corporateActionType == "dividend")
                    return jsonObject.ToObject<Dividend>();
                else if (corporateActionType == "splitconsolidation")
                    return jsonObject.ToObject<SplitConsolidation>();
                else if (corporateActionType == "transformation")
                    return jsonObject.ToObject<Transformation>();
            }

            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
