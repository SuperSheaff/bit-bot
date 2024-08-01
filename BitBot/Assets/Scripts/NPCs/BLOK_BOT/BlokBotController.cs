using System.Collections;
using UnityEngine;

public class BlokBotController : MonoBehaviour
{
    public StateMachine<BlokBotController> stateMachine;
    public BlokBotAsleepState asleepState;
    public BlokBotReactionState reactionState;
    public BlokBotBlockingState blockingState;
    public GameObject barrier; // The barrier object to activate/deactivate
    public LayerMask playerLayer;
    public Transform headTransform;
    public BoxCollider detectionCollider; // The box collider for detection
    public Transform playerTransform; // Reference to the player's transform

    [HideInInspector] public float timeSinceLastSeenPlayer;
    [HideInInspector] public Animator animator;
    [HideInInspector] public bool isPlayerInDetectionZone = false; // Player detection state
    public ParticleSystem blockingParticles; // Particle system for blocking state
    public ParticleSystem sleepingParticles; // Particle system for blocking state
    public ParticleSystem alertParticles; // Particle system for blocking state
    public PlayerController playerController;
    
    void Start()
    {
        stateMachine = new StateMachine<BlokBotController>(false);

        asleepState = new BlokBotAsleepState(this);
        reactionState = new BlokBotReactionState(this);
        blockingState = new BlokBotBlockingState(this);

        stateMachine.Initialize(asleepState);

        animator = GetComponent<Animator>();

        if (blockingParticles != null)
        {
            blockingParticles.Stop();
        }

        // Find the player by tag and store the transform
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found in the scene");
        }

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        stateMachine.Update();
    }

    void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    public void SetPlayerInDetectionZone(bool state)
    {
        isPlayerInDetectionZone = state;
    }

    // Triggers an animation event
    public void AnimationEvent(string eventName)
    {
        stateMachine.CurrentState.OnAnimationEvent(eventName);
    }
}
