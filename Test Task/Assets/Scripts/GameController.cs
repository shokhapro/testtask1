using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    private Camera _camera;

    private List<Vector2> _touchPath;
    [SerializeField] private float touchPathMinStep = 0.1f;

    private void Awake()
    {
        _camera = Camera.main;
        
        _touchPath = new List<Vector2>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchPath.Clear();
            
            _touchPath.Add(GetTouchPos());
        }
        else if (Input.GetMouseButton(0))
        {
            var lastPos = _touchPath[_touchPath.Count - 1];
            var pos = GetTouchPos();
            if (Vector2.Distance(lastPos, pos) < touchPathMinStep) return;
            
            _touchPath.Add(pos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            gm.OnTouch(_touchPath.ToArray());
        }
    }

    private Vector2 GetTouchPos()
    {
        var mpos = Input.mousePosition;
        var pos = _camera.ScreenToWorldPoint(mpos);

        return pos;
    }
}
