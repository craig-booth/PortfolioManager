using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockRepository : SQLiteEffectiveDatedRepository<Stock>, IStockRepository
    {
        protected internal SQLiteStockRepository(SQLiteStockDatabase database) : base(database, "Stocks")
        {
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO Stocks ([Id], [FromDate], [ToDate], [ASXCode], [Name], [Type], [Parent], [DividendRounding], [DRPMethod]) VALUES (@Id, @FromDate, @ToDate, @ASXCode, @Name, @Type, @Parent, @DividendRounding, @DRPMethod)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE Stocks SET [ToDate] = @ToDate, [ASXCode] = @ASXCode, [Name] = @Name, [Type] = @Type, [Parent] = @Parent, [DividendRounding] = @DividendRounding, [DRPMethod] = @DRPMethod WHERE [Id] = @Id AND [FromDate] = @FromDate", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override Stock CreateEntity(SQLiteDataReader reader)
        {
            return SQLiteStockEntityCreator.CreateStock(_Database as SQLiteStockDatabase, reader);
        }

        protected override void AddParameters(SQLiteCommand command, Stock entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@FromDate", entity.FromDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ToDate", entity.ToDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ASXCode", entity.ASXCode);
            command.Parameters.AddWithValue("@Name", entity.Name);
            command.Parameters.AddWithValue("@Type", entity.Type);
            command.Parameters.AddWithValue("@Parent", entity.ParentId.ToString());
            command.Parameters.AddWithValue("@DividendRounding", entity.DividendRoundingRule);
            command.Parameters.AddWithValue("@DRPMethod", entity.DRPMethod);
        }

    }
}
