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
    /// Interaction logic for SRSbrain.xaml
    /// </summary>
    public partial class SRSbrainAutoPlan : UserControl
    {


        Tuple<int, DoseValue> prescription = null;
        string linac = "";
        string calculation_model = "PO_15.6.06";
        public Patient patient = VMS.TPS.Script.context.Patient;
        User user = VMS.TPS.Script.context.CurrentUser;
        StructureSet ss = VMS.TPS.Script.context.StructureSet;
        double DPF = 2.0;
        double DPFescalation;
        int NOF = 1;

        private void LinacIDChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public SRSbrainAutoPlan(ScriptContext context)
        {
            InitializeComponent();
            BED_escalation.Visibility = Visibility.Hidden;
            BED_escalation_value.Visibility = Visibility.Hidden;
            EQD2_escalation.Visibility = Visibility.Hidden;
            EQD2_escalation_value.Visibility = Visibility.Hidden;
            TotalDoseEscalation.Visibility = Visibility.Hidden;

            NoEscalation.IsChecked = true;
            Button_CrtAndCalPlan.IsEnabled = false;
            DataContext = this;
            Button_CrtAndCalPlan.IsEnabled = false;
            //download available structuresets for the opened patient
            foreach (StructureSet structureSet in context.Patient.StructureSets)
            {
                if (structureSet.Id.Contains("CBCT") == false)
                {
                    StructureSetID.Items.Add(structureSet.Id);
                }
            }
        }

        private void DosePerFx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(dosePerFx.Text, out double dose_perFx) && int.TryParse(numberOfFractions.Text, out int numFractions) && (dose_perFx > 0) && (numFractions > 0))
            {
                prescription = Tuple.Create(numFractions, new DoseValue(dose_perFx, DoseValue.DoseUnit.cGy));
                DPF = dose_perFx;
                NOF = numFractions;
                double TD = DPF * NOF;
                DoseToBeDelivered.Text = TD.ToString();
                Button_CrtAndCalPlan.IsEnabled = true;
                linac = LinacID.Text;

                if (double.TryParse(alpha_beta.Text, out double aplha_beta_double1))
                {
                    double bED = (TD * (1 + (DPF / aplha_beta_double1)));
                    double eQD2 = (TD * ((DPF + aplha_beta_double1) / (2 + aplha_beta_double1)));
                    EQD2.Text = (Math.Round(eQD2, 2)).ToString();
                    BED.Text = (Math.Round(bED, 2)).ToString();
                }

                else { EQD2.Text = ""; BED.Text = ""; }


            }

            else
            {
                Button_CrtAndCalPlan.IsEnabled = false;
                return;
            }

            if (Escalation.IsChecked == true)
            {
                if (double.TryParse(dosePerFxEscalation.Text, out double dose_perFxEscalation) && int.TryParse(numberOfFractions.Text, out numFractions) && (dose_perFxEscalation > 0) && (numFractions > 0))
                {
                    BED_escalation.Visibility = Visibility.Visible;
                    BED_escalation_value.Visibility = Visibility.Visible;
                    EQD2_escalation.Visibility = Visibility.Visible;
                    EQD2_escalation_value.Visibility = Visibility.Visible;
                    TotalDoseEscalation.Visibility = Visibility.Visible;
                    prescription = Tuple.Create(numFractions, new DoseValue(dose_perFxEscalation, DoseValue.DoseUnit.cGy));
                    DPFescalation = dose_perFxEscalation;
                    NOF = numFractions;
                    double TDescalation = DPFescalation * NOF;
                    TotalDoseEscalation.Text = TDescalation.ToString();
                    Button_CrtAndCalPlan.IsEnabled = true;

                }
                else
                {
                    Button_CrtAndCalPlan.IsEnabled = false;
                    return;
                }
            }

        }
        private void DosePerFxEscalation_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (Escalation.IsChecked == true)
            {
                if (double.TryParse(dosePerFxEscalation.Text, out double dose_perFxEscalation) && int.TryParse(numberOfFractions.Text, out int numFractions) && (dose_perFxEscalation > 0) && (numFractions > 0))
                {
                    prescription = Tuple.Create(numFractions, new DoseValue(dose_perFxEscalation, DoseValue.DoseUnit.cGy));
                    DPFescalation = dose_perFxEscalation;
                    NOF = numFractions;
                    double TDescalation = DPFescalation * NOF;
                    TotalDoseEscalation.Text = TDescalation.ToString();
                    Button_CrtAndCalPlan.IsEnabled = true;

                }
                else
                {
                    Button_CrtAndCalPlan.IsEnabled = false;
                    return;
                }
            }
            if (double.TryParse(dosePerFxEscalation.Text, out double DPFe) && double.TryParse(alpha_beta.Text, out double aplha_beta_double))
            {
                double TDe = DPFe * NOF;
                double bEDe = (TDe * (1 + (DPFe / aplha_beta_double)));
                double eQD2e = (TDe * ((DPFe + aplha_beta_double) / (2 + aplha_beta_double)));
                EQD2_escalation_value.Text = (Math.Round(eQD2e, 2)).ToString();
                BED_escalation_value.Text = (Math.Round(bEDe, 2)).ToString();
            }
            else { EQD2_escalation_value.Text = ""; BED_escalation_value.Text = ""; }


        }
        private void CrtAndCalPlan(object sender, RoutedEventArgs e)
        {
           
                if (NoEscalation.IsChecked == true)
                {
                    GreenBar.Value = 0;
                    GreenBarListBox.Items.Clear();
                    TextBox text = new TextBox();
                    text.Text = "-------Started-------                                                        ";
                    GreenBarListBox.Items.Add(text.Text);

                     


                    //Apply selected in combobox StructureSet for script execution
                    ss = VMS.TPS.Script.context.Patient.StructureSets.FirstOrDefault(x => x.Id == StructureSetID.Text); 
                    // Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "SRS_Brain_NoEscalation";
                    //ID of the course
                    const string COURSE_ID = "Treatment";
                    //ID of the plan
                    string PLAN_ID = string.Format("SRS_{0}Gy", (Math.Round((NOF * DPF), 0)).ToString());


                    //IDs of fields
                    const string MVCBCT_ID = "MVCBCT";

                    const string BEAM_D_1_ID = "SRS179-181/20";
                    const string BEAM_D_2_ID = "SRS181-179/340";
                    const string BEAM_D_3_ID = "SRS179-181/90";



                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_ID = "PTV";
                    const string PTV_1_ID = "PTV_1";
                    const string PTV_2_ID = "PTV_2";
                    const string PTV_3_ID = "PTV_3";
                    const string OPT_PTV_ID = "PTV_OPT";
                    const string OPT_PTV_1_ID = "PTV_OPT_1";
                    const string OPT_PTV_2_ID = "PTV_OPT_2";
                    const string OPT_PTV_3_ID = "PTV_OPT_3";
                    const string GTV_ID = "GTV";
                    const string GTV_1_ID = "GTV_1";
                    const string GTV_2_ID = "GTV_2";
                    const string GTV_3_ID = "GTV_3";
                    const string OPT_GTV_ID = "GTV_OPT";
                    const string OPT_GTV_1_ID = "GTV_OPT_1";
                    const string OPT_GTV_2_ID = "GTV_OPT_2";
                    const string OPT_GTV_3_ID = "GTV_OPT_3";
                    const string PTV_GTV_ID = "PTV-GTV";
                    const string PTV_GTV_1_ID = "PTV-GTV_1";
                    const string PTV_GTV_2_ID = "PTV-GTV_2";
                    const string PTV_GTV_3_ID = "PTV-GTV_3";
                    const string PTV_SHELL_ID = "PTV_shell";


                    //IDs of critical structures
                    const string BODY_ID = "BODY";
                    const string SKIN_ID = "Skin_bySCRPT";
                    const string OPTIC_PATHWAY_ID = "Optic pathway";
                    const string OPTIC_PATHWAY_PRV_ID = "Optic path_PRV";
                    const string COCHLEA_ID = "Cochlea";
                    const string COCHLEA_PRV_ID = "Cochlea_PRV";
                    const string BRAINSTEM_ID = "Brainstem";
                    const string BRAINSTEM_PRV_ID = "Brainstem_PRV";
                    const string BRAINSTEM_OPT_ID = "Brainstem_PTV";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpinalCo_PRV";
                    const string SPINAL_CORD_OPT = "SpinalCo_PTV";
                    const string SPINAL_CORD_SUBVOL_ID = "SpinalCrd Subvol";
                    const string MEDULLA_ID = "Medulla";
                    const string ESOPHAGUS_ID = "Esophagus";
                    const string ESOPHAGUS_OPT_ID = "Esophagus_PTV";
                    const string BRACHIAL_PLEXUS_ID = "Brachial plexus";
                    const string BRACHIAL_PLEXUS_PRV_ID = "Brachial p_PRV";
                    const string BRACHIAL_PLEXUS_PTV_ID = "Brachial p_PTV";
                    const string TRACHEA_ID = "Trachea";
                    const string TRACHEA_OPT_ID = "Trachea_PTV";



                    //Show greetings window
                    string Greeting = string.Format("Greetings {0}! Please, ensure that the structures 'PTV', 'GTVp', 'PTV-GTV' exist in the currest structure set.\n If you would like to calculate up to 4 targets from one isocenter, name these targets as: 'PTV', 'PTV_1', 'PTV_2', 'PTV_3', 'GTV', 'GTV_1', 'GTV_2', 'GTV_3'\n Script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru'ru)", user);
                    MessageBox.Show(Greeting);


                    //Turn on the stopwatch
                    var stopwatch = DateTime.Now;

                    GreenBar.Value = 5;
                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                    text.Text = "Defining constants...";
                    GreenBarListBox.Items.Add(text.Text);

                     


                    //Begin modifications
                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //Add course
                    Course course;
                    if (patient.Courses.Where(x => x.Id == COURSE_ID).Any()) course = patient.Courses.Where(x => x.Id == COURSE_ID).Single();
                    else
                    {
                        course = patient.AddCourse();
                        course.Id = COURSE_ID;
                    }

                    GreenBar.Value = 10;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Treatment cource creation...";
                    GreenBarListBox.Items.Add(text.Text);
                     



                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution                    

                    List<string> NOT_FOUND_list = new List<string>();

                    //find PTVs. Show an error notification if no optimization PTV will be found
                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found! Please, ensure that PTV is presented in the structure set", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Window.GetWindow(this).Close(); return;
                    }

                    Structure ptv_1 = ss.Structures.FirstOrDefault(x => x.Id == PTV_1_ID);
                    if (ptv_1 == null)
                    {
                        NOT_FOUND_list.Add(PTV_1_ID);
                        try { ptv_1 = ss.AddStructure("Avoidance", PTV_1_ID); } catch { }
                    }

                    Structure ptv_2 = ss.Structures.FirstOrDefault(x => x.Id == PTV_2_ID);
                    if (ptv_2 == null)
                    {
                        NOT_FOUND_list.Add(PTV_2_ID);
                        try { ptv_2 = ss.AddStructure("Avoidance", PTV_2_ID); } catch { }
                    }

                    Structure ptv_3 = ss.Structures.FirstOrDefault(x => x.Id == PTV_3_ID);
                    if (ptv_3 == null)
                    {
                        NOT_FOUND_list.Add(PTV_3_ID);
                        try { ptv_3 = ss.AddStructure("Avoidance", PTV_3_ID); } catch { }
                    }

                    Structure gtv = ss.Structures.FirstOrDefault(x => x.Id == GTV_ID);
                    if (gtv == null)
                    {
                        NOT_FOUND_list.Add(GTV_ID);
                    }

                    Structure gtv_1 = ss.Structures.FirstOrDefault(x => x.Id == GTV_1_ID);
                    if (gtv_1 == null)
                    {
                        NOT_FOUND_list.Add(GTV_1_ID);
                        try { gtv_1 = ss.AddStructure("Avoidance", GTV_1_ID); } catch { }
                    }

                    Structure gtv_2 = ss.Structures.FirstOrDefault(x => x.Id == GTV_2_ID);
                    if (gtv_2 == null)
                    {
                        NOT_FOUND_list.Add(GTV_2_ID);
                        try { gtv_2 = ss.AddStructure("Avoidance", GTV_2_ID); } catch { }
                    }

                    Structure gtv_3 = ss.Structures.FirstOrDefault(x => x.Id == GTV_3_ID);
                    if (gtv_3 == null)
                    {
                        NOT_FOUND_list.Add(GTV_3_ID);
                        try { gtv_3 = ss.AddStructure("Avoidance", GTV_3_ID); } catch { }
                    }

                    Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                    if (ptv_gtv == null)
                    {
                        NOT_FOUND_list.Add(PTV_GTV_ID);
                        try { ptv_gtv = ss.AddStructure("Avoidance", PTV_GTV_ID); } catch { }
                    }

                    Structure ptv_gtv_1 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_1_ID);
                    if (ptv_gtv_1 == null)
                    {
                        NOT_FOUND_list.Add(PTV_GTV_1_ID);
                        try { ptv_gtv_1 = ss.AddStructure("Avoidance", PTV_GTV_1_ID); } catch { }
                    }

                    Structure ptv_gtv_2 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_2_ID);
                    if (ptv_gtv_2 == null)
                    {
                        NOT_FOUND_list.Add(PTV_GTV_2_ID);
                        try { ptv_gtv_2 = ss.AddStructure("Avoidance", PTV_GTV_2_ID); } catch { }
                    }

                    Structure ptv_gtv_3 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_3_ID);
                    if (ptv_gtv_3 == null)
                    {
                        NOT_FOUND_list.Add(PTV_GTV_3_ID);
                        try { ptv_gtv_3 = ss.AddStructure("Avoidance", PTV_GTV_3_ID); } catch { }
                    }


                    //Find OARs and optimization structures

                    //find Skin
                    Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                    if (skin == null)
                    {
                        NOT_FOUND_list.Add(SKIN_ID);
                        try { skin = ss.AddStructure("Avoidance", SKIN_ID); } catch { }
                    }

                    //find Optic pathway
                    Structure optic_pathway = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_PATHWAY_ID);
                    if (optic_pathway == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_PATHWAY_ID);
                        try { optic_pathway = ss.AddStructure("Avoidance", OPTIC_PATHWAY_ID); } catch { }
                    }

                    //find Cochlea
                    Structure cochlea = ss.Structures.FirstOrDefault(x => x.Id == COCHLEA_ID);
                    if (cochlea == null)
                    {
                        NOT_FOUND_list.Add(COCHLEA_ID);
                        try { cochlea = ss.AddStructure("Avoidance", COCHLEA_ID); } catch { }
                    }

                    //find BrainStem
                    Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                    if (brainstem == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_ID);
                        try { brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { }
                    }

                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Medulla
                    Structure medulla = ss.Structures.FirstOrDefault(x => x.Id == MEDULLA_ID);
                    if (medulla == null)
                    {
                        NOT_FOUND_list.Add(MEDULLA_ID);
                        try { medulla = ss.AddStructure("Avoidance", MEDULLA_ID); } catch { }
                    }

                    //find SpinalCord sub volume
                    Structure spinalcord_subvol = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_SUBVOL_ID);
                    if (spinalcord_subvol == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_SUBVOL_ID);
                        try { spinalcord_subvol = ss.AddStructure("Avoidance", SPINAL_CORD_SUBVOL_ID); } catch { }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find Brachial plexus
                    Structure brachial_plexus = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_ID);
                    if (brachial_plexus == null)
                    {
                        NOT_FOUND_list.Add(BRACHIAL_PLEXUS_ID);
                        try { brachial_plexus = ss.AddStructure("Avoidance", BRACHIAL_PLEXUS_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return; ;
                    }

                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);

                    if (ptv_opt == null)
                    {
                        NOT_FOUND_list.Add(OPT_PTV_ID);
                        try { ptv_opt = ss.AddStructure("Avoidance", OPT_PTV_ID); } catch { }
                    }

                    Structure ptv_opt_1 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_1_ID);

                    if (ptv_opt_1 == null)
                    {
                        NOT_FOUND_list.Add(OPT_PTV_1_ID);
                        try { ptv_opt_1 = ss.AddStructure("Avoidance", OPT_PTV_1_ID); } catch { }
                    }

                    Structure ptv_opt_2 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_2_ID);

                    if (ptv_opt_2 == null)
                    {
                        NOT_FOUND_list.Add(OPT_PTV_2_ID);
                        try { ptv_opt_2 = ss.AddStructure("Avoidance", OPT_PTV_2_ID); } catch { }
                    }

                    Structure ptv_opt_3 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_3_ID);

                    if (ptv_opt_3 == null)
                    {
                        NOT_FOUND_list.Add(OPT_PTV_3_ID);
                        try { ptv_opt_3 = ss.AddStructure("Avoidance", OPT_PTV_3_ID); } catch { }
                    }


                    Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_ID);

                    if (gtv_opt == null)
                    {
                        NOT_FOUND_list.Add(OPT_GTV_ID);
                        try { gtv_opt = ss.AddStructure("Avoidance", OPT_GTV_ID); } catch { }
                    }

                    Structure gtv_opt_1 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_1_ID);

                    if (gtv_opt_1 == null)
                    {
                        NOT_FOUND_list.Add(OPT_GTV_1_ID);
                        try { gtv_opt_1 = ss.AddStructure("Avoidance", OPT_GTV_1_ID); } catch { }
                    }

                    Structure gtv_opt_2 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_2_ID);

                    if (gtv_opt_2 == null)
                    {
                        NOT_FOUND_list.Add(OPT_GTV_2_ID);
                        try { gtv_opt_2 = ss.AddStructure("Avoidance", OPT_GTV_2_ID); } catch { }
                    }

                    Structure gtv_opt_3 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_3_ID);

                    if (gtv_opt_3 == null)
                    {
                        NOT_FOUND_list.Add(OPT_GTV_3_ID);
                        try { gtv_opt_3 = ss.AddStructure("Avoidance", OPT_GTV_3_ID); } catch { }
                    }

                    Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);

                    if (ptv_shell == null)
                    {
                        NOT_FOUND_list.Add(PTV_SHELL_ID);
                        try { ptv_shell = ss.AddStructure("Avoidance", PTV_SHELL_ID); } catch { }
                    }



                    Structure optic_pathway_prv = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_PATHWAY_PRV_ID);

                    if (optic_pathway_prv == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_PATHWAY_PRV_ID);
                        try { optic_pathway_prv = ss.AddStructure("Avoidance", OPTIC_PATHWAY_PRV_ID); } catch { }
                    }

                    Structure cochlea_prv = ss.Structures.FirstOrDefault(x => x.Id == COCHLEA_PRV_ID);
                    if (cochlea_prv == null)
                    {
                        NOT_FOUND_list.Add(COCHLEA_PRV_ID);
                        try { cochlea_prv = ss.AddStructure("Avoidance", COCHLEA_PRV_ID); } catch { }
                    }

                    Structure brainstem_prv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV_ID);
                    if (brainstem_prv == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_PRV_ID);
                        try { brainstem_prv = ss.AddStructure("Avoidance", BRAINSTEM_PRV_ID); } catch { }
                    }

                    Structure brainstem_ptv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_OPT_ID);
                    if (brainstem_ptv == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_OPT_ID);
                        try { brainstem_ptv = ss.AddStructure("Avoidance", BRAINSTEM_OPT_ID); } catch { }
                    }

                    Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spinal_cord_prv == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_PRV_ID);
                        try { spinal_cord_prv = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); } catch { }
                    }

                    Structure spinal_cord_ptv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_OPT);
                    if (spinal_cord_ptv == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_OPT);
                        try { spinal_cord_ptv = ss.AddStructure("Avoidance", SPINAL_CORD_OPT); } catch { }
                    }

                    Structure brachial_plexus_prv = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_PRV_ID);
                    if (brachial_plexus_prv == null)
                    {
                        NOT_FOUND_list.Add(BRACHIAL_PLEXUS_PRV_ID);
                        try { brachial_plexus_prv = ss.AddStructure("Avoidance", BRACHIAL_PLEXUS_PRV_ID); } catch { }
                    }

                    Structure brachial_plexus_ptv = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_PTV_ID);
                    if (brachial_plexus_ptv == null)
                    {
                        NOT_FOUND_list.Add(BRACHIAL_PLEXUS_PTV_ID);
                        try { brachial_plexus_ptv = ss.AddStructure("Avoidance", BRACHIAL_PLEXUS_PTV_ID); } catch { }
                    }


                    Structure esophagus_ptv = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_OPT_ID);
                    if (esophagus_ptv == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_OPT_ID);
                        try { esophagus_ptv = ss.AddStructure("Avoidance", ESOPHAGUS_OPT_ID); } catch { }
                    }

                    Structure trachea_ptv = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_OPT_ID);
                    if (trachea_ptv == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_OPT_ID);
                        try { trachea_ptv = ss.AddStructure("Avoidance", TRACHEA_OPT_ID); } catch { }
                    }

                    //show message with list of not found structures
                    string[] NOT_FOUND = NOT_FOUND_list.ToArray();
                    string res = string.Join("\r\n", NOT_FOUND);
                    var array_length = NOT_FOUND.Length;
                    if (array_length > 0)
                    {
                        MessageBox.Show(string.Format("Structures were not found:\n'{0}'", res), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                    //put isocenter to the center of PTV
                    VVector isocenter = new VVector(Math.Round(ptv.CenterPoint.x / 10.0f) * 10.0f, Math.Round(ptv.CenterPoint.y / 10.0f) * 10.0f, Math.Round(ptv.CenterPoint.z / 10.0f) * 10.0f);

                    //Add plan 

                    string report_plan_creation = string.Format("");
                    ExternalPlanSetup plan = course.ExternalPlanSetups.FirstOrDefault(x => x.Id == PLAN_ID);

                    if (plan != null)
                    {
                        plan = course.ExternalPlanSetups.FirstOrDefault(x => x.Id == PLAN_ID);
                        report_plan_creation = string.Format("Course, plan, imaging setup and beams are exist. Continuing with the optimization and dose calculation", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (plan == null)
                    {
                        plan = course.AddExternalPlanSetup(ss);
                        plan.Id = PLAN_ID;
                        report_plan_creation = string.Format("Course and plan creation completed successful. Please, ensure that calibration curve is chosen correctly, review the dose matrix, insert the imaging setup, attach the plan to a clinical protocol and adjust the reference point. If you would like to proceed with the optimization and dose volume calculation, please launch this script again.", MessageBoxButton.OK, MessageBoxImage.Information);
                    }


                    GreenBar.Value = 15;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Treatment plan creation...";
                    GreenBarListBox.Items.Add(text.Text);
                     



                    //Define prescription
                    plan.SetPrescription(NOF, new DoseValue(DPF, DoseValue.DoseUnit.Gy), 1.0);

                    GreenBar.Value = 20;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Prescribed dofe calculation...";
                    GreenBarListBox.Items.Add(text.Text);
                     


                    //Define Machine parameters
                    ExternalBeamMachineParameters MachineParameters =
                    new ExternalBeamMachineParameters(linac, "6X", 800, "SRS ARC", "FFF");


                    //Check if the beams already exist. If no, create beams

                    //Setup by default

                    Beam beam1_D = plan.Beams.FirstOrDefault(x => x.Id == BEAM_D_1_ID);
                    if (beam1_D == null)
                    {
                        beam1_D = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 179, 181, GantryDirection.CounterClockwise, 0, isocenter);
                        beam1_D.Id = BEAM_D_1_ID;
                    }

                    Beam beam2_D = plan.Beams.FirstOrDefault(x => x.Id == BEAM_D_2_ID);
                    if (beam2_D == null)
                    {
                        beam2_D = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 181, 179, GantryDirection.Clockwise, 0, isocenter);
                        beam2_D.Id = BEAM_D_2_ID;
                    }
                    /*
                    Beam beam3_D = plan.Beams.FirstOrDefault(x=>x.Id == BEAM_D_3_ID);
                    if (beam3_D == null)
                    {
                        beam3_D = plan.AddArcBeam(MachineParameters, new VRect<double>(280,280,280,280), 90, 179, 181, GantryDirection.CounterClockwise, 0, isocenter);
                        beam3_D.Id = BEAM_D_3_ID;
                    }
                    */
                    //Fit collimator to structure. For Halcyon machines the next 5 strings are not necessary
                    try
                    {
                        bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                        beam1_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                        beam2_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                        //beam3_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                        FitToStructureMargins margins = new FitToStructureMargins(0);
                    }
                    catch { MessageBox.Show(string.Format("Unfortunately, beams could not be placed for some reason..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    GreenBar.Value = 25;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Placing beams in plan...";
                    GreenBarListBox.Items.Add(text.Text);
                     




                    //Show report message on successful course and plan creation
                    MessageBox.Show(report_plan_creation);

                    //Define optimizator model
                    plan.SetCalculationModel(CalculationType.PhotonVMATOptimization, calculation_model);


                    //Check if the MVCBCT beam exist in the plan. If no, show an error notification and stop execution
                    Beam mvcbct = plan.Beams.FirstOrDefault(x => x.Id == MVCBCT_ID);
                    if (mvcbct == null)
                    {

                        MessageBox.Show(string.Format("Imaging setup was not found! Please, ensure that a beam with ID: '{0}' exist", MVCBCT_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }


                    //Set Normal tissie objective (NTO)
                    plan.OptimizationSetup.AddNormalTissueObjective(120, 0, 100, 35, 0.18);


                    //Set optimization adjustments for PTVs
                    GreenBar.Value = 30;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Defining objectives...";
                    GreenBarListBox.Items.Add(text.Text);
                     


                    double target_lower = Math.Round((DPF * NOF), 2);
                    double target_upper = Math.Round(((DPF * NOF) * 1.05), 2);
                //Set optimization adjustments for PTVs

                if (RapidPlan.IsChecked == true) 
                {
                    //Choose estimation model ID
                    string DVHestimationModelId = "SRS Brain";


                    //Choose target dose level. Due to the reason that no escalation is checked, we will put the same dose target leves for PTV and GTV
                    Dictionary<string, DoseValue> targetDoseLevels = new Dictionary<string, DoseValue>();

                    try { targetDoseLevels.Add("PTV_OPT", plan.TotalDose); } catch { }
                    /*try{targetDoseLevels.Add("PTV_OPT_1", plan.TotalDose); } catch { }
                    try{targetDoseLevels.Add("PTV_OPT_2", plan.TotalDose); } catch { }
                    try{targetDoseLevels.Add("PTV_OPT_3", plan.TotalDose); } catch { }*/
                                        
                    try{targetDoseLevels.Add("GTV_OPT", plan.TotalDose); } catch { }
                    /*try{targetDoseLevels.Add("GTV_OPT_1", plan.TotalDose); } catch { }
                    try{targetDoseLevels.Add("GTV_OPT_2", plan.TotalDose); } catch { }
                    try{targetDoseLevels.Add("GTV_OPT_3", plan.TotalDose); } catch { }*/

                    //Match structures from the structure set to structurel listed in DVHestimationModel 
                    Dictionary<string, string> structureMatches = new Dictionary<string, string>();
                   
                    try { structureMatches.Add(OPT_PTV_ID, "PTV_OPT"); } catch { }
                    try { structureMatches.Add(OPT_GTV_ID, "GTV_OPT"); } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_1_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "PTV_OPT_1").Id, "PTV_OPT_1"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_2_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "PTV_OPT_2").Id, "PTV_OPT_2"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_3_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "PTV_OPT_3").Id, "PTV_OPT_3"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_1_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "GTV_OPT_1").Id, "GTV_OPT_1"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_2_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "GTV_OPT_2").Id, "GTV_OPT_2"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_3_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "GTV_OPT_3").Id, "GTV_OPT_3"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BODY")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BODY").Id, "BODY"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem").Id, "Brainstem"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem_PRV").Id, "Brainstem_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem-PTV").Id, "Brainstem-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm").Id, "Chiasm"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm_PRV").Id, "Chiasm_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm-PTV").Id, "Chiasm-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_L").Id, "Cochlea_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_R").Id, "Cochlea_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_L").Id, "OpticNerve_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_R").Id, "OpticNerve_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Skin")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Skin").Id, "Skin"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord").Id, "SpinalCord"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV").Id, "SpnlCrd_PRV"); } } catch { }


                    plan.CalculateDVHEstimates(DVHestimationModelId, targetDoseLevels, structureMatches);
                }
                else
                {

                    //PTV
                    if (ptv != null & (ptv.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_lower = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);


                            if (ptv_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for PTV could not be defined..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }








                    if (ptv_opt != null & (ptv_opt.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_opt_lower = plan.OptimizationSetup.AddPointObjective(ptv_opt, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);


                            if (ptv_opt_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_opt, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }





                    if (gtv != null & (gtv.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_lower = plan.OptimizationSetup.AddPointObjective(gtv, OptimizationObjectiveOperator.Lower, new DoseValue((target_lower + 0.5), "Gy"), 99.9, 100);


                            if (gtv_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv, OptimizationObjectiveOperator.Lower, new DoseValue((target_lower + 0.5), "Gy"), 99.9, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }





                    if (gtv_opt != null & (gtv_opt.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_opt_lower = plan.OptimizationSetup.AddPointObjective(gtv_opt, OptimizationObjectiveOperator.Lower, new DoseValue((target_lower + 0.5), "Gy"), 99.9, 200);


                            if (gtv_opt_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_opt, OptimizationObjectiveOperator.Lower, new DoseValue((target_lower + 0.5), "Gy"), 99.9, 200);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }





                    if (ptv_gtv != null & (ptv_gtv.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_gtv_upper = plan.OptimizationSetup.AddPointObjective(ptv_gtv, OptimizationObjectiveOperator.Upper, new DoseValue((target_lower + 2), "Gy"), 0, 100);


                            if (ptv_gtv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_gtv, OptimizationObjectiveOperator.Upper, new DoseValue((target_lower + 2), "Gy"), 0, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", PTV_GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }



                    //Set optimization adjustments for OARs and optimization strucures


                    //Optic pathway
                    if (optic_pathway != null & (optic_pathway.Volume > 0.1))
                    {
                        try
                        {
                            var optic_pathway_upper = plan.OptimizationSetup.AddPointObjective(optic_pathway, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 150);

                            if (optic_pathway_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 150);
                            }
                        }
                        catch { }
                    }


                    //Optic pathway PRV
                    if (optic_pathway_prv != null & (optic_pathway_prv.Volume > 0.1))
                    {
                        try
                        {
                            var optic_pathway_prv_upper = plan.OptimizationSetup.AddPointObjective(optic_pathway_prv, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 100);

                            if (optic_pathway_prv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(optic_pathway_prv, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 100);
                            }
                        }
                        catch { }
                    }


                    //Cochlea
                    if (cochlea != null & (cochlea.Volume > 0.1))
                    {
                        try
                        {
                            var cochlea_upper = plan.OptimizationSetup.AddPointObjective(cochlea, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 150);

                            if (cochlea_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(cochlea, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 150);
                            }
                        }
                        catch { }
                    }


                    //Brainstem
                    if (brainstem != null & (brainstem.Volume > 0.1))
                    {
                        try
                        {
                            var brainstem_upper = plan.OptimizationSetup.AddPointObjective(brainstem, OptimizationObjectiveOperator.Upper, new DoseValue(10, "Gy"), 0, 250);
                            var brainstem_mean = plan.OptimizationSetup.AddMeanDoseObjective(brainstem, new DoseValue(3, "Gy"), 80);

                            if (brainstem_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(brainstem, OptimizationObjectiveOperator.Upper, new DoseValue(10, "Gy"), 0, 250);
                            }
                            if (brainstem_mean == null)
                            {
                                plan.OptimizationSetup.AddMeanDoseObjective(brainstem, new DoseValue(3, "Gy"), 80);
                            }
                        }
                        catch { }
                    }


                    //Brainstem PRV
                    if (brainstem_prv != null & (brainstem_prv.Volume > 0.1))
                    {
                        try
                        {
                            var brainstem_prv_upper = plan.OptimizationSetup.AddPointObjective(brainstem_prv, OptimizationObjectiveOperator.Upper, new DoseValue(12, "Gy"), 0, 100);

                            if (brainstem_prv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(brainstem_prv, OptimizationObjectiveOperator.Upper, new DoseValue(12, "Gy"), 0, 100);
                            }
                        }
                        catch { }
                    }


                    //SpinalCord
                    if (spinalcord != null & (spinalcord.Volume > 0.1))
                    {
                        try
                        {
                            var spinalcord_upper = plan.OptimizationSetup.AddPointObjective(spinalcord, OptimizationObjectiveOperator.Upper, new DoseValue(9, "Gy"), 0, 250);

                            if (spinalcord_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(spinalcord, OptimizationObjectiveOperator.Upper, new DoseValue(9, "Gy"), 0, 250);
                            }
                        }
                        catch { }

                    }


                    //SpinalCord PRV
                    if (spinal_cord_prv != null & (spinal_cord_prv.Volume > 0.1))
                    {
                        try
                        {
                            var spinal_cord_prv_upper = plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(9, "Gy"), 0, 100);

                            if (spinal_cord_prv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(9, "Gy"), 0, 100);
                            }
                        }
                        catch { }

                    }


                    //SpinalCord-PTV
                    if (spinal_cord_ptv != null & (spinal_cord_ptv.Volume > 0.1))
                    {
                        try
                        {

                            var spinal_cord_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(spinal_cord_ptv, new DoseValue(4, "Gy"), 80);

                            if (spinal_cord_ptv_mean == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(spinal_cord_ptv, OptimizationObjectiveOperator.Upper, new DoseValue(4, "Gy"), 0, 80);
                            }
                        }
                        catch { }

                    }


                    //Esophagus
                    if (esophagus != null & (esophagus.Volume > 0.1))
                    {
                        try
                        {

                            var esophagus_upper = plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(17, "Gy"), 0, 250);

                            if (esophagus_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(17, "Gy"), 0, 250);
                            }
                        }
                        catch { }
                    }


                    //Brachial Plexus
                    if (brachial_plexus != null & (brachial_plexus.Volume > 0.1))
                    {
                        try
                        {
                            var brachial_plexus_upper = plan.OptimizationSetup.AddPointObjective(brachial_plexus, OptimizationObjectiveOperator.Upper, new DoseValue(15, "Gy"), 0, 250);

                            if (brachial_plexus_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(brachial_plexus, OptimizationObjectiveOperator.Upper, new DoseValue(15, "Gy"), 0, 250);
                            }
                        }
                        catch { }
                    }


                    //Trachea
                    if (trachea != null & (trachea.Volume > 0.1))
                    {
                        try
                        {
                            var trachea_upper = plan.OptimizationSetup.AddPointObjective(trachea, OptimizationObjectiveOperator.Upper, new DoseValue(18, "Gy"), 0, 250);

                            if (trachea_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(trachea, OptimizationObjectiveOperator.Upper, new DoseValue(18, "Gy"), 0, 250);
                            }
                        }
                        catch { }
                    }


                    //Trachea-PTV
                    if (trachea_ptv != null & (trachea_ptv.Volume > 0.1))
                    {
                        try
                        {

                            var trachea_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(trachea_ptv, new DoseValue(4, "Gy"), 80);
                            if (trachea_ptv_mean == null)
                            {
                                trachea_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(trachea_ptv, new DoseValue(4, "Gy"), 80);
                            }
                        }
                        catch { }
                    }


                    //Skin
                    if (skin != null & (skin.Volume > 0.1))
                    {
                        try
                        {
                            var skin_upper = plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(15, "Gy"), 0, 250);
                            var skin_upper_1 = plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(11, "Gy"), 0, 200);
                            if (skin_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(15, "Gy"), 0, 250);
                            }
                            if (skin_upper_1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(11, "Gy"), 0, 200);
                            }
                        }
                        catch { }
                    }


                    //Global maximum limit
                    //PharConst-PTV
                    if (BODY != null & (BODY.Volume > 0.1))
                    {
                        double BODY_upper_dose = Math.Round(((DPF * NOF) * 1.2), 2);
                        var BODY_upper = plan.OptimizationSetup.AddPointObjective(BODY, OptimizationObjectiveOperator.Upper, new DoseValue(BODY_upper_dose, "Gy"), 0, 80);
                        if (BODY_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(BODY, OptimizationObjectiveOperator.Upper, new DoseValue(BODY_upper_dose, "Gy"), 0, 80);
                            MessageBox.Show(string.Format("Please, define avoidance structures manually, if necessary. To continue with calculations, launch this script again"));
                            Window.GetWindow(this).Close(); return; ;
                        }
                    }
                }

                    GreenBar.Value = 35;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Analyze volume dose...";
                    GreenBarListBox.Items.Add(text.Text);
                     



                    //Begin VMAT optimization
                    //Check if the dose already has been calculated. If so, use it is intermediate dose
                    if (plan.Dose != null)
                    {
                        GreenBar.Value = 60;

                        text.Text = "Completed... Already calculated dose found";
                        GreenBarListBox.Items.Add(text.Text);
                         
                        text.Text = "Optimizing treatment plan...";
                        GreenBarListBox.Items.Add(text.Text);
                         


                        plan.OptimizeVMAT(new OptimizationOptionsVMAT(OptimizationOption.ContinueOptimizationWithPlanDoseAsIntermediateDose, string.Empty));
                    }
                    else
                    {

                        GreenBar.Value = 50;


                        text.Text = "Completed... Calculated dose NOT found";
                        GreenBarListBox.Items.Add(text.Text);
                         
                        text.Text = "Optimizing treatment plan...";
                        GreenBarListBox.Items.Add(text.Text);
                         



                        plan.OptimizeVMAT(new OptimizationOptionsVMAT(OptimizationIntermediateDoseOption.UseIntermediateDose, string.Empty));
                    }

                    GreenBar.Value = 70;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Calculating volume dose...";
                    GreenBarListBox.Items.Add(text.Text);




                    //Calculate volume dose
                    
                    try
                    {
                        plan.CalculateDose(); 
                    }
                    catch { try { MessageBox.Show(string.Format("Dear {0},\nUnfortunately, something went wrong during volume dose calculations.\n\nPlease, perform volume dose calculations manually", user), "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }

                    GreenBar.Value = 100;
                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Plan calculation finished!";
                    GreenBarListBox.Items.Add(text.Text);
                     








                    try
                    {
                        //Define DVH parameters to report
                        var ptv_DVH = plan.GetDoseAtVolume(ptv, 98.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                        var brainstem_MAX = plan.GetDoseAtVolume(brainstem, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var optic_pathway_MAX = plan.GetDoseAtVolume(optic_pathway, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var cochlea_MAX = plan.GetDoseAtVolume(cochlea, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var esophagus_MAX = plan.GetDoseAtVolume(esophagus, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var trachea_MAX = plan.GetDoseAtVolume(trachea, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var brachial_plexus_MAX = plan.GetDoseAtVolume(brachial_plexus, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var spinalcord_MAX = plan.GetDoseAtVolume(spinalcord, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);


                        //Show report message
                        var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                        MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! Plan calculation took '{0}' minutes\n 98% '{1}' covered by '{2}' Gy\n '{3}' max point dose = '{4}'Gy\n '{5}' max point dose = '{6}'Gy\n '{7}' max point dose = '{8}'Gy\n '{9}' max point dose = '{10}'Gy\n '{11}' max point dose = '{12}'Gy\n '{13}' max point dose = '{14}'Gy\n '{15}' max point dose = '{16}'Gy",


                            overall_calculation_time,
                            PTV_ID, ptv_DVH,
                            BRAINSTEM_ID, brainstem_MAX,
                            OPTIC_PATHWAY_ID, optic_pathway_MAX,
                            COCHLEA_ID, cochlea_MAX,
                            ESOPHAGUS_ID, esophagus_MAX,
                            TRACHEA_ID, trachea_MAX,
                            BRACHIAL_PLEXUS_ID, brachial_plexus_MAX,
                            SPINAL_CORD_ID, spinalcord_MAX
                            ),


                            SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Information);

                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown...")); }
                }
                if (Escalation.IsChecked == true)
                {
                    GreenBar.Value = 0;
                    GreenBarListBox.Items.Clear();
                    TextBox text = new TextBox();
                    text.Text = "-------Started-------                                                        ";
                    GreenBarListBox.Items.Add(text.Text);

                     


                    //Apply selected in combobox StructureSet for script execution
                    ss = VMS.TPS.Script.context.Patient.StructureSets.FirstOrDefault(x => x.Id == StructureSetID.Text);
                    // Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "SRS_Brain_Escalation";
                    //ID of the course
                    const string COURSE_ID = "Treatment";
                    //ID of the plan
                    string PLAN_ID = string.Format("SRS{0}/{1}Gy", (Math.Round((NOF * DPF), 0)).ToString(), (Math.Round((NOF * DPFescalation), 0)).ToString());


                    //IDs of fields
                    const string MVCBCT_ID = "MVCBCT";

                    const string BEAM_D_1_ID = "SRS179-181/20";
                    const string BEAM_D_2_ID = "SRS181-179/340";
                    const string BEAM_D_3_ID = "SRS179-181/90";



                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_ID = "PTV";
                    const string PTV_1_ID = "PTV_1";
                    const string PTV_2_ID = "PTV_2";
                    const string PTV_3_ID = "PTV_3";
                    const string OPT_PTV_ID = "PTV_OPT";
                    const string OPT_PTV_1_ID = "PTV_OPT_1";
                    const string OPT_PTV_2_ID = "PTV_OPT_2";
                    const string OPT_PTV_3_ID = "PTV_OPT_3";
                    const string GTV_ID = "GTV";
                    const string GTV_1_ID = "GTV_1";
                    const string GTV_2_ID = "GTV_2";
                    const string GTV_3_ID = "GTV_3";
                    const string OPT_GTV_ID = "GTV_OPT";
                    const string OPT_GTV_1_ID = "GTV_OPT_1";
                    const string OPT_GTV_2_ID = "GTV_OPT_2";
                    const string OPT_GTV_3_ID = "GTV_OPT_3";
                    const string PTV_GTV_ID = "PTV-GTV";
                    const string PTV_GTV_1_ID = "PTV-GTV_1";
                    const string PTV_GTV_2_ID = "PTV-GTV_2";
                    const string PTV_GTV_3_ID = "PTV-GTV_3";
                    const string PTV_SHELL_ID = "PTV_shell";


                    //IDs of critical structures
                    const string BODY_ID = "BODY";
                    const string SKIN_ID = "Skin_bySCRPT";
                    const string OPTIC_PATHWAY_ID = "Optic pathway";
                    const string OPTIC_PATHWAY_PRV_ID = "Optic path_PRV";
                    const string COCHLEA_ID = "Cochlea";
                    const string COCHLEA_PRV_ID = "Cochlea_PRV";
                    const string BRAINSTEM_ID = "Brainstem";
                    const string BRAINSTEM_PRV_ID = "Brainstem_PRV";
                    const string BRAINSTEM_OPT_ID = "Brainstem_PTV";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "Spinal Cor_PRV";
                    const string SPINAL_CORD_OPT = "Spinal Cor_PTV";
                    const string SPINAL_CORD_SUBVOL_ID = "SpinalCrd Subvol";
                    const string MEDULLA_ID = "Medulla";
                    const string ESOPHAGUS_ID = "Esophagus";
                    const string ESOPHAGUS_OPT_ID = "Esophagus_PTV";
                    const string BRACHIAL_PLEXUS_ID = "Brachial plexus";
                    const string BRACHIAL_PLEXUS_PRV_ID = "Brachial p_PRV";
                    const string BRACHIAL_PLEXUS_PTV_ID = "Brachial p_PTV";
                    const string TRACHEA_ID = "Trachea";
                    const string TRACHEA_OPT_ID = "Trachea_PTV";



                    //Show greetings window
                    string Greeting = string.Format("Greetings {0}! Please, ensure that the structures 'PTV', 'GTVp', 'PTV-GTV' exist in the currest structure set.\n If you would like to calculate up to 4 targets from one isocenter, name these targets as: 'PTV', 'PTV_1', 'PTV_2', 'PTV_3', 'GTV', 'GTV_1', 'GTV_2', 'GTV_3'\n Script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru'ru).", user);
                    MessageBox.Show(Greeting);


                    //Turn on the stopwatch
                    var stopwatch = DateTime.Now;

                    GreenBar.Value = 5;
                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                    text.Text = "Defining constants...";
                    GreenBarListBox.Items.Add(text.Text);                     


                    //Begin modifications
                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //Add course
                    Course course;
                    if (patient.Courses.Where(x => x.Id == COURSE_ID).Any()) course = patient.Courses.Where(x => x.Id == COURSE_ID).Single();
                    else
                    {
                        course = patient.AddCourse();
                        course.Id = COURSE_ID;
                    }

                    GreenBar.Value = 10;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Treatment cource creation...";
                    GreenBarListBox.Items.Add(text.Text);
                     



                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution                    

                    List<string> NOT_FOUND_list = new List<string>();

                    //find PTVs. Show an error notification if no optimization PTV will be found
                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found! Please, ensure that PTV is presented in the structure set", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Window.GetWindow(this).Close(); return; ;
                    }

                    Structure ptv_1 = ss.Structures.FirstOrDefault(x => x.Id == PTV_1_ID);
                    if (ptv_1 == null)
                    {
                        NOT_FOUND_list.Add(PTV_1_ID);
                    }

                    Structure ptv_2 = ss.Structures.FirstOrDefault(x => x.Id == PTV_2_ID);
                    if (ptv_2 == null)
                    {
                        NOT_FOUND_list.Add(PTV_2_ID);
                    }

                    Structure ptv_3 = ss.Structures.FirstOrDefault(x => x.Id == PTV_3_ID);
                    if (ptv_3 == null)
                    {
                        NOT_FOUND_list.Add(PTV_3_ID);
                    }

                    Structure gtv = ss.Structures.FirstOrDefault(x => x.Id == GTV_ID);
                    if (gtv == null)
                    {
                        NOT_FOUND_list.Add(GTV_ID);
                    }

                    Structure gtv_1 = ss.Structures.FirstOrDefault(x => x.Id == GTV_1_ID);
                    if (gtv_1 == null)
                    {
                        NOT_FOUND_list.Add(GTV_1_ID);
                    }

                    Structure gtv_2 = ss.Structures.FirstOrDefault(x => x.Id == GTV_2_ID);
                    if (gtv_2 == null)
                    {
                        NOT_FOUND_list.Add(GTV_2_ID);
                    }

                    Structure gtv_3 = ss.Structures.FirstOrDefault(x => x.Id == GTV_3_ID);
                    if (gtv_3 == null)
                    {
                        NOT_FOUND_list.Add(GTV_3_ID);
                    }

                    Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                    if (ptv_gtv == null)
                    {
                        NOT_FOUND_list.Add(PTV_GTV_ID);
                    }

                    Structure ptv_gtv_1 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_1_ID);
                    if (ptv_gtv_1 == null)
                    {
                        NOT_FOUND_list.Add(PTV_GTV_1_ID);
                        try { ptv_gtv_1 = ss.AddStructure("Avoidance", PTV_GTV_1_ID); } catch { }
                    }

                    Structure ptv_gtv_2 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_2_ID);
                    if (ptv_gtv_2 == null)
                    {
                        NOT_FOUND_list.Add(PTV_GTV_2_ID);
                        try { ptv_gtv_2 = ss.AddStructure("Avoidance", PTV_GTV_2_ID); } catch { }
                    }

                    Structure ptv_gtv_3 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_3_ID);
                    if (ptv_gtv_3 == null)
                    {
                        NOT_FOUND_list.Add(PTV_GTV_3_ID);
                        try { ptv_gtv_3 = ss.AddStructure("Avoidance", PTV_GTV_3_ID); } catch { }
                    }


                    //Find OARs and optimization structures

                    //find Skin
                    Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                    if (skin == null)
                    {
                        NOT_FOUND_list.Add(SKIN_ID);
                        try { skin = ss.AddStructure("Avoidance", SKIN_ID); } catch { }
                    }

                    //find Optic pathway
                    Structure optic_pathway = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_PATHWAY_ID);
                    if (optic_pathway == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_PATHWAY_ID);
                        try { optic_pathway = ss.AddStructure("Avoidance", OPTIC_PATHWAY_ID); } catch { }
                    }

                    //find Cochlea
                    Structure cochlea = ss.Structures.FirstOrDefault(x => x.Id == COCHLEA_ID);
                    if (cochlea == null)
                    {
                        NOT_FOUND_list.Add(COCHLEA_ID);
                        try { cochlea = ss.AddStructure("Avoidance", COCHLEA_ID); } catch { }
                    }

                    //find BrainStem
                    Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                    if (brainstem == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_ID);
                        try { brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { }
                    }

                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Medulla
                    Structure medulla = ss.Structures.FirstOrDefault(x => x.Id == MEDULLA_ID);
                    if (medulla == null)
                    {
                        NOT_FOUND_list.Add(MEDULLA_ID);
                        try { medulla = ss.AddStructure("Avoidance", MEDULLA_ID); } catch { }
                    }

                    //find SpinalCord sub volume
                    Structure spinalcord_subvol = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_SUBVOL_ID);
                    if (spinalcord_subvol == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_SUBVOL_ID);
                        try { spinalcord_subvol = ss.AddStructure("Avoidance", SPINAL_CORD_SUBVOL_ID); } catch { }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find Brachial plexus
                    Structure brachial_plexus = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_ID);
                    if (brachial_plexus == null)
                    {
                        NOT_FOUND_list.Add(BRACHIAL_PLEXUS_ID);
                        try { brachial_plexus = ss.AddStructure("Avoidance", BRACHIAL_PLEXUS_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return; ;
                    }

                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);

                    if (ptv_opt == null)
                    {
                        NOT_FOUND_list.Add(OPT_PTV_ID);
                        try { ptv_opt = ss.AddStructure("Avoidance", OPT_PTV_ID); } catch { }
                    }

                    Structure ptv_opt_1 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_1_ID);

                    if (ptv_opt_1 == null)
                    {
                        NOT_FOUND_list.Add(OPT_PTV_1_ID);
                        try { ptv_opt_1 = ss.AddStructure("Avoidance", OPT_PTV_1_ID); } catch { }
                    }

                    Structure ptv_opt_2 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_2_ID);

                    if (ptv_opt_2 == null)
                    {
                        NOT_FOUND_list.Add(OPT_PTV_2_ID);
                        try { ptv_opt_2 = ss.AddStructure("Avoidance", OPT_PTV_2_ID); } catch { }
                    }

                    Structure ptv_opt_3 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_3_ID);

                    if (ptv_opt_3 == null)
                    {
                        NOT_FOUND_list.Add(OPT_PTV_3_ID);
                        try { ptv_opt_3 = ss.AddStructure("Avoidance", OPT_PTV_3_ID); } catch { }
                    }


                    Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_ID);

                    if (gtv_opt == null)
                    {
                        NOT_FOUND_list.Add(OPT_GTV_ID);
                        try { gtv_opt = ss.AddStructure("Avoidance", OPT_GTV_ID); } catch { }
                    }

                    Structure gtv_opt_1 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_1_ID);

                    if (gtv_opt_1 == null)
                    {
                        NOT_FOUND_list.Add(OPT_GTV_1_ID);
                        try { gtv_opt_1 = ss.AddStructure("Avoidance", OPT_GTV_1_ID); } catch { }
                    }

                    Structure gtv_opt_2 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_2_ID);

                    if (gtv_opt_2 == null)
                    {
                        NOT_FOUND_list.Add(OPT_GTV_2_ID);
                        try { gtv_opt_2 = ss.AddStructure("Avoidance", OPT_GTV_2_ID); } catch { }
                    }

                    Structure gtv_opt_3 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_3_ID);

                    if (gtv_opt_3 == null)
                    {
                        NOT_FOUND_list.Add(OPT_GTV_3_ID);
                        try { gtv_opt_3 = ss.AddStructure("Avoidance", OPT_GTV_3_ID); } catch { }
                    }

                    Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);

                    if (ptv_shell == null)
                    {
                        NOT_FOUND_list.Add(PTV_SHELL_ID);
                        try { ptv_shell = ss.AddStructure("Avoidance", PTV_SHELL_ID); } catch { }
                    }



                    Structure optic_pathway_prv = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_PATHWAY_PRV_ID);

                    if (optic_pathway_prv == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_PATHWAY_PRV_ID);
                        try { optic_pathway_prv = ss.AddStructure("Avoidance", OPTIC_PATHWAY_PRV_ID); } catch { }
                    }

                    Structure cochlea_prv = ss.Structures.FirstOrDefault(x => x.Id == COCHLEA_PRV_ID);
                    if (cochlea_prv == null)
                    {
                        NOT_FOUND_list.Add(COCHLEA_PRV_ID);
                        try { cochlea_prv = ss.AddStructure("Avoidance", COCHLEA_PRV_ID); } catch { }
                    }

                    Structure brainstem_prv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV_ID);
                    if (brainstem_prv == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_PRV_ID);
                        try { brainstem_prv = ss.AddStructure("Avoidance", BRAINSTEM_PRV_ID); } catch { }
                    }

                    Structure brainstem_ptv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_OPT_ID);
                    if (brainstem_ptv == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_OPT_ID);
                        try { brainstem_ptv = ss.AddStructure("Avoidance", BRAINSTEM_OPT_ID); } catch { }
                    }

                    Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spinal_cord_prv == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_PRV_ID);
                        try { spinal_cord_prv = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); } catch { }
                    }

                    Structure spinal_cord_ptv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_OPT);
                    if (spinal_cord_ptv == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_OPT);
                        try { spinal_cord_ptv = ss.AddStructure("Avoidance", SPINAL_CORD_OPT); } catch { }
                    }

                    Structure brachial_plexus_prv = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_PRV_ID);
                    if (brachial_plexus_prv == null)
                    {
                        NOT_FOUND_list.Add(BRACHIAL_PLEXUS_PRV_ID);
                        try { brachial_plexus_prv = ss.AddStructure("Avoidance", BRACHIAL_PLEXUS_PRV_ID); } catch { }
                    }

                    Structure brachial_plexus_ptv = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_PTV_ID);
                    if (brachial_plexus_ptv == null)
                    {
                        NOT_FOUND_list.Add(BRACHIAL_PLEXUS_PTV_ID);
                        try { brachial_plexus_ptv = ss.AddStructure("Avoidance", BRACHIAL_PLEXUS_PTV_ID); } catch { }
                    }


                    Structure esophagus_ptv = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_OPT_ID);
                    if (esophagus_ptv == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_OPT_ID);
                        try { esophagus_ptv = ss.AddStructure("Avoidance", ESOPHAGUS_OPT_ID); } catch { }
                    }

                    Structure trachea_ptv = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_OPT_ID);
                    if (trachea_ptv == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_OPT_ID);
                        try { trachea_ptv = ss.AddStructure("Avoidance", TRACHEA_OPT_ID); } catch { }
                    }

                    //show message with list of not found structures
                    string[] NOT_FOUND = NOT_FOUND_list.ToArray();
                    string res = string.Join("\r\n", NOT_FOUND);
                    var array_length = NOT_FOUND.Length;
                    if (array_length > 0)
                    {
                        MessageBox.Show(string.Format("Structures were not found:\n'{0}'", res), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                    //put isocenter to the center of PTV
                    VVector isocenter = new VVector(Math.Round(ptv.CenterPoint.x / 10.0f) * 10.0f, Math.Round(ptv.CenterPoint.y / 10.0f) * 10.0f, Math.Round(ptv.CenterPoint.z / 10.0f) * 10.0f);

                    //Add plan 

                    string report_plan_creation = string.Format("");
                    ExternalPlanSetup plan = course.ExternalPlanSetups.FirstOrDefault(x => x.Id == PLAN_ID);

                    if (plan != null)
                    {
                        plan = course.ExternalPlanSetups.FirstOrDefault(x => x.Id == PLAN_ID);
                        report_plan_creation = string.Format("Course, plan, imaging setup and beams are exist. Continuing with the optimization and dose calculation", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (plan == null)
                    {
                        plan = course.AddExternalPlanSetup(ss);
                        plan.Id = PLAN_ID;
                        report_plan_creation = string.Format("Course and plan creation completed successful. Please, ensure that calibration curve is chosen correctly, review the dose matrix, insert the imaging setup, attach the plan to a clinical protocol and adjust the reference point. If you would like to proceed with the optimization and dose volume calculation, please launch this script again.", MessageBoxButton.OK, MessageBoxImage.Information);
                    }


                    GreenBar.Value = 15;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Treatment plan creation...";
                    GreenBarListBox.Items.Add(text.Text);
                     



                    //Define prescription
                    plan.SetPrescription(NOF, new DoseValue(DPF, DoseValue.DoseUnit.Gy), 1.0);

                    GreenBar.Value = 20;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Prescribed dofe calculation...";
                    GreenBarListBox.Items.Add(text.Text);
                     


                    //Define Machine parameters
                    ExternalBeamMachineParameters MachineParameters =
                    new ExternalBeamMachineParameters(linac, "6X", 800, "SRS ARC", "FFF");


                    //Check if the beams already exist. If no, create beams

                    //Setup by default

                    Beam beam1_D = plan.Beams.FirstOrDefault(x => x.Id == BEAM_D_1_ID);
                    if (beam1_D == null)
                    {
                        beam1_D = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 179, 181, GantryDirection.CounterClockwise, 0, isocenter);
                        beam1_D.Id = BEAM_D_1_ID;
                    }

                    Beam beam2_D = plan.Beams.FirstOrDefault(x => x.Id == BEAM_D_2_ID);
                    if (beam2_D == null)
                    {
                        beam2_D = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 181, 179, GantryDirection.Clockwise, 0, isocenter);
                        beam2_D.Id = BEAM_D_2_ID;
                    }
                    /*
                    Beam beam3_D = plan.Beams.FirstOrDefault(x=>x.Id == BEAM_D_3_ID);
                    if (beam3_D == null)
                    {
                        beam3_D = plan.AddArcBeam(MachineParameters, new VRect<double>(280,280,280,280), 90, 179, 181, GantryDirection.CounterClockwise, 0, isocenter);
                        beam3_D.Id = BEAM_D_3_ID;
                    }
                    */
                    //Fit collimator to structure. For Halcyon machines the next 5 strings are not necessary
                    try
                    {
                        bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                        beam1_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                        beam2_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                        //beam3_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                        FitToStructureMargins margins = new FitToStructureMargins(0);
                    }
                    catch { }
                    GreenBar.Value = 25;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Placing beams in plan...";
                    GreenBarListBox.Items.Add(text.Text);
                     




                    //Show report message on successful course and plan creation
                    MessageBox.Show(report_plan_creation);

                    //Define optimizator model
                    plan.SetCalculationModel(CalculationType.PhotonVMATOptimization, calculation_model);


                    //Check if the MVCBCT beam exist in the plan. If no, show an error notification and stop execution
                    Beam mvcbct = plan.Beams.FirstOrDefault(x => x.Id == MVCBCT_ID);
                    if (mvcbct == null)
                    {

                        MessageBox.Show(string.Format("Imaging setup was not found! Please, ensure that a beam with ID: '{0}' exist", MVCBCT_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }


                    //Set Normal tissie objective (NTO)
                    plan.OptimizationSetup.AddNormalTissueObjective(120, 0, 100, 35, 0.18);


                    //Set optimization adjustments for PTVs
                    GreenBar.Value = 30;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Defining objectives...";
                    GreenBarListBox.Items.Add(text.Text);
                     


                    double target_lower = Math.Round((DPF * NOF), 2);
                    double target_upper = Math.Round(((DPF * NOF) * 1.05), 2);
                    double target_escalation_lower = Math.Round((DPFescalation * NOF), 2);

                if (RapidPlan.IsChecked == true) 
                {
                    DoseValue escalationDose = new DoseValue((target_escalation_lower + 0.5), DoseValue.DoseUnit.Gy);



                    //Choose estimation model ID
                    string DVHestimationModelId = "SRS Brain";


                    //Choose target dose level. Due to the reason that no escalation is checked, we will put the same dose target leves for PTV and GTV
                    Dictionary<string, DoseValue> targetDoseLevels = new Dictionary<string, DoseValue>();

                    try { targetDoseLevels.Add("PTV_OPT", plan.TotalDose); } catch { }
                    try { targetDoseLevels.Add("PTV_OPT_1", plan.TotalDose); } catch { }
                    try { targetDoseLevels.Add("PTV_OPT_2", plan.TotalDose); } catch { }
                    try { targetDoseLevels.Add("PTV_OPT_3", plan.TotalDose); } catch { }

                    try { targetDoseLevels.Add("GTV_OPT", escalationDose); } catch { }
                    try { targetDoseLevels.Add("GTV_OPT_1", escalationDose); } catch { }
                    try { targetDoseLevels.Add("GTV_OPT_2", escalationDose); } catch { }
                    try { targetDoseLevels.Add("GTV_OPT_3", escalationDose); } catch { }

                    //Match structures from the structure set to structurel listed in DVHestimationModel 
                    Dictionary<string, string> structureMatches = new Dictionary<string, string>();
                    
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID).Id, "PTV_OPT"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_1_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_1_ID).Id, "PTV_OPT_1"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_2_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_2_ID).Id, "PTV_OPT_2"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_3_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_3_ID).Id, "PTV_OPT_3"); } } catch { }

                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_ID).Id, "GTV_OPT"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_1_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_1_ID).Id, "GTV_OPT_1"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_2_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_2_ID).Id, "GTV_OPT_2"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_3_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_3_ID).Id, "GTV_OPT_3"); } } catch { }

                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID).Id, "PTV-GTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_1_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_1_ID).Id, "PTV-GTV_1"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_2_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_2_ID).Id, "PTV-GTV_2"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_3_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_3_ID).Id, "PTV-GTV_3"); } } catch { }


                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BODY")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BODY").Id, "BODY"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem").Id, "Brainstem"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem_PRV").Id, "Brainstem_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem-PTV").Id, "Brainstem-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm").Id, "Chiasm"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm_PRV").Id, "Chiasm_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm-PTV").Id, "Chiasm-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_L").Id, "Cochlea_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_R").Id, "Cochlea_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_L").Id, "OpticNerve_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_R").Id, "OpticNerve_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Skin")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Skin").Id, "Skin"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord").Id, "SpinalCord"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV").Id, "SpnlCrd_PRV"); } } catch { }


                    plan.CalculateDVHEstimates(DVHestimationModelId, targetDoseLevels, structureMatches);



                }
                else
                {


                    //Set optimization adjustments for PTVs

                    //PTV
                    if (ptv != null & (ptv.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_lower = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);


                            if (ptv_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for PTV could not be defined..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (ptv_1 != null & (ptv_1.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_lower_1 = plan.OptimizationSetup.AddPointObjective(ptv_1, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);


                            if (ptv_lower_1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_1, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", PTV_1_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (ptv_2 != null & (ptv_2.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_lower_2 = plan.OptimizationSetup.AddPointObjective(ptv_2, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);


                            if (ptv_lower_2 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_2, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", PTV_2_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (ptv_3 != null & (ptv_3.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_lower_3 = plan.OptimizationSetup.AddPointObjective(ptv_3, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);


                            if (ptv_lower_3 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_3, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 80);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", PTV_3_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }


                    if (ptv_opt != null & (ptv_opt.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_opt_lower = plan.OptimizationSetup.AddPointObjective(ptv_opt, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);


                            if (ptv_opt_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_opt, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (ptv_opt_1 != null & (ptv_opt_1.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_opt_1_lower = plan.OptimizationSetup.AddPointObjective(ptv_opt_1, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);


                            if (ptv_opt_1_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_opt_1, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_PTV_1_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (ptv_opt_2 != null & (ptv_opt_2.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_opt_2_lower = plan.OptimizationSetup.AddPointObjective(ptv_opt_2, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);


                            if (ptv_opt_2_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_opt_2, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_PTV_2_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (ptv_opt_3 != null & (ptv_opt_3.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_opt_3_lower = plan.OptimizationSetup.AddPointObjective(ptv_opt_3, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);


                            if (ptv_opt_3_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_opt_3, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_PTV_3_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (gtv != null & (gtv.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_lower = plan.OptimizationSetup.AddPointObjective(gtv, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower, "Gy"), 99.9, 100);


                            if (gtv_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower, "Gy"), 99.9, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (gtv_1 != null & (gtv_1.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_lower_1 = plan.OptimizationSetup.AddPointObjective(gtv_1, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower, "Gy"), 99.9, 100);


                            if (gtv_lower_1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_1, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower, "Gy"), 99.9, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", GTV_1_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (gtv_2 != null & (gtv_2.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_lower_2 = plan.OptimizationSetup.AddPointObjective(gtv_2, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower, "Gy"), 99.9, 100);


                            if (gtv_lower_2 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_2, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower, "Gy"), 99.9, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", GTV_2_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }

                    if (gtv_3 != null & (gtv_3.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_lower_3 = plan.OptimizationSetup.AddPointObjective(gtv_3, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower, "Gy"), 99.9, 100);


                            if (gtv_lower_3 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_3, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower, "Gy"), 99.9, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", GTV_3_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }

                    if (gtv_opt != null & (gtv_opt.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_opt_lower = plan.OptimizationSetup.AddPointObjective(gtv_opt, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower + 0.5, "Gy"), 99.9, 200);


                            if (gtv_opt_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_opt, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower + 0.5, "Gy"), 99.9, 200);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }

                    if (gtv_opt_1 != null & (gtv_opt_1.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_opt_lower_1 = plan.OptimizationSetup.AddPointObjective(gtv_opt_1, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower + 0.5, "Gy"), 99.9, 200);


                            if (gtv_opt_lower_1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_opt_1, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower + 0.5, "Gy"), 99.9, 200);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_GTV_1_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }

                    if (gtv_opt_2 != null & (gtv_opt_2.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_opt_lower_2 = plan.OptimizationSetup.AddPointObjective(gtv_opt_2, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower + 0.5, "Gy"), 99.9, 200);


                            if (gtv_opt_lower_2 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_opt_2, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower + 0.5, "Gy"), 99.9, 200);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_GTV_2_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }

                    if (gtv_opt_3 != null & (gtv_opt_3.Volume > 0.01))
                    {
                        try
                        {
                            var gtv_opt_lower_3 = plan.OptimizationSetup.AddPointObjective(gtv_opt_3, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower + 0.5, "Gy"), 99.9, 200);


                            if (gtv_opt_lower_3 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_opt_3, OptimizationObjectiveOperator.Lower, new DoseValue(target_escalation_lower + 0.5, "Gy"), 99.9, 200);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", OPT_GTV_3_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }

                    if (ptv_gtv != null & (ptv_gtv.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_gtv_upper = plan.OptimizationSetup.AddPointObjective(ptv_gtv, OptimizationObjectiveOperator.Upper, new DoseValue(target_escalation_lower, "Gy"), 0, 100);


                            if (ptv_gtv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_gtv, OptimizationObjectiveOperator.Upper, new DoseValue(target_escalation_lower, "Gy"), 0, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", PTV_GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (ptv_gtv_1 != null & (ptv_gtv_1.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_gtv_upper_1 = plan.OptimizationSetup.AddPointObjective(ptv_gtv_1, OptimizationObjectiveOperator.Upper, new DoseValue(target_escalation_lower, "Gy"), 0, 100);


                            if (ptv_gtv_upper_1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_gtv_1, OptimizationObjectiveOperator.Upper, new DoseValue(target_escalation_lower, "Gy"), 0, 100);
                            }
                        }
                        catch
                        {
                            MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", PTV_GTV_1_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    if (ptv_gtv_2 != null & (ptv_gtv_2.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_gtv_upper_2 = plan.OptimizationSetup.AddPointObjective(ptv_gtv_2, OptimizationObjectiveOperator.Upper, new DoseValue(target_escalation_lower, "Gy"), 0, 100);


                            if (ptv_gtv_upper_2 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_gtv_2, OptimizationObjectiveOperator.Upper, new DoseValue(target_escalation_lower, "Gy"), 0, 100);
                            }
                        }
                        catch
                        {
                            MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", PTV_GTV_2_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    if (ptv_gtv_3 != null & (ptv_gtv_3.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_gtv_upper_3 = plan.OptimizationSetup.AddPointObjective(ptv_gtv_3, OptimizationObjectiveOperator.Upper, new DoseValue(target_escalation_lower, "Gy"), 0, 100);


                            if (ptv_gtv_upper_3 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_gtv_3, OptimizationObjectiveOperator.Upper, new DoseValue(target_escalation_lower, "Gy"), 0, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Unfortunately, optimization objectives for {0} could not be defined...", PTV_GTV_3_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    //Set optimization adjustments for OARs and optimization strucures


                    //Optic pathway
                    if (optic_pathway != null & (optic_pathway.Volume > 0.1))
                    {
                        try
                        {
                            var optic_pathway_upper = plan.OptimizationSetup.AddPointObjective(optic_pathway, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 150);

                            if (optic_pathway_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 150);
                            }
                        }
                        catch { }
                    }


                    //Optic pathway PRV
                    if (optic_pathway_prv != null & (optic_pathway_prv.Volume > 0.1))
                    {
                        try
                        {
                            var optic_pathway_prv_upper = plan.OptimizationSetup.AddPointObjective(optic_pathway_prv, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 100);

                            if (optic_pathway_prv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(optic_pathway_prv, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 100);
                            }
                        }
                        catch { }
                    }


                    //Cochlea
                    if (cochlea != null & (cochlea.Volume > 0.1))
                    {
                        try
                        {
                            var cochlea_upper = plan.OptimizationSetup.AddPointObjective(cochlea, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 150);

                            if (cochlea_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(cochlea, OptimizationObjectiveOperator.Upper, new DoseValue(7, "Gy"), 0, 150);
                            }
                        }
                        catch { }
                    }


                    //Brainstem
                    if (brainstem != null & (brainstem.Volume > 0.1))
                    {
                        try
                        {
                            var brainstem_upper = plan.OptimizationSetup.AddPointObjective(brainstem, OptimizationObjectiveOperator.Upper, new DoseValue(10, "Gy"), 0, 250);
                            var brainstem_mean = plan.OptimizationSetup.AddMeanDoseObjective(brainstem, new DoseValue(3, "Gy"), 80);

                            if (brainstem_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(brainstem, OptimizationObjectiveOperator.Upper, new DoseValue(10, "Gy"), 0, 250);
                            }
                            if (brainstem_mean == null)
                            {
                                plan.OptimizationSetup.AddMeanDoseObjective(brainstem, new DoseValue(3, "Gy"), 80);
                            }
                        }
                        catch { }
                    }


                    //Brainstem PRV
                    if (brainstem_prv != null & (brainstem_prv.Volume > 0.1))
                    {
                        try
                        {
                            var brainstem_prv_upper = plan.OptimizationSetup.AddPointObjective(brainstem_prv, OptimizationObjectiveOperator.Upper, new DoseValue(12, "Gy"), 0, 100);

                            if (brainstem_prv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(brainstem_prv, OptimizationObjectiveOperator.Upper, new DoseValue(12, "Gy"), 0, 100);
                            }
                        }
                        catch { }
                    }


                    //SpinalCord
                    if (spinalcord != null & (spinalcord.Volume > 0.1))
                    {
                        try
                        {
                            var spinalcord_upper = plan.OptimizationSetup.AddPointObjective(spinalcord, OptimizationObjectiveOperator.Upper, new DoseValue(9, "Gy"), 0, 250);

                            if (spinalcord_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(spinalcord, OptimizationObjectiveOperator.Upper, new DoseValue(9, "Gy"), 0, 250);
                            }
                        }
                        catch { }

                    }


                    //SpinalCord PRV
                    if (spinal_cord_prv != null & (spinal_cord_prv.Volume > 0.1))
                    {
                        try
                        {
                            var spinal_cord_prv_upper = plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(9, "Gy"), 0, 100);

                            if (spinal_cord_prv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(9, "Gy"), 0, 100);
                            }
                        }
                        catch { }

                    }


                    //SpinalCord-PTV
                    if (spinal_cord_ptv != null & (spinal_cord_ptv.Volume > 0.1))
                    {
                        try
                        {

                            var spinal_cord_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(spinal_cord_ptv, new DoseValue(4, "Gy"), 80);

                            if (spinal_cord_ptv_mean == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(spinal_cord_ptv, OptimizationObjectiveOperator.Upper, new DoseValue(4, "Gy"), 0, 80);
                            }
                        }
                        catch { }

                    }


                    //Esophagus
                    if (esophagus != null & (esophagus.Volume > 0.1))
                    {
                        try
                        {

                            var esophagus_upper = plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(17, "Gy"), 0, 250);

                            if (esophagus_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(17, "Gy"), 0, 250);
                            }
                        }
                        catch { }
                    }


                    //Brachial Plexus
                    if (brachial_plexus != null & (brachial_plexus.Volume > 0.1))
                    {
                        try
                        {
                            var brachial_plexus_upper = plan.OptimizationSetup.AddPointObjective(brachial_plexus, OptimizationObjectiveOperator.Upper, new DoseValue(15, "Gy"), 0, 250);

                            if (brachial_plexus_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(brachial_plexus, OptimizationObjectiveOperator.Upper, new DoseValue(15, "Gy"), 0, 250);
                            }
                        }
                        catch { }
                    }


                    //Trachea
                    if (trachea != null & (trachea.Volume > 0.1))
                    {
                        try
                        {
                            var trachea_upper = plan.OptimizationSetup.AddPointObjective(trachea, OptimizationObjectiveOperator.Upper, new DoseValue(18, "Gy"), 0, 250);

                            if (trachea_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(trachea, OptimizationObjectiveOperator.Upper, new DoseValue(18, "Gy"), 0, 250);
                            }
                        }
                        catch { }
                    }


                    //Trachea-PTV
                    if (trachea_ptv != null & (trachea_ptv.Volume > 0.1))
                    {
                        try
                        {

                            var trachea_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(trachea_ptv, new DoseValue(4, "Gy"), 80);
                            if (trachea_ptv_mean == null)
                            {
                                trachea_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(trachea_ptv, new DoseValue(4, "Gy"), 80);
                            }
                        }
                        catch { }
                    }


                    //Skin
                    if (skin != null & (skin.Volume > 0.1))
                    {
                        try
                        {
                            var skin_upper = plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(15, "Gy"), 0, 250);
                            var skin_upper_1 = plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(11, "Gy"), 0, 200);
                            if (skin_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(15, "Gy"), 0, 250);
                            }
                            if (skin_upper_1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(11, "Gy"), 0, 200);
                            }
                        }
                        catch { }
                    }


                    //Global maximum limit
                    //PharConst-PTV
                    if (BODY != null & (BODY.Volume > 0.1))
                    {
                        double TDescalation = DPFescalation * NOF;
                        double BODY_upper_dose = Math.Round((TDescalation * 1.2), 2);
                        var BODY_upper = plan.OptimizationSetup.AddPointObjective(BODY, OptimizationObjectiveOperator.Upper, new DoseValue(BODY_upper_dose, "Gy"), 0, 80);
                        if (BODY_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(BODY, OptimizationObjectiveOperator.Upper, new DoseValue(BODY_upper_dose, "Gy"), 0, 80);
                            MessageBox.Show(string.Format("Please, define avoidance structures manually, if necessary. To continue with calculations, launch this script again"));
                            Window.GetWindow(this).Close(); return; ;
                        }
                    }
                }

                    GreenBar.Value = 35;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Analyze volume dose...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    //Begin VMAT optimization
                    //Check if the dose already has been calculated. If so, use it is intermediate dose
                    if (plan.Dose != null)
                    {
                        GreenBar.Value = 60;

                        text.Text = "Completed... Already calculated dose found";
                        GreenBarListBox.Items.Add(text.Text);
                         
                        text.Text = "Optimizing treatment plan...";
                        GreenBarListBox.Items.Add(text.Text);
                         


                        plan.OptimizeVMAT(new OptimizationOptionsVMAT(OptimizationOption.ContinueOptimizationWithPlanDoseAsIntermediateDose, string.Empty));
                    }
                    else
                    {

                        GreenBar.Value = 50;


                        text.Text = "Completed... Calculated dose NOT found";
                        GreenBarListBox.Items.Add(text.Text);
                         
                        text.Text = "Optimizing treatment plan...";
                        GreenBarListBox.Items.Add(text.Text);
                         



                        plan.OptimizeVMAT(new OptimizationOptionsVMAT(OptimizationIntermediateDoseOption.UseIntermediateDose, string.Empty));
                    }

                    GreenBar.Value = 70;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Calculating volume dose...";
                    GreenBarListBox.Items.Add(text.Text);

                    //Calculate volume dose
                    
                    try { plan.CalculateDose();  } catch { try { MessageBox.Show(string.Format("Dear {0},\nUnfortunately, something went wrong during volume dose calculations.\n\nPlease, perform volume dose calculations manually", user), "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }

                    GreenBar.Value = 100;
                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Plan calculation finished!";
                    GreenBarListBox.Items.Add(text.Text);
                    
                    try
                    {
                        //Define DVH parameters to report
                        var ptv_DVH = plan.GetDoseAtVolume(ptv, 98.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                        var brainstem_MAX = plan.GetDoseAtVolume(brainstem, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var optic_pathway_MAX = plan.GetDoseAtVolume(optic_pathway, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var cochlea_MAX = plan.GetDoseAtVolume(cochlea, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var esophagus_MAX = plan.GetDoseAtVolume(esophagus, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var trachea_MAX = plan.GetDoseAtVolume(trachea, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var brachial_plexus_MAX = plan.GetDoseAtVolume(brachial_plexus, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var spinalcord_MAX = plan.GetDoseAtVolume(spinalcord, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);


                        //Show report message
                        var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                        MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! Plan calculation took '{0}' minutes\n 98% '{1}' covered by '{2}' Gy\n '{3}' max point dose = '{4}'Gy\n '{5}' max point dose = '{6}'Gy\n '{7}' max point dose = '{8}'Gy\n '{9}' max point dose = '{10}'Gy\n '{11}' max point dose = '{12}'Gy\n '{13}' max point dose = '{14}'Gy\n '{15}' max point dose = '{16}'Gy",


                            overall_calculation_time,
                            PTV_ID, ptv_DVH,
                            BRAINSTEM_ID, brainstem_MAX,
                            OPTIC_PATHWAY_ID, optic_pathway_MAX,
                            COCHLEA_ID, cochlea_MAX,
                            ESOPHAGUS_ID, esophagus_MAX,
                            TRACHEA_ID, trachea_MAX,
                            BRACHIAL_PLEXUS_ID, brachial_plexus_MAX,
                            SPINAL_CORD_ID, spinalcord_MAX
                            ),


                            SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Information);

                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown...")); }
                }
            
            Window.GetWindow(this).Close();

        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();

            AutoPlan auto_plan = new AutoPlan(VMS.TPS.Script.context);
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

        private void GreenBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void EscalationRB_Checked(object sender, RoutedEventArgs e)
        {
            Escalation.IsChecked = true;
            BED_escalation.Visibility = Visibility.Visible;
            BED_escalation_value.Visibility = Visibility.Visible;
            EQD2_escalation.Visibility = Visibility.Visible;
            EQD2_escalation_value.Visibility = Visibility.Visible;
            total.Visibility = Visibility.Visible;
            dosePerFxEscalation.Visibility = Visibility.Visible;
            TotalDoseEscalation.Visibility = Visibility.Hidden;
            //LinearGradientBrush myBrush = new LinearGradientBrush();
            //myBrush.GradientStops.Add(new GradientStop(Colors.White, 255));
            //dosePerFxEscalation.Background = myBrush;
            //LinearGradientBrush myBrush1 = new LinearGradientBrush();
            //myBrush1.GradientStops.Add(new GradientStop(Colors.Black, 0));
            //total.Foreground = myBrush1;
            dosePerFxEscalation.IsReadOnly = false;
            if (double.TryParse(dosePerFxEscalation.Text, out double dose_perFxEscalation) && int.TryParse(numberOfFractions.Text, out int numFractions) && (dose_perFxEscalation > 0) && (numFractions > 0))
            {
                prescription = Tuple.Create(numFractions, new DoseValue(dose_perFxEscalation, DoseValue.DoseUnit.cGy));
                DPFescalation = dose_perFxEscalation;
                NOF = numFractions;
                double TDescalation = DPFescalation * NOF;
                TotalDoseEscalation.Text = TDescalation.ToString();
                Button_CrtAndCalPlan.IsEnabled = true;

            }
            else
            {
                Button_CrtAndCalPlan.IsEnabled = false;
                return;
            }
            if (double.TryParse(dosePerFxEscalation.Text, out double DPFe) && double.TryParse(alpha_beta.Text, out double aplha_beta_double))
            {
                double TDe = DPFe * NOF;
                double bEDe = (TDe * (1 + (DPFe / aplha_beta_double)));
                double eQD2e = (TDe * ((DPFe + aplha_beta_double) / (2 + aplha_beta_double)));
                EQD2_escalation_value.Text = (Math.Round(eQD2e, 2)).ToString();
                BED_escalation_value.Text = (Math.Round(bEDe, 2)).ToString();
            }
        }

        private void NoEscalation_Checked(object sender, RoutedEventArgs e)
        {
            NoEscalation.IsChecked = true;
            TotalDoseEscalation.Visibility = Visibility.Hidden;
            //LinearGradientBrush myBrush = new LinearGradientBrush();
            //myBrush.GradientStops.Add(new GradientStop(Colors.LightGray, 221));
            //dosePerFxEscalation.Background = myBrush;
            //total.Foreground = myBrush;
            //total.Visibility = Visibility.Hidden;
            dosePerFxEscalation.Visibility = Visibility.Hidden;
            dosePerFxEscalation.IsReadOnly = true;
            dosePerFxEscalation.Text = "";
            TotalDoseEscalation.Text = "";
            BED_escalation.Visibility = Visibility.Hidden;
            BED_escalation_value.Visibility = Visibility.Hidden;
            EQD2_escalation.Visibility = Visibility.Hidden;
            EQD2_escalation_value.Visibility = Visibility.Hidden;

            BED_escalation_value.Text = "";

            EQD2_escalation_value.Text = "";
        }

        private void alpha_beta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(alpha_beta.Text, out double aplha_beta_double))
            {
                double TD = DPF * NOF;
                double bED = (TD * (1 + (DPF / aplha_beta_double)));
                double eQD2 = (TD * ((DPF + aplha_beta_double) / (2 + aplha_beta_double)));
                EQD2.Text = (Math.Round(eQD2, 2)).ToString();
                BED.Text = (Math.Round(bED, 2)).ToString();

                if (double.TryParse(dosePerFxEscalation.Text, out double DPFe) && double.TryParse(alpha_beta.Text, out double test))
                {
                    double TDe = DPFe * NOF;
                    double bEDe = (TDe * (1 + (DPFe / aplha_beta_double)));
                    double eQD2e = (TDe * ((DPFe + aplha_beta_double) / (2 + aplha_beta_double)));
                    EQD2_escalation_value.Text = (Math.Round(eQD2e, 2)).ToString();
                    BED_escalation_value.Text = (Math.Round(bEDe, 2)).ToString();
                }
                else { EQD2_escalation_value.Text = ""; BED_escalation_value.Text = ""; }

            }
            else { EQD2.Text = ""; BED.Text = ""; }

        }

        private void EQD2escalation_value_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EQD2escalation_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BEDescalation_value_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BEDescalation_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NumberOfFractions_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(dosePerFx.Text, out double dose_perFx) && int.TryParse(numberOfFractions.Text, out int numFractions) && (dose_perFx > 0) && (numFractions > 0))
            {
                prescription = Tuple.Create(numFractions, new DoseValue(dose_perFx, DoseValue.DoseUnit.cGy));
                DPF = dose_perFx;
                NOF = numFractions;
                double TD = DPF * NOF;
                DoseToBeDelivered.Text = TD.ToString();
                Button_CrtAndCalPlan.IsEnabled = true;
                linac = LinacID.Text;

                if (double.TryParse(alpha_beta.Text, out double aplha_beta_double1))
                {
                    double bED = (TD * (1 + (DPF / aplha_beta_double1)));
                    double eQD2 = (TD * ((DPF + aplha_beta_double1) / (2 + aplha_beta_double1)));
                    EQD2.Text = (Math.Round(eQD2, 2)).ToString();
                    BED.Text = (Math.Round(bED, 2)).ToString();
                }

                else { EQD2.Text = ""; BED.Text = ""; }


            }

            else
            {
                Button_CrtAndCalPlan.IsEnabled = false;
                return;
            }

            if (Escalation.IsChecked == true)
            {
                if (double.TryParse(dosePerFxEscalation.Text, out double dose_perFxEscalation) && int.TryParse(numberOfFractions.Text, out numFractions) && (dose_perFxEscalation > 0) && (numFractions > 0))
                {
                    BED_escalation.Visibility = Visibility.Visible;
                    BED_escalation_value.Visibility = Visibility.Visible;
                    EQD2_escalation.Visibility = Visibility.Visible;
                    EQD2_escalation_value.Visibility = Visibility.Visible;
                    TotalDoseEscalation.Visibility = Visibility.Visible;
                    prescription = Tuple.Create(numFractions, new DoseValue(dose_perFxEscalation, DoseValue.DoseUnit.cGy));
                    DPFescalation = dose_perFxEscalation;
                    NOF = numFractions;
                    double TDescalation = DPFescalation * NOF;
                    TotalDoseEscalation.Text = TDescalation.ToString();
                    Button_CrtAndCalPlan.IsEnabled = true;

                }
                else
                {
                    Button_CrtAndCalPlan.IsEnabled = false;
                    return;
                }
            }
            if (double.TryParse(dosePerFxEscalation.Text, out double DPFe) && double.TryParse(alpha_beta.Text, out double aplha_beta_double))
            {
                double TDe = DPFe * NOF;
                double bEDe = (TDe * (1 + (DPFe / aplha_beta_double)));
                double eQD2e = (TDe * ((DPFe + aplha_beta_double) / (2 + aplha_beta_double)));
                EQD2_escalation_value.Text = (Math.Round(eQD2e, 2)).ToString();
                BED_escalation_value.Text = (Math.Round(bEDe, 2)).ToString();
            }
            else { EQD2_escalation_value.Text = ""; BED_escalation_value.Text = ""; }
        }
        private void CrtAndCalPlanIMRT(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("Dear {0},\n\n Unfortunately, this module is not applicable for Halcyon MV CBCT machine in Eclipse v15.6\n Please, calculate VMAT instead or make necessary calculations manualy", VMS.TPS.Script.context.CurrentUser), "Oh Dear...", MessageBoxButton.OK, MessageBoxImage.Question);
        }

        private void RapidPlanCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void RapidPlanCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
