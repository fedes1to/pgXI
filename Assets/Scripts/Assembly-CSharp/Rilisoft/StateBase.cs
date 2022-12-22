namespace Rilisoft
{
	internal abstract class StateBase<TInput>
	{
		public delegate void EventHandler(object sender, TInput e);

		public event EventHandler InputRequested;

		public virtual void Enter(StateBase<TInput> oldState, TInput input)
		{
		}

		public virtual void Exit(StateBase<TInput> newState, TInput input)
		{
		}

		public abstract ReactionBase<TInput> React(TInput input);
	}
}
