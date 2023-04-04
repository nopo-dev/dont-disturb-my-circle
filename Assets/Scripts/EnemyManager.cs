using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameStateManager _gameStateManager;
    [SerializeField] ObjectPooler _objectPooler;
    [SerializeField] GameObject[] _bosses;

    [SerializeField] private Level[] _levels;
    public Level[] Levels
    {
        get { return _levels; }
    }

    public void SetUpLevel(int level)
    {
        PrePoolEnemies(_levels[level - 1]);
    }

    public void StartLevel(int level)
    {
        foreach (EnemyWave enemyWave in _levels[level - 1].enemyWaves)
        {
            StartCoroutine(SpawnWave(enemyWave));
        }
    }

    IEnumerator SpawnWave(EnemyWave enemyWave)
    {
        yield return new WaitForSeconds(enemyWave.SpawnTime);
        for (int i = 0; i < enemyWave.NumberToSpawn; i++)
        {
            GameObject spawnedEnemy = ObjectPooler.SharedInstance.GetPooledObject(enemyWave.EnemyType.tag);
            if (spawnedEnemy != null)
            {
                spawnedEnemy.GetComponent<EnemyController>().StartPosition = enemyWave.SpawnPosition;
                spawnedEnemy.SetActive(true);
            }
            yield return new WaitForSeconds(enemyWave.SpawnInterval);
        }
    }

    private List<ObjectPoolItem> _enemiesToPrePool = new List<ObjectPoolItem>();
    private void PrePoolEnemies(Level level)
    {
        foreach (EnemyWave enemyWave in level.enemyWaves)
        {
            bool alreadyInPool = false;
            foreach (ObjectPoolItem poolItem in _enemiesToPrePool)
            {
                if (enemyWave.EnemyType.CompareTag(poolItem._objectToPool.tag))
                {
                    alreadyInPool = true;
                }
            }
            if (!alreadyInPool)
                _objectPooler._itemsToPool.Add(
                    new ObjectPoolItem(enemyWave.EnemyType, 5, true));
        }

        _objectPooler.PoolObjects();
    }
}
