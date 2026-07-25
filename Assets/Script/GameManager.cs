using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Настройки")]
    [SerializeField] private int totalItems = 5;
    private int collectedItems = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI counterText;        // Текст в меню паузы
    [SerializeField] private TextMeshProUGUI hudCounterText;     // Текст на HUD (всегда виден)

    [Header("Дверь выхода")]
    [SerializeField] private ExitDoor exitDoor;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;            // Перетащите mus_create сюда
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Создаем AudioSource для звуков
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // 2D звук (везде одинаково)
        audioSource.volume = 0.5f;
    }

    void Start()
    {
        StartCoroutine(InitAfterSpawn());
        LockDoor();
    }

    System.Collections.IEnumerator InitAfterSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        totalItems = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        Debug.Log($"Всего артефактов в сцене: {totalItems}");
        UpdateUI();
    }

    public void CollectItem()
    {
        collectedItems++;
        UpdateUI();
        Debug.Log($"Собрано: {collectedItems}/{totalItems}");

        // Воспроизводим звук сбора
        PlayCollectSound();

        if (collectedItems >= totalItems)
            UnlockDoor();
    }

    public void PlayCollectSound()
    {
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
    }

    void UpdateUI()
    {
        string text = $"Артефактов: {collectedItems}/{totalItems}";

        if (counterText != null)
            counterText.text = text;

        if (hudCounterText != null)
            hudCounterText.text = text;
    }

    void LockDoor()
    {
        // Дверь стоит закрытой по умолчанию
    }

    void UnlockDoor()
    {
        if (exitDoor != null)
        {
            exitDoor.OpenDoor();
            Debug.Log("Все артефакты собраны — выход открыт!");
        }
        else
        {
            Debug.LogError("ExitDoor не назначен в GameManager!");
        }
    }
}