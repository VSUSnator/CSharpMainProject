using UnityEngine;

public abstract class BuffDebuff
{
    public string Name { get; private set; }
    public float Duration { get; private set; }
    public float SpeedModifier { get; private set; }
    private float _elapsedTime;

    protected BuffDebuff(string name, float duration, float speedModifier)
    {
        Name = name;
        Duration = duration;
        SpeedModifier = speedModifier;
        _elapsedTime = 0f;
    }

    public void Update(float deltaTime)
    {
        _elapsedTime += deltaTime;
    }

    public bool IsExpired()
    {
        return _elapsedTime >= Duration;
    }

    public abstract void Apply(IBuffable character);
    public abstract void Remove(IBuffable character);
}