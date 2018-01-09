using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public class StateRepository : IStateRepository
    {
        Dictionary<string, State> _statesList = null;
        public StateRepository(Dictionary<string, State> statesList)
        {
            this._statesList = statesList;
        }
        public State GetState(string stateCode)
        {
            if (string.IsNullOrWhiteSpace(stateCode) || _statesList == null || !_statesList.Any())
                return null;
            stateCode = stateCode.Trim().ToUpper();
            State returnValue = null;
            if (this._statesList.TryGetValue(stateCode, out returnValue))
                return returnValue;
            else
                return null;
        }
        public IEnumerable<State> States
        {
            get
            {
                if (this._statesList == null) yield return null;
                foreach (var item in this._statesList)
                {
                    yield return item.Value;
                }
            }
        }

        public static Dictionary<string, State> GetStates()
        {
            Dictionary<string, State> states = new Dictionary<string, State>();
            states.Add("OK", new State() { StateName = "Oklahoma", StateCode = "OK", TaxRate = 0.0864 });
            states.Add("TX", new State() { StateName = "Texas", StateCode = "TX", TaxRate = 0.0819 });
            states.Add("LA", new State() { StateName = "Louisiana", StateCode = "LA", TaxRate = 0.0998 });
            states.Add("KS", new State() { StateName = "Kansas", StateCode = "KS", TaxRate = 0.0862 });
            states.Add("NM", new State() { StateName = "New Mexico", StateCode = "NM", TaxRate = 0.0755 });
            return states;
        }
    }
}
