using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class GlowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text tmpText;
    private Material mat;
    private Color originalOutline;

    public Color glowColor = Color.green;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        mat = tmpText.fontMaterial;
        originalOutline = mat.GetColor("_OutlineColor");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mat.SetColor("_OutlineColor", glowColor);
        mat.SetFloat("_OutlineWidth", 0.15f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mat.SetColor("_OutlineColor", originalOutline);
        mat.SetFloat("_OutlineWidth", 0f);
    }
}

