using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class StateMachine<T> : MonoBehaviour
	{
		public abstract class State<T>
		{
			protected StateMachine<T> Ctx;

			public T StateId { get; private set; }

			public State(T stateId, StateMachine<T> context)
			{
				StateId = stateId;
				Ctx = context;
			}

			public virtual void In(T fromState)
			{
			}

			public virtual void Out(T toState)
			{
			}

			public virtual void Update()
			{
			}

			protected void To(T state)
			{
				Ctx.To(state);
			}
		}

		public abstract class Transition<T>
		{
			protected StateMachine<T> Ctx;

			public T From { get; private set; }

			public T To { get; private set; }

			public Transition(T from, T to, StateMachine<T> context)
			{
				From = from;
				To = to;
				Ctx = context;
			}

			public virtual void Action()
			{
			}
		}

		private bool _enabled;

		private State<T> _prevState;

		private Dictionary<T, State<T>> _registeredStates;

		private List<Transition<T>> _transitions;

		private int _callStackSize = 10;

		[SerializeField]
		[TextArea(5, 10)]
		private string _callHistory;

		private List<string> _callStack;

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		public State<T> CurrentState { get; private set; }

		public event Action<T, T> OnStateChanged;

		public StateMachine()
		{
			_registeredStates = new Dictionary<T, State<T>>();
			_transitions = new List<Transition<T>>();
		}

		protected StateMachine<T> Register(State<T> state)
		{
			if (_registeredStates.Keys.Contains(state.StateId))
			{
				Debug.LogErrorFormat("state for '{0}' allready exists", state.StateId);
				return this;
			}
			_registeredStates.Add(state.StateId, state);
			return this;
		}

		protected void Clean()
		{
			_registeredStates.Clear();
			_transitions.Clear();
			CurrentState = null;
			_prevState = null;
		}

		protected StateMachine<T> RegisterTransition(Transition<T> transition)
		{
			_transitions.Add(transition);
			return this;
		}

		public void To(T stateId)
		{
			if (!_registeredStates.Keys.Contains(stateId))
			{
				Debug.LogErrorFormat("state for '{0}' not found", stateId);
				return;
			}
			if (CurrentState != null)
			{
				_prevState = CurrentState;
				_prevState.Out(stateId);
				foreach (Transition<T> transition in _transitions)
				{
					if (transition.From.Equals(_prevState.StateId) && transition.To.Equals(stateId))
					{
						transition.Action();
					}
				}
			}
			CurrentState = _registeredStates[stateId];
			CurrentState.In((_prevState == null) ? default(T) : _prevState.StateId);
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(CurrentState.StateId, (_prevState == null) ? default(T) : _prevState.StateId);
			}
		}

		protected virtual void Tick()
		{
			if (Enabled && CurrentState != null)
			{
				CurrentState.Update();
			}
		}
	}
}
