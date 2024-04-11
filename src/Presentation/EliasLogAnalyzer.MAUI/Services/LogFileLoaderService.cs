using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.Services
{
    public class LogFileLoaderService : ILogFileLoaderService
    {
        public async Task<IEnumerable<FileResult>> LoadLogFilesAsync()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new List<string> { ".log" } },
                { DevicePlatform.MacCatalyst , new List<string> { "public.log" } }
            });

            var results = await FilePicker.PickMultipleAsync(new PickOptions
            {
                FileTypes = customFileType
            });

            // TODO: Implement the logic to parse each log file into a LogFile domain model and return an IEnumerable of the results
            return results;
        }
    }
}
