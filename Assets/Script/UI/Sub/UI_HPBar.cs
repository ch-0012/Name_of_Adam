using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    [Header("HP 관련")]
    [SerializeField] private Image _background;
    [SerializeField] private Image _playerBar;
    [SerializeField] private Image _enemyBar;

    [Header("타락 게이지 관련")]
    [SerializeField] private Transform _grid;
    [SerializeField] private GameObject _fallGaugeUnit; // 타락 게이지 각 보석


    private List<UI_FallUnit> _fallGauge = new List<UI_FallUnit>();
    private Team team;

    public void SetHPBar(Team team, Transform trans)
    {
        if (team == Team.Player)
        {
            _playerBar.gameObject.SetActive(true);
            _enemyBar.gameObject.SetActive(false);
        }
        else
        {
            _enemyBar.gameObject.SetActive(true);
            _playerBar.gameObject.SetActive(false);
        }

        this.team = team;
    }

    public void SetFallBar(DeckUnit unit)
    {
        int max = unit.DeckUnitTotalStat.FallMaxCount;
        int current = unit.DeckUnitTotalStat.FallCurrentCount;

        if (max > 6) max = 6; // 스마게까지 타락 Max 6
        
        for (int i = _fallGauge.Count; i<max; i++)
        {
            UI_FallUnit newObject = GameObject.Instantiate(_fallGaugeUnit, _grid).GetComponent<UI_FallUnit>();
            _fallGauge.Add(newObject);

            if (i >= current)
                newObject.EmptyGauge();
        }
    }

    public void RefreshHPBar(float amount)
    {
        if (team == Team.Player)
            _playerBar.fillAmount = amount;
        else
            _enemyBar.fillAmount = amount;
    }

    public void RefreshFallGauge(int current)
    {
        for (int i = 0; i < _fallGauge.Count; i++)
        {
            if (i < current)
                _fallGauge[i].FillGauge();
            else
                _fallGauge[i].EmptyGauge();
        }
    }
}
