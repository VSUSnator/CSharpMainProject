using Model;
using UnitBrains.Pathfinding;
using System.Collections.Generic;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnityEngine;
using Utilities;

public class BuffSystem : MonoBehaviour
{
    private BuffManager<SomeCharacter> buffManager;
    private List<BuffDebuff<SomeCharacter>> availableBuffs = new List<BuffDebuff<SomeCharacter>>();
    public SomeCharacter someCharacter; // Assign in inspector

    private void Awake()
    {
        buffManager = new BuffManager<SomeCharacter>();
        RegisterBuffs();
    }

    private void RegisterBuffs()
    {
        availableBuffs.Add(new DoubleShotBuff(5f)); // Добавляем двойной выстрел
        availableBuffs.Add(new IncreasedShootingRadiusBuff(5f, 2.0f)); // Увеличение радиуса стрельбы
    }

    private void Update()
    {
        buffManager.UpdateActiveBuffs(Time.deltaTime); // Убедитесь, что вызываете этот метод
    }

    public void ApplyBuff(BuffDebuff<SomeCharacter> buff)
    {
        buffManager.ApplyBuff(someCharacter, buff);
    }

    [ContextMenu("Apply Double Shot Buff")]
    private void ApplyDoubleShotBuff()
    {
        ApplyBuff(availableBuffs.Find(x => x is DoubleShotBuff));
    }
    [ContextMenu("Apply Increased Radius Buff")]
    private void ApplyIncreasedRadiusBuff()
    {
        ApplyBuff(availableBuffs.Find(x => x is IncreasedShootingRadiusBuff));
    }
}