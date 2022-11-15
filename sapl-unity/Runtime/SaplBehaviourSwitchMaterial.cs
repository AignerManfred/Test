using UnityEngine;

using Sapl.Pdp.Api;

namespace Sapl.Unity.UI
{
    [AddComponentMenu("SAPL/SAPL Switch Material")]
    [RequireComponent(typeof(SaplBehaviour))]
    public class SaplBehaviourSwitchMaterial : MonoBehaviour
    {
        [Header("Materials")]

        [Tooltip("The material to be used if the decision is permit.")]
        [SerializeField] private Material _permitMaterial;

        [Tooltip("The material to be used if the decision is not permit.")]
        [SerializeField] private Material _denyMaterial;

        public void OnAuthorizationDecision(AuthorizationDecision authorizationDecision)
        {
            Renderer renderer = GetComponent<Renderer>();

            bool isPermited = (authorizationDecision.Decision == Decision.PERMIT);

            renderer.material = isPermited ? _permitMaterial : _denyMaterial;
        }
    }
}