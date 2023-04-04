using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject[] _playerHearts;
    [SerializeField] private GameObject[] _playerSpecials;
    [SerializeField] private GameObject _score;
    [SerializeField] private GameObject _combo;
    [SerializeField] private GameObject _comboBuild;
    [SerializeField] private GameObject _circleProgress;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _shopMenu;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _loadingProgress;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _gameWinScreen;
    [SerializeField] private GameObject _gameOverScore;
    [SerializeField] private GameObject _gameWinScore;
    [SerializeField] private GameObject _level;


    //shop ui
    [SerializeField] private GameObject _damageUpgrade;
    [SerializeField] private GameObject _spreadUpgrade;
    [SerializeField] private GameObject _speedUpgrade;
    [SerializeField] private GameObject _circleUpgrade;
    [SerializeField] private GameObject _lifeCost;
    [SerializeField] private GameObject _specialCost;
    [SerializeField] private GameObject[] _lifePips;
    [SerializeField] private GameObject[] _specialPips;
    [SerializeField] private GameObject _shopScore;

    public void SetShopDamage(int damageLevel)
    {
        if (damageLevel < StatsHelper.damageCosts.Length)
            _damageUpgrade.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text
                = "" + StatsHelper.damageCosts[damageLevel];
        else
            _damageUpgrade.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text
                = " - ";
        _damageUpgrade.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text
            = "lvl. " + (damageLevel + 1);
    }

    public void SetShopSpread(int spreadLevel)
    {
        if (spreadLevel < StatsHelper.numSidesCosts.Length)
            _spreadUpgrade.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text
                = "" + StatsHelper.numSidesCosts[spreadLevel];
        else
            _spreadUpgrade.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text
                = " - ";
        _spreadUpgrade.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text
            = "lvl. " + (spreadLevel + 1);

    }

    public void SetShopSpeed(int speedLevel)
    {
        if (speedLevel < StatsHelper.fireRateCosts.Length)
            _speedUpgrade.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text
                = "" + StatsHelper.fireRateCosts[speedLevel];
        else
            _speedUpgrade.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text
                = " - ";
        _speedUpgrade.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text
            = "lvl. " + (speedLevel + 1);
    }

    public void SetCircleShop(int circleLevel)
    {
        if (circleLevel < StatsHelper.scaleCosts.Length)
            _circleUpgrade.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text
                = "" + StatsHelper.scaleCosts[circleLevel];
        else
            _circleUpgrade.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text
                = " - ";
        _circleUpgrade.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text
            = "lvl. " + (circleLevel + 1);
    }

    public void SetLifeShop(int numLives)
    {
        _lifeCost.GetComponent<TextMeshProUGUI>().text = "" + StatsHelper.lifeCost;
    }

    public void SetSpecialShop(int numSpecials)
    {
        _specialCost.GetComponent<TextMeshProUGUI>().text = "" + StatsHelper.specialCost;
    }

    public void SetShopScore(int score)
    {
        _shopScore.GetComponent<TextMeshProUGUI>().text = "" + score;
    }

    public void SetVictoryDefeatScore(int score)
    {
        _gameWinScore.GetComponent<TextMeshProUGUI>().text = "" + score;
        _gameOverScore.GetComponent<TextMeshProUGUI>().text = "" + score;
    }

    private void Awake()
    {
        _comboBuild.GetComponent<Slider>().maxValue = StatsHelper.comboBuildup;
        _comboBuild.GetComponent<Slider>().value = 0;
    }

    public void ShowGameWinScreen()
    {
        _gameWinScreen.SetActive(true);
    }

    public void HideGameWinScreen()
    {
        _gameWinScreen.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        _gameOverScreen.SetActive(true);
    }

    public void HideGameOverScreen()
    {
        _gameOverScreen.SetActive(false);
    }

    public void ShowGameUI()
    {
        _gameUI.SetActive(true);
    }

    public void HideGameUI()
    {
        _gameUI.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        _pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        _pauseMenu.SetActive(false);
    }

    public void ShowShopMenu()
    {
        _shopMenu.SetActive(true);
    }

    public void HideShopMenu()
    {
        _shopMenu.SetActive(false);
    }

    public void SetPlayerMaxHP(int hp)
    {
        for (int i = 0; i < _playerHearts.Length; i++)
        {
            if (i < hp)
            {
                _playerHearts[i].SetActive(true);
            }
            else
            {
                _playerHearts[i].SetActive(false);
            }
        }
    }

    public void SetPlayerCurrentHP(int hp)
    {
        for (int i = 0; i < _playerHearts.Length; i++)
        {
            if (i < hp)
            {
                _playerHearts[i].transform.GetChild(0).gameObject.SetActive(true);
                _playerHearts[i].transform.GetChild(1).gameObject.SetActive(true);
                _lifePips[i].transform.GetChild(0).gameObject.SetActive(true);
                _lifePips[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                _playerHearts[i].transform.GetChild(0).gameObject.SetActive(true);
                _playerHearts[i].transform.GetChild(1).gameObject.SetActive(false);
                _lifePips[i].transform.GetChild(0).gameObject.SetActive(true);
                _lifePips[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void SetPlayerMaxSpecials(int specials)
    {
        for (int i = 0; i < _playerSpecials.Length; i++)
        {
            if (i < specials)
            {
                _playerSpecials[i].SetActive(true);
            }
            else
            {
                _playerSpecials[i].SetActive(false);
            }
        }
    }

    // increase by 1
    public void SetPlayerCurrentSpecials(int specials)
    {
        for (int i = 0; i < _playerSpecials.Length; i++)
        {
            if (i < specials)
            {
                _playerSpecials[i].transform.GetChild(0).gameObject.SetActive(true);
                _playerSpecials[i].transform.GetChild(1).gameObject.SetActive(true);
                _specialPips[i].transform.GetChild(0).gameObject.SetActive(true);
                _specialPips[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                _playerSpecials[i].transform.GetChild(0).gameObject.SetActive(true);
                _playerSpecials[i].transform.GetChild(1).gameObject.SetActive(false);
                _specialPips[i].transform.GetChild(0).gameObject.SetActive(true);
                _specialPips[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
    
    public void UpdateScore(int score)
    {
        _score.GetComponent<TextMeshProUGUI>().text = "" + score;
    }

    public void UpdateCombo(int combo)
    {
        _combo.GetComponent<TextMeshProUGUI>().text = combo + "x";
    }

    public void BuildCombo(int comboBuildup)
    {
        _comboBuild.GetComponent<Slider>().value = comboBuildup;
    }

    public void PauseCircleProgress()
    {
        _circleProgress.GetComponent<Animator>().SetFloat("Speed", 0);
    }
    
    public void ResetCircleProgress()
    {
        _circleProgress.GetComponent<Animator>().Rebind();
        _circleProgress.GetComponent<Animator>().Update(0f);
        _circleProgress.GetComponent<Animator>().SetFloat("Speed", 1);
    }

    public void LoadTitleScreen()
    {
        StartCoroutine(LoadSceneAsync(0));
    }

    public void ReloadGameScene()
    {
        StartCoroutine(LoadSceneAsync(1));
    }

    public void DisplayLevel(int level)
    {
        StartCoroutine(BrieflyDisplayLevel(level));
    }

    IEnumerator BrieflyDisplayLevel(int level)
    {
        _level.SetActive(true);
        _level.GetComponent<TextMeshProUGUI>().text = "Level " + level;
        yield return new WaitForSeconds(2f);
        _level.SetActive(false);
    }

    IEnumerator LoadSceneAsync(int index)
    {
        _loadingScreen.SetActive(true);
        AsyncOperation load = SceneManager.LoadSceneAsync(index);
        while(!load.isDone)
        {
            _loadingProgress.GetComponent<Slider>().value = load.progress / 0.9f;
            yield return null;
        }
        _loadingScreen.SetActive(false);
    }
}
