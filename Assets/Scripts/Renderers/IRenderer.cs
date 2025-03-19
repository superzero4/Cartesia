using System.Collections.Generic;
using Renderers;
using UnityEngine;
using UnityEngine.Assertions;

namespace Renderers
{
    public interface IRenderer<T>
    {
        public int Index { get; }

        T Data { get; }

        public void SetData(T data, int index);
        public void RefreshView();

        public void SetView(T Data, int index)
        {
            SetData(Data, index);
            RefreshView();
        }

        public void ToggleVisibility(bool visible);

        public static void InstantiateRenderersAndRefresh<Rend>(List<Rend> renderers, IEnumerable<T> data, Rend prefab,
            Transform parentOfNewRenderers)
            where Rend : MonoBehaviour, IRenderer<T>
        {
            int i = 0;
            foreach (var d in data)
            {
                Rend renderer;
                if (i >= renderers.Count)
                {
                    Assert.IsTrue(i - renderers.Count == 0);
                    renderer = GameObject.Instantiate(prefab, parentOfNewRenderers);
                    renderers.Add(renderer);
                }
                else
                {
                    renderer = renderers[i];
                }

                renderer.ToggleVisibility(true);
                renderer.SetData(d, i);
                renderer.RefreshView();
                i++;
            }

            for (; i < renderers.Count; i++)
            {
                renderers[i].ToggleVisibility(false);
            }
        }
    }

    public abstract class SerializedDataRenderer<T> :
        MonoBehaviour, IRenderer<T>
    {
        private int _index;

        public int Index
        {
            get => _index;
        }

        private T _data;

        public T Data
        {
            get => _data;
        }

        public void SetData(T data, int index)
        {
            _data = data;
            _index = index;
        }

        public abstract void RefreshView();

        public abstract void ToggleVisibility(bool visible);
    }
}