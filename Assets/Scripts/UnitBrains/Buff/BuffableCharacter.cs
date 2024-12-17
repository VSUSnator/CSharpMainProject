//using System.Collections.Generic;
//using UnityEngine;
//using Model.Config;
//using Model.Config;

//public class BuffableCharacter : MonoBehaviour, IBuffable<BuffableCharacter>, ISpeedBuffable, IAttackSpeedBuffable
//{
//    private List<SpeedBuff> activeSpeedBuffs = new List<SpeedBuff>();
//    private List<SpeedDebuff> activeSpeedDebuffs = new List<SpeedDebuff>();
//    private List<AttackSpeedBuff> activeAttackSpeedBuffs = new List<AttackSpeedBuff>();
//    private List<AttackSpeedDebuff> activeAttackSpeedDebuffs = new List<AttackSpeedDebuff>();

//    private float baseSpeed = 5f;
//    private float currentSpeed;
//    private float baseAttackSpeed = 1f;
//    private float currentAttackSpeed;

//    private void Start()
//    {
//        currentSpeed = baseSpeed; // ������������� ��������� ��������
//        currentAttackSpeed = baseAttackSpeed; // ������������� ��������� �������� �����
//    }

//    public void ApplyBuff(SpeedBuff buff)
//    {
//        activeSpeedBuffs.Add(buff);
//        currentSpeed *= buff.SpeedModifier; // ��������� ����������� ��������
//    }

//    public void RemoveBuff(SpeedBuff buff)
//    {
//        if (activeSpeedBuffs.Remove(buff))
//        {
//            currentSpeed /= buff.SpeedModifier; // ���������� �������� � ��������� ��������
//        }
//    }

//    public void ApplyDebuff(SpeedDebuff debuff)
//    {
//        activeSpeedDebuffs.Add(debuff);
//        currentSpeed *= debuff.SpeedModifier; // ��������� ����������� ��������
//    }

//    public void RemoveDebuff(SpeedDebuff debuff)
//    {
//        if (activeSpeedDebuffs.Remove(debuff))
//        {
//            currentSpeed /= debuff.SpeedModifier; // ���������� �������� � ��������� ��������
//        }
//    }

//    public void ApplyBuff(AttackSpeedBuff buff)
//    {
//        activeAttackSpeedBuffs.Add(buff);
//        currentAttackSpeed *= buff.AttackModifier; // ��������� ����������� �������� �����
//    }

//    public void RemoveBuff(AttackSpeedBuff buff)
//    {
//        if (activeAttackSpeedBuffs.Remove(buff))
//        {
//            currentAttackSpeed /= buff.AttackModifier; // ���������� �������� ����� � ��������� ��������
//        }
//    }

//    public void ApplyDebuff(AttackSpeedDebuff debuff)
//    {
//        activeAttackSpeedDebuffs.Add(debuff);
//        currentAttackSpeed *= debuff.AttackModifier; // ��������� ����������� �������� �����
//    }

//    public void RemoveDebuff(AttackSpeedDebuff debuff)
//    {
//        if (activeAttackSpeedDebuffs.Remove(debuff))
//        {
//            currentAttackSpeed /= debuff.AttackModifier; // ���������� �������� ����� � ��������� ��������
//        }
//    }

//    private void Update()
//    {
//        // ������ ������������� ������� �������� � �������� �����
//        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
//        // ������ ����� � �������������� currentAttackSpeed
//    }
//}