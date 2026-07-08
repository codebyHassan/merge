namespace AlignHR.Security
{
    /// <summary>
    /// Represents a menu item in the sidebar
    /// </summary>
    public class MenuItem
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public string Icon { get; set; } = "fas fa-circle";
        public int SortOrder { get; set; } = 999;
        public string FunctionName { get; set; } = string.Empty;
    }
}
