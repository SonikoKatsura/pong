using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUController : MonoBehaviour
{

    [SerializeField] float PaddleSpeed;
    [SerializeField] GameObject Ball;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        if (Ball != null) { 
        float PaddlePosition = gameObject.transform.position.y;
        float BallPosition = Ball.transform.position.y;
        
        if (PaddlePosition < BallPosition)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * PaddleSpeed);
            
        }
        if (PaddlePosition > BallPosition)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.down * PaddleSpeed);

        }
        }
    }
}
