using System.Collections;
using DuloGames.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UICastBar uiCastBar;
    public ProgressBar playerProgressBar;
    public ProgressBar horseProgressBar;
    public AbilityUI horseMountAbility;
    public AbilityUI horseDismountAbility;

    public IEnumerator ShowAbility(AbilityUI abilityUI)
    {
        var spellInfo = new UISpellInfo
        {
            Icon = abilityUI.icon,
            Name = abilityUI.abilityName,
            CastTime = abilityUI.duration
        };
        uiCastBar.Show();
        uiCastBar.StartCasting(spellInfo, spellInfo.CastTime, 0);
        return new WaitForSecondsRealtime(spellInfo.CastTime);
    }
}