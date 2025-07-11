using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomDeathController : EnemyDeathBehaviourBase
{
    private MushroomController _controller = default;
    private Animator _animator = default;
    private GameMasterController _gameMasterController = default;
    private PlayerMovementController _pMovementController = default;
    private Rigidbody2D _rb = default;

    private void Start()
    {
        _controller = GetComponent<MushroomController>();
        _animator = GetComponent<Animator>();
        _pMovementController = GameObject.Find("Player").GetComponent<PlayerMovementController>();
        _gameMasterController = GameObject.Find("GameMaster").GetComponent<GameMasterController>();
        _rb = GetComponent<Rigidbody2D>();
    }


    public override void Doit()
    {
        _controller.IsDead = true;
        _rb.velocity = Vector3.zero;
        _controller.DeactivateAttackCldr();
        AudioManager.Instance.PlaySE("SE_Enemy_Mushroom_Death");
        _animator.SetTrigger("T_Death");
        _pMovementController.GainMp(_controller.KillMp);
        _gameMasterController.CountEnemyKill(1);
    }
}
