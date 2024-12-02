namespace SDT.LBl
{
    public interface IFileService
    {
        public Task<FileDownloadDto> DownloadFile(long documentId);
    }
}
