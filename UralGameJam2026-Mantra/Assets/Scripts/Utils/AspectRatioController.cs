using UnityEngine;
using UnityEngine.U2D;

[ExecuteAlways]
public class AspectRatioController : MonoBehaviour
{
    public float targetAspect = 16f / 9f;
    private Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Чёрные полосы сверху/снизу
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // Чёрные полосы по бокам
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }
}