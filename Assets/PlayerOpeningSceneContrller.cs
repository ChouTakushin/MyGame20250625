using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOpeningSceneContrller : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    [SerializeField] private GameObject _heroStopBoxGo;
    [SerializeField] private GameObject _likeBubble;
    [SerializeField] private GameObject _surpriseBubble;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "HeroStopBox")
        {
            _animator.Play("Idle");

        }
    }
    //public void MoveToChest()
    //{
    //    _animator.Play("Walk");
    //    _rd.velocity = Vector2.left * 5f;
    //}

    public IEnumerator MoveToChest()
    {
        _animator.Play("Walk");
        _rb.velocity = Vector2.left * 4f;

        while (true)
        {
            if (Physics2D.Linecast(transform.position + _offsetOfSin, transform.position + new Vector3(-0.54f, 0f, 0f), LayerMask.GetMask("FunctionalColliders")))
            {
                break;
            }
            Debug.DrawLine(transform.position + _offsetOfSin, transform.position + new Vector3(-0.54f, 1.2f, 0f));
            yield return null;
        }
        _animator.Play("Idle");
        _rb.velocity = Vector2.zero;
    }

    public IEnumerator ShowLikeBubble()
    {
        _likeBubble.SetActive(true);
        AudioManager.Instance.PlaySE("SE_Player_Like");
        yield return new WaitForSeconds(1f);
        _likeBubble.SetActive(false);
    }

    public IEnumerator TurnAround()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator GetSurprised()
    {
        _surpriseBubble.SetActive(true);
        AudioManager.Instance.PlaySE("SE_Player_Surprised");
        yield return new WaitForSeconds(1f);
        _surpriseBubble.SetActive(false);
    }
}
