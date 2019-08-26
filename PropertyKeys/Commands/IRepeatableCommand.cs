using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataArcs.Commands
{
    public interface IRepeatableCommand : ICommand
    {
        IRepeatableCommand GetRepeatCommand();
    }
}
