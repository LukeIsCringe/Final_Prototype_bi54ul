using UnityEngine;

public class Sphere : Interactable
{
    [SerializeField] private GameObject playerHand;
    public GameObject environment;
    [SerializeField] private Camera cam;

    [SerializeField] private GameObject inputManagerGO;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Animator doorAnim;
    [SerializeField] private Animator PPanim;
    [SerializeField] private Animator liftAnim;
    
    public GameObject floor;

    public bool inFloor;
    public bool sphereGrounded;
    public bool keepInWorld;

    public Rigidbody rb;
    public bool inHand;
    public bool canLetGo;
    public bool canPickUp;
    public bool moveObject;

    [SerializeField] private float magnitude;

    private HandManager handManager;

    private StopClipping stopClipping;

    private void Start()
    {
        handManager = playerHand.GetComponent<HandManager>();
        inputManager = inputManagerGO.GetComponent<InputManager>();
        stopClipping = cam.GetComponent<StopClipping>();

        inHand = false;
        inFloor = false;
        canLetGo = false;
        canPickUp = true;
        moveObject = false;
        keepInWorld = false;

        magnitude = 8f;
    }

    private void Update()
    {
        ThrowSphere();
        MoveObject();
        DropSphere();

        if (keepInWorld)
        {
            KeepInWorld();
        }
    }

    // this function is where the design for the interaction using code
    protected override void Interact()
    {
        if (handManager.handsEmpty && canPickUp)
        {
            Invoke("CanLetGo", 1f);

            gameObject.transform.SetParent(playerHand.transform);

            moveObject = true;
            gameObject.transform.SetParent(playerHand.transform);
            gameObject.layer = stopClipping.LayerNumber;

            rb.isKinematic = true;
            Debug.Log("Interacted with " + gameObject.name);
            handManager.handsEmpty = false;
            inHand = true;
            canPickUp = false;
        }
    }

    private void DropSphere()
    {
        if (inHand && canLetGo && inputManager.PlayerInteracted())
        {
            gameObject.transform.SetParent(environment.transform);
            rb.isKinematic = false;

            Debug.Log("Let Go of " + gameObject.name);

            gameObject.layer = 3;
            handManager.handsEmpty = true;
            inHand = false;
            canLetGo = false;
            moveObject = false;
            Invoke("CanPickUp", 1f);
        }
    }

    private void ThrowSphere()
    {
        if (inHand && canLetGo && inputManager.ThrowActivated())
        {
            gameObject.transform.SetParent(environment.transform);
            rb.isKinematic = false;
            Debug.Log(gameObject.name + " thrown");

            gameObject.layer = 3;

            rb.AddForce(cam.transform.forward * magnitude, ForceMode.Impulse);

            handManager.handsEmpty = true;
            inHand = false;
            canLetGo = false;
            canPickUp = true;
            moveObject = false;
        }
    }

    private void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            sphereGrounded = false;
        }
    }

    private void SetAnimSpeed()
    {
        doorAnim.speed = 1;
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "Stopper" && !inHand)
        {
            doorAnim.speed = 0;
        }

        if (trigger.gameObject.tag == "Player" && !handManager.handsEmpty && inHand)
        {
            rb.isKinematic = false;
            gameObject.transform.SetParent(environment.transform);
            gameObject.layer = 3;

            //dropItemHitbox.SetActive(false);
            inHand = false;
            canLetGo = false;
            canPickUp = true;
            handManager.handsEmpty = true;

        }

        if (trigger.gameObject.tag == "Floor")
        {
            floor = trigger.gameObject;
            sphereGrounded = true;
        }

        if (trigger.gameObject.tag == "PressurePad")
        {
            PPanim.SetBool("Bucketed", true);
            liftAnim.SetBool("Bucketed", true);
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.tag == "Floor")
        {
            sphereGrounded = false;
        }

        if (trigger.gameObject.tag == "Stopper")
        {
            Invoke("SetAnimSpeed", 1f);
        }
    }

    private void CanLetGo()
    {
        canLetGo = true;
    }

    private void CanPickUp()
    {
        canPickUp = true;
    }

    private void MoveObject()
    {
        if (moveObject)
        {
            var step = 5f * Time.deltaTime;
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, playerHand.transform.position, step);
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, playerHand.transform.forward, step, 1f);

            Debug.DrawRay(transform.position, newDirection, Color.red);

            gameObject.transform.rotation = Quaternion.LookRotation(newDirection);
        }

        if (gameObject.transform.position == playerHand.transform.position && gameObject.transform.rotation == playerHand.transform.rotation)
        {
            moveObject = false;
        }
    }

    public void KeepInWorld()
    {
        if (gameObject.transform.position.y < floor.transform.position.y + 0.5 && inHand)
        {
            gameObject.transform.position = new Vector3(playerHand.transform.position.x, playerHand.transform.position.y + 0.5f, playerHand.transform.position.z);
        }

        if (gameObject.transform.position.y < floor.transform.position.y + 0.5 && !inHand)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z);
        }
    }
}

