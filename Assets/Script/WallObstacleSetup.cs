using UnityEngine;
using UnityEngine.AI;

public class WallObstacleSetup : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;

    void Start()
    {
        // Находим все коллайдеры на слое Wall
        Collider[] wallColliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (Collider col in wallColliders)
        {

            if (((1 << col.gameObject.layer) & wallLayer) == 0)
                continue;


            if (col.GetComponent<NavMeshObstacle>() != null)
                continue;

            NavMeshObstacle obstacle = col.gameObject.AddComponent<NavMeshObstacle>();


            if (col is BoxCollider box)
            {
                obstacle.shape = NavMeshObstacleShape.Box;
                obstacle.center = box.center;
                obstacle.size = box.size;
            }
            else if (col is CapsuleCollider capsule)
            {
                obstacle.shape = NavMeshObstacleShape.Capsule;
                obstacle.center = capsule.center;
                obstacle.radius = capsule.radius;
                obstacle.height = capsule.height;
            }

            obstacle.carving = true;
            obstacle.carveOnlyStationary = true;
            obstacle.carvingMoveThreshold = 0.1f;
            obstacle.carvingTimeToStationary = 0.5f;
        }
    }
}