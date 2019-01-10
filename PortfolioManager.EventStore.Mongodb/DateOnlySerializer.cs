using System;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;


using PortfolioManager.Common;

namespace PortfolioManager.EventStore.Mongodb
{
    [BsonSerializer(typeof(DateOnlySerializer))]
    class DateOnlySerializer : SerializerBase<DateTime>
    {
        public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.String)
            {
                var value = context.Reader.ReadString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return DateUtils.NoDate;
                }
                if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                    return date.Date;
                else
                    return DateUtils.NoDate;

            }
            context.Reader.SkipValue();
            return DateUtils.NoDate;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }
            context.Writer.WriteString(value.ToString("yyyy-MM-dd"));
        }
    }
}
