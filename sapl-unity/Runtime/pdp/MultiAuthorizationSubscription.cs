using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sapl.Pdp.Api
{
    /**
     * A multi-subscription holds a list of subjects, a list of actions, a list of resources,
     * a list of environments (which are the elements of a {@link AuthorizationSubscription
     * SAPL authorization subscription}) and a map holding subscription IDs and corresponding
     * {@link AuthorizationSubscriptionElements authorization subscription elements}. It
     * provides methods to
     * {@link #addAuthorizationSubscription(String, Object, Object, Object, Object) add}
     * single authorization subscriptions and to {@link #iterator() iterate} over all the
     * authorization subscriptions.
     *
     * @see AuthorizationSubscription
     */
    //@Data
    //@JsonInclude(NON_EMPTY)
    public class MultiAuthorizationSubscription // TODO: xtr implements Iterable<IdentifiableAuthorizationSubscription>
    {
        //	private static final ObjectMapper MAPPER = new ObjectMapper().registerModule(new Jdk8Module());

        [JsonProperty("subjects")]
        public ArrayList Subjects { get; } = new ArrayList();

        [JsonProperty("actions")]
        public ArrayList Actions { get; } = new ArrayList();

        [JsonProperty("resources")]
        public ArrayList Resources { get; } = new ArrayList();

        [JsonProperty("environments")]
        public ArrayList Environments { get; } = new ArrayList();

        [JsonProperty("authorizationSubscriptions")]
        public Dictionary<string, AuthorizationSubscriptionElements> AuthorizationSubscriptions { get; } = new Dictionary<string, AuthorizationSubscriptionElements>();

        [JsonIgnore]
        public Dictionary<string, Exchangeable<AuthorizationDecision>> AuthorizationExchangeables { get; } = new Dictionary<string, Exchangeable<AuthorizationDecision>>();

        private int authorizationSubscriptionID = 0;

        public MultiAuthorizationSubscription()
        {
        }

        /**
         * Convenience method to add an authorization subscription without environment data.
         * Calls {@link #addAuthorizationSubscription(String, Object, Object, Object)
         * addAuthorizationSubscription(subscriptionId, subject, action, resource, null)}.
         * @param subscriptionId the id identifying the authorization subscription to be
         * added.
         * @param subscription an authorization subscription.
         * @return this {@code MultiAuthorizationSubscription} instance to support chaining of
         * multiple calls to {@code addAuthorizationSubscription}.
         */
        public Exchangeable<AuthorizationDecision> addAuthorizationSubscription(AuthorizationSubscription subscription)
        {
            return addAuthorizationSubscription(subscription.Subject, subscription.Action, subscription.Resource, subscription.Environment);
        }

        /**
         * Adds the authorization subscription defined by the given subject, action, resource
         * and environment. The given {@code subscriptionId} is associated with the according
         * decision to allow the recipient of the PDP decision to correlate
         * subscription/decision pairs.
         * @param subscriptionId the id identifying the authorization subscription to be
         * added.
         * @param subject the subject of the authorization subscription to be added.
         * @param action the action of the authorization subscription to be added.
         * @param resource the resource of the authorization subscription to be added.
         * @param environment the environment of the authorization subscription to be added.
         * @return this {@code MultiAuthorizationSubscription} instance to support chaining of
         * multiple calls to {@code addAuthorizationSubscription}.
         */
        public Exchangeable<AuthorizationDecision> addAuthorizationSubscription(Object subject, Object action, Object resource, Object environment = null)
        {
            authorizationSubscriptionID++;

            String subscriptionId = authorizationSubscriptionID.ToString("X8");

            if (subject == null)
                throw new ArgumentException("Parameter 'subject' cannot be null.");

            if (action == null)
                throw new ArgumentException("Parameter 'action' cannot be null.");

            if (resource == null)
                throw new ArgumentException("Parameter 'resource' cannot be null.");

            int subjectId = ensureIsElementOfListAndReturnIndex(subject, Subjects);
            int actionId = ensureIsElementOfListAndReturnIndex(action, Actions);
            int resourceId = ensureIsElementOfListAndReturnIndex(resource, Resources);

            int? environmentId = null;
            if (environment != null)
                environmentId = ensureIsElementOfListAndReturnIndex(environment, Environments);

            AuthorizationSubscriptions.Add(subscriptionId, new AuthorizationSubscriptionElements(subjectId, actionId, resourceId, environmentId));

            Exchangeable<AuthorizationDecision> exchangeable = new Exchangeable<AuthorizationDecision>(new AuthorizationDecision(Decision.INDETERMINATE));

            AuthorizationExchangeables.Add(subscriptionId, exchangeable);

            return exchangeable;
        }

        private int ensureIsElementOfListAndReturnIndex(Object element, ArrayList list)
        {
            int index = list.IndexOf(element);
            if (index == -1)
            {
                index = list.Count;
                list.Add(element);
            }
            return index;
        }

        /**
         * @return {@code true} if this multi-subscription holds at least one authorization
         * subscription, {@code false} otherwise.
         */
        public bool hasAuthorizationSubscriptions()
        {
            return AuthorizationSubscriptions.Count > 0;
        }

        /**
         * Returns the authorization subscription related to the given ID or {@code null} if
         * this multi-subscription contains no such ID.
         * @param subscriptionId the ID of the authorization subscription to be returned.
         * @return the authorization subscription related to the given ID or {@code null}.
         */
        public AuthorizationSubscription getAuthorizationSubscriptionWithId(String subscriptionId)
        {
            AuthorizationSubscriptionElements subscriptionElements;

            if (AuthorizationSubscriptions.TryGetValue(subscriptionId, out subscriptionElements))
            {
                return toAuthzSubscription(subscriptionElements);
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MultiAuthorizationSubscription {");
            //for (IdentifiableAuthorizationSubscription subscription : this)
            //{
            //    sb.append("\n\t[").append("SUBSCRIPTION-ID: ").append(subscription.getAuthorizationSubscriptionId())
            //            .append(" | ").append("SUBJECT: ").append(subscription.getAuthorizationSubscription().getSubject())
            //            .append(" | ").append("ACTION: ").append(subscription.getAuthorizationSubscription().getAction())
            //            .append(" | ").append("RESOURCE: ")
            //            .append(subscription.getAuthorizationSubscription().getResource()).append(" | ")
            //            .append("ENVIRONMENT: ").append(subscription.getAuthorizationSubscription().getEnvironment())
            //            .append(']');
            //}
            sb.Append("\n}");
            return sb.ToString();
        }

        private AuthorizationSubscription toAuthzSubscription(AuthorizationSubscriptionElements subscriptionElements)
        {
            // TODO: xtr int? null -> 0
            Object subject = Subjects[subscriptionElements.SubjectId];
            Object action = Actions[subscriptionElements.ActionId];
            Object resource = Resources[subscriptionElements.ResourceId];
            Object environment = subscriptionElements.EnvironmentId == null ? null : Environments[subscriptionElements.EnvironmentId ?? 0];

            // TODO: xtr ... return new AuthorizationSubscription(MAPPER.valueToTree(subject), MAPPER.valueToTree(action), MAPPER.valueToTree(resource), environment == null ? null : MAPPER.valueToTree(environment));
            return null;
        }
    }
}



