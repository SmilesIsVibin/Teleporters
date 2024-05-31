using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [Header("Room List")]
    [SerializeField] MenuItem[] menuList;

    [Header("UI Text")]
    [SerializeField] public TMP_Text errorText;
    [SerializeField] public TMP_Text roomNameText;
    [SerializeField] public TMP_Text roomHostName;
    [SerializeField] public TMP_Text roomPlayerCount;

    [Header("UI Input Fields")]
    [SerializeField] public TMP_InputField inputName;
    [SerializeField] public TMP_InputField createRoomInput;
    [SerializeField] public TMP_InputField joinRoomInput;

    [Header("Misc")]
    [SerializeField] public Transform playerListContent;
    [SerializeField] public GameObject playerListItemPrefab;
    [SerializeField] public GameObject startGameButton;

    private void Awake()
    {
        Instance = this;
    }

    public void GetPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            inputName.text = PlayerPrefs.GetString("PlayerNickname");
        }
        else
        {
            PlayerPrefs.SetString("PlayerNickname", "Guest" + Random.Range(0, 1000).ToString("0000"));
            inputName.text = PlayerPrefs.GetString("PlayerNickname");
        }
    }

    public void SetUpPlayerName()
    {
        PhotonNetwork.NickName = inputName.text;
        PlayerPrefs.SetString("PlayerNickname", PhotonNetwork.NickName);
        OpenMenu("title");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createRoomInput.text))
        {
            return;
        }
        else
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(createRoomInput.text, options);
            OpenMenu("loading");
        }
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(joinRoomInput.text))
        {
            return;
        }
        else
        {
            PhotonNetwork.JoinRoom(joinRoomInput.text);
            OpenMenu("loading");
        }
    }

    public void SetUpRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name.ToString();
        roomHostName.text = PhotonNetwork.MasterClient.NickName.ToString();

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        roomPlayerCount.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void MasterClientTransfer()
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void RoomCreateFailed(string message)
    {
        errorText.text = "Room Creation Has Failed: " + message;
    }

    public void AddPlayerData(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menuList.Length; i++)
        {
            if (menuList[i].menuName == menuName)
            {
                menuList[i].Open();
            }
            else if (menuList[i])
            {
                CloseMenu(menuList[i]);
            }
        }
    }
    public void OpenMenu(MenuItem menu)
    {
        for (int i = 0; i < menuList.Length; i++)
        {
            if (menuList[i])
            {
                CloseMenu(menuList[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(MenuItem menu)
    {
        menu.Close();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}