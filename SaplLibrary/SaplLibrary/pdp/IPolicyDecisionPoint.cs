namespace Sapl.Pdp.Api
{
    /**
     * The policy decision point is the component in the system, which will take an
     * authorization subscription, retrieve matching policies from the policy retrieval point,
     * evaluate the policies while potentially consulting external resources (e.g., through
     * attribute finders), and return a {@link Flux} of authorization decision objects.
     *
     * This interface offers methods to hand over an authorization subscription to the policy
     * decision point, differing in the construction of the underlying authorization
     * subscription object.
     */
    public interface IPolicyDecisionPoint
    {
        //TODO: xtr remove Flux
        /**
         * Takes an authorization subscription object and returns a {@link Flux} emitting
         * matching authorization decisions.
         * @param authzSubscription the SAPL authorization subscription object
         * @return a {@link Flux} emitting the authorization decisions for the given
         * authorization subscription. New authorization decisions are only added to the
         * stream if they are different from the preceding authorization decision.
         */
        //Flux<AuthorizationDecision> decide(AuthorizationSubscription authzSubscription);
        public IPolicyDecisionPointStreamControl decide(AuthorizationSubscription authzSubscription);

        //TODO: xtr remove Flux
        /**
         * Multi-subscription variant of {@link #decide(AuthorizationSubscription)}.
         * @param multiAuthzSubscription the multi-subscription object containing the
         * subjects, actions, resources, and environments of the authorization subscriptions
         * to be evaluated by the PDP.
         * @return a {@link Flux} emitting authorization decisions for the given authorization
         * subscriptions as soon as they are available. Related authorization decisions and
         * authorization subscriptions have the same id.
         */
        //Flux<IdentifiableAuthorizationDecision> decide(MultiAuthorizationSubscription multiAuthzSubscription);
        public IPolicyDecisionPointStreamControl decide(MultiAuthorizationSubscription multiAuthzSubscription);

        //TODO: xtr remove Flux
        /**
         * Multi-subscription variant of {@link #decide(AuthorizationSubscription)}.
         * @param multiAuthzSubscription the multi-subscription object containing the
         * subjects, actions, resources, and environments of the authorization subscriptions
         * to be evaluated by the PDP.
         * @return a {@link Flux} emitting authorization decisions for the given authorization
         * subscriptions as soon as at least one authorization decision for each authorization
         * subscription is available.
         */
        //Flux<MultiAuthorizationDecision> decideAll(MultiAuthorizationSubscription multiAuthzSubscription);
        //public Exchangeable<MultiAuthorizationDecision> decideAll(MultiAuthorizationSubscription multiAuthzSubscription);
    }
}

