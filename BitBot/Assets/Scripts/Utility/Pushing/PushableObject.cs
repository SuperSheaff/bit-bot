using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public bool IsBeingPushed { get; private set; } = false;
    public float pushDistanceCheck = 2f;
    public void StartPushing()
    {
        IsBeingPushed = true;
    }

    public void StopPushing()
    {
        IsBeingPushed = false;
    }
}
