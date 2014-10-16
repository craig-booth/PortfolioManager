using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteRelateNTARepository : SQLiteRepository<RelativeNTA>, IRelativeNTARepository
    {
        protected internal SQLiteRelateNTARepository(SQLiteStockDatabase database)
            : base(database, "RelativeNTAs")
        {
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO RelativeNTAs ([Id], [Date], [Parent], [Child], [Percentage]) VALUES (@Id, @Date, @Parent, @Child, @Percentage)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE RelativeNTAs SET [Date] = @Date, [Parent] = @Parent, [Child] = @Child, [Percentage] = @Percentage WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override RelativeNTA CreateEntity(SQLiteDataReader reader)
        {
            RelativeNTA nta = new RelativeNTA()
            {
                Id = new Guid(reader.GetString(0)),
                Date = reader.GetDateTime(1),
                Parent = new Guid(reader.GetString(2)),
                Child = new Guid(reader.GetString(3)),
                Percentage = DBToDecimal(reader.GetInt32(4))
            };

            return nta;
        }

        protected override void AddParameters(SQLiteCommand command, RelativeNTA entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Date", entity.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Parent", entity.Parent.ToString());
            command.Parameters.AddWithValue("@Child", entity.Child.ToString());
            command.Parameters.AddWithValue("@Percentage", DecimalToDB(entity.Percentage));
        }
    }


}
