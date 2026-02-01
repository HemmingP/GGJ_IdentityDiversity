using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions inputActions;
    private InputSystem_Actions.PlayerActions playerActions;
    private Rigidbody2D RB2D => GetComponent<Rigidbody2D>();
    Vector2 moveInput;
    public float MoveMultiplier = 100f;
    [Range(0f, 1f)]
    public float airControl = 0.2f;
    [SerializeField] private Collider2D groundCollider;
    private bool IsInTheAir => !groundCollider.IsTouchingLayers();
    // Animator has the following parameters:
    // HoriozontalSpeed (float)
    // InTheAir (bool)
    // VerticalSpeed (float)
    // Die (trigger)
    [SerializeField] private Animator animator;
    public Wardrobe wardrobe;

    bool jumping = false;
    float timeSinceJump = 0f;
    public float baseJumpForce = 300f;
    [Range(0f, 1f)]
    [SerializeField]
    private float health = 1f;
    private float timeSinceDamageTaken = 0f;
    private bool IsDead => health <= 0f;
    public ElementalType hazardTypeInflicted;
    private Dictionary<ElementalType, bool> hazardTotallySafeFrom = new Dictionary<ElementalType, bool>();
    public void SetHazardTotallySafeFrom(ElementalType type, bool isSafe)
    {
        hazardTotallySafeFrom[type] = isSafe;
    }

    [SerializeField]
    private VariantSoundPlayer jumpSoundPlayer;
    [SerializeField]
    private VariantSoundPlayer dieSoundPlayer;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        playerActions = inputActions.Player;
        playerActions.AddCallbacks(this);

        hazardTotallySafeFrom[ElementalType.Water] = false;
        hazardTotallySafeFrom[ElementalType.Fire] = false;
        hazardTotallySafeFrom[ElementalType.Gas] = false;
        hazardTotallySafeFrom[ElementalType.Cold] = false;
    }

    void OnEnable()
    {
        playerActions.Enable();
    }

    void OnDisable()
    {
        playerActions.Disable();
    }

    void OnDestroy()
    {
        inputActions.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        float linearVelocityX = RB2D.linearVelocityX;
        animator.SetFloat("HoriozontalSpeed", Mathf.Abs(linearVelocityX));
        animator.SetBool("InTheAir", IsInTheAir);
        animator.SetFloat("VerticalSpeed", RB2D.linearVelocityY);

        float mobilityModifier = 1f;
        WardrobeItems currentWardrobe = wardrobe.WardrobeItemsList.Find(x => x.ElementalType == wardrobe.CurrentElementalType);
        if (currentWardrobe.ElementalType != ElementalType.None)
        {
            mobilityModifier = currentWardrobe.mobilityModifier;
        }

        timeSinceJump += Time.deltaTime;
        if (Mathf.Abs(linearVelocityX) < 3f || IsInTheAir)
        {
            RB2D.AddForce(new Vector2(moveInput.x, 0) * Time.deltaTime * MoveMultiplier * (IsInTheAir ? airControl : 1f) * mobilityModifier);
        }
        RB2D.AddForce(new Vector2(0, baseJumpForce) * Time.deltaTime * (jumping ? 1f : 0f) / (timeSinceJump * 10 + 1f) * mobilityModifier);

        if (!IsInTheAir)
        {
            // Flip player sprite based on movement direction
            if (moveInput.x > 0 && linearVelocityX > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveInput.x < 0 && linearVelocityX < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        timeSinceDamageTaken += Time.deltaTime;

        if (timeSinceDamageTaken > 1f && !IsDead)
        {
            ChangeHealth(Time.deltaTime / 10f);
        }
    }

    void FixedUpdate()
    {
        if (hazardTypeInflicted == ElementalType.None) return;
        float hazardResistance = 0f;
        if (wardrobe != null &&
            !wardrobe.IsVulnerable &&
            wardrobe.CurrentElementalType == hazardTypeInflicted)
        {
            WardrobeItems currentWardrobe = wardrobe.GetWardrobeItemsByType(hazardTypeInflicted);
            hazardResistance = currentWardrobe.hazardResistance;
        }

        switch (hazardTypeInflicted)
        {
            case ElementalType.Water:
                ChangeHealth(-Time.fixedDeltaTime / 30f * (1f - hazardResistance) * (hazardTotallySafeFrom[ElementalType.Water] ? 0f : 1f));
                break;
            case ElementalType.Fire:
                ChangeHealth(-Time.fixedDeltaTime / 2f * (1f - hazardResistance) * (hazardTotallySafeFrom[ElementalType.Fire] ? 0f : 1f));
                break;
            case ElementalType.Gas:
                ChangeHealth(-Time.fixedDeltaTime / 5f * (1f - hazardResistance) * (hazardTotallySafeFrom[ElementalType.Gas] ? 0f : 1f));
                break;
            case ElementalType.Cold:
                ChangeHealth(-Time.fixedDeltaTime / 10f * (1f - hazardResistance) * (hazardTotallySafeFrom[ElementalType.Cold] ? 0f : 1f));
                break;
            default:
                Debug.LogWarning("Hazard type not implemented: " + hazardTypeInflicted);
                break;
        }
    }

    public void InflictHazardType(ElementalType type)
    {
        hazardTypeInflicted = type;
    }

    public void ChangeHealth(float changeAmount)
    {
        health += changeAmount;
        health = Mathf.Clamp01(health);
        UIElementOptionsManager uIElementOptionsManager = UIElementOptionsManager.Instance();
        if (uIElementOptionsManager != null)
        {
            uIElementOptionsManager.SetHealthAmount(health);
        }
        if (changeAmount < 0f)
        {
            timeSinceDamageTaken = 0f;
        }
        if (health <= 0f)
        {
            animator.SetTrigger("Die");
            animator.enabled = false;
            // Disable player controls
            playerActions.Disable();
            RB2D.bodyType = RigidbodyType2D.Static;
            GetComponent<Collider2D>().enabled = false;
            RagdollPartHolder[] ragdollParts = GetComponentsInChildren<RagdollPartHolder>();
            foreach (RagdollPartHolder part in ragdollParts)
            {
                part.EnableRagdoll();
            }
            SceneLoader.Instance()?.ReloadCurrentScene();
            dieSoundPlayer.PlayRandomSound();
        }
    }

    #region IPlayerActions Implementation

    public void OnMove(InputAction.CallbackContext context)
    {
        // Handle movement input
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Handle look input
        Vector2 lookInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Attack performed!");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Interact performed!");
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Crouch performed!");
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump performed!");
            // Your jump code goes here

            if (groundCollider.IsTouchingLayers())
            {
                jumping = true;
                jumpSoundPlayer.PlayRandomSound();
                timeSinceJump = 0f;
            }
        }
        if (context.canceled)
        {
            jumping = false;
        }
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnWaterEquip(InputAction.CallbackContext context)
    {
        wardrobe.ChangeClothesTo(ElementalType.Water);
    }

    public void OnFireEquip(InputAction.CallbackContext context)
    {
        wardrobe.ChangeClothesTo(ElementalType.Fire);
    }

    public void OnGasEquip(InputAction.CallbackContext context)
    {
        wardrobe.ChangeClothesTo(ElementalType.Gas);
    }

    public void OnColdEquip(InputAction.CallbackContext context)
    {
        wardrobe.ChangeClothesTo(ElementalType.Cold);
    }

    #endregion
}
