using Autodesk.Internal.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Windows;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Identity.Client;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using ExcavationMethod.Authentication;
using ExcavationMethod.Updates;
using ExcavationMethod.Domain;
using ExcavationMethod.Revit.Utilities;
using ExcavationMethod.Revit.Utilities.Messages;
using ExcavationMethod.Abstractions.Telemetry;
using ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless;
using ExcavationMethod.Revit.Application.Resources;
using ExcavationMethod.Telemetry;

namespace ExcavationMethod.Revit.Application
{
    public class AppCommand : IExternalApplication
    {
        private const string ApplicationName = "Excavation-Method-Revit";
        private static readonly string AddinDirectory = Path.GetDirectoryName(typeof(AppCommand).Assembly.Location);

        public static AppCommand? thisApp = null;
        //Tab name
        private string TabName = "AECOM Digital";

        // Panel naem (add new panel name with new private string if needed)
        #region Dummy examples
        //declare name of dummy panel
        private string DummyPanel = "Dummy";
        #endregion
        //private string PilingPanel = "Piling";

        private UIControlledApplication? _uiCtrlApp;
        private UIApplication? _uiApp;
        private string? _addinVersion;

        public List<Autodesk.Windows.RibbonItem>? RibbonItem { get; private set; }
        public Dictionary<string, ToolEntry> Tools { get; set; } = new Dictionary<string, ToolEntry>();

        public AuthenticationResult? AuthenticationResult { get; set; }
        public bool IsAuthenticated { get; set; }

        #region Dummy examples
        // declare external event and request handler of dummy button
        public ExternalEvent? DummyWpfModelessEE { get; set; }
        public DummyWpfModelessRequestHandler? DummyWpfModelessRH { get; set; }
        #endregion

        //public PileNumberingRequestHandler PileNumberingRH { get; set; }

        public List<Autodesk.Windows.RibbonItem>? RibbonItems { get; private set; }

        public UIControlledApplication? Get_uiCtrlApp()
        {
            return _uiCtrlApp;
        }

        public Result OnShutdown(UIControlledApplication app)
        { 
            if(_uiCtrlApp == null)
            {
                return Result.Failed;
            }
            _uiCtrlApp.ApplicationClosing -= ApplicationClosing;

            Debug.WriteLine($"Shut down Revit");
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication app)
        {
            thisApp = this;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;

            _addinVersion = AssemblyUtils.GetAddinVersion();
            _uiCtrlApp = app;
            _uiCtrlApp.Idling += OnIdling;

            #region telemetry and authentication 
            /*
            // Initiateds the telemetry client
            var telemetryClient = TelemetryClientFactory.CreateClient(
                TelemetryConfig.ConnectionString,
                ApplicationName,
                GetCustomTelemetryProperties);
            Usage.Client = new UsageTracker(telemetryClient);

            // Start the authentication process.
            AuthenticationResult = Task.Run(() => Authenticator.AcquireTokenAsync(
                app.MainWindowHandle)).Result;
            if (AuthenticationResult == null)
            {
                Usage.TrackEvent(AppAuthentication.Failure);
                TaskDialogUtils.Error("The user is not authenticated!\nThe AECOM Apps will not be loaded!");

                return Result.Succeeded;
            }

            telemetryClient.Context.User.AuthenticatedUserId = AuthenticationResult.Account.Username ?? string.Empty;

            Usage.TrackEvent(AppAuthentication.Success);
            Usage.TrackEvent(Addin.Started);
            */
            #endregion

            // create tab, panel and button
            try
            {
                app.CreateRibbonTab(TabName);
            }
            catch (Exception ex)
            {
                //ignore
            }

            var dummyPanel = app.GetRibbonPanels(TabName).FirstOrDefault(p => p.Name == DummyPanel) ??
                app.CreateRibbonPanel(TabName, DummyPanel);
            DummyWpfModelessCommand.CreateButton(dummyPanel);
            /*
            RibbonUtils.CreateButton(
                dummyPanel,
                "Dummy Wpf\nModeless",
                "This is modeless dialog using WPF UI",
                IconsPackX32.Placeholder);
            */
            DummyWpfModelessRequestHandler.RegisterHandler();

            // Subscribe to the click UI event.
            ComponentManager.ItemExecuted += ComponentManager_ItemExecuted;

            // Subscribe to Application closing. This is helpful during the update process.
            _uiCtrlApp.ApplicationClosing += ApplicationClosing;

            return Result.Succeeded;
        }

        private void ApplicationClosing(object sender, ApplicationClosingEventArgs e)
        {
            Debug.WriteLine($"Close Application");
        }
        private static Assembly? AssemblyResolveHandler(object _, ResolveEventArgs args)
        {
            Assembly loadedAssembly = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.FullName == args.Name);
            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            string assemblyPath = $"{Path.Combine(AddinDirectory, new AssemblyName(args.Name).Name)}.dll";
            if (!File.Exists(assemblyPath))
            {
                return null;
            }

            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private void OnIdling(object sender, IdlingEventArgs e)
        {
            if (_uiCtrlApp == null || sender is not UIApplication)
            {
                return;
            }
            _uiCtrlApp.Idling -= OnIdling;
            _uiCtrlApp.ControlledApplication.DocumentChanged += OnDocumentChanged; 
        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            Document doc = e.GetDocument();

            ICollection<ElementId> addedElementIds = e.GetAddedElementIds();
            ICollection<ElementId> deletedElementIds = e.GetDeletedElementIds();
            ICollection<ElementId> modifiedElementIds = e.GetModifiedElementIds();

            bool noElementChanged = addedElementIds.Count() == 0 && deletedElementIds.Count() == 0 && modifiedElementIds.Count() == 0;
            if (noElementChanged) 
            {
                return;
            }

            ChangedElementIdWrapper wrapper = new ChangedElementIdWrapper()
            {
                Document = doc,
                AddedElementIds = addedElementIds,
                DeletedElementIds = deletedElementIds,
                ModifiedElementIds = modifiedElementIds
            };

            WeakReferenceMessenger.Default.Send(new ChangedElementMessage(wrapper));
        }
        private void ComponentManager_ItemExecuted(object sender, RibbonItemExecutedEventArgs e)
        {
            if (Tools.Count == 0)
                InitializeToolsDictionary();
            var toolEntry = GetToolName(e.Item.Id);
            if (toolEntry != null)
            {
                Usage.TrackEvent(toolEntry.ThirdParty
                    ? ThirdParty.Started(toolEntry.ToolName)
                    : Tool.Started(toolEntry.ToolName));
            }
        }
        private void InitializeToolsDictionary()
        {
            // Automation Factory Tools
            // RibbonUtils.RibbonItemsToTools(TabName, false).ToList().ForEach(item => Tools.Add(item.Key, item.Value));

            RibbonUtils.RibbonItemsToTools("BIM Interoperability Tools", true).ToList().ForEach(item => Tools.Add(item.Key, item.Value));
            RibbonUtils.RibbonItemsToTools("Interoperability Tools", true).ToList().ForEach(item => Tools.Add(item.Key, item.Value));
        }
        private ToolEntry GetToolName(string id)
        {
            Tools.TryGetValue(id, out var toolEntry);
            return toolEntry;
        }
        #region Telemetry related functions
        /*
        private IDictionary<string, string> GetCustomTelemetryProperties()
        {
            var dictionary = new Dictionary<string, string>();

            void AddProperty(string key, string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                dictionary[key] = value;
            }

            AddProperty("ModelFileName", _uiApp?.ActiveUIDocument?.Document.Title);
            AddProperty("ProjectName", _uiApp?.ActiveUIDocument?.Document.ProjectInformation.Name);
            AddProperty("RevitVersion", _uiApp?.Application.VersionNumber);
            AddProperty("RevitSubVersion", _uiApp?.Application.SubVersionNumber);
            AddProperty("AddinVersion", _addinVersion);

            return dictionary;
        }
        */
        #endregion
    }
}
