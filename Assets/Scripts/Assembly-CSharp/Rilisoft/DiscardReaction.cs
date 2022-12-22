namespace Rilisoft
{
	internal sealed class DiscardReaction<TInput> : ReactionBase<TInput>
	{
		public static readonly DiscardReaction<TInput> Default = new DiscardReaction<TInput>();

		internal override StateBase<TInput> GetNewState()
		{
			return null;
		}
	}
}
