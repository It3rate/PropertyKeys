using System.Drawing;

namespace DataArcs.Commands
{
	public interface ICommand
	{
		int StartTime { get; }
		int EndTime { get; }

        void Execute();
		void UnExecute();

		void Update(float time);
		void Draw(Graphics graphics);
	}
}