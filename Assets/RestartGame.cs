using UnityEngine;
using UnityEngine.SceneManagement;
public class RestartGame : MonoBehaviour
{
    public void onClick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Puzzle_1");
    }
}
