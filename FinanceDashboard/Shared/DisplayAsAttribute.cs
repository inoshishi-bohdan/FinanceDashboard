namespace FinanceDashboard.Shared
{
    public class DisplayAsAttribute : Attribute
    {
        private readonly string _displayAs;

        public DisplayAsAttribute(string displayAs)
        {
            _displayAs = displayAs;
        }

        public string DisplayAs => _displayAs;
    }
}
