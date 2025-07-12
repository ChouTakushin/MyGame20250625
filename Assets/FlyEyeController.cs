using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FlyEyeController : MonoBehaviour
{
    [SerializeField] private int _killScore = default;
    [SerializeField] protected int _killMp = default;
    [SerializeField] private float _attackInterval = default;
    [SerializeField] private GameObject _slashFx = default;
    [SerializeField] private Vector3 _vSlashFxOffset = default;
    [SerializeField] protected Vector2 _vHitBack = default;
    [SerializeField] private Material _origMaterial = default;
    [SerializeField] private Material _whiteMaterial = default;
    [SerializeField] private float _speedScale = default;
    [SerializeField] private Vector3 _vPrepareAttack = default;
    private GameObject _GameMasterGo;
    protected GameMasterController _GameMasterController;

    private bool _canAttack = false;
    protected bool _isDead = false;
    private Vector3 _vFlipX = new Vector3(-1f, 1f, 1f);
    private Vector3 _stopPos = default;
    private float _attackTimer = default;
    protected Rigidbody2D _rb;
    private GameObject _selfGo;
    protected Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private GameObject _PlayerGo;
    protected PlayerMovementController _pMovementController;
    private PlayerCombatManager _pCombatManager;
    private _AttackStatus _attackStatus;
    private EnemyDeathBehaviourBase _deathController = default;
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public int KillMp { get { return _killMp; } }
    public Vector2 VHitBack { get { return _vHitBack; } }

    private enum _AttackStatus
    {
        Idle,
        Preparing,
        Attacking,
        AttackLanded
    }

    void Start()
    {
        transform.localScale = _vFlipX;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _PlayerGo = GameObject.Find("Player");
        _pMovementController = _PlayerGo.GetComponent<PlayerMovementController>();
        _attackTimer = _attackInterval;
        _rb.velocity = Vector2.left * _speedScale;
        _attackStatus = _AttackStatus.Idle;
        _GameMasterGo = GameObject.Find("GameMaster");
        _GameMasterController = _GameMasterGo.GetComponent<GameMasterController>();
        _deathController = GetComponent<EnemyDeathBehaviourBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_isDead) return;
        if (_canAttack)
        {
            _attackTimer += Time.deltaTime;
            Debug.DrawLine(transform.position, _stopPos + _vPrepareAttack);
            switch (_attackStatus)
            {
                case _AttackStatus.Idle:
                    if (_attackTimer >= _attackInterval)
                    {
                        PrepareAttack();
                    }
                    break;
                case _AttackStatus.Preparing:
                    CheckAttackPrepared();
                    break;
                case _AttackStatus.AttackLanded:
                    CheckInStopPosition();
                    break;
                default:
                    break;
            }
        }
        else
        {
            CheckFirstInPosition();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(_isDead) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovementController>().TakeDamage(1);
            AttackEnd();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead) return;

        if (collision.gameObject.tag == "PlayerAtkCldr")
        {
            AudioManager.Instance.PlaySE("SE_Player_Attack_Hit");
            Instantiate(_slashFx, transform.position + _vSlashFxOffset, Quaternion.identity);
            Death();
        }
    }

    public void PrepareAttack()
    {
        _attackStatus = _AttackStatus.Preparing;
        _attackTimer = 0;
        _rb.velocity = _vPrepareAttack / 0.5f;
    }

    public void Attack()
    {
        _animator.SetTrigger("T_Attack");
        _attackStatus = _AttackStatus.Attacking;
        Vector2 vAttack = _PlayerGo.transform.position - transform.position;
        _rb.velocity = vAttack / 0.8f;
        AudioManager.Instance.PlaySE("SE_Enemy_FlyEye_Attack");
    }
    public void AttackEnd()
    {
        _animator.SetTrigger("T_Idle");
        _attackStatus = _AttackStatus.AttackLanded;
        Vector2 vReturn = _stopPos - transform.position;
        _rb.velocity = vReturn / 0.6f;
    }

    public void CheckFirstInPosition()
    {
        RaycastHit2D lineHit = Physics2D.Linecast(transform.position, transform.position + Vector3.left, LayerMask.GetMask("FunctionalColliders"));
        Debug.DrawLine(transform.position, transform.position + Vector3.left);
        if(lineHit)
        {
            InPosition();
        }
    }
    public void InPosition()
    {
        _canAttack = true;
        _rb.velocity = Vector2.zero;
        _stopPos = transform.position;
        if(_attackTimer >= _attackInterval)
        {
            PrepareAttack();
        }
        else
        {
            _animator.SetTrigger("T_Idle");
        }
    }

    public void Death()
    {
        _deathController.DoDeath();
    }
    IEnumerator DoFlash()
    {
        _spriteRenderer.material = _whiteMaterial;
        yield return new WaitForSeconds(0.15f);
        _spriteRenderer.material = _origMaterial;
    }
    public void FlyEyeFadeOut()
    {
        _spriteRenderer.DOFade(0f, 0.5f).onComplete = () => { Destroy(gameObject); };
    }
    private void CheckAttackPrepared()
    {
        float distance = Vector2.Distance(transform.position, _stopPos + _vPrepareAttack);
        if(distance < 0.1f)
        {
            Attack();
        }
    }
    private void CheckInStopPosition()
    {
        float distance = Vector2.Distance(transform.position, _stopPos);
        if(distance < 0.03f)
        {
            _rb.velocity = Vector2.zero;
            _attackStatus = _AttackStatus.Idle;
        }
    }
}
