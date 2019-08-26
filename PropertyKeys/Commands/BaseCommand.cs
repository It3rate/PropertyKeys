using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void UnExecute()
        {
            throw new NotImplementedException();
        }
    }
}
