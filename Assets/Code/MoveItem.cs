using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveItem : MonoBehaviour
{
    public float DurationRay;
    public float Offset;
    public Transform PointRay;
    public GameObject Body;
    public float Speed;
    public bool Move = true;

    private void Update()
    {
        if (Move )
        {
            Body.transform.position += Vector3.down * Speed * Time.deltaTime;
        }
     //   var position = PointRay.position;
//
     //   RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, DurationRay);
     //   Debug.DrawLine(position, Vector2.down , Color.red);
     //   if (hit.collider)
     //   {
     //       Debug.Log("Collision");
     //   }
      
         
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Item>() || other.gameObject.CompareTag("limiter"))
        {
            Move = false;
            Debug.Log("Stop");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Start");
        Move = true;
    }

  // private void OnCollisionStay2D(Collision2D other)
  // {
  //     if (other.gameObject.GetComponent<Item>())
  //     {
  //         Move = false;
  //         Debug.Log("Stop");
  //     }
  //     else
  //     {
  //         
  //         Debug.Log("Start");
  //         Move = true;
  //     }
  // }

    

     
}