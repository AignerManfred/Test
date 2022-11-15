using UnityEngine;
using UnityEngine.UI;

using Sapl.Pdp.Api;

namespace Sapl.Unity.UI
{
    [AddComponentMenu("SAPL/SAPL Deactivate UI")]
    [RequireComponent(typeof(SaplBehaviour))]
    public class SaplBehaviourDeactivateUI : MonoBehaviour
    {
        public void OnAuthorizationDecision(AuthorizationDecision authorizationDecision)
        {
            Selectable selectable = GetComponent<Selectable>();

            if (selectable != null)
                selectable.interactable = (authorizationDecision.Decision == Decision.PERMIT);
        }
    }
}