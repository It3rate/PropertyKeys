using System;
using System.Drawing;

namespace Motive.Commands
{
	public abstract class CommandBase : ICommand
	{
		public double StartTime { get; protected set; }
		public double EndTime { get; protected set; }

        public CommandBase()
		{
		}

		public virtual void Execute()
		{
			StartTime = Runner.GetRunnerById(0).CurrentMs;
		}

        public virtual void UnExecute()
		{
			EndTime = Runner.GetRunnerById(0).CurrentMs;
        }

        public virtual void Update(double time)
        {
        }

        public virtual void Draw(Graphics graphics)
		{
		}
	}
}