using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Portfolios
{ 
    [Serializable]
    class TransctionNotSupportedForStapledSecurity: Exception 
    {
        public ITransaction Transacation { get; private set; }

        public TransctionNotSupportedForStapledSecurity(ITransaction transcation, string message)
            : base(message)
        {
            Transacation = transcation;
        }
    }

    [Serializable]
class NoParcelsForTransaction : Exception
    {
        public ITransaction Transacation { get; private set; }

        public NoParcelsForTransaction(ITransaction transcation, string message)
            : base(message)
        {
            Transacation = transcation;
        }
    }
}
