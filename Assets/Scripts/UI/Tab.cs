using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Button _interactable;
    public bool interactable
    {
        get => _interactable.interactable;
        set => _interactable.interactable = value;
    }
    public void Init(string text)
    {
        _text.text = text;
    }
}
