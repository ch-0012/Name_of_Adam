using System.Collections.Generic;
using UnityEngine;

public class 자애 : Passive
{
    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        List<Vector2> targetCoords = new List<Vector2>();
        targetCoords.Add(caster.Location + Vector2.up);
        targetCoords.Add(caster.Location + Vector2.down);
        targetCoords.Add(caster.Location + Vector2.right);
        targetCoords.Add(caster.Location + Vector2.left);
        targetCoords.Add(caster.Location + new Vector2(-1, -1));
        targetCoords.Add(caster.Location + new Vector2(-1, 1));
        targetCoords.Add(caster.Location + new Vector2(1, 1));
        targetCoords.Add(caster.Location + new Vector2(1, -1));

        List<BattleUnit> targetUnits = BattleManager.Instance.GetArroundUnits(targetCoords);

        foreach (BattleUnit unit in targetUnits)
        {
            if (unit.Team == caster.Team)
                unit.ChangeHP(20);
        }
    }
}