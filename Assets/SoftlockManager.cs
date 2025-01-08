using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoftlockManager : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private GameObject player;

    private Box box;
    [SerializeField]  private GameObject boxGO;

    public bool playerSoftLocked;
    public bool cubeSoftLocked;

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        box = boxGO.GetComponent<Box>();

        playerSoftLocked = false;
        cubeSoftLocked = false;
    }

    private void Update()
    {
        if (playerSoftLocked && cubeSoftLocked)
        {
            SceneManager.LoadScene("Puzzle_1");
        }
    }

    
}
