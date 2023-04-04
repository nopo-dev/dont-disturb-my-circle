using UnityEngine;

public class MagicCircleController : MonoBehaviour
{
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private GameUIHandler _gameUIHandler;
    private int _scaleLevel;
    public int ScaleLevel
    {
        get { return _scaleLevel; }
        set
        {
            _scaleLevel = value;
            gameObject.transform.localScale = new Vector3(StatsHelper.scaleLevels[_scaleLevel],
                StatsHelper.scaleLevels[_scaleLevel], StatsHelper.scaleLevels[_scaleLevel]);
            _gameUIHandler.SetCircleShop(_scaleLevel);
        }
    }

    public bool CircleActive()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("MagicCircleActive");
    }

    private Animator _animator;

    public void SetAnimatorBool(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        ScaleLevel = 0;
    }
}
