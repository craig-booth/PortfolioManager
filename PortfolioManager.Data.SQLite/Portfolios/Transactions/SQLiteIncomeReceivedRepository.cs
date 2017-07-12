using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios.Transactions
{
    class SQLiteIncomeReceivedRepository : SQLiteRepository<Transaction>
    {
        public SQLiteIncomeReceivedRepository(SqliteTransaction transaction)
            : base(transaction, "IncomeReceived", new SQLitePortfolioEntityCreator())
        {

        }

        private SqliteCommand _GetCurrentRecordCommand;
        protected override SqliteCommand GetCurrentRecordCommand()
        {
            if (_GetCurrentRecordCommand == null)
            {
                _GetCurrentRecordCommand = new SqliteCommand("SELECT [Transactions].*, [IncomeReceived].* FROM [IncomeReceived] LEFT OUTER JOIN [Transactions] ON [IncomeReceived].[Id] = [Transactions].[Id] WHERE [IncomeReceived].[Id] = @Id", _Transaction.Connection, _Transaction);
                _GetCurrentRecordCommand.Parameters.Add("@Id", SqliteType.Text);

                _GetCurrentRecordCommand.Prepare();
            }

            return _GetCurrentRecordCommand;
        }

        private SqliteCommand _GetAddRecordCommand;
        protected override SqliteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SqliteCommand("INSERT INTO [IncomeReceived] ([Id], [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred], [CreateCashTransaction], [DRPCashBalance]) VALUES (@Id, @FrankedAmount, @UnfrankedAmount, @FrankingCredits, @Interest, @TaxDeferred, @CreateCashTransaction, @DRPCashBalance)", _Transaction.Connection, _Transaction);

                _GetAddRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetAddRecordCommand.Parameters.Add("@FrankedAmount", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@UnfrankedAmount", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@FrankingCredits", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@Interest", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@TaxDeferred", SqliteType.Integer);
                _GetAddRecordCommand.Parameters.Add("@CreateCashTransaction", SqliteType.Text);

                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SqliteCommand _GetUpdateRecordCommand;
        protected override SqliteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SqliteCommand("UPDATE [IncomeReceived] SET [FrankedAmount] = @FrankedAmount, [UnfrankedAmount] = @UnfrankedAmount, [FrankingCredits] = @FrankingCredits, [Interest] = @Interest, [TaxDeferred] = @TaxDeferred, [CreateCashTransaction] = @CreateCashTransaction, [DRPCashBalance] = @DRPCashBalance WHERE [Id] = @Id", _Transaction.Connection, _Transaction);

                _GetUpdateRecordCommand.Parameters.Add("@Id", SqliteType.Text);
                _GetUpdateRecordCommand.Parameters.Add("@FrankedAmount", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@UnfrankedAmount", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@FrankingCredits", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@Interest", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@TaxDeferred", SqliteType.Integer);
                _GetUpdateRecordCommand.Parameters.Add("@CreateCashTransaction", SqliteType.Text);

                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }


        protected override void AddParameters(SqliteParameterCollection parameters, Transaction entity)
        {
            var income = entity as IncomeReceived;

            parameters["@Id"].Value = income.Id.ToString();
            parameters["@FrankedAmount"].Value = SQLiteUtils.DecimalToDB(income.FrankedAmount);
            parameters["@UnfrankedAmount"].Value = SQLiteUtils.DecimalToDB(income.UnfrankedAmount);
            parameters["@FrankingCredits"].Value = SQLiteUtils.DecimalToDB(income.FrankingCredits);
            parameters["@Interest"].Value = SQLiteUtils.DecimalToDB(income.Interest);
            parameters["@TaxDeferred"].Value = SQLiteUtils.DecimalToDB(income.TaxDeferred);
            parameters["@CreateCashTransaction"].Value = SQLiteUtils.BoolToDb(income.CreateCashTransaction);
        }
    }
}
