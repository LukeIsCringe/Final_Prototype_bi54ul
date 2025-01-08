using Unity.VisualScripting;
using UnityEngine;

public class PlayerInRange : MonoBehaviour
{
    public Animator anim;
    [SerializeField] private AudioSource openSound;

    [SerializeField] private SwapObject swapObject;

    [SerializeField] private GameObject switchPlane;
    [SerializeField] private ObjectOnPlane obj;

    private void Start()
    {
        obj = switchPlane.GetComponent<ObjectOnPlane>();
    }
    private void Update()
    {
        swapObject = obj.inSwitcher.GetComponent<SwapObject>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "PlayerTag" && !swapObject.switchingObjs)
        {
            anim.SetBool("playerInRange", true);
            openSound.Play();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "PlayerTag" && !swapObject.switchingObjs)
        {
            anim.SetBool("playerInRange", false);
            if (!openSound.isPlaying)
            {
                openSound.Play();
            }
        }
    }
}