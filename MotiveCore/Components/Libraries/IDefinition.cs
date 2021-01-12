using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotiveCore.Components.Libraries
{
    public interface IDefinition
    {
	    string Name { get; set; }
	    int Id { get; }

	    bool AssignIdIfUnset(int id);

	    void OnActivate();
	    void OnDeactivate();

	    void Update(double currentTime, double deltaTime);
    }
}
