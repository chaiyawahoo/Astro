using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

    public enum SpawnType {
        Grid,
        Cube,
        Circle
    }

    public GameObject ObstaclePrefab;
    public SpawnType type;
    public float ObstacleBounds = 20f;
    public float ObstacleGap = 5f;
    public int ObstacleCount = 8;


    private void Awake() {
        switch (type) {
            case SpawnType.Grid:
                GridReference();
                break;
            case SpawnType.Cube:
                CubeReference();
                break;
            case SpawnType.Circle:
                CirlceReference();
                break;
        }
    }

    private void CirlceReference() {
        for (int i = 0; i < ObstacleCount; i++) {
            Vector3 pos = new Vector3 {
                x = Mathf.Cos(i * Mathf.PI / (0.5f * ObstacleCount)) * ObstacleBounds,
                z = Mathf.Sin(i * Mathf.PI / (0.5f * ObstacleCount)) * ObstacleBounds
            };
            Instantiate(ObstaclePrefab, pos, Quaternion.identity, transform).name = string.Format("{0}_{1}", ObstaclePrefab.name, i);
        }
    }

    private void GridReference() {
        for (float i = 0, x = 0; x <= ObstacleBounds / ObstacleGap; x++) {
            for (float z = 0; z <= ObstacleBounds / ObstacleGap; z++, i++) {
                Vector3 pos = new Vector3 {
                    x = x * ObstacleGap - ObstacleBounds * 0.5f,
                    z = z * ObstacleGap - ObstacleBounds * 0.5f
                };
                if (!(pos.x == 0f && pos.y == 0f)) {
                    Instantiate(ObstaclePrefab, pos, Quaternion.identity, transform).name = string.Format("{0}_{1}", ObstaclePrefab.name, (int)i);
                }
            }
        }
    }

    private void CubeReference() {
        for (float x = 0, i = 0; x <= ObstacleBounds / ObstacleGap; x++) {
            for (float y = 0; y <= ObstacleBounds / ObstacleGap; y++) {
                for (float z = 0; z <= ObstacleBounds / ObstacleGap; z++, i++) {
                    Vector3 pos = new Vector3 {
                        x = x * ObstacleGap - ObstacleBounds * 0.5f,
                        y = y * ObstacleGap - ObstacleBounds * 0.5f,
                        z = z * ObstacleGap - ObstacleBounds * 0.5f
                    };
                    if (!(pos.x == 0f && pos.y == 0f && pos.z == 0f)) {
                        GameObject cube = Instantiate(ObstaclePrefab, pos, Quaternion.identity, transform);
                        cube.name = string.Format("{0}_{1}", ObstaclePrefab.name, (int)i);
                        Color posColor = new Color {
                            r = x / (ObstacleBounds / ObstacleGap),
                            g = y / (ObstacleBounds / ObstacleGap),
                            b = z / (ObstacleBounds / ObstacleGap),
                            a = 0.5f
                        };
                        cube.GetComponent<MeshRenderer>().material.color = posColor;
                        cube.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", posColor);
                    }
                }
            }
        }
    }
}
