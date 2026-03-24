using UnityEngine.EventSystems;

public class IconInfo : EffectInfo<IconText>
{

    public override string Describe()
    {
        if(Value == null) return string.Empty;

        return Value.Info;
    }
}