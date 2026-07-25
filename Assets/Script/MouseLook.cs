using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivityX = 10f;
    public float sensitivityY = 10f;
    public Transform body;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        body.Rotate(Vector3.up * mouseX);
        transform.Rotate(-mouseY, 0, 0);
    }
}
