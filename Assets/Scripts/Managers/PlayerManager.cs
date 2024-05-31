using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    public PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }
    void CreateController()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Instantiated Player Controller 1");
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player1"), GameManager.Instance.playerRespawn1.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Instantiated Player Controller 2");
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player2"), GameManager.Instance.playerRespawn2.transform.position, Quaternion.identity);
        }
    }
}
