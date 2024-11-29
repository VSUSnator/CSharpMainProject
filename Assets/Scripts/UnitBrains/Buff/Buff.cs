public class SpeedBuff : BuffDebuff
{
    public SpeedBuff(float duration, float speedModifier)
        : base("Speed Buff", duration, speedModifier) { }

    public override void Apply(IBuffable character)
    {
        character.ApplyBuff(this);
    }

    public override void Remove(IBuffable character)
    {
        character.RemoveBuff(this);
    }
}

public class SpeedDebuff : BuffDebuff
{
    public SpeedDebuff(float duration, float speedModifier)
        : base("Speed Debuff", duration, speedModifier) { }

    public override void Apply(IBuffable character)
    {
        character.ApplyDebuff(this);
    }

    public override void Remove(IBuffable character)
    {
        character.RemoveDebuff(this);
    }
}

public class AttackSpeedDebuff : BuffDebuff
{
    public AttackSpeedDebuff(float duration, float attackSpeedModifier)
        : base("Attack Speed Debuff", duration, attackSpeedModifier) { }

    public override void Apply(IBuffable character)
    {
        character.ApplyDebuff(this);
    }

    public override void Remove(IBuffable character)
    {
        character.RemoveDebuff(this);
    }
}

public class AttackSpeedBuff : BuffDebuff
{
    public AttackSpeedBuff(float duration, float attackSpeedModifier)
        : base("Attack Speed Buff", duration, attackSpeedModifier) { }

    public override void Apply(IBuffable character)
    {
        character.ApplyBuff(this);
    }

    public override void Remove(IBuffable character)
    {
        character.RemoveBuff(this);
    }
}


