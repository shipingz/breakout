using System.Collections.Generic;
using Godot;

public partial class ObjectPool<T> : Node where T : class, IPoolable
{
    private List<T> pool = new List<T>();
    private List<T> activeObjects = new List<T>();
    private List<T> inactiveObjects = new List<T>();
    private PackedScene obejctScene;
    private Node parent;

    public ObjectPool(PackedScene scene, int initialSize, Node parent)
    {
        obejctScene = scene;
        this.parent = parent;
        for (int i = 0; i < initialSize; i++)
        {
            T instance = obejctScene.Instantiate<T>();
            instance.OnDespawn();
            pool.Add(instance);
            inactiveObjects.Add(instance);
            parent.AddChild(instance as Node);
        }
    }

    public virtual T Get()
    {
        if (inactiveObjects.Count > 0)
        {
            GD.Print("Reusing instance from pool.");
            T instance = inactiveObjects[0];
            inactiveObjects.RemoveAt(0);
            activeObjects.Add(instance);
            instance.OnSpawn();
            return instance;
        }
        GD.Print("Pool exhausted, creating new instance.");
        T newInstance = obejctScene.Instantiate<T>();
        pool.Add(newInstance);
        activeObjects.Add(newInstance);
        parent.AddChild(newInstance as Node);
        newInstance.OnSpawn();

        return newInstance;
    }

    public virtual void Return(T obj)
    {
        activeObjects.Remove(obj);
        inactiveObjects.Add(obj);
        obj.OnDespawn();
    }

    public int Active => activeObjects.Count;
    public int Inactive => inactiveObjects.Count;
}