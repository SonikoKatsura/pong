using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    [SerializeField] float BallSpeed;
    [SerializeField] int score1;
    [SerializeField] int score2;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject Score1;
    [SerializeField] GameObject Score2;
    [SerializeField] GameObject RStick;
    [SerializeField] GameObject LStick;

    void Start()
    {
        Spawn();
    }
   
    // Update is called once per frame
    void Update()
    {
        if (ball.transform.position.x > RStick.transform.position.x + 2.0) {
            score1++;
            Score1.GetComponent<TMP_Text>().text = score1.ToString();
            Spawn();
        }
        if (ball.transform.position.x < LStick.transform.position.x - 2.0)
        {
            score2++;
            Score2.GetComponent<TMP_Text>().text = score2.ToString();
            Spawn();
        }

    }
    
    private void Spawn()
    {
        ball.transform.position = Vector2.zero;
        float randomNumber = Random.Range(1, 4);
        rb = ball.GetComponent<Rigidbody2D>();
        Vector2 upleft = new Vector2(-1, 1);
        Vector2 upright = new Vector2(1, 1);
        Vector2 downleft = new Vector2(-1, -1);
        Vector2 downright = new Vector2(1, -1);

        switch (randomNumber)
        {
            case 1:
                rb.velocity = upleft * BallSpeed;
                break;
            case 2:
                rb.velocity = upright * BallSpeed;
                break;
            case 3:
                rb.velocity = downleft * BallSpeed;
                break;
            case 4:
                rb.velocity = downright * BallSpeed;
                break;

        }
    }
}
