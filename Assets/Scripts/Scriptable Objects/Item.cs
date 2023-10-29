using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptables/Item")]
public class Item : ScriptableObject
{
    public Enums.TypeOfItem Type;
    public Sprite image;
}