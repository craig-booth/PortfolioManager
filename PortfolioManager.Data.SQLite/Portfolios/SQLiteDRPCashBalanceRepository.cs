using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteDRPCashBalanceRepository : SQLiteEffectiveDatedRepository<DRPCashBalance>, IDRPCashBalanceRepository
    {
        protected internal SQLiteDRPCashBalanceRepository(SQLitePortfolioDatabase database)
            : base(database, "DRPCashBalances", new SQLitePortfolioEntityCreator(database))
        {
        }


        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO DRPCashBalances ([Id], [FromDate], [ToDate], [Balance]) VALUES (@Id, @FromDate, @ToDate, @Balance)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE DRPCashBalances SET [ToDate] = @ToDate, [Balance] = @Balance WHERE [Id] = @Id AND [FromDate] = @FromDate", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SQLiteCommand command, DRPCashBalance entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@FromDate", entity.FromDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ToDate", entity.ToDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Balance", SQLiteUtils.DecimalToDB(entity.Balance));
        }
    }
}
