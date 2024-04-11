using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliasLogAnalyzer.MAUI.Services
{
    public interface ILogFileLoaderService
    {
        Task<IEnumerable<FileResult>> LoadLogFilesAsync();
    }
}
