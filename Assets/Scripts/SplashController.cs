using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour {

    /// <summary>
    /// Link to FadeManager
    /// </summary>
    [SerializeField]
    private FadeManager FadeManager = null;

    void Start () {
        Application.targetFrameRate = 60;
        FadeManager.Process (Color.clear, Color.black, Config.FadeDuration, () => {
            SceneManager.LoadScene (Config.MenuScene);
        });
    }

}