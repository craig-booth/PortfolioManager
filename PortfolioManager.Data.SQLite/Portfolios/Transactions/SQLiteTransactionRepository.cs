using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    class SQLiteTransactionRepository : SQLiteRepository<Transaction>, ITransactionRepository
    {
        private Dictionary<TransactionType, SQLiteRepository<Transaction>> _DetailRepositories;

        protected internal SQLiteTransactionRepository(SqliteTransaction transaction)
            : base(transaction, "Transactions", new SQLitePortfolioEntityCreator())
        {
            _DetailRepositories = new Dictionary<TransactionType, SQLiteRepository<Transaction>>();
            _DetailRepositories.Add(TransactionType.Aquisition, new SQLiteAquisitionRepository(transaction));
            _DetailRepositories.Add(TransactionType.CostBaseAdjustment, new SQLiteCostBaseAdjustmentRepository(transaction));
            _DetailRepositories.Add(TransactionType.Disposal, new SQLiteDisposalRepository(transaction));
            _DetailRepositories.Add(TransactionType.Income, new SQLiteIncomeReceivedRepository(transaction));
            _DetailRepositories.Add(TransactionType.OpeningBalance, new SQLiteOpeningBalanceRepository(transaction));
            _DetailRepositories.Add(TransactionType.ReturnOfCapital, new SQLiteReturnOfCapitalRepository(transaction));
            _DetailRepositories.Add(TransactionType.UnitCountAdjustment, new SQLiteUnitCountAdjustmentRepository(transaction));
            _DetailRepositories.Add(TransactionType.CashTransaction, new SQLiteCashTransactionsRepository(transaction));
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [Transactions] ([Id], [TransactionDate], [Type], [ASXCode], [Description], [Attachment], [RecordDate], [Comment]) VALUES (@Id, @TransactionDate, @Type, @ASXCode, @Description, @Attachment, @RecordDate, @Comment)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@TransactionDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Type", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@ASXCode", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Description", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Attachment", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@RecordDate", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@Comment", SqliteType.Text);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [Transactions] SET [TransactionDate] = @TransactionDate, [Description] = @Description, [Attachment] = @Attachment, [RecordDate] = @RecordDate, [Comment] = @Comment WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@TransactionDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Type", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@ASXCode", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Description", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Attachment", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@RecordDate", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@Comment", SqliteType.Text);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        private SQLiteRepository<Transaction> GetDetailRepository(TransactionType type)
        {
            SQLiteRepository<Transaction> detailRepository;
            if (_DetailRepositories.TryGetValue(type, out detailRepository))
                return detailRepository;
            else
                throw new NotSupportedException();
            
        }

        public override void Add(Transaction entity)
        {
            /* Add header record */
            base.Add(entity);

            var detailRepository = GetDetailRepository(entity.Type);
            detailRepository.Add(entity);
        }

        public override void Update(Transaction entity)
        {
            /* Update header record */
            base.Update(entity);

            var detailRepository = GetDetailRepository(entity.Type);
            detailRepository.Update(entity);
        }

        public override void Delete(Guid id)
        {
            var entity = Get(id);

            /* Delete header record */
            base.Delete(id);

            var detailRepository = GetDetailRepository(entity.Type);
            detailRepository.Delete(entity.Id);
        }
        
        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            parameters["@Id"].Value = entity.Id.ToString();
            parameters["@TransactionDate"].Value = entity.TransactionDate.ToString("yyyy-MM-dd");
            parameters["@Type"].Value = entity.Type;
            parameters["@ASXCode"].Value = entity.ASXCode;
            parameters["@Description"].Value = entity.Description;
            parameters["@Attachment"].Value = entity.Attachment.ToString();
            parameters["@RecordDate"].Value = entity.RecordDate.ToString("yyyy-MM-dd");
            parameters["@Comment"].Value = entity.Comment;
        }
    }

}
