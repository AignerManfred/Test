using UnityEngine;

using Sapl.Pdp.Api;

using System.Collections;
using System.Collections.Concurrent;

namespace Sapl.Unity.UI
{
    [AddComponentMenu("SAPL/SAPL Behaviour")]
    public class SaplBehaviour : MonoBehaviour
    {
        [Header("SAPL Authorization Subscription")]

        [Tooltip("The subject of the SAPL Authorization Subscription.")]
        [SerializeField] private string _subject = "user";

        [Tooltip("The action of the SAPL Authorization Subscription.")]
        [SerializeField] private string _action = "action";

        [Tooltip("The resource of the SAPL Authorization Subscription.")]
        [SerializeField] private string _resource = "resoure";

        [Tooltip("The environment of the SAPL Authorization Subscription.")]
        [SerializeField] private string _environment = "environment";

        public string Subject { get { return _subject; } }
        public string Action { get { return _action; } }
        public string Resource { get { return _resource; } }
        public string Environment { get { return _environment; } }


        private Exchangeable<AuthorizationDecision> _exAuthorizationDecision = null;

        private ConcurrentQueue<AuthorizationDecision> authorizationDecisionQueue = new ConcurrentQueue<AuthorizationDecision>();

        private AuthorizationDecision _authorizationDecision = new AuthorizationDecision(Decision.INDETERMINATE);

        public AuthorizationDecision AuthorizationDecision { get { return _authorizationDecision; } }


        public SaplBehaviour()
        {
        }        

        public void _internalSetExAuthorizationDecision(Exchangeable<AuthorizationDecision> exAuthorizationDecision)
        {
            authorizationDecisionQueue.Enqueue(exAuthorizationDecision.Value);

            _exAuthorizationDecision = exAuthorizationDecision;
            _exAuthorizationDecision.OnExchanged += source => { authorizationDecisionQueue.Enqueue(_exAuthorizationDecision.Value); };

            StartCoroutine(CheckAuthorizationDecisionUpdate());
        }

        private IEnumerator CheckAuthorizationDecisionUpdate()
        {
            AuthorizationDecision authorizationDecision;

            while (true)
            {
                if (authorizationDecisionQueue.TryDequeue(out authorizationDecision))
                {
                    _authorizationDecision = authorizationDecision;
                    gameObject.SendMessage("OnAuthorizationDecision", authorizationDecision, SendMessageOptions.DontRequireReceiver);
                }

                yield return null;
            }
        }
    }
}