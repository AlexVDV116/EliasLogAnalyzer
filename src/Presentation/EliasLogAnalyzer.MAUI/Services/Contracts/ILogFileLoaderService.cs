namespace EliasLogAnalyzer.MAUI.Services.Contracts
{
    public interface ILogFileLoaderService
    {
        Task<IEnumerable<FileResult>> LoadLogFilesAsync();
    }
}
