using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBall : MonoBehaviour
{
    
    public GameController GC;

    private GameController.BallState state;

    void Start()
    {
       
    }

    void Update()
    {

    }

    private void OnBecameInvisible()
    {
        GC.GameOver();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        GC.BallEnteredHole(collision.collider);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //to avoid bugs in case the ball bounces around the hole
        if (collision == GC.stick)
        {
            GC.floor.isTrigger = false;
            GC.stick.isTrigger = false;
        }
    }
}
