using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Common.Enums;
using Common.Exceptions;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;

namespace FileUploadMonitor.Core.Parsers
{
    public class XmlParser : IFileParser
    {
        public IEnumerable<TransactionBatchEventDto> ParseFile(string fileBody, string fileName)
        {
            var exceptionList = new List<ValidationException>();
            var xDoc = new XmlDocument();
            var byteArray = Encoding.UTF8.GetBytes(fileBody);
            var stream = new MemoryStream(byteArray);
            xDoc.Load(stream);
            var xRoot = xDoc.DocumentElement;

            if (xRoot == null)
            {
                throw new ValidationException("Invalid file structure", fileName);
            }


            if (exceptionList.Count > 0)
            {
                throw new ValidationAggregationException("Transaction creation error", exceptionList);
            }

            //TODO Add logic to get lines of transaction from body
            throw new NotImplementedException("XML type is not supported yet");

        }

        public IEnumerable<TransactionDto> ParseTransaction(int from, int to, string fileBody)
        {
            var xDoc = new XmlDocument();
            var byteArray = Encoding.UTF8.GetBytes(fileBody);
            var stream = new MemoryStream(byteArray);
            xDoc.Load(stream);
            var xRoot = xDoc.DocumentElement;
            if (xRoot == null)
            {
                throw new ValidationException("Invalid file structure", nameof(fileBody));
            }

            var transaction = new TransactionDto();

            var transactionAttr = xRoot.Attributes.GetNamedItem("id");
            if (transactionAttr != null)
            {
                transaction.TransactionId = transactionAttr.Value;
            }
            else
            {
                throw new ValidationException("Invalid value",
                    $"transaction.{nameof(transaction.TransactionId)}");
            }

            foreach (XmlNode child in xRoot.ChildNodes)
            {
                if (child.Name == "TransactionDate")
                {
                    if (DateTime.TryParseExact(child.InnerText, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var dateTime))
                    {
                        transaction.TransactionDate = dateTime;
                    }
                    else
                    {
                        throw new ValidationException("Invalid value",
                            $"transaction.{nameof(transaction.TransactionDate)}");
                    }
                }

                if (child.Name == "PaymentDetails")
                {
                    foreach (XmlNode childnode in child.ChildNodes)
                    {
                        if (childnode.Name == "Amount")
                        {
                            if (decimal.TryParse(childnode.InnerText, out var amount))
                            {
                                transaction.Amount = amount;
                            }
                            else
                            {
                                throw new ValidationException("Invalid value",
                                    $"transaction.{nameof(transaction.Amount)}");
                            }
                        }

                        if (childnode.Name == "CurrencyCode")
                        {
                            transaction.CurrencyCode = childnode.InnerText;
                        }
                    }
                }

                if (child.Name == "Status")
                {
                    if (Enum.TryParse(child.InnerText, true, out StatusType status))
                    {
                        transaction.Status = status;
                    }
                    else
                    {
                        throw new ValidationException("Invalid value",
                            $"transaction.{nameof(transaction.Status)}");
                    }
                }
            }
            throw new NotImplementedException("XML type is not supported yet");

            //return transaction;
        }
    }
}