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
//        currentSpeed = baseSpeed; // Устанавливаем начальную скорость
//        currentAttackSpeed = baseAttackSpeed; // Устанавливаем начальную скорость атаки
//    }

//    public void ApplyBuff(SpeedBuff buff)
//    {
//        activeSpeedBuffs.Add(buff);
//        currentSpeed *= buff.SpeedModifier; // Применяем модификатор скорости
//    }

//    public void RemoveBuff(SpeedBuff buff)
//    {
//        if (activeSpeedBuffs.Remove(buff))
//        {
//            currentSpeed /= buff.SpeedModifier; // Возвращаем скорость к исходному значению
//        }
//    }

//    public void ApplyDebuff(SpeedDebuff debuff)
//    {
//        activeSpeedDebuffs.Add(debuff);
//        currentSpeed *= debuff.SpeedModifier; // Применяем модификатор скорости
//    }

//    public void RemoveDebuff(SpeedDebuff debuff)
//    {
//        if (activeSpeedDebuffs.Remove(debuff))
//        {
//            currentSpeed /= debuff.SpeedModifier; // Возвращаем скорость к исходному значению
//        }
//    }

//    public void ApplyBuff(AttackSpeedBuff buff)
//    {
//        activeAttackSpeedBuffs.Add(buff);
//        currentAttackSpeed *= buff.AttackModifier; // Применяем модификатор скорости атаки
//    }

//    public void RemoveBuff(AttackSpeedBuff buff)
//    {
//        if (activeAttackSpeedBuffs.Remove(buff))
//        {
//            currentAttackSpeed /= buff.AttackModifier; // Возвращаем скорость атаки к исходному значению
//        }
//    }

//    public void ApplyDebuff(AttackSpeedDebuff debuff)
//    {
//        activeAttackSpeedDebuffs.Add(debuff);
//        currentAttackSpeed *= debuff.AttackModifier; // Применяем модификатор скорости атаки
//    }

//    public void RemoveDebuff(AttackSpeedDebuff debuff)
//    {
//        if (activeAttackSpeedDebuffs.Remove(debuff))
//        {
//            currentAttackSpeed /= debuff.AttackModifier; // Возвращаем скорость атаки к исходному значению
//        }
//    }

//    private void Update()
//    {
//        // Пример использования текущей скорости и скорости атаки
//        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
//        // Логика атаки с использованием currentAttackSpeed
//    }
//}