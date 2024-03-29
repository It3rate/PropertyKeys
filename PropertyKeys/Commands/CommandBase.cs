﻿using System;
using System.Drawing;

namespace DataArcs.Commands
{
	public abstract class CommandBase : ICommand
	{
		public int StartTime { get; }
		public int EndTime { get; }

		public void Execute()
		{
			throw new NotImplementedException();
		}

		public void UnExecute()
		{
			throw new NotImplementedException();
		}

		public void Update(float time)
		{
			throw new NotImplementedException();
		}

		public void Draw(Graphics graphics)
		{
			throw new NotImplementedException();
		}
	}
}