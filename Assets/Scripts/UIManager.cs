using UnityEngine;

public class UIManager : MonoBehaviour {

    public PlayerShip Player { get; private set; }

    public RectTransform ThrottleBackground { get; private set; }
    public RectTransform ThrottleHandle { get; private set; }

    public float MaximumThrottle { get; private set; }
    public float MaximumVelocity { get; private set; }

    public float CurrentThrottle { get; private set; }
    public float CurrentVelocity { get; private set; }

    public float ThrottleRatio { get; private set; }
    public float VelocityRatio { get; private set; }

    private void Start() {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShip>();
        ThrottleBackground = GameObject.Find("Throttle Background").GetComponent<RectTransform>();
        ThrottleHandle = GameObject.Find("Throttle Handle").GetComponent<RectTransform>();
        MaximumThrottle = Player.MaximumThrottle;
        MaximumVelocity = Player.MaximumVelocity;
        CurrentThrottle = Player.CurrentThrottle;
        CurrentVelocity = Player.CurrentVelocity;
        ThrottleRatio = CurrentThrottle / MaximumThrottle;
        VelocityRatio = CurrentVelocity / MaximumVelocity;
    }

    private void Update() {
        UpdateValues();
        UpdateUI();
    }

    private void UpdateValues() {
        CurrentThrottle = Player.CurrentThrottle;
        CurrentVelocity = Player.CurrentVelocity;
        ThrottleRatio = CurrentThrottle / MaximumThrottle;
        VelocityRatio = CurrentVelocity / MaximumVelocity;
    }

    private void UpdateUI() {
        float throttleTop = ThrottleBackground.rect.height - ThrottleHandle.rect.height;
        ThrottleHandle.localPosition = new Vector2(ThrottleBackground.rect.width / 2f, throttleTop * ThrottleRatio);
    }
}
