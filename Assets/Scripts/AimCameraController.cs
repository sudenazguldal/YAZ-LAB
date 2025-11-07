using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimCameraController : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] Transform yawTarget;     // dünya Y ekseni
    [SerializeField] Transform pitchTarget;   // local X ekseni

    [Header("Input")]
    [SerializeField] InputActionReference lookInput;          // CameraControls/Look (Vector2)
    [SerializeField] InputActionReference switchShouldInput;  // Q/E vb. (opsiyonel)


    [Header("Sensitivity")]
    [SerializeField] private float mouseSensitivity = 0.05f;

    [SerializeField] private float sensitivity = 1.5f;

    [Header("Pitch Limits")]
    [SerializeField] float pitchMax = -40f;
    [SerializeField] float pitchMin = 80f;

    [Header("Cinemachine")]
    [SerializeField] CinemachineThirdPersonFollow aimCam;
    [SerializeField] float shoulderSwitchSpeed = 5f;



    float yaw, pitch;
    private float targetCameraSide;

    private void Awake()
    {
        aimCam = GetComponent<CinemachineThirdPersonFollow>();
        targetCameraSide = aimCam.CameraSide;
    }

    
    void Start()
    {
        Vector3 angles = yawTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        lookInput.asset.Enable();
    }

    private void OnEnable()
    {
        switchShouldInput.action.Enable();
        switchShouldInput.action.performed += OnSwitchShoulder;
    }

    private void OnDisable()
    {
        switchShouldInput.action.Disable();
        switchShouldInput.action.performed -= OnSwitchShoulder;
    }

    private void OnSwitchShoulder(InputAction.CallbackContext context)
    {
        targetCameraSide = aimCam.CameraSide < 0.5f ? 1f : 0f;
    }

  
    void Update()
    {
        Vector2 look = lookInput.action.ReadValue<Vector2>();
        //look (x,y) seklinde look.x =+5 

        //hangi cihazi kullandigini anlamak icin
        if (Mouse.current != null && Mouse.current.delta.IsActuated())
        {
            look *= mouseSensitivity * SettingsMenu.MouseSensitivity;
            //genelde gridiler buyuk oldugu icin sensivty ile carpiyor
        }


        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        //�nce y ekseninde doner sonra x ekseninde
        //TAM oalrak donusu bu yapiyor

        aimCam.CameraSide = Mathf.Lerp(aimCam.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwitchSpeed);
    }

    public void SetYawPitchFromCameraForward(Transform cameraTransform)
    {
        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0;

        if (flatForward.sqrMagnitude < 0.001f)
            return;

        yaw = Quaternion.LookRotation(flatForward).eulerAngles.y;

        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }//kamernin baktigi yere donsun diye karakter
}
