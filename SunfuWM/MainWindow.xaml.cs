using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SunfuWM;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
    public partial class MainWindow : Window
    {
        private WindowEnumerator _enumerator;
        private System.Windows.Threading.DispatcherTimer _timer;
        private string _currentLayout = "Tall";

        public MainWindow()
        {
            InitializeComponent();
            ApplyConfig();
            
            _enumerator = new WindowEnumerator();
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += (s, e) => RefreshWindows();
            _timer.Start();

            RefreshState();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            
            // Get current styles
            var exStyle = Interop.GetWindowLongPtr(helper.Handle, Interop.GWL_EXSTYLE);
            
            // Add WS_EX_TOOLWINDOW to hide from Alt+Tab
            var newExStyle = new IntPtr(exStyle.ToInt64() | Interop.WS_EX_TOOLWINDOW);
            
            Interop.SetWindowLongPtr(helper.Handle, Interop.GWL_EXSTYLE, newExStyle);
        }

        private void ApplyConfig()
        {
            var config = ConfigManager.Load();
            
            this.Height = config.BarHeight;
            
            try
            {
                var brush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.BarBackgroundColor));
                this.Background = brush;

                // Hack: Pass text color via Tag to be accessible in DataTemplate
                this.Tag = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.BarTextColor));
            }
            catch { /* Ignore invalid colors */ }

            // Ensure full width
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Left = 0;
            this.Top = 0;
        }

        private void OnRefreshState(object sender, RoutedEventArgs e)
        {
            RefreshState();
        }

        private void RefreshState()
        {
            RefreshWorkspaces();
            RefreshWindows();
        }

        private void RefreshWorkspaces()
        {
            // Simple fallback IWorkspaceProvider logic
            var workspaces = new List<WorkspaceInfo>();
            
            // For now, let's assume we can't reliably get the count so we show "1"
            // In Story 6 we might add real detection.
            workspaces.Add(new WorkspaceInfo { Name = "1", Id = 1, IsActive = true });
            
            WorkspaceControl.ItemsSource = workspaces;
        }

        private void RefreshWindows()
        {
            var helper = new WindowInteropHelper(this);
            var activeHWnd = Interop.GetForegroundWindow();
            var windows = _enumerator.GetOpenWindows(helper.Handle);

            foreach (var win in windows)
            {
                win.IsFocused = (win.Handle == activeHWnd);
            }

            WindowListControl.ItemsSource = windows;
        }

        private void OnToggleLayout(object sender, RoutedEventArgs e)
        {
            _currentLayout = (_currentLayout == "Tall") ? "Full" : "Tall";
            LayoutToggleButton.Content = _currentLayout;
            RetileCurrentWorkspace();
        }

        private void OnWorkspaceClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is WorkspaceInfo ws)
            {
                // Logic for switching would go here in Story 6
                // For now, we just refresh to stay robust.
                RefreshState();
            }
        }

        private void RetileCurrentWorkspace()
        {
            var helper = new WindowInteropHelper(this);
            var windows = _enumerator.GetOpenWindows(helper.Handle);
            
            // Mark focus for the engine
            var activeHWnd = Interop.GetForegroundWindow();
            foreach (var win in windows) win.IsFocused = (win.Handle == activeHWnd);

            var engine = new LayoutEngine();
            var rects = engine.GetLayoutRects(_currentLayout, windows);
            
            LayoutApplier.Apply(rects);
        }
    }