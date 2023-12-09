using LearnXR.Core;
using UnityEngine;

public class HelicopterInputController : Singleton<HelicopterInputController>
{
    [SerializeField] private float moveForce = 15.0f;

    [SerializeField] private float rotationAmount = 1.0f;

    [SerializeField] private float altitudeForce = 12.0f;
    
    [field: SerializeField]
    public bool IsGrounded { get; private set; }

    private float liftAmount, descendAmount;

    private Vector2 directionValue;

    private HelicopterInputActions inputActions;

    private Rigidbody physics;

    private void Start()
    {
        inputActions = new HelicopterInputActions();
        inputActions.Enable();
        physics = GetComponent<Rigidbody>();
        
        HelicopterLogicManager.Instance.onHelicopterLifted.AddListener((lifted) =>
        {
            IsGrounded = !lifted;
        });
    }

    private void Update()
    {
        directionValue = inputActions.Player.Move.ReadValue<Vector2>();
        liftAmount = inputActions.Player.Lift.IsPressed() ? 1.0f : 0;
        descendAmount = inputActions.Player.Descend.IsPressed() ? 1.0f : 0;
    }

    private void FixedUpdate() => ApplyForces();

    private void ApplyForces()
    {
        if (!IsGrounded)
        {
            // move forward based on user input or backward
            Vector3 forwardForce = transform.forward * (directionValue.y * moveForce);
            physics.AddForce(forwardForce);
            
            // rotate helicopter 
            Vector3 rotationTorque = new Vector3(0, directionValue.x * rotationAmount, 0);
            physics.AddRelativeTorque(rotationTorque);
        }

        Vector3 liftForce = transform.up * ((liftAmount - descendAmount) * altitudeForce);
        physics.AddForce(liftForce);
    }
}
