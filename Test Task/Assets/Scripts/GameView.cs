using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform squarePrefab;
    [SerializeField] private Text distanceText;
    [SerializeField] private Text scoreText;

    private Transform _circle;
    private List<Transform> _squares;

    private void Start()
    {
        _circle = Instantiate(circlePrefab);
        
        _squares = new List<Transform>();
    }

    private void Update()
    {
        _circle.position = gm.CirclePosition;
        
        if (gm.SquareCount > _squares.Count)
            _squares.Add(Instantiate(squarePrefab));

        for (var i = 0; i < _squares.Count; i++)
        {
            if (_squares[i].gameObject.activeSelf != gm.SquareActive(i))
                _squares[i].gameObject.SetActive(gm.SquareActive(i));

            if (_squares[i].gameObject.activeSelf)
                _squares[i].position = gm.SquarePosition(i);
        }

        distanceText.text = "Distance: " + gm.CircleDistance;
        scoreText.text = "Score: " + gm.KillScore;
    }
}
