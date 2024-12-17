using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;
using View;

public class DoubleShotBuff : BuffDebuff<SomeCharacter>
{
    public DoubleShotBuff(float duration) : base("Double Shot Buff", duration) { }

    public override void Apply(SomeCharacter character)
    {
        character.EnableDoubleShot(Duration);
    }

    public override void Remove(SomeCharacter character)
    {
        character.DisableDoubleShot();
    }

    public override bool CanApply(SomeCharacter character)
    {
        return !character.CanDoubleShot;
    }
}

public class IncreasedShootingRadiusBuff : BuffDebuff<SomeCharacter>
{
    private float rangeModifier;

    public IncreasedShootingRadiusBuff(float duration, float rangeModifier)
        : base("Range Increase Buff", duration)
    {
        this.rangeModifier = rangeModifier;
    }

    public override void Apply(SomeCharacter character)
    {
        character.SetShootingRadius(character.GetShootingRadius() + rangeModifier);
    }

    public override void Remove(SomeCharacter character)
    {
        character.SetShootingRadius(character.GetShootingRadius() - rangeModifier);
    }
    public override bool CanApply(SomeCharacter character)
    {
        // Проверка, например, что текущий радиус стрельбы ниже максимального
        return true; // Можно добавить более сложную логику
    }
}