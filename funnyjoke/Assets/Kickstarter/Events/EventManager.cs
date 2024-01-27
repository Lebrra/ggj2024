using System;
using System.Collections.Generic;

namespace Kickstarter.Events
{
    /// <summary>
    /// Used for sending data between classes without the requirement of directly referencing it.
    /// Very useful for when many objects need to notified with some particular information.
    /// </summary>
    /// <typeparam name="TArgument">The key for the event being used and the type of required parameter.</typeparam>
    public static class EventManager<TArgument>
    {
        private static readonly Dictionary<Type, List<Action<TArgument>>> listeners = new Dictionary<Type, List<Action<TArgument>>>();
        
        /// <summary>
        /// Register a listener to an event according to the parameter type of the listener.
        /// </summary>
        /// <param name="listener">The method to be called when the matching event is triggered.</param>
        public static void Register(Action<TArgument> listener)
        {
            listeners.TryAdd(typeof(TArgument), new List<Action<TArgument>>());
            var typedListeners = listeners[typeof(TArgument)];
            if (typedListeners.Contains(listener))
                return;
            typedListeners.Add(listener);
        }

        /// <summary>
        /// Deregister a listener from the event which shares a parameter type with the listener.
        /// </summary>
        /// <param name="listener">The method being removed from the event with a matching parameter type.</param>
        public static void Deregister(Action<TArgument> listener)
        {
            listeners.TryAdd(typeof(TArgument), new List<Action<TArgument>>());
            var typedListeners = listeners[typeof(TArgument)];
            if (!typedListeners.Contains(listener))
                return;
            typedListeners.Remove(listener);
        }
        
        /// <summary>
        /// Call an event to trigger all the listeners which are currently registered to it.
        /// </summary>
        /// <param name="argument">The information being passed into the event to be received by the listeners.</param>
        public static void Trigger(TArgument argument)
        {
            listeners.TryGetValue(typeof(TArgument), out var typedListeners);
            (typedListeners as List<Action<TArgument>>).ForEach(listener => listener(argument));
        }
    }
}