using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerShip : MonoBehaviour {

    public GameObject MissilePrefab;

    // Components
    private Rigidbody rb;
    private PlayerInput pInput;

    // Children
    private Transform cam;
    private Transform ship;

    public float MinimumThrottle { get; private set; } = 0f;
    public float MinimumAcceleration { get; private set; } = 0f;
    public float MinimumVelocity { get; private set; } = 5f;

    public float MaximumThrottle { get; private set; } = 1f;
    public float MaximumAcceleration { get; private set; } = 1f;
    public float MaximumVelocity { get; private set; } = 100f;

    public float CurrentThrottle { get; private set; } = 0f;
    public float CurrentAcceleration { get; private set; } = 0f;
    public float CurrentVelocity { get; private set; } = 0f;

    public Quaternion CurrentRotationGoal { get; private set; } = Quaternion.identity;
    public Quaternion CurrentPitchGoal { get; private set; } = Quaternion.identity;
    public Quaternion CurrentRollGoal { get; private set; } = Quaternion.identity;
    public Quaternion CurrentYawGoal { get; private set; } = Quaternion.identity;

    public float ThrottleCoefficient { get; private set; } = 0.005f;
    public float PitchCoefficient { get; private set; } = 1.25f;
    public float RollCoefficient { get; private set; } = 1.75f;
    public float YawCoefficient { get; private set; } = 0.125f;
    // TODO: allow debug tweaking
    public float RotationDelaySeconds { get; private set; } = 1f / 3f;
    public float ShipRotationMultiplier { get; private set; } = 5f;

    public bool Firing { get; private set; } = false;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        pInput = GetComponent<PlayerInput>();
        CurrentRotationGoal = rb.rotation;
    }

    private void Start() {
        cam = transform.Find("Main Camera");
        ship = transform.Find("Ship");
    }

    private void Update() {
        HandleInput();
        Fire();
    }

    private void FixedUpdate() {
        Move();
        Rotate();
    }

    private void Move() {
        float tempMaxVelocity = Mathf.Lerp(MinimumVelocity, MaximumVelocity, CurrentThrottle);
        CurrentAcceleration = Mathf.Lerp(MinimumAcceleration, MaximumAcceleration, CurrentThrottle);
        CurrentVelocity += CurrentAcceleration;
        if (CurrentVelocity >= tempMaxVelocity) {
            CurrentVelocity = tempMaxVelocity;
        } else if (CurrentVelocity <= MinimumVelocity) {
            CurrentVelocity = MinimumVelocity;
        }
        rb.velocity = transform.forward * CurrentVelocity;
    }

    private void Rotate() {
        float playerLerp = 1f / (RotationDelaySeconds / Time.fixedDeltaTime);
        float shipLerp = ShipRotationMultiplier * playerLerp;
        Quaternion rotation = Quaternion.Lerp(rb.rotation, CurrentRotationGoal, playerLerp);
        Transform playerTransform = transform;
        rotation *= Quaternion.Inverse(rb.rotation);
        playerTransform.RotateAround(ship.position, Vector3.right, rotation.eulerAngles.x);
        playerTransform.RotateAround(ship.position, Vector3.up, rotation.eulerAngles.y);
        playerTransform.RotateAround(ship.position, Vector3.forward, rotation.eulerAngles.z);
        rb.MoveRotation(playerTransform.rotation);
        rb.MovePosition(playerTransform.position);
        ship.transform.rotation = Quaternion.Lerp(rb.rotation, CurrentRotationGoal, shipLerp);
    }

    private void Fire() {
        if (Firing) {
            Instantiate(MissilePrefab, ship.transform.position, ship.transform.rotation);
            Firing = false;
        }
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
        // Handle Roll, Pitch, and Yaw
        float rollInput = pInput.actions["Roll"].ReadValue<float>();
        float pitchInput = pInput.actions["Pitch"].ReadValue<float>();
        float yawInput = pInput.actions["Yaw"].ReadValue<float>();
        float deltaRoll = rollInput * RollCoefficient;
        float deltaPitch = pitchInput * PitchCoefficient;
        float deltaYaw = yawInput * YawCoefficient;
        CurrentPitchGoal = Quaternion.Euler(Vector3.right * deltaPitch);
        CurrentRollGoal = Quaternion.Euler(Vector3.forward * deltaRoll);
        CurrentYawGoal = Quaternion.Euler(Vector3.up * deltaYaw);
        CurrentRotationGoal *= CurrentPitchGoal * CurrentRollGoal * CurrentYawGoal;
        // Handle Firing
        Firing = pInput.actions["Fire"].triggered;
    }

    private void Die() {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        CurrentThrottle = 0f;
        CurrentAcceleration = 0f;
        CurrentVelocity = 0f;
        CurrentRotationGoal = rb.rotation;
        CurrentPitchGoal = Quaternion.identity;
        CurrentRollGoal = Quaternion.identity;
        CurrentYawGoal = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("collide");
        Die();
    }
}