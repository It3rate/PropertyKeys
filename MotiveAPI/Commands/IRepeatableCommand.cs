namespace MotiveCore.Commands
{
	public interface IRepeatableCommand : ICommand
	{
		IRepeatableCommand GetRepeatCommand();
	}
}