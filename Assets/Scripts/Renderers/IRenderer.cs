using System;
using System.Collections.Generic;
using Renderers;
using UnityEngine;
using UnityEngine.Assertions;

namespace Renderers
{
    public static class IRendererHelpers
    {
        public static void InstantiateRenderersAndRefresh<Rend,T>(List<Rend> renderers, IEnumerable<T> data, Rend prefab,
            Transform parentOfNewRenderers, Action<Rend, T> additionalOnCreate = null, int offsetFromEndChild = 0)
            where Rend : MonoBehaviour, IRenderer<T>
        {
            int i = 0;
            if (renderers == null || prefab == null)
                return;
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
                if (additionalOnCreate != null)
                    additionalOnCreate.Invoke(renderer, d);
                i++;
            }

            for (; i < renderers.Count; i++)
            {
                renderers[i].ToggleVisibility(false);
            }
        }
    }
    public interface IRenderer<T>
    {
        public int Index { get; }

        T Data { get; }

        public void SetData(T data, int index);
        public void RefreshView();
        
        public void ToggleVisibility(bool visible);

        
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

        public virtual void SetData(T data, int index)
        {
            _data = data;
            _index = index;
        }

        public abstract void RefreshView();

        public abstract void ToggleVisibility(bool visible);
    }
}