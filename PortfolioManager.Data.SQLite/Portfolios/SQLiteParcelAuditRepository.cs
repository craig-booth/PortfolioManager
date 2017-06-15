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
    class SQLiteParcelAuditRepository : SQLiteRepository<ShareParcelAudit>, IParcelAuditRepository
    {

        protected internal SQLiteParcelAuditRepository(SQLitePortfolioDatabase database)
            :base(database, "ParcelAudit", new SQLitePortfolioEntityCreator(database))
        {
            _Database = database;
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO ParcelAudit ([Id], [Parcel], [Date], [Transaction], [UnitCount], [CostBaseChange], [AmountChange]) VALUES (@Id, @Parcel, @Date, @Transaction, @UnitCount, @CostBaseChange, @AmountChange)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE ParcelAudit SET [Parcel] = @Parcel, [Date] = @Date, [Transaction] = @Transaction, [UnitCount] = @UnitCount, [CostBaseChange] = @CostBaseChange, [AmountChange] = @AmountChange WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SQLiteCommand command, ShareParcelAudit entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Parcel", entity.Parcel.ToString());
            command.Parameters.AddWithValue("@Date", entity.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Transaction", entity.Transaction.ToString());
            command.Parameters.AddWithValue("@UnitCount", SQLiteUtils.DecimalToDB(entity.UnitCount));
            command.Parameters.AddWithValue("@CostBaseChange", SQLiteUtils.DecimalToDB(entity.CostBaseChange));
            command.Parameters.AddWithValue("@AmountChange", SQLiteUtils.DecimalToDB(entity.AmountChange));
         }

    }
}
