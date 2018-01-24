using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Transactions
{
    interface ITransactionSerializer
    {
        string ElementName { get; }
        Task Serialize(Transaction transaction, XmlWriter xmlWriter);
        Task<Transaction> Deserialize(XmlReader xmlReader);
    }

    class TransactionExporter
    {
        private static string _NameSpace = @"http://portfolio.boothfamily.id.au";
        private TransactionSerializerFactory _TransactionSerializerFactory;

        public TransactionExporter()
        {
            _TransactionSerializerFactory = new TransactionSerializerFactory("pm", _NameSpace);
        }

        public Task ExportTransactions(string fileName, IEnumerable<Transaction> transactions)
        {
            using (var fileStream = File.OpenWrite(fileName))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                    return ExportTransactions(streamWriter, transactions);
            }
        }

        public async Task ExportTransactions(TextWriter textWriter, IEnumerable<Transaction> transactions)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = true
            };
            using (var xmlWriter = XmlWriter.Create(textWriter, settings))
            {
                await xmlWriter.WriteStartDocumentAsync();

                await xmlWriter.WriteStartElementAsync("pm", "transactions", _NameSpace);
                foreach (var transaction in transactions)
                {
                    var transactionSerializer = _TransactionSerializerFactory.GetSerializer(transaction);

                    await transactionSerializer.Serialize(transaction, xmlWriter);
                }
                await xmlWriter.WriteEndElementAsync();

                await xmlWriter.WriteEndDocumentAsync();
            }
        }
    }

    class TransactionImporter
    {
        private static string _NameSpace = @"http://portfolio.boothfamily.id.au";
        private TransactionSerializerFactory _TransactionSerializerFactory;

        public TransactionImporter()
        {
            _TransactionSerializerFactory = new TransactionSerializerFactory("pm", _NameSpace);
        }

        public Task<IEnumerable<Transaction>> ImportTransactions(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                using (var streamReader = new StreamReader(fileStream))
                    return ImportTransactions(streamReader);
            }
        }

        public async Task<IEnumerable<Transaction>> ImportTransactions(TextReader textReader)
        {
            var transactions = new List<Transaction>();

            var settings = new XmlReaderSettings
            {
                Async = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };
            using (var xmlReader = XmlReader.Create(textReader, settings))
            {
                while (await xmlReader.ReadAsync())
                {
                    if (xmlReader.Name == "pm:transactions")
                    {
                        while (await xmlReader.ReadAsync())
                        {
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.NamespaceURI == _NameSpace))
                            {
                                var transactionSerializer = _TransactionSerializerFactory.GetSerializer(xmlReader.LocalName);

                                if (transactionSerializer != null)
                                {
                                    var transaction = await transactionSerializer.Deserialize(xmlReader);
                                    transactions.Add(transaction);
                                }
                            }

                        }
                    }
                }
            }

            return transactions;
        }
    }

    class TransactionSerializerFactory
    {
        private readonly ServiceFactory<ITransactionSerializer> _TransactionSerializers = new ServiceFactory<ITransactionSerializer>();
        private readonly Dictionary<string, Type> _ElementNames = new Dictionary<string, Type>();

        public TransactionSerializerFactory(string prefix, string nameSpace)
        {
            AddSerializer<CashTransaction>(new CashTransactionSerializer(prefix, nameSpace))
                .AddSerializer<Aquisition>(new AquisitionSerializer(prefix, nameSpace))
                .AddSerializer<Disposal>(new DisposalSerializer(prefix, nameSpace))
                .AddSerializer<CostBaseAdjustment>(new CostBaseAdjustmentSerializer(prefix, nameSpace))
                .AddSerializer<IncomeReceived>(new IncomeReceivedSerializer(prefix, nameSpace))
                .AddSerializer<OpeningBalance>(new OpeningBalanceSerializer(prefix, nameSpace))
                .AddSerializer<ReturnOfCapital>(new ReturnOfCapitalSerializer(prefix, nameSpace))
                .AddSerializer<UnitCountAdjustment>(new UnitCountAdjustmentSerializer(prefix, nameSpace));
        }

        private TransactionSerializerFactory AddSerializer<T>(ITransactionSerializer serializer)
        {
            _TransactionSerializers.Register<T>(serializer);
            _ElementNames.Add(serializer.ElementName, typeof(T));

            return this;
        }

        public ITransactionSerializer GetSerializer(Transaction transaction)
        {
            return _TransactionSerializers.GetService(transaction);
        }

        public ITransactionSerializer GetSerializer(string elementName)
        {
            if (_ElementNames.TryGetValue(elementName, out var type))
                return _TransactionSerializers.GetService(type);
            else
                return null;
        }
    }

    abstract class TransactionSerializer<T> where T: Transaction, new()
    {
        public string Prefix { get; }
        public string NameSpace { get; }
        public string ElementName { get; }

        public TransactionSerializer(string prefix, string nameSpace, string elementName)
        {
            Prefix = prefix;
            NameSpace = nameSpace;
            ElementName = elementName;
        }

        protected Task WriteProperty(string name, string value, XmlWriter xmlWriter)
        {
            return xmlWriter.WriteElementStringAsync(Prefix, name, NameSpace, value);
        }

        protected Task WriteProperty(string name, DateTime value, XmlWriter xmlWriter)
        {
            return xmlWriter.WriteElementStringAsync(Prefix, name, NameSpace, value.ToString("yyyy-MM-dd"));
        }

        protected Task WriteProperty(string name, int value, XmlWriter xmlWriter)
        {
            return xmlWriter.WriteElementStringAsync(Prefix, name, NameSpace, value.ToString());
        }

        protected Task WriteProperty(string name, decimal value, XmlWriter xmlWriter)
        {
            return xmlWriter.WriteElementStringAsync(Prefix, name, NameSpace, value.ToString("n5"));
        }

        protected Task WriteProperty(string name, bool value, XmlWriter xmlWriter)
        {
            return xmlWriter.WriteElementStringAsync(Prefix, name, NameSpace, (value ? "true" : "false"));
        }

        protected Task WriteProperty(string name, Guid value, XmlWriter xmlWriter)
        {
            return xmlWriter.WriteElementStringAsync(Prefix, name, NameSpace, value.ToString());
        }

        protected DateTime PropertyAsDateTime(string value)
        {
            return DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.CurrentCulture);
        }

        protected int PropertyAsInteger(string value)
        {
            return int.Parse(value);
        }

        protected decimal PropertyAsDecimal(string value)
        {
            return decimal.Parse(value);
        }

        protected bool PropertyAsBoolean(string value)
        {
            return (value == "true");
        }

        protected Guid PropertyAsGuid(string value)
        {
             return Guid.Parse(value);
        }

        protected abstract Task SerializeProperties(T transaction, XmlWriter xmlWriter);
        protected abstract void SetProperty(T transaction, string propertyName, string propertyValue);

        public async Task Serialize(Transaction transaction, XmlWriter xmlWriter)
        {
            await xmlWriter.WriteStartElementAsync("pm", ElementName, NameSpace);

            await WriteProperty("transactiondate", transaction.TransactionDate, xmlWriter);
            await WriteProperty("asxcode", transaction.ASXCode, xmlWriter);
            await WriteProperty("recorddate", transaction.RecordDate, xmlWriter);
            await WriteProperty("comment", transaction.Comment, xmlWriter);

            await SerializeProperties((T)transaction, xmlWriter);

            await xmlWriter.WriteEndElementAsync();
        }

        public async Task<Transaction> Deserialize(XmlReader xmlReader)
        {
            var transaction = new T();

            await xmlReader.ReadAsync();

            while (!((xmlReader.NodeType == XmlNodeType.EndElement) && (xmlReader.LocalName == ElementName)))
            {
                var propertyName = xmlReader.LocalName;
                var propertyValue = await xmlReader.ReadElementContentAsStringAsync();

                if (propertyName == "transactiondate")
                    transaction.TransactionDate = PropertyAsDateTime(propertyValue);
                else if (propertyName == "asxcode")
                    transaction.ASXCode = propertyValue;
                else if (propertyName == "recorddate")
                    transaction.RecordDate = PropertyAsDateTime(propertyValue);
                else if (propertyName == "comment")
                    transaction.Comment = propertyValue;
                else
                    SetProperty(transaction, propertyName, propertyValue);
            }

            return transaction;
        }
    }
}
