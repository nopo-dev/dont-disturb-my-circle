using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletFury;
using BulletFury.Data;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private EnemyBehaviorData _enemyBehaviorData;
    [SerializeField] private GameObject _target;

    private Animator _animator;
    private bool _isDead;
    private int _phase;
    private float _timeInPhase;
    private Vector3 _rotation;
    // private bool _inPhase;
    private bool _aggro;
    public bool Aggro
    {
        get { return _aggro; }
        set { _aggro = value; }
    }

    private Vector3 _startPosition;
    public Vector3 StartPosition
    {
        get { return _startPosition; }
        set { _startPosition = value; }
    }

    private int _currentHP;
    public int CurrentHP
    {
        get { return _currentHP; }
        set { _currentHP = value; }
    }

    private Vector3 _lookDirection;
    public Vector3 LookDirection
    {
        get { return _lookDirection; }
        set { _lookDirection = value; }
    }

    private bool _aimAtTarget;
    public bool AimAtTarget
    {
        get { return _aimAtTarget; }
        set { _aimAtTarget = value; }
    }


    public float AimRotationSpeed { get; set; }

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    // make sure phaselength > 0
    IEnumerator PerformPhase(BehaviorPhase phase)
    {
        float phaseLength = phase._phaseLength;
        float movementSpeed = Mathf.Clamp(phase._speed,
            -_enemyData._movementSpeed, _enemyData._movementSpeed);
        float angularVelocity = phase._angularVelocity;
        bool moveRelativeToTarget = phase._moveRelativeToTarget;
        AimRotationSpeed = phase._aimRotationSpeed;
        AimAtTarget = phase._aimAtTarget;
        Vector3 trackRotation = _rotation;

        Aggro = phase._aggro;
        if (!_isDead)
            gameObject.GetComponent<BulletCollider>().enabled = !phase._invulnerable;
        
        if (_isDead)
            Aggro = false;

        while (_timeInPhase <= phaseLength && !_isDead)
        {
            // update rotation
            if (moveRelativeToTarget)
            {
                _rotation = (_target.transform.position - transform.position).normalized;
            }
            if (Aggro)
                LookDirection = (_target.transform.position - transform.position).normalized;
            if (angularVelocity != 0f)
            {
                _rotation = (Quaternion.Euler(0f, 0f,
                    angularVelocity * Time.deltaTime) * _rotation);
                if (moveRelativeToTarget)
                {
                    trackRotation = (Quaternion.Euler(0f, 0f,
                        angularVelocity * Time.deltaTime) * trackRotation);
                    _rotation = (_rotation + trackRotation).normalized;
                }
            }

            // update position
            Vector3 movement = movementSpeed * Time.deltaTime * _rotation;
            transform.position += movementSpeed * Time.deltaTime * _rotation;

            // update time
            _timeInPhase += Time.deltaTime;
            
            yield return null;
        }

        if (!phase._lastPhase)
            IncrementPhase(phase._loopPhase);
    }

    private void IncrementPhase(bool loopPhase)
    {
        _timeInPhase = 0f;
        _phase = (_phase + (loopPhase ? 0 : 1)) % _enemyBehaviorData._behaviorPhases.Length;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Stage") && gameObject.activeSelf)
        {
            // gameObject.SetActive(false);
            Aggro = false;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(DelayedDeactivate());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerSpecial") && gameObject.activeSelf)
        {
            if (!gameObject.CompareTag("BigSlime") && !gameObject.CompareTag("PinkSlimeBoss")
                 && !gameObject.CompareTag("BlackSlimeBoss"))
            {
                _animator.SetTrigger("Die");
                gameObject.GetComponent<BulletCollider>().enabled = false;
                gameObject.GetComponent<CircleCollider2D>().enabled = false;
                _isDead = true;
                Aggro = false;
                _gameStateManager.AddScoreKill(_enemyData._score);
                StartCoroutine(DisableAfterDeath());
            }
        }
    }

    IEnumerator DelayedDeactivate()
    {
        if (gameObject.CompareTag("BigSlime") || gameObject.CompareTag("PinkSlimeBoss")
             || gameObject.CompareTag("BlackSlimeBoss"))
        {
            yield return new WaitForSeconds(2.5f);
            _gameStateManager.LevelWin();
        }
        if (gameObject.GetComponent<EnemyFire>() != null)
        {
            yield return new WaitForSeconds(gameObject.GetComponent<EnemyFire>().Lifetime);
        }
        if (gameObject.GetComponentInChildren<EnemyFire>() != null)
        {
            yield return new WaitForSeconds(gameObject.GetComponentInChildren<EnemyFire>().Lifetime);
        }
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _phase = 0;
        _timeInPhase = 0f;
        // _inPhase = false;
        _rotation = Vector3.down;
        CurrentHP = (int)_enemyData._maxHP;
        _isDead = false;
        gameObject.GetComponent<BulletCollider>().enabled = true;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        transform.position = StartPosition;
        if (gameObject.CompareTag("BigSlime") || gameObject.CompareTag("PinkSlimeBoss")
            || gameObject.CompareTag("BlackSlimeBoss"))
        {
            _audioManager.FadeOut("MainMusic");
            StartCoroutine(DelayStartBossMusic());
        }
    }

    IEnumerator DelayStartBossMusic()
    {
        yield return new WaitForSeconds(1f);
        _audioManager.PlaySound("BossMusicIntro");
        _audioManager.PlayDelayedSound("BossMusicLoop", _audioManager.GetLength("BossMusicIntro"));
    }

    public void TakeDamage(BulletContainer container, BulletCollider collider)
    {
        if (!_enemyBehaviorData._behaviorPhases[_phase]._invulnerable && !_isDead)
        {
            CurrentHP -= (int)container.Damage;
            _audioManager.PlaySound("SlimeHit");
            _gameStateManager.BuildCombo();
            _gameStateManager.AddScoreHit();
            if (CurrentHP <= 0)
            {
                _animator.SetTrigger("Die");
                gameObject.GetComponent<BulletCollider>().enabled = false;
                gameObject.GetComponent<CircleCollider2D>().enabled = false;
                _isDead = true;
                Aggro = false;
                _gameStateManager.AddScoreKill(_enemyData._score);
                if (gameObject.CompareTag("BigSlime") || gameObject.CompareTag("PinkSlimeBoss")
                    || gameObject.CompareTag("BlackSlimeBoss"))
                {
                    _target.GetComponent<PlayerController>().UseSpecial(false);
                    _audioManager.FadeOut("BossMusicIntro");
                    _audioManager.FadeOut("BossMusicLoop");
                }
                StartCoroutine(DisableAfterDeath());
            }
        }
    }

    IEnumerator DisableAfterDeath()
    {
        _audioManager.PlaySound("SlimeDeath" + Random.Range(1, 3));
        yield return new WaitForSeconds(1f);
        if (gameObject.CompareTag("BigSlime") || gameObject.CompareTag("PinkSlimeBoss")
            || gameObject.CompareTag("BlackSlimeBoss"))
            yield return new WaitForSeconds(1.75f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        // gameObject.SetActive(false);
        StartCoroutine(DelayedDeactivate());
    }
    private void Update()
    {
        if (_timeInPhase == 0f)
        {
            StartCoroutine(PerformPhase(_enemyBehaviorData._behaviorPhases[_phase]));
        }
    }
}
