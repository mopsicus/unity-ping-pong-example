using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

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
    void OnCollisionEnter2D (Collision2D collision) {
        Controller.AddPoint (PlayerIndex);
    }

    /// <summary>
    /// Move racket to X position
    /// </summary>
    public void Move (float x) {
        Vector3 position = transform.position;
        position.x = x;
        transform.position = position;
    }

}