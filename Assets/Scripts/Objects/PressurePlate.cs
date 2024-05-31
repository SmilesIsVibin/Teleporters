using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PressurePlate : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    public float massReq;
    public float collectedMass;
    public MovingPlatform obstacle;
    public bool isInteracting;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        pv.RPC(nameof(RPC_LowerObstacle), RpcTarget.AllBufferedViaServer);
        isInteracting = false;
    }

    private void Update()
    {
        if (collectedMass >= massReq)
        {
            pv.RPC(nameof(RPC_RaiseObstacle), RpcTarget.AllBufferedViaServer);
        }
        else if (collectedMass <= massReq)
        {
            pv.RPC(nameof(RPC_LowerObstacle), RpcTarget.AllBufferedViaServer);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (pv.IsMine)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                float _mass = collision.gameObject.GetComponent<Rigidbody2D>().mass;
                pv.RPC(nameof(RPC_AddMass), RpcTarget.AllBufferedViaServer, _mass);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (pv.IsMine)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                float _mass = collision.gameObject.GetComponent<Rigidbody2D>().mass;
                pv.RPC(nameof(RPC_RemoveMass), RpcTarget.AllBufferedViaServer, _mass);
            }
        }
    }

    [PunRPC]
    public void RPC_RaiseObstacle()
    {
        RaiseObstacle();
    }

    private void RaiseObstacle()
    {
        obstacle.RaiseObstacle();
    }


    [PunRPC]
    public void RPC_LowerObstacle()
    {
        LowerObstacle();
    }

    private void LowerObstacle()
    {
        obstacle.LowerObstacle();
    }

    [PunRPC]
    public void RPC_AddMass(float _mass)
    {
        collectedMass += _mass;
        isInteracting = true;
    }

    [PunRPC]
    public void RPC_RemoveMass(float _mass)
    {
        collectedMass -= _mass;
        isInteracting = false;
    }
}
