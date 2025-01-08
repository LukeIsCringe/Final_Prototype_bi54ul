using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{
    private static int currentScene;

    private void Start()
    {
        currentScene = 0;
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "LevelLoader")
        {
            SceneManager.LoadScene(currentScene + 1);
        }
    }
}
