using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSettings : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;          // Панель настроек
    public GameObject pauseMenuPanel;         // Основное меню паузы
    public Slider masterVolumeSlider;         // Слайдер общей громкости
    public TMP_Text masterVolumeText;         // Текст с процентами

    [Header("Pause Manager")]
    public PauseManager pauseManager;         // Ссылка на скрипт управления паузой

    private float masterVolume = 1f;

    void Start()
    {
        // Загружаем сохраненную громкость
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = masterVolume;

        // Настраиваем слайдер
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        UpdateVolumeText();
    }

    // ===== ОТКРЫТЬ НАСТРОЙКИ =====
    public void OpenSettings()
    {
        // Показываем панель настроек
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        // Скрываем основное меню паузы, НО игра остается на паузе
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Убеждаемся, что игра НА ПАУЗЕ (если есть PauseManager)
        if (pauseManager != null)
        {
            pauseManager.Pause(); // Принудительно включаем паузу
        }
        else
        {
            // Если нет PauseManager - используем Time.timeScale
            Time.timeScale = 0f;
        }
    }

    // ===== ЗАКРЫТЬ НАСТРОЙКИ =====
    public void CloseSettings()
    {
        // Скрываем панель настроек
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Показываем основное меню паузы
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        // Игра остается на паузе (меню паузы активно)
        // Не меняем Time.timeScale - пусть остается 0
    }

    // ===== ВЫЙТИ ИЗ МЕНЮ ПАУЗЫ (для кнопки "Продолжить") =====
    public void Resume()
    {
        // Закрываем все меню
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Возобновляем игру
        if (pauseManager != null)
        {
            pauseManager.Resume();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    // ===== ИЗМЕНЕНИЕ ГРОМКОСТИ =====
    public void OnMasterVolumeChanged(float value)
    {
        masterVolume = value;
        AudioListener.volume = masterVolume;

        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.Save();

        UpdateVolumeText();
    }

    // ===== ОБНОВЛЕНИЕ ТЕКСТА =====
    void UpdateVolumeText()
    {
        if (masterVolumeText != null)
        {
            int percent = Mathf.RoundToInt(masterVolume * 100f);
            masterVolumeText.text = $"{percent}%";
        }
    }
}