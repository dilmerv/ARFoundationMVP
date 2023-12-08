using TMPro;
using UnityEngine;

public class ARReticleInfo : MonoBehaviour
{
    [SerializeField] private float reticleOffset = 0.001f;
    [SerializeField] private Color reticleValid = Color.green;
    [SerializeField] private Color reticleInValid = Color.red;

    [SerializeField] private bool debugInfoOn = false;
    
    // plane or meshing debug info
    [SerializeField] private TextMeshProUGUI reticleAngleInfo;
    [SerializeField] private TextMeshProUGUI reticleMagnitudeInfo;
    [SerializeField] private TextMeshProUGUI reticleDistanceInfo;

    public float ReticleOffset => reticleOffset;

    public Color ReticleValidColor => reticleValid;
    
    public Color ReticleInValidColor => reticleInValid;

    private Camera mainCamera;

    private Transform parent;

    private void Start()
    {
        mainCamera = Camera.main;
        parent = reticleAngleInfo.transform.parent;
    }

    private void Update()
    {
        if (debugInfoOn)
        {
            gameObject.SetActive(true);
            FaceUIPanelTowardsCamera();
        }
        else
        {
            gameObject.SetActive(false);
        }
    } 

    private void FaceUIPanelTowardsCamera()
    {
        parent.transform.LookAt(parent.transform.position + mainCamera.transform.rotation
        * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    public void UpdateInfo(string angleInfo, string magnitudeInfo, string distanceInfo)
    {
        reticleAngleInfo.text = angleInfo;
        reticleMagnitudeInfo.text = magnitudeInfo;
        reticleDistanceInfo.text = distanceInfo;
    }
}
