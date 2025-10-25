using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerControls.IGameplayActions
{
    [Header("Refs")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform yawTarget;
    [SerializeField] private Transform pitchTarget; // Kameranın pitch'i takip ettiği obje
    [SerializeField] private float standingPitchY = 1.04f; // Ayakta dururken pitchTarget'ın Y pozisyonu (Göz hizası)
    [SerializeField] private float crouchPitchY = 0.641f;   // Çömelmişken pitchTarget'ın Y pozisyonu

    [Header("Components")]
    [SerializeField] private PlayerShooting shootingHandler; // Yeni script'e referans

    [Header("Movement")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float sprintMultiplier = 1.6f;


    [Header("Jump/Gravity")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f * 2f;   // biraz daha kuvvetli his için *2
    [SerializeField] private float groundedSnap = -2f;      // yere yapıştırma
    [SerializeField] private float coyoteTime = 0.1f;       // kısa tolerans

    [Header("Crouch")]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;   // yürüme hızına çarpan
    [SerializeField] private float standHeight = 1.8f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float standCenterY = 0.9f;
    [SerializeField] private float crouchCenterY = 0.6f;
    [SerializeField] private float crouchTransitionSpeed = 12f;    // yükseklik geçiş hızı




    private CharacterController controller;
    private Animator Player_anim; // (opsiyonel ama şiddetle önerilir)
    private PlayerControls input; // GENERATED sınıf
    public PlayerStance currentStance = PlayerStance.Standing;
    private MultiAimConstraint aimConstraint; // Rig bileşeni için

    private Vector2 moveInput;
    private Vector3 velocity;    // sadece dikey bileşen
    private bool isSprinting;
    public bool isAiming;
    
    private float lastGroundedTime;




    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Player_anim = GetComponentInChildren<Animator>();
        input = new PlayerControls();
        aimConstraint = GetComponentInChildren<MultiAimConstraint>();
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
    #region STANCE CONTROL

    public void SetStance(PlayerStance newStance)
    {
        if (currentStance == newStance) return; // Aynı duruma tekrar geçmesin

        currentStance = newStance;

        // 🔹 Geçiş sırasında yapılacak ortak işler
        switch (currentStance)
        {
            case PlayerStance.Standing:
                Debug.Log("Ayakta duruyor");
                Player_anim.SetBool("isCrouching", false);
                Player_anim.SetBool("isInCover", false);
                break;

            case PlayerStance.Crouching:
                Debug.Log("Crouch durumunda");
                Player_anim.SetBool("isCrouching", true);
                Player_anim.SetBool("isInCover", false);
                break;

            case PlayerStance.Covering:
                Debug.Log("Cover aldı");
                Player_anim.SetBool("isCrouching", false);
                Player_anim.SetBool("isInCover", true);
                isSprinting = false;
                break;
        }
    }

    #endregion


    // ---- Input Callbacks ----
    public void OnMove(InputAction.CallbackContext ctx)
    {

        moveInput = ctx.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude > 1f) moveInput = moveInput.normalized;

    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (currentStance == PlayerStance.Crouching || currentStance == PlayerStance.Covering)
        {
            isSprinting = false; // crouch veya cover'da koşma kapalı
            return;
        }

        isSprinting = ctx.ReadValueAsButton();

    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        // Sadece komutu PlayerShooting'e ilet!
        if (shootingHandler != null)
        {
            shootingHandler.HandleShootInput(ctx.ReadValueAsButton());
        }
    }


    public void OnCrouch(InputAction.CallbackContext ctx) {

        // cover yok: sadece crouch kontrolü
        // C -> toggle, LeftCtrl -> hold
        var keyName = ctx.control?.name;         // "c", "leftCtrl" vb.
        bool pressed = ctx.ReadValueAsButton();  // basılı mı?

        if (keyName == "c")
        {
            if (!ctx.performed) return;
            // Toggle
            if (currentStance == PlayerStance.Crouching)
                SetStance(PlayerStance.Standing);
            else
                SetStance(PlayerStance.Crouching);
        }
        else if (keyName == "leftCtrl")
        {
            // Hold
            if (pressed) SetStance(PlayerStance.Crouching);
            if (ctx.canceled) SetStance(PlayerStance.Standing);
        }
        else
        {
            // Fallback: toggle davranışı
            if (ctx.performed)
                SetStance(currentStance == PlayerStance.Crouching ? PlayerStance.Standing : PlayerStance.Crouching);
        }
    }

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

        //  Aim durumunu PlayerShooting'e de bildir.
        if (shootingHandler != null)
        {
            shootingHandler.SetAiming(isAiming);
        }
    }

    public void OnSwitchShoulder(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Switch shoulder!");
        }
    }

    // Ateşleme input'u PlayerShooting'e delege edilecek
   


    void Update()
    {
        #region ESC & Cursor kontrolü
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        #endregion
       


        #region Ground Handling
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
        #endregion

        #region Movement hesaplamaları
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

        Vector3 moveDirection = camRight * in2.x + camFwd * in2.y;

        if (moveDirection.sqrMagnitude > 1e-4f)
            moveDirection.Normalize();
        #endregion

        #region Rotation
        

        
        // her zaman kameranın baktığı yöne dön (TPS stili)
        Vector3 lookDir = isAiming ? yawTarget.forward : cameraTransform.forward;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 12f * Time.deltaTime);
        }
        #endregion

        #region Hareket ve hiz
        float walkSpeed = speed;
        float runSpeed = speed * sprintMultiplier;
        //  Stance'e göre hız düzenle
        switch (currentStance)
        {
            case PlayerStance.Crouching:
                walkSpeed *= 0.5f; // çömelince yarı hızda yürü
                break;

            case PlayerStance.Covering:
                walkSpeed *= 0.3f; // coverdayken daha yavaş hareket
                
                break;

            case PlayerStance.Standing:
            default:
                break;
        }
        float targetMax = isSprinting ? runSpeed : walkSpeed;
        float targetSpeed = targetMax * Mathf.Clamp01(in2.magnitude);

        Vector3 horizontal = moveDirection * targetSpeed;
        Vector3 delta = (horizontal + new Vector3(0, velocity.y, 0)) * Time.deltaTime;
        controller.Move(delta);

        // --- CROUCH HEIGHT LERP ---
        float targetHeight = (currentStance == PlayerStance.Crouching) ? crouchHeight : standHeight;
        float targetCenterY = (currentStance == PlayerStance.Crouching) ? crouchCenterY : standCenterY;

        // Yumuşak geçiş
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        Vector3 c = controller.center;
        c.y = Mathf.Lerp(c.y, targetCenterY, Time.deltaTime * crouchTransitionSpeed);
        controller.center = c;
        #endregion

        #region Pitch Target Height Lerp

        float targetPitchY = (currentStance == PlayerStance.Standing) ? standingPitchY : crouchPitchY;

        // Eğer PitchTarget referansı ayarlıysa
        if (pitchTarget != null)
        {
            Vector3 currentPitchPos = pitchTarget.localPosition;

            // Y pozisyonunu yumuşakça hedef yüksekliğe taşı
            currentPitchPos.y = Mathf.Lerp(
                currentPitchPos.y,
                targetPitchY,
                Time.deltaTime * crouchTransitionSpeed // Crouch geçiş hızıyla aynı hızı kullan
            );

            pitchTarget.localPosition = currentPitchPos;
        }

        #endregion


        // PlayerController.cs (Update metodunun içindeki #region Animator bloğu)

        #region Animator
        if (Player_anim != null)
        {
            // HIZ SABİTİ: Animasyon ve Rig geçişlerinin yumuşak ama hızlı olması için (30f önerilir)
            // Sizin kodunuzdaki 'damp' (0.12f) değerini hareket animasyonları için koruyoruz.
            const float BLEND_SPEED = 30f;

            // ----------------------------------------------------
            // HAREKET VE YÖN PARAMETRELERİ (Mevcut kodunuz)
            // ----------------------------------------------------
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
            Player_anim.SetBool("isAiming", isAiming);


            // ==============================================================
            // 1. AIM LAYER (Üst Vücut Nişan Alma) Kontrolü [Index 1]
            // ==============================================================
            {
                int aimLayerIndex = 1; // Base Layer = 0, AimLayer = 1
                float currentWeight = Player_anim.GetLayerWeight(aimLayerIndex);
                float targetWeight = isAiming ? 1f : 0f;

                float newWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * BLEND_SPEED);
                Player_anim.SetLayerWeight(aimLayerIndex, newWeight);
            }

            // ==============================================================
            // 2. RIG CONSTRAINT (MultiAimConstraint) Kontrolü
            // ==============================================================
            if (aimConstraint != null)
            {
                float targetRigWeight = isAiming ? 1f : 0f;

                // Rig ağırlığını da Animasyon ağırlığıyla senkronize olarak ayarla
                aimConstraint.weight = Mathf.Lerp(
                    aimConstraint.weight,
                    targetRigWeight,
                    Time.deltaTime * BLEND_SPEED // Aynı hızlı geçişi kullan
                );
            }


            // ==============================================================
            // 3. HAND LAYER (Crouch Düzeltme) Kontrolü [Index 2]
            // ==============================================================
            {
                int aimLayerHandIndex = 2; // AimLayer'dan sonraki 3. layer'ın index'i
                float currentHandWeight = Player_anim.GetLayerWeight(aimLayerHandIndex);

                float targetHandWeight = 0f;

                // KESİN ÖNCELİK MANTIĞI:
                // Yalnızca ÇÖMELMİŞSEK VE NİŞAN ALMIYORSAK düzeltme katmanını aç.
                if (currentStance == PlayerStance.Crouching && !isAiming)
                {
                    targetHandWeight = 1f;
                }

                float newHandWeight = Mathf.Lerp(
                    currentHandWeight,
                    targetHandWeight,
                    Time.deltaTime * BLEND_SPEED // Hızlı geçiş
                );
                Player_anim.SetLayerWeight(aimLayerHandIndex, newHandWeight);
            }
        }
        #endregion
    }


}

