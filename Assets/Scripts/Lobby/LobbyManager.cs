using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace LDW
{
    // PhotonNetwork 에서 CallBack 을 받고 싶다(서버에서의 반응을 구현하고 싶다) -> MonoBehaviourPunCallbacks 상속
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        public enum Panel { Login, Menu, Lobby, Room, Register }

        [SerializeField] StatePanel statePanel;

        [SerializeField] LoginPanel loginPanel;
        [SerializeField] MenuPanel menuPanel;
        [SerializeField] RoomPanel roomPanel;
        [SerializeField] LobbyPanel lobbyPanel;
        [SerializeField] RegisterPanel registerPanel;

        private void Start()
        {
            if (PhotonNetwork.IsConnected)
                OnConnectedToMaster();
            else if (PhotonNetwork.InRoom)
                OnJoinedRoom();
            else if (PhotonNetwork.InLobby)
                OnJoinedLobby();
            else
                OnDisconnected(DisconnectCause.None);
        }

        public override void OnConnectedToMaster()
        {
            // 접속됐을 때 행동
            SetActivePanel(Panel.Menu);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            // 접속이 끊겼을 때 행동
            SetActivePanel(Panel.Login);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            // 방 만들기 실패 콜백 함수
            SetActivePanel(Panel.Menu);
            Debug.Log($"Create room failed with error({returnCode}) : {message}");
            statePanel.AddMessage($"Create room failed with error({returnCode}) : {message}");
        }

        public override void OnJoinedRoom()
        {
            // 방 만들기 성공 -> 방에 들어가므로 OnJoinedRoom 호출
            SetActivePanel(Panel.Room);

            // 방에 들어갈 때 ready 해제 / 방을 나갈 때 해제해도 됨
            PhotonNetwork.LocalPlayer.SetReady(false);
            PhotonNetwork.LocalPlayer.SetLoad(false);

            PhotonNetwork.AutomaticallySyncScene = true; // true : 방장의 씬을 자동으로 따라감
            roomPanel.UpdatePlayerList();
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.AutomaticallySyncScene = false; // 방을 나갈 때 해제

            // 방을 나갔을 때
            SetActivePanel(Panel.Menu);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // 방에 플레이어가 들어왔을 때
            roomPanel.UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // 방에서 플레이어가 나갔을 때
            roomPanel.UpdatePlayerList();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            // 방장 플레이어가 바뀌었을 때
            roomPanel.UpdatePlayerList();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
        {
            // 플레이어 프로퍼티가 변경되었을 때(현재는 Ready 상황)
            roomPanel.UpdatePlayerList();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // 방 입장 실패
            SetActivePanel(Panel.Menu);
            Debug.Log($"Join room failed with error({returnCode}) : {message}");
            statePanel.AddMessage($"Create room failed with error({returnCode}) : {message}");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            // 랜덤 방 입장 실패
            SetActivePanel(Panel.Menu);
            Debug.Log($"Join random room failed with error({returnCode}) : {message}");
            statePanel.AddMessage($"Create room failed with error({returnCode}) : {message}");
        }

        public override void OnJoinedLobby()
        {
            // Join Lobby
            SetActivePanel(Panel.Lobby);
        }

        public override void OnLeftLobby()
        {
            // Leave Lobby
            SetActivePanel(Panel.Menu);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // 방 목록이 갱신될 때 마다 호출되는 함수
            lobbyPanel.UpdateRoomList(roomList);
        }

        public void OnRegisterPanel()
        {
            SetActivePanel(Panel.Register);
        }

        public void OnRegisterCancel()
        {
            SetActivePanel(Panel.Login);
        }

        private void SetActivePanel(Panel panel)
        {
            loginPanel.gameObject?.SetActive(panel == Panel.Login);
            menuPanel.gameObject?.SetActive(panel == Panel.Menu);
            roomPanel.gameObject?.SetActive(panel == Panel.Room);
            lobbyPanel.gameObject?.SetActive(panel == Panel.Lobby);
            registerPanel.gameObject?.SetActive(panel == Panel.Register);
        }
    }
}