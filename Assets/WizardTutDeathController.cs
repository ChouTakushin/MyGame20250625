using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardTutDeathController : EnemyDeathBehaviourBase
{
    private WizardController _controller = default;
    private Animator _animator = default;
    private GameMasterController _gameMasterController = default;
    private PlayerMovementController _pMovementController = default;

    private void Start()
    {
        _controller = GetComponent<WizardController>();
        _animator = GetComponent<Animator>();
        _pMovementController = GameObject.Find("Player").GetComponent<PlayerMovementController>();
        _gameMasterController = GameObject.Find("GameMaster").GetComponent<GameMasterController>();
    }

    public override void DoDeath()
    {
        _controller.IsDead = true;
        _animator.SetTrigger("T_Death");
        _pMovementController.GainMp(_controller.KillMp);
        AudioManager.Instance.PlaySE("SE_Enemy_Wizard_Death");
        AudioManager.Instance.BgmFadeOut(1f);
        //_gameMasterController.GameStatus = GameMasterController.EnumGameStatus.Start;
        _gameMasterController.ShowGoodAndGoNext(GameMasterController.EnumGameStatus.TutTips);
    }

    IEnumerator ShowNext()
    {
        yield return new WaitForSeconds(1f);
        _gameMasterController.GameStatus = GameMasterController.EnumGameStatus.Start;
    }
}
