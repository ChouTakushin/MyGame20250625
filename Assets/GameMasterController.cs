using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameMasterController : MonoBehaviour
{
    private GameObject _playerGo;
    private PlayerMovementController _playerMovementController;
    private PlayerCombatManager _playerCombatManager;
    private Animator _playerAnimator;
    [SerializeField] private GameObject _uiTextHp;
    [SerializeField] private GameObject _uiTextMp;
    [SerializeField] private GameObject _uiTextEC;
    [SerializeField] private GameObject _uiTextGood;
    [SerializeField] private GameObject _uiTextGoodluck;
    [SerializeField] private GameObject _uiTextFinalRush;
    [SerializeField] private GameObject _uiTextFinalRush2;
    [SerializeField] private GameObject _uiTextPressQ;
    [SerializeField] private GameObject _uiTextGameClear;
    [SerializeField] private GameObject _uiTextGameOver;
    [SerializeField] private GameObject _uiTextRetry;
    [SerializeField] private Image _uiImgTutControl;
    [SerializeField] private Image _uiImgTutSlime;
    [SerializeField] private Image _uiImgTutSkeleton;
    [SerializeField] private Image _uiImgTutMushroom;
    [SerializeField] private Image _uiImgTutFlyEye;
    [SerializeField] private Image _uiImgTutWizard;
    [SerializeField] private Image _uiImgTutTips;
    [SerializeField] private Image _uiImgWhiteFull;
    [SerializeField] private Image _uiImgRedFull;
    [SerializeField] private Image _uiImgBlackFull;
    [SerializeField] private Image _uiImgFinalSpFx1;
    [SerializeField] private Image _uiImgFinalSpFx2;
    [SerializeField] private Image _uiImgFinalSpFx3;
    [SerializeField] private Image _uiImgFinalSpFx4;
    [SerializeField] private Image _uiImgFinalSpFx5;
    [SerializeField] private Image _uiImgBigSlash;
    [SerializeField] private GameObject _enemyGenerator;
    [SerializeField] private GameObject _tutEnemySlime;
    [SerializeField] private GameObject _tutEnemySkeleton;
    [SerializeField] private GameObject _tutEnemyMushroom;
    [SerializeField] private GameObject _tutEnemyFlyEye;
    [SerializeField] private GameObject _tutEnemyWizard;
    [SerializeField] private int _enemyCountMax = 50;
    [SerializeField] private int _spawnLevelupEvery = 10;
    [SerializeField] private float _spawnLevelupTime = 0.5f;
    [SerializeField] private float _enemySpawnIntervalMax = 2.5f;
    [SerializeField] private float _tutFadeTime = 0.3f;
    [SerializeField] private int _finalRushMaxMp = 50;
    [SerializeField] private Color _mpOrigColor = new Color(0.6773585f, 0.7287073f, 1f);
    [SerializeField] private Color _mpGlowColor = new Color(1f, 0.9487633f, 0.005660295f);
    private int _enemyCount = 0;
    private int _prevLvEnemyCount = 0;
    private EnemyGeneratorController _enemyGeneratorController;
    private EnumGameStatus _gameStatus;
    private EnumGameStatus _prevGameStatus;
    private bool _canCloseTut;
    private bool _finalRushStarted = false;

    public int FinalRushMaxMp { get { return _finalRushMaxMp; } }
    public EnumGameStatus GameStatus { get { return _gameStatus; } set { _gameStatus = value; } }
    

    public enum EnumGameStatus
    {
        Start,
        TutControl,
        TutSlime,
        TutSkeleton,
        TutMushroom,
        TutFlyEye,
        TutWizard,
        TutTips,
        Started,
        FinalRush,
        ToFinish,
        Finished,
        GameOver,
    }

    void Start()
    {
        _gameStatus = EnumGameStatus.Start;
        _prevGameStatus = EnumGameStatus.Start;
        _playerGo = GameObject.Find("Player");
        _playerMovementController = _playerGo.GetComponent<PlayerMovementController>();
        _playerCombatManager = _playerGo.GetComponent<PlayerCombatManager>();
        _playerAnimator = _playerGo.GetComponent<Animator>();
        _enemyGeneratorController = _enemyGenerator.GetComponent<EnemyGeneratorController>();

        _uiImgBlackFull.gameObject.SetActive(true);
        _uiImgBlackFull.DOFade(0f, 1.5f).SetEase(Ease.InQuint).OnComplete(GameStart);
        if (!AudioManager.Instance.BGMSource.isPlaying)
        {
            AudioManager.Instance.PlayBGM("BGM_Opening");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_prevGameStatus == EnumGameStatus.Start && _gameStatus == EnumGameStatus.TutControl)
        {
            ShowTutControl();
        }
        else if (_prevGameStatus == EnumGameStatus.TutControl && _gameStatus == EnumGameStatus.TutSlime)
        {
            ShowTutSlime();
        }
        else if (_prevGameStatus == EnumGameStatus.TutSlime && _gameStatus == EnumGameStatus.TutSkeleton)
        {
            ShowTutSkeleton();
        }
        else if (_prevGameStatus == EnumGameStatus.TutSkeleton && _gameStatus == EnumGameStatus.TutMushroom)
        {
            ShowTutMushroom();
        }
        else if (_prevGameStatus == EnumGameStatus.TutMushroom && _gameStatus == EnumGameStatus.TutFlyEye)
        {
            ShowTutFlyEye();
        }
        else if (_prevGameStatus == EnumGameStatus.TutFlyEye && _gameStatus == EnumGameStatus.TutWizard)
        {
            ShowTutWizard();
        }
        else if (_prevGameStatus == EnumGameStatus.TutWizard && _gameStatus == EnumGameStatus.TutTips)
        {
            ShowTutTips();
        }
        else if (_prevGameStatus == EnumGameStatus.TutTips && _gameStatus == EnumGameStatus.Start)
        {
            StartCoroutine(ShowGoodLuck());
        }
        else if (_prevGameStatus == EnumGameStatus.Start && _gameStatus == EnumGameStatus.Started)
        {
            _enemyGeneratorController.CanSpawn = true;
            _prevGameStatus = _gameStatus;
        }
        else if (_prevGameStatus != EnumGameStatus.FinalRush && _gameStatus == EnumGameStatus.FinalRush)
        {
            StartCoroutine(EnterFinalRush());
        }
        else if (_prevGameStatus == EnumGameStatus.ToFinish && _gameStatus == EnumGameStatus.ToFinish && Input.GetKeyDown(KeyCode.Q))
        {
            _gameStatus = EnumGameStatus.Finished;
        }

        if (Input.GetKeyDown(KeyCode.Return) && _canCloseTut)
        {
            if (_prevGameStatus == EnumGameStatus.TutControl && _gameStatus == EnumGameStatus.TutControl)
            {
                CloseTutControl();
            }
            else if (_prevGameStatus == EnumGameStatus.TutSlime && _gameStatus == EnumGameStatus.TutSlime)
            {
                CloseTutSlime();
            }
            else if (_prevGameStatus == EnumGameStatus.TutSkeleton && _gameStatus == EnumGameStatus.TutSkeleton)
            {
                CloseTutSkeleton();
            }
            else if (_prevGameStatus == EnumGameStatus.TutMushroom && _gameStatus == EnumGameStatus.TutMushroom)
            {
                CloseTutMushroom();
            }
            else if (_prevGameStatus == EnumGameStatus.TutFlyEye && _gameStatus == EnumGameStatus.TutFlyEye)
            {
                CloseTutFlyEye();
            }
            else if (_prevGameStatus == EnumGameStatus.TutWizard && _gameStatus == EnumGameStatus.TutWizard)
            {
                CloseTutWizard();
            }
            else if (_prevGameStatus == EnumGameStatus.TutTips && _gameStatus == EnumGameStatus.TutTips)
            {
                CloseTutTips();
            }
        }
        if (_prevGameStatus != EnumGameStatus.Finished && _gameStatus == EnumGameStatus.Finished)
        {
            StartCoroutine(DoFinish());
        }
        if (Input.GetKeyDown(KeyCode.Return) && _gameStatus == EnumGameStatus.GameOver)
        {
            DoRetry();
        }
    }

    public void GameStart()
    {
        _prevGameStatus = _gameStatus;
        _gameStatus = EnumGameStatus.TutControl;
        DeactiveBlackImage();
    }
    public void ShowTutControl()
    {
        _prevGameStatus = _gameStatus;
        _uiImgTutControl.DOFade(1f, _tutFadeTime).SetEase(Ease.OutCubic).OnComplete(() => { _canCloseTut = true; });
    }
    public void CloseTutControl()
    {
        _canCloseTut = false;
        _uiImgTutControl.DOFade(0f, _tutFadeTime).SetEase(Ease.InCubic).OnComplete(() => {
            _uiImgTutControl.gameObject.SetActive(false);
            _gameStatus = EnumGameStatus.TutSlime; });
    }
    public void ShowTutSlime()
    {
        _prevGameStatus = _gameStatus;
        _uiImgTutSlime.DOFade(1f, _tutFadeTime).SetEase(Ease.OutCubic).OnComplete(() => { _canCloseTut = true; });
    }
    public void CloseTutSlime()
    {
        _canCloseTut = false;
        _uiImgTutSlime.DOFade(0f, _tutFadeTime).SetEase(Ease.InCubic).OnComplete(() => {
            _uiImgTutSlime.gameObject.SetActive(false);
            //_gameStatus = EnumGameStatus.TutSkeleton;
            ActivateTutSlime();
        });
    }
    public void ActivateTutSlime()
    {
        _tutEnemySlime.SetActive(true);
    }
    public void ShowGoodAndGoNext(EnumGameStatus nextStatus)
    {
        StartCoroutine(DoShowGoodAndGoNext(nextStatus));
    }

    private IEnumerator DoShowGoodAndGoNext(EnumGameStatus nextStatus)
    {
        AudioManager.Instance.PlaySE("SE_Good");
        yield return GoodTextIn();
        yield return new WaitForSeconds(2f);
        yield return GoodTextOut();
        _gameStatus = nextStatus;
    }
    IEnumerator GoodTextIn()
    {
        Text goodText = _uiTextGood.GetComponent<Text>();
        float textAlpha = 0f;
        float textScale = 1.5f;
        _uiTextGood.SetActive(true);
        goodText.color = new Color(goodText.color.r, goodText.color.g, goodText.color.b, textAlpha);
        goodText.transform.localScale = Vector3.one * textScale;
        while (textAlpha < 1f)
        {
            textAlpha += Time.deltaTime * 4f;
            textScale -= Time.deltaTime * 2f;
            goodText.color = new Color(goodText.color.r, goodText.color.g, goodText.color.b, textAlpha);
            goodText.transform.localScale = Vector3.one * textScale;
            yield return null;
        }
        goodText.color = new Color(goodText.color.r, goodText.color.g, goodText.color.b, 1f);
    }
    IEnumerator GoodTextOut()
    {
        Text goodText = _uiTextGood.GetComponent<Text>();
        float textAlpha = 1f;
        while (textAlpha > 0f)
        {
            textAlpha -= Time.deltaTime * 2f;
            goodText.color = new Color(goodText.color.r, goodText.color.g, goodText.color.b, textAlpha);
            yield return null;
        }
        _uiTextGoodluck.SetActive(false);
    }
    public void ShowGoodText()
    {
        
        _uiTextGood.SetActive(true);
        _uiTextGood.transform.localScale = Vector3.one * 1.5f;
        _uiTextGood.GetComponent<Text>().DOFade(1f, 0.5f);
        _uiTextGood.transform.DOScale(1, 0.5f);
    }
    public void ShowTutSkeleton()
    {
        _prevGameStatus = _gameStatus;
        _uiImgTutSkeleton.DOFade(1f, _tutFadeTime).SetEase(Ease.OutCubic).OnComplete(() => { _canCloseTut = true; });
    }
    public void CloseTutSkeleton()
    {
        _canCloseTut = false;
        _uiImgTutSkeleton.DOFade(0f, _tutFadeTime).SetEase(Ease.InCubic).OnComplete(() => {
            _uiImgTutSkeleton.gameObject.SetActive(false);
            //_gameStatus = EnumGameStatus.TutMushroom;
            ActivateTutSkeleton();
        });
    }
    public void ActivateTutSkeleton()
    {
        _tutEnemySkeleton.SetActive(true);
    }
    public void ShowTutMushroom()
    {
        _prevGameStatus = _gameStatus;
        _uiImgTutMushroom.DOFade(1f, _tutFadeTime).SetEase(Ease.OutCubic).OnComplete(() => { _canCloseTut = true; });
    }
    public void CloseTutMushroom()
    {
        _canCloseTut = false;
        _uiImgTutMushroom.DOFade(0f, _tutFadeTime).SetEase(Ease.InCubic).OnComplete(() => {
            _uiImgTutMushroom.gameObject.SetActive(false);
            //_gameStatus = EnumGameStatus.TutFlyEye;
            ActivateTutMushroom();
        });
    }
    public void ActivateTutMushroom()
    {
        _tutEnemyMushroom.SetActive(true);
    }
    public void ShowTutFlyEye()
    {
        _prevGameStatus = _gameStatus;
        _uiImgTutFlyEye.DOFade(1f, _tutFadeTime).SetEase(Ease.OutCubic).OnComplete(() => { _canCloseTut = true; });
    }
    public void CloseTutFlyEye()
    {
        _canCloseTut = false;
        _uiImgTutFlyEye.DOFade(0f, _tutFadeTime).SetEase(Ease.InCubic).OnComplete(() => {
            _uiImgTutFlyEye.gameObject.SetActive(false);
            //_gameStatus = EnumGameStatus.TutWizard;
            ActivateTutFlyEye();
        });
    }
    public void ActivateTutFlyEye()
    {
        _tutEnemyFlyEye.SetActive(true);
    }
    public void ShowTutWizard()
    {
        _prevGameStatus = _gameStatus;
        _uiImgTutWizard.DOFade(1f, _tutFadeTime).SetEase(Ease.OutCubic).OnComplete(() => { _canCloseTut = true; });
    }
    public void CloseTutWizard()
    {
        //AudioManager.Instance.BgmFadeOut(1f);
        _canCloseTut = false;
        _uiImgTutWizard.DOFade(0f, _tutFadeTime).SetEase(Ease.InCubic).OnComplete(() => {
            _uiImgTutWizard.gameObject.SetActive(false);
            //_gameStatus = EnumGameStatus.Start;
            ActivateTutWizard();
        });
    }
    public void ActivateTutWizard()
    {
        _tutEnemyWizard.SetActive(true);
    }
    public void ShowTutTips()
    {
        _prevGameStatus = _gameStatus;
        _uiImgTutTips.DOFade(1f, _tutFadeTime).SetEase(Ease.OutCubic).OnComplete(() => { _canCloseTut = true; });
    }
    public void CloseTutTips()
    {
        //AudioManager.Instance.BgmFadeOut(1f);
        _canCloseTut = false;
        _uiImgTutTips.DOFade(0f, _tutFadeTime).SetEase(Ease.InCubic).OnComplete(() => {
            _uiImgTutTips.gameObject.SetActive(false);
            _gameStatus = EnumGameStatus.Start;
        });
    }

    IEnumerator ShowGoodLuck()
    {
        _prevGameStatus = _gameStatus;
        yield return GoodLuckIn();
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayBGM("BGM_InGame");
        yield return GoodLuckOut();
        _gameStatus = EnumGameStatus.Started;
    }

    IEnumerator GoodLuckIn()
    {
        Text goodluckText = _uiTextGoodluck.GetComponent<Text>();
        float textAlpha = 0f;
        float textScale = 3f;
        _uiTextGoodluck.SetActive(true);
        goodluckText.color = new Color(goodluckText.color.r, goodluckText.color.g, goodluckText.color.b, textAlpha);
        goodluckText.transform.localScale = Vector3.one * textScale;
        while (textAlpha < 1f)
        {
            textAlpha += Time.deltaTime * 2f;
            textScale -= 2f * Time.deltaTime * 2f;
            goodluckText.color = new Color(goodluckText.color.r, goodluckText.color.g, goodluckText.color.b, textAlpha);
            goodluckText.transform.localScale = Vector3.one * textScale;
            yield return null;
        }
        goodluckText.color = new Color(goodluckText.color.r, goodluckText.color.g, goodluckText.color.b, 1f);
        AudioManager.Instance.PlaySE("SE_BattleStart");
    }
    IEnumerator GoodLuckOut()
    {
        Text goodluckText = _uiTextGoodluck.GetComponent<Text>();
        float textAlpha = 1f;
        while (textAlpha > 0f)
        {
            textAlpha -= Time.deltaTime / 0.75f;
            goodluckText.color = new Color(goodluckText.color.r, goodluckText.color.g, goodluckText.color.b, textAlpha);
            yield return null;
        }
        _uiTextGoodluck.SetActive(false);
    }
    public void DeactiveBlackImage()
    {
        _uiImgBlackFull.gameObject.SetActive(false);
    }

    public void SetUiHpText(int hp)
    {
        _uiTextHp.GetComponent<Text>().text = "HP: " + hp + "/" + _playerCombatManager.MaxHp;
    }
    public void SetUiMpText(int mp)
    {
        _uiTextMp.GetComponent<Text>().text = "MP: " + mp.ToString("00") + "/" + _playerCombatManager.MaxMp;
    }
    public void SetUiECText(int ec)
    {
        _uiTextEC.GetComponent<Text>().text = "Enemy Count: " + ec.ToString("00") + "/" + _enemyCountMax.ToString("00");
    }
    public void CountEnemyKill(int count)
    {
        if (_finalRushStarted) return;
        _enemyCount += count;
        SetUiECText(_enemyCount);
        if(_enemyCount >= _prevLvEnemyCount + _spawnLevelupEvery
            && _enemyGeneratorController.SpawnInterval > _enemySpawnIntervalMax) { 

            _enemyGeneratorController.SetSpawnInterval(_enemyGeneratorController.SpawnInterval - _spawnLevelupTime);
            _prevLvEnemyCount = _enemyCount;
        }
        if(_enemyCount >= _enemyCountMax)
        {
            _gameStatus = EnumGameStatus.FinalRush;
            _finalRushStarted = true;
        }
    }
    public IEnumerator DoGameOver()
    {
        _playerMovementController.CanInput = false;
        AudioManager.Instance.StopBGM();
        StopEnemies();
        Time.timeScale = 0.05f;
        _playerMovementController.BeInvulnerable();
        _playerMovementController.Death();
        yield return new WaitForSecondsRealtime(0.75f);
        AudioManager.Instance.PlaySE("SE_Player_Death");
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1.5f);
        AudioManager.Instance.PlayBGM("BGM_GameOver");
        _uiTextGameOver.SetActive(true);
        _uiTextGameOver.GetComponent<Text>().DOFade(1f, 2f);
        yield return new WaitForSeconds(2f);
        _uiTextRetry.SetActive(true);
        _uiTextRetry.GetComponent<Text>().DOFade(1f, 0.3f).OnComplete(() => { _gameStatus = EnumGameStatus.GameOver; });
    }

    public void StopEnemies()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            go.GetComponent<StopMovementController>().StopMovement();
        }
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("EnemyProjectile"))
        {
            Destroy(go);
        }
        _enemyGeneratorController.CanSpawn = false;
    }

    /// <summary>
    /// 画面上の敵を全滅させ、弾を消す
    /// </summary>
    public void ClearEnemies()
    {
        // 敵のAnimation速度を戻し、死亡時処理を呼び出す
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            go.GetComponent<Animator>().speed = 1f;
            go.GetComponent<EnemyDeathBehaviourBase>().DoDeath();
        }
        // 画面上の敵弾を消す
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("EnemyProjectile"))
        {
            Destroy(go);
        }
    }
    public void DoRetry()
    {
        AudioManager.Instance.BgmFadeOut(0.75f);
        _uiImgBlackFull.gameObject.SetActive(true);
        _uiImgBlackFull.color = new Color(0f, 0f, 0f, 0f);
        _uiImgBlackFull.DOFade(1f, 1f).SetEase(Ease.OutQuint).OnComplete(() => {SceneManager.LoadScene("InGame"); });
    }
    public IEnumerator EnterFinalRush()
    {
        _prevGameStatus = _gameStatus;
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySE("SE_FinalRush");
        DoMpGlowEffect();
        _enemyGeneratorController.EnterFinalRush();
        _playerCombatManager.MaxMp = _finalRushMaxMp;
        SetUiMpText(_playerCombatManager.Mp); 
        _uiTextFinalRush.gameObject.SetActive(true);
        _uiTextFinalRush.transform.localScale = Vector3.one * 3f;
        _uiTextFinalRush.GetComponent<Text>().DOFade(1f, 0.3f);
        _uiTextFinalRush.transform.DOScale(1f, 0.3f);
        _uiTextEC.GetComponent<Text>().DOFade(0f, 0.3f);
        yield return new WaitForSeconds(2f);
        AudioManager.Instance.PlayBGM("BGM_FinalRush");
        _uiTextFinalRush.GetComponent<Text>().DOFade(0f, 1f).OnComplete(() => { _uiTextFinalRush.gameObject.SetActive(false); });
        yield return new WaitForSeconds(1f);
        _uiTextFinalRush2.gameObject.SetActive(true);
        _uiTextFinalRush2.transform.localScale = Vector3.one * 3f;
        _uiTextFinalRush2.GetComponent<Text>().DOFade(1f, 0.3f);
        _uiTextFinalRush2.transform.DOScale(1f, 0.3f);
    }
    public void WaitForFinish()
    {
        _prevGameStatus = _gameStatus;
        _uiTextFinalRush2.gameObject.SetActive(false);
        _enemyGeneratorController.StopSpawning();
        //_playerMovementController.BeInvulnerable();
        //StopEnemies();
        Time.timeScale = 0.05f;
        AudioManager.Instance.BgmFadeOut(0.025f);
        AudioManager.Instance.PlaySE("SE_ToFinish");
        _uiTextPressQ.gameObject.SetActive(true);
        _uiTextPressQ.transform.localScale = Vector3.one * 3f;
        _uiTextPressQ.GetComponent<Text>().DOFade(1f, 0.1f);
        _uiTextPressQ.transform.DOScale(1f, 0.1f);
    }
    public void EnterToFinish()
    {
        if (_gameStatus != EnumGameStatus.ToFinish && _prevGameStatus != EnumGameStatus.Finished)
        {
            _gameStatus = EnumGameStatus.ToFinish;
        }
        else
        {
            return;
        }

        if (_prevGameStatus != EnumGameStatus.ToFinish && _gameStatus == EnumGameStatus.ToFinish)
        {
            WaitForFinish();
        }
    }

    public void ShowAndPlayFinalSp(Image fx)
    {
        fx.gameObject.SetActive(true);
    }
    public void ShowBigSlash()
    {
        _uiImgWhiteFull.gameObject.SetActive(true);
        _uiImgWhiteFull.color = new Color(1f, 1f, 1f, 1f);
        _uiImgWhiteFull.DOFade(0f, 1.5f).SetEase(Ease.OutQuart);

        _uiImgBigSlash.gameObject.SetActive(true);
        _uiImgBigSlash.transform.DOScaleY(0f, 0.5f);
        AudioManager.Instance.PlaySE("SE_Player_Sp02_BigSlash");
    }

    public void GameClearIn()
    {
        _uiTextGameClear.gameObject.SetActive(true);
        _uiTextGameClear.transform.DOScale(1f, 2f);
        _uiTextGameClear.GetComponent<Text>().DOFade(1f, 2f).OnComplete(() => { AudioManager.Instance.PlaySE("SE_GameClear"); });
    }

    public void GameClearOut()
    {
        _uiTextGameClear.GetComponent<Text>().DOFade(0f, 1f);
    }

    public IEnumerator DoFinish()
    {
        // 奥義演出開始すると、文字列"YES"をキー名"gameCleared"でセーブデータに保存する
        PlayerPrefs.SetString("gameCleared", "YES");
        Time.timeScale = 1f;
        _uiTextPressQ.gameObject.SetActive(false);
        _prevGameStatus = _gameStatus;
        _playerMovementController.BeProtected();
        _playerMovementController.FreezePosition();
        _playerMovementController.DoSp02Charge();
        _playerAnimator.SetBool("P_CanTransit", false);
        StopEnemies();
        AudioManager.Instance.PlaySE("SE_Player_Sp02_Charge");
        AudioManager.Instance.PlaySE("SE_Player_Sp02_Charge_Voice");
        yield return new WaitForSeconds(2f);

        _uiImgBlackFull.gameObject.SetActive(true);
        _uiImgBlackFull.color = new Color(0f, 0f, 0f, 1f);
        _playerMovementController.RemuseRb();
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySE("SE_Player_Sp02_Slashes");

        ShowAndPlayFinalSp(_uiImgFinalSpFx1);
        yield return new WaitForSeconds(0.3f);

        ShowAndPlayFinalSp(_uiImgFinalSpFx2);
        yield return new WaitForSeconds(0.3f);

        ShowAndPlayFinalSp(_uiImgFinalSpFx3);
        yield return new WaitForSeconds(0.3f);

        ShowAndPlayFinalSp(_uiImgFinalSpFx4);
        yield return new WaitForSeconds(0.3f);

        ShowAndPlayFinalSp(_uiImgFinalSpFx5);
        yield return new WaitForSeconds(0.75f);

        _playerMovementController.DoFinishPose();

        _uiImgBlackFull.color = new Color(0f, 0f, 0f, 0f);
        ShowBigSlash();
        yield return new WaitForSeconds(1f);

        ClearEnemies();
        yield return new WaitForSeconds(2f);

        GameClearIn();
        yield return new WaitForSeconds(3f);

        GameClearOut();
        yield return new WaitForSeconds(1f);

        _uiImgBlackFull.DOFade(1f, 1.5f).SetEase(Ease.OutQuint).OnComplete(() => { SceneManager.LoadScene("Ending"); });
    }
    public bool IsInTutorial()
    {
        if(_gameStatus == EnumGameStatus.TutControl || _gameStatus == EnumGameStatus.TutSlime || _gameStatus == EnumGameStatus.TutSkeleton || _gameStatus == EnumGameStatus.TutMushroom || _gameStatus == EnumGameStatus.TutFlyEye || _gameStatus == EnumGameStatus.TutWizard || _gameStatus == EnumGameStatus.TutTips)
        {
            return true;
        }
        return false;
    }

    public void DoMpNgEffect()
    {
        _uiTextMp.GetComponent<Text>().DOColor(new Color(1f, 0.3f, 0.3f), 0.1f).SetLoops(2).OnComplete(() => { _uiTextMp.GetComponent<Text>().color = _mpOrigColor; });
    }

    public void DoMpGlowEffect()
    {
        _uiTextMp.GetComponent<Text>().DOColor(_mpGlowColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
}
