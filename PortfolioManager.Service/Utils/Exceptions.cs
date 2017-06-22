﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Utils
{

    public abstract class TransactionException : Exception
    {
        public Transaction Transacation { get; private set; }

        public TransactionException(Transaction transcation, string message)
            : base(message)
        {
            Transacation = transcation;
        }
    }

    public class TransctionNotSupportedForStapledSecurity: TransactionException 
    {   
        public TransctionNotSupportedForStapledSecurity(Transaction transcation, string message)
            : base(transcation, message)
        {
        }
    }

    public class TransctionNotSupportedForChildSecurity : TransactionException
    {
        public TransctionNotSupportedForChildSecurity(Transaction transcation, string message)
            : base(transcation, message)
        {
        }
    }

    public class NoParcelsForTransaction : TransactionException 
    {
        public NoParcelsForTransaction(Transaction transcation, string message)
            : base(transcation, message)
        {

        }
    }

    public class NotEnoughSharesForDisposal : TransactionException
    {
        public NotEnoughSharesForDisposal(Transaction transcation, string message)
            : base(transcation, message)
        {
        }
    }

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
