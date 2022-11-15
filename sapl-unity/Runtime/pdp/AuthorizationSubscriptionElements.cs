using Newtonsoft.Json;

namespace Sapl.Pdp.Api
{
    /**
     * Data structure holding IDs for the elements of an {@link AuthorizationSubscription}
     * SAPL authorization subscription.
     */
    public class AuthorizationSubscriptionElements
    {
        [JsonProperty("subjectId")]
        public int SubjectId { get; set; } = 0;

        [JsonProperty("actionId")]
        public int ActionId { get; set; } = 0;

        [JsonProperty("resourceId")]
        public int ResourceId { get; set; } = 0;

        [JsonProperty("environmentId", NullValueHandling = NullValueHandling.Ignore)]
        public int? EnvironmentId { get; set; } = null;

        public AuthorizationSubscriptionElements()
        {
        }

        public AuthorizationSubscriptionElements(int subjectId, int actionId, int resourceId, int? environmentId = null)
        {
            this.SubjectId = subjectId;
            this.ActionId = actionId;
            this.ResourceId = resourceId;
            this.EnvironmentId = environmentId;
        }
    }
}

