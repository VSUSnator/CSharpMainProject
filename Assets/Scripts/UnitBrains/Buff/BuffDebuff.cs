using UnityEngine;
public abstract class BuffDebuff<T> where T : IBuffable<T>
{
    public string Name { get; }
    public float Duration { get; }

    public BuffDebuff(string name, float duration)
    {
        Name = name;
        Duration = duration;
    }

    public abstract void Apply(T character);
    public abstract void Remove(T character);
    public abstract bool CanApply(T character);
}