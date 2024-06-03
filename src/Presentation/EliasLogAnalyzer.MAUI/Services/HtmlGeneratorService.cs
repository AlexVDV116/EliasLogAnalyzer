using System;
using System.Collections.Generic;
using System.Linq;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Net;
using System.Text;
using System.Text.Json;
using EliasLogAnalyzer.MAUI.Resources;

namespace EliasLogAnalyzer.MAUI.Services
{
    public class HtmlGeneratorService : IHtmlGeneratorService
    {
        private readonly ISettingsService _settingsService;
        
        private Theme CurrentTheme => _settingsService.AppTheme;
        private string BackgroundColor => CurrentTheme == Theme.Light ? "#f9f9f9" : "#333";
        private string TextColor => CurrentTheme == Theme.Light ? "#333" : "#f9f9f9";
        private string HeaderColor => CurrentTheme == Theme.Light ? "#e9e9e9" : "#333";
        private string BorderColor => CurrentTheme == Theme.Light ? "#ddd" : "#666";
        
        
        public HtmlGeneratorService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        
        public string ConvertDataToHtml(LogEntry logEntry)
        {
            var dateTimeString = logEntry.LogTimeStamp.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            var logTypeString = logEntry.LogType.ToString();
            var sourceString = logEntry.Source;
            var userString = logEntry.User;
            var computerString = logEntry.Computer;
            var descriptionString = logEntry.Description;
            var markedAsMainText = logEntry.IsMarked ? "\u2B50" : string.Empty;

            var dataString = WebUtility.HtmlEncode(logEntry.Data);

            return $@"
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <style>
                    body {{ margin: 0; padding: 0; font-family: 'Consolas', 'Courier New', monospace; background-color: {BackgroundColor}; color: {TextColor}; }}
                    pre {{ margin: 0; padding: 10px 20px; white-space: pre-wrap; word-wrap: break-word; }}
                    h4 {{ margin: 0; padding: 10px 20px; background-color: {HeaderColor}; color: {TextColor}; border-bottom: 1px solid {BorderColor}; }}
                </style>
            </head>
            <body>
                <h4><pre><b>{markedAsMainText} {WebUtility.HtmlEncode(dateTimeString)}  -  {WebUtility.HtmlEncode(logTypeString)}  -  {WebUtility.HtmlEncode(sourceString)}  -  {WebUtility.HtmlEncode(userString)}  -  {WebUtility.HtmlEncode(computerString)}</b></pre></h4>
                <pre><b>Description: {WebUtility.HtmlEncode(descriptionString)}</b></pre>
                <pre>Data: {dataString}</pre>
            </body>
            </html>";
        }

        public string GenerateTimeLineHtml(IEnumerable<LogEntry> logEntries)
        {
            var sb = new StringBuilder();
            AppendHtmlHeader(sb, "main", "calc(100% - 120px)", "400px");

            if (!logEntries.Any())
            {
                AppendEmptyChartScript(sb, "main", "line");
            }
            else
            {
                AppendTimeLineChartScript(sb, logEntries);
            }

            AppendHtmlFooter(sb);
            return sb.ToString();
        }

        public string GeneratePieChartHtml(IEnumerable<LogEntry> logEntries)
        {
            var sb = new StringBuilder();
            AppendHtmlHeader(sb, "pieChart", "calc(100% - 80px)", "100%");

            if (!logEntries.Any())
            {
                AppendEmptyChartScript(sb, "pieChart", "pie");
            }
            else
            {
                AppendPieChartScript(sb, logEntries);
            }

            AppendHtmlFooter(sb);
            return sb.ToString();
        }

        private static void AppendHtmlHeader(StringBuilder sb, string divId, string width, string height)
        {
            sb.AppendLine("<html><head>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; font-family: Arial; }");
            sb.AppendFormat("#{0} {{ width: {1}; height: {2}; }}\n", divId, width, height);
            sb.AppendLine("</style>");
            sb.AppendLine("<script src=\"Resources/JavaScript/echarts.min.js\"></script>");
            sb.AppendLine("</head><body>");
            sb.AppendFormat("<div id='{0}'></div>\n", divId);
            sb.AppendLine("<script>");
        }

        private static void AppendEmptyChartScript(StringBuilder sb, string divId, string chartType)
        {
            sb.AppendFormat("var myChart = echarts.init(document.getElementById('{0}'));\n", divId);
            sb.AppendLine("var option = {");
            sb.AppendLine("    tooltip: { show: false },");

            if (chartType == "line")
            {
                sb.AppendLine("    xAxis: {");
                sb.AppendLine("        type: 'category',");
                sb.AppendLine("        data: ['No Data'],");
                sb.AppendLine("    },");
                sb.AppendLine("    yAxis: {");
                sb.AppendLine("        type: 'value',");
                sb.AppendLine("        show: true,");
                sb.AppendLine("    },");
                sb.AppendLine("    series: [{");
                sb.AppendLine("        data: [0],");
                sb.AppendLine("        type: 'line',");
                sb.AppendLine("        showSymbol: true,");
                sb.AppendLine("        symbolSize: 0,");
                sb.AppendLine("    }]");
            }
            else if (chartType == "pie")
            {
                sb.AppendLine("    legend: { data: ['No Data'] },");
                sb.AppendLine("    series: [{");
                sb.AppendLine("        type: 'pie',");
                sb.AppendLine("        radius: '50%',");
                sb.AppendLine("        data: [{ value: 0, name: 'No Data' }],");
                sb.AppendLine("        emphasis: {");
                sb.AppendLine("            itemStyle: {");
                sb.AppendLine("                shadowBlur: 10,");
                sb.AppendLine("                shadowOffsetX: 0,");
                sb.AppendLine("                shadowColor: 'rgba(0, 0, 0, 0.5)'");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
                sb.AppendLine("    }]");
            }

            sb.AppendLine("};");
            sb.AppendLine("myChart.setOption(option);");
        }

        private static void AppendHtmlFooter(StringBuilder sb)
        {
            sb.AppendLine("</script>");
            sb.AppendLine("</body></html>");
        }

        private static void AppendTimeLineChartScript(StringBuilder sb, IEnumerable<LogEntry> logEntries)
        {
            var times = logEntries
           .GroupBy(e => new DateTime(e.LogTimeStamp.DateTime.Year, e.LogTimeStamp.DateTime.Month, e.LogTimeStamp.DateTime.Day, e.LogTimeStamp.DateTime.Hour, e.LogTimeStamp.DateTime.Minute, 0))
           .Select(g => g.Key)
           .OrderBy(t => t)
           .ToList();

            var logTypes = new List<LogType> { LogType.Error, LogType.Warning, LogType.Information, LogType.Debug, LogType.None };
            sb.AppendLine("var myChart = echarts.init(document.getElementById('main'));");
            sb.AppendLine("window.onresize = function() { myChart.resize(); };");
            sb.AppendLine("var option = {");
            sb.AppendLine("    tooltip: { trigger: 'axis' },");
            sb.AppendLine("    legend: {");
            sb.AppendLine("        data: " + JsonSerializer.Serialize(logTypes.Select(lt => lt.ToString())) + ",");
            sb.AppendLine("        orient: 'vertical',");
            sb.AppendLine("        left: 'right',");
            sb.AppendLine("        selected: {");
            // Default all log types to not visible except for errors
            logTypes.ForEach(logType =>
            {
                sb.AppendFormat("            '{0}': {1},\n", logType, logType == LogType.Error ? "true" : "false");
            });
            sb.AppendLine("        }");
            sb.AppendLine("    },");
            sb.AppendLine("    xAxis: { type: 'category', data: " + JsonSerializer.Serialize(times.Select(t => t.ToString("yyyy-MM-dd HH:mm:ss"))) + " },");
            sb.AppendLine("    yAxis: {");
            sb.AppendLine("        type: 'value',");
            sb.AppendLine("        minInterval: 1,");
            sb.AppendLine("        axisLabel: {");
            sb.AppendLine("            formatter: '{value}',");
            sb.AppendLine("        }");
            sb.AppendLine("    },");
            sb.AppendLine("    series: [");

            foreach (var logType in logTypes)
            {
                var data = times.Select(time => logEntries.Count(e =>
                    e.LogTimeStamp.DateTime >= time && e.LogTimeStamp.DateTime < time.AddMinutes(1) &&
                    e.LogType == logType)).ToList();

                sb.AppendLine("{");
                sb.AppendLine($"    name: '{logType}',");
                sb.AppendLine("    type: 'line',");
                sb.AppendLine("    data: " + JsonSerializer.Serialize(data) + ",");
                sb.AppendLine("    itemStyle: { color: '" + GetColorForLogType(logType.ToString()) + "' },");
                sb.AppendLine("    lineStyle: { color: '" + GetColorForLogType(logType.ToString()) + "' },");
                sb.AppendLine("},");
            }

            sb.AppendLine("    ]");
            sb.AppendLine("};");
            sb.AppendLine("myChart.setOption(option);");
        }

        private static void AppendPieChartScript(StringBuilder sb, IEnumerable<LogEntry> logEntries)
        {
            sb.AppendLine("var pieChart = echarts.init(document.getElementById('pieChart'));");
            sb.AppendLine("window.onresize = function() { pieChart.resize(); };");

            var logTypeCounts = logEntries
              .GroupBy(e => e.LogType)
              .Select(g => new
              {
                  name = g.Key.ToString(),
                  value = g.Count(),
                  itemStyle = new { color = GetColorForLogType(g.Key.ToString()) }
              })
              .ToList();

            var logTypes = new List<LogType> { LogType.Error, LogType.Warning, LogType.Information, LogType.Debug, LogType.None };

            sb.AppendLine("var option = {");
            sb.AppendLine("    tooltip: { trigger: 'item' },");
            sb.AppendLine("    legend: { orient: 'vertical', left: 'left', data: " + JsonSerializer.Serialize(logTypes.Select(lt => lt.ToString())) + " },");
            sb.AppendLine("    series: [{ name: 'Log Type', type: 'pie', radius: '50%', data: " + JsonSerializer.Serialize(logTypeCounts) + ", emphasis: { itemStyle: { shadowBlur: 10, shadowOffsetX: 0, shadowColor: 'rgba(0, 0, 0, 0.5)' } } }]");
            sb.AppendLine("};");
            sb.AppendLine("pieChart.setOption(option);");
        }

        private static string GetColorForLogType(string logType)
        {
            return logType switch
            {
                "Error" => "#FF0000",  // Red
                "Warning" => "#FFA500",  // Orange
                "Information" => "#008000",  // Green
                "Debug" => "#000000",  // Black
                "None" => "#808080",  // Gray
                _ => "#808080"  // Default to gray for undefined types
            };
        }
    }
}
