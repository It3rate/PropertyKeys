using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
