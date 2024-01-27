using System;

namespace Kickstarter.Observer
{
    /// <summary>
    /// Gives the ability to observe changes or events from implementations of the Observable class.
    /// </summary>
    /// <typeparam name="T">The type of argument being notified. Must be an enumeration.</typeparam>
    public interface IObserver<in T> : IObserver
    {
        /// <summary>
        /// Method called by an Observable instance to notify observers of changes or events.
        /// </summary>
        /// <param name="argument">The argument associated with the notification.</param>
        public void OnNotify(T argument);
    }

    public interface IObserver
    {
        
    }
}
