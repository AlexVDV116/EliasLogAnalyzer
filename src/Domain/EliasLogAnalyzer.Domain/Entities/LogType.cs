namespace EliasLogAnalyzer.Domain.Entities;

/// <summary>
/// Log type enumeration
/// </summary>
public enum LogType
{
    /// <summary>Not specified</summary>
    None,

    /// <summary>Debug</summary>
    Debug,

    /// <summary>Information</summary>
    Information,

    /// <summary>Warning</summary>
    Warning,

    /// <summary>Error</summary>
    Error
}