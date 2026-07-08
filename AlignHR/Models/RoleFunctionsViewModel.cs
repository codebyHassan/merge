namespace AlignHR.Models
{
    public class FunctionItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Route { get; set; } = "";
    }

    public class RoleFunctionsViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public List<FunctionItem> AllFunctions { get; set; } = new();
        public List<int> SelectedFunctionIds { get; set; } = new();
    }
}
