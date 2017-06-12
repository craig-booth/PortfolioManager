using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteTransactionRepository : SQLiteRepository<Transaction>, ITransactionRepository
    {
        private Dictionary<TransactionType, TransactionDetailRepository> _DetailRepositories;

        protected internal SQLiteTransactionRepository(SQLitePortfolioDatabase database)
            : base(database, "Transactions", new SQLitePortfolioEntityCreator(database))
        {
            _Database = database;

            _DetailRepositories = new Dictionary<TransactionType, TransactionDetailRepository>();
            _DetailRepositories.Add(TransactionType.Aquisition, new AquisitionRepository(_Connection));
            _DetailRepositories.Add(TransactionType.CostBaseAdjustment, new CostBaseAdjustmentRepository(_Connection));
            _DetailRepositories.Add(TransactionType.Disposal, new DisposalRepository(_Connection));
            _DetailRepositories.Add(TransactionType.Income, new IncomeReceivedRepository(_Connection));
            _DetailRepositories.Add(TransactionType.OpeningBalance, new OpeningBalanceRepository(_Connection));
            _DetailRepositories.Add(TransactionType.ReturnOfCapital, new ReturnOfCapitalRepository(_Connection));
            _DetailRepositories.Add(TransactionType.UnitCountAdjustment, new UnitCountAdjustmentRepository(_Connection));
            _DetailRepositories.Add(TransactionType.CashTransaction, new CashTransactionsRepository(_Connection));
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO [Transactions] ([Id], [TransactionDate], [Type], [ASXCode], [Description], [Attachment], [RecordDate], [Comment]) VALUES (@Id, @TransactionDate, @Type, @ASXCode, @Description, @Attachment, @RecordDate, @Comment)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE [Transactions] SET [TransactionDate] = @TransactionDate, [Description] = @Description, [Attachment] = @Attachment, [RecordDate] = @RecordDate, [Comment] = @Comment WHERE [Id] = @Id", _Connection);
                _GetUpdateRecordCommand.Prepare();
            }

            return _GetUpdateRecordCommand;
        }

        private TransactionDetailRepository GetDetailRepository(TransactionType type)
        {
            TransactionDetailRepository detailRepository;
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
            detailRepository.AddRecord(entity);
        }

        public override void Update(Transaction entity)
        {
            /* Update header record */
            base.Update(entity);

            var detailRepository = GetDetailRepository(entity.Type);
            detailRepository.UpdateRecord(entity);
        }

        public override void Delete(Guid id)
        {
            var entity = Get(id);

            /* Delete header record */
            base.Delete(id);

            var detailRepository = GetDetailRepository(entity.Type);
            detailRepository.DeleteRecord(entity.Id);
        }
        
        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@TransactionDate", entity.TransactionDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Type", entity.Type);
            command.Parameters.AddWithValue("@ASXCode", entity.ASXCode);
            command.Parameters.AddWithValue("@Description", entity.Description);
            command.Parameters.AddWithValue("@Attachment", entity.Attachment.ToString());
            command.Parameters.AddWithValue("@RecordDate", entity.RecordDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Comment", entity.Comment);
        }
    }

    public class TransactionDetailRepository
    {
        private SQLiteConnection _Connection;

        public TransactionDetailRepository(SQLiteConnection connection)
        {
            _Connection = connection;
        }

        private SQLiteCommand _AddRecordCommand;
        public void AddRecord(Transaction entity)
        {
            if (_AddRecordCommand == null)
            {
                _AddRecordCommand = new SQLiteCommand(GetAddSQL(), _Connection);
                _AddRecordCommand.Prepare();
            }

            AddParameters(_AddRecordCommand, entity);
            _AddRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _UpdateRecordCommand;
        public void UpdateRecord(Transaction entity)
        {
            if (_UpdateRecordCommand == null)
            {
                _UpdateRecordCommand = new SQLiteCommand(GetUpdateSQL(), _Connection);
                _UpdateRecordCommand.Prepare();
            }

            AddParameters(_UpdateRecordCommand, entity);
            _UpdateRecordCommand.ExecuteNonQuery();
        }

        private SQLiteCommand _DeleteRecordCommand;
        public void DeleteRecord(Guid id)
        {
            if (_DeleteRecordCommand == null)
            {
                _DeleteRecordCommand = new SQLiteCommand(GetDeleteSQL(), _Connection);
                _DeleteRecordCommand.Prepare();
            }

            _DeleteRecordCommand.Parameters.AddWithValue("@Id", id.ToString());
            _DeleteRecordCommand.ExecuteNonQuery();
        }

        protected virtual string GetAddSQL()
        {
            throw new NotSupportedException("");
        }

        protected virtual string GetUpdateSQL()
        {
            throw new NotSupportedException("");
        }

        protected virtual string GetDeleteSQL()
        {
            throw new NotSupportedException("");
        }

        protected virtual void AddParameters(SQLiteCommand command, Transaction entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());   
        }
    }


    public class AquisitionRepository : TransactionDetailRepository
    {

        public AquisitionRepository(SQLiteConnection connection)
            : base(connection)
        {
           
        }

        protected override string GetAddSQL()
        {
            return "INSERT INTO [Aquisitions] ([Id], [Units], [AveragePrice], [TransactionCosts], [CreateCashTransaction]) VALUES (@Id, @Units, @AveragePrice, @TransactionCosts, @CreateCashTransaction)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE [Aquisitions] SET [Units] = @Units, [AveragePrice] = @AveragePrice, [TransactionCosts] = @TransactionCosts, [CreateCashTransaction] = @CreateCashTransaction WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [Aquisitions] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            Aquisition aquisition = entity as Aquisition;

            command.Parameters.AddWithValue("@Id", aquisition.Id.ToString());
            command.Parameters.AddWithValue("@Units", aquisition.Units);
            command.Parameters.AddWithValue("@AveragePrice", SQLiteUtils.DecimalToDB(aquisition.AveragePrice));
            command.Parameters.AddWithValue("@TransactionCosts", SQLiteUtils.DecimalToDB(aquisition.TransactionCosts));
            command.Parameters.AddWithValue("@CreateCashTransaction", SQLiteUtils.BoolToDb(aquisition.CreateCashTransaction));
        }
    }

    public class CostBaseAdjustmentRepository: TransactionDetailRepository
    {

        public CostBaseAdjustmentRepository(SQLiteConnection connection) 
            : base(connection)
        {
        }

        protected override string GetAddSQL()
        {
            return "INSERT INTO [CostBaseAdjustments] ([Id], [Percentage]) VALUES (@Id, @Percentage)";

        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[CostBaseAdjustments] SET [Percentage] = @Percentage WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [CostBaseAdjustments] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            CostBaseAdjustment costBaseAdjustment = entity as CostBaseAdjustment;

            command.Parameters.AddWithValue("@Id", costBaseAdjustment.Id.ToString());
            command.Parameters.AddWithValue("@Percentage", SQLiteUtils.DecimalToDB(costBaseAdjustment.Percentage));
        }
    }

    public class DisposalRepository : TransactionDetailRepository
    {

        public DisposalRepository(SQLiteConnection connection) 
            : base(connection)
        {
        }

        protected override string GetAddSQL()
        {
            return "INSERT INTO [Disposals] ([Id], [Units], [AveragePrice], [TransactionCosts], [CGTMethod], [CreateCashTransaction]) VALUES (@Id, @Units, @AveragePrice, @TransactionCosts, @CGTMethod, @CreateCashTransaction)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[Disposals] SET [Units] = @Units, [AveragePrice] = @AveragePrice, [TransactionCosts] = @TransactionCosts, [CGTMethod] = @CGTMethod , [CreateCashTransaction] = @CreateCashTransaction WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [Disposals] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            Disposal disposal = entity as Disposal;

            command.Parameters.AddWithValue("@Id", disposal.Id.ToString());
            command.Parameters.AddWithValue("@Units", disposal.Units);
            command.Parameters.AddWithValue("@AveragePrice", SQLiteUtils.DecimalToDB(disposal.AveragePrice));
            command.Parameters.AddWithValue("@TransactionCosts", SQLiteUtils.DecimalToDB(disposal.TransactionCosts));
            command.Parameters.AddWithValue("@CGTMethod", disposal.CGTMethod);
            command.Parameters.AddWithValue("@CreateCashTransaction", SQLiteUtils.BoolToDb(disposal.CreateCashTransaction));
        }
    }

    public class IncomeReceivedRepository : TransactionDetailRepository
    {

        public IncomeReceivedRepository(SQLiteConnection connection) 
            : base(connection)
        {
        }

        protected override string GetAddSQL()
        {
            return "INSERT INTO [IncomeReceived] ([Id], [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred], [CreateCashTransaction], [DRPCashBalance]) VALUES (@Id, @FrankedAmount, @UnfrankedAmount, @FrankingCredits, @Interest, @TaxDeferred, @CreateCashTransaction, @DRPCashBalance)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[IncomeReceived] SET [FrankedAmount] = @FrankedAmount, [UnfrankedAmount] = @UnfrankedAmount, [FrankingCredits] = @FrankingCredits, [Interest] = @Interest, [TaxDeferred] = @TaxDeferred, [CreateCashTransaction] = @CreateCashTransaction, [DRPCashBalance] = @DRPCashBalance WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [IncomeReceived] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            IncomeReceived incomeReceived = entity as IncomeReceived;

            command.Parameters.AddWithValue("@Id", incomeReceived.Id.ToString());
            command.Parameters.AddWithValue("@FrankedAmount", SQLiteUtils.DecimalToDB(incomeReceived.FrankedAmount));
            command.Parameters.AddWithValue("@UnfrankedAmount", SQLiteUtils.DecimalToDB(incomeReceived.UnfrankedAmount));
            command.Parameters.AddWithValue("@FrankingCredits", SQLiteUtils.DecimalToDB(incomeReceived.FrankingCredits));
            command.Parameters.AddWithValue("@Interest", SQLiteUtils.DecimalToDB(incomeReceived.Interest));
            command.Parameters.AddWithValue("@TaxDeferred", SQLiteUtils.DecimalToDB(incomeReceived.TaxDeferred));
            command.Parameters.AddWithValue("@CreateCashTransaction", SQLiteUtils.BoolToDb(incomeReceived.CreateCashTransaction));
            command.Parameters.AddWithValue("DRPCashBalance", SQLiteUtils.DecimalToDB(incomeReceived.DRPCashBalance));
        }
    }

    public class OpeningBalanceRepository : TransactionDetailRepository
    {

        public OpeningBalanceRepository(SQLiteConnection connection) 
            : base(connection)
        {
        }

        protected override string GetAddSQL()
        {
            return "INSERT INTO [OpeningBalances] ([Id], [Units], [CostBase], [AquisitionDate], [PurchaseId]) VALUES (@Id, @Units, @CostBase, @AquisitionDate, @PurchaseId)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[OpeningBalances] SET [Units] = @Units, [CostBase] = @CostBase, [AquisitionDate] = @AquisitionDate, [PurchaseId] = @PurchaseId WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [OpeningBalances] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            OpeningBalance openingBalance = entity as OpeningBalance;

            command.Parameters.AddWithValue("@Id", openingBalance.Id.ToString());
            command.Parameters.AddWithValue("@Units", openingBalance.Units);
            command.Parameters.AddWithValue("@CostBase", SQLiteUtils.DecimalToDB(openingBalance.CostBase));
            command.Parameters.AddWithValue("@AquisitionDate", openingBalance.AquisitionDate.ToString("yyyy-MM-dd")); 
            command.Parameters.AddWithValue("@PurchaseId", openingBalance.PurchaseId.ToString());
        }
    }

    public class ReturnOfCapitalRepository : TransactionDetailRepository
    {

        public ReturnOfCapitalRepository(SQLiteConnection connection) 
            : base(connection)
        {
        }

        protected override string GetAddSQL()
        {
            return "INSERT INTO [ReturnsOfCapital] ([Id], [Amount], [CreateCashTransaction]) VALUES (@Id, @Amount, @CreateCashTransaction)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[ReturnsOfCapital] SET [Amount] = @Amount, [CreateCashTransaction] = @CreateCashTransaction WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [ReturnsOfCapital] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            ReturnOfCapital returnOfCapital = entity as ReturnOfCapital;

            command.Parameters.AddWithValue("@Id", returnOfCapital.Id.ToString());
            command.Parameters.AddWithValue("@Amount", SQLiteUtils.DecimalToDB(returnOfCapital.Amount));
            command.Parameters.AddWithValue("@CreateCashTransaction", SQLiteUtils.BoolToDb(returnOfCapital.CreateCashTransaction));
        }
    }

    public class UnitCountAdjustmentRepository : TransactionDetailRepository
    {

        public UnitCountAdjustmentRepository(SQLiteConnection connection)
            : base(connection)
        {
        }

        protected override string GetAddSQL()
        {
            return "INSERT INTO [UnitCountAdjustments] ([Id], [OriginalUnits], [NewUnits]) VALUES (@Id, @OriginalUnits, @NewUnits)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE [UnitCountAdjustments] SET [OriginalUnits] = @OriginalUnits, [NewUnits] = @NewUnits WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [UnitCountAdjustments] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            UnitCountAdjustment unitCountAdjustment = entity as UnitCountAdjustment;

            command.Parameters.AddWithValue("@Id", unitCountAdjustment.Id.ToString());
            command.Parameters.AddWithValue("@OriginalUnits", unitCountAdjustment.OriginalUnits);
            command.Parameters.AddWithValue("@NewUnits", unitCountAdjustment.NewUnits);
        }
    }

    public class CashTransactionsRepository : TransactionDetailRepository
    {

        public CashTransactionsRepository(SQLiteConnection connection)
            : base(connection)
        {
        }

        protected override string GetAddSQL()
        {
            return "INSERT INTO [CashTransactions] ([Id], [Type], [Amount]) VALUES (@Id, @Type, @Amount)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE [CashTransactions] SET [Type] = @Type, [Amount] = @Amount WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [CashTransactions] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, Transaction entity)
        {
            CashTransaction cashTransaction = entity as CashTransaction;

            command.Parameters.AddWithValue("@Id", cashTransaction.Id.ToString());
            command.Parameters.AddWithValue("@Type", cashTransaction.CashTransactionType);
            command.Parameters.AddWithValue("@Amount", SQLiteUtils.DecimalToDB(cashTransaction.Amount));
        }
    }

}
