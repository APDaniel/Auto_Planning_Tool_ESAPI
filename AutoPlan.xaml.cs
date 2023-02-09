using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace AutoPlanningTool
{
    /// <summary>
    /// Interaction logic for AutoPlan.xaml
    /// </summary>
    public partial class AutoPlan : UserControl
    {
                
        public Patient patient = null;
        User user = null;
        StructureSet ss = null;

        public AutoPlan(ScriptContext context)
        {
            InitializeComponent();
            patient = VMS.TPS.Script.context.Patient;
            user = VMS.TPS.Script.context.CurrentUser;
            ss = VMS.TPS.Script.context.StructureSet;
        }


        private void BrainAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            BrainAutoPlan brainAutoPlan = new BrainAutoPlan(VMS.TPS.Script.context);
            brainAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = brainAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Brain auto plan";

            ;
        }
        private void EsophagusAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            EsophagusAutoPlan esophagusAutoPlan = new EsophagusAutoPlan(VMS.TPS.Script.context);
            esophagusAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = esophagusAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Esophagus auto plan";

        }
        private void HeadAndNeckAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            HeadAndNeckAutoplan headAndNeckAutoPlan = new HeadAndNeckAutoplan(VMS.TPS.Script.context);
            headAndNeckAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = headAndNeckAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Head and neck auto plan";

        }
        private void LungAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            LungAutoPlan lungAutoPlan = new LungAutoPlan(VMS.TPS.Script.context);
            lungAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = lungAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Lung auto plan";

        }
        private void LymphomaAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            LymphomaAutoPlan lymphomaAutoPlan = new LymphomaAutoPlan(VMS.TPS.Script.context);
            lymphomaAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = lymphomaAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Lymphoma auto plan";

        }
        private void PelvisAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            PelvisAutoPlan pelvisAutoPlan = new PelvisAutoPlan(VMS.TPS.Script.context);
            pelvisAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = pelvisAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Pelvis auto plan";

        }
        private void SBRTabdominalAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            SBRTabdominalAutoPlan sBRTabdominalAutoPlan = new SBRTabdominalAutoPlan(VMS.TPS.Script.context);
            sBRTabdominalAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = sBRTabdominalAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "SBRT abdominal auto plan";

        }
        private void SBRTthoraxAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            SBRTthoraxAutoPlan sBRTthoraxAutoPlan = new SBRTthoraxAutoPlan(VMS.TPS.Script.context);
            sBRTthoraxAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = sBRTthoraxAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "SBRT thorax auto plan";

        }
        private void SRSbrainAutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            SRSbrainAutoPlan sRSbrainAutoPlan = new SRSbrainAutoPlan(VMS.TPS.Script.context);
            sRSbrainAutoPlan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = sRSbrainAutoPlan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "SRS brain auto plan";

        }
        private void UniversalAutoPlan(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("Dear {0},\n\n Unfortunately, this module is still under development.\n Please, let keep things going...",VMS.TPS.Script.context.CurrentUser),"Oh Dear...", MessageBoxButton.OK, MessageBoxImage.Question);
        }
        private void CancelAutoPlan(object sender, RoutedEventArgs e)
        {       
            Window.GetWindow(this).Close();

            AutoPlanningTool.GreetingsWindow usercontrol = new AutoPlanningTool.GreetingsWindow(VMS.TPS.Script.context);
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = usercontrol;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Auto planning tool";

        }
        
    }
}
