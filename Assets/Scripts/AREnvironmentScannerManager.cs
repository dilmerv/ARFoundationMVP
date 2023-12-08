using System;
using Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AREnvironmentScannerManager : MonoBehaviour
{
   [SerializeField] private GameObject reticlePrefab;
   //TODO add the meshes and planes layers 
   [SerializeField] private LayerMask layersToInclude;

   public UnityEvent<Vector3> onPlaceableArea = new ();

   private GameObject reticle;

   private GameObject terminal;

   private MeshRenderer reticleRenderer;

   private Camera mainCamera;

   // plane or meshes informations
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
      //TODO we need to check and make sure the game is playing before we continue
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
         
         //TODO section will be to determine if we can place survivors
      }
      else
      {
         reticle.SetActive(false);
      }
   }
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   
}
