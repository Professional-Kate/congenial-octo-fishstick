using System;

namespace IdelPog.Repository
{
    public interface IRepositoryHooks<out T>
    {
        public event Action<int, T> OnAdd;
        public event Action<int, T> OnRemove;
        public event Action<int, T> OnGet;
        public event Action<T, T> OnUpdate;
        public event Action<int, bool> OnContains;
    }
}