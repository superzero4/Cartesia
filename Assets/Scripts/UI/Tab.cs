using Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Button _interactable;
    public Button Interactable
    {
        get => _interactable;
    }
    public void Init(SelectionMode selectionMode, SelectionModeControl selectionModeControl)
    {
        _text.text = selectionMode.ToString();
        _interactable.onClick.AddListener(() => selectionModeControl.SetSelectionMode(selectionMode));
    }
}
