using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PortfolioManager.Web
{
    /*
    public class TransactionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Transaction));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            if (jsonObject.TryGetValue("Type", out var jsonToken))
            {
                var transactionType = (TransactionType)jsonToken.Value<int>();

                if (transactionType == TransactionType.Aquisition)
                    return jsonObject.ToObject<Aquisition>();
                else if (transactionType == TransactionType.Disposal)
                    return jsonObject.ToObject<Disposal>();
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
    }*/

}
