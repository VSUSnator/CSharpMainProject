using Model;
using UnitBrains.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnityEngine;
using Utilities;

public class BuffSystem : MonoBehaviour
{
    private BuffManager buffManager;
    private List<BuffDebuff> availableBuffs = new List<BuffDebuff>();

    private void Awake()
    {
        buffManager = new BuffManager();
        ServiceLocator.Register(buffManager); // Регистрация BuffManager

        // Регистрация доступных баффов
        RegisterBuffs();
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<BuffManager>(); // Удаление BuffManager при уничтожении
    }

    private void RegisterBuffs()
    {
        availableBuffs.Add(new SpeedBuff(5f, 1.5f)); // Добавляем бафф скорости
        availableBuffs.Add(new AttackSpeedBuff(5f, 1.5f)); // Добавляем бафф скорости атаки
        availableBuffs.Add(new SpeedDebuff(5f, 0.5f)); // Добавляем дебафф скорости
        availableBuffs.Add(new AttackSpeedDebuff(5f, 0.5f)); // Добавляем дебафф скорости атаки
    }

    public List<BuffDebuff> GetAvailableBuffs()
    {
        return availableBuffs;
    }

    public BuffManager GetBuffManager()
    {
        return buffManager; // Метод для получения BuffManager
    }

    private void Update()
    {
        // Обновляем состояние баффов
        buffManager.Update(Time.deltaTime);
    }
}