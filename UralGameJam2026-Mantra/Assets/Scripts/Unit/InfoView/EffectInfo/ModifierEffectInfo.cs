using System.Collections.Generic;
using System.Linq;

public class ModifierEffectInfo : EffectInfo<List<ModifierEffect>>
{
    public override string Describe()
    {
        if (!Value.Any()) return string.Empty;

        var info = string.Join('\n', Value
            .Select(x => string
            .Join('\n', x.Name, x.Description, $"{x.Turn} ходов осталось")));
        return info;
    }
}