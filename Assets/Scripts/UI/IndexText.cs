using Renderers;
using UnityEngine;
using UnityEngine.UI;

public class IndexText : SerializedDataRenderer<int>
{
    [SerializeField] private Text _text;

    public void SetIndex(int index)
    {
        _text.text = index.ToString();
    }
    public override void SetData(int data, int index)
    {
        base.SetData(data, index);
        SetIndex(data);
    }
    public override void RefreshView()
    {
        
    }

    public override void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}