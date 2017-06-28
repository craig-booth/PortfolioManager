using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLiteAttachmentRepository :SQLiteRepository<Attachment>, IAttachmentRepository
    {
        
        protected internal SQLiteAttachmentRepository(SQLitePortfolioDatabase database, IEntityCreator entityCreator)
            : base(database, "Attachments", entityCreator)
        {
            _Database = database;
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [Attachments] ([Id], [Extension], [Data]) VALUES (@Id, @Extension, @Data)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [Attachments] SET [Extension] = @Extension, [Data] = @Data WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteCommand command, Attachment entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@Extension", entity.Extension);
            command.Parameters.AddWithValue("@Data", entity.Data.ToArray());
        }

    }
}
