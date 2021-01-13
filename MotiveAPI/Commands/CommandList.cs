using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motive.Commands
{
	// Needed to store commands by start time, as multiple commands may have the same start time.
    public class CommandList
    {
	    private static readonly Comparer<int> DescendingComparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
        // Sorted on end times.
        public SortedList<int, ICommand> Commands = new SortedList<int, ICommand>(DescendingComparer);

		// All commands in list have the same start time.
	    public double StartTime => Commands.Count > 0 ? Commands.Values[0].StartTime : 0;
        public double MaxEndTime => Commands.Count > 0 ? Commands.Keys[Commands.Count - 1] : 0;
    }
}
