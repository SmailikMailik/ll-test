using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RankUpRequirementDocumentEntryJsonConverter : JsonConverter
    {
        private const string TypeProperty = "Type";
        private const string QuestType = "quest";
        private const string PaymentType = "payment";

        public RankUpRequirementDocumentEntryJsonConverter() { }

        public override bool CanConvert(Type objectType)
        {
            return typeof(RankUpRequirementDocumentEntry).IsAssignableFrom(objectType);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var document = JObject.Load(reader);
            var type = document.Value<string>(TypeProperty);
            var requirementId = document.Value<string>(nameof(RankUpRequirementDocumentEntry.RequirementId));

            return type switch
            {
                QuestType => ReadQuest(document, requirementId),
                PaymentType => ReadPayment(document, requirementId, serializer),
                _ => throw new JsonSerializationException($"Unsupported rank-up requirement type '{type}'.")
            };
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            switch (value)
            {
                case QuestRankUpRequirementDocumentEntry quest:
                    WriteCommonProperties(writer, QuestType, quest.RequirementId);
                    WriteProperty(writer, nameof(quest.QuestId), quest.QuestId);
                    WriteProperty(writer, nameof(quest.RequiredCount), quest.RequiredCount);
                    WriteProperty(writer, nameof(quest.DurationMinutes), quest.DurationMinutes);
                    break;
                case PaymentRankUpRequirementDocumentEntry payment:
                    WriteCommonProperties(writer, PaymentType, payment.RequirementId);
                    writer.WritePropertyName(nameof(payment.Payment));
                    serializer.Serialize(writer, payment.Payment);
                    break;
                default:
                    throw new JsonSerializationException(
                        $"Unsupported rank-up requirement type '{value.GetType().Name}'.");
            }

            writer.WriteEndObject();
        }

        private static QuestRankUpRequirementDocumentEntry ReadQuest(
            JObject document,
            string requirementId)
        {
            EnsureKnownProperties(
                document,
                TypeProperty,
                nameof(RankUpRequirementDocumentEntry.RequirementId),
                nameof(QuestRankUpRequirementDocumentEntry.QuestId),
                nameof(QuestRankUpRequirementDocumentEntry.RequiredCount),
                nameof(QuestRankUpRequirementDocumentEntry.DurationMinutes));

            return new QuestRankUpRequirementDocumentEntry(
                requirementId,
                document.Value<string>(nameof(QuestRankUpRequirementDocumentEntry.QuestId)),
                document.Value<int>(nameof(QuestRankUpRequirementDocumentEntry.RequiredCount)),
                document.Value<int>(nameof(QuestRankUpRequirementDocumentEntry.DurationMinutes)));
        }

        private static PaymentRankUpRequirementDocumentEntry ReadPayment(
            JObject document,
            string requirementId,
            JsonSerializer serializer)
        {
            EnsureKnownProperties(
                document,
                TypeProperty,
                nameof(RankUpRequirementDocumentEntry.RequirementId),
                nameof(PaymentRankUpRequirementDocumentEntry.Payment));

            return new PaymentRankUpRequirementDocumentEntry(
                requirementId,
                document[nameof(PaymentRankUpRequirementDocumentEntry.Payment)]
                    ?.ToObject<PaymentDocumentEntry>(serializer));
        }

        private static void EnsureKnownProperties(JObject document, params string[] propertyNames)
        {
            foreach (var property in document.Properties())
            {
                if (Array.IndexOf(propertyNames, property.Name) < 0)
                    throw new JsonSerializationException($"Unknown rank-up requirement property '{property.Name}'.");
            }
        }

        private static void WriteCommonProperties(
            JsonWriter writer,
            string type,
            string requirementId)
        {
            WriteProperty(writer, TypeProperty, type);
            WriteProperty(writer, nameof(RankUpRequirementDocumentEntry.RequirementId), requirementId);
        }

        private static void WriteProperty(JsonWriter writer, string name, string value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        private static void WriteProperty(JsonWriter writer, string name, int value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }
    }
}