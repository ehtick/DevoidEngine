using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Utilities
{
    public class Pool<T> where T : class, new()
    {
        public readonly ConcurrentQueue<T> objects;
        
        public Pool()
        {
            objects = new ConcurrentQueue<T>();
        }


        public T Get()
        {
            if (!objects.TryDequeue(out T result))
            {
                result = new T();
            }
            return result;
        }

        public void Return(ref T obj)
        {
            objects.Enqueue(obj);
            obj = default;
        }

    }
}
