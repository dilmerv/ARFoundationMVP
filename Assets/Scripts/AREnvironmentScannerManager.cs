using Core;
using UnityEngine;
using UnityEngine.Events;
using LearnXR.Core;

public class AREnvironmentScannerManager : Singleton<AREnvironmentScannerManager>
{
   [SerializeField] private GameObject reticlePrefab;
   
   [SerializeField] private LayerMask layersToInclude;

   public UnityEvent<Vector3> onPlaceableArea = new ();

   private GameObject reticle;

   private GameObject terminal;

   private MeshRenderer reticleRenderer;

   private Camera mainCamera;

   // plane or meshes information
   private ARReticleInfo reticleInfo;

   private void Start()
   {
      mainCamera = Camera.main;
      terminal = GameObject.FindWithTag(Constants.TERMINAL_TAG);

      reticle = Instantiate(reticlePrefab, Vector3.zero, Quaternion.identity);
      reticleInfo = reticle.GetComponent<ARReticleInfo>();
      reticle.SetActive(false);
      reticleRenderer = reticle.GetComponent<MeshRenderer>();
   }

   private void Update()
   {
      if (GameManager.Instance.currentGameMode != GameMode.Scanning) return;
      
      Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

      if (Physics.Raycast(ray, out RaycastHit raycastHit, float.PositiveInfinity, layersToInclude))
      {
         reticle.SetActive(true);
         reticle.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
         
         Vector3 offsetPosition = raycastHit.point + raycastHit.normal * reticleInfo.ReticleOffset;
         reticle.transform.position = offsetPosition;

         float areaAngle = Vector3.Angle(raycastHit.normal, Vector3.up);
         float hitMagnitude = raycastHit.transform.GetComponent<MeshFilter>().mesh
            .bounds.size.magnitude;
         float distanceFromterminal = Vector3.Distance(terminal.transform.position, raycastHit.point);
         
         reticleInfo.UpdateInfo($"Angle: {areaAngle}", $"Magnitude: {hitMagnitude}",
            $"Distance: {distanceFromterminal}");

         var currentMissionState = HelicopterMissionManager.Instance.currentHelicopterMission;
         if (areaAngle >= currentMissionState.minAngle &&
             areaAngle <= currentMissionState.maxAngle &&
             hitMagnitude >= currentMissionState.minAreaSize &&
             distanceFromterminal >= currentMissionState.minDistanceFromTerminal)
         {
            reticleRenderer.material.color = reticleInfo.ReticleValidColor;
            onPlaceableArea.Invoke(raycastHit.point);
         }
         else
         {
            reticleRenderer.material.color = reticleInfo.ReticleInValidColor;
         }
      }
      else
      {
         reticle.SetActive(false);
      }
   }
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   
}
