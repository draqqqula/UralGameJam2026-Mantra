public class ActionEffectInfo : EffectInfo<UnitAction>
{

    public override string Describe()
    {
        if(!Value) return string.Empty;

        var info = string.Join('\n', Value.ActionName, Value.ActionDescription);
        return info;
    }
}