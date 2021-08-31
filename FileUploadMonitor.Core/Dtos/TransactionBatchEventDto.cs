namespace FileUploadMonitor.Core.Dtos
{
    public class TransactionBatchEventDto
    {
        public int From { get; set; }

        public int To { get; set; }
        
        public string FileName { get; set; }
    }
}