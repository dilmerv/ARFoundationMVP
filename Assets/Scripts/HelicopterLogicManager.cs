using System;
using Core;
using LearnXR.Core;
using UnityEngine;
using UnityEngine.Events;

public class HelicopterLogicManager : Singleton<HelicopterLogicManager>
{
    [SerializeField] private AudioSource soundEffect;

    [SerializeField] private float maxLandingZoneDistance = 0.5f;

    [SerializeField] private float maxGroundZoneDistance = 0.09f;

    [Range(0, 35)] [SerializeField] private float minAngleLandingArea;

    [SerializeField] private LayerMask groundLayers;
    
    public UnityEvent<bool> onHelicopterLifted = new();
    
    [field: SerializeField]
    public bool CanLand { get; private set; }
    
    [field: SerializeField]
    public bool Landed { get; private set; }

    private Collider helicopterCollider;
    
    private Vector3 playerInitialPosition = Vector3.zero;

    private void Start()
    {
        helicopterCollider = GetComponent<Collider>();

        playerInitialPosition = transform.position;

        onHelicopterLifted.Invoke(!Landed);

        onHelicopterLifted.AddListener((lifted) =>
        {
            if (lifted) 
            {
                if (!soundEffect.isPlaying)
                {
                    soundEffect.Play();
                }
            }
            else
            {
                soundEffect.Stop();
            }
        });
        
        GameUIManager.Instance.onWonStepDismissed.AddListener(() =>
        {
            transform.position = playerInitialPosition;
        });
        
        GameUIManager.Instance.onFailedStepDismissed.AddListener(() =>
        {
            transform.position = playerInitialPosition;
        });
    }

    private void FixedUpdate()
    {
        Vector3 origin = transform.position;
        Vector3 down = -transform.up;
        
        Debug.DrawLine(origin, origin + transform.TransformDirection(down) * maxLandingZoneDistance, Color.red);
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit raycastHit, maxLandingZoneDistance, groundLayers))
        {
            var hitCollider = raycastHit.transform.GetComponent<Collider>();
            float landingAreaAngle = Vector3.Angle(raycastHit.normal, Vector3.up);

            // safe landing state changes
            if (hitCollider.bounds.size.magnitude > helicopterCollider.bounds.size.magnitude &&
                Math.Abs(landingAreaAngle) <= minAngleLandingArea)
            {
                CanLand = true;
            }
            else
            {
                CanLand = false;
            }
            
            // landing state changes
            var distanceFromGround = Vector3.Distance(origin, raycastHit.point);
            if (distanceFromGround <= maxGroundZoneDistance)
            {
                Landed = true;
                onHelicopterLifted.Invoke(false);
            }
            else
            {
                if (Landed)
                {
                    Landed = false;
                    onHelicopterLifted.Invoke(true);
                }
            }
        }
        else
        {
            CanLand = false;
            Landed = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag(Constants.SURVIVORS_TAG))
        {
            ++HelicopterMissionManager.Instance.currentHelicopterMission.numberOfSurvivorsRescued;
            Destroy(other.gameObject);
        }
    }
}
