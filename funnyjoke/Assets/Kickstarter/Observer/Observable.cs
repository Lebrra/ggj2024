using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kickstarter.Observer
{
    /// <summary>
    /// Base class for observable mono-behaviours, facilitating notification updates to registered observers based on defined observer types.
    /// </summary>
    public abstract class Observable : MonoBehaviour
    {
        private readonly Dictionary<Type, List<object>> observerLists = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Registers an observer into relevant observer lists based on implemented observer types.
        /// </summary>
        /// <param name="observer">The observer to be added.</param>
        public void AddObserver<TObserver>(TObserver observer) where TObserver : IObserver
        {
            var observerType = observer.GetType();
            var interfaceTypes = observerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IObserver<>));

            foreach (var interfaceType in interfaceTypes)
            {
                var genericType = interfaceType.GetGenericArguments()[0];

                if (!observerLists.ContainsKey(genericType))
                    observerLists.Add(genericType, new List<object>());

                var observers = GetObserverList(genericType);

                observers.Add(observer);
            }
        }

        /// <summary>
        /// Unregisters an observer from relevant observer lists, preventing further notification updates.
        /// </summary>
        /// <param name="observer">The observer to be removed.</param>
        public void RemoveObserver<T>(T observer)
        {
            var observerType = observer.GetType();
            var interfaceTypes = observerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IObserver<>));

            foreach (var interfaceType in interfaceTypes)
            {
                var genericType = interfaceType.GetGenericArguments()[0];

                if (!observerLists.ContainsKey(genericType))
                    observerLists.Add(genericType, new List<object>());

                var observers = GetObserverList(genericType);

                observers.Remove(observer);
            }
        }
        
        /// <summary>
        /// Notifies registered observers listening for specific data types with matching argument values.
        /// </summary>
        /// <param name="argument">The argument to be passed to observers.</param>
        /// <typeparam name="TType">The type of notification argument (Enum or Observable.Notification).</typeparam>
        protected void NotifyObservers<TType>(TType argument)
        {
            if (!observerLists.ContainsKey(typeof(TType)))
                return;
            var observers = GetObserverList<TType>();

            switch (argument)
            {
                case INotification:
                case Enum:
                    for (int i = observers.Count - 1; i >= 0; i--)
                        if (observers[i] is IObserver<TType> observer)
                            observer.OnNotify(argument);
                    break;
                default:
                    throw new Exception("Invalid Argument Type Used for Notification");
            }
        }
        
        private IList GetObserverList(Type type)
        {
            if (observerLists.TryGetValue(type, out var list))
                return list;
            throw new KeyNotFoundException($"No observers found for type {type.Name}");
        }

        private List<object> GetObserverList<T>()
        {
            return observerLists[typeof(T)];
        }
        
        public interface INotification
        {
            
        }
    }
}