using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLiteAttachmentRepository : SQLiteRepository<Attachment>, IAttachmentRepository
    {
        
        protected internal SQLiteAttachmentRepository(SqliteTransaction transaction, IEntityCreator entityCreator)
            : base(transaction, "Attachments", entityCreator)
        {
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [Attachments] ([Id], [Extension], [Data]) VALUES (@Id, @Extension, @Data)", _Transaction.Connection, _Transaction);
                
                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Extension", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Data", SqliteType.Blob);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [Attachments] SET [Extension] = @Extension, [Data] = @Data WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
                
                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Extension", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Data", SqliteType.Blob);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        protected override void AddParameters(SqliteParameterCollection parameters, Attachment entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@Extension"].Value = entity.Extension;
            parameters["@Data"].Value = entity.Data.ToArray();
        }

    }
}
