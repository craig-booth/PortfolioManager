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
        void Serialize(Transaction transaction, XmlWriter xmlWriter);
        Transaction Deserialize(XmlReader xmlReader);
    }

    class TransactionExporter
    {
        private static string _NameSpace = @"http://portfolio.boothfamily.id.au";
        private TransactionSerializerFactory _TransactionSerializerFactory;

        public TransactionExporter()
        {
            _TransactionSerializerFactory = new TransactionSerializerFactory("pm", _NameSpace);
        }

        public void ExportTransactions(string fileName, IEnumerable<Transaction> transactions)
        {
            using (var fileStream = File.OpenWrite(fileName))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                    ExportTransactions(streamWriter, transactions);
            }
        }

        public void ExportTransactions(TextWriter textWriter, IEnumerable<Transaction> transactions)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true
            };
            using (var xmlWriter = XmlWriter.Create(textWriter, settings))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("pm", "transactions", _NameSpace);
                foreach (var transaction in transactions)
                {
                    var transactionSerializer = _TransactionSerializerFactory.GetSerializer(transaction);

                    transactionSerializer.Serialize(transaction, xmlWriter);
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
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

        public IEnumerable<Transaction> ImportTransactions(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                using (var streamReader = new StreamReader(fileStream))
                    return ImportTransactions(streamReader);
            }
        }

        public IEnumerable<Transaction> ImportTransactions(TextReader textReader)
        {
            var transactions = new List<Transaction>();

            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };
            using (var xmlReader = XmlReader.Create(textReader, settings))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.Name == "pm:transactions")
                    {
                        while (xmlReader.Read())
                        {
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.NamespaceURI == _NameSpace))
                            {
                                var transactionSerializer = _TransactionSerializerFactory.GetSerializer(xmlReader.LocalName);

                                if (transactionSerializer != null)
                                {
                                    var transaction = transactionSerializer.Deserialize(xmlReader);
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

        protected void WriteProperty(string name, string value, XmlWriter xmlWriter)
        {
            xmlWriter.WriteElementString(Prefix, name, NameSpace, value);
        }

        protected void WriteProperty(string name, DateTime value, XmlWriter xmlWriter)
        {
            xmlWriter.WriteElementString(Prefix, name, NameSpace, value.ToString("yyyy-MM-dd"));
        }

        protected void WriteProperty(string name, int value, XmlWriter xmlWriter)
        {
            xmlWriter.WriteElementString(Prefix, name, NameSpace, value.ToString());
        }

        protected void WriteProperty(string name, decimal value, XmlWriter xmlWriter)
        {
            xmlWriter.WriteElementString(Prefix, name, NameSpace, value.ToString("n5"));
        }

        protected void WriteProperty(string name, bool value, XmlWriter xmlWriter)
        {
            xmlWriter.WriteElementString(Prefix, name, NameSpace, (value ? "true" : "false"));
        }

        protected void WriteProperty(string name, Guid value, XmlWriter xmlWriter)
        {
            xmlWriter.WriteElementString(Prefix, name, NameSpace, value.ToString());
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

        protected abstract void SerializeProperties(T transaction, XmlWriter xmlWriter);
        protected abstract void SetProperty(T transaction, string propertyName, string propertyValue);

        public void Serialize(Transaction transaction, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("pm", ElementName, NameSpace);

            WriteProperty("transactiondate", transaction.TransactionDate, xmlWriter);
            WriteProperty("asxcode", transaction.ASXCode, xmlWriter);
            WriteProperty("recorddate", transaction.RecordDate, xmlWriter);
            WriteProperty("comment", transaction.Comment, xmlWriter);

            SerializeProperties((T)transaction, xmlWriter);

            xmlWriter.WriteEndElement();
        }

        public Transaction Deserialize(XmlReader xmlReader)
        {
            var transaction = new T();

            xmlReader.Read();

            while (!((xmlReader.NodeType == XmlNodeType.EndElement) && (xmlReader.LocalName == ElementName)))
            {
                var propertyName = xmlReader.LocalName;
                var propertyValue = xmlReader.ReadElementContentAsString();

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
