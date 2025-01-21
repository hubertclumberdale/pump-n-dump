using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class CardScriptable : ScriptableObject
{
    public string cardName;
    public CompanyType targetCompany;
    public int bonusValue;
    public int malusValue;
    public CardEffectType effectType;
}