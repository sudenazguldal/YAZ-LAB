using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerControls.IGameplayActions
{
    [Header("Refs")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform yawTarget;
    [SerializeField] private Transform pitchTarget; 
    [SerializeField] private float standingPitchY = 1.04f; 
    [SerializeField] private float crouchPitchY = 0.641f;  

    [Header("Components")]
    [SerializeField] public PlayerShooting shootingHandler; 
    [Header("Movement")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float sprintMultiplier = 1.6f;


    [Header("Gravity")]
   
    [SerializeField] private float gravity = -9.81f * 2f;  
    [SerializeField] private float groundedSnap = -2f;      
    [SerializeField] private float coyoteTime = 0.1f;       

    [Header("Crouch")]

    [SerializeField] private float standHeight = 1.8f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float standCenterY = 0.9f;
    [SerializeField] private float crouchCenterY = 0.6f;
    [SerializeField] private float crouchTransitionSpeed = 12f;   

    [Header("Cover Detection")]
    [SerializeField] private LayerMask coverLayerMask; // Sadece siper alınabilecek objelerin katmanı
    [SerializeField] private float coverDetectionRange = 1.0f; // Ne kadar yakından siper alınabilir
    [SerializeField] private float coverDetectionHeight = 1.5f;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] walkFootstepSounds; 
    [SerializeField] private AudioClip[] runFootstepSounds;  
    [Header("Footstep Timing")]
    [SerializeField] private float walkTimeBetweenSteps = 0.45f; // Yürüme aralığı
    [SerializeField] private float runTimeBetweenSteps = 0.25f;  // Koşma aralığı 
    [SerializeField] private float speedThreshold = 0.05f;

    private float stepTimer;
    public bool isInCutscene = false;





    private CharacterController controller;
    private Animator Player_anim; 
    private PlayerControls input; 
    public PlayerStance currentStance = PlayerStance.Standing;
    private MultiAimConstraint aimConstraint; // Rig bileşenleri için

    private Vector2 moveInput;
    private Vector3 velocity;    // sadece dikey bileşende hız
    private bool isSprinting;
    public bool isAiming;

    private float lastGroundedTime;

    private PickupItem nearItem = null;




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
        Cursor.visible = false;                
        Cursor.lockState = CursorLockMode.Locked;
    }
    #region STANCE CONTROL

    public void SetStance(PlayerStance newStance)
    {
        if (currentStance == newStance) return; // Aynı duruma tekrar geçmesin

        currentStance = newStance;

        // Geçiş sırasında yapılacak ortak işler
        switch (currentStance)
        {
            case PlayerStance.Standing:
                Debug.Log("Ayakta duruyor");
                // Animasyon ve diğer durum parametrelerini sıfırla
                Player_anim.SetBool("isCrouching", false); 
                Player_anim.SetBool("isInCover", false);  
                                                        
                break;

            case PlayerStance.Crouching:
                Debug.Log("Crouch durumunda");
                // Animasyon
                Player_anim.SetBool("isCrouching", true); 
                Player_anim.SetBool("isInCover", false);  
                isSprinting = false;
               
                break;

            case PlayerStance.Covering:
                Debug.Log("Siper Aldı (Alçak)");
                
                Player_anim.SetBool("isCrouching", true);  
                Player_anim.SetBool("isInCover", true);   
                isSprinting = false;
               
                break;
        }



    }

    #endregion


  
    public void OnMove(InputAction.CallbackContext ctx)
    {

        moveInput = ctx.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude > 1f) moveInput = moveInput.normalized;

    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (currentStance == PlayerStance.Crouching || currentStance == PlayerStance.Covering)
        {
            isSprinting = false; // crouch veya coverda koşma kapalı
            return;
        }

        isSprinting = ctx.ReadValueAsButton();

    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        // Sadece komutu PlayerShootinge iletir gerisi orda
        if (shootingHandler != null)
        {
            shootingHandler.HandleShootInput(ctx.ReadValueAsButton());
        }
    }

    public void SetNearItem(PickupItem item)
    {
        // PickupItem yaklaştığında/uzaklaştığında referansı günceller ki ne aldığımızı bilelim
        nearItem = item;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        // Sadece tuşa basıldığında (down) çalışsın
        if (!ctx.performed) return;

        // Eğer yakında bir öge varsa
        if (nearItem != null)
        {
            // Toplama işlemini InventoryCollector'a atıyo
            InventoryCollector collector = GetComponent<InventoryCollector>();

            if (collector != null)
            {
                // PickupItem üzerindeki Collect metodunu çağırıyoruz.
                nearItem.Collect(collector);

                // İşlem bittiği için referansı sıfırla
                SetNearItem(null);
            }
        }

    }

    private PlayerStance CheckCoverType()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, coverDetectionRange, coverLayerMask))
        {
            // Mutlak Yükseklik yerine, objenin zemin seviyesine göre boyunu baz alıyoruz doğru bir hesap olsun diye
            float hitPointY = hit.point.y;
            float groundY = transform.position.y;
            float objectHitHeightFromGround = hitPointY - groundY;

            //  Cover kontrolü:Çarptığımız nokta Cover sınırından küçük mü
            if (objectHitHeightFromGround <= coverDetectionHeight)
            {
                
                return PlayerStance.Covering;
            }

            // Çarptı ama çok yüksek  siper alınamaz.
            
            return PlayerStance.Standing;
        }

        // Çarpışma yoksa
        return PlayerStance.Standing;
    }

    private void EnterCover(PlayerStance stance)
    {
        
        SetStance(stance);
       
       
    }

    private void ExitCover()
    {
        // Ayakta duruşa geçişi SetStance'e devret
        SetStance(PlayerStance.Standing);
        
    }

    private void Crouch()
    {
       
        SetStance(PlayerStance.Crouching);
       
    }

    private void StandUp()
    {
        
        SetStance(PlayerStance.Standing);
      
    }

    private void TryCoverOrCrouch()
    {
        //  ÇIKIŞ ÖNCELİĞİ Eğer zaten Cover modundaysa -> Ayakta duruşa geç
        if (currentStance == PlayerStance.Covering) 
        {
            ExitCover();
            return;
        }

        // COVER GİRİŞ ÖNCELİĞİ Siper bulabiliyorsa -> Cover pozisyonuna gir
        PlayerStance coverType = CheckCoverType();

        // Eğer Covering döndüyse 
        if (coverType == PlayerStance.Covering) 
        {
            EnterCover(coverType);
            return;
        }

        // CROUCH/STAND: Cover yoksa -> Crouch/Stand toggle yap

        if (currentStance == PlayerStance.Crouching)
        {
            StandUp();
        }
        else if (currentStance == PlayerStance.Standing)
        {
            Crouch();
        }
    }
   

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        var keyName = ctx.control?.name;
        bool pressed = ctx.ReadValueAsButton();

       
        // C TUŞU: TOGGLE 
       
        if (keyName == "c")
        {
            if (!ctx.performed) return;

            // Tüm karar mantığı TryCoverOrCrouch metodunda
            TryCoverOrCrouch();
        }
       
        // LEFT CTRL TUŞU: HOLD (Basılı Tutma)
        
        else if (keyName == "leftCtrl")
        {
            // HOLD tuşlarındaki mantık farklı:

            // Hangi moda geçeceğine karar ver (Cover veya Crouch)
            PlayerStance nextStance = CheckCoverType();

            // Eğer siper yoksa (Standing döndüyse), zorla Crouch yap
            if (nextStance == PlayerStance.Standing)
                nextStance = PlayerStance.Crouching;

            if (pressed)
                SetStance(nextStance); // Basılı tutulursa Cover/Crouch'a girer
            if (ctx.canceled)
                SetStance(PlayerStance.Standing); // Bırakılırsa ayağa kalkar
        }


    }

    
    void PlayFootstepSound()
    {
        // Hangi ses dizisini kullanılacağını belirle
        AudioClip[] currentSoundArray;

       
        if (isSprinting)
        {
            currentSoundArray = runFootstepSounds;
        }
        else
        {
            currentSoundArray = walkFootstepSounds;
        }

        if (currentSoundArray.Length == 0 || footstepSource == null || !controller.isGrounded) return;
        if (footstepSource.isPlaying) return;

        // Rastgele bir ses klibi seçer akıcı olsun diye
        AudioClip clip = currentSoundArray[UnityEngine.Random.Range(0, currentSoundArray.Length)];

       
        footstepSource.PlayOneShot(clip);
    }


    public void OnAim(InputAction.CallbackContext ctx)
    {
        bool pressed = ctx.ReadValueAsButton();
        isAiming = pressed;

        // Siperde mi?
        bool isCurrentlyCovering = (currentStance == PlayerStance.Covering);


        if (Player_anim != null)
        {
            // Peeking animasyonunu tetikle -siperdeyse VE nişan alıyorsa
            bool shouldPeekAnimate = isCurrentlyCovering && isAiming;
            Player_anim.SetBool("isPeeking", shouldPeekAnimate);
        }

        // PlayerShooting'e hem Aim durumunu hem de Cover durumunu bildirmek için
        if (shootingHandler != null)
        {
            // isCurrentlyCovering'i ikinci parametre olarak gönderiyo
            shootingHandler.SetAiming(isAiming, isCurrentlyCovering);
        }


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

        if (isInCutscene)
        {
            
            // Hareket parametrelerini sıfırla ki yürüme animasyonu dursun cutscenedeyiz çünkü
            if (Player_anim != null)
            {
                Player_anim.SetFloat("MoveX", 0f);
                Player_anim.SetFloat("MoveY", 0f);
                Player_anim.SetFloat("Speed01", 0f);
            }

            
            
            HandleLayersAndRig();

            //  yerçekimi de duruyor çünkü cutscenede havada kalmasın
            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = groundedSnap; // Yere yapıştır
            }
            controller.Move(velocity * Time.deltaTime); // Sadece dikey hareketi uygula

            return; 
        }
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

        // --- Kamera yönlerini al Y düzleminde---
        Vector3 camFwd, camRight;
        if (isAiming)
        {   // aimcam aktif -> yaw target yönleri
            camFwd = yawTarget.forward;
            camRight = yawTarget.right;
        }
        else
        {   // normal TPS -> ana kamera yönleri
            camFwd = cameraTransform.forward;
            camRight = cameraTransform.right;
        }

        //neden iki farklı şekilde aldık?
        // aim kamera yawtargetin çocuğu pitche bağlı
        //tps ise playerın childi tps-followera bağlı
        //sadece tps alsak aim tpse göre hareket ediyor yani 90 derece dönünce a tuşunun ileri götürmesi gibi sorunlar oluyor


        camFwd.y = 0f; 
        camRight.y = 0f;
        camFwd.Normalize(); 
        camRight.Normalize();

        Vector3 moveDirection = camRight * in2.x + camFwd * in2.y;

        if (moveDirection.sqrMagnitude > 1e-4f)
            moveDirection.Normalize();
        #endregion

        #region Rotation



        // her zaman kameranın baktığı yöne dön (yani kamera ile gideceği yönü kararlaştırma strafe kullanmamızın başlıca nedeni)
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
        //  Stance göre hız düzenle
        switch (currentStance)
        {
            case PlayerStance.Crouching:
                walkSpeed *= 0.5f; // çömelince yarı hızda yürü
                break;

            case PlayerStance.Covering:
                walkSpeed *= 0.4f;

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


        #region Footstep Logic
        // Karakterin Yatay Hızını Hesaplama
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);

        //  Karakter hareket ediyor mu?
        bool isMoving = controller.isGrounded && horizontalVelocity.sqrMagnitude > speedThreshold * speedThreshold;

        //  Adım Sesleri
        if (isMoving)
        {
            //  Adım süresini hıza göre belirler
            float targetTimeBetweenSteps = isSprinting ? runTimeBetweenSteps : walkTimeBetweenSteps;

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayFootstepSound();

                // Zamanlayıcıyı bir sonraki adım için sıfırlar
                stepTimer += targetTimeBetweenSteps; // Eski kalan sürenin üzerine ekle

                // Eğer stepTimer çok geride kaldıysa (oyun takıldıysa), sadece hedef süreye ayarla.
                if (stepTimer < 0)
                {
                    stepTimer = targetTimeBetweenSteps;
                }
            }
        }
        else
        {
            // Hareket etmiyorsa zamanlayıcıyı resetle
            stepTimer = walkTimeBetweenSteps;
        }

        #endregion




        #region Cover Validation (Siper Geçerliliğini Kontrol Etme)


        if (currentStance == PlayerStance.Covering)
        {
            // Yalnızca siperdeysek nümüzde hala siper var mı kontrol et
           PlayerStance requiredStance = CheckCoverType();

            // Eğer CheckCoverType artık Cover değil (Standing döndü) ise karakter nesneden ayrıldı demektir.
            
            if (requiredStance != PlayerStance.Covering)
            {
                // Cover durumundan Crouch durumuna indir
                SetStance(PlayerStance.Crouching);
               
            }
        }
        #endregion

        #region Animator (Movement)
        if (Player_anim != null)
        {
            const float damp = 0.12f;

            float moveX;
            float moveY;

            if (currentStance == PlayerStance.Covering)
            {
                moveX = moveInput.x;
                moveY = 0f;
            }
            else
            {
                
                
                moveX = Vector3.Dot(moveDirection, camRight);
                moveY = Vector3.Dot(moveDirection, camFwd);
            }
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
        }
        #endregion

        HandleLayersAndRig();




    }


    private void HandleLayersAndRig()
    {
        #region Animator
        if (Player_anim != null)
        {
            if (Player_anim == null) return; // Güvenlik kontrolü

            const float BLEND_SPEED = 30f;

            // -----------------------------------------------------------------
            // 1. AIM LAYER Üst Vücut Nişan Alma Kontrolü [Index 1]
            // --------------------------------------------------------------
            {
                int aimLayerIndex = 1; // Base Layer = 0, AimLayer = 1
                float currentWeight = Player_anim.GetLayerWeight(aimLayerIndex);
                float targetWeight = isAiming ? 1f : 0f;

                float newWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * BLEND_SPEED);
                Player_anim.SetLayerWeight(aimLayerIndex, newWeight);
            }

            // --------------------------------------------------
            // 2. RIG CONSTRAINT MultiAimConstraint Kontrolü
            // --------------------------------------------------
            if (aimConstraint != null)
            {
                //   Peeking yapıyorsak (SpinePeek açık), Rig'i Kapat
                bool shouldBePeeking = currentStance == PlayerStance.Covering && isAiming;

                //  Eğer Peeking yapmıyorsak ve Aiming yapıyorsak Rig'i aç (1f).
                // Peeking yapıyorsak veya Nişan almıyorsak Rig'i kapat (0f).
                float targetRigWeight = (isAiming && !shouldBePeeking) ? 1f : 0f;

                // Rig ağırlığını senkronize olarak ayarla
                aimConstraint.weight = Mathf.Lerp(
                    aimConstraint.weight,
                    targetRigWeight,
                    Time.deltaTime * BLEND_SPEED
                );
            }


            // -------------------------------------------------------------
            // 3. HAND LAYER Crouch Sol El Düzeltme Kontrolü [Index 2]
            // -------------------------------------------------------------
            {
                int aimLayerHandIndex = 2;
                float currentHandWeight = Player_anim.GetLayerWeight(aimLayerHandIndex);

                float targetHandWeight = 0f;

                // Yalnızca ÇÖMELMİŞSEK VE NİŞAN ALMIYORSAK düzeltme katmanını açar

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
            {

                int spinePeekLayerIndex = 3;

                float currentSpineWeight = Player_anim.GetLayerWeight(spinePeekLayerIndex);

                //  SADECE Siperdeysek VE Nişan Alıyorsak açar
                bool shouldBePeeking = currentStance == PlayerStance.Covering && isAiming;

                float targetSpineWeight = shouldBePeeking ? 1f : 0f;


                float newSpineWeight = Mathf.Lerp(
                    currentSpineWeight,
                    targetSpineWeight,
                    Time.deltaTime * BLEND_SPEED // Hızlı geçiş (30f)
                );
                Player_anim.SetLayerWeight(spinePeekLayerIndex, newSpineWeight);
            }
        }
        #endregion
    }


}

