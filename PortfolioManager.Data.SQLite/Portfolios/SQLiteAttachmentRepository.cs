using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLiteAttachmentRepository :SQLiteRepository<Attachment>, IAttachmentRepository
    {
        
        protected internal SQLiteAttachmentRepository(SQLitePortfolioDatabase database, IEntityCreator entityCreator)
            : base(database, "Attachments", entityCreator)
        {
            _Database = database;
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO [Attachments] ([Id], [Extension], [Data]) VALUES (@Id, @Extension, @Data)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE [Attachments] SET [Extension] = @Extension, [Data] = @Data WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SQLiteCommand command, Attachment entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Extension", entity.Extension);
            command.Parameters.AddWithValue("@Data", entity.Data.ToArray());
        }

    }
}
