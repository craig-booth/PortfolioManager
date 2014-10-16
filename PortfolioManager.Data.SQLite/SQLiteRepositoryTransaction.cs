using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace PortfolioManager.Data.SQLite
{
    public class SQLiteRepositoryTransaction
    {      
        private SQLiteConnection _Connection;
        private SQLiteTransaction _Transaction;
        private int _Level;
        private bool _SaveOnEnd;
        public bool SaveOnEnd
        {
            get
            {
                return _SaveOnEnd;
            }

            set
            {
                if (_Level == 1)
                    _SaveOnEnd = true; 
            }
        }

        public SQLiteRepositoryTransaction(SQLiteConnection connection)
        {
            _Connection = connection;
            _Level = 0;
            _SaveOnEnd = false;
        }

        public void BeginTransaction()
        {
            if (_Level == 0)
                _Transaction = _Connection.BeginTransaction();
            _Level++;
        }
        

        public void EndTransaction()
        {
            if (_Level > 0)
                _Level--;

            if ((_Level == 0) && (_Transaction.Connection != null))
            {
                if (_SaveOnEnd)
                    _Transaction.Commit();
                else
                    _Transaction.Rollback();
            }
        }
      
    }
}
