using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class IncrementalIndex : IndexText
    {
        [SerializeField] private Button _increment;
        [SerializeField] private Button _decrement;
        public void SetEvent<T>(UnityEvent<T> uiEvent, T sampleData) where T : UiEvents.EventDataBase
        {
            _increment.onClick.RemoveAllListeners();
            _increment.onClick.AddListener(() =>
            {
                var data = sampleData;
                data.up = true;
                uiEvent.Invoke(data);
            });
            _decrement.onClick.RemoveAllListeners();
            _decrement.onClick.AddListener(() =>
            {
                var data = sampleData;
                data.up = false;
                uiEvent.Invoke(data);
            });
        }
    }
}