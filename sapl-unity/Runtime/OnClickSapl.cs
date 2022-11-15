
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

using Sapl.Pdp.Api;
using Sapl.Pep.Config;

namespace Sapl.Unity
{
    public class OnClickSapl : MonoBehaviour
    {
        public string Resource = "";

        private Button button;

        private Exchangeable<AuthorizationDecision> _exAuthorizationDecision;
        private IPolicyDecisionPointStreamControl _pdpStreamControl;

        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(TaskOnClick);
            string nameGameObject = button.gameObject.name;

            AuthorizationSubscription authorizationSubscription = new AuthorizationSubscription();

            _exAuthorizationDecision = authorizationSubscription.setAuthorizationSubscription(
                JValue.CreateString("peter"), JValue.CreateString(nameGameObject + ".OnClick"), JValue.CreateString(Resource), JValue.CreateString("my_environment")
             );

            Debug.Log("authorizationSubscription subject: " + authorizationSubscription.Subject.ToString());
            Debug.Log("authorizationSubscription action: " + authorizationSubscription.Action.ToString());


            IPolicyDecisionPoint pdp = PepConfig.Instance.PolicyDecisionPoint;

            _pdpStreamControl = pdp.decide(authorizationSubscription);
            Debug.Log(_exAuthorizationDecision.Value.ToString());
            //_exAuthorizationDecision.OnExchanged += (object sender) => { Debug.Log("exAuthorizationDecision: " + _exAuthorizationDecision.Value); };
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        void OnDestroy()
        {
            // TODO: xtr
            // _pdpStreamControl.Stop();
        }

        void TaskOnClick()
        {
            if (_exAuthorizationDecision.Value.Decision == Decision.PERMIT)
            {
                Debug.Log("Click Aktion ausgeführt!");
            }
            else
            {
                Debug.Log("Click Aktion verweigert!");
            }
        }
    }
}
