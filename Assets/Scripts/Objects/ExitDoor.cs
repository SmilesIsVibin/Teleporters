using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExitDoor : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    public List<PlayerController> players = new List<PlayerController>();
    public bool gameWon;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        gameWon = false;
    }

    private void Update()
    {
        if (!gameWon)
        {
            if (players.Count >= PhotonNetwork.PlayerList.Length)
            {
                pv.RPC(nameof(WaitForLevelWin), RpcTarget.AllBufferedViaServer);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (pv.IsMine)
        {
            if (!gameWon)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    string _name = collision.gameObject.name;
                    pv.RPC(nameof(RPC_PlayerCheck), RpcTarget.AllBufferedViaServer, _name);
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(pv.IsMine)
        {
            if (!gameWon)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    string _name = collision.gameObject.name;
                    pv.RPC(nameof(RPC_PlayerRemove), RpcTarget.AllBufferedViaServer, _name);
                }
            }
        }
    }

    [PunRPC]
    public void RPC_PlayerCheck(string _name)
    {
        GameObject _go = GameObject.Find(_name);
        players.Add(_go.GetComponent<PlayerController>());
    }

    [PunRPC]
    public void RPC_PlayerRemove(string _name)
    {
        GameObject _go = GameObject.Find(_name);
        players.Remove(_go.GetComponent<PlayerController>());
    }


    [PunRPC]
    public void WaitForLevelWin()
    {
        StartCoroutine(nameof(LevelWin));
    }

    public IEnumerator LevelWin()
    {
        if(players.Count >= PhotonNetwork.PlayerList.Length)
        {
            gameWon = true;
            yield return new WaitForSeconds(3f);
            GameManager.Instance.LevelWon();
        }
        else
        {
            gameWon = false;
            Debug.Log("A Player is still out the exit");
        }
    }
}
