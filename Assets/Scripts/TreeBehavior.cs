using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehavior : MonoBehaviour
{
    public float minCycle;
    public float maxCycle;
    public float cycle;

    public AudioSource r1;
    public AudioSource r2;
    public AudioSource r3;

    private bool r1Played = false;
    private bool r2Played = false;
    private bool r3Played = false;

    public GameManager manager;

    private void Start()
    {
        r1.Stop();
        r2.Stop();
        r3.Stop();
    }
    public IEnumerator MoveCycle()
    {
        cycle = Random.Range(minCycle, maxCycle);
        yield return new WaitForSeconds(cycle);
        MoveDistance(1f);
    }

    public void MoveDistance(float d)
    {
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + d, transform.position.z);
        transform.position = newPos;
        if(Height() >= -180 && Height() <= -160 && r1Played == false)
        {
            r1.Play();
            r1Played = true;
        }

        if (Height() >= -130 && Height() <= -110 && r2Played == false)
        {
            r2.Play();
            r2Played = true;
        }

        if (Height() >= -60 && Height() <= -40 && r3Played == false)
        {
            r3.Play();
            r3Played = true;
        }

        manager.CheckTreeHeight();
        StartCoroutine(MoveCycle());
    }

    public void StopCoroutine()
    {
        StopAllCoroutines();
    }

    public float Height()
    {
        return transform.position.y;
    }
}
