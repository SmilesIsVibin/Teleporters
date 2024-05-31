using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBoxHead : MonoBehaviourPunCallbacks
{
    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.position.y > transform.position.y && collision.gameObject.CompareTag("Player"))
        {
            string _player = collision.gameObject.name;
            pv.RPC(nameof(RPC_SetParentTransform), RpcTarget.AllBufferedViaServer, _player);
            Debug.Log("Player 2 is on top of Player 1");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            string _player = collision.gameObject.name;
            pv.RPC(nameof(RPC_ClearParentTransform), RpcTarget.AllBufferedViaServer, _player);
            Debug.Log("Player 2 has left Player 1");
        }
    }

    [PunRPC]
    public void RPC_SetParentTransform(string playerName)
    {
        GameObject go = GameObject.Find(playerName);

        go.transform.SetParent(transform);
    }

    [PunRPC]
    public void RPC_ClearParentTransform(string playerName)
    {
        GameObject go = GameObject.Find(playerName);

        go.transform.SetParent(null);
    }
}
