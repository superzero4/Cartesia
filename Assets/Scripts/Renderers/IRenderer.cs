using Renderers;
using UnityEngine;

namespace Renderers
{
    public interface IRenderer<T>
    {
        T Data { get; set; }

        public void RefreshView();

        public void SetView(T Data)
        {
            this.Data = Data;
            RefreshView();
        }

        public void ToggleVisibility(bool visible);
    }

    public abstract class SerializedDataRenderer<T> :
        MonoBehaviour, IRenderer<T>
    {
        private T _data;

        public T Data
        {
            get => _data;
            set => _data = value;
        }

        public abstract void RefreshView();

        public abstract void ToggleVisibility(bool visible);
    }
}