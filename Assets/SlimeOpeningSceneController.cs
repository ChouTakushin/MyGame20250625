using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeOpeningSceneController : MonoBehaviour
{
    [SerializeField] private Vector2 _vJump = default;
    private Animator _animator;
    private Rigidbody2D _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("P_YSpeed", _rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            AudioManager.Instance.PlaySE("SE_Enemy_Slime_Land");
            _animator.SetTrigger("T_Land");
            _rb.velocity = Vector2.zero;
        }
    }

    public IEnumerator JumpIn()
    {
        _animator.SetTrigger("T_Jump");
        _rb.AddForce(_vJump, ForceMode2D.Impulse);
        return null;
    }
}
