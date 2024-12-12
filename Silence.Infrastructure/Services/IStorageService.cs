using Silence.Infrastructure.DataContracts;

namespace Silence.Infrastructure.Services
{
    public interface IStorageService
    {
        Task<BaseResponse<bool>> Download(string fileName);

        Task<BaseResponse<bool>> Upload(FileInfo file);
    }
}
