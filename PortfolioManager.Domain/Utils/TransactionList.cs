﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Utils
{
    public enum TransationListPosition { First, Last };

    public interface ITransaction
    {
        Guid Id { get; }
        DateTime Date { get; }
    }

    public interface ITransactionList<T> : ITransactionRange<T> where T : ITransaction
    {
        T this[int index] { get; }
        T this[Guid id] { get; }

        ITransactionRange<T> FromDate(DateTime date);
        ITransactionRange<T> ToDate(DateTime date);
        ITransactionRange<T> InDateRange(DateRange dateRange);
    }

    public interface ITransactionRange<T> : IEnumerable<T> where T : ITransaction
    {
        int Count { get; }
        DateTime Earliest { get; }
        DateTime Latest { get; }
    }

    public class TransactionRange<T> : ITransactionRange<T> where T : ITransaction
    {
        private ITransactionList<T> _TransactionList;
        private int _FromIndex;
        private int _ToIndex;

        public int Count => _ToIndex - _FromIndex + 1;

        public TransactionRange(ITransactionList<T> transactionList, int fromIndex, int toIndex)
        {
            _TransactionList = transactionList;
            _FromIndex = fromIndex;
            _ToIndex = toIndex;
        }

        public DateTime Earliest
        {
            get
            {
                if (Count >= 0)
                    return _TransactionList[_FromIndex].Date;
                else
                    return DateUtils.NoDate;
            }
        }

        public DateTime Latest
        {
            get
            {
                if (Count >= 0)
                    return _TransactionList[_ToIndex].Date;
                else
                    return DateUtils.NoDate;
            }
        }

        private class TransactionRangeEnumerator<Y> : IEnumerator<Y> where Y : ITransaction
        {
            private TransactionRange<Y> _TransactionRange;
            private int _CurrentIndex;

            public TransactionRangeEnumerator(TransactionRange<Y> transactionRange)
            {
                _TransactionRange = transactionRange;
                _CurrentIndex = transactionRange._FromIndex - 1;
            }

            public Y Current => _TransactionRange._TransactionList[_CurrentIndex];

            object IEnumerator.Current => _TransactionRange._TransactionList[_CurrentIndex];

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_CurrentIndex >= _TransactionRange._ToIndex)
                    return false;

                _CurrentIndex++;
                return true;
            }

            public void Reset()
            {
                _CurrentIndex = _TransactionRange._FromIndex - 1;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new TransactionRangeEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TransactionRangeEnumerator<T>(this);
        }
    }
    public abstract class TransactionList<T>: ITransactionList<T>
        where T : ITransaction
    {
        private Dictionary<Guid, T> _IdLookup = new Dictionary<Guid, T>();
        private List<DateTime> _Dates = new List<DateTime>();
        private List<T> _Transactions = new List<T>();

        public T this[int index]
        {
            get
            {
                return _Transactions[index];
            }

            set
            {
                _Transactions[index] = value;
            }
        }

        public T this[Guid id]
        {
            get
            {
                return _IdLookup[id];
            }

            set
            {
                if (_IdLookup.ContainsKey(id))
                    _IdLookup[id] = value;
            }
        }

        public int Count
        {
            get { return _Transactions.Count; }
        }

        public DateTime Earliest
        {
            get
            {
                if (_Dates.Count > 0)
                    return _Dates[0];
                else
                    return DateUtils.NoDate;
            }
        }

        public DateTime Latest
        {
            get
            {
                if (_Dates.Count > 0)
                    return _Dates[_Dates.Count - 1];
                else
                    return DateUtils.NoDate;
            }
        }

        protected void Add(T transaction)
        {
            _IdLookup.Add(transaction.Id, transaction);

            if ((_Dates.Count == 0) || (transaction.Date >= Latest))
            {
                _Dates.Add(transaction.Date);
                _Transactions.Add(transaction);
            }
            else
            {
                var index = IndexOf(transaction.Date, TransationListPosition.Last);
                if (index < 0)
                    index = ~index;
                else
                    index = index + 1;

                _Dates.Insert(index, transaction.Date);
                _Transactions.Insert(index, transaction);
            }
        }

        protected void Clear()
        {
            _IdLookup.Clear();
            _Dates.Clear();
            _Transactions.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Transactions.GetEnumerator();
        }

        public int IndexOf(DateTime date, TransationListPosition position)
        {
            var index = _Dates.BinarySearch(date);
            if (index >= 0)
            {
                if (position == TransationListPosition.First)
                {
                    while ((index > 0) && (_Dates[index - 1] == date))
                        index--;
                }
                else
                {
                    while ((index < _Dates.Count - 1) && (_Dates[index + 1] == date))
                        index++;
                }
            }

            return index;
        }

        public ITransactionRange<T> FromDate(DateTime date)
        {
            var start = IndexOf(date, TransationListPosition.First);
            if (start < 0)
                start = ~start;

            return new TransactionRange<T>(this, start, Count - 1);
        }

        public ITransactionRange<T> ToDate(DateTime date)
        {
            var end = IndexOf(date, TransationListPosition.Last);
            if (end < 0)
                end = ~end;
            else
                end = end + 1;

            return new TransactionRange<T>(this, 0, end);
        }

        public ITransactionRange<T> InDateRange(DateRange dateRange)
        {
            var start = IndexOf(dateRange.FromDate, TransationListPosition.First);
            if (start < 0)
                start = ~start;

            var end = IndexOf(dateRange.ToDate, TransationListPosition.Last);
            if (end < 0)
                end = ~end - 1;

            return new TransactionRange<T>(this, start, end);
        }

        protected void RemoveAt(int index)
        {
            var id = _Transactions[index].Id;
            _IdLookup.Remove(id);
            _Dates.RemoveAt(index);
            _Transactions.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Transactions.GetEnumerator();
        }
    }

}
