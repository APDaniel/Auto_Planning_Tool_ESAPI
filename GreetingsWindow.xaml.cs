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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GreetingsWindow : UserControl
    {
        
        public static ScriptContext context;
        public Patient patient = null;
        User user = null;
        StructureSet ss = null;
        public GreetingsWindow(ScriptContext context)
        {
            InitializeComponent();
            patient = context.Patient;
            user = context.CurrentUser;
            ss = context.StructureSet;
        }

        private void OPT_STR(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            OPT_STR opt_str = new OPT_STR(VMS.TPS.Script.context);
            opt_str.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = opt_str;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Create optimization structures";

        }

        private void AutoPlan(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Hide();
            AutoPlan auto_plan = new AutoPlan(context);
            auto_plan.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = auto_plan;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Auto planning tool";
            


        }

        public void EXIT(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        
        
    }
}