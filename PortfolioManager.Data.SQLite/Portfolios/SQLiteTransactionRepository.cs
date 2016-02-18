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
    class SQLiteTransactionRepository : SQLiteRepository<ITransaction>, ITransactionRepository
    {
        private Dictionary<TransactionType, TransactionDetailRepository> _DetailRepositories;

        protected internal SQLiteTransactionRepository(SQLitePortfolioDatabase database)
            : base(database, "Transactions")
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
        }

        private SQLiteCommand _GetAddRecordCommand;
        protected override SQLiteCommand GetAddRecordCommand()
        {
            if (_GetAddRecordCommand == null)
            {
                _GetAddRecordCommand = new SQLiteCommand("INSERT INTO [Transactions] ([Id], [TransactionDate], [Type], [ASXCode], [Description]) VALUES (@Id, @TransactionDate, @Type, @ASXCode, @Description)", _Connection);
                _GetAddRecordCommand.Prepare();
            }

            return _GetAddRecordCommand;
        }

        private SQLiteCommand _GetUpdateRecordCommand;
        protected override SQLiteCommand GetUpdateRecordCommand()
        {
            if (_GetUpdateRecordCommand == null)
            {
                _GetUpdateRecordCommand = new SQLiteCommand("UPDATE [Transactions] SET [TransactionDate] = @TransactionDate, [Description] = @Description WHERE [Id] = @Id", _Connection);
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

        public override void Add(ITransaction entity)
        {
            /* Add header record */
            base.Add(entity);

            var detailRepository = GetDetailRepository(entity.Type);
            detailRepository.AddRecord(entity);
        }

        public override void Update(ITransaction entity)
        {
            /* Update header record */
            base.Update(entity);

            var detailRepository = GetDetailRepository(entity.Type);
            detailRepository.UpdateRecord(entity);
        }

        public override void Delete(Guid id)
        {
            ITransaction entity = Get(id);

            /* Delete header record */
            base.Delete(id);

            var detailRepository = GetDetailRepository(entity.Type);
            detailRepository.DeleteRecord(entity.Id);
        }
        
        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            command.Parameters.AddWithValue("@Id", entity.Id.ToString());
            command.Parameters.AddWithValue("@TransactionDate", entity.TransactionDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Type", entity.Type);
            command.Parameters.AddWithValue("@ASXCode", entity.ASXCode);
            command.Parameters.AddWithValue("@Description", entity.Description);
        }

        protected override ITransaction CreateEntity(SQLiteDataReader reader)
        {
            return SQLitePortfolioEntityCreator.CreateTransaction(_Database as SQLitePortfolioDatabase, reader);             
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
        public void AddRecord(ITransaction entity)
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
        public void UpdateRecord(ITransaction entity)
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

        protected virtual void AddParameters(SQLiteCommand command, ITransaction entity)
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
            return "INSERT INTO [Aquisitions] ([Id], [Units], [AveragePrice], [TransactionCosts], [Comment]) VALUES (@Id, @Units, @AveragePrice, @TransactionCosts, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[Aquisitions] SET [Units] = @Units, [AveragePrice] = @AveragePrice, [TransactionCosts] = @TransactionCosts, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [Aquisitions] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            Aquisition aquisition = entity as Aquisition;

            command.Parameters.AddWithValue("@Id", aquisition.Id.ToString());
            command.Parameters.AddWithValue("@Units", aquisition.Units);
            command.Parameters.AddWithValue("@AveragePrice", SQLiteUtils.DecimalToDB(aquisition.AveragePrice));
            command.Parameters.AddWithValue("@TransactionCosts", SQLiteUtils.DecimalToDB(aquisition.TransactionCosts));
            command.Parameters.AddWithValue("@Comment", aquisition.Comment); 
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
            return "INSERT INTO [CostBaseAdjustments] ([Id], [RecordDate], [Percentage], [Comment]) VALUES (@Id, @RecordDate, @Percentage, @Comment)";

        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[CostBaseAdjustments] SET [RecordDate] = @RecordDate, [Percentage] = @Percentage, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [CostBaseAdjustments] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            CostBaseAdjustment costBaseAdjustment = entity as CostBaseAdjustment;

            command.Parameters.AddWithValue("@Id", costBaseAdjustment.Id.ToString());
            command.Parameters.AddWithValue("@RecordDate", costBaseAdjustment.RecordDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Percentage", SQLiteUtils.DecimalToDB(costBaseAdjustment.Percentage));
            command.Parameters.AddWithValue("@Comment", costBaseAdjustment.Comment); 
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
            return "INSERT INTO [Disposals] ([Id], [Units], [AveragePrice], [TransactionCosts], [CGTMethod], [Comment]) VALUES (@Id, @Units, @AveragePrice, @TransactionCosts, @CGTMethod, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[Disposals] SET [Units] = @Units, [AveragePrice] = @AveragePrice, [TransactionCosts] = @TransactionCosts, [CGTMethod] = @CGTMethod, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [Disposals] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            Disposal disposal = entity as Disposal;

            command.Parameters.AddWithValue("@Id", disposal.Id.ToString());
            command.Parameters.AddWithValue("@Units", disposal.Units);
            command.Parameters.AddWithValue("@AveragePrice", SQLiteUtils.DecimalToDB(disposal.AveragePrice));
            command.Parameters.AddWithValue("@TransactionCosts", SQLiteUtils.DecimalToDB(disposal.TransactionCosts));
            command.Parameters.AddWithValue("@CGTMethod", disposal.CGTMethod);
            command.Parameters.AddWithValue("@Comment", disposal.Comment); 
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
            return "INSERT INTO [IncomeReceived] ([Id], [RecordDate], [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred], [Comment]) VALUES (@Id, @RecordDate, @FrankedAmount, @UnfrankedAmount, @FrankingCredits, @Interest, @TaxDeferred, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[IncomeReceived] SET [RecordDate] = @RecordDate, [FrankedAmount] = @FrankedAmount, [UnfrankedAmount] = @UnfrankedAmount, [FrankingCredits] = @FrankingCredits, [Interest] = @Interest, [TaxDeferred] = @TaxDeferred, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [IncomeReceived] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            IncomeReceived incomeReceived = entity as IncomeReceived;

            command.Parameters.AddWithValue("@Id", incomeReceived.Id.ToString());
            command.Parameters.AddWithValue("@RecordDate", incomeReceived.RecordDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FrankedAmount", SQLiteUtils.DecimalToDB(incomeReceived.FrankedAmount));
            command.Parameters.AddWithValue("@UnfrankedAmount", SQLiteUtils.DecimalToDB(incomeReceived.UnfrankedAmount));
            command.Parameters.AddWithValue("@FrankingCredits", SQLiteUtils.DecimalToDB(incomeReceived.FrankingCredits));
            command.Parameters.AddWithValue("@Interest", SQLiteUtils.DecimalToDB(incomeReceived.Interest));
            command.Parameters.AddWithValue("@TaxDeferred", SQLiteUtils.DecimalToDB(incomeReceived.TaxDeferred));
            command.Parameters.AddWithValue("@Comment", incomeReceived.Comment);

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
            return "INSERT INTO [OpeningBalances] ([Id], [Units], [CostBase], [AquisitionDate], [Comment]) VALUES (@Id, @Units, @CostBase, @AquisitionDate, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[OpeningBalances] SET [Units] = @Units, [CostBase] = @CostBase, [AquisitionDate] = @AquisitionDate, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [OpeningBalances] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            OpeningBalance openingBalance = entity as OpeningBalance;

            command.Parameters.AddWithValue("@Id", openingBalance.Id.ToString());
            command.Parameters.AddWithValue("@Units", openingBalance.Units);
            command.Parameters.AddWithValue("@CostBase", SQLiteUtils.DecimalToDB(openingBalance.CostBase));
            command.Parameters.AddWithValue("@AquisitionDate", openingBalance.AquisitionDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Comment", openingBalance.Comment); 
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
            return "INSERT INTO [ReturnsOfCapital] ([Id], [RecordDate], [Amount], [Comment]) VALUES (@Id, @RecordDate, @Amount, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[ReturnsOfCapital] SET [RecordDate] = @RecordDate, [Amount] = @Amount, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [ReturnsOfCapital] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            ReturnOfCapital returnOfCapital = entity as ReturnOfCapital;

            command.Parameters.AddWithValue("@Id", returnOfCapital.Id.ToString());
            command.Parameters.AddWithValue("@RecordDate", returnOfCapital.RecordDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Amount", SQLiteUtils.DecimalToDB(returnOfCapital.Amount));
            command.Parameters.AddWithValue("@Comment", returnOfCapital.Comment); 
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
            return "INSERT INTO [UnitCountAdjustments] ([Id], [OriginalUnits], [NewUnits], [Comment]) VALUES (@Id, @OriginalUnits, @NewUnits, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[UnitCountAdjustments] SET [OriginalUnits] = @OriginalUnits, [NewUnits] = @NewUnits, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [UnitCountAdjustments] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            UnitCountAdjustment unitCountAdjustment = entity as UnitCountAdjustment;

            command.Parameters.AddWithValue("@Id", unitCountAdjustment.Id.ToString());
            command.Parameters.AddWithValue("@OriginalUnits", unitCountAdjustment.OriginalUnits);
            command.Parameters.AddWithValue("@NewUnits", unitCountAdjustment.NewUnits);
            command.Parameters.AddWithValue("@Comment", unitCountAdjustment.Comment);
        }
    }

}
