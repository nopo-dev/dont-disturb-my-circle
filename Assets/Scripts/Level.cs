using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct EnemyWave
{
    public float SpawnTime;
    public GameObject EnemyType;
    public int NumberToSpawn;
    public float SpawnInterval;
    public Vector3 SpawnPosition;

}

public class Level : MonoBehaviour
{
    public EnemyWave[] enemyWaves;
}
