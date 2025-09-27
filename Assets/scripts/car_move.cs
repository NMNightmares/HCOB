using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_move : MonoBehaviour
{
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    
    private Vector2 movementInput;
    
    
    // Update is called once per frame
    void Update()
    {
        float horzontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        movementInput = new Vector2(horzontalInput, verticalInput).normalized;
    }

    void FixedUpdate()
    {
        Vector2 velocity = movementInput * speed;
        
        rb.velocity = velocity;
    }
}
