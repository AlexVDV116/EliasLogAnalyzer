namespace EliasLogAnalyzer.Domain.Entities
{
    public class BugReport
    {
        public string DeveloperName { get; set; }
        public string WorkstationName { get; set; }
        public DateTime ReportDateTime { get; set; }
        public string Situation { get; set; }
        public string Observation { get; set; }
        public string Expectation { get; set; }
        public string Tag { get; set; }
        public string Build { get; set; }
        public string Severity { get; set; }
        public string Analysis { get; set; }
        public string PossibleSolutions { get; set; }
        public string WhatToTest { get; set; }
        public int Effort { get; set; }
        public string Risk { get; set; }
        public string Workaround { get; set; }
        public string Recommendation { get; set; }

        public BugReport()
        {
            DeveloperName = string.Empty;
            WorkstationName = string.Empty;
            ReportDateTime = DateTime.MinValue;
            Situation = string.Empty;
            Observation = string.Empty;
            Expectation = string.Empty;
            Tag = string.Empty;
            Build = string.Empty;
            Severity = string.Empty;
            Analysis = string.Empty;
            PossibleSolutions = string.Empty;
            WhatToTest = string.Empty;
            Effort = 0;
            Risk = string.Empty;
            Workaround = string.Empty;
            Recommendation = string.Empty;
        }
    }

}
