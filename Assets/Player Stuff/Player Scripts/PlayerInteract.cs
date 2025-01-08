using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private GameObject environment;

    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;

    private PlayerUI playerUI;

    [SerializeField] private GameObject inputManagerGO;
    private InputManager inputManager;

    [SerializeField] private GameObject playerHand;
    private HandManager handManager;

    [SerializeField] private GameObject heldObjectGO;

    private int itemHeldCount;
    private Box box;
    private Sphere sphere;

    private void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        inputManager = inputManagerGO.GetComponent<InputManager>();

        handManager = playerHand.GetComponent<HandManager>();

        itemHeldCount = 0;

        box = GetComponent<Box>();
    }

    void Update()
    {
        SetHeldItem();
        rayInitialiser();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Win")
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Win_Screen");
        }
    }

    void rayInitialiser()
    {
        playerUI.UpdateText(string.Empty);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;

        

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                playerUI.UpdateText(interactable.promptMessage);
                if (inputManager.PlayerInteracted())
                {
                    interactable.BaseInteract();
                    heldObjectGO = hitInfo.collider.gameObject;
                    box = heldObjectGO.GetComponent<Box>();
                }
            }

            if (hitInfo.collider.tag != "Box" && heldObjectGO != null && itemHeldCount == 1)
            {
                box.gameObject.transform.SetParent(environment.transform);
                box.rb.isKinematic = false;

                handManager.handsEmpty = true;
                box.inHand = false;
                box.canLetGo = false;
                box.moveObject = false;
            }

            if (hitInfo.collider.tag != "Sphere" && heldObjectGO != null && itemHeldCount == 2)
            {
                sphere.gameObject.transform.SetParent(environment.transform);
                sphere.rb.isKinematic = false;

                handManager.handsEmpty = true;
                sphere.inHand = false;
                sphere.canLetGo = false;
                sphere.moveObject = false;
            }
        }
    }

    

    private void SetHeldItem()
    {
        if (heldObjectGO != null && heldObjectGO.tag == "Box")
        {
            box = heldObjectGO.GetComponent<Box>();
            itemHeldCount = 1;
        }

        if (heldObjectGO != null && heldObjectGO.tag == "Sphere")
        {
            sphere = heldObjectGO.GetComponent<Sphere>();
            itemHeldCount = 2;
        } 
    }
}
