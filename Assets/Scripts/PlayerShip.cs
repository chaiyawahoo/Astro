using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerShip : MonoBehaviour {

    // Components
    private Rigidbody rb;
    private PlayerInput pInput;

    // Other Game Objects
    private GameObject cam;
    private GameObject ship;

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

    public float ThrottleCoefficient { get; private set; } = 0.01f;
    public float PitchCoefficient { get; private set; } = 2f;
    public float RollCoefficient { get; private set; } = 3f;
    public float YawCoefficient { get; private set; } = 0.25f;
    // TODO: lerp sensitivity per axis
    // TODO: allow debug tweaking
    public float RotationCoefficient { get; private set; } = 0.05f;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        pInput = GetComponent<PlayerInput>();
        CurrentRotationGoal = rb.rotation;
    }

    private void Start() {
        cam = transform.Find("Main Camera").gameObject;
        ship = transform.Find("Ship").gameObject;

        canvas = GameObject.Find("Canvas");
        throttleText = canvas.transform.Find("Throttle Text").GetComponent<Text>();
        accelerationText = canvas.transform.Find("Acceleration Text").GetComponent<Text>();
        velocityText = canvas.transform.Find("Velocity Text").GetComponent<Text>();
        UpdateUIText();
    }

    private void Update() {
        UpdateUIText();
    }

    private void FixedUpdate() {
        HandleInput();
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
        rb.rotation = Quaternion.Slerp(rb.rotation, CurrentRotationGoal, RotationCoefficient);
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
        Vector3 deltaRotation = new Vector3(deltaPitch, deltaYaw, deltaRoll);
        CurrentRotationGoal *= Quaternion.Euler(deltaRotation);
    }

    private void UpdateUIText() {
        throttleText.text = string.Format("Current Throttle: {0}%", CurrentThrottle * 100f);
        accelerationText.text = string.Format("Current Acceleration: {0} m/s^2", CurrentAcceleration);
        velocityText.text = string.Format("Current Velocity: {0} m/s", CurrentVelocity);
    }
}