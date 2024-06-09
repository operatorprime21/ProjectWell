using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashScreenLoad : MonoBehaviour
{
    public string video;
    public VideoPlayer player;
    void Start()
    {
        StartCoroutine(Load());
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, video);
        player.url = path;
        player.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(1);
    }
}
