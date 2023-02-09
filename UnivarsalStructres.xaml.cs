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
    /// Interaction logic for UnivarsalStructres.xaml
    /// </summary>
    public partial class UnivarsalStructres : UserControl
    {
        
        public Patient patient = null;
        User user = null;
        StructureSet ss = null;
        StructureSet ss1 = null;
        TextBox flash = new TextBox();
        TextBox target_PTV = new TextBox();
        ComboBox id_PTV = new ComboBox();
        Button toDeleteButton = new Button();
        string new_id_PTV = "";
        public UnivarsalStructres(ScriptContext context)
        {
            InitializeComponent();
            
            //Check if the patient, structure set are loaded. If yes, then download structure set. If no, show the error message to the user and stop execution
            if (VMS.TPS.Script.context.Patient == null || VMS.TPS.Script.context.StructureSet == null)
            {
                MessageBox.Show("Please load a patient, 3D image, and structure set before running this script.", "No patient and StructureSet loaded!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Window.GetWindow(this).Close(); return; ;
            }
            patient = VMS.TPS.Script.context.Patient;
            user = VMS.TPS.Script.context.CurrentUser;
            ss = VMS.TPS.Script.context.StructureSet;

            try { foreach (StructureSet structureSet in context.Patient.StructureSets)
                
                {if (structureSet.Id.Contains("CBCT") == false){StructureSetID.Items.Add(structureSet.Id);}}} catch { }

            try { ss1 = patient.StructureSets.FirstOrDefault(x => x.Id == StructureSetID.SelectedItem.ToString()); } catch { }
            try { foreach (Structure structure in ss1.Structures) {TargetID.Items.Add(structure.Id);}}catch { }

            #region create content for ListViews
            toDeleteButton.Content = "-";
            toDeleteButton.IsEnabled = true;
            toDeleteButton.Visibility = Visibility.Visible;
            toDeleteButton.Width = 20;
            toDeleteButton.Click += ToDeleteButton_Click;
            id_PTV.SelectionChanged += Id_PTV_SelectionChanged;
            target_PTV.IsReadOnly = true;
            target_PTV.Text = TargetID.Text;
            target_PTV.TextChanged += Target_PTV_TextChanged;
            #endregion
            #region hideButtons
            //Hide create button
            CrtStrsBYtmpltButtonName.IsEnabled = false;
            //Create OPT structures hide
            AddStrPTVButtonName.Visibility = Visibility.Hidden;
            MarginPTVTBName.Visibility = Visibility.Hidden;
            MarginPTVTBValue.Visibility = Visibility.Hidden;
            //Create PRV structures hide
            AddStrPRVButtonName.Visibility = Visibility.Hidden;
            MarginPRVTBName.Visibility = Visibility.Hidden;
            MarginPRVTBValue.Visibility = Visibility.Hidden;
            //Create ring structure hide
            AddStrRingButtonName.Visibility = Visibility.Hidden;
            MarginRingInnerTBName.Visibility = Visibility.Hidden;
            MarginRingInnerTBValue.Visibility = Visibility.Hidden;
            MarginRingRadiusTBName.Visibility = Visibility.Hidden;
            MarginRingRadiusTBValue.Visibility = Visibility.Hidden;
            //Create substracted structure hide
            AddStrSubstractButtonName.Visibility = Visibility.Hidden;
            MarginSubstractTBName.Visibility = Visibility.Hidden;
            MarginSubstractTBValue.Visibility = Visibility.Hidden;
            //Create PTV_ALL structure hide
            AddStrPTVALLButtonName.Visibility = Visibility.Hidden;
            //Create PTV_OPT structure hide
            AddStrPTVOPTButtonName.Visibility = Visibility.Hidden;
            MarginPTVOPTTBName.Visibility = Visibility.Hidden;
            MarginPTVOPTTBValue.Visibility = Visibility.Hidden;
            //Hide OFF radiobuttons
            OFFStructurePTVRB.Visibility = Visibility.Hidden;
            OFFStructurePRVRB.Visibility = Visibility.Hidden;
            OFFStructurePRVRB.Visibility = Visibility.Hidden;
            OFFStructureRingRB.Visibility = Visibility.Hidden;
            OFFStructureSubstractRB.Visibility = Visibility.Hidden;
            OFFStructurePTVALLRB.Visibility = Visibility.Hidden;
            OFFStructurePTVOPTRB.Visibility = Visibility.Hidden;
            #endregion
        }

        private void Target_PTV_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { target_PTV.Text = TargetID.Text; } catch { }
        }

        private void Id_PTV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            id_PTV.Items.Clear();
            try { ss1 = patient.StructureSets.FirstOrDefault(x => x.Id == StructureSetID.SelectedItem.ToString()); } catch { }
            try { foreach (Structure structure in ss1.Structures) { TargetID.Items.Add(structure.Id); } } catch { }
            try { new_id_PTV = string.Format("{0}-{1}", (id_PTV.SelectedItem.ToString()).Substring(0, 9), TargetID.Text.Substring(0, 2)); } catch { }
        }

        private void ToDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            StructurePTV.Items.Remove(StructurePRV.SelectedItem);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();

            AutoPlanningTool.OPT_STR usercontrol = new AutoPlanningTool.OPT_STR(VMS.TPS.Script.context);
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = usercontrol;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Optimization structures";
        }

        private void CrtStrBYtmplt_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StructurePTVALL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TargetID_Changed(object sender, SelectionChangedEventArgs e)
        {
            try { target_PTV.Text = TargetID.Text; } catch { }
        }

        private void StructureSetID_Changed(object sender, SelectionChangedEventArgs e)
        {
            TargetID.Items.Clear();
            
            try { ss1 = patient.StructureSets.FirstOrDefault(x => x.Id == StructureSetID.SelectedItem.ToString()); } catch { }
            try { foreach (Structure structure in ss1.Structures) { TargetID.Items.Add(structure.Id); } } catch { }
        }

        private void StructurePTV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StructurePRV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StructureRing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StructureSubstract_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StructurePTV_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OFFStructurePTV_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructurePTVRB.Visibility = Visibility.Hidden;
            AddStrPTVButtonName.Visibility = Visibility.Hidden;
            MarginPTVTBName.Visibility = Visibility.Hidden;
            MarginPTVTBValue.Visibility = Visibility.Hidden;
            MarginPTVTBValue.Text = "";
        }

        private void StructurePRV_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OFFStructurePRV_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructurePRVRB.Visibility = Visibility.Hidden;
            AddStrPRVButtonName.Visibility = Visibility.Hidden;
            MarginPRVTBName.Visibility = Visibility.Hidden;
            MarginPRVTBValue.Visibility = Visibility.Hidden;
            MarginPRVTBValue.Text = "";

        }

        private void StructureRing_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OFFStructureRing_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructureRingRB.Visibility = Visibility.Hidden;
            AddStrRingButtonName.Visibility = Visibility.Hidden;
            MarginRingInnerTBName.Visibility = Visibility.Hidden;
            MarginRingInnerTBValue.Visibility = Visibility.Hidden;
            MarginRingRadiusTBName.Visibility = Visibility.Hidden;
            MarginRingRadiusTBValue.Visibility = Visibility.Hidden;
            MarginRingInnerTBValue.Text = "";
            MarginRingInnerTBValue.Text = "";
        }   

        private void OFFStructureSubstarct_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void StructurePTVALL_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OFFStructurePTVALL_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructurePTVALLRB.Visibility = Visibility.Hidden;
            AddStrPTVALLButtonName.Visibility = Visibility.Hidden;
        }

        private void ONStructurePTV_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructurePTVRB.Visibility = Visibility.Visible;
            AddStrPTVButtonName.Visibility = Visibility.Visible;
            MarginPTVTBName.Visibility = Visibility.Visible;
            MarginPTVTBValue.Visibility = Visibility.Visible;
            MarginPTVTBValue.Text = "";

        }

        private void ONStructurePRV_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructurePRVRB.Visibility = Visibility.Visible;
            AddStrPRVButtonName.Visibility = Visibility.Visible;
            MarginPRVTBName.Visibility = Visibility.Visible;
            MarginPRVTBValue.Visibility = Visibility.Visible;
            MarginPRVTBValue.Text = "";
        }

        private void ONStructureRing_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructureRingRB.Visibility = Visibility.Visible;
            AddStrRingButtonName.Visibility = Visibility.Visible;
            MarginRingInnerTBName.Visibility = Visibility.Visible;
            MarginRingInnerTBValue.Visibility = Visibility.Visible;
            MarginRingRadiusTBName.Visibility = Visibility.Visible;
            MarginRingRadiusTBValue.Visibility = Visibility.Visible;
            MarginRingInnerTBValue.Text = "";
            MarginRingRadiusTBValue.Text = "";
        }

        private void ONStructureSubstract_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructureSubstractRB.Visibility = Visibility.Visible;
            AddStrSubstractButtonName.Visibility = Visibility.Visible;
            MarginSubstractTBName.Visibility = Visibility.Visible;
            MarginSubstractTBValue.Visibility = Visibility.Visible;
            MarginSubstractTBValue.Text = "";
        }

        private void ONStructurePTVALL_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructurePTVALLRB.Visibility = Visibility.Visible;
            AddStrPTVALLButtonName.Visibility = Visibility.Visible;
        }

        private void ONStructurePTVOPT_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructurePTVOPTRB.Visibility = Visibility.Visible;
            AddStrPTVOPTButtonName.Visibility = Visibility.Visible;
            MarginPTVOPTTBName.Visibility = Visibility.Visible;
            MarginPTVOPTTBValue.Visibility = Visibility.Visible;
            MarginPTVOPTTBValue.Text = "";
        }

        private void OFFStructureSubstract_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructureSubstractRB.Visibility = Visibility.Hidden;
            AddStrSubstractButtonName.Visibility = Visibility.Hidden;
            MarginSubstractTBName.Visibility = Visibility.Hidden;
            MarginSubstractTBValue.Visibility = Visibility.Hidden;
            MarginSubstractTBValue.Text = "";
        }

        private void OFFStructurePTVOPT_Checked(object sender, RoutedEventArgs e)
        {
            OFFStructurePTVOPTRB.Visibility = Visibility.Hidden;
            AddStrPTVOPTButtonName.Visibility = Visibility.Hidden;
            MarginPTVOPTTBName.Visibility = Visibility.Hidden;
            MarginPTVOPTTBValue.Visibility = Visibility.Hidden;
            MarginPTVOPTTBValue.Text = "";
        }

        

        private void AddStrPRVButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddStrRingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddSubstractButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddPTVALLButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddPTVOPTButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddStrPTVButton_Click(object sender, RoutedEventArgs e)
        {
            
            StructurePTV.Items.Add(new Items_PTV { ToDelete = toDeleteButton, Id_PTV=id_PTV, Flash=flash,From="from",Target_PTV=target_PTV,NEW_PTV=" " }) ;
        
        }

        private void MarginPRVTBValueChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginRingTBValueChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginRadiusTBValueChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginSubstractTBValueChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginPTVOPTTBValueChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginPTVTBNameChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginTBNameChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginInnerRingTBNameChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginRadiusRingTBNameChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginSubstractTBNameChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MarginPTVOPTTBNameChanged(object sender, TextChangedEventArgs e)
        {

        }

        public class Items_PTV
        {
            public Button ToDelete { get; set; }
            public ComboBox Id_PTV { get; set; }
            public TextBox Flash { get; set; }
            public string From { get; set; }
            public TextBox Target_PTV { get; set; }
            public string NEW_PTV { get; set; }

        }
    }
}
