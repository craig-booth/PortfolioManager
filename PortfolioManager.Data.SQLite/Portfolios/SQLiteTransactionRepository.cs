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
        protected internal SQLiteTransactionRepository(SQLitePortfolioDatabase database)
            : base(database, "Transactions")
        {
            _Database = database;
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

        private TransactionDetailRepository _AquisitionRepository;
        private TransactionDetailRepository _CostBaseAdjustmentRepository;
        private TransactionDetailRepository _DisposalRepository;
        private TransactionDetailRepository _IncomeReceivedRepository;
        private TransactionDetailRepository _OpeningBalanceRepository;
        private TransactionDetailRepository _ReturnOfCapitalRepository;
        private TransactionDetailRepository GetDetailRepository(TransactionType type)
        {
            if (type == TransactionType.Aquisition)
            {
                if (_AquisitionRepository == null)
                    _AquisitionRepository = new AquisitionRepository(_Connection);
                return _AquisitionRepository;
            }
           else if (type == TransactionType.CostBaseAdjustment)
            {
                if (_CostBaseAdjustmentRepository == null)
                    _CostBaseAdjustmentRepository = new CostBaseAdjustmentRepository(_Connection);
                return _CostBaseAdjustmentRepository;
            }
            else if (type == TransactionType.Disposal)
            {
                if (_DisposalRepository == null)
                    _DisposalRepository = new DisposalRepository(_Connection);
                return _DisposalRepository;
            }
            else if (type == TransactionType.Income)
            {
                if (_IncomeReceivedRepository == null)
                    _IncomeReceivedRepository = new IncomeReceivedRepository(_Connection);
                return _IncomeReceivedRepository;
            }
            else if (type == TransactionType.OpeningBalance)
            {
                if (_OpeningBalanceRepository == null)
                    _OpeningBalanceRepository = new OpeningBalanceRepository(_Connection);
                return _OpeningBalanceRepository;
            }
            else if (type == TransactionType.ReturnOfCapital)
            {
                if (_ReturnOfCapitalRepository == null)
                    _ReturnOfCapitalRepository = new ReturnOfCapitalRepository(_Connection);
                return _ReturnOfCapitalRepository;
            }  
            else
                return null;
            
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

        public int DecimalToDB(decimal value)
        {
            return (int)Math.Floor(value * 100000);
        }

        public decimal DBToDecimal(int value)
        {
            return (decimal)value / 100000;
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
            command.Parameters.AddWithValue("@AveragePrice", DecimalToDB(aquisition.AveragePrice));
            command.Parameters.AddWithValue("@TransactionCosts", DecimalToDB(aquisition.TransactionCosts));
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
            return "INSERT INTO [CostBaseAdjustments] ([Id], [Methpd], [Value], [Comment]) VALUES (@Id, @Percentage, @Comment)";

        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[CostBaseAdjustments] SET [Method] = @Method, [Value] = @Value, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [CostBaseAdjustments] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            CostBaseAdjustment costBaseAdjustment = entity as CostBaseAdjustment;

            command.Parameters.AddWithValue("@Id", costBaseAdjustment.Id.ToString());
            command.Parameters.AddWithValue("@Method", costBaseAdjustment.Method);
            command.Parameters.AddWithValue("@Percentage", DecimalToDB(costBaseAdjustment.Value));
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
            command.Parameters.AddWithValue("@AveragePrice", DecimalToDB(disposal.AveragePrice));
            command.Parameters.AddWithValue("@TransactionCosts", DecimalToDB(disposal.TransactionCosts));
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
            return "INSERT INTO [IncomeReceived] ([Id], [PaymentDate], [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred], [Comment]) VALUES (@Id, @PaymentDate, @FrankedAmount, @UnfrankedAmount, @FrankingCredits, @Interest, @TaxDeferred, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[IncomeReceived] SET [PaymentDate] = @PaymentDate, [FrankedAmount] = @FrankedAmount, [UnfrankedAmount] = @UnfrankedAmount, [FrankingCredits] = @FrankingCredits, [Interest] = @Interest, [TaxDeferred] = @TaxDeferred, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [IncomeReceived] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            IncomeReceived incomeReceived = entity as IncomeReceived;

            command.Parameters.AddWithValue("@Id", incomeReceived.Id.ToString());
            command.Parameters.AddWithValue("@PaymentDate", incomeReceived.PaymentDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FrankedAmount", DecimalToDB(incomeReceived.FrankedAmount));
            command.Parameters.AddWithValue("@UnfrankedAmount", DecimalToDB(incomeReceived.UnfrankedAmount));
            command.Parameters.AddWithValue("@FrankingCredits", DecimalToDB(incomeReceived.FrankingCredits));
            command.Parameters.AddWithValue("@Interest", DecimalToDB(incomeReceived.Interest));
            command.Parameters.AddWithValue("@TaxDeferred", DecimalToDB(incomeReceived.TaxDeferred));
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
            return "INSERT INTO [OpeningBalances] ([Id], [Units], [CostBase], [Comment]) VALUES (@Id, @Units, @CostBase, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[OpeningBalances] SET [Units] = @Units, [CostBase] = @CostBase, [Comment] = @Comment WHERE [Id] = @Id";
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
            command.Parameters.AddWithValue("@CostBase", DecimalToDB(openingBalance.CostBase));
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
            return "INSERT INTO [ReturnsOfCapital] ([Id], [Amount], [Comment]) VALUES (@Id, @Amount, @Comment)";
        }

        protected override string GetUpdateSQL()
        {
            return "UPDATE[ReturnsOfCapital] SET [Amount] = @Amount, [Comment] = @Comment WHERE [Id] = @Id";
        }

        protected override string GetDeleteSQL()
        {
            return "DELETE FROM [ReturnsOfCapital] WHERE [Id] = @Id";
        }

        protected override void AddParameters(SQLiteCommand command, ITransaction entity)
        {
            ReturnOfCapital returnOfCapital = entity as ReturnOfCapital;

            command.Parameters.AddWithValue("@Id", returnOfCapital.Id.ToString());
            command.Parameters.AddWithValue("@Amount", DecimalToDB(returnOfCapital.Amount));
            command.Parameters.AddWithValue("@Comment", returnOfCapital.Comment); 
        }
    }

}
