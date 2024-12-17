using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.Config;

public interface IBuffable<T>
{
    void EnableDoubleShot(float duration);
    void DisableDoubleShot();
    void SetShootingRadius(float newRadius);
    float GetShootingRadius();
}

public class SomeCharacter : MonoBehaviour, IBuffable<SomeCharacter>
{
    private float shootingRadius;
    public bool CanDoubleShot { get; private set; }

    public void EnableDoubleShot(float duration)
    {
        CanDoubleShot = true;
        StartCoroutine(DisableDoubleShotAfterDuration(duration));
    }

    private IEnumerator DisableDoubleShotAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        DisableDoubleShot();
    }

    public void DisableDoubleShot()
    {
        CanDoubleShot = false;
    }

    public void SetShootingRadius(float newRadius)
    {
        shootingRadius = newRadius;
    }

    public float GetShootingRadius()
    {
        return shootingRadius;
    }
}


