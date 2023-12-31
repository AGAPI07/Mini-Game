using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnect : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject heart;
    private void Start()
    {
    
    }

    private void ConnectToPhoton()
    {
        Debug.Log("Connecting to Photon...");

        // Connect to the Photon Cloud or your own Photon Server
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        
    }



    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        // heart.SetActive(true);
    }




}
