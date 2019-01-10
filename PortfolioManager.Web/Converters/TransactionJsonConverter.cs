using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PortfolioManager.Web;
using PortfolioManager.RestApi.Transactions;

namespace PortfolioManager.Web.Converters
{
    public class TransactionJsonConverter : JsonConverter
    {
        private Dictionary<string, Type> _TransactionTypes = new Dictionary<string, Type>();

        public TransactionJsonConverter()
        {
            _TransactionTypes.Add("aquisition", typeof(Aquisition));
            _TransactionTypes.Add("disposal", typeof(Disposal));
            _TransactionTypes.Add("cashtransaction", typeof(CashTransaction));
            _TransactionTypes.Add("openingbalance", typeof(OpeningBalance));
            _TransactionTypes.Add("incomereceived", typeof(IncomeReceived));
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(RestApi.Transactions.Transaction));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            if (jsonObject.TryGetValue("type", out var jsonToken))
            {
                var transactionType = jsonToken.Value<string>();

                if (_TransactionTypes.TryGetValue(transactionType, out var type))
                    return jsonObject.ToObject(type, serializer);
                else
                    return null;
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
