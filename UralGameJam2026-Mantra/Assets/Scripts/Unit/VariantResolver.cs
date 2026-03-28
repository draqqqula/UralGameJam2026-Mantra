using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;


public class VariantResolver : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private SpriteLibrary _library;
    [SerializeField] private List<SpriteLibraryAsset> _variantsEnemy;
    [SerializeField] private List<SpriteLibraryAsset> _variantsPlayer;

    private void Start()
    {
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        var battle = ServiceLocator.Instance.GetService<BattleManager>();
        var isEnemyTeam = battle.IsEnemyPartyMember(_unit);
        var list = isEnemyTeam ? _variantsEnemy : _variantsPlayer;
        var variant = _unit.Variant;
        if (variant >= list.Count || variant < 0)
        {
            variant = 0;
        }
        _library.spriteLibraryAsset = list[variant];
    }
}