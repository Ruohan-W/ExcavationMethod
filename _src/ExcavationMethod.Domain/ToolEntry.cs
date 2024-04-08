namespace ExcavationMethod.Domain
{
    public class ToolEntry
    {
        public string ToolName { get; }
        public bool ThirdParty { get; }
        public ToolEntry(string toolName, bool thirdParty)
        {
            ToolName = toolName;
            ThirdParty = thirdParty;
        }

    }
}
