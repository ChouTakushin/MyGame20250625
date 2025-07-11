using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEndingSceneController : MonoBehaviour
{

    private Animator _animator;
    private Rigidbody2D _rb;
    [SerializeField] private GameObject _delightedBubble;
    [SerializeField] private GameObject _embarrassedBubble;
    [SerializeField] private Vector3 _offsetOfSin;
    //private 
    private SceneStatus _sceneStatus;

    enum SceneStatus
    {
        Wait,
        MovingToChest,
        Reached
    }
    void Start()
    {
        _sceneStatus = SceneStatus.Wait;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

    }

    public IEnumerator ShowDelightedBubble()
    {
        _delightedBubble.SetActive(true);
        AudioManager.Instance.PlaySE("SE_Player_Delighted");
        yield return new WaitForSeconds(1f);
        _delightedBubble.SetActive(false);
    }

    public void TurnAround()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);
    }

    public IEnumerator GetEmbarrassed()
    {
        _embarrassedBubble.SetActive(true);
        AudioManager.Instance.PlaySE("SE_Player_Embarrassed");
        _animator.speed = 0;
        yield return new WaitForSeconds(2f);
        _embarrassedBubble.SetActive(false);
    }
}
