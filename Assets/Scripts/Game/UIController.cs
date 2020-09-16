using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    /// <summary>
    /// Link to fade manager
    /// </summary>
    [SerializeField]
    private FadeManager FadeManager = null;

    /// <summary>
    /// Link to score text object
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI ScoreText = null;

    /// <summary>
    /// Link to best score text object
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI BestScoreText = null;

    /// <summary>
    /// Link to best info text object
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI InfoText = null;

    public void Start () {
        FadeManager.Process (Color.clear, Color.black, Config.FadeDurationFlash, () => {
            FadeManager.Process (Color.black, Color.clear, Config.FadeDuration);
        });
    }

    /// <summary>
    /// Go to menu
    /// </summary>
    public void BackToMenu () {
        FadeManager.Process (Color.clear, Color.black, Config.FadeDuration, () => {
            SceneManager.LoadScene (Config.MenuScene);
        });
    }

    /// <summary>
    /// Set current score
    /// </summary>
    /// <param name="score">Score value</param>
    public void SetScore (int score) {
        ScoreText.text = score.ToString ();
    }

    /// <summary>
    /// Set current score for two players
    /// </summary>
    /// <param name="scoreOne">Player one score</param>
    /// <param name="scoreTwo">Player two score</param>
    public void SetTwoScores (int scoreOne, int scoreTwo) {
        ScoreText.text = string.Format ("{0}:{1}", scoreOne, scoreTwo);
    }

    /// <summary>
    /// Set best score value
    /// If negative – clear text
    /// </summary>
    /// <param name="score">Score value</param>
    public void SetBestScore (int score) {
        BestScoreText.text = (score < 0) ? "" : string.Format ("Best: {0}", score);
    }

    /// <summary>
    /// Set info text in the middle
    /// </summary>
    public void SetInfoText (string text) {
        InfoText.text = text;
    }
}