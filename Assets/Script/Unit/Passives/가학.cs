using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class 가학 : Passive
{
    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        if (isUsed)
            return;

        caster.ChangedStat.ATK += 3;
        // 현재 DeckUnit Stat 건드림, BattleUnit으로 바꿔야 해 (까먹지마)
        isUsed = true;
    }
}