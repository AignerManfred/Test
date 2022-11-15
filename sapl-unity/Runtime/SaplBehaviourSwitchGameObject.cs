using UnityEngine;

using Sapl.Pdp.Api;


namespace Sapl.Unity.UI
{
    [AddComponentMenu("SAPL/SAPL Switch GameObject")]
    [RequireComponent(typeof(SaplBehaviour))]
    public class SaplBehaviourSwitchGameObject : MonoBehaviour
    {
        [Header("Game Objects")]

        [Tooltip("The gameobject to be used if the decision is permit.")]
        [SerializeField] private GameObject _permitGameObject;

        [Tooltip("The gameobject to be used if the decision is not permit.")]
        [SerializeField] private GameObject _denyGameObject;

        public void OnAuthorizationDecision(AuthorizationDecision authorizationDecision)
        {
            bool isPermited = (authorizationDecision.Decision == Decision.PERMIT);

            _permitGameObject.SetActive(isPermited);
            _denyGameObject.SetActive(!isPermited);
        }
    }
}