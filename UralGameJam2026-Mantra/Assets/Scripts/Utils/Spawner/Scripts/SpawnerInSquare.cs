using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SpawnerInSquare : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public T Spawn<T>(T prefab) where T : MonoBehaviour
    {
        var rect = _rectTransform.rect;
        var randomX = Random.Range(rect.xMin, rect.xMax);
        var randomY = Random.Range(rect.yMin, rect.yMax);

        var spawnedObj = Instantiate(prefab, _rectTransform);
        var rt = spawnedObj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(randomX, randomY);

        return spawnedObj;
    }
}