using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Game Manager Info")]
    public static GameManager Instance;
    public int gameLevel;
    public PhotonView pv;
    [SerializeField] public Transform playerRespawn1;
    [SerializeField] public Transform playerRespawn2;
    public float timer;
    public bool timerActive;
    public TMP_Text timeText;

    [Header("Win Screen")]
    public GameObject winScreen;
    public GameObject winButtons;
    public TMP_Text winTimeText;

    [SerializeField] public PlayerController player1;
    [SerializeField] public PlayerController player2;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timerActive = true;
        winScreen.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC(nameof(RPC_FindPlayers), RpcTarget.AllBufferedViaServer);
        }
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (timerActive)
            {
                timer += Time.deltaTime;
                pv.RPC(nameof(RPC_DisplayTime), RpcTarget.AllBufferedViaServer, timer);
            }
        }
    }

    [PunRPC]
    public void RPC_DisplayTime(float _time)
    {
        DisplayTime(_time);
    }
    public void DisplayTime(float _time)
    {
        float minutes = Mathf.FloorToInt((_time) / 60);
        float seconds = Mathf.FloorToInt(_time % 60);
        timeText.text = string.Format("{00:00} : {01:00}", minutes, seconds);
    }

    [PunRPC]
    public void RPC_FindPlayers()
    {
        StartCoroutine(nameof(FindPlayers));
    }

    private IEnumerator FindPlayers()
    {
        GameObject[] playerObjects;
        int attempts = 0;
        yield return new WaitForSeconds(0.25f);
        do
        {
            playerObjects = GameObject.FindGameObjectsWithTag("Player");
            attempts++;
            yield return new WaitForSeconds(0.25f);
        } while ((playerObjects.Length < PhotonNetwork.PlayerList.Length) && (attempts < 5));

        if(playerObjects.Length <= 1)
        {
            player1 = playerObjects[0].GetComponent<PlayerController>();
        }

        else if(playerObjects.Length >= 2)
        {
            for(int i = 0; i < playerObjects.Length; i++)
            {
                if (playerObjects[i].GetComponent<PlayerController>().playerName == "PLAYER1")
                {
                    player1 = playerObjects[i].GetComponent<PlayerController>();
                }
                else
                {
                    player2 = playerObjects[i].GetComponent<PlayerController>();
                }
            }
        }
    }

    public void LevelWon()
    {
        pv.RPC(nameof(RPC_LevelWon), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void RPC_LevelWon()
    {
        Debug.Log("Level Completed");
        SetupWinScreen();
        timerActive = false;
    }

    public void CharacterSwapPostion()
    {
        if(PhotonNetwork.PlayerList.Length >= 2)
        {
            pv.RPC(nameof(RPC_CharacterSwapPositon), RpcTarget.AllBufferedViaServer);
        }
        else
        {
            Debug.Log("No other player is available to swap");
        }
    }

    [PunRPC]

    public void RPC_CharacterSwapPositon()
    {
        StartCoroutine(nameof(CharacterSwapPosition));
    }

    public IEnumerator CharacterSwapPosition()
    {
        Vector2 playerPos1 = new Vector2(player1.transform.position.x, player1.transform.position.y);
        Vector2 playerPos2 = new Vector2(player2.transform.position.x, player2.transform.position.y);

        player1.transform.position = playerPos2;
        player1.transform.rotation = Quaternion.identity;
        player1.gameObject.GetComponent<CircleCollider2D>().isTrigger = true;

        player2.transform.position = playerPos1;
        player2.transform.rotation = Quaternion.identity;
        player2.gameObject.GetComponent<CircleCollider2D>().isTrigger = true;
        yield return new WaitForSeconds(0.1f);

        player1.gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        player2.gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
    }

    public void SetupWinScreen()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            winButtons.SetActive(true);
        }
        else
        {
            winButtons.SetActive(false);
        }

        winScreen.SetActive(true);
        winTimeText.text = timeText.text;

        player1.isActive = false;
        player2.isActive = false;
    }

    public void GoToNextLevel()
    {
        pv.RPC(nameof(RPC_GoToNextLevel), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void RPC_GoToNextLevel()
    {
        if (gameLevel >= 5)
        {
            LeaveGame();
        }
        else
        {
            int index = gameLevel + 1;
            PhotonNetwork.LoadLevel("Level" + index);
        }
    }

    public void LeaveGame()
    {
        pv.RPC(nameof(RPC_LeaveGame), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void RPC_LeaveGame()
    {
        GameObject _go = GameObject.FindGameObjectWithTag("RoomManager");
        Destroy(_go);
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.LeaveRoom();
    }
}
