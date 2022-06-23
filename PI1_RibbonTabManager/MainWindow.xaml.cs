using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PI1_RibbonTabManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region private members

        private UIDocument uidoc;

        private Document doc;

        private RibbonTabCollection tabs;

        #endregion

        #region public members

        public bool AllVisible { get; private set; } = Properties.Settings.Default.AllVisible;

        public ObservableCollection<RibbonTab> RibbonTabsList { get; private set; } = new ObservableCollection<RibbonTab>();

        #endregion

        #region constructor

        public MainWindow(UIDocument uidoc)
        {
            InitializeComponent();

            this.uidoc = uidoc;
            this.doc = uidoc.Application.ActiveUIDocument.Document;

            RibbonControl ribbon = ComponentManager.Ribbon;
            this.tabs = ribbon.Tabs;

            foreach (RibbonTab tab in tabs)
            {
                Type tabType = tab.GetType();
                if (!(tabType.Name == "RvtRibbonTab") && tab.Name != "PI1")
                {
                    RibbonTabsList.Add(tab);
                }
            }

            icRibbonTabsList.ItemsSource = RibbonTabsList;

            this.DataContext = this;
        }

        #endregion

        #region events

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        #endregion

        private void chbAllTabVisibility_ValueChange(object sender, RoutedEventArgs e)
        {
            bool chbValue = (bool)chbAllTabVisibility.IsChecked;
            Properties.Settings.Default.AllVisible = chbValue;
            Properties.Settings.Default.Save();

            foreach (RibbonTab tab in tabs)
            {
                Type tabType = tab.GetType();
                if (!(tabType.Name == "RvtRibbonTab") && tab.Name != "PI1")
                {
                    tab.IsVisible = chbValue;
                }
            }
        }
    }
}
