using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteParcelRepository : SQLiteEffectiveDatedRepository<ShareParcel>, IParcelRepository
    {

        protected internal SQLiteParcelRepository(SQLitePortfolioDatabase database, IEntityCreator entityCreator)
            : base(database, "Parcels", entityCreator)
        {
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO Parcels ([Id], [FromDate], [ToDate], [Stock], [AquisitionDate], [Units], [UnitPrice], [Amount], [CostBase], [PurchaseId]) VALUES (@Id, @FromDate, @ToDate, @Stock, @AquisitionDate, @Units, @UnitPrice, @Amount, @CostBase, @PurchaseId)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE Parcels SET [ToDate] = @ToDate, [Stock] = @Stock, [AquisitionDate] = @AquisitionDate, [Units] = @Units, [UnitPrice] = @UnitPrice, [Amount] = @Amount, [CostBase] = @CostBase, [PurchaseId] = @PurchaseId WHERE [Id] = @Id AND [FromDate] = @FromDate", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SQLiteCommand command, ShareParcel entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@FromDate", entity.FromDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ToDate", entity.ToDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Stock", entity.Stock.ToString());
            command.Parameters.AddWithValue("@AquisitionDate", entity.AquisitionDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Units", entity.Units);
            command.Parameters.AddWithValue("@UnitPrice", SQLiteUtils.DecimalToDB(entity.UnitPrice));
            command.Parameters.AddWithValue("@Amount", SQLiteUtils.DecimalToDB(entity.Amount));
            command.Parameters.AddWithValue("@CostBase", SQLiteUtils.DecimalToDB(entity.CostBase));
            command.Parameters.AddWithValue("@PurchaseId", entity.PurchaseId.ToString());
        }

    }
}
