using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public List<SoundData> _bgmClips;
    public List<SoundData> _seClips;

    public AudioSource BGMSource => _bgmSource; // プロパティ
    private AudioSource _bgmSource;

    public AudioSource SESource => _seSource;
    private AudioSource _seSource;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 自身のゲームオブジェクトにAudioSourceコンポーネントを追加する
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _seSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayBGM(string name)
    {
        SoundData data = _bgmClips.Find(sound => sound._name == name);

        if (data != null)
        {
            // データが見つかったら、登録されたデータをBGM用のAudioSourceに適用する
            BGMSource.clip = data._clip;
            BGMSource.volume = data._volume;
            BGMSource.pitch = data._pitch;
            BGMSource.loop = true;
            BGMSource.Play();
        }
        else
        {
            Debug.LogError("BGMが見つかりませんでした：" + name);
        }
    }
    public void PlaySE(string name)
    {
        SoundData data = _seClips.Find(sound => sound._name == name);

        if (data != null)
        {
            _seSource.PlayOneShot(data._clip, data._volume);
        }
        else
        {
            Debug.LogError("SEが見つかりませんでした：" + name);
        }
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void BgmFadeOut(float timePeriod)
    {
        _bgmSource.DOFade(0f, timePeriod).OnComplete(StopBGM);
    }
}
