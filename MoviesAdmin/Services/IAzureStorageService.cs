namespace MoviesAdmin.Services
{
    public interface IAzureStorageService
    {
        public Task<string> UploadFileAsync(IFormFile file, string FileName);
    }
}
