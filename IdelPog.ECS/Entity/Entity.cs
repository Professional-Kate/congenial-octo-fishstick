using System.ComponentModel;
using IdelPog.Infrastructure.Repository;

namespace IdelPog.ECS.Entity
{
    public sealed record Entity : IEntity
    {
        private readonly IRepository<Type, IComponent> _componentRepository;

        public Entity(IRepository<Type, IComponent> components)
        {
            _componentRepository = components;
        }

        public void AddComponent(IComponent component)
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }

        public T GetComponent<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }

        public bool TryGetComponent<T>(out T component) where T : IComponent
        {
            throw new NotImplementedException();
        }
    }
}