namespace PortfolioManager.UI.Utilities
{

    enum DateSelectionType { None, Single, Range, FinancialYear }

    class PageViewOptions
    {
        public bool AllowStockSelection { get; set; }
        public DateSelectionType DateSelection { get; set; }
    }

    interface IPageViewModel
    {
        string Label { get; }
        string Heading { get; }
        PageViewOptions Options { get; }

        void Activate();
        void Deactivate();
    }

    abstract class PageViewModel : ViewModel, IPageViewModel
    {
        public string Label { get; protected set; }
        public string Heading { get; protected set; }
        public PageViewOptions Options { get; set; }

        public virtual void Activate()
        {
        }

        public virtual void Deactivate()
        {
        }

        public PageViewModel(string label)
        {
            Label = label;
            Heading = label;

            Options = new PageViewOptions()
            {
                AllowStockSelection = false,
                DateSelection = DateSelectionType.None
            };
        }
    }

}
