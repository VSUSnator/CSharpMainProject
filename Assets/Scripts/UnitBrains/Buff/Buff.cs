//public class SpeedBuff<T> : BuffDebuff<T> where T : IBuffable<T>
//{
//    public SpeedBuff(float duration, float speedModifier)
//        : base("Speed Buff", duration, speedModifier, 0) { }

//    public override void Apply(T character)
//    {
//        character.ApplyBuff(this); // ��������� ����
//    }

//    public override void Remove(T character)
//    {
//        character.RemoveBuff(this); // ������� ����
//    }

//    public override bool CanApply(T character)
//    {
//        // ����� �� ������ �������� ������ ��������
//        // ��������, �������� �� ������� ������� ����� ��� ��������� �����
//        return true; // ��� �������, ���������� true
//    }
//}

//public class AttackSpeedBuff<T> : BuffDebuff<T> where T : IBuffable<T>
//{
//    public AttackSpeedBuff(float duration, float attackSpeedModifier)
//        : base("Attack Speed Buff", duration, 0, attackSpeedModifier) { }

//    public override void Apply(T character)
//    {
//        character.ApplyBuff(this);
//    }

//    public override void Remove(T character)
//    {
//        character.RemoveBuff(this);
//    }

//    public override bool CanApply(T character)
//    {
//        // ����� �� ������ �������� ������ ��������
//        // ��������, �������� �� ������� ������� ����� ��� ��������� �����
//        return true; // ��� �������, ���������� true
//    }
//}

//// ���������� ��� ��������
//public class SpeedDebuff<T> : BuffDebuff<T> where T : IBuffable<T>
//{
//    public SpeedDebuff(float duration, float speedModifier)
//        : base("Speed Debuff", duration, speedModifier, 0) { }

//    public override void Apply(T character)
//    {
//        character.ApplyDebuff(this);
//    }

//    public override void Remove(T character)
//    {
//        character.RemoveDebuff(this);
//    }

//    public override bool CanApply(T character)
//    {
//        // ����� �� ������ �������� ������ ��������
//        // ��������, �������� �� ������� ������� ����� ��� ��������� �����
//        return true; // ��� �������, ���������� true
//    }
//}

//public class AttackSpeedDebuff<T> : BuffDebuff<T> where T : IBuffable<T>
//{
//    public AttackSpeedDebuff(float duration, float attackSpeedModifier)
//        : base("Attack Speed Debuff", duration, 0, attackSpeedModifier) { }

//    public override void Apply(T character)
//    {
//        character.ApplyDebuff(this);
//    }

//    public override void Remove(T character)
//    {
//        character.RemoveDebuff(this);
//    }

//    public override bool CanApply(T character)
//    {
//        // ����� �� ������ �������� ������ ��������
//        // ��������, �������� �� ������� ������� ����� ��� ��������� �����
//        return true; // ��� �������, ���������� true
//    }
//}