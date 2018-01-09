using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public interface IStateRepository
    {
        State GetState(string state);
        IEnumerable<State> States { get; }
    }
}
