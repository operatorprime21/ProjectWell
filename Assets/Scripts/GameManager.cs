using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Main Mechanics")]
    [Tooltip("Camera object")] public GameObject wellCamera;
    [Tooltip("Fishing Rod object")] public GameObject fishingRod;
    [Space(10)]

    [Header("Animators")]
    public Animator monitorAnims;
    public Animator playerToolsAnims;
    public Animator camAnims;
    [Space(10)]

    [Header("UI Menu")]
    public GameObject startButton;
    public GameObject manualButton;
    public AudioSource a_button;
    public GameObject shopButton;
    public GameObject exitButton;
    public GameObject restartButton;
    public GameObject title;
    public AudioSource ambience;
    private bool isPlaying = true;
    public TMP_Text endText;
    public GameObject endCanvas;
    [Space(10)]

    [Header("Manual")]
    public GameObject manual;
    public List<GameObject> manualPage = new List<GameObject>();
    public int page = 0;
    public AudioSource a_page;
    [Space(10)]

    [Header("Shop")]
    public GameObject shop;
    [Space(10)]

    [Header("Tree")]
    public TreeBehavior tree;
    [Space(10)]

    [Header("Game state variables and UI")]
    public int currency;
    public int quota;
    public TMP_Text txtGoal;
    [Space(10)]

    [Header("Fish list and variables")]
    public List<FishBase> tier1Fish = new List<FishBase>();
    public List<FishBase> tier2Fish = new List<FishBase>();
    public List<FishBase> tier3Fish = new List<FishBase>();
    public List<FishBase> tier4Fish = new List<FishBase>();
    [Space(10)]
    [SerializeField] private float[] yT1;
    [SerializeField] private float[] yT2;
    [SerializeField] private float[] yT3;
    [SerializeField] private float[] yT4;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AmbienceLoop());
        currency = 0;
        a_page.Stop();
        a_button.Stop();
        txtGoal.text = currency.ToString() + "/" + quota.ToString();
        yT1 = new float[] { 5, 100 };
        yT2 = new float[] { 80, 160 };
        yT3 = new float[] { 120, 180 };
        yT3 = new float[] { 100, 200 };
        //Vector3 randCoords = Random.insideUnitCircle.normalized * 1.2f;
        //Debug.Log(randCoords);
        SetSpawnPoints();
    }

    void SpawnFish(float yMin, float yMax, FishBase fish)
    {
        float randY = Random.Range(yMin, yMax);
        Vector3 randCoords = Random.insideUnitCircle.normalized * 1.2f;
        Vector3 spawnPoint = new Vector3(randCoords.x, -randY, randCoords.y);
        Vector3 dir = spawnPoint - new Vector3(0, 0, 0);
        float angle = Vector3.Angle(dir, new Vector3(0, 0, 1));
        Vector3 newAngle = new Vector3(fish.transform.rotation.x, fish.transform.rotation.y - angle, fish.transform.rotation.z);
        var item = Instantiate(fish, spawnPoint, Quaternion.Euler(newAngle));
        item.name = fish.name;
    }

    void SetSpawnPoints()
    {
        int t1 = Random.Range(19, 25);
        int t2 = Random.Range(16, 22);
        int t3 = Random.Range(12, 18);
        int t4 = Random.Range(8, 15);
        for (int t = 1; t <= 3; t++)
        {
            if(t == 1)
            {
                for(int n = 0; n <= t1; n++)
                {
                    int r = Random.Range(0, tier1Fish.Count);
                    SpawnFish(yT1[0], yT1[1], tier1Fish[r]);
                }
            }

            if(t == 2)
            {
                for (int n = 0; n <= t2; n++)
                {
                    int r = Random.Range(0, tier2Fish.Count);
                    SpawnFish(yT2[0], yT2[1], tier2Fish[r]);
                }
            }

            if(t == 3)
            {
                for (int n = 0; n <= t3; n++)
                {
                    int r = Random.Range(0, tier3Fish.Count);
                    SpawnFish(yT3[0], yT3[1], tier3Fish[r]);
                }
            }

            if (t == 4)
            {
                for (int n = 0; n <= t4; n++)
                {
                    int r = Random.Range(0, tier4Fish.Count);
                    SpawnFish(yT4[0], yT4[1], tier4Fish[r]);
                }
            }
        }
    }

    public void GameStart()
    {
        a_button.Play();
        tree.MoveDistance(1f);
        fishingRod.SetActive(true);
        wellCamera.SetActive(true);
        monitorAnims.Play("monitor_entry");
        playerToolsAnims.Play("game_start");
        camAnims.Play("menu_entry");
        title.SetActive(false);
        startButton.SetActive(false);
        manualButton.SetActive(false);
        exitButton.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator AmbienceLoop()
    {
        ambience.Play();
        yield return new WaitForSeconds(ambience.clip.length);
        StartCoroutine(AmbienceLoop());
    }

    public void EventChangeCurrency(int add)
    {
        currency += add;
        txtGoal.text = currency.ToString() + "/" + quota.ToString();
        if (currency >= quota && isPlaying == true)
        {
            isPlaying = false;
            StartCoroutine(EventGameEnd("OBJECTIVE COMPLETE"));
        }
    }

    public void EventHastenTree(float d)
    {
        tree.StopCoroutine();
        tree.MoveDistance(d);
    }

    public void CheckTreeHeight()
    {
        if(tree.Height()>=-10 && isPlaying == true)
        {
            isPlaying = false;
            StartCoroutine(EventGameEnd("OBJECTIVE FAILED"));
        }
    }

    public IEnumerator EventGameEnd(string end)
    {
        wellCamera.SetActive(false);
        fishingRod.SetActive(false);
        yield return new WaitForSeconds(2f);
        endText.text = end;
        endCanvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        Restart();
    }


    //IEnumerator Timer()
    //{
    //    if(time > 0)
    //    {
    //        yield return new WaitForSeconds(1f);
    //        time--;
    //        StartCoroutine(Timer());
    //        TimerText();
    //    }
    //    else
    //    {
    //        EventLose();
    //    }
    //}

    //public void TimerText()
    //{
    //    int min = Mathf.FloorToInt(time / 60);
    //    int remainSec = time - min * 60;
    //    if(remainSec < 10)
    //    {
    //        txtTime.text = min.ToString() + ":0" + remainSec.ToString();
    //    }
    //    else
    //    txtTime.text = min.ToString()+":"+remainSec.ToString();
    //}

    public void ToggleManual()
    {
        a_page.Play();
        if (manual.activeInHierarchy)
        {
            ExitManual();
        }
        else manual.SetActive(true);
    }

    public void ManualNextPage()
    {
        a_page.Play();
        if(page<manualPage.Count-1)
        {
            manualPage[page].SetActive(false);
            page++;
            manualPage[page].SetActive(true);
        }
    }

    public void ManualPrevPage()
    {
        a_page.Play();
        if (page!=0)
        {
            manualPage[page].SetActive(false);
            page--;
            manualPage[page].SetActive(true);
        }
    }

    public void ExitManual()
    {
        manual.SetActive(false);
        page = 0;
        foreach(GameObject page in manualPage)
        {
            if (page != manualPage[0])
            {
                page.SetActive(false);
            }
            else page.SetActive(true);
        }
    }

    public void ToggleShop()
    {
        a_button.Play();
        if (shop.activeInHierarchy)
        {
            shop.SetActive(false);
        }
        else shop.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

}
