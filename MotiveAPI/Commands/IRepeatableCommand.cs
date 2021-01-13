namespace Motive.Commands
{
	public interface IRepeatableCommand : ICommand
	{
		IRepeatableCommand GetRepeatCommand();
	}
}