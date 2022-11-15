using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Sapl.Pdp.Api;
using Sapl.Unity.UI;

[AddComponentMenu("SAPL/SAPL SecuredOnClick")]
[RequireComponent(typeof(SaplBehaviour))]
public class SecuredOnClick : MonoBehaviour
{
    public UnityEventSerializable OnPermit;
    private Button _button;

    private SaplBehaviour _saplBehaviour;

    private void Awake()
    {
        if (!_button) _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleOnClick);

        _saplBehaviour = GetComponent<SaplBehaviour>();

        //Get Information about PersistenEvents and maybe disable/warn/...
        //Debug.Log(_button.onClick.GetPersistentEventCount().ToString());
        //for (int i = 0; i< _button.onClick.GetPersistentEventCount(); i++) {
        //    Debug.Log("state: " + _button.onClick.GetPersistentListenerState(i).ToString());
        //    Debug.Log("methodName: " + _button.onClick.GetPersistentMethodName(i).ToString());
        //    Debug.Log("target: " + _button.onClick.GetPersistentTarget(i).ToString());
        //    _button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
        //    Debug.Log("state: " + _button.onClick.GetPersistentListenerState(i).ToString());
        //}
    }

    private void HandleOnClick()
    {
        Debug.Log("HandleOnClick");

        if (_saplBehaviour.AuthorizationDecision.Decision == Decision.PERMIT)
        {
            //Obligation Handling?
            OnPermit.Invoke();
        }
    }

    //tmp Debugging
    public void DebugLog()
    {
        Debug.Log("DebugLog");
    }
    public void DebugLog2()
    {
        Debug.Log("DebugLog2");
    }
}

[Serializable]
public class UnityEventSerializable : UnityEvent
{
}
