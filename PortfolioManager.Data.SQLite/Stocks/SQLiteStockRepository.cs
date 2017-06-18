using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;
using PortfolioManager.Data;
using PortfolioManager.Data.SQLite;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockRepository : SQLiteEffectiveDatedRepository<Stock>, IStockRepository
    {
        protected internal SQLiteStockRepository(SQLiteStockDatabase database, IEntityCreator entityCreator) 
            : base(database, "Stocks", entityCreator)
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO Stocks ([Id], [FromDate], [ToDate], [ASXCode], [Name], [Type], [Parent], [DividendRounding], [DRPMethod], [Category]) VALUES (@Id, @FromDate, @ToDate, @ASXCode, @Name, @Type, @Parent, @DividendRounding, @DRPMethod, @Category)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE Stocks SET [ToDate] = @ToDate, [ASXCode] = @ASXCode, [Name] = @Name, [Type] = @Type, [Parent] = @Parent, [DividendRounding] = @DividendRounding, [DRPMethod] = @DRPMethod, [Category] = @Category WHERE [Id] = @Id AND [FromDate] = @FromDate", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteCommand command, Stock entity)
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
            command.Parameters.AddWithValue("@Category", entity.Category);
        }

    }
}
