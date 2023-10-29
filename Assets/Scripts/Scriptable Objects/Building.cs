using UnityEngine;

[CreateAssetMenu(fileName = "Building Data", menuName = "Scriptables/Building")]
public class Building : ScriptableObject
{
    public Enums.TypeOfBuildins Type;
    public Sprite Sprite;
    [Space]
    public ComponentsToBuild[] AllComponents;
    public float TimeToBuild;
    [Space]
    public float TimeToCraft;
    public Item[] inNodes_C;
    public Item[] outNodes_C;
}

[System.Serializable]
public class ComponentsToBuild
{
    public Item Component;
    public int ComponentCount;
}