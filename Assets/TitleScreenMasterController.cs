using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenMasterController : MonoBehaviour
{
    [SerializeField] private GameObject _titleText;
    [SerializeField] private GameObject _pressEnterText;
    [SerializeField] private GameObject _playerGo;
    [SerializeField] private GameObject _slimeGo;
    [SerializeField] private Image _uiImgBlackFull;
    private PlayerOpeningSceneContrller _pOpeningController;
    private SlimeOpeningSceneController _sOpeningController;

    private SceneStatus _sceneStatus;
    private Tween _tween;
    enum SceneStatus
    {
        Wait,
        Start,
        ReachBox
    }
    
    void Start()
    {
        _sceneStatus = SceneStatus.Wait;
        _tween = _pressEnterText.GetComponent<Text>().DOFade(0, 1f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
        _pOpeningController = _playerGo.GetComponent<PlayerOpeningSceneContrller>();
        _sOpeningController = _slimeGo.GetComponent<SlimeOpeningSceneController>();
        AudioManager.Instance.PlayBGM("BGM_Title");
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && _sceneStatus == SceneStatus.Wait)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySE("SE_Enter");
            _tween.Kill();
            _pressEnterText.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
            foreach(Text text in _titleText.GetComponentsInChildren<Text>()){
                text.DOFade(0, 2f);
            }
            _pressEnterText.GetComponent<Text>().DOFade(0, 2f).OnComplete(StartScene);
            _sceneStatus = SceneStatus.Start;
        }
    }
    public void StartScene()
    {
        AudioManager.Instance.PlayBGM("BGM_Opening");
        StartCoroutine(PlaySceneEvents());
    }
    IEnumerator PlaySceneEvents()
    {
        yield return new WaitForSeconds(2f);
        yield return _pOpeningController.MoveToChest();
        yield return new WaitForSeconds(0.3f);
        yield return _pOpeningController.ShowLikeBubble();
        yield return new WaitForSeconds(0.3f);
        yield return _sOpeningController.JumpIn();
        yield return new WaitForSeconds(2f);
        yield return _pOpeningController.TurnAround();
        yield return _pOpeningController.GetSurprised();
        yield return new WaitForSeconds(0.5f);
        _uiImgBlackFull.DOFade(1f, 1f).SetEase(Ease.InQuart).OnComplete(() => { SceneManager.LoadScene("InGame"); });
    }
}
