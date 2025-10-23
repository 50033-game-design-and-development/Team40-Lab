using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        var mario = collision.GetComponent<MarioController>();
        if (mario != null)
        {
            if (SceneManager.GetActiveScene().name == "Week 5-2")
                SceneManager.LoadScene("Week 5-1");
            else
                SceneManager.LoadScene("Week 5-2");
        }
    }
}
