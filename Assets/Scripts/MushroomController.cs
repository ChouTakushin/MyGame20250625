using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    [SerializeField] private int _killScore = default;
    [SerializeField] protected int _killMp = default;
    [SerializeField] private int _maxHp = default;
    [SerializeField] private float _moveSpeedScaler = default;
    [SerializeField] private float _attackInterval = default;
    [SerializeField] private GameObject _slashFx = default;
    [SerializeField] private Vector3 _vFxOffset = default;
    [SerializeField] private Material _origMaterial = default;
    [SerializeField] private Material _whiteMaterial = default;
    private GameObject _GameMasterGo;
    protected GameMasterController _GameMasterController;
    private bool _canMelee = false;
    private bool _isGround = false;
    private bool _isWalking = false;
    protected bool _isDead = false;
    private int _hp = default;

    private Vector3 _vFlipX = new Vector3(-1f, 1f, 1f);
    private Vector3 _vSlashFxOffset = default;
    private float _AttackTimer = default;

    protected Rigidbody2D _rb;
    private GameObject _selfGo;
    protected Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private GameObject _PlayerGo;
    protected PlayerMovementController _pMovementController;
    private PlayerCombatManager _pCombatManager;

    private BoxCollider2D _atkCldr = default;
    private EnemyDeathBehaviourBase _deathController = default;

    public bool CanMelee { get { return _canMelee; } set { _canMelee = value; } }
    public Rigidbody2D Rb { get { return _rb; } set { _rb = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public int KillMp { get { return _killMp; } }
    void Start()
    {
        transform.localScale = _vFlipX;
        _PlayerGo = GameObject.Find("Player");
        _pMovementController = _PlayerGo.GetComponent<PlayerMovementController>();
        _pCombatManager = _PlayerGo.GetComponent<PlayerCombatManager>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _AttackTimer = _attackInterval;
        _atkCldr = transform.Find("AttackCldr").GetComponent<BoxCollider2D>();
        _vSlashFxOffset = new Vector3(-0.5f, 1f, 0f);
        _hp = _maxHp;
        _GameMasterGo = GameObject.Find("GameMaster");
        _GameMasterController = _GameMasterGo.GetComponent<GameMasterController>();
        _deathController = GetComponent<EnemyDeathBehaviourBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;

        _AttackTimer += Time.deltaTime;
        if (!_canMelee)
        {
            CheckCanMelee();
        }
        // ‹ßÚUŒ‚ŠÖ˜A
        if (_canMelee)
        {
            if (_AttackTimer >= _attackInterval)
            {
                Attack();
                _AttackTimer = 0;
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

        if (collision.gameObject.CompareTag("PlayerAtkCldr"))
        {
            StartCoroutine(DoFlash());
            AudioManager.Instance.PlaySE("SE_Player_Attack_Hit");
            Instantiate(_slashFx, transform.position + _vFxOffset, Quaternion.identity);
            _hp--;
            // flash effect
            if (_hp <= 0)
            {
                Death();
            }
        }
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            StartCoroutine(DoFlash());
            AudioManager.Instance.PlaySE("SE_Player_Attack_Hit");
            Instantiate(_slashFx, transform.position + _vFxOffset, Quaternion.identity);
            _hp -= collision.gameObject.GetComponent<EnergyBladeController>().HitDamage;
            if (_hp <= 0)
            {
                Death();
            }
        }

    }

    public void Death()
    {
        //_isDead = true;
        //_rb.velocity = Vector3.zero;
        //DeactivateAttackCldr();
        //AudioManager.Instance.PlaySE("SE_Enemy_Mushroom_Death");
        //_animator.SetTrigger("T_Death");
        //_pMovementController.GainMp(_killMp);
        //_GameMasterController.CountEnemyKill(1);
        _deathController.Doit();
    }

    public void MeleeAreaIn()
    {
        _canMelee = true;
        _rb.velocity = Vector3.zero;
        _animator.SetTrigger("T_Idle");
    }

    public void IdleIn()
    {
        if (_canMelee)
        {
            if (_AttackTimer >= _attackInterval)
            {
                Attack();
            }
        }
        else
        {
            Walk();
        }
    }
    private void CheckCanMelee()
    {
        Vector2 vLineEnd = new Vector2(transform.position.x - 1.73f, transform.position.y);
        Debug.DrawLine(transform.position, vLineEnd);
        RaycastHit2D lineHit = Physics2D.Linecast(transform.position, vLineEnd, LayerMask.GetMask("FunctionalColliders"));
        if (lineHit && !_canMelee)
        {
            MeleeAreaIn();
        }
    }
    public void Walk()
    {
        _animator.SetTrigger("T_Walk");
        _rb.velocity = Vector2.left * _moveSpeedScaler;
    }
    public void Attack()
    {
        _animator.SetTrigger("T_Attack");
    }
    public void MushroomFadeOut()
    {
        _spriteRenderer.DOFade(0f, 0.5f).onComplete = () => { Destroy(gameObject); };
    }
    public void ActivateAttackCldr()
    {
        _atkCldr.enabled = true;
    }

    public void DeactivateAttackCldr()
    {
        _atkCldr.enabled = false;
    }
    IEnumerator DoFlash()
    {
        _spriteRenderer.material = _whiteMaterial;
        yield return new WaitForSeconds(0.15f);
        _spriteRenderer.material = _origMaterial;
    }

    public void PlayAttackVoice()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Mushroom_Attack_Voice");
    }
    public void PlayAttackSE()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Mushroom_Attack");
    }

    public void PlayStepSE()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Mushroom_Step");
    }
}
