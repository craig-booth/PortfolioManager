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
    class SQLiteCashAccountRepository : SQLiteRepository<CashAccountTransaction>, ICashAccountRepository
    {
  
        protected internal SQLiteCashAccountRepository(SQLitePortfolioDatabase database)
            : base(database, "CashAccountTransactions", new SQLitePortfolioEntityCreator(database))
        {
        }


        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO CashAccountTransactions ([Id], [Type], [Date], [Description], [Amount]) VALUES (@Id, @Type, @Date, @Description, @Amount)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE CashAccountTransactions SET [Type] = @Type, [Date] = @Date, [Description] = @Description, [Amount] = @Amount WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SQLiteCommand command, CashAccountTransaction entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Type", entity.Type);
            command.Parameters.AddWithValue("@Date", entity.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Description", entity.Description);
            command.Parameters.AddWithValue("@Amount", SQLiteUtils.DecimalToDB(entity.Amount));
        }

    }
}
