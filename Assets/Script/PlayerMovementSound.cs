using UnityEngine;
using TMPro;
using UnityEngine.UI; // ДОБАВЛЯЕМ для работы с Image

public class PlayerMovementSound : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;       // В 2 раза быстрее
    public float jumpHeight = 2f;
    public float gravityScale = 1f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 30f;   // Сколько стамины тратится в секунду
    public float staminaRegenRate = 20f;   // Сколько восстанавливается в секунду
    public float minStaminaToSprint = 10f; // Минимальная стамина для спринта

    [Header("Audio Settings")]
    public AudioClip footstepSound;
    public float maxVolume = 0.3f;
    public float minSpeed = 0.1f;
    public float maxSpeedForSound = 7f;
    public float fadeSpeed = 3f;

    [Header("UI")]
    public UnityEngine.UI.Slider staminaSlider;     // Полоска стамины
    public TMPro.TextMeshProUGUI staminaText;       // Текст с цифрами (опционально)
    public Image staminaFillImage;                  // <--- НОВОЕ ПОЛЕ для цвета полоски

    private CharacterController controller;
    private AudioSource audioSource;
    private Vector3 velocity;
    private float targetVolume = 0f;
    private bool isMoving = false;

    private float currentStamina;
    private bool isSprinting = false;

    void Start()
    {
        Debug.Log("=== PlayerMovementSound ЗАПУЩЕН! ===");

        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("!!! НЕТ CharacterController на объекте !!!");
            return;
        }

        currentStamina = maxStamina;
        UpdateStaminaUI();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = footstepSound;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.pitch = 1f;
        audioSource.spatialBlend = 1f;
        audioSource.Play();
    }

    void Update()
    {
        // ===== ДВИЖЕНИЕ =====
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        // ===== СПРИНТ (Shift) =====
        isSprinting = Input.GetKey(KeyCode.LeftShift) && move.magnitude > 0.1f && currentStamina > minStaminaToSprint;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 currentVelocity = move * currentSpeed;

        controller.Move(currentVelocity * Time.deltaTime);

        // ===== СТАМИНА =====
        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }

        UpdateStaminaUI();

        // ===== ПРЫЖОК =====
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityScale);
        }

        // ===== ГРАВИТАЦИЯ =====
        velocity.y += Physics.gravity.y * gravityScale * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // ===== ЗВУК ХОДЬБЫ =====
        isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;
        float currentSpeedForSound = isMoving ? currentSpeed : 0f;

        if (isMoving && currentSpeedForSound > minSpeed && controller.isGrounded)
        {
            float normalizedSpeed = Mathf.InverseLerp(minSpeed, maxSpeedForSound, currentSpeedForSound);
            targetVolume = normalizedSpeed * maxVolume;
            audioSource.pitch = Mathf.Lerp(0.8f, 1.2f, normalizedSpeed);
        }
        else
        {
            targetVolume = 0f;
        }

        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);

        // ===== ОТЛАДКА =====
        Debug.Log($"Moving: {isMoving} | Speed: {currentSpeed:F2} | Stamina: {currentStamina:F1} | Sprinting: {isSprinting}");
    }

    void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina / maxStamina; // 0 - 1
        }

        if (staminaText != null)
        {
            staminaText.text = $"{Mathf.RoundToInt(currentStamina)}/{maxStamina}";
        }

        // ===== МЕНЯЕМ ЦВЕТ ПОЛОСКИ =====
        UpdateStaminaColor();
    }

    // ===== НОВЫЙ МЕТОД ДЛЯ СМЕНЫ ЦВЕТА =====
    void UpdateStaminaColor()
    {
        if (staminaFillImage == null) return;

        float percent = currentStamina / maxStamina;

        if (percent > 0.5f)
        {
            // От зеленого к желтому (100% → 50%)
            staminaFillImage.color = Color.Lerp(Color.yellow, Color.green, (percent - 0.5f) * 2f);
        }
        else
        {
            // От желтого к красному (50% → 0%)
            staminaFillImage.color = Color.Lerp(Color.red, Color.yellow, percent * 2f);
        }
    }
}