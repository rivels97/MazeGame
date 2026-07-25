using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float gravityScale = 1f;

    [Header("Audio Settings")]
    public AudioClip footstepSound;
    public float maxVolume = 0.3f;
    public float minSpeed = 0.1f;
    public float maxSpeedForSound = 7f;
    public float fadeSpeed = 3f;

    private CharacterController controller;
    private AudioSource audioSource;
    private Vector3 velocity;
    private float targetVolume = 0f;
    private bool isMoving = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

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
        controller.Move(move * speed * Time.deltaTime);

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

        // ===== ЗВУК =====
        isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        if (isMoving && currentSpeed > minSpeed && controller.isGrounded)
        {
            float normalizedSpeed = Mathf.InverseLerp(minSpeed, maxSpeedForSound, currentSpeed);
            targetVolume = normalizedSpeed * maxVolume;
            audioSource.pitch = Mathf.Lerp(0.8f, 1.2f, normalizedSpeed);
        }
        else
        {
            targetVolume = 0f;
        }

        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);

        // ===== ОТЛАДКА (ВСТАВЬТЕ ЭТОТ БЛОК) =====
        Debug.Log($"Moving: {isMoving} | Speed: {currentSpeed:F2} | TargetVol: {targetVolume:F2} | ActualVol: {audioSource.volume:F2} | Grounded: {controller.isGrounded}");
    }
}