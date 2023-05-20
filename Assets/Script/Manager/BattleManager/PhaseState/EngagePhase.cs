public class EngagePhase : Phase
{
    private UI_Info _engageInfo;

    public override void OnStateEnter()
    {
        GameManager.Sound.Play("Stage_Transition/Engage/EngageEnter");
        if (BattleManager.Phase.Current != BattleManager.Phase.BattleOver)
            BattleManager.Instance.EngagePhase();

        BattleUnit unit = BattleManager.Data.GetNowUnit();
        if (unit != null)
        {
            _engageInfo = GameManager.UI.ShowPopup<UI_Info>();
            _engageInfo.Set(unit.DeckUnit, unit.Team, unit.HP.GetCurrentHP(), unit.Fall.GetCurrentFallCount());
        }
        BattleManager.BattleUI.ChangeButtonName();
    }

    public override void OnStateUpdate()
    {
        
    }

    public override void OnStateExit()
    {
        GameManager.UI.ClosePopup(_engageInfo);
    }
}