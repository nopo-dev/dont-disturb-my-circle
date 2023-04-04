using UnityEngine;

[System.Serializable]
public struct BehaviorPhase
{
    public BehaviorPhase(float phaseLength, bool loopPhase, float speed,
        float angularVelocity, bool aggro, bool aimAtTarget, bool lastPhase,
        bool invulnerable, bool moveRelativeToTarget, float aimRotationSpeed)
    {
        _phaseLength = phaseLength;
        _loopPhase = loopPhase;
        _speed = speed;
        _angularVelocity = angularVelocity;
        _moveRelativeToTarget = moveRelativeToTarget;
        _aggro = aggro;
        _aimAtTarget = aimAtTarget;
        _aimRotationSpeed = aimRotationSpeed;
        _lastPhase = lastPhase;
        _invulnerable = invulnerable;
    }

    public float _phaseLength;
    public bool _loopPhase;
    public float _speed;
    public float _angularVelocity;
    public bool _invulnerable;
    public bool _aggro;
    public bool _aimAtTarget;
    public bool _moveRelativeToTarget;
    public float _aimRotationSpeed;
    public bool _lastPhase;
}

[CreateAssetMenu(menuName = "EnemyBehaviorData")]
public class EnemyBehaviorData : ScriptableObject
{
    public BehaviorPhase[] _behaviorPhases;
}
