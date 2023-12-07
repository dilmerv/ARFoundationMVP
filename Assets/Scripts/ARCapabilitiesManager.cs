using System.Collections;
using System.Collections.Generic;
using LearnXR.Core;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using Logger = LearnXR.Core.Logger;

public class ARCapabilitiesManager : Singleton<ARCapabilitiesManager>
{
    private ARSession arSession;
    
    private ARPlaneManager arPlaneManager;

    private ARMeshManager arMeshManager;

    private AROcclusionManager arOcclusionManager;

    public List<string> capabilitiesAvailable = new ();

    public UnityEvent<List<string>> onCapabilitiesDetermined = new();

    private bool capabilitiesEnabled = true;

    private void Awake()
    {
        arSession = FindObjectOfType<ARSession>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arMeshManager = FindObjectOfType<ARMeshManager>();
        arOcclusionManager = FindObjectOfType<AROcclusionManager>();
        arSession.enabled = false;
        
        CapabilitiesToggle();    
    }

    private void Start()
    {
        StartCoroutine(CheckARSupport());
        GameUIManager.Instance.onWizardStepsDismissed.AddListener(CapabilitiesToggle);
    }

    private void CapabilitiesToggle()
    {
        capabilitiesEnabled = !capabilitiesEnabled;

        if (arPlaneManager.subsystem != null) arPlaneManager.enabled = capabilitiesEnabled;
        if (arMeshManager.subsystem != null) arMeshManager.enabled = capabilitiesEnabled;
        if (arOcclusionManager.subsystem != null) arOcclusionManager.enabled = capabilitiesEnabled;
    }

    private void OnEnable()
    {
        ARSession.stateChanged += ARSessionStateChanged;
    }

    private void OnDisable()
    {
        ARSession.stateChanged -= ARSessionStateChanged;
    }

    private void ARSessionStateChanged(ARSessionStateChangedEventArgs obj)
    {
        Logger.Instance.LogInfo($"AR session state changed: {obj.state}");
    }

    private IEnumerator CheckARSupport()
    {
        if (ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
        {
            Logger.Instance.LogInfo($"Checking if AR is available on this device");
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state != ARSessionState.Unsupported)
        {
            Logger.Instance.LogInfo($"AR is supported on this device");
            arSession.enabled = true;

            bool planeDetection = arPlaneManager.subsystem != null;
            bool meshing = arMeshManager.subsystem != null;
            bool occlusion = arOcclusionManager.subsystem != null;
        
            capabilitiesAvailable.Add($"Plane detection support available: <b>{planeDetection}</b>");
            capabilitiesAvailable.Add($"Meshing support available: <b>{meshing}</b>");
            capabilitiesAvailable.Add($"Occlusion support available: <b>{occlusion}</b>");
        }
        else
        {
            Logger.Instance.LogInfo($"AR is not supported on this device :(");
        }

        if (capabilitiesAvailable.Count == 0)
        {
            capabilitiesAvailable.Add("Your phone does not have any AR capabilities, or you're running in " +
                                      "Unity without XR Simulation enabled (Go to File > Build Settings > Player " +
                                      "Settings > XR Plug-in Manager > Standalone Platform > XR Simulation)");
        }
        
        onCapabilitiesDetermined.Invoke(capabilitiesAvailable);
    }
}
