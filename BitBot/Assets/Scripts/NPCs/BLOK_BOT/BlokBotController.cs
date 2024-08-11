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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found in the scene");
        }

        playerController = player.GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        if (blockingParticles != null)
        {
            blockingParticles.Stop();
        }
        stateMachine.ChangeState(asleepState);
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
}
