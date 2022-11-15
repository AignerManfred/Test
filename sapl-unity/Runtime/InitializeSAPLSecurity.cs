using Sapl.Pdp.Api;
using Sapl.Pep.Config;
using Sapl.Unity.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Sapl.Unity
{
    public class InitializeSAPLSecurity
    {
        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
            Debug.Log("After Scene is loaded and game is running");
            Debug.Log("Scan started.");
            Debug.Log("Securitymanager initialisiert");
            Debug.Log("Number of Scenes: " + SceneManager.sceneCount);

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Additive);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name + "; LoadSceneMode: " + mode);

            var saplBehaviours = GameObject.FindObjectsOfType<SaplBehaviour>(true);

            if (saplBehaviours.Length > 0)
            {
                int count = 0;

                var multiAuthorizationSubscription = new MultiAuthorizationSubscription();

                foreach (SaplBehaviour saplBehaviour in saplBehaviours)
                {
                    if (saplBehaviour.gameObject.scene == scene)
                    {
                        count++;

                        Exchangeable<AuthorizationDecision> exAuthorizationDecision = multiAuthorizationSubscription.addAuthorizationSubscription(
                            saplBehaviour.Subject, saplBehaviour.Action, saplBehaviour.Resource, saplBehaviour.Environment
                        );

                        saplBehaviour._internalSetExAuthorizationDecision(exAuthorizationDecision);
                    }
                }

                if (count > 0)
                {
                    IPolicyDecisionPoint pdp = PepConfig.Instance.PolicyDecisionPoint;
                    IPolicyDecisionPointStreamControl pdpStreamControl = pdp.decide(multiAuthorizationSubscription);

                    //TODO:xtr store pdpStreamControl
                }
            }
        }

        static void OnSceneUnLoaded(Scene scene)
        {
            Debug.Log("OnSceneUnLoaded: " + scene.name + ";");

            //TODO:xtr pdpStreamControl.Stop();
        }
    }
}