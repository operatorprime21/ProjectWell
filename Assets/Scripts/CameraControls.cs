using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraControls : MonoBehaviour
{
    [Header("Control variables")]
    [Tooltip("Camera object")] public GameObject wellCamera;
    [Tooltip("how fast the camera drops")] public float vertMove;
    [Tooltip("how fast the camera rotates")] public float horRotate;
    [Space(10)]

    [Header("Movement debug variables. All private")]
    [Tooltip("allows the camera to turn with the last pressed key")] [SerializeField] private string turning = "none";
    [Tooltip("allows the camera to turn with the last held key")] [SerializeField] private string overloadTurn = "none";
    [Tooltip("allows the camera to move with the last pressed key")] [SerializeField] private string moving = "none";
    [Tooltip("allows the camera to move with the last held key")] [SerializeField] private string overloadMove = "none"; 
    [Space(10)]

    [Header("Position and Event tracking")]
    [Tooltip("tracks camera depth. Private")] [SerializeField] private float depth;
    [Tooltip("tracks camera depth. Private")] [SerializeField] public float maxDepth;
    [Tooltip("tracks image capturing and flash cooldown. Private")] [SerializeField] private bool flashed = false; 
    private Vector3 start;
    [Space(10)]

    [Header("Monitor related")]
    [Tooltip("indicator of camera's rotation")] public GameObject dirIndicator;
    [Tooltip("Game manager")] public GameManager manager;
    [Tooltip("monitor object")] public GameObject monitor;
    [Tooltip("checks zooming state. Private")] [SerializeField] private bool zoomedIn = false;
    [Tooltip("camera depth indicator on the monitor")] public TMP_Text textDepth;
    [Tooltip("flash cooldown indicator on the monitor")] public TMP_Text textFlash;
    [Tooltip("cooldown speed")] public float flashCd;
    [Tooltip("flash cooldown slider")] public Slider rechargeSlider;
    [Tooltip("time for slider")] private float startTime;
    [Tooltip("prevent monitor moving")] private bool isMonitorMoving = false;
    [Tooltip("flash lights")] public Animator flash;

    [Space(10)]
    [Header("Audio Sources")]
    [Tooltip("Gear turning")] public AudioSource gear;
    [Tooltip("Flash")] public AudioSource flashS;
    // Start is called before the first frame update
    void Start()
    {
        depth = 0f;
        start = wellCamera.transform.position;
        gear.Stop();
        flashS.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();

        CameraCapture();

        MonitorViewing();
    }

    private void CameraMovement()
    {
        VerticalMove();
        HorizontalRotate();
        
        Turn();
        
        Move();
        if (depth >= 0f)
        {
            wellCamera.transform.position = start;
            depth = 0f;
        }
        else if (depth <= -maxDepth)
        {
            wellCamera.transform.position = new Vector3(wellCamera.transform.position.x, -maxDepth, wellCamera.transform.position.z);
            depth = -maxDepth;
            textFlash.text = "maximum depth reached";
            textFlash.GetComponent<Animator>().Play("text_flash");
        }
        textDepth.text = Mathf.RoundToInt(-depth).ToString()+"m";
    }

    private void VerticalMove()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            moving = "upward";
            if(overloadMove == "none")
            {
                overloadMove = "upward";
            }
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            if(overloadMove == "none")
            {
                moving = "none";
            }
            else if (overloadMove == "upward")
            {
                if (moving == "upward")
                {
                    moving = "none";
                    overloadMove = "none";
                }
                else
                {
                    overloadMove = "downward";
                }
            }
            else
            {
                moving = overloadMove;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            moving = "downward";
            if (overloadMove == "none")
            {
                overloadMove = "downward";
            }
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if(overloadMove == "none")
            {
                moving = "none";
            }
            else if(overloadMove == "downward")
            {
                if(moving == "downward")
                {
                    moving = "none";
                    overloadMove = "none";
                }
                else
                {
                    overloadMove = "upward";
                }
            }
            else
            {
                moving = overloadMove;
            }
        }
    }

    private void HorizontalRotate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            turning = "left";
            if (overloadTurn == "none")
            {
                overloadTurn = "left";
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (overloadTurn == "none")
            {
                turning = "none";
            }
            else if (overloadTurn == "left")
            {
                if (turning == "left")
                {
                    turning = "none";
                    overloadTurn = "none";
                }
                else
                {
                    overloadTurn = "right";
                }
            }
            else
            {
                turning = overloadTurn;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            turning = "right";
            if (overloadTurn == "none")
            {
                overloadTurn = "right";
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (overloadTurn == "none")
            {
                turning = "none";
            }
            else if (overloadTurn == "right")
            {
                if (turning == "right")
                {
                    turning = "none";
                    overloadTurn = "none";
                }
                else
                {
                    overloadTurn = "left";
                }
            }
            else
            {
                turning = overloadTurn;
            }
        }
    }

    public void Move()
    {
        if (moving == "upward")
        {
            wellCamera.transform.position += wellCamera.transform.up * vertMove * Time.deltaTime;
            depth += vertMove*Time.deltaTime;
            if (!gear.isPlaying)
            {
                gear.Play();
            }
        }
        if (moving == "downward")
        {
            wellCamera.transform.position -= wellCamera.transform.up * vertMove * Time.deltaTime;
            depth -= vertMove * Time.deltaTime;
            if (!gear.isPlaying)
            {
                gear.Play();
            }
        }

        if(moving == "none" && turning == "none")
        {
            if (gear.isPlaying)
            {
                gear.Stop();
            }
        }
        
    }

    public void Turn()
    {
        dirIndicator.transform.rotation = wellCamera.transform.rotation;
        switch (turning)
        {
            case "left":
                wellCamera.transform.Rotate(0f, -Input.GetAxis("Horizontal") * -horRotate * Time.deltaTime, 0f);
                if (!gear.isPlaying)
                {
                    gear.Play();
                }
                break;
            case "right":
                wellCamera.transform.Rotate(0f, Input.GetAxis("Horizontal") * horRotate * Time.deltaTime, 0f) ;
                if (!gear.isPlaying)
                {
                    gear.Play();
                }
                break;
            case "none":
                if (gear.isPlaying && moving == "none")
                {
                    gear.Stop();
                }
                break;
        }
    }

    private void CameraCapture()
    {
        if (flashed)
        {
            float progress = +(Time.time - startTime) * flashCd;
            rechargeSlider.value = Mathf.Lerp(0f, 1f, progress);
            if (progress >= 1f)
            {
                flashed = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashed == false)
            {
                Snap();
            }
            else
            {
                FlashText("recharging");
            }

        }
    }

    public void FlashText(string text)
    {
        textFlash.text = text;
        textFlash.GetComponent<Animator>().Play("text_flash");
    }

    public void Snap()
    {
        flashed = true;
        FlashLight();
        flashS.Play();
        //textFlash.text = "Flas";
        startTime = Time.time;
    }
    void FlashLight()
    {
        flash.Play("flash");
    }

    private void MonitorViewing()
    {
        if (Input.GetKeyDown(KeyCode.E)&& isMonitorMoving == false)
        {
            isMonitorMoving = true;
            if (zoomedIn == false)
            {
                monitor.GetComponent<Animator>().Play("monitor_zoom_in");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                manager.manualButton.SetActive(true);
                manager.shopButton.SetActive(true);
                manager.restartButton.SetActive(true);
                manager.fishingRod.SetActive(false);
                zoomedIn = true;
            }
            else
            {
                monitor.GetComponent<Animator>().Play("monitor_zoom_out");
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                manager.manualButton.SetActive(false);
                manager.shopButton.SetActive(false);
                manager.restartButton.SetActive(false);
                manager.fishingRod.SetActive(true);
                zoomedIn = false;
            }
        }
    }
    public void StopMoving()
    {
        isMonitorMoving = false;
    }

}
