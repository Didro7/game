
using UnityEngine;

[CreateAssetMenu(
    fileName = "ItemStats",
    menuName = "Coin Adventure/Item Stats"
)]
public class ItemStats : ScriptableObject
{
    [Header("Coin Settings")]
    [Min(0)]
    public int coinReward = 1;
}
