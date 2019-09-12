namespace DataArcs.Commands
{
	public interface IRepeatableCommand : ICommand
	{
		IRepeatableCommand GetRepeatCommand();
	}
}