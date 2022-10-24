using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CircleCollider2D _circle;
    private Vector2[] _circleMovePath;
    private int _circleMovePathStep;
    [SerializeField] private float circleMoveSpeed = 1f;
    private float _circleMoveFullDistance;

    private List<BoxCollider2D> _squares;
    [SerializeField] private float squareSpawnDeltaTime = 3f;
    private float _squareNextSpawnTime;
    [SerializeField] private Vector2 squareSpawnRandPos = new Vector2(5f, 5f);
    [SerializeField] private int squareMaxCount = 5;
    private int _squareKillCount;

    private void Awake()
    {
        _circle = new GameObject().AddComponent<CircleCollider2D>();
        _circle.transform.SetParent(transform);
        
        if (!PlayerPrefs.HasKey("circle_distance")) PlayerPrefs.SetFloat("circle_distance", 0f);
        _circleMoveFullDistance = PlayerPrefs.GetFloat("circle_distance");

        _squares = new List<BoxCollider2D>();
        _squareNextSpawnTime = Time.time + squareSpawnDeltaTime;
        
        if (!PlayerPrefs.HasKey("score")) PlayerPrefs.SetInt("score", 0);
        _squareKillCount = PlayerPrefs.GetInt("score");
    }

    private void FixedUpdate()
    {
        CircleMoveUpdate();

        SquareSpawnUpdate();

        CollisionCheckUpdate();
    }

    private void CircleMoveUpdate()
    {
        if (_circleMovePath == null) return;
        
        var deltaMove = circleMoveSpeed * Time.fixedDeltaTime;

        Vector2 p0 = _circle.transform.position;
        Vector2 p1;
        
        while (true)
        {
            p1 = _circleMovePath[_circleMovePathStep];
            
            var dis = Vector2.Distance(p0, p1);

            if (deltaMove > dis)
            {
                deltaMove -= dis;
                p0 = p1;
                
                _circleMoveFullDistance += dis;
                
                if (_circleMovePathStep == _circleMovePath.Length - 1)
                {
                    deltaMove = 0f;
                    
                    _circleMovePath = null;
                    
                    break;
                }
                
                _circleMovePathStep++;
                
                continue;
            }

            break;
        }
        
        var dir = (p1 - p0).normalized;

        _circle.transform.position = p0 + deltaMove * dir;
        
        _circleMoveFullDistance += deltaMove;
    }
    
    private void SquareSpawnUpdate()
    {
        if (Time.time < _squareNextSpawnTime) return;
        _squareNextSpawnTime = Time.time + squareSpawnDeltaTime;
        
        int squareId = -1;

        for (var i = 0; i < _squares.Count; i++)
        {
            if (_squares[i].gameObject.activeSelf) continue;
            
            _squares[i].gameObject.SetActive(true);
            squareId = i;
            break;
        }

        if (squareId == -1 && _squares.Count < squareMaxCount)
        {
            _squares.Add(new GameObject().AddComponent<BoxCollider2D>());
            squareId = _squares.Count - 1;
            _squares[squareId].transform.SetParent(transform);
            
        }
        
        if (squareId == -1) return;
        
        _squares[squareId].transform.position =
            new Vector3(
                Random.Range(-squareSpawnRandPos.x, squareSpawnRandPos.x),
                Random.Range(-squareSpawnRandPos.y, squareSpawnRandPos.y), 0f);
    }
    
    private void CollisionCheckUpdate()
    {
        RaycastHit2D[] results = new RaycastHit2D[1];
        var hits = _circle.Cast(Vector2.up, results, 0f, false);
        if (hits == 0) return;
        
        results[0].collider.gameObject.SetActive(false);

        _squareKillCount++;
    }

    private void CircleMovePath(Vector2[] path)
    {
        _circleMovePath = path;
        
        _circleMovePathStep = 0;
    }
    
    private void CircleMoveStop()
    {
        _circleMovePath = null;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("circle_distance", _circleMoveFullDistance);
        PlayerPrefs.SetInt("score", _squareKillCount);
    }
    
    
    //controls
    public void OnTouch(Vector2[] path)
    {
        if (path.Length == 0) return;
            
        if (path.Length <= 1)
        {
            var hit = Physics2D.CircleCast(path[0], 0.1f, Vector2.up, 0f);
            var circleHit = hit.collider == _circle;

            if (circleHit)
            {
                CircleMoveStop();
                
                return;
            }
        }

        CircleMovePath(path);
    }
    
    
    //view
    public Vector2 CirclePosition => _circle.transform.position;
    public int SquareCount => _squares.Count;
    public bool SquareActive(int i) => _squares[i] && _squares[i].gameObject.activeSelf;
    public Vector2 SquarePosition(int i) => _squares[i].transform.position;
    public float CircleDistance => _circleMoveFullDistance;
    public int KillScore => _squareKillCount;
}
