
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sapl.Pdp.Api
{
    /**
     * The authorization subscription object defines the tuple of objects constituting a SAPL
     * authorization subscription. Each authorization subscription consists of:
     * <ul>
     * <li>a subject describing the entity which is requesting permission</li>
     * <li>an action describing for which activity the subject is requesting permission</li>
     * <li>a resource describing or even containing the resource for which the subject is
     * requesting the permission to execute the action</li>
     * <li>an environment object describing additional contextual information from the
     * environment which may be required for evaluating the underlying policies.</li>
     * </ul>
     *
     * Are marshaled using the Jackson ObjectMapper. If omitted, a default mapper is used. A
     * custom mapper can be supplied.
     */
    //@Data
    //@JsonInclude(Include.NON_NULL)
    public class AuthorizationSubscription
    {
        /* TODO: xtr */
        //	private static final ObjectMapper MAPPER = new ObjectMapper().registerModule(new Jdk8Module());

        [JsonProperty("subject")]
        public JToken Subject { get; set; } = JValue.CreateString("");

        [JsonProperty("action")]
        public JToken Action { get; set; } = JValue.CreateString("");

        [JsonProperty("resource")]
        public JToken Resource { get; set; } = JValue.CreateString("");

        [JsonProperty("environment")]
        public JToken Environment { get; set; } = null;


        private Exchangeable<AuthorizationDecision> _exchangeable;

        [JsonIgnore]
        public Exchangeable<AuthorizationDecision> Exchangeable { get { return _exchangeable; } }

        public AuthorizationSubscription()
        {
        }

        public Exchangeable<AuthorizationDecision> setAuthorizationSubscription(JToken subject, JToken action, JToken resource, JToken environment = null)
        {
            if (subject == null)
                throw new ArgumentException("Parameter 'subject' cannot be null.");

            if (action == null)
                throw new ArgumentException("Parameter 'action' cannot be null.");

            if (resource == null)
                throw new ArgumentException("Parameter 'resource' cannot be null.");

            Subject = subject;
            Action = action;
            Resource = resource;
            Environment = environment;

            _exchangeable = new Exchangeable<AuthorizationDecision>(new AuthorizationDecision(Decision.INDETERMINATE));

            return _exchangeable;
        }
    }
}



