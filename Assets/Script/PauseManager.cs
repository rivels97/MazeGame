using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private MonoBehaviour mouseLookScript; // УБИРАЕМ [SerializeField]
    private bool isPaused = false;

    void Start()
    {
        // Автоматически находим скрипт управления камерой на Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Ищем MouseCamLook или MouseLook на Player или его детях
            mouseLookScript = player.GetComponent<MouseCamLook>();
            if (mouseLookScript == null)
                mouseLookScript = player.GetComponent<MouseLook>();

            if (mouseLookScript == null)
                mouseLookScript = player.GetComponentInChildren<MouseCamLook>();
            if (mouseLookScript == null)
                mouseLookScript = player.GetComponentInChildren<MouseLook>();
        }

        if (mouseLookScript == null)
            Debug.LogWarning("Скрипт управления камерой не найден! Поищи вручную.");

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        else
        {
            Debug.LogError("ОШИБКА: pauseMenuUI не назначен в инспекторе!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // ОТКЛЮЧАЕМ УПРАВЛЕНИЕ КАМЕРОЙ
        if (mouseLookScript != null)
            mouseLookScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // ВКЛЮЧАЕМ УПРАВЛЕНИЕ КАМЕРОЙ ОБРАТНО
        if (mouseLookScript != null)
            mouseLookScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}