using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace LDW
{
    public class RoomPanel : MonoBehaviour
    {
        [SerializeField] RectTransform playerContent;
        [SerializeField] PlayerEntry playerEntryPrefab;
        [SerializeField] Button startButton;

        public void UpdatePlayerList()
        {
            // Clear player list
            for (int i = 0; i < playerContent.childCount; i++)
            {
                Destroy(playerContent.GetChild(i).gameObject);
            }

            // Update player list
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PlayerEntry entry = Instantiate(playerEntryPrefab, playerContent);
                entry.SetPlayer(player);
            }

            // player add
            if (PhotonNetwork.IsMasterClient) // ���� �������� Ȯ�� ��û
                CheckPlayerReady();
            else
                startButton.gameObject.SetActive(false);
        }

        // Start Button
        public void StartGame()
        {
            PhotonNetwork.LoadLevel("PlayTestSceneVer1");
        }

        // Leave Button
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void CheckPlayerReady()
        {
            int readyCount = 0;

            // ��ü �÷��̾��� ���� ��Ȳ Ȯ��
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.GetReady())
                    readyCount++;
            }

            startButton.gameObject.SetActive(readyCount == PhotonNetwork.PlayerList.Length);
        }
    }
}