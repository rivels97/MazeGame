using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Chase Settings")]
    [SerializeField] private float updatePathRate = 0.2f;
    [SerializeField] private float searchRadius = 2f;

    [Header("Audio Settings - Мычание")]
    [SerializeField] private AudioClip[] mooSounds; // Массив звуков мычания
    [SerializeField] private float minInterval = 3f;  // Минимальная пауза между звуками
    [SerializeField] private float maxInterval = 7f;  // Максимальная пауза между звуками
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float volume = 0.7f;

    private NavMeshAgent agent;
    private float nextPathUpdateTime;
    private AudioSource audioSource;
    private float nextSoundTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // === ОДИН AUDIOSOURCE ДЛЯ МЫЧАНИЯ ===
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D звук
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = volume;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        Debug.Log("AudioSource для мычания создан!");
        Debug.Log($"Звуков мычания: {mooSounds?.Length ?? 0}");
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        // Первое мычание через случайное время
        nextSoundTime = Time.time + Random.Range(1f, 3f);
    }

    private void Update()
    {
        if (player == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        if (Time.time >= nextPathUpdateTime)
        {
            UpdatePath();
            nextPathUpdateTime = Time.time + updatePathRate;
        }

        // ===== МЫЧАНИЕ С ПЕРЕРЫВОМ =====
        if (Time.time >= nextSoundTime)
        {
            PlayMoo();
            // Следующее мычание через случайное время
            nextSoundTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }

    private void UpdatePath()
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(player.position, out hit, searchRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void PlayMoo()
    {
        if (mooSounds == null || mooSounds.Length == 0)
        {
            Debug.LogWarning("Нет звуков мычания!");
            return;
        }

        // Выбираем случайный звук из массива
        AudioClip clip = mooSounds[Random.Range(0, mooSounds.Length)];

        // Случайная высота звука для разнообразия
        audioSource.pitch = Random.Range(0.9f, 1.1f);

        // Воспроизводим
        audioSource.PlayOneShot(clip, volume);

        Debug.Log($"Мычание! Следующее через {Random.Range(minInterval, maxInterval):F1} сек");
    }
}