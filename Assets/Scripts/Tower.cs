using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static List<Transform> ActiveTowers = new List<Transform>();

    void OnEnable()
    {
        ActiveTowers.Add(transform);
    }

    void OnDisable()
    {
        ActiveTowers.Remove(transform);
    }
}