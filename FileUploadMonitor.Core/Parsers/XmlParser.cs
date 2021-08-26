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
using Microsoft.AspNetCore.Http;

namespace FileUploadMonitor.Core.Parsers
{
    public class XmlParser : IFileParser
    {
        public IEnumerable<TransactionDto> ParseFile(string fileBody, string fileName)
        {
            var transactionsList = new List<TransactionDto>();
            var exceptionList = new List<ValidationException>();
            var xDoc = new XmlDocument();
            var byteArray = Encoding.UTF8.GetBytes(fileBody);
            var stream = new MemoryStream(byteArray);
            xDoc.Load(stream);
            var xRoot = xDoc.DocumentElement;
            var transactionCounter = 0;

            if (xRoot == null)
            {
                throw new ValidationException("Invalid file structure", fileName);
            }
            foreach (XmlNode node in xRoot)
            {
                var transaction = new TransactionDto();
                if (node.Attributes != null)
                {
                    var transactionAttr = node.Attributes.GetNamedItem("id");
                    if (transactionAttr != null)
                    {
                        transaction.TransactionId = transactionAttr.Value;
                    }
                    else
                    {
                        exceptionList.Add(new ValidationException("Invalid value", $"transaction.{transactionCounter}.{nameof(transaction.TransactionId)}"));
                        continue;
                    }
                }
                foreach (XmlNode child in node.ChildNodes)
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
                            exceptionList.Add(new ValidationException("Invalid value", $"transaction.{transactionCounter}.{nameof(transaction.TransactionDate)}"));
                            continue;
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
                                    exceptionList.Add(new ValidationException("Invalid value", $"transaction.{transactionCounter}.{nameof(transaction.Amount)}"));
                                    continue;
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
                            exceptionList.Add(new ValidationException("Invalid value", $"transaction.{transactionCounter}.{nameof(transaction.Status)}"));
                        }
                    }
                }
                transactionsList.Add(transaction);
                transactionCounter++;
            }

            if (exceptionList.Count > 0)
            {
                throw new ValidationAggregationException("Transaction creation error", exceptionList);
            }

            return transactionsList;
        }
    }
}
