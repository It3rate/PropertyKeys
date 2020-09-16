using System;
using System.Drawing;
using DataArcs.Players;

namespace DataArcs.Commands
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
			StartTime = Player.GetPlayerById(0).CurrentMs;
		}

        public virtual void UnExecute()
		{
			EndTime = Player.GetPlayerById(0).CurrentMs;
        }

        public virtual void Update(double time)
        {
        }

        public virtual void Draw(Graphics graphics)
		{
		}
	}
}