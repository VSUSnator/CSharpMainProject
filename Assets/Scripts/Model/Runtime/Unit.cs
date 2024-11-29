using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;

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
        private IReadOnlyRuntimeModel _runtimeModel;
        private BaseUnitBrain _brain;
        private UnitCoordinator _unitCoordinator; // Поле для координатора

        private float _nextBrainUpdateTime = 0f;
        private float _nextMoveTime = 0f;
        private float _nextAttackTime = 0f;

        private float _nextBuffDebuffTime = 0f; // Время для следующего применения баффа/дебаффа
        private float _buffDebuffInterval = 3f; // Интервал применения (например, каждые 3 секунды)
        private List<BuffDebuff> _availableBuffsAndDebuffs; // Список доступных баффов и дебаффов
        private List<BuffDebuff> _activeBuffsAndDebuffs = new List<BuffDebuff>(); // Список активных баффов и дебаффов

        public Unit(UnitConfig config, Vector2Int startPos)
        {
            Config = config;
            Pos = startPos;
            Health = config.MaxHealth;
            _brain = UnitBrainProvider.GetBrain(config);
            _brain.SetUnit(this);
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();

            // Инициализация доступных баффов и дебаффов
            _availableBuffsAndDebuffs = new List<BuffDebuff>
            {
                new SpeedBuff(5f, 1.5f),
                new AttackSpeedBuff(5f, 1.5f),
                new SpeedDebuff(5f, 0.5f),
                new AttackSpeedDebuff(5f, 0.5f)
            };
        }

        // Новый метод инициализации для установки координатора
        public void Initialize(UnitCoordinator unitCoordinator)
        {
            _unitCoordinator = unitCoordinator;
        }

        public void Update(float deltaTime, float time)
        {
            if (IsDead)
                return;

            // Обновляем все активные баффы и дебаффы
            for (int i = _activeBuffsAndDebuffs.Count - 1; i >= 0; i--)
            {
                var buffOrDebuff = _activeBuffsAndDebuffs[i];
                buffOrDebuff.Update(deltaTime); // Обновляем время

                if (buffOrDebuff.IsExpired())
                {
                    buffOrDebuff.Remove(this);
                    Debug.Log($"{Config.Name} removed {buffOrDebuff.Name} due to expiration.");
                    _activeBuffsAndDebuffs.RemoveAt(i); // Удаляем из списка активных эффектов
                }
            }

            // Логика применения баффов и дебаффов
            if (_nextBuffDebuffTime < time)
            {
                _nextBuffDebuffTime = time + _buffDebuffInterval;
                ApplyRandomBuffOrDebuff();
            }

            // Остальная логика обновления
            if (_nextBrainUpdateTime < time)
            {
                _nextBrainUpdateTime = time + Config.BrainUpdateInterval;
                _brain.Update(deltaTime, time);
            }

            if (_nextMoveTime < time)
            {
                _nextMoveTime = time + Config.MoveDelay;
                Move();
            }

            if (_nextAttackTime < time && Attack())
            {
                _nextAttackTime = time + Config.AttackDelay;
            }
        }

        // Метод для применения случайного баффа или дебаффа
        private void ApplyRandomBuffOrDebuff()
        {
            if (_availableBuffsAndDebuffs.Count == 0)
                return;

            // Выбор случайного баффа или дебаффа
            int randomIndex = Random.Range(0, _availableBuffsAndDebuffs.Count);
            var selectedBuffOrDebuff = _availableBuffsAndDebuffs[randomIndex];

            selectedBuffOrDebuff.Apply(this);
            Debug.Log($"{Config.Name} applied {selectedBuffOrDebuff.Name}");
            _activeBuffsAndDebuffs.Add(selectedBuffOrDebuff); // Добавляем в активные
        }

        private bool Attack()
        {
            var projectiles = _brain.GetProjectiles();
            if (projectiles == null || projectiles.Count == 0)
                return false;

            _pendingProjectiles.AddRange(projectiles);
            return true;
        }

        private void Move()
        {
            var targetPos = _brain.GetNextStep();
            var delta = targetPos - Pos;
            if (delta.sqrMagnitude > 2)
            {
                Debug.LogError($"Brain for unit {Config.Name} returned invalid move: {delta}");
                return;
            }

            if (_runtimeModel.RoMap[targetPos] ||
                _runtimeModel.RoUnits.Any(u => u.Pos == targetPos))
            {
                return;
            }

            Pos = targetPos;
        }

        public void ClearPendingProjectiles()
        {
            _pendingProjectiles.Clear();
        }

        public void TakeDamage(int projectileDamage)
        {
            Health -= projectileDamage;
        }

        // Реализация методов интерфейса IBuffable
        public void ApplyBuff(BuffDebuff buff)
        {
            // Логика применения баффа
            Debug.Log($"Applying Buff: {buff.Name}");
        }

        public void RemoveBuff(BuffDebuff buff)
        {
            // Логика удаления баффа
            Debug.Log($"Removing Buff: {buff.Name}");
        }

        public void ApplyDebuff(BuffDebuff debuff)
        {
            // Логика применения дебаффа
            Debug.Log($"Applying Debuff: {debuff.Name}");
        }

        public void RemoveDebuff(BuffDebuff debuff)
        {
            // Логика удаления дебаффа
            Debug.Log($"Removing Debuff: {debuff.Name}");
        }
    }
}