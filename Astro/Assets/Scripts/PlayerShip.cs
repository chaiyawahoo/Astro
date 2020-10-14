using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerShip : MonoBehaviour {

    private Rigidbody rb;
    private PlayerInput pInput;

    public float MinimumVelocity { get; private set; } = 1f;
    public float MinimumThrottle { get; private set; } = 0f;

    public float MaximumVelocity { get; private set; } = 100f;
    public float MaximumThrottle { get; private set; } = 1f;

    public float ThrottleCoefficient { get; private set; } = 0.01f;

    public float CurrentVelocity { get; private set; } = 0f;
    public float CurrentThrottle { get; private set; } = 0f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        pInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate() {
        HandleInput();
        Move();
    }

    private void Move() {
        float forwardVelocity = Mathf.Lerp(MinimumVelocity, MaximumVelocity, CurrentThrottle);
        float externalVelocity;
        if (forwardVelocity > CurrentVelocity) {
            externalVelocity = rb.velocity.z - CurrentVelocity;
        } else {
            externalVelocity = CurrentVelocity - rb.velocity.z;
        }
        CurrentVelocity = externalVelocity + forwardVelocity;
        if (CurrentVelocity >= MaximumVelocity) {
            CurrentVelocity = MaximumVelocity;
        } else if (CurrentVelocity <= MinimumVelocity) {
            CurrentVelocity = MinimumVelocity;
        }
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, CurrentVelocity);
    }

    private void HandleInput() {
        // Handle Throttle
        float throttleInput = pInput.actions["Throttle"].ReadValue<float>();
        float deltaThrottle = throttleInput * ThrottleCoefficient;
        CurrentThrottle += deltaThrottle;
        if (CurrentThrottle >= MaximumThrottle) {
            CurrentThrottle = MaximumThrottle;
        } else if (CurrentThrottle <= MinimumThrottle) {
            CurrentThrottle = MinimumThrottle;
        }
    }
}
