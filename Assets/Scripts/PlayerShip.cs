using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerShip : MonoBehaviour {

    // Components
    private Rigidbody rb;
    private PlayerInput pInput;

    // Children
    private Transform cam;
    private Transform ship;

    // Canvas Objects
    private GameObject canvas;
    private Text throttleText;
    private Text accelerationText;
    private Text velocityText;

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
    public float PitchCoefficient { get; private set; } = 1f;
    public float RollCoefficient { get; private set; } = 1.5f;
    public float YawCoefficient { get; private set; } = 0.125f;
    // TODO: allow debug tweaking
    public float RotationDelayFrames { get; private set; } = 20f;
    public float CameraLookAhead { get; private set; } = 3f;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        pInput = GetComponent<PlayerInput>();
        CurrentRotationGoal = transform.rotation;
    }

    private void Start() {
        cam = transform.Find("Main Camera");
        ship = transform.Find("Ship");

        canvas = GameObject.Find("Canvas");
        throttleText = canvas.transform.Find("Throttle Text").GetComponent<Text>();
        accelerationText = canvas.transform.Find("Acceleration Text").GetComponent<Text>();
        velocityText = canvas.transform.Find("Velocity Text").GetComponent<Text>();
        UpdateUIText();
    }

    private void Update() {
        UpdateUIText();
        HandleInput();
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
        cam.transform.rotation = Quaternion.Lerp(transform.rotation, CurrentRotationGoal, 1f / RotationDelayFrames * CameraLookAhead);
        Quaternion rotation = Quaternion.Lerp(transform.rotation, CurrentRotationGoal, 1f / RotationDelayFrames) * Quaternion.Inverse(transform.rotation);
        transform.RotateAround(ship.position, Vector3.right, rotation.eulerAngles.x);
        transform.RotateAround(ship.position, Vector3.up, rotation.eulerAngles.y);
        transform.RotateAround(ship.position, Vector3.forward, rotation.eulerAngles.z);
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
    }

    private void UpdateUIText() {
        throttleText.text = string.Format("Current Throttle: {0}%", CurrentThrottle * 100f);
        accelerationText.text = string.Format("Current Acceleration: {0} m/s^2", CurrentAcceleration);
        velocityText.text = string.Format("Current Velocity: {0} m/s", CurrentVelocity);
    }
}