using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WizardController : MonoBehaviour
{
    [SerializeField] private int _killScore = default;
    [SerializeField] protected int _killMp = default;
    [SerializeField] private float _attackInterval = default;
    [SerializeField] private GameObject _slashFx = default;
    [SerializeField] private GameObject _teleportFx = default;
    [SerializeField] private GameObject _bulletPrefab = default;
    [SerializeField] private Vector3 _vBulletCastOffset = default;
    [SerializeField] private Vector3 _vSlashFxOffset = default;
    [SerializeField] private Vector3 _vTeleportFxOffset = default;
    [SerializeField] private Material _origMaterial = default;
    [SerializeField] private Material _whiteMaterial = default;
    [SerializeField] private float _bulletSpeedScale = default;

    private bool _isGround = false;
    protected bool _isDead = false;
    private Vector3 _vFlipX = new Vector3(-1f, 1f, 1f);
    private float _attackTimer = default;
    private Rigidbody2D _rb;
    private GameObject _selfGo;
    protected Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isVulnerable = false;

    private GameObject _PlayerGo;
    protected PlayerMovementController _pMovementController;
    private PlayerCombatManager _pCombatManager;
    private GameObject _GameMasterGo;
    protected GameMasterController _GameMasterController;
    private EnemyDeathBehaviourBase _deathController = default;

    public Rigidbody2D Rb { get { return _rb; } set { _rb = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public int KillMp { get { return _killMp; } }
    void Start()
    {
        transform.localScale = _vFlipX;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _attackTimer = 0;
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0f);
        _PlayerGo = GameObject.Find("Player");
        _pMovementController = _PlayerGo.GetComponent<PlayerMovementController>();
        _GameMasterGo = GameObject.Find("GameMaster");
        _GameMasterController = _GameMasterGo.GetComponent<GameMasterController>();
        _deathController = GetComponent<EnemyDeathBehaviourBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;

        if (_isGround)
        {
            _attackTimer += Time.deltaTime;
            if(_attackTimer > _attackInterval)
            {
                Attack();
                _attackTimer = 0;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && !_isGround)
        {
            _isGround = true;
            _isVulnerable = true;
            GenerateTeleportFx();
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead || !_isVulnerable) return;

        if (collision.gameObject.tag == "PlayerAtkCldr")
        {
            StartCoroutine(DoFlash());
            AudioManager.Instance.PlaySE("SE_Player_Attack_Hit");
            Instantiate(_slashFx, transform.position + _vSlashFxOffset, Quaternion.identity);
            Death();
        }

        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            StartCoroutine(DoFlash());
            AudioManager.Instance.PlaySE("SE_Player_Attack_Hit");
            Instantiate(_slashFx, transform.position + _vSlashFxOffset, Quaternion.identity);
            Death();
        }
    }
    public void Attack()
    {
        _animator.SetTrigger("T_Attack");
    }
    public void CastBullet()
    {
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + _vBulletCastOffset, Quaternion.identity);
        Vector3 bulletVelocity = new Vector3(transform.localScale.x, 0f, 0f) * _bulletSpeedScale;
        bullet.GetComponent<WizardBulletController>().InitOnCast(transform.localScale, bulletVelocity);
        AudioManager.Instance.PlaySE("SE_Enemy_Wizard_Shoot");
    }
    public void Death()
    {
        //_isDead = true;
        //_animator.SetTrigger("T_Death");
        //_pMovementController.GainMp(_killMp);
        //_GameMasterController.CountEnemyKill(1);
        //AudioManager.Instance.PlaySE("SE_Enemy_Wizard_Death");
        _deathController.Doit();
    }
    public void GenerateTeleportFx()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Wizard_Teleport");
        Instantiate(_teleportFx, transform.position + _vTeleportFxOffset, Quaternion.identity);
    }
    public void WizardFadeOut()
    {
        GenerateTeleportFx();
        _spriteRenderer.DOFade(0f, 0.5f).onComplete = () => { Destroy(gameObject); };
    }
    IEnumerator DoFlash()
    {
        _spriteRenderer.material = _whiteMaterial;
        yield return new WaitForSeconds(0.15f);
        _spriteRenderer.material = _origMaterial;
    }
    public void PlayDeathFallSE()
    {
        AudioManager.Instance.PlaySE("SE_Enemy_Wizard_Death_Fall");
    }
}
