﻿using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface IHashService
{
    string GenerateLogEntryHash(LogEntry logEntry);
    string GenerateLogFileHash(LogFile logFile);
}
