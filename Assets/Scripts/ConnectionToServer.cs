using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using Photon.Pun;
using Photon.Realtime;
//
using UnityEngine.UI;
using TMPro;

public class ConnectionToServer : MonoBehaviourPunCallbacks
{
    public static ConnectionToServer Instance;              // сінглтон

    [SerializeField] private TMP_Text countOfPlayersText;
    //
    [SerializeField] private TMP_InputField inputRoomName;
    [SerializeField] private TMP_Text roomName;
    //
    [SerializeField] private Transform transformRoomList;
    [SerializeField] private GameObject roomItemPrefab;
    //
    [SerializeField] private Transform transformPlayerList;
    [SerializeField] private GameObject playerListItem;
    //
    [SerializeField] private GameObject startGameButton;

    private void Awake()
    {
        Instance = this;
        PhotonNetwork.ConnectUsingSettings();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        countOfPlayersText.text = "Players: " + PhotonNetwork.CountOfPlayers.ToString();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Welcome to server");
        PhotonNetwork.JoinLobby();
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
        Debug.Log(PhotonNetwork.NickName);

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Join to LOBBY");
        WindowsManager.Layout.OpenLayout("Panel_MainMenu");
    }

    public void CreateNewRoom()
    {
        if(string.IsNullOrEmpty(inputRoomName.text))
        {
            return;
        }
        //
        PhotonNetwork.CreateRoom(inputRoomName.text);
    }

    public override void OnJoinedRoom()
    {
        WindowsManager.Layout.OpenLayout("Panel_GameRoom");
        //
        if(PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
        //
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Join " + PhotonNetwork.CurrentRoom.Name.ToString());
        //
        //
        Player [] players = PhotonNetwork.PlayerList;
        foreach(Transform trns in transformPlayerList)
        {
            Destroy(trns.gameObject);
        }
        for(int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItem, transformPlayerList).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
    }
    //
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        WindowsManager.Layout.OpenLayout("Panel_MainMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trns in transformRoomList)
        {
            Destroy(trns.gameObject);
        }
        for(int i = 0; i < roomList.Count; i++)
        {
            Instantiate(roomItemPrefab, transformRoomList).GetComponent<RoomItem>().SetUp(roomList[i]);
        }
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        Debug.Log("Join room: " + info.Name.ToString());
    }

    //
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName.ToString());
        Instantiate(playerListItem, transformPlayerList).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void ConnectToRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnMasterClientSwitched(Player newPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }

    public void StartGameLevel(int levelIndex)
    {
        PhotonNetwork.LoadLevel(levelIndex);
    }
}
