using System.Collections;
using UnityEngine;

public class BlokBotController : MonoBehaviour
{
    public StateMachine<BlokBotController> stateMachine;
    public BlokBotAsleepState asleepState;
    public BlokBotReactionState reactionState;
    public BlokBotBlockingState blockingState;
    public GameObject barrier;
    public LayerMask playerLayer;
    public Transform headTransform;
    public BoxCollider detectionCollider;
    public Transform playerTransform;

    [HideInInspector] public float timeSinceLastSeenPlayer;
    [HideInInspector] public Animator animator;
    [HideInInspector] public bool isPlayerInDetectionZone = false;
    public ParticleSystem blockingParticles;
    public ParticleSystem sleepingParticles;
    public ParticleSystem alertParticles;
    public PlayerController playerController;

    void Awake()
    {
        InitializeComponents();
        InitializeStateMachine();
    }

    void OnEnable()
    {
        if (blockingParticles != null)
        {
            blockingParticles.Stop();
        }

        // Ensure the state machine is not null before changing states
        if (stateMachine != null)
        {
            stateMachine.ChangeState(asleepState);
        }
    }

    void Update()
    {
        stateMachine.Update();
    }

    void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    void OnDisable()
    {
        StopAllSounds();
    }

    public void SetPlayerInDetectionZone(bool state)
    {
        if (playerController != null && playerController.IsAlive)  // Ensure the player is alive before setting detection state
        {
            isPlayerInDetectionZone = state;
        }
        else
        {
            isPlayerInDetectionZone = false;
        }
    }

    public void ResetDetection()
    {
        isPlayerInDetectionZone = false;
        timeSinceLastSeenPlayer = 0;
        stateMachine.ChangeState(asleepState);
    }

    public void AnimationEvent(string eventName)
    {
        stateMachine.CurrentState.OnAnimationEvent(eventName);
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("Player not found in the scene");
        }
    }

    private void InitializeStateMachine()
    {
        stateMachine = new StateMachine<BlokBotController>(false);

        asleepState = new BlokBotAsleepState(this);
        reactionState = new BlokBotReactionState(this);
        blockingState = new BlokBotBlockingState(this);

        stateMachine.Initialize(asleepState);
    }

    private void StopAllSounds()
    {
        if (SoundManager.instance != null)
        {
            // Add here all the sound names that should be stopped when the object is disabled
            SoundManager.instance.StopSound("blok_bot_snore");
            SoundManager.instance.StopSound("blok_bot_activate");
            SoundManager.instance.StopSound("blok_bot_laser");
            SoundManager.instance.StopSound("blok_bot_alert");
            // Add other sounds related to BlokBot here
        }
    }
}
