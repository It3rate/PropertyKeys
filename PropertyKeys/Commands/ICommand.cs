using System.Drawing;

namespace DataArcs.Commands
{
	public interface ICommand
	{
		void Execute();
		void UnExecute();

		void Update(float time);
		void Draw(Graphics graphics);
	}
}