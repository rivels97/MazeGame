using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Коллектибл")]
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Настройки генерации")]
    [SerializeField] private int totalItems = 5;
    [SerializeField] private float minDistanceBetween = 5f;  // минимум между артефактами
    [SerializeField] private float minDistanceFromPlayer = 8f; // минимум от игрока
    [SerializeField] private float spawnHeight = 0.5f; // высота спавна над полом
    [SerializeField] private Transform player;

    [Header("Отладка")]
    [SerializeField] private bool showGizmos = true;

    private List<Vector3> spawnedPositions = new List<Vector3>();
    private int maxAttempts = 100; // максимум попыток найти точку

    void Start()
    {
        StartCoroutine(SpawnCollectibles());
    }

    IEnumerator SpawnCollectibles()
    {
        // Ждём пока NavMesh прокарвится
        yield return new WaitForSeconds(0.3f);

        int spawned = 0;
        int attempts = 0;

        while (spawned < totalItems && attempts < maxAttempts * totalItems)
        {
            attempts++;

            // Получаем случайную точку на NavMesh
            Vector3 randomPoint;
            if (!GetRandomNavMeshPoint(out randomPoint))
                continue;

            // Проверяем дистанцию от игрока
            if (player != null && Vector3.Distance(randomPoint, player.position) < minDistanceFromPlayer)
                continue;

            // Проверяем дистанцию от других артефактов
            if (IsTooCloseToOthers(randomPoint))
                continue;

            // Спавним артефакт
            Vector3 spawnPos = new Vector3(randomPoint.x, randomPoint.y + spawnHeight, randomPoint.z);
            Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);
            spawnedPositions.Add(spawnPos);
            spawned++;

            Debug.Log($"Артефакт {spawned} заспавнен на {spawnPos}");
        }

        if (spawned < totalItems)
            Debug.LogWarning($"Удалось заспавнить только {spawned}/{totalItems} артефактов! Увеличь лабиринт или уменьши minDistanceBetween");
        else
            Debug.Log("Все артефакты успешно заспавнены!");
    }

    bool GetRandomNavMeshPoint(out Vector3 result)
    {
        // Берём случайную точку в большом радиусе вокруг центра
        Vector3 randomDirection = Random.insideUnitSphere * 50f;
        randomDirection += Vector3.zero; // центр лабиринта

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    bool IsTooCloseToOthers(Vector3 point)
    {
        foreach (Vector3 existing in spawnedPositions)
        {
            if (Vector3.Distance(point, existing) < minDistanceBetween)
                return true;
        }
        return false;
    }

    // Отображение точек в редакторе
    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        foreach (Vector3 pos in spawnedPositions)
        {
            Gizmos.DrawWireSphere(pos, 0.5f);
        }
    }
}