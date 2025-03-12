using System;
using UnityEngine;

[Serializable]
public abstract class Dirty<TSource, TResult>
{
    [SerializeField] private TSource data;

    public TSource Data
    {
        set
        {
            data = value;
            MarkDirty();
        }
        get => data;
    }

    //[SerializeField]
    private TResult result;

    //[SerializeField]
    protected bool dirty = true;

    public void MarkDirty()
    {
        dirty = true;
    }

    public void Edit(TSource newData)
    {
        data = newData;
        MarkDirty();
    }

    public TResult Value => dirty ? (result = Compute(data)) : result;

    public abstract TResult Compute(TSource tSource);
}