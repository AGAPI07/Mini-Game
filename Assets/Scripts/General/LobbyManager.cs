using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI Elements")]
    public TMP_InputField roomNameInput;
    public Button joinRoomButton;
    public Button createRoomButton;
    public GameObject inputPanel;
    public TextMeshProUGUI errorMessage;
    public GameObject connectingPanel;

    [Header("Player Prefab")]
    public GameObject playerPrefab;
    private Coroutine connectionCoroutine;

    private void Start()
    {
        // Attach button click events
        joinRoomButton.onClick.AddListener(JoinRoom);
        createRoomButton.onClick.AddListener(CreateRoom);
    }

    public void OpenLobby()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Not connected yet");
            SetUIState(false, false, true);
            if (connectionCoroutine == null) connectionCoroutine = StartCoroutine(ConnectToServer());
        }
        else
        {
            Debug.Log("Already Connected");
            SetUIState(true, false, false);
        }
    }

    private void SetUIState(bool inputPanelActive, bool errorMessageActive, bool connectingPanelActive)
    {
        inputPanel.SetActive(inputPanelActive);
        errorMessage.gameObject.SetActive(errorMessageActive);
        connectingPanel.SetActive(connectingPanelActive);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        SetUIState(true, false, false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
        SetUIState(false, true, false);
        if (connectionCoroutine != null)
        {
            StopCoroutine(connectionCoroutine);
            connectionCoroutine = null;
        }
    }

    IEnumerator ConnectToServer()
    {
        Debug.Log("Connecting...");
        bool connected = PhotonNetwork.ConnectUsingSettings();
        yield return new WaitUntil(() => connected);
    }

    private void JoinRoom()
    {
        string roomName = roomNameInput.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            ShowErrorMessage("Room name cannot be empty!");
        }
    }

    private void CreateRoom()
    {
        string roomName = roomNameInput.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
        }
        else
        {
            ShowErrorMessage("Room name cannot be empty!");
        }
    }

    private void ShowErrorMessage(string message)
    {
        Debug.LogWarning(message);
        errorMessage.text = message;
        errorMessage.gameObject.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowErrorMessage("Failed to create room: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowErrorMessage("Failed to join room: " + message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        // Instantiate player prefab
        // PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        // Load the main game scene or perform other tasks

        PhotonNetwork.LoadLevel("Multiplayer");
    }
}
