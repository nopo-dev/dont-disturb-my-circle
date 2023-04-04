using UnityEngine;
using BulletFury;
using BulletFury.Data;
using System.Collections;

public class PlayerBasicBulletFire : MonoBehaviour
{
    [SerializeField] private BulletManager _bulletManager;
    [SerializeField] private PlayerController _playerController;

    [SerializeField] private BulletSettings _bulletSettings;
    [SerializeField] private SpawnSettings _spawnSettings;

    public float Damage
    {
        get { return _bulletSettings.Damage; }
        set { _bulletSettings.Damage = value; }
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

        Damage = StatsHelper.damageLevels[_playerController.DamageLevel];
        FireRate = StatsHelper.fireRateLevels[_playerController.FireRateLevel];
        NumSides = StatsHelper.numSidesLevels[_playerController.NumSidesLevel];
        Arc = StatsHelper.arcLevels[_playerController.ArcLevel];
    }

    private void Update()
    {
        if (_bulletManager == null)
            return;
        
        if (_playerController.CanAct() &&
            _playerController.CanFire() && _playerController.IsFiring())
        {
            if (_playerController.Focused)
                Arc = StatsHelper.focusArcLevels[_playerController.ArcLevel];
            else
                Arc = StatsHelper.arcLevels[_playerController.ArcLevel];

            _bulletManager.Spawn(transform.position, _playerController.LookDirection);
            if (_spawnSettings.FireRate > 0)
                _playerController.AnimationSpeed = 1f / _spawnSettings.FireRate;
        }
        else
        {
            _playerController.AnimationSpeed = 1f;
        }

    }
}