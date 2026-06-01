using Godot;
using System;

public interface ObjectPoolReturnHandler<T> where T : class, IPoolable
{
    public void Return(T obj)
    {
    }
}
