using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreenController : MonoBehaviour {

    /// <summary>
    /// Key for red color
    /// </summary>
    const string RED_KEY = "r";

    /// <summary>
    /// Key for green color
    /// </summary>
    const string GREEN_KEY = "g";

    /// <summary>
    /// Key for blue color
    /// </summary>
    const string BLUE_KEY = "b";

    /// <summary>
    /// Key for game type
    /// </summary>
    const string GAME_TYPE_KEY = "game";

    /// <summary>
    /// Link to screen view
    /// </summary>    
    private SettingsView _view = null;

    /// <summary>
    /// Current ball color
    /// </summary>
    private Color _ballColor = Color.white;

    /// <summary>
    /// Flag to check online game type
    /// </summary>
    private GameType _gameType = GameType.SINGLE;

    /// <summary>
    /// Icon for online enabled
    /// </summary>
    [SerializeField]
    private Sprite CheckedIcon = null;

    /// <summary>
    /// Icon for online disabled
    /// </summary>
    [SerializeField]
    private Sprite UncheckedIcon = null;

    void Awake () {
        _view = GetComponent<SettingsView> ();
    }

    /// <summary>
    /// Action on screen enabled
    /// </summary>
    void OnEnable () {
        LoadSettings ();
    }

    /// <summary>
    /// Action on screen hide
    /// </summary>
    void OnDisable () {
        SaveSettings ();
    }

    /// <summary>
    /// Callback on change red color
    /// </summary>
    public void OnRedChange (float value) {
        _ballColor.r = value;
        _view.SetBallColor (_ballColor);
    }

    /// <summary>
    /// Callback on change green color
    /// </summary>
    public void OnGreenChange (float value) {
        _ballColor.g = value;
        _view.SetBallColor (_ballColor);
    }

    /// <summary>
    /// Callback on change blue color
    /// </summary>
    public void OnBlueChange (float value) {
        _ballColor.b = value;
        _view.SetBallColor (_ballColor);
    }

    /// <summary>
    /// Load settings
    /// </summary>
    void LoadSettings () {
        _ballColor.r = PlayerPrefs.GetFloat (RED_KEY, 1f);
        _ballColor.g = PlayerPrefs.GetFloat (GREEN_KEY, 1f);
        _ballColor.b = PlayerPrefs.GetFloat (BLUE_KEY, 1f);
        _ballColor.a = 1f;
        _view.SetSliderValue (SliderColor.RED, _ballColor.r);
        _view.SetSliderValue (SliderColor.GREEN, _ballColor.g);
        _view.SetSliderValue (SliderColor.BLUE, _ballColor.b);
        int type = PlayerPrefs.GetInt (GAME_TYPE_KEY, 0);
        _gameType = (GameType) type;
        SetOnlineIcon ();
    }

    /// <summary>
    /// Save settings
    /// </summary>
    void SaveSettings () {
        PlayerPrefs.SetFloat (RED_KEY, _ballColor.r);
        PlayerPrefs.SetFloat (GREEN_KEY, _ballColor.g);
        PlayerPrefs.SetFloat (BLUE_KEY, _ballColor.b);
        PlayerPrefs.Save ();
    }

    /// <summary>
    /// Switch online state
    /// </summary>
    public void SwitchOnlineGameState () {
        _gameType = (_gameType == GameType.SINGLE) ? GameType.ONLINE : GameType.SINGLE;
        PlayerPrefs.SetInt (GAME_TYPE_KEY, (int) _gameType);
        PlayerPrefs.Save ();
        SetOnlineIcon ();
    }

    /// <summary>
    /// Set sprite for icon
    /// </summary>
    void SetOnlineIcon () {
        switch (_gameType) {
            case GameType.SINGLE:
                _view.SetOnlineIcon (UncheckedIcon);
                break;
            case GameType.ONLINE:
                _view.SetOnlineIcon (CheckedIcon);
                break;
            default:
                break;
        }
    }
}