using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchBorder : MonoBehaviour {

    /// <summary>
    /// Link to GameController
    /// </summary>
    [SerializeField]
    private GameController Controller = null;

    /// <summary>
    /// Player side index
    /// </summary>
    public int PlayerIndex = 0;

    /// <summary>
    /// Callback on trigger enter
    /// </summary>
    void OnTriggerEnter2D (Collider2D collider) {
        Controller.PlayerLoose (PlayerIndex);
    }
}