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
    /// Interaction logic for SBRTthoraxAutoPlan.xaml
    /// </summary>
    public partial class SBRTthoraxAutoPlan : UserControl
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

        public SBRTthoraxAutoPlan(ScriptContext context)
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
                    const string SCRIPT_NAME = "SBRT_Thorax_NoEscalation";
                    //ID of the course
                    const string COURSE_ID = "Treatment";
                    //ID of the plan
                    string PLAN_ID = string.Format("SBRT_{0}Gy", (Math.Round((NOF * DPF), 0)).ToString());


                    //IDs of fields
                    const string MVCBCT_ID = "MVCBCT";

                    const string BEAM_L_1_ID = "SBRT20-181/20";
                    const string BEAM_L_2_ID = "SBRT181-20/340";
                    const string BEAM_L_3_ID = "SBRT20-181/90";

                    const string BEAM_R_1_ID = "SBRT340-179/20";
                    const string BEAM_R_2_ID = "SBRT179-340/340";
                    const string BEAM_R_3_ID = "SBRT340-179/90";

                    const string BEAM_D_1_ID = "SBRT179-181/20";
                    const string BEAM_D_2_ID = "SBRT181-179/340";
                    const string BEAM_D_3_ID = "SBRT179-181/90";



                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_ID = "PTV";
                    const string OPT_PTV_ID = "PTV_OPT";
                    const string GTVp_ID = "GTVp";
                    const string OPT_GTVp_ID = "GTVp_OPT";
                    const string PTV_GTV_ID = "PTV-GTV";
                    const string PTV_SHELL_ID = "PTV_shell";


                    //IDs of critical structures
                    const string BODY_ID = "BODY";
                    const string SKIN_ID = "Skin_bySCRPT";
                    const string CAUDA_EQUINA_ID = "Cauda equina";
                    const string CAUDA_EQUINA_PRV_ID = "CaudaEq_PRV";
                    const string SACRAL_PLEXUS_ID = "Sacral plexus";
                    const string SACRAL_PLEXUS_PRV_ID = "SacralPl_PRV";
                    const string STOMACH_ID = "Stomach";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SplCrd_PRV";
                    const string MEDULLA_ID = "Medulla";
                    const string SPINALCORD_SUB_VOLUME_ID = "SpinalCrd Subvol";
                    const string RIB_ID = "Rib";
                    const string DUODENUM_ID = "Duodenum";
                    const string LUNG_R_ID = "Lung_R";
                    const string LUNG_L_Id = "Lung_L";
                    const string LUNGS_ID = "Lungs";
                    const string LUNGS_PTV_ID = "Lungs-PTV";
                    const string LIVER_ID = "Liver";
                    const string BOTH_LUNGS_ID = "Both Lungs";
                    const string HEART_ID = "Heart/prcrdm";
                    const string GREAT_VESSELS_ID = "Great Vessels";
                    const string TRACHEA_ID = "Trachea";
                    const string LARGE_BRONCHUS_ID = "Large Bronchus";
                    const string BRNHS_SMALL_AIR_WAYS_ID = "BrnchsSmrAirWays";
                    const string ESOPHAGUS_ID = "Esophagus";



                    //Show greetings window
                    string Greeting = string.Format("Greetings {0}! Please, ensure that the structures 'PTV', 'GTVp', 'PTV-GTV' exist in the currest structure set. Script is made by 'PET_Tehnology'. Before implementing this script, please contact to Daniel Peidus (d.peidus@pet-net.ru).", user);
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

                    //find BODY
                    Structure body = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (body == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    //find Cauda Equina
                    Structure cauda_equina = ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_ID);
                    if (cauda_equina == null)
                    {
                        NOT_FOUND_list.Add(CAUDA_EQUINA_ID);
                        try { cauda_equina = ss.AddStructure("Avoidance", CAUDA_EQUINA_ID); } catch { }
                    }

                    //find Sacral Plexus
                    Structure sacral_plexus = ss.Structures.FirstOrDefault(x => x.Id == SACRAL_PLEXUS_ID);
                    if (sacral_plexus == null)
                    {
                        NOT_FOUND_list.Add(SACRAL_PLEXUS_ID);
                        try { sacral_plexus = ss.AddStructure("Avoidance", SACRAL_PLEXUS_ID); } catch { }
                    }

                    //find Stomach
                    Structure stomach = ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID);
                    if (stomach == null)
                    {
                        NOT_FOUND_list.Add(STOMACH_ID);
                        try { stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinal_cord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinal_cord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try { spinal_cord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Medulla
                    Structure medulla = ss.Structures.FirstOrDefault(x => x.Id == MEDULLA_ID);
                    if (medulla == null)
                    {
                        NOT_FOUND_list.Add(MEDULLA_ID);
                        try { medulla = ss.AddStructure("Avoidance", MEDULLA_ID); } catch { }
                    }

                    //find SpinalCrd Subvol
                    Structure spinalcrd_subvol = ss.Structures.FirstOrDefault(x => x.Id == SPINALCORD_SUB_VOLUME_ID);
                    if (spinalcrd_subvol == null)
                    {
                        NOT_FOUND_list.Add(SPINALCORD_SUB_VOLUME_ID);
                        try { spinalcrd_subvol = ss.AddStructure("Avoidance", SPINALCORD_SUB_VOLUME_ID); } catch { }
                    }

                    //find Rib
                    Structure rib = ss.Structures.FirstOrDefault(x => x.Id == RIB_ID);
                    if (rib == null)
                    {
                        NOT_FOUND_list.Add(RIB_ID);
                        try { rib = ss.AddStructure("Avoidance", RIB_ID); } catch { }
                    }

                    //find Duodenum
                    Structure duodenum = ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID);
                    if (duodenum == null)
                    {
                        NOT_FOUND_list.Add(DUODENUM_ID);
                        try { duodenum = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }

                    //find Lung_L
                    Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_Id);
                    if (lung_L == null)
                    {
                        NOT_FOUND_list.Add(LUNG_L_Id);
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_Id); } catch { }
                    }

                    //find Lung_R
                    Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                    if (lung_R == null)
                    {
                        NOT_FOUND_list.Add(LUNG_R_ID);
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }

                    //find Liver
                    Structure liver = ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID);
                    if (liver == null)
                    {
                        NOT_FOUND_list.Add(LIVER_ID);
                        try { liver = ss.AddStructure("Avoidance", LIVER_ID); } catch { }
                    }

                    //find BothLungs
                    Structure both_lungs = ss.Structures.FirstOrDefault(x => x.Id == BOTH_LUNGS_ID);
                    if (both_lungs == null)
                    {
                        NOT_FOUND_list.Add(BOTH_LUNGS_ID);
                        try { both_lungs = ss.AddStructure("Avoidance", BOTH_LUNGS_ID); } catch { }
                    }

                    //find Heart
                    Structure heart = ss.Structures.FirstOrDefault(x => x.Id == HEART_ID);
                    if (heart == null)
                    {
                        NOT_FOUND_list.Add(HEART_ID);
                        try { heart = ss.AddStructure("Avoidance", HEART_ID); } catch { }
                    }

                    //find GreatVessels
                    Structure great_vessels = ss.Structures.FirstOrDefault(x => x.Id == GREAT_VESSELS_ID);
                    if (great_vessels == null)
                    {
                        NOT_FOUND_list.Add(GREAT_VESSELS_ID);
                        try { great_vessels = ss.AddStructure("Avoidance", GREAT_VESSELS_ID); } catch { }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find LargeBronchus
                    Structure large_brochus = ss.Structures.FirstOrDefault(x => x.Id == LARGE_BRONCHUS_ID);
                    if (large_brochus == null)
                    {
                        NOT_FOUND_list.Add(LARGE_BRONCHUS_ID);
                        try { large_brochus = ss.AddStructure("Avoidance", LARGE_BRONCHUS_ID); } catch { }
                    }

                    //find BrnchsSmrAirWays
                    Structure brnchssmrairways = ss.Structures.FirstOrDefault(x => x.Id == BRNHS_SMALL_AIR_WAYS_ID);
                    if (brnchssmrairways == null)
                    {
                        NOT_FOUND_list.Add(BRNHS_SMALL_AIR_WAYS_ID);
                        try { brnchssmrairways = ss.AddStructure("Avoidance", BRNHS_SMALL_AIR_WAYS_ID); } catch { }
                    }

                    //find SpinalCord PRV
                    Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spinal_cord_prv == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_PRV_ID);
                        try { spinal_cord_prv = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); } catch { }
                    }

                    //find Skin
                    Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                    if (skin == null)
                    {
                        NOT_FOUND_list.Add(SKIN_ID);
                        try { skin = ss.AddStructure("Avoidance", SKIN_ID); } catch { }
                    }

                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV and GTV structures in the structure set. If PTV and GTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null || (ptv.Volume < 0.01))
                    {
                        MessageBox.Show(string.Format("'{0}' not found or null volume!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        try { ptv = ss.AddStructure("Avoidance", PTV_ID); }
                        catch { }
                        Window.GetWindow(this).Close(); return;
                    }

                    Structure gtv = ss.Structures.FirstOrDefault(x => x.Id == GTVp_ID);
                    if (gtv == null || (gtv.Volume < 0.01))
                    {

                        NOT_FOUND_list.Add(GTVp_ID);
                    }

                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);
                    if (ptv_opt == null || (ptv_opt.Volume < 0.01))
                    {
                        MessageBox.Show(string.Format("{0} not found!", OPT_PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTVp_ID);
                    if (gtv_opt == null || (gtv_opt.Volume < 0.01))
                    {

                        NOT_FOUND_list.Add(OPT_GTVp_ID);
                    }

                    Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                    if (ptv_gtv == null && (ptv_gtv.Volume < 0.01))
                    {

                        NOT_FOUND_list.Add(PTV_GTV_ID);
                    }

                    Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);
                    if (ptv_shell == null && (ptv_shell.Volume < 0.01))
                    {

                        NOT_FOUND_list.Add(PTV_SHELL_ID);
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
                    if (ss.Id != "SIM_Lung_L" && ss.Id != "SIM_Lung_R")
                    {
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
                        catch { MessageBox.Show(string.Format("Could not place eams for some reason...."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }


                    //Setup for left-side localizations
                    if (ss.Id == "SIM_Lung_L")
                    {

                        Beam beam1_L = plan.Beams.FirstOrDefault(x => x.Id == BEAM_L_1_ID);
                        if (beam1_L == null)
                        {
                            beam1_L = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 340, 179, GantryDirection.Clockwise, 0, isocenter);
                            beam1_L.Id = BEAM_L_1_ID;
                        }

                        Beam beam2_L = plan.Beams.FirstOrDefault(x => x.Id == BEAM_L_2_ID);
                        if (beam2_L == null)
                        {
                            beam2_L = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 179, 340, GantryDirection.CounterClockwise, 0, isocenter);
                            beam2_L.Id = BEAM_L_2_ID;
                        }
                        /*
                        Beam beam3_L = plan.Beams.FirstOrDefault(x=>x.Id == BEAM_L_3_ID);
                        if (beam3_L == null)
                        {
                            beam3_L = plan.AddArcBeam(MachineParameters, new VRect<double>(280,280,280,280), 90, 340, 179, GantryDirection.Clockwise, 0, isocenter);
                            beam3_L.Id = BEAM_L_3_ID;
                        } 
                        */

                        //Fit collimator to structure. For Halcyon machines the next 5 strings are not necessary
                        try
                        {
                            bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                            beam1_L.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            beam2_L.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            //beam3_L.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                        }
                        catch { MessageBox.Show(string.Format("Could not place eams for some reason...."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                        FitToStructureMargins margins = new FitToStructureMargins(0);
                    }


                    //Setup for right-side localizations
                    if (ss.Id == "SIM_Lung_R")
                    {

                        Beam beam1_R = plan.Beams.FirstOrDefault(x => x.Id == BEAM_R_1_ID);
                        if (beam1_R == null)
                        {
                            beam1_R = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 20, 181, GantryDirection.CounterClockwise, 0, isocenter);
                            beam1_R.Id = BEAM_R_1_ID;
                        }

                        Beam beam2_R = plan.Beams.FirstOrDefault(x => x.Id == BEAM_R_2_ID);
                        if (beam2_R == null)
                        {
                            beam2_R = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 181, 20, GantryDirection.Clockwise, 0, isocenter);
                            beam2_R.Id = BEAM_R_2_ID;
                        }
                        /*
                        Beam beam3_R = plan.Beams.FirstOrDefault(x=>x.Id == BEAM_R_3_ID);
                        if (beam3_R == null)
                        {
                            beam3_R = plan.AddArcBeam(MachineParameters, new VRect<double>(280,280,280,280), 90, 20, 181, GantryDirection.CounterClockwise, 0, isocenter);
                            beam3_R.Id = BEAM_R_3_ID;
                        } 
                        */
                        try
                        {
                            //Fit collimator to structure. For Halcyon machines the next 5 strings are not necessary
                            bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                            beam1_R.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            beam2_R.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            //beam3_R.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                        }
                        catch { MessageBox.Show(string.Format("Could not place eams for some reason...."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                        FitToStructureMargins margins = new FitToStructureMargins(0);
                    }

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
                    plan.OptimizationSetup.AddNormalTissueObjective(120, 1, 100, 45, 0.12);


                    //Set optimization adjustments for PTVs
                    GreenBar.Value = 30;

                    text.Text = "Completed...";
                    GreenBarListBox.Items.Add(text.Text);
                     
                    text.Text = "Defining objectives...";
                    GreenBarListBox.Items.Add(text.Text);
                     

                    
                    double target_lower = Math.Round((DPF * NOF), 2);
                    double target_upper = Math.Round(((DPF * NOF) * 1.05), 2);

                if (RapidPlan.IsChecked==true) 
                {
                    //Choose estimation model ID
                    string DVHestimationModelId = "SBRT Abdominal 3fx";

                    if (NOF >= 4) { DVHestimationModelId = "SBRT Abdominal 5fx"; }

                    

                    //Choose target dose level. Due to the reason that no escalation is checked, we will put the same dose target leves for PTV and GTV
                    Dictionary<string, DoseValue> targetDoseLevels = new Dictionary<string, DoseValue>();

                    try { targetDoseLevels.Add("PTV_OPT", plan.TotalDose); } catch { }
                    try { targetDoseLevels.Add("PTV", plan.TotalDose); } catch { }
                    try { targetDoseLevels.Add("PTV-GTV", plan.TotalDose); } catch { }
                    try { targetDoseLevels.Add("GTV_OPT", plan.TotalDose); } catch { }
                    try { targetDoseLevels.Add("GTVp", plan.TotalDose); } catch { }

                    //Match structures from the structure set to structurel listed in DVHestimationModel 
                    Dictionary<string, string> structureMatches = new Dictionary<string, string>();
                    try { structureMatches.Add(PTV_ID, "PTV"); } catch { }
                    try { structureMatches.Add(OPT_PTV_ID, "PTV_OPT"); } catch { }
                    try { structureMatches.Add(GTVp_ID, "GTVp"); } catch { }
                    try { structureMatches.Add(OPT_GTVp_ID, "GTV_OPT"); } catch { }
                    try { structureMatches.Add(PTV_GTV_ID, "PTV-GTV"); } catch { }



                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BODY")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BODY").Id, "BODY"); } } catch { }

                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_ID).Id, "Cauda equina"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_PRV_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_PRV_ID).Id, "Cd_qn_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID).Id, "Duodenum"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID).Id, "Esophagus"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == HEART_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == HEART_ID).Id, "Heart/prcrdm"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID).Id, "Liver"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID).Id, "Lungs"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == RIB_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == RIB_ID).Id, "Rib"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Skin")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Skin").Id, "Skin"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord").Id, "SpinalCord"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV").Id, "SpnlCrd_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID).Id, "Stomach"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID).Id, "Trachea"); } } catch { }

                    plan.CalculateDVHEstimates(DVHestimationModelId, targetDoseLevels, structureMatches);
                }
                else {
                    //PTV
                    if (ptv != null & (ptv.Volume > 0.1))
                    {
                        try
                        {
                            var ptv_lower = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                            var ptv_upper = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);

                            if (ptv_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                            }
                            if (ptv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Could not define optimization objectives for:'{0}'....", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
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
                        catch { MessageBox.Show(string.Format("Could not define optimization objectives for:'{0}'....", GTVp_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }
                    if (ptv_opt != null & (ptv_opt.Volume > 0.1))
                    {
                        try
                        {
                            var opt_ptv_lower1 = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            var opt_ptv_upper1 = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 150);

                            if (opt_ptv_lower1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            }
                            if (opt_ptv_upper1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 150);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Could not define optimization objectives for:'{0}'....", OPT_PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
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
                        catch { MessageBox.Show(string.Format("Could not define optimization objectives for:'{0}'....", OPT_GTVp_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    }

                    if (ptv_gtv != null & (ptv_gtv.Volume > 0.01))
                    {
                        try
                        {
                            var ptv_gtv_upper = plan.OptimizationSetup.AddPointObjective(ptv_gtv, OptimizationObjectiveOperator.Upper, new DoseValue((target_upper + 0.5), "Gy"), 0, 100);


                            if (ptv_gtv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_gtv, OptimizationObjectiveOperator.Upper, new DoseValue((target_upper + 0.5), "Gy"), 0, 100);
                            }
                        }
                        catch { MessageBox.Show(string.Format("Could not define optimization objectives for:'{0}'....", PTV_GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    //Set optimization adjustments for OARs and optimization strucures

                    //Esophagus
                    if (esophagus != null & (esophagus.Volume > 0.01))
                    {
                        try
                        {
                            var esophagus_upper = plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);

                            if (esophagus_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);
                            }
                        }
                        catch { }
                    }

                    //Heart
                    if (heart != null & (heart.Volume > 0.01))
                    {
                        try {
                            var heart_upper = plan.OptimizationSetup.AddPointObjective(heart, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);

                            if (heart_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(heart, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);
                            }
                        }
                        catch { }
                    }

                    //Trachea

                    if (trachea != null & (trachea.Volume > 0.01))
                    {
                        try {
                            var trachea_upper = plan.OptimizationSetup.AddPointObjective(trachea, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);

                            if (trachea_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(trachea, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);
                            }
                        }
                        catch { }
                    }


                    //Set optimization adjustments for OARs and optimization strucures


                    //SpinalCord PRV
                    if (spinal_cord_prv != null & (spinal_cord_prv.Volume > 0.1))
                    {
                        try {

                            var spinalcord_upper = plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(12, "Gy"), 0, 150);


                            if (spinalcord_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(12, "Gy"), 0, 150);
                            }
                        }
                        catch { }

                    }

                    //Skin
                    if (skin != null & (skin.Volume > 0.1))
                    {
                        try {

                            var skin_upper = plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                            if (skin_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                            }
                        }
                        catch { }


                    }

                    //Stomach
                    if (stomach != null & (stomach.Volume > 0.1))
                    {
                        try {

                            var stomach_upper = plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                            if (stomach_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                            }
                        }
                        catch { }


                    }

                    //Rib
                    if (rib != null & (rib.Volume > 0.1))
                    {
                        try {

                            var rib_upper = plan.OptimizationSetup.AddPointObjective(rib, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                            if (rib_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(rib, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                            }
                        }
                        catch { }


                    }


                    //Cauda equina
                    if (cauda_equina != null & (cauda_equina.Volume > 0.1))
                    {
                        try {

                            var cauda_equina_upper = plan.OptimizationSetup.AddPointObjective(cauda_equina, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                            if (cauda_equina_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(cauda_equina, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                            }
                        }
                        catch { }

                    }


                    //Sacral plexus
                    if (sacral_plexus != null & (sacral_plexus.Volume > 0.1))
                    {
                        try {

                            var sacral_plexus_upper = plan.OptimizationSetup.AddPointObjective(sacral_plexus, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                            if (sacral_plexus_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(sacral_plexus, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                            }
                        }
                        catch { }

                    }


                    //Duodenum
                    if (duodenum != null & (duodenum.Volume > 0.1))
                    {
                        try {

                            var duodenum_upper = plan.OptimizationSetup.AddPointObjective(duodenum, OptimizationObjectiveOperator.Upper, new DoseValue(18, "Gy"), 0, 100);

                            if (duodenum_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(duodenum, OptimizationObjectiveOperator.Upper, new DoseValue(18, "Gy"), 0, 100);
                            }
                        }
                        catch { }

                    }

                    //Lungs-PTV
                    if (both_lungs != null & (both_lungs.Volume > 0.1))
                    {
                        try {


                            var both_lungs_mean = plan.OptimizationSetup.AddMeanDoseObjective(both_lungs, new DoseValue(8, "Gy"), 80);

                            if (both_lungs_mean == null)
                            {
                                both_lungs_mean = plan.OptimizationSetup.AddMeanDoseObjective(both_lungs, new DoseValue(8, "Gy"), 80);
                            }
                        }
                        catch { }

                    }


                    //Global maximum limit
                    //PharConst-PTV
                    if (body != null & (body.Volume > 0.1))
                    {
                        try {
                            double body_upper_dose = Math.Round(((DPF * NOF) * 1.2), 2);
                            var body_upper = plan.OptimizationSetup.AddPointObjective(body, OptimizationObjectiveOperator.Upper, new DoseValue(body_upper_dose, "Gy"), 0, 80);
                            if (body_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(body, OptimizationObjectiveOperator.Upper, new DoseValue(body_upper_dose, "Gy"), 0, 80);
                                MessageBox.Show(string.Format("Please, define avoidance structures manually, if necessary. To continue with calculations, launch this script again"));
                                Window.GetWindow(this).Close(); return;
                            }
                        }
                        catch { }
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
                    
                    try {
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
                        var ptv_DVH = plan.GetDoseAtVolume(ptv_opt, 98.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                        var cauda_equina_MAX = plan.GetDoseAtVolume(cauda_equina, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var sacral_plexus_MAX = plan.GetDoseAtVolume(sacral_plexus, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var spinal_cord_MAX = plan.GetDoseAtVolume(spinal_cord_prv, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var skin_MAX = plan.GetDoseAtVolume(skin, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var duodenum_MAX = plan.GetDoseAtVolume(duodenum, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var stomach_MAX = plan.GetDoseAtVolume(stomach, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var heart_MAX = plan.GetDoseAtVolume(heart, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var trachea_MAX = plan.GetDoseAtVolume(trachea, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                        var esophagus_MAX = plan.GetDoseAtVolume(esophagus, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);


                        //Show report message
                        var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                        MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! Plan calculation took '{0}' minutes\n 98% '{1}' covered by '{2}' Gy\n '{3}' max point dose = '{4}'Gy\n '{5}' max point dose = '{6}'Gy\n '{7}' max point dose = '{8}'Gy\n '{9}' max point dose = '{10}'Gy\n '{11}' max point dose = '{12}'Gy\n '{13}' max point dose = '{14}'Gy\n '{15}' max point dose = '{16}'Gy\n '{17}' max point dose = '{18}'Gy\n '{19}' max point dose = '{20}'Gy",


                            overall_calculation_time,
                            ptv_opt.Id, ptv_DVH,
                            cauda_equina.Id, cauda_equina_MAX,
                            sacral_plexus.Id, sacral_plexus_MAX,
                            spinal_cord_prv.Id, spinal_cord_MAX,
                            skin.Id, skin_MAX,
                            duodenum.Id, duodenum_MAX,
                            stomach.Id, stomach_MAX,
                            heart.Id, heart_MAX,
                            trachea.Id, trachea_MAX,
                            esophagus.Id, esophagus_MAX
                            ),


                            SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }
                if (Escalation.IsChecked == true)
                {

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
                        const string SCRIPT_NAME = "SBRT_Thorax_Escalation";
                        //ID of the course
                        const string COURSE_ID = "Treatment";
                        //ID of the plan
                        string PLAN_ID = string.Format("SBRT{0}/{1}Gy", (Math.Round((NOF * DPF), 0)).ToString(), (Math.Round((NOF * DPFescalation), 0)).ToString());


                        //IDs of fields
                        const string MVCBCT_ID = "MVCBCT";

                        const string BEAM_L_1_ID = "SBRT20-181/20";
                        const string BEAM_L_2_ID = "SBRT181-20/340";
                        const string BEAM_L_3_ID = "SBRT20-181/90";

                        const string BEAM_R_1_ID = "SBRT340-179/20";
                        const string BEAM_R_2_ID = "SBRT179-340/340";
                        const string BEAM_R_3_ID = "SBRT340-179/90";

                        const string BEAM_D_1_ID = "SBRT179-181/20";
                        const string BEAM_D_2_ID = "SBRT181-179/340";
                        const string BEAM_D_3_ID = "SBRT179-181/90";



                        //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                        const string PTV_ID = "PTV";
                        const string OPT_PTV_ID = "PTV_OPT";
                        const string GTVp_ID = "GTVp";
                        const string OPT_GTVp_ID = "GTVp_OPT";
                        const string PTV_GTV_ID = "PTV-GTV";
                        const string PTV_SHELL_ID = "PTV_shell";


                        //IDs of critical structures
                        const string BODY_ID = "BODY";
                        const string SKIN_ID = "Skin_bySCRPT";
                        const string CAUDA_EQUINA_ID = "Cauda equina";
                        const string CAUDA_EQUINA_PRV_ID = "CaudaEq_PRV";
                        const string SACRAL_PLEXUS_ID = "Sacral plexus";
                        const string SACRAL_PLEXUS_PRV_ID = "SacralPl_PRV";
                        const string STOMACH_ID = "Stomach";
                        const string SPINAL_CORD_ID = "SpinalCord";
                        const string SPINAL_CORD_PRV_ID = "SplCrd_PRV";
                        const string MEDULLA_ID = "Medulla";
                        const string SPINALCORD_SUB_VOLUME_ID = "SpinalCrd Subvol";
                        const string RIB_ID = "Rib";
                        const string DUODENUM_ID = "Duodenum";
                        const string LUNG_R_ID = "Lung_R";
                        const string LUNG_L_Id = "Lung_L";
                        const string LUNGS_ID = "Lungs";
                        const string LUNGS_PTV_ID = "Lungs-PTV";
                        const string LIVER_ID = "Liver";
                        const string BOTH_LUNGS_ID = "Both Lungs";
                        const string HEART_ID = "Heart/prcrdm";
                        const string GREAT_VESSELS_ID = "Great Vessels";
                        const string TRACHEA_ID = "Trachea";
                        const string LARGE_BRONCHUS_ID = "Large Bronchus";
                        const string BRNHS_SMALL_AIR_WAYS_ID = "BrnchsSmrAirWays";
                        const string ESOPHAGUS_ID = "Esophagus";



                        //Show greetings window
                        string Greeting = string.Format("Greetings {0}! Please, ensure that the structures 'PTV', 'GTVp', 'PTV-GTV' exist in the currest structure set. Script is made by 'PET_Tehnology'. Before implementing this script, please contact to Daniel Peidus (d.peidus@pet-net.ru).", user);
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

                        //find BODY
                        Structure body = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                        if (body == null)
                        {
                            MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                            Window.GetWindow(this).Close(); return;
                        }

                        //find Cauda Equina
                        Structure cauda_equina = ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_ID);
                        if (cauda_equina == null)
                        {
                            NOT_FOUND_list.Add(CAUDA_EQUINA_ID);
                            try { cauda_equina = ss.AddStructure("Avoidance", CAUDA_EQUINA_ID); } catch { }
                        }

                        //find Sacral Plexus
                        Structure sacral_plexus = ss.Structures.FirstOrDefault(x => x.Id == SACRAL_PLEXUS_ID);
                        if (sacral_plexus == null)
                        {
                            NOT_FOUND_list.Add(SACRAL_PLEXUS_ID);
                            try { sacral_plexus = ss.AddStructure("Avoidance", SACRAL_PLEXUS_ID); } catch { }
                        }

                        //find Stomach
                        Structure stomach = ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID);
                        if (stomach == null)
                        {
                            NOT_FOUND_list.Add(STOMACH_ID);
                            try { stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                        }

                        //find SpinalCord
                        Structure spinal_cord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                        if (spinal_cord == null)
                        {
                            NOT_FOUND_list.Add(SPINAL_CORD_ID);
                            try { spinal_cord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                        }

                        //find Medulla
                        Structure medulla = ss.Structures.FirstOrDefault(x => x.Id == MEDULLA_ID);
                        if (medulla == null)
                        {
                            NOT_FOUND_list.Add(MEDULLA_ID);
                            try { medulla = ss.AddStructure("Avoidance", MEDULLA_ID); } catch { }
                        }

                        //find SpinalCrd Subvol
                        Structure spinalcrd_subvol = ss.Structures.FirstOrDefault(x => x.Id == SPINALCORD_SUB_VOLUME_ID);
                        if (spinalcrd_subvol == null)
                        {
                            NOT_FOUND_list.Add(SPINALCORD_SUB_VOLUME_ID);
                            try { spinalcrd_subvol = ss.AddStructure("Avoidance", SPINALCORD_SUB_VOLUME_ID); } catch { }
                        }

                        //find Rib
                        Structure rib = ss.Structures.FirstOrDefault(x => x.Id == RIB_ID);
                        if (rib == null)
                        {
                            NOT_FOUND_list.Add(RIB_ID);
                            rib = ss.AddStructure("Avoidance", RIB_ID);
                        }

                        //find Duodenum
                        Structure duodenum = ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID);
                        if (duodenum == null)
                        {
                            NOT_FOUND_list.Add(DUODENUM_ID);
                            duodenum = ss.AddStructure("Avoidance", DUODENUM_ID);
                        }

                        //find Lung_L
                        Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_Id);
                        if (lung_L == null)
                        {
                            NOT_FOUND_list.Add(LUNG_L_Id);
                            lung_L = ss.AddStructure("Avoidance", LUNG_L_Id);
                        }

                        //find Lung_R
                        Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                        if (lung_R == null)
                        {
                            NOT_FOUND_list.Add(LUNG_R_ID);
                            lung_R = ss.AddStructure("Avoidance", LUNG_R_ID);
                        }

                        //find Liver
                        Structure liver = ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID);
                        if (liver == null)
                        {
                            NOT_FOUND_list.Add(LIVER_ID);
                            liver = ss.AddStructure("Avoidance", LIVER_ID);
                        }

                        //find BothLungs
                        Structure both_lungs = ss.Structures.FirstOrDefault(x => x.Id == BOTH_LUNGS_ID);
                        if (both_lungs == null)
                        {
                            NOT_FOUND_list.Add(BOTH_LUNGS_ID);
                            both_lungs = ss.AddStructure("Avoidance", BOTH_LUNGS_ID);
                        }

                        //find Heart
                        Structure heart = ss.Structures.FirstOrDefault(x => x.Id == HEART_ID);
                        if (heart == null)
                        {
                            NOT_FOUND_list.Add(HEART_ID);
                            heart = ss.AddStructure("Avoidance", HEART_ID);
                        }

                        //find GreatVessels
                        Structure great_vessels = ss.Structures.FirstOrDefault(x => x.Id == GREAT_VESSELS_ID);
                        if (great_vessels == null)
                        {
                            NOT_FOUND_list.Add(GREAT_VESSELS_ID);
                            great_vessels = ss.AddStructure("Avoidance", GREAT_VESSELS_ID);
                        }

                        //find Trachea
                        Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                        if (trachea == null)
                        {
                            NOT_FOUND_list.Add(TRACHEA_ID);
                            trachea = ss.AddStructure("Avoidance", TRACHEA_ID);
                        }

                        //find LargeBronchus
                        Structure large_brochus = ss.Structures.FirstOrDefault(x => x.Id == LARGE_BRONCHUS_ID);
                        if (large_brochus == null)
                        {
                            NOT_FOUND_list.Add(LARGE_BRONCHUS_ID);
                            large_brochus = ss.AddStructure("Avoidance", LARGE_BRONCHUS_ID);
                        }

                        //find BrnchsSmrAirWays
                        Structure brnchssmrairways = ss.Structures.FirstOrDefault(x => x.Id == BRNHS_SMALL_AIR_WAYS_ID);
                        if (brnchssmrairways == null)
                        {
                            NOT_FOUND_list.Add(BRNHS_SMALL_AIR_WAYS_ID);
                            brnchssmrairways = ss.AddStructure("Avoidance", BRNHS_SMALL_AIR_WAYS_ID);
                        }

                        //find SpinalCord PRV
                        Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                        if (spinal_cord_prv == null)
                        {
                            NOT_FOUND_list.Add(SPINAL_CORD_PRV_ID);
                            spinal_cord_prv = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID);
                        }

                        //find Skin
                        Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                        if (skin == null)
                        {
                            NOT_FOUND_list.Add(SKIN_ID);
                            skin = ss.AddStructure("Avoidance", SKIN_ID);
                        }

                        //find Esophagus
                        Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                        if (esophagus == null)
                        {
                            NOT_FOUND_list.Add(ESOPHAGUS_ID);
                            esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID);
                        }
                        //===========================================================================================================================================================================
                        //===========================================================================================================================================================================


                        // find PTV and GTV structures in the structure set. If PTV and GTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                        Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                        if (ptv == null || (ptv.Volume < 0.01))
                        {
                            MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                            ptv = ss.AddStructure("Avoidance", PTV_ID);
                            Window.GetWindow(this).Close(); return;
                        }

                        Structure gtv = ss.Structures.FirstOrDefault(x => x.Id == GTVp_ID);
                        if (gtv == null || (gtv.Volume < 0.01))
                        {

                            NOT_FOUND_list.Add(GTVp_ID);
                        }

                        Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);
                        if (ptv_opt == null || (ptv_opt.Volume < 0.01))
                        {
                            MessageBox.Show(string.Format("{0} not found!", OPT_PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                            Window.GetWindow(this).Close(); return;
                        }

                        Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTVp_ID);
                        if (gtv_opt == null || (gtv_opt.Volume < 0.01))
                        {

                            NOT_FOUND_list.Add(OPT_GTVp_ID);
                        }

                        Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                        if (ptv_gtv == null && (ptv_gtv.Volume < 0.01))
                        {

                            NOT_FOUND_list.Add(PTV_GTV_ID);
                        }

                        Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);
                        if (ptv_shell == null && (ptv_shell.Volume < 0.01))
                        {

                            NOT_FOUND_list.Add(PTV_SHELL_ID);
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
                        if (ss.Id != "SIM_Lung_L" && ss.Id != "SIM_Lung_R")
                        {
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
                            bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                            beam1_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            beam2_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            //beam3_D.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            FitToStructureMargins margins = new FitToStructureMargins(0);
                        }


                        //Setup for left-side localizations
                        if (ss.Id == "SIM_Lung_L")
                        {

                            Beam beam1_L = plan.Beams.FirstOrDefault(x => x.Id == BEAM_L_1_ID);
                            if (beam1_L == null)
                            {
                                beam1_L = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 340, 179, GantryDirection.Clockwise, 0, isocenter);
                                beam1_L.Id = BEAM_L_1_ID;
                            }

                            Beam beam2_L = plan.Beams.FirstOrDefault(x => x.Id == BEAM_L_2_ID);
                            if (beam2_L == null)
                            {
                                beam2_L = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 179, 340, GantryDirection.CounterClockwise, 0, isocenter);
                                beam2_L.Id = BEAM_L_2_ID;
                            }
                            /*
                            Beam beam3_L = plan.Beams.FirstOrDefault(x=>x.Id == BEAM_L_3_ID);
                            if (beam3_L == null)
                            {
                                beam3_L = plan.AddArcBeam(MachineParameters, new VRect<double>(280,280,280,280), 90, 340, 179, GantryDirection.Clockwise, 0, isocenter);
                                beam3_L.Id = BEAM_L_3_ID;
                            } 
                            */

                            //Fit collimator to structure. For Halcyon machines the next 5 strings are not necessary
                            bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                            beam1_L.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            beam2_L.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            //beam3_L.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            FitToStructureMargins margins = new FitToStructureMargins(0);
                        }


                        //Setup for right-side localizations
                        if (ss.Id == "SIM_Lung_R")
                        {

                            Beam beam1_R = plan.Beams.FirstOrDefault(x => x.Id == BEAM_R_1_ID);
                            if (beam1_R == null)
                            {
                                beam1_R = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 20, 181, GantryDirection.CounterClockwise, 0, isocenter);
                                beam1_R.Id = BEAM_R_1_ID;
                            }

                            Beam beam2_R = plan.Beams.FirstOrDefault(x => x.Id == BEAM_R_2_ID);
                            if (beam2_R == null)
                            {
                                beam2_R = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 181, 20, GantryDirection.Clockwise, 0, isocenter);
                                beam2_R.Id = BEAM_R_2_ID;
                            }
                            /*
                            Beam beam3_R = plan.Beams.FirstOrDefault(x=>x.Id == BEAM_R_3_ID);
                            if (beam3_R == null)
                            {
                                beam3_R = plan.AddArcBeam(MachineParameters, new VRect<double>(280,280,280,280), 90, 20, 181, GantryDirection.CounterClockwise, 0, isocenter);
                                beam3_R.Id = BEAM_R_3_ID;
                            } 
                            */

                            //Fit collimator to structure. For Halcyon machines the next 5 strings are not necessary
                            bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                            beam1_R.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            beam2_R.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                            //beam3_R.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);

                            FitToStructureMargins margins = new FitToStructureMargins(0);
                        }

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
                        plan.OptimizationSetup.AddNormalTissueObjective(120, 1, 100, 45, 0.12);


                        //Set optimization adjustments for PTVs
                        GreenBar.Value = 30;

                        text.Text = "Completed...";
                        GreenBarListBox.Items.Add(text.Text);
                         
                        text.Text = "Defining objectives...";
                        GreenBarListBox.Items.Add(text.Text);
                         

                        
                        double target_lower = Math.Round((DPF * NOF), 2);
                        double target_upper = Math.Round(((DPF * NOF) * 1.05), 2);
                        double target_escalation_lower = Math.Round((DPFescalation * NOF), 2);
                        double targer_escalation_upper = Math.Round(((DPF * NOF) * 1.20), 2);

                    if (RapidPlan.IsChecked == true) 
                    {
                        //Choose estimation model ID
                        string DVHestimationModelId = "SBRT Abdominal 3fx";
                        
                        if (NOF >= 4) { DVHestimationModelId = "SBRT Abdominal 5fx"; }

                        DoseValue escalationDose = new DoseValue((target_escalation_lower + 0.5), DoseValue.DoseUnit.Gy);

                        //Choose target dose level. Due to the reason that no escalation is checked, we will put the same dose target leves for PTV and GTV
                        Dictionary<string, DoseValue> targetDoseLevels = new Dictionary<string, DoseValue>();

                        try { targetDoseLevels.Add("PTV_OPT", plan.TotalDose); } catch { }
                        try { targetDoseLevels.Add("PTV", plan.TotalDose); } catch { }                    
                        try { targetDoseLevels.Add("PTV-GTV", plan.TotalDose); } catch { }
                        try { targetDoseLevels.Add("GTV_OPT", escalationDose); } catch { }
                        try { targetDoseLevels.Add("GTVp", escalationDose); } catch { }

                        //Match structures from the structure set to structurel listed in DVHestimationModel 
                        Dictionary<string, string> structureMatches = new Dictionary<string, string>();
                        try { structureMatches.Add(PTV_ID, "PTV"); } catch { }
                        try { structureMatches.Add(OPT_PTV_ID, "PTV_OPT"); } catch { }
                        try { structureMatches.Add(GTVp_ID, "GTVp"); } catch { }
                        try { structureMatches.Add(OPT_GTVp_ID, "GTV_OPT"); } catch { }
                        try { structureMatches.Add(PTV_GTV_ID, "PTV-GTV"); } catch { }



                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BODY")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BODY").Id, "BODY"); } } catch { }
                        
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_ID).Id, "Cauda equina"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_PRV_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_PRV_ID).Id, "Cd_qn_PRV"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID).Id, "Duodenum"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID).Id, "Esophagus"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == HEART_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == HEART_ID).Id, "Heart/prcrdm"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID).Id, "Liver"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID).Id, "Lungs"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == RIB_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == RIB_ID).Id, "Rib"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Skin")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Skin").Id, "Skin"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord").Id, "SpinalCord"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV").Id, "SpnlCrd_PRV"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID).Id, "Stomach"); } } catch { }
                        try { if ((ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID)).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID).Id, "Trachea"); } } catch { }
                        
                        plan.CalculateDVHEstimates(DVHestimationModelId, targetDoseLevels, structureMatches);
                    }
                    else
                    {
                        //PTV
                        if (ptv != null & (ptv.Volume > 0.1))
                        {

                            var ptv_lower = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);


                            if (ptv_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                            }

                        }

                        if (gtv != null & (gtv.Volume > 0.01))
                        {
                            var gtv_lower = plan.OptimizationSetup.AddPointObjective(gtv, OptimizationObjectiveOperator.Lower, new DoseValue((target_escalation_lower + 0.5), "Gy"), 99.9, 100);


                            if (gtv_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv, OptimizationObjectiveOperator.Lower, new DoseValue((target_escalation_lower + 0.5), "Gy"), 99.9, 100);
                            }

                        }
                        if (ptv_opt != null & (ptv_opt.Volume > 0.1))
                        {

                            var opt_ptv_lower1 = plan.OptimizationSetup.AddPointObjective(ptv_opt, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            var opt_ptv_upper1 = plan.OptimizationSetup.AddPointObjective(ptv_opt, OptimizationObjectiveOperator.Upper, new DoseValue((target_upper), "Gy"), 0, 100);

                            if (opt_ptv_lower1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_opt, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 150);
                            }
                            if (opt_ptv_upper1 == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_opt, OptimizationObjectiveOperator.Upper, new DoseValue((target_upper), "Gy"), 0, 100);
                            }
                        }

                        if (gtv_opt != null & (gtv_opt.Volume > 0.01))
                        {
                            var gtv_opt_lower = plan.OptimizationSetup.AddPointObjective(gtv_opt, OptimizationObjectiveOperator.Lower, new DoseValue((target_escalation_lower + 0.5), "Gy"), 99.9, 200);


                            if (gtv_opt_lower == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(gtv_opt, OptimizationObjectiveOperator.Lower, new DoseValue((target_escalation_lower + 0.5), "Gy"), 99.9, 200);
                            }

                        }

                        if (ptv_gtv != null & (ptv_gtv.Volume > 0.01))
                        {
                            var ptv_gtv_upper = plan.OptimizationSetup.AddPointObjective(ptv_gtv, OptimizationObjectiveOperator.Upper, new DoseValue((target_upper + 0.5), "Gy"), 0, 100);


                            if (ptv_gtv_upper == null)
                            {
                                plan.OptimizationSetup.AddPointObjective(ptv_gtv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);
                            }
                        }
                        //Set optimization adjustments for OARs and optimization strucures

                        //Esophagus
                        if (esophagus != null & (esophagus.Volume > 0.01))
                        {
                            try
                            {
                                var esophagus_upper = plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);

                                if (esophagus_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(esophagus, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);
                                }
                            }
                            catch { }
                        }

                        //Heart
                        if (heart != null & (heart.Volume > 0.01))
                        {
                            try
                            {
                                var heart_upper = plan.OptimizationSetup.AddPointObjective(heart, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);

                                if (heart_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(heart, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);
                                }
                            }
                            catch { }
                        }

                        //Trachea

                        if (trachea != null & (trachea.Volume > 0.01))
                        {
                            try
                            {
                                var trachea_upper = plan.OptimizationSetup.AddPointObjective(trachea, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);

                                if (trachea_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(trachea, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 120);
                                }
                            }
                            catch { }
                        }


                        //Set optimization adjustments for OARs and optimization strucures


                        //SpinalCord PRV
                        if (spinal_cord_prv != null & (spinal_cord_prv.Volume > 0.1))
                        {
                            try
                            {

                                var spinalcord_upper = plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(12, "Gy"), 0, 150);


                                if (spinalcord_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(12, "Gy"), 0, 150);
                                }
                            }
                            catch { }

                        }

                        //Skin
                        if (skin != null & (skin.Volume > 0.1))
                        {
                            try
                            {

                                var skin_upper = plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                                if (skin_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                                }
                            }
                            catch { }


                        }

                        //Stomach
                        if (stomach != null & (stomach.Volume > 0.1))
                        {
                            try
                            {

                                var stomach_upper = plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                                if (stomach_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(skin, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                                }
                            }
                            catch { }


                        }

                        //Rib
                        if (rib != null & (rib.Volume > 0.1))
                        {
                            try
                            {

                                var rib_upper = plan.OptimizationSetup.AddPointObjective(rib, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                                if (rib_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(rib, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                                }
                            }
                            catch { }


                        }


                        //Cauda equina
                        if (cauda_equina != null & (cauda_equina.Volume > 0.1))
                        {
                            try
                            {

                                var cauda_equina_upper = plan.OptimizationSetup.AddPointObjective(cauda_equina, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                                if (cauda_equina_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(cauda_equina, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                                }
                            }
                            catch { }

                        }


                        //Sacral plexus
                        if (sacral_plexus != null & (sacral_plexus.Volume > 0.1))
                        {
                            try
                            {

                                var sacral_plexus_upper = plan.OptimizationSetup.AddPointObjective(sacral_plexus, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);

                                if (sacral_plexus_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(sacral_plexus, OptimizationObjectiveOperator.Upper, new DoseValue(24, "Gy"), 0, 150);
                                }
                            }
                            catch { }

                        }


                        //Duodenum
                        if (duodenum != null & (duodenum.Volume > 0.1))
                        {
                            try
                            {

                                var duodenum_upper = plan.OptimizationSetup.AddPointObjective(duodenum, OptimizationObjectiveOperator.Upper, new DoseValue(18, "Gy"), 0, 100);

                                if (duodenum_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(duodenum, OptimizationObjectiveOperator.Upper, new DoseValue(18, "Gy"), 0, 100);
                                }
                            }
                            catch { }

                        }

                        //Lungs-PTV
                        if (both_lungs != null & (both_lungs.Volume > 0.1))
                        {
                            try
                            {


                                var both_lungs_mean = plan.OptimizationSetup.AddMeanDoseObjective(both_lungs, new DoseValue(8, "Gy"), 80);

                                if (both_lungs_mean == null)
                                {
                                    both_lungs_mean = plan.OptimizationSetup.AddMeanDoseObjective(both_lungs, new DoseValue(8, "Gy"), 80);
                                }
                            }
                            catch { }

                        }


                        //Global maximum limit
                        //PharConst-PTV
                        if (body != null & (body.Volume > 0.1))
                        {
                            try
                            {
                                double body_upper_dose = Math.Round(((DPF * NOF) * 1.2), 2);
                                var body_upper = plan.OptimizationSetup.AddPointObjective(body, OptimizationObjectiveOperator.Upper, new DoseValue(body_upper_dose, "Gy"), 0, 80);
                                if (body_upper == null)
                                {
                                    plan.OptimizationSetup.AddPointObjective(body, OptimizationObjectiveOperator.Upper, new DoseValue(body_upper_dose, "Gy"), 0, 80);
                                    MessageBox.Show(string.Format("Please, define avoidance structures manually, if necessary. To continue with calculations, launch this script again"));
                                    Window.GetWindow(this).Close(); return;
                                }
                            }
                            catch { }
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
                            var ptv_DVH = plan.GetDoseAtVolume(ptv_opt, 98.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                            var cauda_equina_MAX = plan.GetDoseAtVolume(cauda_equina, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            var sacral_plexus_MAX = plan.GetDoseAtVolume(sacral_plexus, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            var spinal_cord_MAX = plan.GetDoseAtVolume(spinal_cord_prv, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            var skin_MAX = plan.GetDoseAtVolume(skin, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            var duodenum_MAX = plan.GetDoseAtVolume(duodenum, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            var stomach_MAX = plan.GetDoseAtVolume(stomach, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            var heart_MAX = plan.GetDoseAtVolume(heart, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            var trachea_MAX = plan.GetDoseAtVolume(trachea, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            var esophagus_MAX = plan.GetDoseAtVolume(esophagus, 0.01f, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);


                            //Show report message
                            var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                            MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! Plan calculation took '{0}' minutes\n 98% '{1}' covered by '{2}' Gy\n '{3}' max point dose = '{4}'Gy\n '{5}' max point dose = '{6}'Gy\n '{7}' max point dose = '{8}'Gy\n '{9}' max point dose = '{10}'Gy\n '{11}' max point dose = '{12}'Gy\n '{13}' max point dose = '{14}'Gy\n '{15}' max point dose = '{16}'Gy\n '{17}' max point dose = '{18}'Gy\n '{19}' max point dose = '{20}'Gy",


                                overall_calculation_time,
                                ptv_opt.Id, ptv_DVH,
                                cauda_equina.Id, cauda_equina_MAX,
                                sacral_plexus.Id, sacral_plexus_MAX,
                                spinal_cord_prv.Id, spinal_cord_MAX,
                                skin.Id, skin_MAX,
                                duodenum.Id, duodenum_MAX,
                                stomach.Id, stomach_MAX,
                                heart.Id, heart_MAX,
                                trachea.Id, trachea_MAX,
                                esophagus.Id, esophagus_MAX
                                ),


                                SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
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
                Button_CrtAndCalPlan.IsEnabled = false; return;
            }
            if (double.TryParse(dosePerFxEscalation.Text, out double DPFe)&&double.TryParse(alpha_beta.Text, out double aplha_beta_double))
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
            total.Visibility = Visibility.Hidden;
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

                if (double.TryParse(dosePerFxEscalation.Text, out double DPFe)&& double.TryParse(alpha_beta.Text, out double test))
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
                Button_CrtAndCalPlan.IsEnabled = false; return;
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
