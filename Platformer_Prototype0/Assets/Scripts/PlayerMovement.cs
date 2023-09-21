using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{

    public CharacterController2D controller;

    public float runSpeed = 40f;

    float horizontalMove = 0f;

    // Update is called once per frame
    void Update(){
        Debug.Log(Input.GetAxisRaw("Horizontal"));
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

    }

    void FixedUpdate() { 
        //move the player
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
    
    }
}
