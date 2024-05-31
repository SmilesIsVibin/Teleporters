using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovingPlatform : MonoBehaviourPunCallbacks
{
    public Transform pointA;
    public Transform pointB;
    public float speed;
    public bool isActivated = false;
    private Transform destination;

    PhotonView platformPV;

    void Start()
    {
        transform.position = pointA.position;
        platformPV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (isActivated)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination.position, speed * Time.deltaTime);
            if(Vector2.Distance(transform.position, destination.position) < 0.02f)
            {
                isActivated = false;
            }
        }
    }

    public void ActivatePlatform()
    {
        if (Vector2.Distance(transform.position, pointA.position) < 0.02f)
        {
            destination = pointB;
        }
        else if (Vector2.Distance(transform.position, pointB.position) < 0.02f)
        {
            destination = pointA;
        }
        isActivated = true;
    }

    public void RaiseObstacle()
    {
        destination = pointA;
        isActivated = true;
    }

    public void LowerObstacle()
    {
        destination = pointB;
        isActivated = true;
    }
}
