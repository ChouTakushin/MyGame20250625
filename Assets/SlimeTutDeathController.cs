using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTutDeathController : EnemyDeathBehaviourBase
{

    private SlimeController _controller = default;
    private Animator _animator = default;
    private GameMasterController _gameMasterController = default;
    private PlayerMovementController _pMovementController = default;

    private void Start()
    {
        _controller = GetComponent<SlimeController>();
        _animator = GetComponent<Animator>();
        _pMovementController = GameObject.Find("Player").GetComponent<PlayerMovementController>();
        _gameMasterController = GameObject.Find("GameMaster").GetComponent<GameMasterController>();
    }
    public override void DoDeath()
    {
        _animator.SetTrigger("T_Death");
        _pMovementController.GainMp(_controller.KillMp);
        //_gameMasterController.GameStatus = GameMasterController.EnumGameStatus.TutSkeleton;
        _gameMasterController.ShowGoodAndGoNext(GameMasterController.EnumGameStatus.TutSkeleton);
    }

    IEnumerator ShowNext()
    {
        yield return new WaitForSeconds(1f);
        _gameMasterController.GameStatus = GameMasterController.EnumGameStatus.TutSkeleton;
    }
}
