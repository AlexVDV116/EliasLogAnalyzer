using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogEntryAnalysisService
{
    void CalcDiffTicks();
    void AnalyzeLogEntries();

}