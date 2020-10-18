using UnityEngine;

public class MovementReference : MonoBehaviour {

    public enum ReferenceType {
        Grid,
        Circle
    }

    public GameObject ReferencePrefab;
    public ReferenceType type;
    public float ReferenceSize = 20f;
    public float ReferenceSpacing = 5f;
    public int RefernceCount = 8;


    private void Awake() {
        switch (type) {
            case ReferenceType.Grid:
                GridReference();
                break;
            case ReferenceType.Circle:
                CirlceReference();
                break;
        }
    }

    private void CirlceReference() {
        for (int i = 0; i < RefernceCount; i++) {
            Vector3 pos = new Vector3 {
                x = Mathf.Cos(i * Mathf.PI / (0.5f * RefernceCount)) * ReferenceSize,
                z = Mathf.Sin(i * Mathf.PI / (0.5f * RefernceCount)) * ReferenceSize
            };
            Instantiate(ReferencePrefab, pos, Quaternion.identity, transform).name = string.Format("{0}_{1}", ReferencePrefab.name, i);
        }
    }

    private void GridReference() {
        for (float i = 0, x = 0; x <= ReferenceSize / ReferenceSpacing; x++) {
            for (float z = 0; z <= ReferenceSize / ReferenceSpacing; z++, i++) {
                Vector3 pos = new Vector3 {
                    x = x * ReferenceSpacing - ReferenceSize * 0.5f,
                    z = z * ReferenceSpacing - ReferenceSize * 0.5f
                };
                Instantiate(ReferencePrefab, pos, Quaternion.identity, transform).name = string.Format("{0}_{1}", ReferencePrefab.name, (int)i);
            }
        }
    }
}
