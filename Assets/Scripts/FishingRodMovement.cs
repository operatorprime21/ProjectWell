using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingRodMovement : MonoBehaviour
{
    [Header("Control variables")]
    [Tooltip("how fast the rod moves with mouse")] public float sensitivity;
    [Tooltip("how fast the bait drops")] [SerializeField] private float finalDropSpeed;
    [Tooltip("bait speed stat")] public float dropSpeed;
    [Space(10)]

    [Header("Rod movement debug")]
    [Tooltip("checks the distance from Well center")] [SerializeField] private float distanceFromCenter;
    [Tooltip("checks reeling, prevents horizontal movement while reeling")] [SerializeField] private bool reeling;
    [Tooltip("checks grappling, prevents reeling")] [SerializeField] private bool grappling;
    [Tooltip("checks height to prevent skyward reeling")] [SerializeField] private float baitHeight; 
    [Space(10)]

    [Header("Fishing Rod Objects")]
    [Tooltip("Horizontal moving component")] public GameObject rod;
    [Tooltip("depth reeling component")] public GameObject bait;
    [Tooltip("object to grab")] public FishBase fishObject;
    [Tooltip("game manager")] public GameManager manager;
    [Space(10)]

    [Header("UI")]
    [Tooltip("bait's height indicator")] public TMP_Text textHeight;
    [Space(10)]

    [Header("Sounds")]
    [Tooltip("Reeling")] public AudioSource a_reeling;
    [Tooltip("Chime")] public AudioSource a_chime;
    [Tooltip("Collect")] public AudioSource a_collect;

    // Start is called before the first frame update
    void Start()
    {
        baitHeight = bait.transform.position.y;
        a_chime.Stop();
        a_reeling.Stop();
        a_collect.Stop();
    }

    void Awake()
    {
        a_chime.Stop();
        a_reeling.Stop();
        a_collect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        textHeight.text = Mathf.RoundToInt(-bait.transform.position.y).ToString()+"m";
        if (reeling == true)
        {
            bait.transform.position += bait.transform.up * finalDropSpeed * Time.deltaTime;
        }
        else
        {
            RodMovement();
        }
        if(grappling == false)
        {
            BaitMovement();
        }    
        
        if (bait.transform.position.y >= baitHeight)
        {
            bait.transform.position = new Vector3(bait.transform.position.x, baitHeight, bait.transform.position.z);
        }

        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    grappling = true;
        //}
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    grappling = false;
        //}

    }

    private void BaitMovement()
    {
        if (Input.GetMouseButtonDown(0) && reeling == false)
        {
            reeling = true;
            finalDropSpeed = -dropSpeed;
            if (!a_reeling.isPlaying)
            {
                StartCoroutine(PlayReelingLoop());
            }
        }

        if (Input.GetMouseButtonUp(0) && reeling == true)
        {
            reeling = false;
            if (a_reeling.isPlaying)
            {
                a_reeling.Stop();
                StopAllCoroutines();
            }
        }

        if (Input.GetMouseButtonDown(1) && reeling == false)
        {
             reeling = true;
             finalDropSpeed = dropSpeed;
            if(!a_reeling.isPlaying)
            {
                StartCoroutine(PlayReelingLoop());
            }
        }

        if (Input.GetMouseButtonUp(1) && reeling == true)
        {
            reeling = false;
            if (a_reeling.isPlaying)
            {
                a_reeling.Stop();
                StopAllCoroutines();
            }
            if (fishObject != null)
            {
                DropBait();
            }
        }

        if (Input.GetMouseButton(1) && fishObject != null)
        {
            HookBait();
        }
    }

    private void RodMovement()
    {
        float inputX = Input.GetAxis("Mouse X") * sensitivity;
        float inputY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.position += new Vector3(inputX, 0, inputY);

        distanceFromCenter = Vector3.Distance(new Vector3(0, 0, 0), transform.position);
        if (distanceFromCenter <= 1.2f)
        {
            rod.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }
        else
        {
            Vector3 dir = transform.position - new Vector3(0, transform.position.y, 0);
            Vector3 point = new Vector3(0, 1, 0) + dir.normalized * 1.2f;
            rod.transform.position = point;
        }
    }

    void HookBait()
    {
        fishObject.gameObject.transform.position = bait.transform.position;
    }

    void DropBait()
    {
        if(bait.transform.position.y >= -5)
        {
            fishObject.CollectEvent(this);
        }
        fishObject.gameObject.SetActive(false);
        fishObject = null;
    }

    public void PlayChime()
    {
        a_chime.Play();
    }
    public void PlayCollect()
    {
        a_collect.Play();
    }
    IEnumerator PlayReelingLoop()
    {
        a_reeling.Play();
        yield return new WaitForSeconds(a_reeling.clip.length);
        StartCoroutine(PlayReelingLoop());
    }
}
