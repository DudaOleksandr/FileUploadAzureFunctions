using System;

namespace FileUploadMonitor.Core.Dtos
{
    public class TransactionBatchEventDto
    {
        public int From { get; set; }

        public int To { get; set; }
        
        public string FileName { get; set; }

        protected bool Equals(TransactionBatchEventDto other)
        {
            return From == other.From && To == other.To && FileName == other.FileName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((TransactionBatchEventDto)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, FileName);
        }
    }
}
