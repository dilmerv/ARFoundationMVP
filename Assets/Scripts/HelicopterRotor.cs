using Core;
using UnityEngine;

public class HelicopterRotor : MonoBehaviour
{
    [SerializeField] private float rotorSpeed = 2.0f;

    [SerializeField] private RotorAxis rotorAxis = RotorAxis.Y;

    [SerializeField] private bool debugRotor = false;

    private bool rotorRunning;
    
    private void Start()
    {
        //Binding to a onHelicopterLifted
    }

    private void Update() => ApplyRotation();

    private void RotorStateChanged(bool state)
    {
        rotorRunning = state;
    }

    private void ApplyRotation()
    {
        if (!rotorRunning && !debugRotor) return;

        if (rotorAxis == RotorAxis.Y)
        {
            transform.Rotate(Vector3.up * rotorSpeed, Space.Self);
        }
        else if (rotorAxis == RotorAxis.X)
        {
            transform.Rotate(Vector3.left * rotorSpeed, Space.Self);
        }
        else if (rotorAxis == RotorAxis.Z)
        {
            transform.Rotate(Vector3.forward * rotorSpeed, Space.Self);
        }
    }
}
