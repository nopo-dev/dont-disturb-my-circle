using UnityEngine;

[CreateAssetMenu(menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public int _score;
    public int _maxHP;
    public float _movementSpeed;
}
