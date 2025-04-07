using UnityEngine;
using UnityEngine.UI;

public class IndexText : MonoBehaviour
{
    [SerializeField] private Text _text;

    public void SetIndex(int index)
    {
        _text.text = index.ToString();
    }
}