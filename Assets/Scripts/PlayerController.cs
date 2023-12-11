using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float PaddleSpeed;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        float motion = Input.GetAxisRaw("Vertical");

        if (motion > 0)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * PaddleSpeed;
        }
        else if (motion < 0)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.down * PaddleSpeed;
        }
        else
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody2D>().freezeRotation = false;
            GetComponent<Rigidbody2D>().AddTorque(50);
            GetComponent<Rigidbody2D>().rotation = (12f);
        }
        else if (GetComponent<Rigidbody2D>().rotation % 360 > 10)
        {
            GetComponent<Transform>().rotation = Quaternion.identity;
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
            GetComponent<Rigidbody2D>().freezeRotation = true;
        }
    
    
    }
    
    private void FixedUpdate()
    {
        
    }
}
