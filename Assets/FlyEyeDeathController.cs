using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEyeDeathController : EnemyDeathBehaviourBase
{
    private FlyEyeController _controller = default;
    private Animator _animator = default;
    private GameMasterController _gameMasterController = default;
    private PlayerMovementController _pMovementController = default;
    private Rigidbody2D _rb = default;

    private void Start()
    {
        _controller = GetComponent<FlyEyeController>();
        _animator = GetComponent<Animator>();
        _pMovementController = GameObject.Find("Player").GetComponent<PlayerMovementController>();
        _gameMasterController = GameObject.Find("GameMaster").GetComponent<GameMasterController>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public override void Doit()
    {
        _controller.IsDead = true;
        _rb.velocity = Vector2.zero;
        _rb.gravityScale = 1f;
        _rb.velocity = _controller.VHitBack;
        _animator.SetTrigger("T_Death");
        _pMovementController.GainMp(_controller.KillMp);
        _gameMasterController.CountEnemyKill(1);
        AudioManager.Instance.PlaySE("SE_Enemy_FlyEye_Death");
    }
}
