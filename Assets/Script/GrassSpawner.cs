using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GrassSpawner : MonoBehaviour
{
    [Header("Префабы кустов (5 штук)")]
    [SerializeField] private GameObject[] bushPrefabs;

    [Header("Настройки")]
    [SerializeField] private int totalBushes = 1000;
    [SerializeField] private float minDistanceBetween = 1.5f;
    [SerializeField] private float searchRadius = 100f;
    [SerializeField] private float spawnHeightOffset = 0f;

    [Header("Случайный масштаб")]
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.3f;

    [Header("Статичная сборка")]
    [SerializeField] private bool makeStatic = true;

    private System.Collections.Generic.List<Vector3> spawnedPoints
        = new System.Collections.Generic.List<Vector3>();
    void Awake() { StartCoroutine(SpawnGrass()); }
    //void Start()
    //{
  //      StartCoroutine(SpawnGrass());
   // }

    IEnumerator SpawnGrass()
    {
        // Ждём пока NavMesh прокарвится
        yield return new WaitForSeconds(0f);

        // Родительский объект для порядка в иерархии
        GameObject parent = new GameObject("GrassContainer");

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = totalBushes * 50;

        while (spawned < totalBushes && attempts < maxAttempts)
        {
            attempts++;

            // Случайная точка в пределах сцены
            Vector3 randomPoint = Random.insideUnitSphere * searchRadius;
            randomPoint.y = 0;

            // Ищем ближайшую точку на NavMesh
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
                continue;

            // Проверяем минимальное расстояние между кустами
            if (IsTooClose(hit.position))
                continue;

            // Выбираем случайный префаб из 5
            GameObject prefab = bushPrefabs[Random.Range(0, bushPrefabs.Length)];

            // Случайный поворот по Y
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            // Случайный масштаб
            float scale = Random.Range(minScale, maxScale);

            // Спавним
            Vector3 pos = hit.position + Vector3.up * spawnHeightOffset;
            GameObject bush = Instantiate(prefab, pos, rotation, parent.transform);
            bush.transform.localScale = Vector3.one * scale;

            // Делаем статичным для Static Batching
            if (makeStatic)
                SetStaticRecursively(bush);

            spawnedPoints.Add(hit.position);
            spawned++;
        }

        Debug.Log($"GrassSpawner: заспавнено {spawned}/{totalBushes} кустов за {attempts} попыток");
    }

    bool IsTooClose(Vector3 point)
    {
        foreach (Vector3 existing in spawnedPoints)
        {
            if (Vector3.Distance(point, existing) < minDistanceBetween)
                return true;
        }
        return false;
    }

    void SetStaticRecursively(GameObject obj)
    {
        obj.isStatic = true;
        foreach (Transform child in obj.transform)
            SetStaticRecursively(child.gameObject);
    }
}