using System;
using System.Collections.Generic;

namespace Kickstarter.StateControllers
{
    /// <summary>
    /// A generic state machine for managing state transitions and associated entry/exit actions.
    /// </summary>
    /// <typeparam name="TState">The type representing different states.</typeparam>
    public class StateMachine<TState> where TState : Enum
    {
        
        public enum StateChange
        {
            Entry,
            Exit,
        }

        private TState state;
        public TState State
        {
            get => state;
            set
            {
                if (!stateTransitions[State].Contains(value))
                    return;
                stateExitListeners[state]();
                state = value;
                stateEntryListeners[state]();
            }
        }

        private Dictionary<TState, List<TState>> stateTransitions;
        private Dictionary<TState, Action> stateEntryListeners;
        private Dictionary<TState, Action> stateExitListeners;

        /// <summary>
        /// Represents the builder class for constructing a StateMachine instance.
        /// </summary>
        public class Builder
        {
            private TState initialState;
            private readonly Dictionary<TState, List<TState>> stateTransitions = new Dictionary<TState, List<TState>>();
            private readonly Dictionary<TState, Action> stateExitListeners = new Dictionary<TState, Action>();
            private readonly Dictionary<TState, Action> stateEntryListeners = new Dictionary<TState, Action>();

            /// <summary>
            /// Sets the initial state of the StateMachine being built
            /// </summary>
            /// <param name="initialState">The initial state of the desired StateMachine</param>
            /// <returns></returns>
            public Builder WithInitialState(TState initialState)
            {
                this.initialState = initialState;
                return this;
            }

            /// <summary>
            /// Adds a potential transition from one state to another
            /// </summary>
            /// <param name="from">The state that the StateMachine can leave from</param>
            /// <param name="to">The state that the StateMachine can enter into</param>
            /// <returns></returns>
            public Builder WithTransition(TState from, TState to)
            {
                if (!stateTransitions.ContainsKey(from))
                    stateTransitions.Add(from, new List<TState>());
                stateTransitions[from].Add(to);
                return this;
            }

            /// <summary>
            /// Adds a listener to a state transition
            /// </summary>
            /// <param name="state">The state being monitored</param>
            /// <param name="listener">The listener to be called on state transition</param>
            /// <param name="changeType">The type of transition to listen to: Entry or Exit</param>
            /// <returns></returns>
            public Builder WithTransitionListener(TState state, Action listener, StateChange changeType)
            {
                switch (changeType)
                {
                    case StateChange.Exit:
                        stateExitListeners.TryAdd(state, null);
                        stateExitListeners[state] += listener;
                        break;
                    case StateChange.Entry:
                        stateEntryListeners.TryAdd(state, null);
                        stateEntryListeners[state] += listener;
                        break;
                }
                return this;
            }

            /// <summary>
            /// Constructs a StateMachine instance based on the builder's configurations.
            /// </summary>
            /// <returns>The constructed StateMachine instance.</returns>
            public StateMachine<TState> Build()
            {
                return new StateMachine<TState>
                {
                    state = initialState,
                    stateTransitions = stateTransitions,
                    stateEntryListeners = stateEntryListeners,
                    stateExitListeners = stateExitListeners,
                };
            }
        }
    }
}
