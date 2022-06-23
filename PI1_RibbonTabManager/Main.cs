using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using PI1_UI;
using System;
using System.Linq;

namespace PI1_RibbonTabManager
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    // Running interface class
    public class Main : IExternalApplication
    {
        #region public methods

        /// <summary>
        /// Implement this method to execute some tasks when Autodesk Revit shuts down.
        /// </summary>
        /// <param name="application">A handle to the application being shut down.</param>
        /// <returns>
        /// Indicates if the external application completes its work successfully.
        /// </returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened -= application_DocumentOpened;

            return Result.Succeeded;
        }

        /// <summary>
        /// Implement this method to create tab, ribbon and button or add elements if tab and ribbon was created when Autodesk Revit starts.
        /// </summary>
        /// <param name="application">A handle to the application being started.</param>
        /// <returns>
        /// Indicates if the external application completes its work successfully.
        /// </returns>
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "PI1";
            string ribbonPanelName = RibbonName.Name(RibbonNameType.Instruments); ;
            Autodesk.Revit.UI.RibbonPanel ribbonPanel = null;

            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { }

            try
            {
                ribbonPanel = application.CreateRibbonPanel(tabName, ribbonPanelName);
            }
            catch
            {
                ribbonPanel = application.GetRibbonPanels(tabName)
                    .FirstOrDefault(panel => panel.Name.Equals(ribbonPanelName));
            }

            var btnData = new RevitPushButtonData
            {
                Label = "Менеджер\nприложений",
                Panel = ribbonPanel,
                ToolTip = "С помощью менеджера приложений можно скрыть или показать те или иные загруженные в Revit приложения.",
                CommandNamespacePath = Command.GetPath(),
                ImageName = "icon_PI1_RibbonTabManager_16x16.png",
                LargeImageName = "icon_PI1_RibbonTabManager_32x32.png"
            };

            var btn = RevitPushButton.Create(btnData);

            try
            {
                application.ControlledApplication.DocumentOpened += new EventHandler
                     <Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(application_DocumentOpened);
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }


        #endregion

        #region private methods

        private void application_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            RibbonControl ribbon = ComponentManager.Ribbon;
            RibbonTabCollection tabs = ribbon.Tabs;

            foreach (RibbonTab tab in tabs)
            {
                Type tabType = tab.GetType();
                if (!(tabType.Name == "RvtRibbonTab") && tab.Name != "PI1")
                {
                    tab.IsVisible = Properties.Settings.Default.AllVisible;
                }
            }
        }

        #endregion
    }
}
