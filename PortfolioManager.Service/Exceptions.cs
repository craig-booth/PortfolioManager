using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service
{

    [Serializable]
    public abstract class TransactionException : Exception
    {
        public ITransaction Transacation { get; private set; }

        public TransactionException(ITransaction transcation, string message)
            : base(message)
        {
            Transacation = transcation;
        }
    }

    [Serializable]
    public class TransctionNotSupportedForStapledSecurity: TransactionException 
    {   
        public TransctionNotSupportedForStapledSecurity(ITransaction transcation, string message)
            : base(transcation, message)
        {
        }
    }

    [Serializable]
    public class TransctionNotSupportedForChildSecurity : TransactionException
    {
        public TransctionNotSupportedForChildSecurity(ITransaction transcation, string message)
            : base(transcation, message)
        {
        }
    }

    [Serializable]
    public class NoParcelsForTransaction : TransactionException 
    {
        public NoParcelsForTransaction(ITransaction transcation, string message)
            : base(transcation, message)
        {

        }
    }

    [Serializable]
    public class NotEnoughSharesForDisposal : TransactionException
    {
        public NotEnoughSharesForDisposal(ITransaction transcation, string message)
            : base(transcation, message)
        {
        }
    }

    [Serializable]
    public class AttemptToModifyPreviousParcelVersion : Exception
    {
        public Guid Parcel { get; private set; }

        public AttemptToModifyPreviousParcelVersion(Guid parcel, string message)
            : base (message)
        {
            Parcel = parcel;
        }
    }
}
