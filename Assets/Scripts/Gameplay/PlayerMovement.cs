using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    private Vector3 startTouch;
    private Vector3 endTouch;
    private Vector3 force;
    private Vector3 clampedForce;

    [SerializeField] private GameObject arrow;
    private Transform arrowControl;
    public Rigidbody2D rb;

    private const float max = 5f;
    private const float power = 3f;

    private void Start()
    {
        arrowControl = arrow.transform;
        arrow.SetActive(false);
        
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

      

        if (Input.touchCount > 0)
        {
            HandleTouch(Input.GetTouch(0));
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleTouch(Touch currentTouch)
    {
        switch (currentTouch.phase)
        {
            case TouchPhase.Began:
                startTouch = Camera.main.ScreenToWorldPoint(currentTouch.position);
                break;

            case TouchPhase.Moved:
                HandleDrag(currentTouch.position);
                break;

            case TouchPhase.Ended:
                HandleRelease(currentTouch.position);

                // Notify other players about the launch
                photonView.RPC("OnLaunch", RpcTarget.Others, clampedForce);
                break;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            HandleDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleRelease(Input.mousePosition);

            // Notify other players about the launch
            photonView.RPC("OnLaunch", RpcTarget.Others, clampedForce);
        }
    }

    private void HandleDrag(Vector3 dragPosition)
    {
        Vector3 dragPos = Camera.main.ScreenToWorldPoint(dragPosition);
        force = dragPos - startTouch;
        float angle = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;
        arrowControl.rotation = Quaternion.Euler(0, 0, angle);
        arrow.SetActive(true);
    }

    private void HandleRelease(Vector3 releasePosition)
    {
        rb.velocity = Vector2.zero;
        endTouch = Camera.main.ScreenToWorldPoint(releasePosition);
        force = endTouch - startTouch;
        clampedForce = Vector3.ClampMagnitude(force, max) * power;
        rb.AddForce(clampedForce, ForceMode2D.Impulse);
        arrow.SetActive(false);
    }

    [PunRPC]
    private void OnLaunch(Vector3 launchForce)
    {
        // Handle the launch on other clients
        rb.velocity = Vector2.zero;
        rb.AddForce(launchForce, ForceMode2D.Impulse);
        arrow.SetActive(false);
    }
}
