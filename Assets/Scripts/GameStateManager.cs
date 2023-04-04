using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameState
{
    public GameState(bool paused, int stage, int level)
    {
        Paused = paused;
        Stage = stage;
        Level = level;
    }
    // 1 = paused, 0 = in play
    public bool Paused { get; set; }
    // 0 = level in progress, 2 = level done/shop, -1 = level failed,
    // 3 = game complete, 1 = boss in progress
    public int Stage { get; set; }
    public int Level { get; set; }
}

public class GameStateManager : MonoBehaviour
{
    [SerializeField] PlayerController _playerController;
    [SerializeField] MagicCircleController _magicCircleController;
    [SerializeField] GameUIHandler _gameUIHandler;
    [SerializeField] AudioManager _audioManager;
    [SerializeField] EnemyManager _enemyManager;


    private GameState _gameState;
    private Transform _playerTransform;
    private Transform _magicCircleTransform;
    private int _score;
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            _gameUIHandler.UpdateScore(_score);
            _gameUIHandler.SetShopScore(_score);
        }
    }

    public void AddScoreHit()
    {
        Score += Combo;
    }

    public void AddScoreKill(int score)
    {
        Score += Combo * score;
    }

    private int _combo;
    public int Combo
    {
        get { return _combo; }
        set
        {
            _combo = value;
            _gameUIHandler.UpdateCombo(_combo);
        }
    }

    private float _isComboBuildDropping;
    private float _timeSinceComboBuildDrop;
    private float _timeSinceComboBuild;
    private int _comboBuild;
    public int ComboBuild
    {
        get { return _comboBuild; }
        set
        {
            _comboBuild = value;
            _gameUIHandler.BuildCombo(_comboBuild);
        }
    }

    public void BuildCombo()
    {
        _timeSinceComboBuild = 0f;
        if (Combo < StatsHelper.maxCombo)
        {
            ComboBuild++;
            if (_comboBuild >= StatsHelper.comboBuildup)
            {
                Combo++;
                ComboBuild = 0;
            }
        }
    }

    public void ResetCombo()
    {
        Combo = 1;
        ComboBuild = 0;
    }

    public void BuyDamage()
    {
        if (_playerController.DamageLevel >= StatsHelper.damageLevels.Length - 1)
            return;
        if (Score >= StatsHelper.damageCosts[_playerController.DamageLevel])
        {
            Score -= StatsHelper.damageCosts[_playerController.DamageLevel];
            _playerController.DamageLevel++;
        }
    }

    public void BuySpread()
    {
        if (_playerController.NumSidesLevel >= StatsHelper.numSidesLevels.Length - 1)
            return;
        if (Score >= StatsHelper.numSidesCosts[_playerController.NumSidesLevel])
        {
            Score -= StatsHelper.numSidesCosts[_playerController.NumSidesLevel];
            _playerController.NumSidesLevel++;
        }
    }

    public void BuySpeed()
    {
        if (_playerController.FireRateLevel >= StatsHelper.fireRateLevels.Length - 1)
            return;
        if (Score >= StatsHelper.fireRateCosts[_playerController.FireRateLevel])
        {
            Score -= StatsHelper.fireRateCosts[_playerController.FireRateLevel];
            _playerController.FireRateLevel++;
        }
    }

    public void BuyCircle()
    {
        if (_magicCircleController.ScaleLevel >= StatsHelper.scaleLevels.Length - 1)
            return;
        if (Score >= StatsHelper.scaleCosts[_magicCircleController.ScaleLevel])
        {
            Score -= StatsHelper.scaleCosts[_magicCircleController.ScaleLevel];
            _magicCircleController.ScaleLevel++;
        }
    }

    public void BuyLife()
    {
        if (Score >= StatsHelper.lifeCost &&
            _playerController.CurrentHP < StatsHelper.maxHP)
        {
            Score -= StatsHelper.lifeCost;
            _playerController.CurrentHP++;
        }
    }

    public void BuySpecial()
    {
        if (Score >= StatsHelper.specialCost &&
            _playerController.CurrentSpecials < StatsHelper.maxSpecials)
        {
            Score -= StatsHelper.specialCost;
            _playerController.CurrentSpecials++;
        }
    }

    public bool CanPlayerAct()
    {
        return (_gameState.Stage == 0 || _gameState.Stage == 1) && _playerController.CurrentHP != 0;
    }

    public bool CanPlayerFire()
    {
        return CanPlayerAct() && PlayerInsideCircle() &&
            _magicCircleController.CircleActive();
    }

    public bool PlayerInsideCircle()
    {
        return Vector3.Distance(_playerTransform.position + 0.5f * Vector3.down,
            _magicCircleTransform.position) <= 1.95f *
            StatsHelper.scaleLevels[_magicCircleController.ScaleLevel];
    }

    public void LevelWin()
    {
        if (_gameState.Level != 3)
        {
            _gameState.Stage = 2;
            _gameUIHandler.HideGameUI();
            _gameUIHandler.ShowShopMenu();
        }
        else
            ContinueGame();
    }

    public void ExitShop()
    {
        _gameUIHandler.HideShopMenu();
        _gameUIHandler.ShowGameUI();
        ContinueGame();
    }

    private void ContinueGame()
    {
        _gameState.Level++;
        if (_gameState.Level - 1 == _enemyManager.Levels.Length)
        {
            _gameState.Stage = 3;
            GameWin();
        }
        else
        {
            StartLevel();
        }

    }

    private void GameWin()
    {
        if (_gameState.Stage == 3)
        {
            PauseGame();
            _gameUIHandler.HideGameUI();
            _gameUIHandler.SetVictoryDefeatScore(Score);
            _gameUIHandler.ShowGameWinScreen();
            _audioManager.PlaySound("Victory");
        }
    }

    private void GameOver()
    {
        if (_gameState.Stage == -1)
        {
            PauseGame();
            _gameUIHandler.HideGameUI();
            _gameUIHandler.SetVictoryDefeatScore(Score);
            _gameUIHandler.ShowGameOverScreen();
            _audioManager.PlaySound("Defeat");
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        _gameState.Paused = true;
        _gameUIHandler.ShowPauseMenu();
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        _gameState.Paused = false;
        _gameUIHandler.HidePauseMenu();
    }

    public void PlayerHPUpdate()
    {
        if (_playerController.CurrentHP <= 0)
        {
            _gameState.Stage = -1;
            StartCoroutine(DelayedGameOver());
            _gameUIHandler.PauseCircleProgress();
        }
    }

    IEnumerator DelayedGameOver()
    {
        _audioManager.FadeOut("BossMusicLoop");
        _audioManager.FadeOut("BossMusicIntro");
        _audioManager.FadeOut("MainMusic");
        yield return new WaitForSeconds(2f);
        GameOver();
    }

    private void StartLevel()
    {
        _gameState.Stage = 0;
        _gameState.Paused = false;
        _gameUIHandler.DisplayLevel(_gameState.Level);
        _enemyManager.SetUpLevel(_gameState.Level);
        _enemyManager.StartLevel(_gameState.Level);
        _gameUIHandler.DisplayLevel(_gameState.Level);
        _audioManager.PlaySound("MainMusic");
    }

    private void Awake()
    {
        _gameState = new GameState(false, 0, 1);

        _playerTransform = _playerController.gameObject.transform;
        _playerTransform.position = new Vector3(0f, -6.125f, 0f);
        _magicCircleTransform = _magicCircleController.gameObject.transform;
        Combo = 1;
    }

    private void Start()
    {
        StartLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (_gameState.Paused)
                UnpauseGame();
            else
                PauseGame();
        }
        _magicCircleController.SetAnimatorBool("PlayerInside", PlayerInsideCircle());
        if (CanPlayerAct())
        {
            _timeSinceComboBuild += Time.deltaTime;

            if (_timeSinceComboBuild >= StatsHelper.comboBuildDropOff)
            {
                if (_timeSinceComboBuildDrop >= StatsHelper.comboBuildDropOff)
                {
                    ComboBuild--;
                    _timeSinceComboBuildDrop = 0;
                }
                _timeSinceComboBuildDrop += Time.deltaTime;
            }
            if (_timeSinceComboBuild >= StatsHelper.comboDropOff)
                ResetCombo();
        }
    }
}
