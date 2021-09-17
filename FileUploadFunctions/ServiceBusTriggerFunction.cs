using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;
using FileUploadMonitor.Core.Services;
using FileUploadMonitor.Domain.Entities;
using FileUploadMonitor.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileUploadFunctions
{
    public class ServiceBusTriggerFunction
    {
        private readonly IBlobService _blobService;
        
        private readonly IFileUploadService _fileUploadService;

        private readonly ITransactionRepository _transactionsRepository;

        public ServiceBusTriggerFunction(IBlobService blobService, IFileUploadService fileUploadService, ITransactionRepository transactionsRepository)
        {
            _blobService = blobService;
            _fileUploadService = fileUploadService;
            _transactionsRepository = transactionsRepository;
        }

        [Function("ServiceBusTriggerFunction")]
        public async Task Run([ServiceBusTrigger("fileupload", Connection = "ServiceBusConnectionRead")] string fileContent, FunctionContext context)
        {
            ILogger logger = context.GetLogger("ServiceBusTriggerFunction");
            TransactionBatchEventDto? deserializedMessages = JsonConvert.DeserializeObject<TransactionBatchEventDto>(fileContent);
            if (deserializedMessages != null)
            {
                Task<string> fileBody = _blobService.GetBlob(deserializedMessages.FileName);
                IFileParser parser = _fileUploadService.GetFileParser(deserializedMessages.FileName);
                var transactionDtoList = parser.ParseTransaction(deserializedMessages.From, deserializedMessages.To, fileBody.Result).ToList();
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<TransactionDto, Transaction>()
                        .ForMember("Id", opt => opt.MapFrom(c => c.TransactionId))
                        .ForMember("TransactionDate", opt => opt.MapFrom(c => c.TransactionDate))
                        .ForMember("Status", opt => opt.MapFrom(c => c.Status))
                        .ForMember("CurrencyCode", opt => opt.MapFrom(c => c.CurrencyCode))
                        .ForMember("Amount", opt => opt.MapFrom(c => c.Amount));
                });
                IMapper iMapper = config.CreateMapper();
                List<Transaction> transactionList = iMapper.Map<List<TransactionDto>, List<Transaction>>(transactionDtoList);
                await _transactionsRepository.AddRange(transactionList);
                logger.LogInformation($"\n \n New Message received: \n {fileContent} \n \n \n transaction output: \n {JsonConvert.SerializeObject(transactionDtoList)}");
            }
        }
    }
}
