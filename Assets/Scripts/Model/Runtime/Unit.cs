using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using Model;
using Model.Runtime;
using System.Linq;
using System.Collections.Generic;
using UnitBrains.Pathfinding;
using UnitBrains;
using UnityEngine;
using Utilities;

public class Unit : IReadOnlyUnit
{
    public UnitConfig Config { get; }
    public Vector2Int Pos { get; private set; }
    public int Health { get; private set; }
    public bool IsDead => Health <= 0;
    public BaseUnitPath ActivePath => _brain?.ActivePath;
    public IReadOnlyList<BaseProjectile> PendingProjectiles => _pendingProjectiles;

    private readonly List<BaseProjectile> _pendingProjectiles = new();
    private readonly IReadOnlyRuntimeModel _runtimeModel;
    private readonly BaseUnitBrain _brain;
    private UnitCoordinator _unitCoordinator;

    private float _nextBrainUpdateTime;
    private float _nextMoveTime;
    private float _nextAttackTime;

    private bool _isMoving;

    public float BaseSpeed { get; private set; }
    public float ShootingRadius { get; private set; }
    public bool CanDoubleShot { get; private set; }

    public Unit(UnitConfig config, Vector2Int startPos)
    {
        Config = config;
        Pos = startPos;
        Health = config.MaxHealth;
        _brain = UnitBrainProvider.GetBrain(config);

        if (_brain != null)
        {
            InitializeBrain();
        }
        else
        {
            Debug.LogError($"Brain could not be initialized for unit: {Config.Name}");
        }

        _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();

        BaseSpeed = 5f; // начальная скорость
        ShootingRadius = 10f; // начальный радиус стрельбы
        CanDoubleShot = false; // по умолчанию двойного выстрела нет
    }

    private void InitializeBrain()
    {
        _brain.SetUnit(this);
    }

    public void Initialize(UnitCoordinator unitCoordinator)
    {
        _unitCoordinator = unitCoordinator;
    }

    public void Update(float deltaTime, float time)
    {
        if (IsDead) return;

        // Логика обновления юнита
        if (_nextBrainUpdateTime < time)
        {
            _nextBrainUpdateTime = time + Config.BrainUpdateInterval;
            _brain.Update(deltaTime, time);
        }

        var targetPosition = _brain.GetNextStep();

        if (_nextMoveTime < time && !_isMoving)
        {
            _nextMoveTime = time + Config.MoveDelay;
            Move(targetPosition);
        }

        if (_nextAttackTime < time && Attack())
        {
            _nextAttackTime = time + Config.AttackDelay;
        }
    }

    private bool Attack()
    {
        var projectiles = _brain.GetProjectiles();
        if (projectiles == null || projectiles.Count == 0) return false;

        foreach (var projectile in projectiles)
        {
            _pendingProjectiles.Add(projectile);
        }
        return true;
    }

    public void Move(Vector2Int targetPos)
    {
        var delta = targetPos - Pos;
        if (delta.sqrMagnitude > 2)
        {
            Debug.LogError($"Brain for unit {Config.Name} returned invalid move: {delta}");
            return;
        }

        if (!_runtimeModel.RoMap[targetPos] && !_runtimeModel.RoUnits.Any(u => u.Pos == targetPos))
        {
            Pos = targetPos;
        }
    }

    public void StopMoving()
    {
        _isMoving = false;
    }

    public void StartMoving()
    {
        _isMoving = true;
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    public void ClearPendingProjectiles()
    {
        _pendingProjectiles.Clear();
    }

    public void TakeDamage(int projectileDamage)
    {
        Health -= projectileDamage;
    }

    // Методы для модификации характеристик
    public void SetBaseSpeed(float value)
    {
        BaseSpeed = value;
    }

    public void SetShootingRadius(float value)
    {
        ShootingRadius = value;
    }

    public void EnableDoubleShot()
    {
        CanDoubleShot = true;
    }

    public void DisableDoubleShot()
    {
        CanDoubleShot = false;
    }

    // Публичные методы для применения баффов
    public void ApplySpeedBuff(float amount)
    {
        BaseSpeed += amount;
    }

    public void ApplyShootingRadiusBuff(float amount)
    {
        ShootingRadius += amount;
    }

    public void ApplyDoubleShotBuff()
    {
        EnableDoubleShot();
    }

    public void RemoveSpeedBuff(float amount)
    {
        BaseSpeed -= amount;
    }

    public void RemoveShootingRadiusBuff(float amount)
    {
        ShootingRadius -= amount;
    }

    public void RemoveDoubleShotBuff()
    {
        DisableDoubleShot();
    }
}