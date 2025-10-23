    using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{


    void Awake()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EdgeCollider2D>(out var edge))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
