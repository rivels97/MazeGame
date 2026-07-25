using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.3f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        // Добавляем триггер коллайдер если нет
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void Update()
    {
        // Вращение
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        // Парение вверх-вниз
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.CollectItem();
        Destroy(gameObject);
    }
}