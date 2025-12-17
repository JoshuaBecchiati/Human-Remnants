using System.Collections.Generic;
using System.Linq;

public class AbilitySave : SaveableObject
{
    private List<AbilityData> _abilities = new List<AbilityData>();

    public override void SaveState(SaveData save)
    {
        if (AbilityManager.Instance != null)
            _abilities = AbilityManager.Instance.GetAbilites().ToList();

        if (save.abilities != null)
            save.abilities = _abilities;
    }

    public override void LoadState(SaveData save)
    {
        if (save.abilities != null)
            AbilityManager.Instance.SetAbilities(save.abilities);
    }
}
