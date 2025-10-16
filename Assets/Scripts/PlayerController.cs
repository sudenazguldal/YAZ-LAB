using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerControls.IGameplayActions
{
    [Header("Refs")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform yawTarget;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 1.8f;


    [Header("Jump/Gravity")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f * 2f;   // biraz daha kuvvetli his için *2
    [SerializeField] private float groundedSnap = -2f;      // yere yapıştırma
    [SerializeField] private float coyoteTime = 0.1f;       // kısa tolerans

    private CharacterController controller;
    private Animator Player_anim; // (opsiyonel ama şiddetle önerilir)
    private PlayerControls input; // GENERATED sınıf

    private Vector2 moveInput;
    private Vector3 velocity;    // sadece dikey bileşen
    private bool isSprinting;
    public bool isAiming;
    private bool isGrounded;
    private float lastGroundedTime;




    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Player_anim = GetComponentInChildren<Animator>(); ; // yoksa null kalır, sorun değil
        input = new PlayerControls();
    }

    void OnEnable()
    {
        input.Gameplay.SetCallbacks(this);
        input.Gameplay.Enable();
    }

    void OnDisable()
    {
        input.Gameplay.Disable();
        input.Gameplay.RemoveCallbacks(this);
    }

    void OnDestroy() { input.Dispose(); }

    void Start()
    {
        Cursor.visible = false;                 // imleci gizle
        Cursor.lockState = CursorLockMode.Locked; // imleci ekranın ortasına kilitle
    }


    // ---- Input Callbacks ----
    public void OnMove(InputAction.CallbackContext ctx)
    {

        moveInput = ctx.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude > 1f) moveInput = moveInput.normalized;

    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        isSprinting = ctx.ReadValueAsButton();
    }

    public void OnReload(InputAction.CallbackContext ctx) { }

    public void OnFire(InputAction.CallbackContext ctx) { }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        // coyote time: son ~0.1 saniye içinde yerdeydiyse izin ver
        bool grounded = controller.isGrounded || (Time.time - lastGroundedTime) <= coyoteTime;
        if (grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);


        }
    }

    public void OnAim(InputAction.CallbackContext ctx)
    {
        isAiming = ctx.ReadValueAsButton();
    }

    public void OnSwitchShoulder(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Switch shoulder!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        // --- Ground handling ---
        bool grounded = controller.isGrounded;
        if (grounded)
        {
            lastGroundedTime = Time.time;
            if (velocity.y < 0f)
                velocity.y = groundedSnap;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // --- INPUT ---
        Vector2 in2 = moveInput;
        if (in2.sqrMagnitude > 1f)
            in2.Normalize();

        // --- Kamera yönlerini al (Y düzleminde) ---
        Vector3 camFwd, camRight;
        if (isAiming)
        {
            // AimCamera aktif → yawTarget yönleri
            camFwd = yawTarget.forward;
            camRight = yawTarget.right;
        }
        else
        {
            // Normal TPS → ana kamera yönleri
            camFwd = cameraTransform.forward;
            camRight = cameraTransform.right;
        }

        //neden iki farklı şekilde aldık?
        // aim kamera yawtargetin çocuğu pitche bağlı
        //tps ise playerın childi tps-followera bağlı
        //sadece tps alsak aim tpse göre hareket ediyor yani 90 derece dönünce a tuşunun ileri götürmesi gibi sorunlar oluyor


        camFwd.y = 0f; camRight.y = 0f;
        camFwd.Normalize(); camRight.Normalize();

        // --- Hareket yönü (kamera uzayında) ---
        Vector3 moveDirection = camRight * in2.x + camFwd * in2.y;
        if (moveDirection.sqrMagnitude > 1e-4f)
            moveDirection.Normalize();

        // --- ROTATION ---
        // her zaman kameranın baktığı yöne dön (TPS stili)
        Vector3 lookDir = isAiming ? yawTarget.forward : cameraTransform.forward;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 12f * Time.deltaTime);
        }

        // --- HAREKET ---
        float walkSpeed = speed;
        float runSpeed = speed * sprintMultiplier;
        float targetMax = isSprinting ? runSpeed : walkSpeed;
        float targetSpeed = targetMax * Mathf.Clamp01(in2.magnitude);

        Vector3 horizontal = moveDirection * targetSpeed;
        Vector3 delta = (horizontal + new Vector3(0, velocity.y, 0)) * Time.deltaTime;
        controller.Move(delta);

        // --- ANIMATOR PARAMETRELERİ ---
        if (Player_anim != null)
        {
            const float damp = 0.12f;

            float moveX = Vector3.Dot(moveDirection, camRight);
            float moveY = Vector3.Dot(moveDirection, camFwd);
            Player_anim.SetFloat("MoveX", moveX, damp, Time.deltaTime);
            Player_anim.SetFloat("MoveY", moveY, damp, Time.deltaTime);

            float speed01;
            if (runSpeed <= walkSpeed)
                speed01 = targetSpeed > 0 ? 1f : 0f;
            else if (targetSpeed <= walkSpeed)
                speed01 = Mathf.InverseLerp(0f, walkSpeed, targetSpeed) * 0.5f;
            else
                speed01 = 0.5f + Mathf.InverseLerp(walkSpeed, runSpeed, targetSpeed) * 0.5f;

            Player_anim.SetFloat("Speed01", speed01, damp, Time.deltaTime);
        }
    }


}

