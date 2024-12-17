using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public static int IdBilling = 0;
        public int Id;

        //private BuffManager<Unit> _buffManager;
        private Vector2Int _notRangeEnemyPosition;
        private List<Vector2Int> allTargetEnemies = new List<Vector2Int>();

        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private const int MaxTarget = 3;

        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;

        public SecondUnitBrain()
        {
            Id = IdBilling++;
            //_buffManager = new BuffManager<Unit>();
        }

        public override string TargetUnitName => "Cobra Commando";

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            if (!_overheated)
            {
                for (int i = 0; i < GetTemperature(); i++)
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }

                if (unit.CanDoubleShot)
                {
                    var secondProjectile = CreateProjectile(forTarget);
                    AddProjectileToList(secondProjectile, intoList);
                    Debug.Log($"Двойной выстрел: создан второй снаряд для юнита на позиции {unit.Pos}.");
                }

                IncreaseTemperature();
            }
        }

        public override Vector2Int GetNextStep()
        {
            return unit.Pos.CalcNextStepTowards(_notRangeEnemyPosition);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            allTargetEnemies.Clear();
            allTargetEnemies = GetAllTargets().ToList();

            Vector2Int targetBaseEnemy = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
            allTargetEnemies.Remove(targetBaseEnemy);

            if (allTargetEnemies.Count > 0)
            {
                SortByDistanceToOwnBase(allTargetEnemies);

                if (allTargetEnemies.Count > MaxTarget)
                {
                    allTargetEnemies = allTargetEnemies.GetRange(0, MaxTarget);
                }

                int idEnemy = (Id - 1) % allTargetEnemies.Count;
                Vector2Int target = allTargetEnemies[idEnemy];
                allTargetEnemies.Clear();

                if (!HasTargetsInRange())
                {
                    _notRangeEnemyPosition = target;
                }
                else
                {
                    allTargetEnemies.Add(target);
                }
            }
            else
            {
                allTargetEnemies.Clear();

                if (!HasTargetsInRange())
                {
                    _notRangeEnemyPosition = targetBaseEnemy;
                }
                else
                {
                    allTargetEnemies.Add(targetBaseEnemy);
                }
            }
            return allTargetEnemies;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += deltaTime; // Используем аргумент метода
                float t = _cooldownTime / (OverheatCooldown / 10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);

                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            return _overheated ? (int)OverheatTemperature : (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature)
            {
                _overheated = true;
            }
        }

        public Vector2Int CalcNextStepTowards(Vector2Int target)
        {
            Vector2Int currentPosition = unit.Pos; // Используйте реальную позицию юнита
            Vector2Int direction = target - currentPosition;

            if (direction.sqrMagnitude < 0.001f) // Проверка на нулевое направление
            {
                return currentPosition; // Возвращаем текущее положение
            }

            if (direction.sqrMagnitude > 1)
            {
                return NormalizeDirection(currentPosition, direction);
            }

            return currentPosition + direction;
        }

        private Vector2Int NormalizeDirection(Vector2Int currentPosition, Vector2Int direction)
        {
            float distance = direction.magnitude;
            float normalizedX = direction.x / distance;
            float normalizedY = direction.y / distance;

            return currentPosition + new Vector2Int(Mathf.RoundToInt(normalizedX), Mathf.RoundToInt(normalizedY));
        }

        private void ApplyBuffsToUnits()
        {
            var doubleShotBuff = new DoubleShotBuff(5f);
            //_buffManager.ApplyBuff(unit, doubleShotBuff); // Передаем юнит
            Debug.Log($"Бафф {doubleShotBuff.Name} применён к юниту на позиции {unit.Pos}.");
        }
    }
}