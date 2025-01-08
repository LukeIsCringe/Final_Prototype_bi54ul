/*using Unity.VisualScripting;
using UnityEngine;

public class detectFloor : MonoBehaviour
{
    private GameObject playerFloor;
    private Box box;

    [SerializeField] private GameObject playerHands;
    private HandManager handManger;

    private void Start()
    {
        handManger = playerHands.GetComponent<HandManager>();
    }

    private void Update()
    {
        setBox();
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.tag == "Floor")
        {
            playerFloor = trigger.gameObject;
        }
    }

    private void setBox()
    {
        if (handManger.heldObj != null)
        {
            box = handManger.heldObj.GetComponent<Box>();
        }
    }

    private void balanceFloors()
    {
        if (playerFloor != box.floor)
        {
            box.floor = playerFloor;
        }
    }
}
*/