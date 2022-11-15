using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Sapl.Pdp.Api
{
    /**
     * Container for a decision
     */
    public class AuthorizationDecision
    {
        [JsonProperty("decision")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Decision Decision { get; set; } = Decision.INDETERMINATE;

        //@JsonInclude(Include.NON_ABSENT)
        [JsonProperty("resource")]
        public JToken Resource = null;

        //@JsonInclude(Include.NON_ABSENT)
        [JsonProperty("obligations")]
        public JToken Obligations { get; set; } = null;

        //@JsonInclude(Include.NON_ABSENT)
        [JsonProperty("advice")]
        public JToken Advice { get; set; } = null;


        public AuthorizationDecision()
        {
        }

        public AuthorizationDecision(Decision decision)
        {
            this.Decision = decision;
        }

        public AuthorizationDecision(Decision decision, JToken resource, JToken obligations, JToken advice)
        {
            this.Decision = decision;
            this.Resource = resource;
            this.Obligations = obligations;
            this.Advice = advice;
        }

        public override string ToString()
        {
            string strResources = Resource == null ? "null" : Resource.ToString();
            string strObligations = Obligations == null ? "null" : Obligations.ToString();
            string strAdvice = Advice == null ? "null" : Advice.ToString();

            return "AuthorizationDecision(" + Decision.ToString() + ", " + strResources + ", " + strObligations + ", " + strAdvice + ")";
        }
    }
}


