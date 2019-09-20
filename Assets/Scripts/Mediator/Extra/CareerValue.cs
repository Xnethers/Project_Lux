using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum attackType
{
    NormalDamage, FirstDamage, SecondDamage, RushDamage, RushAddDamage, AirDamage, ForceMinDamage, ForceDifferenceDamage
}

[CreateAssetMenu]
public class CareerValue : ScriptableObject
{

    //攻擊
    [SerializeField] private float normalDamage;
    [SerializeField] private float firstDamage;
    [SerializeField] private float secondDamage;
    [SerializeField] private float rushDamage;
    [SerializeField] private float rushAddDamage;
    [SerializeField] private float airDamage;
    [SerializeField] private float forceMinDamage;
    [SerializeField] private float forceDifferenceDamage;

    //攻擊CD時間
    [SerializeField] private float secondCD;
    [SerializeField] private float rushingCD;
    [SerializeField] private float airCD;
    [SerializeField] private float forceCD;
    [SerializeField] private float forcingCD;
    public float NormalDamage { get { return normalDamage; } }
    public float FirstDamage { get { return firstDamage; } }
    public float SecondDamage { get { return secondDamage; } }
    public float RushDamage { get { return rushDamage; } }
    public float RushAddDamage { get { return rushAddDamage; } }
    public float AirDamage { get { return airDamage; } }
    public float ForceMinDamage { get { return forceMinDamage; } }
    public float ForceDifferenceDamage { get { return forceDifferenceDamage; } }
    public float SecondCD { get { return secondCD; } }
    public float RushingCD { get { return rushingCD; } }
    public float AirCD { get { return airCD; } }
    public float ForceCD { get { return forceCD; } }
    public float ForcingCD { get { return forcingCD; } }

    public float GetDamageByString(attackType type)
    {
        float value = 1;
        if (type == attackType.NormalDamage)
        { value = NormalDamage; }
        else if (type == attackType.FirstDamage)
        { value = FirstDamage; }
        else if (type == attackType.SecondDamage)
        { value = SecondDamage; }
        else if (type == attackType.RushDamage)
        { value = RushDamage; }
        else if (type == attackType.RushAddDamage)
        { value = RushAddDamage; }
        else if (type == attackType.AirDamage)
        { value = AirDamage; }
        else if (type == attackType.ForceMinDamage)
        { value = ForceMinDamage; }
        else if (type == attackType.ForceDifferenceDamage)
        { value = ForceDifferenceDamage; }
        else
        { }
        return value;
    }
}
