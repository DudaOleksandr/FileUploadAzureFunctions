using System.Threading.Tasks;

namespace FileUploadMonitor.Core.Services
{
    public interface IBlobService
    {
        public Task<string> GetBlob(string fileName);
    }
}