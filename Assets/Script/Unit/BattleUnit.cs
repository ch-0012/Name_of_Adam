using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : DeckUnit
{
    [SerializeField] private Team _team;
    public Team Team => _team;

    private SpriteRenderer _renderer;
    private Animator _animator;

    public UnitAIController AI;

    [SerializeField] public UnitHP HP;
    [SerializeField] public UnitFall Fall;
    [SerializeField] public UnitSkill Skill;
    [SerializeField] public List<Passive> Passive = new List<Passive>();

    [SerializeField] Vector2 _location;
    public Vector2 Location => _location;

    // 23.02.16 임시 수정
    private Action<BattleUnit> _UnitDeadAction;
    public Action<BattleUnit> UnitDeadAction
    {
        set { _UnitDeadAction = value; }
    }
    
    public void Init()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        AI.SetCaster(this);
        HP.Init(Stat.HP, Stat.CurrentHP);
        Fall.Init(Stat.Fall);

        _renderer.sprite = Data.Image;
    }

    public void SetTeam(Team team)
    {
        _team = team;

        // 적군일 경우 x축 뒤집기
        _renderer.flipX = (Team == Team.Enemy) ? true : false;
    }

    public void SetLocate(Vector2 coord) {
        _location = coord;
    }
    
    public void UnitDiedEvent()
    {
        _UnitDeadAction(this);
        if (_team == Team.Enemy)
        {
            GameManager.Data.DarkEssenseChage(Data.DarkEssenseDrop);
        }
        Destroy(gameObject);
    }

    public void UnitFallEvent()
    {
        //타락 시 낙인 체크
        if (ChangeTeam() == Team.Enemy)
        {
            Fall.Editfy();
        }
        ChangedStat.CurrentHP = Stat.HP;
        HP.Init(Stat.HP, Stat.CurrentHP);
        Debug.Log($"{Data.name} Fall");
    }

    public Team ChangeTeam(Team team = default)
    {
        if(team != default)
        {
            SetTeam(team);
            return team;
        }

        if (Team == Team.Player)
        {
            SetTeam(Team.Enemy);
            return Team.Enemy;
        }
        else
        {
            SetTeam(Team.Player);
            return Team.Player;
        }

    }

    public void SetPosition(Vector3 dest)
    {
        transform.position = dest;
    }

    public void SkillUse(BattleUnit _unit) {
        if(_unit != null)
        {
            //피격 전 낙인 체크
            Skill.Use(this, _unit);
            //피격 후 낙인 체크
        }
    }                   

    public Stat GetStat(bool buff = true) {
        return Stat;
    }

    public void ChangeHP(int value) {
        HP.ChangeHP(value);
    }

    public void ChangeFall(int value)
    {
        Fall.ChangeFall(value);
    }
    
    public bool GetFlipX() => _renderer.flipX;

    public CutSceneType GetCutSceneType() => CutSceneType.center; // Skill 없어져서 바꿨어요

    public List<Vector2> GetAttackRange()
    {
        List<Vector2> RangeList = new List<Vector2>();

        int Acolumn = 11;
        int Arow = 5;

        for (int i = 0; i < Data.AttackRange.Length; i++)
        {
            if (Data.AttackRange[i])
            {
                int x = (i % Acolumn) - (Acolumn >> 1);
                int y = (i / Acolumn) - (Arow >> 1);

                Vector2 vec = new Vector2(x, y);

                RangeList.Add(vec);
            }
        }

        return RangeList;
    }

    public List<Vector2> GetMoveRange()
    {
        List<Vector2> RangeList = new List<Vector2>();

        int Mrow = 5;
        int Mcolumn = 5;

        for (int i = 0; i < Data.MoveRange.Length; i++)
        {
            if (Data.MoveRange[i])
            {
                int x = (i % Mcolumn) - (Mcolumn >> 1);
                int y = -((i / Mcolumn) - (Mrow >> 1));

                Vector2 vec = new Vector2(x, y);

                RangeList.Add(vec);
            }
        }

        return RangeList;
    }

    public List<Vector2> GetSplashRange(Vector2 target, Vector2 caster)
    {
        List<Vector2> SplashList = new List<Vector2>();

        int Scolumn = 11;
        int Srow = 5;

        for (int i = 0; i < Data.SplashRange.Length; i++)
        {
            if (Data.SplashRange[i])
            {
                int x = (i % Scolumn) - (Scolumn >> 1);
                int y = (i / Scolumn) - (Srow >> 1);

                if ((target - caster).x > 0) SplashList.Add(new Vector2(x, y)); //오른쪽
                else if ((target - caster).x < 0) SplashList.Add(new Vector2(-x, y)); //왼쪽
                else if ((target - caster).y > 0) SplashList.Add(new Vector2(y, x)); //위쪽
                else if ((target - caster).y < 0) SplashList.Add(new Vector2(-y, x)); //아래쪽
            }
        }
        return SplashList;
    }


    // 낙인 타입에 따라 낙인 내용 실행하는 함수 BattleManager나 BattleUnit 혹은 제 3자에 넣을 지 고민 중
    public void PassiveCheck(BattleUnit receiver, PassiveType type)
    {
        if(type == PassiveType.BEFOREATTACKED || type == PassiveType.AFTERATTACKED || type == PassiveType.FALLED)
        {
            foreach (Passive passive in receiver.Passive)
            {
                if (passive.GetPassiveType() == type)
                {
                    passive.Use(this, receiver);
                }
            }
        }
        else
        {
            foreach(Passive passive in Passive)
            {
                if (passive.GetPassiveType() == type)
                {
                    passive.Use(this, receiver);
                }
            }   
        }
    }


}
