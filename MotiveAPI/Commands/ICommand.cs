using System;
using System.Drawing;

namespace MotiveCore.Commands
{
	public interface ICommand
	{
		double StartTime { get; }
		double EndTime { get; }

        void Execute();
		void UnExecute();

		void Update(double time);
		void Draw(Graphics graphics);
	}
}