
using Newtonsoft.Json;

namespace Sapl.Pdp.Api
{
    /**
     * Holds a {@link AuthorizationDecision SAPL authorization decision} together with the ID
     * of the corresponding {@link AuthorizationSubscription SAPL authorization subscription}.
     *
     * @see AuthorizationDecision
     * @see IdentifiableAuthorizationSubscription
     */
    //@Data
    //@JsonInclude(NON_NULL)
    public class IdentifiableAuthorizationDecision
    {
        //	@JsonProperty(required = true)
        [JsonProperty("authorizationSubscriptionId")]
        public string AuthorizationSubscriptionId { get; set; } = null;

        //	@JsonProperty(required = true)
        [JsonProperty("authorizationDecision")]
        public AuthorizationDecision AuthorizationDecision { get; set; } = null;

        public IdentifiableAuthorizationDecision()
        {
        }

        public IdentifiableAuthorizationDecision(string authorizationSubscriptionId, AuthorizationDecision authorizationDecision)
        {
            this.AuthorizationSubscriptionId = authorizationSubscriptionId;
            this.AuthorizationDecision = authorizationDecision;
        }
    }
}
