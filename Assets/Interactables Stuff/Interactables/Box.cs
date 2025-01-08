using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Box : Interactable
{
    [SerializeField] private GameObject playerHand;
    public GameObject environment;
    [SerializeField] private Camera cam;

    [SerializeField] private GameObject inputManagerGO;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Animator buttonAnim;

    [SerializeField] private GameObject SLM;
    private SoftlockManager softlockManager;

    [SerializeField] private Animator doorAnim;
    [SerializeField] private Animator rampAnim;

    //[SerializeField] private GameObject dropItemHitbox;
    public GameObject floor;
    public GameObject wall;

    public bool inFloor;
    public bool boxGrounded;
    public bool keepInWorld;

    public Rigidbody rb;
    public bool inHand;
    public bool canLetGo;
    public bool canPickUp;
    public bool moveObject;

    private bool inX_Wall;
    private bool inZ_Wall;

    [SerializeField] private float magnitude;

    private HandManager handManager;

    private StopClipping stopClipping;

    private void Start()
    {
        handManager = playerHand.GetComponent<HandManager>();
        inputManager = inputManagerGO.GetComponent<InputManager>();
        stopClipping = cam.GetComponent<StopClipping>();
        softlockManager = SLM.GetComponent<SoftlockManager>();

        //dropItemHitbox.SetActive(false);

        inHand = false;
        inFloor = false;
        canLetGo = false;
        canPickUp = true;
        moveObject = false;
        keepInWorld = false;

        inX_Wall = false;
        inZ_Wall = false;

        magnitude = 8f;
    }

    private void Update()
    {
        ThrowBox();
        MoveObject();
        DropBox();

        if (keepInWorld)
        {
            KeepInWalls();
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

    private void DropBox()
    {
        if (inHand && canLetGo && inputManager.PlayerInteracted())
        {
            Debug.Log("WORKING");

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

    private void ThrowBox()
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
            boxGrounded = false;
        }
    }

    private void SetAnimSpeed()
    {
        doorAnim.speed = 1;
    }

    private void OnTriggerEnter(Collider trigger)
    {
        // Not Being Used Now
        /*if (trigger.gameObject.tag == "DropItemHitbox" && !handManager.handsEmpty && inHand)
        {
            gameObject.transform.SetParent(environment.transform);
            dropItemHitbox.SetActive(false);
            inHand = false;
            handManager.handsEmpty = true;
            rb.isKinematic = false;
        } */

        if (trigger.gameObject.tag == "PressurePad")
        {
            buttonAnim.SetBool("CubeOnButton", true);
            rampAnim.SetBool("ButtonPressed", true);

        }

        if (trigger.gameObject.tag == "Wall")
        {
            wall = trigger.gameObject;
        }

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

        if (trigger.gameObject.tag == "CSL" && !inHand)
        {
            softlockManager.cubeSoftLocked = true;
        }

        else if (trigger.gameObject.tag == "Floor")
        {
            floor = trigger.gameObject;
            boxGrounded = true;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.tag == "PressurePad")
        {
            buttonAnim.SetBool("CubeOnButton", false);
        }

        if (trigger.gameObject.tag == "Floor")
        {
            boxGrounded = false;
        }

        if (trigger.gameObject.tag == "Stopper")
        {
            Invoke("SetAnimSpeed", 1f);
        }

        if (trigger.gameObject.tag == "CSL")
        {
            softlockManager.cubeSoftLocked = false;
        }

        inX_Wall = false;
        inZ_Wall = false;
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

    public void KeepInWalls()
    {

        if (gameObject.transform.position.z < wall.transform.position.z + 0.5 && inHand)
        {
            gameObject.transform.position = new Vector3(playerHand.transform.position.x,
                playerHand.transform.position.y, playerHand.transform.position.z + 0.5f);

            gameObject.transform.SetParent(environment.transform);
            rb.isKinematic = false;
            Debug.Log("Hit Wall");

            handManager.handsEmpty = true;
            inHand = false;
            canLetGo = false;
            canPickUp = true;
            moveObject = false;

        }

        if (gameObject.transform.position.z < wall.transform.position.z + 0.5 && inHand)
        {
            gameObject.transform.position = new Vector3(playerHand.transform.position.x, playerHand.transform.position.y, playerHand.transform.position.z + 0.5f);

            gameObject.transform.SetParent(environment.transform);
            rb.isKinematic = false;
            Debug.Log("Hit Wall");

            handManager.handsEmpty = true;
            inHand = false;
            canLetGo = false;
            canPickUp = true;
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
