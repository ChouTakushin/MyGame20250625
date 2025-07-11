using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutSkeletonController : MonoBehaviour
{
    [SerializeField] private int _killScore = default;
    [SerializeField] protected int _killMp = default;
    [SerializeField] private int _guardMp = default;
    [SerializeField] private float _moveSpeedScaler = default;
    [SerializeField] private float _attackInterval = default;
    [SerializeField] private float _stunPeriod = default;
    [SerializeField] private GameObject _slashFx = default;
    [SerializeField] private GameObject _blockFx = default;
    private GameObject _GameMasterGo;
    protected GameMasterController _GameMasterController;
    private bool _canMelee = false;
    private bool _isGround = false;
    private bool _inGuardHit = false;
    private bool _isVulnerable = false;
    private bool _isStun = false;
    private float _stunTimer = default;
    protected bool _isDead = false;

    private Vector3 _vFlipX = new Vector3(-1f, 1f, 1f);
    private Vector3 _vBlockFxOffset = default;
    private Vector3 _vSlashFxOffset = default;
    private float _AttackTimer = default;

    private Rigidbody2D _rb;
    protected Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private GameObject _PlayerGo;
    protected PlayerMovementController _pMovementController;

    private CapsuleCollider2D _atkCldr = default;

    public bool CanMelee { get { return _canMelee; } set { _canMelee = value; } }
    public bool InGuardHit { get { return _inGuardHit; } set { _inGuardHit = value; } }
    public bool IsVulnerable { get { return _isVulnerable; } set { _isVulnerable = value; } }
    public bool IsStun { get { return _isStun; } set { _isStun = value; } }
    public Rigidbody2D Rb { get { return _rb; } set { _rb = value; } }
    void Start()
    {
        transform.localScale = _vFlipX;
        _PlayerGo = GameObject.Find("Player");
        _pMovementController = _PlayerGo.GetComponent<PlayerMovementController>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _AttackTimer = _attackInterval;
        _stunTimer = 0f;
        _isStun = false;
        _inGuardHit = false;
        _atkCldr = transform.Find("AttackCldr").GetComponent<CapsuleCollider2D>();
        _vSlashFxOffset = new Vector3(-0.5f, 0.75f, 0f);
        _vBlockFxOffset = new Vector3(-1f, 0.5f, 0f);
        _GameMasterGo = GameObject.Find("GameMaster");
        _GameMasterController = _GameMasterGo.GetComponent<GameMasterController>();
    }

    void Update()
    {
        if (!_canMelee)
        {
            CheckCanMelee();
        }
        _animator.SetBool("P_CanMelee", _canMelee);
        // d’¼ŠÖ˜A
        if (_isStun)
        {
            _stunTimer += Time.deltaTime;
            if (_stunTimer > _stunPeriod)
            {
                StunRecover();
                AttackExit();
            }
        }
        // ‹ßÚUŒ‚ŠÖ˜A
        if (_canMelee)
        {
            _AttackTimer += Time.deltaTime;
            if (_AttackTimer > _attackInterval && !_isStun && !_inGuardHit)
            {
                AttackStart();
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && !_isGround)
        {
            _isGround = true;
            Walk();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead) return;

        if (collision.gameObject.CompareTag("PlayerAtkCldr") || collision.gameObject.CompareTag("PlayerProjectile"))
        {

            if (_isVulnerable)
            {
                Vector2 collisionPos = collision.ClosestPoint(transform.position);
                AudioManager.Instance.PlaySE("SE_Player_Attack_Hit");
                Instantiate(_slashFx, transform.position + _vSlashFxOffset, Quaternion.identity);
                Death();
            }
            else
            {
                Instantiate(_blockFx, transform.position + _vBlockFxOffset, Quaternion.identity);
                GuardHit();
            }
        }
    }
    public void Walk()
    {
        _animator.SetTrigger("T_Walk");
        _rb.velocity = Vector2.left * _moveSpeedScaler;
    }
    public void AddWalkSpeed()
    {
        _rb.velocity = Vector2.left * _moveSpeedScaler;
    }

    public void MeleeAreaIn()
    {
        _canMelee = true;
        _rb.velocity = Vector3.zero;
        _animator.SetTrigger("T_Idle");
    }

    public void Guard()
    {
        _rb.velocity = Vector2.zero;
        _animator.SetTrigger("T_Guard");
    }

    public void GuardHit()
    {
        _inGuardHit = true;
        _rb.velocity = Vector2.zero;
        _animator.SetTrigger("T_GuardHit");
        AudioManager.Instance.PlaySE("SE_GuardHit");
    }


    public void ActivateAttackCldr()
    {
        _atkCldr.enabled = true;
    }

    public void DeactivateAttackCldr()
    {
        _atkCldr.enabled = false;
    }

    public void AttackStart()
    {
        _AttackTimer = 0f;
        _animator.SetTrigger("T_Attack");
    }

    public void AttackExit()
    {
        _isVulnerable = false;
    }

    public void IdleIn()
    {
        _isVulnerable = false;
        _inGuardHit = false;
        _isStun = false;
        if (_canMelee)
        {
            if (_AttackTimer >= _attackInterval)
            {
                AttackStart();
            }
            else
            {
                Guard();
            }
        }
        else
        {
            if (_isGround)
            {
                Walk();
            }
        }
    }
    public void AttackRepelled()
    {
        _animator.SetTrigger("T_Stun");
        _isStun = true;
        _stunTimer = 0f;
        DeactivateAttackCldr();
        //_pMovementController.GainMp(_guardMp);
    }

    public void StunRecover()
    {
        _stunTimer = 0f;
        _AttackTimer = 0f;
        _animator.SetTrigger("T_StunRecover");
    }

    public void SkeletonFadeOut()
    {
        _spriteRenderer.DOFade(0f, 0.5f).onComplete = () => { Destroy(gameObject); };
    }
    private void CheckCanMelee()
    {
        Vector2 vLineEnd = new Vector2(transform.position.x - 2.4f, transform.position.y);
        RaycastHit2D lineHit = Physics2D.Linecast(transform.position, vLineEnd, LayerMask.GetMask("FunctionalColliders"));
        if (lineHit && !_canMelee)
        {
            MeleeAreaIn();
        }
    }
    public void PlayAttackSE()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Skeleton_Attack");
    }
    public void Death()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Skeleton_Death");
        _isDead = true;
        _animator.SetTrigger("T_Death");
        _pMovementController.GainMp(_killMp);
        _GameMasterController.GameStatus = GameMasterController.EnumGameStatus.TutMushroom;
    }
}
