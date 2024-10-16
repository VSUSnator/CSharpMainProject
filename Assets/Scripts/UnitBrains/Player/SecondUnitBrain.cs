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
            // Начальная позиция юнита
            Vector2Int position = Vector2Int.zero; 

            // Предположим, что у вас есть список целей
            List<Vector2Int> targets = new List<Vector2Int>(); 

            // Проверяем, есть ли цели
            if (targets.Count == 0)
            {
                return position; // Нет целей, возвращаем позицию юнита
            }

            // Получаем позицию первой цели
            Vector2Int targetPosition = targets[0]; // Предполагается, что цель - это Vector2Int

            // Проверяем, находится ли цель в области атаки
            if (Vector2Int.Distance(position, targetPosition) <= attackRange) 
            {
                return position; // Цель в области атаки, возвращаем позицию юнита
            }
            else
            {
                // Двигаемся к цели
                return CalcNextStepTowards(targetPosition); 
            }
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
                int enemyBaseId = runtimeModel.BotPlayerId; // Получаем ID противника//error CS1061: 'IReadOnlyRuntimeModel'

                if (runtimeModel.RoMap.Bases.ContainsKey(enemyBaseId))//error CS1061: 'IReadOnlyRuntimeModel'
                {
                    Vector2Int enemyBase = runtimeModel.RoMap.Bases[enemyBaseId]; // Получаем базу противника
                    result.Add(enemyBase); // Добавляем базу в список целей
                }
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

        public Vector2Int CalcNextStepTowards(Vector2Int target)
        {
            // Например, просто двигаемся на один шаг в сторону цели
            Vector2Int currentPosition = Vector2Int.zero; // Здесь используйте реальную позицию юнита
            Vector2Int direction = target - currentPosition;

            if (direction.sqrMagnitude > 1)
            {
                // Нормализуем вектор вручную
                // Получаем длину вектора
                float distance = direction.magnitude;

                // Нормализуем направление
                float normalizedX = direction.x / distance;
                float normalizedY = direction.y / distance;

                // Создаем новый Vector2Int на основе нормализованного направления
                return currentPosition + new Vector2Int(Mathf.RoundToInt(normalizedX), Mathf.RoundToInt(normalizedY));
            }

            // Если расстояние меньше 1, просто возвращаем текущее положение + направление
            return currentPosition + direction;
        }
    }
}