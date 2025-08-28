using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ObjectPoolHandling
{
    public class ObjectPool<T>
    {
        private Queue<T> queue;
        private int maxCount;
        private bool hasInfiniteStorage = false;

        private Func<T> OnCreate;
        private Action<T> OnGet;
        private Action<T> OnReturnToPool;
        private Action<T> OnDestroy;

        public ObjectPool(Func<T> OnCreate, Action<T> OnGet, Action<T> OnReturnToPool, Action<T> OnDestroy, int initialCapacity, bool hasInfiniteStorage = false)
        {
            queue = new Queue<T>();
            this.hasInfiniteStorage = hasInfiniteStorage;
            if(!hasInfiniteStorage)
            {
                this.maxCount = initialCapacity;
            }

            this.OnCreate = OnCreate;
            this.OnGet = OnGet;
            this.OnReturnToPool = OnReturnToPool;
            this.OnDestroy = OnDestroy;

            for(int i = 1; i <= initialCapacity; i++)
            {
                T value = OnCreate.Invoke();
                queue.Enqueue(value);
            }
        }

        public T Get()
        {
            if(queue.Count == 0)
            {
                Debug.LogError("Pool is empty, returning default");
                return default(T);
            }
            T value = queue.Dequeue();
            OnGet?.Invoke(value);
            return value;
        }


        public void ReturnToPool(T value)
        {
            if(!hasInfiniteStorage && queue.Count == maxCount)
            {
                Debug.LogError("Pool is full, can't take anymore.");
                return;
            }

            OnReturnToPool?.Invoke(value);
            queue.Enqueue(value);
        }

        public void Clear()
        {
            while(queue.Count > 0)
            {
                T value = queue.Dequeue();
                OnDestroy?.Invoke(value);
            }
        }
    }
}
