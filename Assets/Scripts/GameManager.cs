using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus
{
    next,
    play,
    gameover,
    win,
}

public class GameManager: Singleton<GameManager>
{
    [SerializeField]
    private int totalWaves = 10;
    [SerializeField]
    private Text totalMoneyTxt;
    [SerializeField]
    private Text waveNumberTxt;
    [SerializeField]
    private Text escapedTxt;
    [SerializeField]
    private Text playBtnTxt;
    [SerializeField]
    private Button playBtn;


    [SerializeField]
    private Transform _spawnPoint;
    [SerializeField]
    private GameObject[] _enemies;
    [SerializeField]
    private int _totalEnemies; // Oyunda cikacak olan toplam dusman sayisi
    [SerializeField]
    private int _enemiesPerSpawn; // Her bir dalgada kac dusman bulunacaginin sayisi
    [SerializeField]
    private float _spawnDelay = 1;

    public List<Enemy> _enemyList;

    private int waveNumber = 0;
    private int totalMoney = 0;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int whichEnemiesToSpawn = 0;
    private gameStatus currentState = gameStatus.play;

    private AudioSource _audioSource;

    Tower tower;


    public AudioSource AudioSource
    {
        get
        {
            return _audioSource;
        }
    }

    public int TotalMoney
    {
        get
        {
            return totalMoney;
        }
        set
        {
            totalMoney = value;
            totalMoneyTxt.text = totalMoney.ToString();
        }
    }

    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }

    public int RoundEscaped
    {
        get
        {
            return roundEscaped;
        }
        set
        {
            roundEscaped = value;
        }
    }

    public int TotalKilled
    {
        get
        {
            return totalKilled;
        }
        set
        {
            totalKilled = value;
        }
    }

    private void Awake()
    {
        _enemyList = new List<Enemy>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        playBtn.gameObject.SetActive(false);
        ShowMenu();
        tower = new Tower();
    }

    private void Update()
    {
        HandleEscape();
    }

    IEnumerator Spawn()
    {
        if (_enemyList.Count < _totalEnemies && _enemiesPerSpawn > 0)
        {
            for (int i = 0; i < _enemiesPerSpawn; i++)
            {
                if (_enemyList.Count < _totalEnemies)
                {
                    GameObject newEnemy = Instantiate(_enemies[Random.Range(0,whichEnemiesToSpawn)], _spawnPoint.position, _spawnPoint.rotation) as GameObject;
                }
                yield return new WaitForSeconds(_spawnDelay);
            }
            StartCoroutine(Spawn());
        }
    }


    public void RegisterEnemy(Enemy enemy)
    {
        _enemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        _enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    private void DestroyAllEnemies()
    {
        foreach(Enemy enemy in _enemyList)
        {
            Destroy(enemy.gameObject);
        }
        _enemyList.Clear();
    }

    public void AddMoney(int amountMoney)
    {
        TotalMoney += amountMoney;
    }

    public void SubtractMoney(int amountMoney)
    {
        TotalMoney -= amountMoney;
    }

    private void SetCurrentGameState()
    {
        // gameover state
        if(TotalEscaped >= 10)
        {
            currentState = gameStatus.gameover;
        }
        // play state
        else if(waveNumber == 0 && (RoundEscaped + TotalKilled == 0))
        {
            currentState = gameStatus.play;
        }
        // win state
        else if(waveNumber >= totalWaves)
        {
            currentState = gameStatus.win;
        }
        // next state
        else
        {
            currentState = gameStatus.next;
        }
    }

    public void isWaveOver()
    {
        escapedTxt.text = "ESCAPED " + TotalEscaped + "/10";
        // if wave is over
        if(RoundEscaped + TotalKilled == _totalEnemies)
        {
            if(waveNumber <= _enemies.Length)
            {
                whichEnemiesToSpawn = waveNumber;
            }
            SetCurrentGameState();
            ShowMenu();
        }
    }

    private void ShowMenu()
    {
        switch(currentState)
        {
            case gameStatus.gameover:
                playBtnTxt.text = "Play Again";
                AudioSource.PlayOneShot(SoundManager.Instance.Gameover);
                break;
            case gameStatus.next:
                playBtnTxt.text = "Next Wave";
                break;
            case gameStatus.play:
                playBtnTxt.text = "Play";
                break;
            case gameStatus.win:
                playBtnTxt.text = "Play";
                break;
        }
        playBtn.gameObject.SetActive(true);
    }


    private void HandleEscape()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instance.SelectedTowerBtn = null;
            TowerManager.Instance.DisableSpriteRenderer();
        }
    }

    public void playButtonPressed()
    {
        switch(currentState)
        {
            case gameStatus.next:
                waveNumber++;
                _totalEnemies += waveNumber;
                break;
            // for these states: gameover, play, win
            default:
                _audioSource.PlayOneShot(SoundManager.Instance.Newgame);
                whichEnemiesToSpawn = 0;
                waveNumber = 0;
                _totalEnemies = 3;
                TotalEscaped = 0;
                TotalMoney = 10;
                escapedTxt.text = "ESCAPED " + TotalEscaped + "/10";
                TowerManager.Instance.DestroyAllTowers();
                break;
        }
        tower.DestroyAllProjectiles();
        DestroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        waveNumberTxt.text = "WAVE " + (waveNumber+1).ToString();
        StartCoroutine(Spawn());
        playBtn.gameObject.SetActive(false);
    }
}
