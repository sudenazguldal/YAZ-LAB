using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{

    [Header("Cams")]
    [SerializeField] private CinemachineCamera freelookCam;
    [SerializeField] private CinemachineCamera aimCam;
    [SerializeField] private Camera mainCamera;

    [Header("Other")]
    [SerializeField] private CinemachineInputAxisController inputAxisController;
  
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject crosshairUICanvas;
    [SerializeField] private PlayerControls input;

    [Header("For Aim")]
    [SerializeField] private Transform aimPos;
    [SerializeField] private float aimSmoothSpeed = 20f;
    [SerializeField] private LayerMask aimMask;


    private InputAction aimAction;
    private bool isAiming = false;
    private Transform yawTarget;
    private Transform pitchTarget;

    private bool isAimLocked = false;

    private AimCameraController aimCamController;

   
    void Awake()
    {
        crosshairUICanvas.SetActive(false);

        aimCamController = aimCam.GetComponent<AimCameraController>();

        inputAxisController = freelookCam.GetComponent<CinemachineInputAxisController>();

        input = new PlayerControls();
        input.Enable();
        aimAction = input.Gameplay.Aim;
    }

    // Update is called once per frame
    void Update()
    {

        if (isAimLocked)
        {
            // Aim pozisyonunu (Raycast) güncellemeye devam et
            if (isAiming)
            {
                UpdateAimTargetPosition();
            }
            return; 
        }
        bool aimPressed = aimAction.IsPressed();
        player.isAiming = aimPressed;

        if (aimPressed && !isAiming)
        {
            EnterAimMode();

        }
        else if (!aimPressed && isAiming)
        {
            ExitAimMode();
        }
        if (isAiming)
        {
            UpdateAimTargetPosition();
        }

    }
    private void UpdateAimTargetPosition()
    {
        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
        }
    }

    public void ForceAim(bool forceOn)
    {
        if (forceOn)
        {
            isAimLocked = true;
            EnterAimMode(); // Aim moduna girmeye zorla
        }
        else
        {
            isAimLocked = false;
            ExitAimMode(); // Normal TPS moduna dön
        }
    }
    private void ExitAimMode()
    {
        crosshairUICanvas.SetActive(false);

        isAiming = false;

        SnapFreeLookBehindPlayer();

        aimCam.Priority = 1;
        freelookCam.Priority = 2;

        inputAxisController.enabled = true;

    }

    private void SnapFreeLookBehindPlayer()
    {
        CinemachineOrbitalFollow orbitalFollow = freelookCam.GetComponent<CinemachineOrbitalFollow>();
        Vector3 forward = aimCam.transform.forward;
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        orbitalFollow.HorizontalAxis.Value = angle;
    }

    private void SnapAimCameraToPlayerForward()
    {
        aimCamController.SetYawPitchFromCameraForward(freelookCam.transform);
    }

    private void EnterAimMode()
    {
        crosshairUICanvas.SetActive(true);

        isAiming = true;

        SnapAimCameraToPlayerForward();

        aimCam.Priority = 2;
        freelookCam.Priority = 1;

        inputAxisController.enabled = false;

    }

}
