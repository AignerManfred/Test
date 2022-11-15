using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;

using Sapl.Pdp.Api;
using Sapl.Pep.Config;

namespace Sapl.Unity.UI
{

    [AddComponentMenu("SAPL/SAPL UI Activator")]
    public class UIActivator : MonoBehaviour
    {
        public string Action = "";
        public string Resource = "";

        private Selectable selectable;

        private Exchangeable<AuthorizationDecision> _exAuthorizationDecision;
        private IPolicyDecisionPointStreamControl _pdpStreamControl;

        void Start()
        {
            selectable = GetComponent<Selectable>();
            
            string nameGameObject = selectable.gameObject.name;
            Debug.Log(nameGameObject);

            AuthorizationSubscription authorizationSubscription = new AuthorizationSubscription();
            _exAuthorizationDecision = authorizationSubscription.setAuthorizationSubscription(
                    JValue.CreateString("peter"), JValue.CreateString(nameGameObject), JValue.CreateString(Resource), JValue.CreateString("my_environment")
                 );
            Debug.Log("authorizationSubscription subject: " + authorizationSubscription.Subject.ToString());
            Debug.Log("authorizationSubscription action: " + authorizationSubscription.Action.ToString());

            IPolicyDecisionPoint pdp = PepConfig.Instance.PolicyDecisionPoint;

            _pdpStreamControl = pdp.decide(authorizationSubscription);
            Debug.Log(_exAuthorizationDecision.Value.ToString());
            //_exAuthorizationDecision.OnExchanged += (object sender) => { Debug.Log("_exAuthorizationDecision: " + _exAuthorizationDecision.Value); };
        }

        void Update()
        {
            selectable.interactable = (_exAuthorizationDecision.Value.Decision == Decision.PERMIT);
            //Debug.Log(_exAuthorizationDecision.Value.Decision.ToString());
        }

        void OnDestroy()
        {
            // TODO: xtr
            // _pdpStreamControl.Stop();
        }
    }


}