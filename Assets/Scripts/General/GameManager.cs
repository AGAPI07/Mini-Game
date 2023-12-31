using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPun, IPunObservable
{
    public Button winButton;
    public TextMeshProUGUI resultText;

    private bool hasClickedWin = false;

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Enable button for the local player only
            winButton.onClick.AddListener(OnClickWin);
        }
        else
        {
            // Disable components for remote players
            winButton.interactable = false;
        }
    }

    private void OnClickWin()
    {
        // Ensure a player can only click the win button once
        if (!hasClickedWin)
        {
            hasClickedWin = true;

            // Notify other players about the win
            photonView.RPC("RpcDisplayResult", RpcTarget.All, true);
        }
    }

    [PunRPC]
    private void RpcDisplayResult(bool isWinner)
    {
      
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Add serialization logic for sending data to other players
            stream.SendNext(hasClickedWin);
        }
        else
        {
            // Add deserialization logic for receiving data from other players
            hasClickedWin = (bool)stream.ReceiveNext();
        }
    }
}
