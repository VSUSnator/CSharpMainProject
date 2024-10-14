    protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
    {
        float overheatTemperature = OverheatTemperature;

        int temp = GetTemperature();
        if (temp >= overheatTemperature) { return; }
        IncreaseTemperature();
        for (int i = 0; i <= temp; i++)
        {
            var projectile = CreateProjectile(forTarget);
            AddProjectileToList(projectile, intoList);
        }

    }

    public override Vector2Int GetNextStep()
    {
    ///////////////////////////////////////
    // Homework 1.4 (1st block, 4rd module)
    ///////////////////////////////////////
        List<Vector2Int> targets = SelectTargets();
        if (targets.Count == 0 || IsTargetInRange(targets[0]))
        {
            return base.GetNextStep();
        }
        else
        {
            return CalcNextStepTowards(targets[0]);
        }
    }

    protected override List<Vector2Int> SelectTargets()
    {
        List<Vector2Int> result = GetAllTargets();
        if (result.Count == 0)
        {
            result.Add(GetEnemyBasePosition());
        }
        else
        {
            Vector2Int closestTarget = result[0];
            float minDistance = DistanceToOwnBase(closestTarget);
            for (int i = 1; i < result.Count; i++)
            {
                float distance = DistanceToOwnBase(result[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = result[i];
                }
            }
            if (!IsTargetInRange(closestTarget)) // цели не в атакующей зоне
            {
                List<Vector2Int> outOfRangeTargets = new List<Vector2Int>();
                outOfRangeTargets.Add(closestTarget); // Использую .Value для получения значения из Nullable
                return outOfRangeTargets;
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
        float t = _cooldownTime / (OverheatCooldown / 10);
        _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
        if (t >= 1)
        {
            _cooldownTime = 0;
            _overheated = false;
        }
    }
}

protected int GetTemperature()
{
    if (_overheated) return (int)OverheatTemperature;
    else return (int)_temperature;
}

protected void IncreaseTemperature()
{
    _temperature += 1f;
    if (_temperature >= OverheatTemperature) _overheated = true;
}
