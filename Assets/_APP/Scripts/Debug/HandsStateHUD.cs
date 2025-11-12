// Assets/Scripts/HandsStateHUD.cs
using UnityEngine;
using UnityEngine.XR.Hands;

public sealed class HandsStateHUD : MonoBehaviour
{
  XRHandSubsystem sub;
  void Start()
  {
    sub = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.activeLoader
             .GetLoadedSubsystem<XRHandSubsystem>();
    if (sub == null) { Debug.LogError("XRHandSubsystem not found"); enabled = false; return; }
    InvokeRepeating(nameof(Log), 0, 1f);
  }
  void Log()
  {
    bool L = sub.leftHand.isTracked;
    bool R = sub.rightHand.isTracked;
    Debug.Log($"Hands tracked  L:{L}  R:{R}");
  }
}
