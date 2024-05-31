using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LeverInteraction : MonoBehaviourPunCallbacks
{
    public MovingPlatform platform;
    private bool isInteracting = false;
    private PhotonView itemPV;

    private void Start()
    {
        itemPV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (isInteracting)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                itemPV.RPC(nameof(RPC_ActivateLever), RpcTarget.AllBufferedViaServer);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInteracting = true;
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInteracting = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInteracting = false;
        }
    }

    [PunRPC]
    public void RPC_ActivateLever()
    {
        ActivateLever();
    }

    private void ActivateLever()
    {
        platform.ActivatePlatform();
    }

}
