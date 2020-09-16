using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using NiceJson;
using UnityEngine;

/// <summary>
/// Game types
/// </summary>
public enum GameType {

    SINGLE = 0,

    ONLINE = 1
}

public class GameController : MonoBehaviour {

    /// <summary>
    /// Online command type
    /// </summary>
    enum OnlineCommand {

        INIT = 0,

        READY = 1,

        MOVE = 2,

        START = 3

    }

    /// <summary>
    /// Key for best score
    /// </summary>
    const string BEST_SCORE_KEY = "best";

    /// <summary>
    /// Key for game type
    /// </summary>
    const string GAME_TYPE_KEY = "game";

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
    /// Duration to spin ball 360
    /// </summary>
    const float SPIN_DURATION = 0.5f;

    /// <summary>
    /// Vector for spin ball
    /// </summary>
    private Vector3 SPIN_VECTOR = new Vector3 (0f, 0f, 360f);

    /// <summary>
    /// Link to player controllers
    /// </summary>
    [SerializeField]
    private PlayerController[] Players = null;

    /// <summary>
    /// Link to UIController
    /// </summary>
    [SerializeField]
    private UIController UIController = null;

    /// <summary>
    /// Link to Connector
    /// </summary>
    [SerializeField]
    private Connector Connector = null;

    /// <summary>
    /// Link to Ball object
    /// </summary>
    [SerializeField]
    private GameObject BallObject = null;

    /// <summary>
    /// Link to player one racket transform
    /// </summary>
    [SerializeField]
    private Transform PlayerOneRacket = null;

    /// <summary>
    /// Link to player two racket transform
    /// </summary>
    [SerializeField]
    private Transform PlayerTwoRacket = null;

    /// <summary>
    /// Ball 2d body
    /// </summary>
    private Rigidbody2D _ballBody = null;

    /// <summary>
    /// SpriteRenderer ball
    /// </summary>
    private SpriteRenderer _ballSprite = null;

    /// <summary>
    /// Ball transform for scaling and position
    /// </summary>
    private Transform _ballTransform = null;

    /// <summary>
    /// Ball color cache
    /// </summary>
    private Color _ballColor = Color.white;

    /// <summary>
    /// Current ball direction
    /// </summary>
    private Vector2 _ballDirection = Vector2.zero;

    /// <summary>
    /// Current ball speed
    /// </summary>
    private float _ballSpeed = 100f;

    /// <summary>
    /// Current ball scale
    /// </summary>
    private float _ballScale = 0.1f;

    /// <summary>
    /// Current best score
    /// </summary>
    private int _bestScore = -1;

    /// <summary>
    /// Current game score
    /// </summary>
    private int _currentScore = 0;

    /// <summary>
    /// Scores storage
    /// </summary>
    private int[] _scores = null;

    /// <summary>
    /// Current game type sigle or two players
    /// </summary>
    private GameType _currentType = GameType.SINGLE;

    /// <summary>
    /// Cache for camera
    /// </summary>
    private Camera _camera;

    /// <summary>
    /// Flag to check game start
    /// </summary>
    private bool _isGameStarted = false;

    /// <summary>
    /// Flag to check server connected
    /// </summary>
    private bool _isConnected = false;

    /// <summary>
    /// Player index for online game
    /// </summary>
    private int _playerIndex = -1;

    /// <summary>
    /// Flag to check opponent connected
    /// </summary>
    private bool _isOpponentReady = false;

    void Start () {
        _camera = Camera.main;
        _ballBody = BallObject.GetComponent<Rigidbody2D> ();
        _ballSprite = BallObject.GetComponent<SpriteRenderer> ();
        _ballTransform = BallObject.transform;
        LeanTouch.OnFingerUpdate += OnFingerUpdate;
        LoadSettings ();
        ResetGame ();
        if (_currentType == GameType.ONLINE) {
            UIController.SetInfoText ("Connecting...");
            Connector.Connect (() => {
                _isConnected = true;
                UIController.SetInfoText ("Waiting for opponent...");
            }, () => {
                UIController.SetInfoText ("Can't connect to server :(");
            }, OnDataReceived);
        }
    }

    /// <summary>
    /// Unsubscribe event
    /// </summary>
    void OnDisable () {
        LeanTouch.OnFingerUpdate -= OnFingerUpdate;
    }

    /// <summary>
    /// Callback when some data received from socket
    /// </summary>
    /// <param name="data">JSON string</param>
    void OnDataReceived (string json) {
        try {
            JsonObject data = (JsonObject) JsonNode.ParseJsonString (json);
            OnlineCommand cmd = (OnlineCommand) (int) data["cmd"];
            switch (cmd) {
                case OnlineCommand.INIT:
                    if (_playerIndex < 0) {
                        _playerIndex = (data["c"] > 1) ? (data["c"] - 1) : 0;
                        if (_playerIndex > 0) {
                            _isOpponentReady = true;
                            Invoke ("ClearInfoText", 1f);
                        }
                        SendReady ();
                    }
                    break;
                case OnlineCommand.READY:
                    int player = data["p"];
                    if (player != _playerIndex) {
                        _isOpponentReady = true;
                        UIController.SetInfoText ("Opponent ready to play");
                        Invoke ("ClearInfoText", 1f);
                        if (_playerIndex == 0) {
                            SendStart ();
                        }
                    }
                    break;
                case OnlineCommand.MOVE:
                    int index = data["p"];
                    if (index != _playerIndex) {
                        Players[index].Move (data["x"]);
                    }
                    break;
                case OnlineCommand.START:
                    _ballDirection = new Vector2 (data["x"], data["y"]);
                    _ballSpeed = data["s"];
                    _ballScale = data["z"];
                    SetBallSize ();
                    StartGame ();
                    break;
                default:
                    break;
            }
        } catch (Exception e) {
#if DEBUG
            Debug.LogErrorFormat ("Fail to parse JSON: {0}", json);
#endif
        }
    }

    /// <summary>
    /// Send ready command
    /// </summary>
    void SendReady () {
        JsonObject data = new JsonObject ();
        data["p"] = _playerIndex;
        data["cmd"] = (int) OnlineCommand.READY;
        Connector.Send (data.ToJsonString ());
    }

    /// <summary>
    /// Send start command
    /// </summary>
    void SendStart () {
        JsonObject data = new JsonObject ();
        data["cmd"] = (int) OnlineCommand.START;
        data["x"] = _ballDirection.x;
        data["y"] = _ballDirection.y;
        data["s"] = _ballSpeed;
        data["z"] = _ballScale;
        Connector.Send (data.ToJsonString ());
    }

    /// <summary>
    /// Clear middle info text
    /// </summary>
    void ClearInfoText () {
        UIController.SetInfoText ("");
    }

    /// <summary>
    /// Callback on finger move
    /// </summary>
    private void OnFingerUpdate (LeanFinger finger) {
        Vector3 position = finger.GetWorldPosition (_camera.transform.position.y, _camera);
        if (_currentType == GameType.SINGLE) {
            Players[0].Move (position.x);
            Players[1].Move (position.x);
        } else if (_isOpponentReady) {
            Players[_playerIndex].Move (position.x);
            if (_isConnected) {
                JsonObject data = new JsonObject ();
                data["p"] = _playerIndex;
                data["x"] = position.x;
                data["cmd"] = (int) OnlineCommand.MOVE;
                Connector.Send (data.ToJsonString ());
            }
        }
        if (!_isGameStarted) {
            switch (_currentType) {
                case GameType.SINGLE:
                    StartGame ();
                    break;
                case GameType.ONLINE:
                    if (_playerIndex == 0 && _isOpponentReady) {
                        SendStart ();
                    }
                    break;
                default:
                    break;
            }            
        }
    }

    /// <summary>
    /// Add point to player
    /// </summary>
    /// <param name="index">Player side index</param>
    public void AddPoint (int index) {
        if (_currentType == GameType.SINGLE) {
            _currentScore++;
            UIController.SetScore (_currentScore);
        }
    }

    /// <summary>
    /// Action when player/side loose
    /// </summary>
    /// <param name="index">Player side index</param>
    public void PlayerLoose (int index) {
#if DEBUG
        Debug.LogFormat ("Player/side {0} loose", index);
#endif
        if (_currentType == GameType.ONLINE) {
            if (index == 0) {
                _scores[1]++;
            } else {
                _scores[0]++;
            }
            UIController.SetTwoScores (_scores[0], _scores[1]);
        }
        _ballBody.Sleep ();
        _isGameStarted = false;
        ResetGame ();
    }

    /// <summary>
    /// Reset game and wait for start tap
    /// </summary>
    void ResetGame () {
        SaveScore ();
        _currentScore = 0;
        _ballDirection = new Vector2 (1f, UnityEngine.Random.Range (2f, -2f));
        _ballDirection.x *= (UnityEngine.Random.Range (0f, 2f) == 1f) ? -1f : 1f;
        _ballSpeed = UnityEngine.Random.Range (Config.MinBallSpeed, Config.MaxBallSpeed);
        _ballTransform.position = Vector2.zero;
        _ballScale = UnityEngine.Random.Range (Config.MinBallSize, Config.MaxBallSize);
        SetBallSize ();
        _ballTransform.DOKill ();
        if (_currentType == GameType.SINGLE) {
            UIController.SetScore (_currentScore);
        } else {
            UIController.SetTwoScores (_scores[0], _scores[1]);
        }
    }

    /// <summary>
    /// Set size for ball
    /// </summary>
    void SetBallSize () {
        _ballTransform.localScale = new Vector3 (_ballScale, _ballScale, _ballScale);
    }

    /// <summary>
    /// Run game
    /// </summary>
    void StartGame () {
        _isGameStarted = true;
        _ballBody.WakeUp ();
        _ballBody.AddForce (_ballDirection * _ballSpeed);
        _ballTransform.DORotate (SPIN_VECTOR, SPIN_DURATION, RotateMode.FastBeyond360).SetRelative ().SetLoops (-1).SetEase (Ease.Linear);
    }

    /// <summary>
    /// Load settings and apply
    /// </summary>
    void LoadSettings () {
        int type = PlayerPrefs.GetInt (GAME_TYPE_KEY, 0);
        _currentType = (GameType) type;
        if (_currentType == GameType.ONLINE) {
            _scores = new int[2] { 0, 0 };
        }
        _ballColor.r = PlayerPrefs.GetFloat (RED_KEY, 1f);
        _ballColor.g = PlayerPrefs.GetFloat (GREEN_KEY, 1f);
        _ballColor.b = PlayerPrefs.GetFloat (BLUE_KEY, 1f);
        _ballColor.a = 1f;
        _ballSprite.color = _ballColor;
        _bestScore = (_currentType == GameType.SINGLE) ? PlayerPrefs.GetInt (BEST_SCORE_KEY, -1) : -1;
        UIController.SetBestScore (_bestScore);
#if DEBUG
        Debug.LogFormat ("Game type: {0}", _currentType.ToString ());
#endif            
    }

    /// <summary>
    /// Check and save best score
    /// </summary>
    void SaveScore () {
        if (_currentScore > 0 && _currentScore > _bestScore && _currentType == GameType.SINGLE) {
#if DEBUG
            Debug.LogFormat ("New best score: {0}", _currentScore);
#endif            
            _bestScore = _currentScore;
            UIController.SetBestScore (_bestScore);
            PlayerPrefs.SetInt (BEST_SCORE_KEY, _bestScore);
            PlayerPrefs.Save ();
        }
    }

}