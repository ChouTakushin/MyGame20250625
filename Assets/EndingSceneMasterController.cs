using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingSceneMasterController : MonoBehaviour
{
    [SerializeField] private GameObject _playerGo;
    [SerializeField] private GameObject _chestGo;
    [SerializeField] private Image _uiImgBlackFull;
    [SerializeField] private GameObject _nothingBubble;
    private PlayerEndingSceneController _pEndingController;

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
        _pEndingController = _playerGo.GetComponent<PlayerEndingSceneController>();
        _uiImgBlackFull.DOFade(0f, 1.5f).SetEase(Ease.OutQuart).OnComplete(() => { StartScene(); });
    }


    void Update()
    {
    }
    public void StartScene()
    {
        StartCoroutine(PlaySceneEvents());
    }
    public void ChestNothing()
    {
        _nothingBubble.transform.DOMoveY(_nothingBubble.transform.position.y + 0.5f, 1f);
        _nothingBubble.GetComponent<SpriteRenderer>().DOFade(1f, 0.5f).SetLoops(2,LoopType.Yoyo).OnComplete(() => { _nothingBubble.SetActive(false); });
        AudioManager.Instance.PlaySE("SE_Nothing");
    }
    IEnumerator PlaySceneEvents()
    {
        yield return new WaitForSeconds(0.5f);
        _pEndingController.TurnAround();
        yield return new WaitForSeconds(0.5f);
        yield return _pEndingController.ShowDelightedBubble();
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance.PlaySE("SE_ChestOpen");
        _chestGo.GetComponent<Animator>().Play("Open");
        yield return new WaitForSeconds(1f);
        _playerGo.GetComponent<Animator>().speed = 0;
        ChestNothing();
        yield return new WaitForSeconds(1.3f);
        yield return _pEndingController.GetEmbarrassed();
        yield return new WaitForSeconds(1.5f);
        _uiImgBlackFull.DOFade(1f, 1f).SetEase(Ease.InQuart);
        yield return new WaitForSeconds(1.5f);
        AudioManager.Instance.PlaySE("SE_Player_Grab");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("EndScene");
    }
}
