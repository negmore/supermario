using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            if (null == m_instance)
                m_instance = FindObjectOfType<GameManager>();

            return m_instance;
        }
    }

    public enum GameState
    {
        GAME_READY = 0,
        GAME_PLAY,
        GAME_PUASE,
        GAME_DIE,
        GAME_CLEAR,
        GAME_END,
        GAME_CHANGE,
    }


    public static GameManager m_instance;

    public GameObject camera;
    public GameObject player;
    public GameObject clearEffect;

    public GameObject fadeSystem;
    public GameObject soundPlayer;

    public GameState state;

    public int life = 3;
    public int score = 0;
    public int coin = 0;

    public int timeLimit= 300;

    private int comboKillScore = 100;

    private float timeCount = 1f;

    public GameObject[] uiLife;
    public GameObject[] uiScore;
    public GameObject[] uiCoin;
    public GameObject[] uiTime;
    public GameObject uiAddScore;
    public GameObject uiAddLife;

    public Sprite[] number;

    private bool isClearTime = false;
    private bool isHurryTime = false;
    private bool isDeadTime = false;

    private void Awake()
    {
        if (this != instance)
            Destroy(gameObject);

        score = PlayerPrefs.GetInt("score");
        coin = PlayerPrefs.GetInt("coin");
        life = PlayerPrefs.GetInt("life");

        state = GameState.GAME_PLAY;

        UpdateCoin();
        UpdateLife();
        UpdateScore();
        UpdateTime();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateLife();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.GAME_PLAY == state)
        {
            InPutSystemKey();
            CheckedTIme();

            if (GameState.GAME_PUASE == state)
                return;

            UpdateTime();
        }
        else if(GameState.GAME_CLEAR == state && true == isClearTime)
        {
            ClearTime();
        }
        
    }

    void InPutSystemKey()
    {
        if (true == Input.GetKeyDown(KeyCode.Q))
        {
            if (GameState.GAME_PUASE == state)
                state = GameState.GAME_PLAY;
            else
                state = GameState.GAME_PUASE;
        }
    }

    void CheckedTIme()
    {
        if (false == isHurryTime && timeLimit < 100)
        {
            isHurryTime = true;
            StartBgm(1);
        }

        if (false == isDeadTime && timeLimit <= 0)
        {
            isDeadTime = true;
            player.transform.GetComponent<PlayerController>().Dead();
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public void SetCoin(Vector3 Position, bool IsSound = false)
    {
        ++coin;

        if (100 == coin)
        {
            ++life;
            coin = 0;

            StartEffect(1);
        }

        UpdateCoin();
        SetScore(Position, 100);

        if (true == IsSound)
            StartEffect(3);
    }

    public void SetScore(Vector3 Pos, int Score)
    {
        score += Score;

        UpdateScore();

        GameObject game = Instantiate(uiAddScore, Pos, Quaternion.identity);
        game.GetComponent<AddScoreUI>().SetScore(Score);
    }

    public void SetHiScore(int Score)
    {
        score = Score;

        UpdateScore();
    }

    public void SetKillScore(Vector3 Pos)
    {
        if (false == player.transform.GetComponent<PlayerController>().IsGround())
            comboKillScore *= 2;
        else
            InitComboKillScore();

        score += comboKillScore;

        UpdateScore();

        GameObject game = Instantiate(uiAddScore, Pos, Quaternion.identity);
        game.GetComponent<AddScoreUI>().SetScore(comboKillScore);
    }

    public void InitComboKillScore()
    {
        comboKillScore = 200;
    }

    public void SetLife(Vector3 Pos, int Life)
    {
        life = Life;
        UpdateLife();

        GameObject game = Instantiate(uiAddLife, Pos, Quaternion.identity);
    }

    public int GetCoin()
    {
        return coin;
    }

    public int GetScore()
    {
        return score;
    }

    public float GetTime()
    {
        return timeCount;
    }

    public int GetLife()
    {
        return life;
    }

    private void UpdateLife()
    {
        ChangeImage(ref uiLife, life);
    }

    private void UpdateTime()
    {
        timeCount += Time.deltaTime;

        if (timeCount < 1f)
            return;

        timeCount = 0f;
        --timeLimit;

        ChangeImage(ref uiTime, timeLimit);
    }

    private void UpdateCoin()
    {
        ChangeImage(ref uiCoin, coin);
    }

    private void UpdateScore()
    {
        ChangeImage(ref uiScore, score);
    }

    private void ChangeImage(ref GameObject[] gameobj, int value)
    {
        int temp = value;

        for (int i = 0; i < gameobj.Length; ++i)
        {
            int val = temp % 10;
            int index = gameobj.Length - i - 1;

            Image iamge = gameobj[index].GetComponent<Image>();
            iamge.sprite = number[val];

            temp /= 10;
        }
    }

    public GameState GetGameState()
    {
        return state;
    }

    public bool IsGameStatePlay()
    {
        return state == GameState.GAME_PLAY;
    }
    public void SetGameState(GameState Gamestate)
    {
        state = Gamestate;

        if (GameState.GAME_CLEAR == state)
        {
            float sizeX = player.GetComponent<BoxCollider2D>().size.x;
            Vector3 pos = player.transform.position;

            pos.x += sizeX * 1.5f;

            float per = pos.y / 1.8f;
            int result = (int)(5000 * per);

            if (result <= 0)
                result = 100;
            else if (5000 < result)
                result = 5000;

            SetScore(pos, result);
        }
    }

    public Sprite GetNumberSprite(int index)
    {
        return number[index];
    }

    public void StageClear()
    {
        isClearTime = true;
    }

    private void ClearTime()
    {
        timeCount += Time.fixedDeltaTime * 24f;

        if (timeCount < 1f)
            return;

        score += (int)timeCount * 100;
        UpdateScore();

        timeLimit -= (int)timeCount;
        ChangeImage(ref uiTime, timeLimit);

        timeCount = timeCount - 1f;

        if (timeLimit <= 0)
        {
            timeLimit = 0;
            timeCount = 0f;

            state = GameState.GAME_END;

            clearEffect.GetComponent<ClearEffectPlay>().SetPlay();
        }
    }

    public void SetFadeInOut(bool OnOff)
    {
        fadeSystem.SetActive(true);
        fadeSystem.SendMessage("Fade", OnOff);
    }

    public void SetFadeOff()
    {
        Invoke("FadeOff", 0.4f);
    }

    private void FadeOff()
    {
        fadeSystem.SetActive(false);
    }

    public void Save()
    {
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("coin", coin);
        PlayerPrefs.SetInt("life", life);

        PlayerPrefs.Save();
    }

    private void Reset()
    {
        coin = 0;
        life = 3;

        PlayerPrefs.SetInt("coin", 0);
        PlayerPrefs.SetInt("life", 3);
    }

    public void PlayerDead()
    {
        --life;
        Save();

        string sceneName = "intro";

        if (life < 0)
            sceneName = "gameover";
        else
        {
            if (timeLimit <= 0)
                sceneName = "timeover";
        }

        SceneManager.LoadScene(sceneName);
    }

    public void StartBgm(int Index)
    {
        if (0 == Index || 5 == Index)
        {
            if (timeLimit < 100)
                Index += 1;
        }
        soundPlayer.SendMessage("PlayBGM", Index);
    }

    public void StopBgm()
    {
        soundPlayer.SendMessage("StopBGM");
    }

    public void StartEffect(int Index)
    {
        soundPlayer.SendMessage("PlayEffect", Index);
    }
}
