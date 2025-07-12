using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private Vector2 _vJump = default;
    [SerializeField] private Vector2 _vHitBack = default;
    [SerializeField] private Vector2 _vDeathBlow = default;
    [SerializeField] private int _killScore = default;
    [SerializeField] protected int _killMp = default;
    [SerializeField] private float _jumpInterval = default;
    [SerializeField] private GameObject _slashFx = default;
    [SerializeField] private Vector3 _vSlashFxOffset = default;
    [SerializeField] private Material _origMaterial = default;
    [SerializeField] private Material _whiteMaterial = default;
    private GameObject _GameMasterGo;
    protected GameMasterController _GameMasterController;
    private Vector3 _vFlipX = new Vector3(-1f, 1f, 1f);

    private bool _isGround = false;
    private float _timer = default;
    private Rigidbody2D _rb;
    private GameObject _selfGo;
    protected Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private GameObject _PlayerGo;
    protected PlayerMovementController _pMovementController;
    private PlayerCombatManager _pCombatManager;
    private EnemyDeathBehaviourBase _deathController = default;

    public int KillMp {  get { return _killMp; } set { _killMp = value; } }
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = _vFlipX;
        _PlayerGo = GameObject.Find("Player");
        _pMovementController = _PlayerGo.GetComponent<PlayerMovementController>();
        _pCombatManager = _PlayerGo.GetComponent<PlayerCombatManager>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _timer = 0f;
        _GameMasterGo = GameObject.Find("GameMaster");
        _GameMasterController = _GameMasterGo.GetComponent<GameMasterController>();
        _deathController = GetComponent<EnemyDeathBehaviourBase>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        _animator.SetFloat("P_YSpeed", _rb.velocity.y);
        if (_timer >= _jumpInterval && _isGround)
        {
            _animator.SetTrigger("T_JumpStart");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _isGround = true;
            _animator.SetBool("P_Grounded", true);
            _animator.SetTrigger("T_Land");
            _rb.velocity = Vector2.zero;
            AudioManager.Instance.PlaySE("SE_Enemy_Slime_Land");
        }
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerMovementController>().TakeDamage(1);
            _rb.velocity = Vector2.zero;
            _rb.AddForce(_vHitBack, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerGuardCldr")
        {
            _PlayerGo.GetComponent<PlayerMovementController>().GuardHit();
            _rb.velocity = Vector2.zero;
            _rb.AddForce(_vHitBack, ForceMode2D.Impulse);
        }
        if (collision.gameObject.CompareTag("PlayerAtkCldr") || collision.gameObject.CompareTag("PlayerProjectile"))
        {
            AudioManager.Instance.PlaySE("SE_Player_Attack_Hit");
            Instantiate(_slashFx, transform.position + _vSlashFxOffset, Quaternion.identity);
            Death();
        }
    }

    public void Jump()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Slime_Jump");
        _rb.AddForce(_vJump, ForceMode2D.Impulse);
        //_animator.SetTrigger("T_Jump");
        _animator.SetBool("P_Grounded", false);
        _isGround = false;
        _timer = 0f;
    }

    public void Death()
    {
        //_rb.gravityScale = 1f;
        //_rb.velocity = Vector3.zero;
        //_rb.AddForce(_vDeathBlow, ForceMode2D.Impulse);
        //_spriteRenderer.DOFade(0f, 0.5f).onComplete = () => { Destroy(gameObject); };
        //StartCoroutine(DoFlash());
        _deathController.DoDeath();
        //_animator.SetTrigger("T_Death");
        //_pMovementController.GainMp(_killMp);
        //_GameMasterController.CountEnemyKill(1);
    }

    public void DeathFromAnimator()
    {
        _rb.gravityScale = 1f;
        _rb.velocity = Vector3.zero;
        _rb.AddForce(_vDeathBlow, ForceMode2D.Impulse);
        _spriteRenderer.DOFade(0f, 0.5f).onComplete = () => { Destroy(gameObject); };
        StartCoroutine(DoFlash());
        AudioManager.Instance.PlaySE("SE_Enemy_Slime_Death");
    }
    IEnumerator DoFlash()
    {
        _spriteRenderer.material = _whiteMaterial;
        yield return new WaitForSeconds(0.15f);
        _spriteRenderer.material = _origMaterial;
    }
}
