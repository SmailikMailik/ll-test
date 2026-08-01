using Newtonsoft.Json;

namespace LL.Game.Data.Persistence.Documents
{
    [JsonConverter(typeof(RankUpRequirementDocumentEntryJsonConverter))]
    internal abstract class RankUpRequirementDocumentEntry
    {
        public string RequirementId { get; }

        protected RankUpRequirementDocumentEntry(string requirementId)
        {
            RequirementId = requirementId;
        }
    }
}