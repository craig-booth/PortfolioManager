using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Transactions
{
    public enum TransationListPosition { First, Last };

    public interface ITransactionList<T>
    : IEnumerable<T>

    {
        int Count { get; }
        T this[int index] { get; }
        T this[Guid id] { get; }

        int IndexOf(DateTime date, TransationListPosition position);
        DateTime Earliest { get; }
        DateTime Latest { get; }

        IEnumerable<T> FromDate(DateTime date);
        IEnumerable<T> ToDate(DateTime date);
        IEnumerable<T> InDateRange(DateRange dateRange);
    }

    public class TransactionList<T>
        : ITransactionList<T>
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

        public void Add(T transaction)
        {
            _IdLookup.Add(transaction.Id, transaction);

            if ((_Dates.Count == 0) || (transaction.TransactionDate >= Latest))
            {
                _Dates.Add(transaction.TransactionDate);
                _Transactions.Add(transaction);
            }
            else
            {
                var index = IndexOf(transaction.TransactionDate, TransationListPosition.Last);
                if (index < 0)
                    index = ~index;
                else
                    index = index + 1;

                _Dates.Insert(index, transaction.TransactionDate);
                _Transactions.Insert(index, transaction);
            }
        }

        public void Clear()
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
            if (index > 0)
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

        public IEnumerable<T> FromDate(DateTime date)
        {
            var start = IndexOf(date, TransationListPosition.First);
            if (start < 0)
                start = ~start;

            return _Transactions.Skip(start);
        }

        public IEnumerable<T> ToDate(DateTime date)
        {
            var end = IndexOf(date, TransationListPosition.Last);
            if (end < 0)
                end = ~end;
            else
                end = end + 1;

            return _Transactions.Take(end);
        }

        public IEnumerable<T> InDateRange(DateRange dateRange)
        {
            var start = IndexOf(dateRange.FromDate, TransationListPosition.First);
            if (start < 0)
                start = ~start;

            var end = IndexOf(dateRange.ToDate, TransationListPosition.Last);
            if (end < 0)
                end = ~end;
            else
                end = end + 1;

            return _Transactions.Skip(start).Take(end - start);
        }

        public void RemoveAt(int index)
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
