using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    /// <summary>
    /// Name of game scene to load and play
    /// </summary>
    const string GAME_SCENE = "Game";

    /// <summary>
    /// Name of settings screen
    /// </summary>
    const string SETTINGS_SCREEN = "Settings";

    /// <summary>
    /// Name of first main screen
    /// </summary>
    const string MAIN_SCREEN = "Main";    

    /// <summary>
    /// Link to FadeManager
    /// </summary>
    [SerializeField]
    private FadeManager FadeManager = null;

    /// <summary>
    /// Link to ScreensManager for control inmenu screens
    /// </summary>
    [SerializeField]
    private ScreensManager ScreensManager = null;

    void Start () {
        FadeManager.Process (Color.clear, Color.black, Config.FadeDurationFlash, () => {
            FadeManager.Process (Color.black, Color.clear, Config.FadeDuration);
            ScreensManager.Show (MAIN_SCREEN);
        });
    }

    /// <summary>
    /// Begin play
    /// </summary>
    public void Play () {
        FadeManager.Process (Color.clear, Color.black, Config.FadeDuration, () => {
            SceneManager.LoadScene (GAME_SCENE);
        });
    }

    /// <summary>
    /// Open Settings screen
    /// </summary>
    public void OpenSettings () {
        ScreensManager.Show (SETTINGS_SCREEN);
    }

    // public void OpenCredits () {
    //     ScreensManager.Open ("Credits");
    // }

}