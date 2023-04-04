using UnityEngine;
using BulletFury;
using BulletFury.Data;
using System.Collections;

public class EnemyFire : MonoBehaviour
{
    [SerializeField] private BulletManager _bulletManager;
    [SerializeField] private EnemyController _enemyController;

    [SerializeField] private BulletSettings _bulletSettings;
    [SerializeField] private SpawnSettings _spawnSettings;

    public float Lifetime
    {
        get { return _bulletSettings.Lifetime; }
    }

    public float FireRate
    {
        get { return _spawnSettings.FireRate; }
        set { _spawnSettings.FireRate = value; }
    }

    public int NumSides
    {
        get { return _spawnSettings.NumSides; }
        set { _spawnSettings.NumSides = value; }
    }

    public float Arc
    {
        get { return _spawnSettings.Arc; }
        set { _spawnSettings.Arc = value; }
    }

    private void Awake()
    {
        _bulletSettings = _bulletManager.GetBulletSettings().Clone<BulletSettings>();
        _spawnSettings = _bulletManager.GetSpawnSettings().Clone<SpawnSettings>();

        _bulletManager.SetBulletSettings(_bulletSettings);
        _bulletManager.SetSpawnSettings(_spawnSettings);
    }

    private void Update()
    {
        if (_bulletManager == null)
            return;
        
        if (_enemyController.Aggro)
        {
            if (_enemyController.AimAtTarget)
                _bulletManager.Spawn(transform.position, _enemyController.LookDirection);
            else
            {
                _bulletManager.Spawn(transform.position, -transform.up);
                transform.Rotate(Vector3.forward, _enemyController.AimRotationSpeed * Time.smoothDeltaTime);
            }
        }

    }
}