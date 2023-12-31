using UnityEngine;
using Photon.Pun;
using Cinemachine;
using System.Collections.Generic;

public class PhotonGameplay : MonoBehaviourPun
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtual;
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    private int filledSpawnPointCounter = 0;

    void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;

        // Only the master client should instantiate the player
        if (photonView.IsMine)
        {
            photonView.RPC("InstantiatePlayer", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void InstantiatePlayer()
    {
        // Instantiate player prefab for each player in the room
        GameObject playerGameObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);






        // Check if the current client is the local player
        if (playerGameObject.GetPhotonView().IsMine)
        {
            // Set the camera follow target for the local player


            cinemachineVirtual.Follow = playerGameObject.transform;
        }
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        playerGameObject.transform.position = spawnPoints[players.Length - 1].position;
        Debug.Log("Player count: "+players.Length);
    }
}
