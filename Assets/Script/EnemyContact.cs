using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyContact : MonoBehaviour
{
    [SerializeField] private string gameOverScene = "GameOver";

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(gameOverScene);
    }
}










