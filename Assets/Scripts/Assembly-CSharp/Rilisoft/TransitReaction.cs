using System;

namespace Rilisoft
{
	internal sealed class TransitReaction<TState, TInput> : ReactionBase<TInput> where TState : StateBase<TInput>
	{
		private readonly StateBase<TInput> _newState;

		public TransitReaction(StateBase<TInput> newState)
		{
			if (newState == null)
			{
				throw new ArgumentNullException("newState");
			}
			_newState = newState;
		}

		internal override StateBase<TInput> GetNewState()
		{
			return _newState;
		}
	}
}
