using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TempStageStorage
{
    public Stage[] Stages = new Stage[3];
}

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager : MonoBehaviour
{
    // public Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();
    
    private List<Stage> _stageInfo;
    public List<Stage> StageInfo { get { StageDataInit(); return _stageInfo; } set { _stageInfo = value; } }
    public List<Stage> LocalStageInfo;
    public List<MapSign> MapList;

    public Stage[] StageArray = new Stage[3];

    public StageDataContainer StageDatas;
    public StageSpawnData CurrentStageData;
    public List<TempStageStorage> SmagaMap;
    public List<Stage> SmagaRandomStage;

    [SerializeField] public GameData GameData;
    [SerializeField] public GameData GameDataOriginal;

    public Dictionary<string, List<Script>> ScriptData = new Dictionary<string, List<Script>>();
    public PassiveManager Passive;

    public void Init()
    {
        // StatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
        StageDatas = LoadJson<StageDataContainer>("StageData");
        ScriptData = LoadJson<ScriptLoader, string, List<Script>>("Script").MakeDict();
        Passive = new PassiveManager();

        _darkEssense = GameData.DarkEssence;
        _playerSkillCount = GameData.Incarna.PlayerSkillCount;
    }

    public void DeckClear()
    {
        GameData.Incarna = GameDataOriginal.Incarna;
        GameData.Money = GameDataOriginal.Money;
        GameData.DarkEssence = GameDataOriginal.DarkEssence;
        GameData.PlayerSkillCount = GameDataOriginal.PlayerSkillCount;
        GameData.DeckUnits = GameDataOriginal.DeckUnits;
        GameData.isVisitUpgrade = GameDataOriginal.isVisitUpgrade;
        GameData.isVisitStigma = GameDataOriginal.isVisitStigma;

        foreach (DeckUnit unit in GameData.DeckUnits)
        {
            unit.DeckUnitChangedStat.ClearStat();
            unit.DeckUnitUpgradeStat.ClearStat();
            unit.Stigma.Clear();
        }
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = GameManager.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    T LoadJson<T>(string path)
    {
        TextAsset textAsset = GameManager.Resource.Load<TextAsset>($"Data/{path}");

        return JsonUtility.FromJson<T>(textAsset.text);
    }

    [SerializeField] private List<DeckUnit> _playerDeck = new();
    public List<DeckUnit> PlayerDeck => _playerDeck;

    public void AddDeckUnit(DeckUnit unit)
    {
        GameData.DeckUnits.Add(unit);
    }

    public void RemoveDeckUnit(DeckUnit unit)
    {
        GameData.DeckUnits.Remove(unit);
    }

    public List<DeckUnit> GetDeck() 
    {
        return GameData.DeckUnits;
    }

    public void SetDeck(List<DeckUnit> deck)
    {
        GameData.DeckUnits = deck;
    }
    
    public void StageDataInit()
    {
        if (_stageInfo == null)
        {
            _stageInfo = new List<Stage>();
            LocalStageInfo = new List<Stage>();
            MapList = new List<MapSign>();
        }
    }

    private int _money;
    public int Money => _money;

    public bool MoneyChage(int cost)
    {
        if (_money + cost < 0)
        {
            return false;
        }
        else
        {
            _money += cost;
            return true;
        }
    }

    private int _darkEssense = 0;
    public int DarkEssense => _darkEssense;

    public bool DarkEssenseChage(int cost)
    {
        Debug.Log("Dark Essense: " + _darkEssense + " Change: " + cost);
        if (_darkEssense + cost < 0)
        {
            return false;
        }
        else
        {
            _darkEssense += cost;
            return true;
        }
    }

    public bool CanUseDarkEssense(int value)
    {
        if (_darkEssense >= value)
            return true;

        Debug.Log("not enough Dark Essense");
        return false;
    }

    private int _playerSkillCount = 0;
    public int PlayerSkillCount => _playerSkillCount;

    public bool PlayerSkillCountChage(int cost)
    {
        Debug.Log("Player Skill Count: " + _playerSkillCount + " Change: " + cost);
        if (_playerSkillCount + cost < 0)
        {
            return false;
        }
        else
        {
            _playerSkillCount += cost;
            return true;
        }
    }

    public List<PlayerSkill> GetPlayerSkillList() => GameData.Incarna.PlayerSkillList;
}
