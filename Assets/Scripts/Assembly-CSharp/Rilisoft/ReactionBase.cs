namespace Rilisoft
{
	internal abstract class ReactionBase<TInput>
	{
		internal abstract StateBase<TInput> GetNewState();
	}
}
