using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// 전투를 담당하는 매니저
// 필드와 턴의 관리
// 필드에 올라와있는 캐릭터의 제어를 배틀매니저에서 담당

public class BattleManager : MonoBehaviour
{
    private BattleDataManager _battleData;
    public BattleDataManager Data => _battleData;
    private Field _field;
    public Field Field => _field;
    private Mana _mana;
    public Mana Mana => _mana;
    private PhaseController _phase;
    public PhaseController Phase => _phase;

    private Vector2 coord;

    public FieldColorType fieldColorType = FieldColorType.none;

    private void Awake()
    {
        _battleData = Util.GetOrAddComponent<BattleDataManager>(gameObject);
        _mana = Util.GetOrAddComponent<Mana>(gameObject);
        _phase = new PhaseController();
    }

    private void Update()
    {
        _phase.OnUpdate();
    }

    public void SetupField()
    {
        GameObject fieldObject = GameObject.Find("Field");

        if (fieldObject == null)
            fieldObject = GameManager.Resource.Instantiate("Field");

        _field = fieldObject.GetComponent<Field>();
    }

    public void SpawnInitialUnit()
    {
        GetComponent<UnitSpawner>().SpawnInitialUnit();
    }

    public void MovePhase()
    {
        BattleUnit unit = Data.GetNowUnit();

        if (Field.Get_Abs_Pos(unit, ClickType.Move).Contains(coord) == false)
            return;
        Vector2 dest = coord - unit.Location;

        
        MoveLocate(unit, dest); //이동시 낙인 체크

        _phase.ChangePhase(_phase.Action);
    }

    public void ActionPhase()
    {
        BattleUnit unit = Data.GetNowUnit();

        if (Field.Get_Abs_Pos(unit, ClickType.Attack).Contains(coord) == false)
            return;

        if (coord != unit.Location)
        {
            List<Vector2> splashRange = unit.GetSplashRange(coord, unit.Location);

            foreach (Vector2 splash in splashRange)
            {
                BattleUnit targetUnit = Field.GetUnit(coord + splash);

                if (targetUnit == null)
                    continue;

                if (targetUnit.Team == Team.Enemy)
                {
                    //공격 전 낙인 체크
                    unit.SkillUse(Field.GetUnit(coord + splash));
                    unit.PassiveCheck(targetUnit, PassiveType.AFTERATTACK); //공격 후 낙인 체크
                }


            }
        }

        Field.ClearAllColor();
        Data.BattleOrderRemove(unit);
        _phase.ChangePhase(_phase.Engage);
        BattleOverCheck();
    }

    public void EngagePhase()
    {
        Field.ClearAllColor();

        if (Data.OrderUnitCount <= 0)
        {
            _phase.ChangePhase(_phase.Prepare);
            return;
        }

        BattleUnit unit = Data.GetNowUnit();
        if (unit.Team == Team.Enemy)
        {
            unit.AI.AIAction();

            Data.BattleOrderRemove(unit);
            BattleOverCheck();

            return;
        }

        _phase.ChangePhase(_phase.Move);
    }

    public void StartPhase()
    {
        if (Field._coloredTile.Count <= 0)
            return;

        if (fieldColorType == FieldColorType.UnitSpawn)
        {
            SpawnUnitOnField();
        }
    }

    public void PreparePhase()
    {
        if (fieldColorType == FieldColorType.UnitSpawn)
        {
            SpawnUnitOnField();
        }
        else if (fieldColorType == FieldColorType.FallPlayerSkill)
        {
            FallUnitOnField();
        }

        // 마나 
    }

    private void SpawnUnitOnField()
    {
        DeckUnit unit = Data.UI_hands.GetSelectedUnit();
        if (Field._coloredTile.Contains(coord) == false)
            return;
        GetComponent<UnitSpawner>().DeckSpawn(unit, coord);
        Mana.ChangeMana(-unit.Stat.ManaCost);
        //배치 시 낙인 체크
        Data.RemoveHandUnit(unit);
        Field.ClearAllColor();
    }

    private void FallUnitOnField()
    {
        if (Field._coloredTile.Contains(coord) == false)
            return;
        Field.GetUnit(coord).Fall.ChangeFall(1);
        _battleData.UI_PlayerSkill.CancleSelect();
        Field.ClearAllColor();
    }

    public void OnClickTile(Tile tile)
    {
        coord = Field.FindCoordByTile(tile);
        _phase.OnClickEvent();
    }

    public void UnitSetting(BattleUnit _unit, Vector2 coord)
    {
        _unit.SetTeam(_unit.Team);
        Field.EnterTile(_unit, coord);
        _unit.UnitDeadAction = UnitDeadAction;

        Data.BattleUnitAdd(_unit);
    }

    private void UnitDeadAction(BattleUnit _unit)
    {
        Data.BattleUnitRemove(_unit);
        Data.BattleOrderRemove(_unit);
    }

    public void BattleOverCheck()
    {
        int MyUnit = 0;
        int EnemyUnit = 0;

        foreach (BattleUnit BUnit in Data.BattleUnitList)
        {
            if (BUnit.Team == Team.Player)//아군이면
                MyUnit++;
            else
                EnemyUnit++;
        }

        MyUnit += Data.PlayerDeck.Count;
        //EnemyUnit 대기 중인 리스트만큼 추가하기

        if (MyUnit == 0)
        {
            Debug.Log("YOU LOSE");
            _phase.ChangePhase(new BattleOverPhase());

        }
        else if (EnemyUnit == 0)
        {
            Debug.Log("YOU WIN");
            _phase.ChangePhase(new BattleOverPhase());
        }

    }

    public void TurnChange()
    {
        if (_phase.Current == _phase.Prepare)
            _phase.ChangePhase(_phase.Engage);
        else
            _phase.ChangePhase(_phase.Prepare);
    }

    // 이동 경로를 받아와 이동시킨다
    private void MoveLocate(BattleUnit caster, Vector2 coord)
    {
        Vector2 current = caster.Location;
        Vector2 dest = current + coord;

        Field.MoveUnit(current, dest);
    }

    public enum FieldColorType
    {
        none,
        UnitSpawn,
        FallPlayerSkill
    }

    public bool UnitSpawnReady(bool b)
    {
        if (_phase.Current != _phase.Prepare)
            return false;

        if (b)
        {
            Field.SetTileColor();
            fieldColorType = FieldColorType.UnitSpawn;
        }
        else
        {
            Field.ClearAllColor();
            fieldColorType = FieldColorType.none;
        }

        return true;
    }

    public bool FallPlayerSkillReady(bool b)
    {
        if (_phase.Current != _phase.Prepare)
            return false;

        if (b)
        {
            Field.SetEnemyUnitTileColor();
            fieldColorType = FieldColorType.FallPlayerSkill;
        }
        else
        {
            Field.ClearAllColor();
            fieldColorType = FieldColorType.none;
        }

        return true;
    }
}