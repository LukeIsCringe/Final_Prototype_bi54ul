using UnityEngine;

public class HandManager : MonoBehaviour
{
    public bool handsEmpty;
    public GameObject heldObj;

    private void Start()
    {
        handsEmpty = true;
    }

    private void Update()
    {
        setObj();
    }

    private void setObj()
    {
        if (!handsEmpty)
        {
            heldObj = gameObject.transform.GetChild(0).gameObject;
        }

        if (handsEmpty)
        {
            heldObj = null;
        }
    }
}
