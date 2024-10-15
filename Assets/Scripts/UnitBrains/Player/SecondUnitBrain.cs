﻿using System.Collections.Generic;
using System.Linq;
using Model.Runtime.Projectiles;
using UnityEngine;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private int _shotCount = 0;

        // Новое поле для хранения целей вне зоны досягаемости
        private List<Vector2Int> unreachableTargets = new List<Vector2Int>();
        private float attackRange = 5f;

        protected float GetDistanceToTarget(Vector2Int targetPosition)
        {
            return Vector2Int.Distance(Pos, targetPosition); // Расчет расстояния
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            int temp = GetTemperature();
            if (temp >= overheatTemperature)
            {
                return;
            }
            IncreaseTemperature();
            for (int i = 0; i <= temp; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
        }

        public override Vector2Int GetNextStep()
        {
            Vector2Int position = Vector2Int.zero;
            Vector2Int nextPoition = Vector2Int.right;
            position = position;//.CalcNextStepTowards(nextPoition);
            return base.GetNextStep();
            Vector2Int currentPosition = Unit.Pos; // Предполагаем, что у вас есть ссылка на экземпляр Unit

            List<Vector2Int> targets = GetAllTargets().ToList();

            if (targets.Count > 0)
            {
                Vector2Int target = targets[0];

                if (GetDistanceToTarget(target) > attackRange)
                {
                    // Рассчитываем следующую позицию к цели
                    Vector2Int nextPosition = currentPosition.CalcNextStepTowards(target);
                    return nextPosition; // Возвращаем следующую позицию
                }
            }

            return currentPosition; // Возвращаем текущую позицию, если ничего не изменилось
        }

        protected override float GetDistanceToTarget(Vector2Int targetPosition)
        {
            return Vector2Int.Distance(Unit.Pos, targetPosition); // Рассчитываем расстояние до цели
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> allTargets = GetAllTargets().ToList(); // Получаем в List
            List<Vector2Int> reachableTargets = GetReachableTargets();
            List<Vector2Int> result = new List<Vector2Int>();


            // Если нужно оставить только одну цель
            if (allTargets.Count > 0)
            {
                Vector2Int? closestTarget = null;
                float minDistance = float.MaxValue;

                foreach (Vector2Int target in allTargets)
                {
                    float distance = DistanceToOwnBase(target);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = target;
                    }
                }

                if (closestTarget != null)
                {
                    if (reachableTargets.Contains(closestTarget.Value))
                    {
                        result.Add(closestTarget.Value); // Цель в зоне досягаемости
                    }
                    else
                    {
                        unreachableTargets.Add(closestTarget.Value); // Цель вне зоны досягаемости
                    }
                }
            }
            // Если целей нет, добавляем базу противника в список целей
            if (result.Count == 0)
            {
               // int enemyBaseId = runtimeModel.BotPlayerId; // Получаем ID противника
               // Vector2Int enemyBase = runtimeModel.RoMap.Bases[enemyBaseId]; // Получаем базу противника
               // result.Add(enemyBase); // Добавляем базу в список целей
            }
            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
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
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}