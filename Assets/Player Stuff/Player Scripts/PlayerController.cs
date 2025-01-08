using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private int loadedScene;

    [SerializeField] private CharacterController controller;
    [SerializeField] private Vector3 playerVelocity;

    [SerializeField]private bool groundedPlayer;

    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    private InputManager inputManager;

    [SerializeField] private GameObject SLM;
    private SoftlockManager softlockManager;

    private Transform cameraTransform;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;

        softlockManager = SLM.GetComponent<SoftlockManager>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);

        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0;

        controller.Move(move * Time.deltaTime * playerSpeed);

        // Makes the player jump
        if (inputManager.PlayerJumped() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (Input.GetKeyDown("]"))
        {
            loadedScene = SceneManager.GetActiveScene().buildIndex;
            
            SceneManager.LoadScene(loadedScene);
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "PSL")
        {
            softlockManager.playerSoftLocked = true;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.tag == "PSL")
        {
            softlockManager.playerSoftLocked = false;
        }
    }
}
