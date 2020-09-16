using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordersManager : MonoBehaviour {

    [SerializeField]
    private BoxCollider2D Top;

    [SerializeField]
    private BoxCollider2D Bottom;

    [SerializeField]
    private BoxCollider2D Left;

    [SerializeField]
    private BoxCollider2D Right;

    /// <summary>
    /// Resolution cache
    /// </summary>
    private Vector2 _resolution = Vector2.zero;

    /// <summary>
    /// Cache for camera
    /// </summary>
    private Camera _camera;

    /// <summary>
    /// Cache for screen size
    /// </summary>
    private Vector2 _screenSize = Vector2.zero;

    void Awake () {
        _camera = Camera.main;
    }

    /// <summary>
    /// Set sizes if screen size change
    /// </summary>
    void SetSizes () {
        _screenSize.y = _camera.orthographicSize * 2.0f;
        _screenSize.x = _screenSize.y * Screen.width / Screen.height;
        Vector2 vertical = new Vector2 (0.1f, _screenSize.y);
        Vector2 horizontal = new Vector2 (_screenSize.x, 0.1f);
        Left.size = vertical;
        Right.size = vertical;
        Top.size = horizontal;
        Bottom.size = horizontal;
    }

    /// <summary>
    /// Update if resolution changed
    /// </summary>
    void LateUpdate () {
        if (_resolution.x != Screen.width || _resolution.y != Screen.height) {
            _resolution.x = Screen.width;
            _resolution.y = Screen.height;
            SetSizes ();
        }
    }
}