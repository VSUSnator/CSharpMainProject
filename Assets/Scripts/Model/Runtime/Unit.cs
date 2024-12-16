using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;
using View;

namespace Model.Runtime
{
    public class Unit : IReadOnlyUnit, IBuffable
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

        private HashSet<BuffDebuff> _activeBuffsAndDebuffs = new(); // Используйте HashSet для уникальности
        private readonly List<BuffDebuff> _availableBuffsAndDebuffs;

        private float _nextBuffApplicationTime;
        private const float BuffApplicationInterval = 5f;
        private const float AttackRadius = 2f;

        private bool _isMoving; // Флаг, указывающий, движется ли юнит

        public Unit(UnitConfig config, Vector2Int startPos)
        {
            Config = config;
            Pos = startPos;
            Health = config.MaxHealth;
            _brain = UnitBrainProvider.GetBrain(config);
            _brain.SetUnit(this);
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();

            _availableBuffsAndDebuffs = new List<BuffDebuff>
            {
                new SpeedBuff(5f, 1.5f),
                new AttackSpeedBuff(5f, 1.5f),
                new SpeedDebuff(5f, 0.5f),
                new AttackSpeedDebuff(5f, 0.5f)
            };
        }

        public void Initialize(UnitCoordinator unitCoordinator)
        {
            _unitCoordinator = unitCoordinator;
        }

        public void Update(float deltaTime, float time)
        {
            if (IsDead) return;

            UpdateActiveBuffsAndDebuffs(deltaTime);

            if (_nextBrainUpdateTime < time)
            {
                _nextBrainUpdateTime = time + Config.BrainUpdateInterval;
                _brain.Update(deltaTime, time);
            }

            // Получаем следующую позицию для движения
            var targetPosition = _brain.GetNextStep();

            if (_nextMoveTime < time && !_isMoving) // Движение только если юнит не остановлен
            {
                _nextMoveTime = time + Config.MoveDelay;
                Move(targetPosition); // Передаем целевую позицию
            }

            if (_nextAttackTime < time && Attack())
            {
                _nextAttackTime = time + Config.AttackDelay;
            }

            ApplyBuffToAlliesInRange(time);
        }

        private void UpdateActiveBuffsAndDebuffs(float deltaTime)
        {
            foreach (var buffOrDebuff in _activeBuffsAndDebuffs.ToList()) // Используем ToList для безопасной модификации
            {
                buffOrDebuff.Update(deltaTime);
                if (buffOrDebuff.IsExpired())
                {
                    RemoveBuff(buffOrDebuff);
                }
            }
        }

        private void ApplyBuffToAlliesInRange(float time)
        {
            if (_nextBuffApplicationTime < time)
            {
                _nextBuffApplicationTime = time + BuffApplicationInterval;

                foreach (var ally in _unitCoordinator.GetUnitsInRadius(Pos, AttackRadius)
                    .Where(u => u != this && !u.IsDead)) // Исключаем себя и мёртвых союзников
                {
                    var buffToApply = _availableBuffsAndDebuffs.FirstOrDefault();
                    if (buffToApply != null)
                    {
                        ally.ApplyBuff(buffToApply);
                    }
                }
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
            _isMoving = false; // Остановка движения
        }

        public void StartMoving()
        {
            _isMoving = true; // Начало движения
        }

        public bool IsMoving()
        {
            return _isMoving; // Возвращаем состояние движения
        }

        public void ClearPendingProjectiles()
        {
            _pendingProjectiles.Clear();
        }

        public void TakeDamage(int projectileDamage)
        {
            Health -= projectileDamage;
        }

        public void ApplyBuff(BuffDebuff buff)
        {
            if (_activeBuffsAndDebuffs.Add(buff)) // Проверка и добавление в HashSet
            {
                Debug.Log($"Applying Buff: {buff.Name}");
                buff.Apply(this);
            }
            else
            {
                Debug.Log($"{Config.Name} already has a buff of type {buff.GetType().Name}");
            }
        }

        public void ApplyDebuff(BuffDebuff debuff)
        {
            if (_activeBuffsAndDebuffs.Add(debuff)) // Проверка и добавление в HashSet
            {
                Debug.Log($"Applying Debuff: {debuff.Name}");
                debuff.Apply(this);
            }
            else
            {
                Debug.Log($"{Config.Name} already has a debuff of type {debuff.GetType().Name}");
            }
        }

        public void RemoveBuff(BuffDebuff buff)
        {
            if (_activeBuffsAndDebuffs.Remove(buff))
            {
                Debug.Log($"Removing Buff: {buff.Name}");
                buff.Remove(this);
            }
        }

        public void RemoveDebuff(BuffDebuff debuff)
        {
            if (_activeBuffsAndDebuffs.Remove(debuff))
            {
                Debug.Log($"Removing Debuff: {debuff.Name}");
                debuff.Remove(this);
            }
        }

        public IReadOnlyCollection<BuffDebuff> GetActiveBuffs() // Возвращаем коллекцию
        {
            return _activeBuffsAndDebuffs.ToList(); // Можно вернуть как список, если нужно
        }
    }
}