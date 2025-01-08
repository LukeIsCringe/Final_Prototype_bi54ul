using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class StopClipping : MonoBehaviour
{
    private HandManager handManager;
    [SerializeField] private GameObject playerHand;

    private Box box;
    private Sphere sphere;

    public int LayerNumber;

    private void Start()
    {
        handManager = playerHand.GetComponent<HandManager>();
        LayerNumber = LayerMask.NameToLayer("holdLayer");
    }

    private void Update()
    {
        if (handManager.heldObj != null && handManager.heldObj.tag == "Box")
        {
            box = handManager.heldObj.gameObject.GetComponent<Box>();
        }

        if (handManager.heldObj != null && handManager.heldObj.tag == "Sphere")
        {
            sphere = handManager.heldObj.gameObject.GetComponent<Sphere>();
        }

        runStopClipping();
    }

    void runStopClipping()
    {
        if (handManager.heldObj != null)
        {
            stopClipping();
            StopDroppingThroughWalls();
        }
    }

    void stopClipping()
    {
        var clipRange = Vector3.Distance(handManager.heldObj.transform.position, transform.position);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);

        if (hits.Length > 1 && handManager.heldObj.tag == "Box")
        {
            box.KeepInWorld();
        }

        if (hits.Length > 1 && handManager.heldObj.tag == "Sphere")
        {
            sphere.KeepInWorld();
        }
    }

    void StopDroppingThroughWalls()
    {
        var ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.cyan);

            if (hit.transform.gameObject.tag != "Box" && handManager.heldObj != null && handManager.heldObj.tag == "Box")
            {
                handManager.heldObj.GetComponent<Box>().canLetGo = false;
            }

            if (hit.transform.gameObject.tag != "Sphere" && handManager.heldObj != null && handManager.heldObj.tag == "Sphere")
            {
                handManager.heldObj.GetComponent<Sphere>().canLetGo = false;
            }

            else
            {
                if (handManager.heldObj.tag == "Box")
                {
                    handManager.heldObj.GetComponent<Box>().canLetGo = true;
                }
                if (handManager.heldObj.tag == "Sphere")
                {
                    handManager.heldObj.GetComponent<Sphere>().canLetGo = true;
                }
                if (handManager.heldObj.tag == "Box")
                {
                    handManager.heldObj.GetComponent<Box>().canLetGo = true;
                }
            }

            hit = new RaycastHit();
        }
    }
}
