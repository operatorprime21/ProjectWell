using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBase : MonoBehaviour
{
    public int valueMax;
    public int valueMin;
    public int value;
    public float weight;
    public AudioClip a_collectClip;
    void Start()
    {
        value = Random.Range(valueMin, valueMax);
        this.gameObject.name = this.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "bait")
        {
            FishingRodMovement baitData = GameObject.Find("FishingRodManager").GetComponent<FishingRodMovement>();
            if (baitData.fishObject == null)
            {
                baitData.fishObject = this;
                baitData.PlayChime();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "bait")
        {
            FishingRodMovement baitData = GameObject.Find("FishingRodManager").GetComponent<FishingRodMovement>();
            if (baitData.fishObject == this)
            {
                baitData.fishObject = null;
            }
        }
    }

    public void CollectEvent(FishingRodMovement rod)
    {
        rod.a_collect.clip = a_collectClip;
        rod.PlayCollect();
        rod.manager.EventChangeCurrency(value);
        rod.manager.wellCamera.GetComponent<CameraControls>().FlashText("Obtained: " + this.gameObject.name + " (+"+value+")");
        rod.manager.EventHastenTree(weight);
    }
}
