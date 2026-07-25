using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamLook : MonoBehaviour
{
    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;

    [Header("Vertical Limits")]
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;

    public GameObject character;
    private Vector2 mouseLook;
    private Vector2 smoothV;

    void Start()
    {
        character = this.transform.parent.gameObject;
    }

    void Update()
    {
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);

        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, minVerticalAngle, maxVerticalAngle);

        // Вращение камеры (вверх-вниз)
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);

        // Вращение персонажа (влево-вправо)
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Vector3.up);
    }
}