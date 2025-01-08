using UnityEngine;

public class SwapObject : MonoBehaviour
{
    [SerializeField] private GameObject swapObject;
    [SerializeField] private GameObject unswappedObject;

    [SerializeField] private AudioSource jeremyLine;
    [SerializeField] private AudioSource switchNoise;

    [SerializeField] private GameObject playerTrigger;
    private PlayerInRange pInRange;

    [SerializeField] private GameObject playerHand;
    private HandManager handManager;

    private Vector3 insideSwitcher;
    private Vector3 outsideSwitcher;

    private Box box;
    private Sphere sphere;

    private ObjectOnPlane obj;
    [SerializeField] private GameObject switchPlane;

    [SerializeField] private Animator doorAnim;

    [SerializeField] private ParticleSystem electricParticles;

    public bool switchingObjs;
    private bool inTrigger;

    private void Start()
    {
        if (gameObject.tag == "Box")
        {
            box = GetComponent<Box>();
        }

        if (gameObject.tag == "Sphere")
        {
            sphere = GetComponent<Sphere>();
        }

        handManager = playerHand.GetComponent<HandManager>();
        obj = switchPlane.GetComponent<ObjectOnPlane>();

        pInRange = playerTrigger.GetComponent<PlayerInRange>();

        switchingObjs = false;
        inTrigger = false;
    }

    private void Update()
    {
        swapObject = obj.onPlatform;

        if (obj.onPlatform != gameObject)
        {
            unswappedObject = gameObject;
        }

        isSwitching();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (gameObject.tag == "Box")
        {
            if (collider.tag == "Switcher" && !box.inHand && swapObject != gameObject)
            {
                Invoke("TriggerEnter", 1f);
            }
        }


        if (gameObject.tag == "Sphere")
        {
            if (collider.tag == "Switcher" && !sphere.inHand && swapObject != gameObject)
            {
                Invoke("TriggerEnter", 1f);
            }
        }

        if (collider.tag == "SwitchPlatform")
        {
            obj.inSwitcher = obj.onPlatform;
            obj.onPlatform = gameObject;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        inTrigger = false;
    }

    private void TriggerEnter()
    {
        inTrigger = true;
        obj.inSwitcher = gameObject;
        insideSwitcher = gameObject.transform.position;
        outsideSwitcher = swapObject.transform.position;

        if (!pInRange.anim.GetBool("playerInRange"))
        {
            Invoke("PlaySounds", 3f);
        }
    }

    private void PlaySounds()
    {
        jeremyLine.Play();
        switchNoise.PlayDelayed(0.8f);
        electricParticles.Play();

        Invoke("SwitchObjects", 3f);
    }

    private void SwitchObjects()
    {
        gameObject.transform.position = outsideSwitcher;
        swapObject.transform.position = insideSwitcher;
    }

    private void isSwitching()
    {
        if (jeremyLine.isPlaying || switchNoise.isPlaying)
        {
            doorAnim.SetBool("playerInRange", false);
            switchingObjs = true;
        }
        else
        {
            switchingObjs = false;
        }
    }
}
