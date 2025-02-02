using UnityEngine;

public abstract class Buff : MonoBehaviour
{
    protected BuffEnum _buffEnum;
    public BuffEnum BuffEnum => _buffEnum;

    protected string _name;
    public string Name => _name;

    protected Sprite _sprite;
    public Sprite Sprite => _sprite;

    protected string _description;
    public string Description => _description;

    protected int _count;
    public int Count => _count;

    protected ActiveTiming _countDownTiming;
    public ActiveTiming CountDownTiming => _countDownTiming;

    protected ActiveTiming _buffActiveTiming;
    public ActiveTiming BuffActiveTiming => _buffActiveTiming;

    protected bool _passiveBuff;
    public bool PassiveBuff => _passiveBuff;

    public abstract void Init();
    public abstract void Active(BattleUnit unit = null);

    public abstract void Stack();

    public abstract Stat GetBuffedStat();

    public void CountChange(int num)
    {
        _count += num;
    }
}