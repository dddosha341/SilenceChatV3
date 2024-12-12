using Silence.Infrastructure.DataContracts;
using Silence.Infrastructure.Helpers;

namespace Silence.Infrastructure.Services
{
    public class StorageService : IStorageService
    {
        private readonly string _path = "/storage/";
        public async Task<BaseResponse<bool>> Download(string fileName)
        {
            if (File.Exists(_path + fileName))
            {
                return new BaseResponse<bool>()
                {
                    StatusCode = StatusCode.OK,
                    Description = _path + fileName,
                    Data = true
                };
            }
            else
            {
                return new BaseResponse<bool>()
                {
                    StatusCode = StatusCode.FileNotFound,
                    Description = "There no file in storage",
                    Data = false
                };
            }
        }

        public async Task<BaseResponse<bool>> Upload(FileInfo file)
        {
            if (File.Exists(_path + file.FullName))
            {
                return new BaseResponse<bool>()
                {
                    StatusCode = StatusCode.FileAlreadyExists,
                    Description = "File already exists",
                    Data = false
                };
            }

            file.CopyTo(_path + file.FullName);

            return new BaseResponse<bool>()
            {
                StatusCode = StatusCode.OK,
                Data = true
            };

        }
    }
}
