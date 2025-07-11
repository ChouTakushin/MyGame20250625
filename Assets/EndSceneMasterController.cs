using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneMasterController : MonoBehaviour
{
    [SerializeField] private Text _uiTextPressEnter;
    [SerializeField] private Image _uiImgBlackFull;

    private bool _canPushEnter = false;
    private Tween _tween;
    void Start()
    {
        _uiImgBlackFull.DOFade(0f, 2f).SetEase(Ease.InQuart).OnComplete(() => { _canPushEnter = true; });
        _tween = _uiTextPressEnter.DOFade(0, 1f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
        AudioManager.Instance.PlayBGM("BGM_EndScene");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _canPushEnter)
        {
            _uiImgBlackFull.DOFade(1f, 2f).SetEase(Ease.OutQuart).OnComplete(() => {
                _tween.Kill();
                AudioManager.Instance.StopBGM();
                ToTitle();
            });
        }
    }

    public void ToTitle()
    {
        SceneManager.LoadScene("Opening");
    }
}
