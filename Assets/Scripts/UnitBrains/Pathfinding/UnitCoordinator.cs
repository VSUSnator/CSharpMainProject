using System.Collections.Generic;
using System.Linq;
using Model.Runtime; // Убедитесь, что это пространство имен правильно указано
using Model.Runtime.ReadOnly; // Для IReadOnlyRuntimeModel
using UnityEngine;
using Utilities;
using Model;
using Model.Config;
using View;

public class UnitCoordinator : MonoBehaviour
{
    public static UnitCoordinator Instance { get; private set; }

    private IReadOnlyRuntimeModel _runtimeModel;
    private TimeUtil _timeUtil;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Получаем зависимости
        _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
        _timeUtil = TimeUtil.Create(); // Создаем или получаем существующий экземпляр TimeUtil
    }

    public Vector2Int RecommendedTarget { get; private set; }
    public Vector2Int RecommendedPosition { get; private set; }

    private void UpdateRecommendations(int attackRadius, int mapHalf)
    {
        // Получаем врагов из IReadOnlyRuntimeModel
        var enemyUnits = _runtimeModel.RoUnits
            .Where(unit => unit is Unit)  // Приводим к типу Unit
            .Cast<Unit>() // Приводим к типу Unit
            .ToList(); // Сохраняем полные объекты, а не только позиции

        // Предполагается, что базу можно получить с помощью класса, который имеет свойство Pos
        var playerBase = _runtimeModel.RoBases[RuntimeModel.PlayerId] as Base; // Приводим к типу Base
        var playerBasePos = playerBase?.Pos ?? Vector2Int.zero; // Получаем позицию базы игрока

        var enemiesOnOurSide = enemyUnits.Where(unit => unit.Pos.x < mapHalf).ToList();

        if (enemiesOnOurSide.Count > 0)
        {
            // Есть враги на нашей половине карты
            RecommendedTarget = GetClosestEnemy(enemiesOnOurSide, playerBasePos);
            RecommendedPosition = playerBasePos + new Vector2Int(-1, 0); // Точка перед базой
        }
        else
        {
            // Враги на другой стороне
            RecommendedTarget = GetWeakestEnemy(enemyUnits);
            RecommendedPosition = RecommendedTarget + new Vector2Int(0, 1); // Точка на расстоянии выстрела от ближайшего врага
        }
    }

    private Vector2Int GetClosestEnemy(List<Unit> enemies, Vector2Int playerBase)
    {
        return enemies.OrderBy(unit => Vector2Int.Distance(unit.Pos, playerBase)).FirstOrDefault()?.Pos ?? Vector2Int.zero;
    }

    private Vector2Int GetWeakestEnemy(List<Unit> enemies)
    {
        // Здесь необходимо определить логику получения врага с наименьшим здоровьем
        return enemies
            .Where(unit => !unit.IsDead) // Убедимся, что юнит не мертв
            .OrderBy(unit => unit.Health) // Сортируем по здоровью
            .FirstOrDefault()?.Pos ?? Vector2Int.zero; // Возвращаем позицию или ноль, если врагов нет
    }
}