using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GoalCount : MonoBehaviour
{
    [SerializeField] GameObject Ball;
    [SerializeField] int points;
    [SerializeField] GameObject Score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        points++;
        Score.GetComponent<TMP_Text>().text = points.ToString();
        Ball.GetComponent<Transform>().position = Vector3.zero;
    }

}
