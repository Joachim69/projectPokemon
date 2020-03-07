using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "fire", menuName = "type")]
public class Type : ScriptableObject
{
    //public new string name;
    public List<Type> vunerabilities = new List<Type> {};
    public List<Type> resistances = new List<Type> { };
    public List<Type> immunities = new List<Type> { };
    public Color themeColor;
    public Color themeAccents;
}
