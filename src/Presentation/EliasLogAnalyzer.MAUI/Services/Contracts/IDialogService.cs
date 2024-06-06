using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliasLogAnalyzer.MAUI.Services.Contracts
{
    public interface IDialogService
    {
        Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel);
    }
}
