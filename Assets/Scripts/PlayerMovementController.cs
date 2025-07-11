using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _jumpPower = default;
    [SerializeField] private GameObject _sp01ChargeFx = default;
    [SerializeField] private GameObject _sp01BladePrefab = default;
    [SerializeField] private GameObject _sp02ChargeFx = default;
    [SerializeField] private Vector3 _sp01ChargeFxOffset = default;
    [SerializeField] private Vector3 _sp01PrefabOffset = default;
    [SerializeField] private Vector3 _sp02ChargeFxOffset = default;
    [SerializeField] private float _damageInvTime = default;
    [SerializeField] private float _invAlpha = 0.75f;
    [SerializeField] private GameMasterController _gameMasterController;

    private bool _isGround = false;
    private Animator _animator;
    private GroundDetect _groundDetect;
    private Rigidbody2D _rb;
    private PlayerCombatManager _playerCombatManager;
    private SpriteRenderer _spriteRenderer;
    private bool _canInput = true;
    private bool _isGuarding = false;
    private bool _isVulnerable = true;
    private float _invTimer = 0;
    private bool _isProtected = false;
    public bool IsVulnerable { get { return _isVulnerable; } set { _isVulnerable = value; } }
    public bool CanInput { get { return _canInput; } set { _canInput = value; } }
    public bool IsGround { get { return _isGround; } set { _isGround = value; } }
    public bool IsProtected { get { return _isProtected; } set { _isProtected = value; } }
    public Rigidbody2D Rb { get { return _rb; } }
    void Start()
    {
        _animator = GetComponent<Animator>();
        _groundDetect = GetComponentInChildren<GroundDetect>();
        _rb = GetComponent<Rigidbody2D>();
        _playerCombatManager = GetComponent<PlayerCombatManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator.SetBool("Grounded", _isGround);
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("ySpeed", _rb.velocity.y);
        if(_canInput) { 
            // 攻撃ボタン押下処理
            if (Input.GetKeyDown(KeyCode.J))
            {
                _animator.SetTrigger("T_Attack");
            }
            // ジャンプボタン押下処理
            if (Input.GetKeyDown(KeyCode.Space) && _isGround)
            {
                Jump();
            }
            // ガードボタン入力あり時の処理
            if (Input.GetKey(KeyCode.K) && _isGround)
            {
                if(!(_animator.GetCurrentAnimatorStateInfo(0).IsName("Guard") || _animator.GetCurrentAnimatorStateInfo(0).IsName("GuardHit")))
                {
                    _animator.SetTrigger("T_GuardIn");
                }
                _animator.SetBool("P_Guarding", true);
                _isGuarding = true;
            }
            // ガードボタン解除処理
            if (Input.GetKeyUp(KeyCode.K))
            {
                _animator.SetBool("P_Guarding", false);
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Guard") || _animator.GetCurrentAnimatorStateInfo(0).IsName("GuardHit"))
                {
                    _animator.SetTrigger("T_GuardOut");
                }
                _isGuarding = false;
            }
            // SP攻撃ボタン押下処理
            if(Input.GetKeyDown(KeyCode.L) && _isGround)
            {
                DoSpecialAttack01();
            }
        }
    }

    public void DoSpecialAttack01()
    {
        if (_playerCombatManager.Mp < 5)
        {
            AudioManager.Instance.PlaySE("SE_Player_Sp01_NG");
            _gameMasterController.DoMpNgEffect();
            return;
        }
        _playerCombatManager.MpAlteration(-5);
        _animator.SetTrigger("T_SpAttack01");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && _groundDetect.IsGround)
        {
            _isGround = true;
            _animator.SetBool("Grounded", true);
            _animator.SetTrigger("T_Land");
        }
    }

    private void Jump()
    {
        _isGround = false;
        _rb.AddForce(new Vector2(0f, _jumpPower), ForceMode2D.Impulse);
        _animator.SetFloat("ySpeed", _rb.velocity.y);
        _animator.SetTrigger("Jump");
    }

    public void TakeDamage(int damage)
    {
        //Debug.Log("Vulnerable: " + _isVulnerable + " Protected: " + _isProtected);
        if (!_isVulnerable || _isProtected) return;
        _playerCombatManager.HpAlteration(damage * -1);
    }
    public void DoTakeDamage()
    {
        AudioManager.Instance.PlaySE("SE_Player_Damage");
        _animator.SetTrigger("T_Hurt");
        StartCoroutine(DamageInvulnerable());
    }
    public void GuardHit()
    {
        AudioManager.Instance.PlaySE("SE_GuardHit");
        _animator.SetTrigger("T_GuardHit");
    }
    public bool CheckGuaring()
    {
        return _isGuarding || _animator.GetBool("P_Guarding");
    }
    public void GainMp(int mp)
    {
        _playerCombatManager.MpAlteration(mp);
    }
    public void GenerateSp01Fx()
    {
        AudioManager.Instance.PlaySE("SE_Player_Sp01_Charge");
        Instantiate(_sp01ChargeFx, transform.position + _sp01ChargeFxOffset, Quaternion.identity);
    }
    public void GenerateSp01Projectile()
    {
        AudioManager.Instance.PlaySE("SE_Player_Sp01");
        Instantiate(_sp01BladePrefab, transform.position + _sp01PrefabOffset, Quaternion.identity);
    }
    public void BeInvulnerable()
    {
        _isVulnerable = false;
    }
    public void BeVulnerable()
    {
        _isVulnerable = true;
    }
    public void BeProtected()
    {
        _isProtected = true;
    }
    public void BeUnprotected()
    {
        _isProtected = false;
    }
    //public IEnumerator PlayerDeath()
    //{
    //    Time.timeScale = 0.05f;
    //    BeInvulnerable();
    //    yield return new WaitForSecondsRealtime(0.5f);
    //    Time.timeScale = 1f;
    //}
    public void Death()
    {
        _animator.SetTrigger("T_Death");
    }

    IEnumerator DamageInvulnerable()
    {
        _isVulnerable = false;
        _spriteRenderer.color = new Color(1f, 1f, 1f, _invAlpha);
        yield return new WaitForSeconds(1.5f);
        _isVulnerable = true;
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }


    public void FreezePosition()
    {
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
    }
    public void RemuseRb()
    {
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        _rb.velocity = Vector3.down * 0.5f;
    }
    public void DoSp02Charge()
    {
        _animator.Play("FinalSpCharge");
        Instantiate(_sp02ChargeFx, transform.position + _sp02ChargeFxOffset, Quaternion.identity);
    }
    public void DoFinishPose()
    {
        _animator.Play("FinishPose");
    }

    public void PlayAttackSE()
    {
        AudioManager.Instance.PlaySE("SE_Player_Attack");
    }
}
