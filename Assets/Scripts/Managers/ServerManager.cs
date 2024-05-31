using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class ServerManager : MonoBehaviourPunCallbacks
{
    public static ServerManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        MenuManager.Instance.GetPlayerName();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("You are currently connected to Master Server");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        base.OnConnectedToMaster();
    }

    public void CreateRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            return;
        }
        else
        {
            PhotonNetwork.CreateRoom(roomName);
            MenuManager.Instance.OpenMenu("loading");
        }
    }

    public void JoinRoom(RoomInfo roomName)
    {
        MenuManager.Instance.OpenMenu("loading");
        PhotonNetwork.JoinRoom(roomName.Name);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenMenu("user");
        base.OnJoinedLobby();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room has been created: " + PhotonNetwork.CurrentRoom.Name.ToString());
        base.OnCreatedRoom();
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.SetUpRoom();
        MenuManager.Instance.OpenMenu("room");
        base.OnJoinedRoom();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        MenuManager.Instance.MasterClientTransfer();
        base.OnMasterClientSwitched(newMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.RoomCreateFailed(message);
        MenuManager.Instance.OpenMenu("error");
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room, Room cannot be found");
        MenuManager.Instance.OpenMenu("error");
        base.OnJoinRoomFailed(returnCode, message);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
        base.OnLeftRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        MenuManager.Instance.AddPlayerData(newPlayer);
        MenuManager.Instance.SetUpRoom();
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MenuManager.Instance.SetUpRoom();
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public void StartGame()
    {
        MenuManager.Instance.OpenMenu("loading");
        PhotonNetwork.LoadLevel(1);
    }
}