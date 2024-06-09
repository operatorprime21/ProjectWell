using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorEvents : MonoBehaviour
{
    public void MonitorAnimMovingFinished()
    {
        GameObject.Find("WellCameraManager").GetComponent<CameraControls>().StopMoving();
    }


}
