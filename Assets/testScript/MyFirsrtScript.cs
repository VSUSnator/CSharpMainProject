using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirsrtScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(CalcArea(CalcWidth(), 3));
    }
    float CalcWidth()
    {
        return 5;
    }

    float CalcArea(float width, float lenght)
    {
        float area = width * lenght;
        return area;
        
    }        
}
