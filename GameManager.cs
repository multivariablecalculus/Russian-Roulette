using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform[] spawnPoints; // Assign chair positions in Inspector
    public GameObject playerPrefab;

    void Start()
    {
        int index = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[index].position, Quaternion.identity);
    }
}
