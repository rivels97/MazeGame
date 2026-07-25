using UnityEngine;
using System.Collections;

public class ExitDoor : MonoBehaviour
{
    [Header("Настройки анимации")]
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float openAngle = 90f;

    [Header("Триггер выхода")]
    [SerializeField] private GameObject exitTrigger;

    [Header("Луч")]
    [SerializeField] private GameObject lightBeam; // Перетащите сюда объект DoorLightBeam

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + openAngle,
            transform.eulerAngles.z
        );

        if (exitTrigger != null)
            exitTrigger.SetActive(false);

        // Луч выключен по умолчанию
        if (lightBeam != null)
            lightBeam.SetActive(false);
    }

    public void OpenDoor()
    {
        Debug.Log("OpenDoor вызван!");
        if (isOpen || isAnimating) return;
        StartCoroutine(AnimateDoor());
    }

    IEnumerator AnimateDoor()
    {
        isAnimating = true;
        float t = 0f;

        Debug.Log("Анимация двери началась");

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Lerp(closedRotation, openRotation, t);
            yield return null;
        }

        transform.rotation = openRotation;
        isOpen = true;
        isAnimating = false;

        if (exitTrigger != null)
            exitTrigger.SetActive(true);

        // ВКЛЮЧАЕМ ЛУЧ
        if (lightBeam != null)
            // Вместо lightBeam.SetActive(true);
            StartCoroutine(FadeInLight());

        IEnumerator FadeInLight()
        {
            lightBeam.SetActive(true);
            Light light = lightBeam.GetComponent<Light>();
            if (light != null)
            {
                float targetIntensity = light.intensity;
                light.intensity = 0f;
                float duration = 1f;
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime / duration;
                    light.intensity = Mathf.Lerp(0f, targetIntensity, t);
                    yield return null;
                }
                light.intensity = targetIntensity;
            }
        }

        Debug.Log("Дверь полностью открыта! Луч активирован!");
    }
}