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
    /// Interaction logic for LymphomaAutoPlan.xaml
    /// </summary>
    public partial class LymphomaAutoPlan : UserControl
    {
        
        Tuple<int, DoseValue> prescription = null;
        string linac = "";
        string calculation_model = "PO_15.6.06";
        public Patient patient = VMS.TPS.Script.context.Patient;
        User user = VMS.TPS.Script.context.CurrentUser;
        StructureSet ss = VMS.TPS.Script.context.StructureSet;
        double DPF = 2.0;
        int NOF = 1;

        private void LinacIDChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public LymphomaAutoPlan(ScriptContext context)
        {
            InitializeComponent();
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
            RapidPlanTextBox.IsEnabled = false;
            RapidPlanTextBox.Visibility = Visibility.Hidden;
            RapidPlanModelId.IsEnabled = false;
            RapidPlanModelId.Visibility = Visibility.Hidden;
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


                if (double.TryParse(alpha_beta.Text, out double aplha_beta_double))
                {
                    double bED = (TD * (1 + (DPF / aplha_beta_double)));
                    double eQD2 = (TD * ((DPF + aplha_beta_double) / (2 + aplha_beta_double)));
                    EQD2.Text = (Math.Round(eQD2, 2)).ToString();
                    BED.Text = (Math.Round(bED, 2)).ToString();
                }
                else { EQD2.Text = ""; BED.Text = ""; }

            }
            else
            {
                Button_CrtAndCalPlan.IsEnabled = false;
                
            }
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
            }
            else { EQD2.Text = ""; BED.Text = ""; }
        }
        private void CrtAndCalPlan(object sender, RoutedEventArgs e)
        {
                if (RapidPlan.IsChecked==true)
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
                const string SCRIPT_NAME = "AutoPlan_Lymphoma";
                //ID of the course
                const string COURSE_ID = "Treatment";
                //ID of the plan
                string PLAN_ID = string.Format("Lmphm_{0}Gy", (Math.Round((NOF * DPF), 0)).ToString());


                //IDs of fields
                const string MVCBCT_ID = "MVCBCT";

                const string BEAM_1_ID = "Thrx_60-300/20";
                const string BEAM_2_ID = "Thrx_300-60/340";
                const string BEAM_3_ID = "Thrx_60-300/0";



                //IDs of PTV structures

                const string PTV_ID = "PTV";
                const string PTV_lungs_ID = "PTV-Lungs";


                //IDs of critical structures
                const string BowelBag_ID = "BowelBag";
                const string OPT_BowelBag_ID = "BowelBag-PTV";
                const string LUNG_L_ID = "Lung_L";
                const string OPT_LUNG_L_ID = "Lung_L-PTV";
                const string LUNG_R_ID = "Lung_R";
                const string OPT_LUNG_R_ID = "Lung_R-PTV";
                const string LUNGS_ID = "Lungs";
                const string LUNGS_PTV_ID = "Lungs-PTV";
                const string SPINAL_CORD_ID = "SpinalCord";
                const string SPINAL_CORD_PRV_ID = "SpnlCrd_PRV";
                const string TRACHEA_ID = "Trachea";
                const string OPT_TRACHEA_ID = "Trachea-PTV";
                const string HEART_ID = "Heart";
                const string OPT_HEART_ID = "Heart-PTV";
                const string PTV_LUNGS_ID = "PTV-Lungs";
                const string BODY_ID = "BODY";



                //Show greetings window
                string Greeting = string.Format("Greetings {0}! Please, ensure that the structure 'PTV' exist in the currest structure set. Script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru').", user);
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




                List<string> NOT_FOUND_list = new List<string>();

                //find PTVs. Show an error notification if no optimization PTV will be found
                Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                if (ptv == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found! Please, ensure that PTV is presented in the structure set", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Window.GetWindow(this).Close(); return; ;
                }

                Structure ptv_lungs = ss.Structures.FirstOrDefault(x => x.Id == PTV_LUNGS_ID);
                if (ptv_lungs == null)
                {
                    NOT_FOUND_list.Add(PTV_LUNGS_ID);
                }


                //Find OARs and optimization structures

                Structure BowelBag = ss.Structures.FirstOrDefault(x => x.Id == BowelBag_ID);
                if (BowelBag == null)
                {
                    NOT_FOUND_list.Add(BowelBag_ID);
                    try { BowelBag = ss.AddStructure("Avoidance", BowelBag_ID); } catch { }
                }

                Structure opt_BowelBag = ss.Structures.FirstOrDefault(x => x.Id == OPT_BowelBag_ID);
                if (opt_BowelBag == null)
                {
                    NOT_FOUND_list.Add(OPT_BowelBag_ID);
                }

                Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_ID);
                if (lung_L == null)
                {
                    NOT_FOUND_list.Add(LUNG_L_ID);
                }

                Structure opt_lung_L = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_L_ID);
                if (opt_lung_L == null)
                {
                    NOT_FOUND_list.Add(OPT_LUNG_L_ID);
                }

                Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                if (lung_R == null)
                {
                    NOT_FOUND_list.Add(LUNG_R_ID);
                }

                Structure opt_lung_R = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_R_ID);
                if (opt_lung_R == null)
                {
                    NOT_FOUND_list.Add(OPT_LUNG_R_ID);
                }

                Structure lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID);
                if (lungs == null)
                {
                    NOT_FOUND_list.Add(LUNGS_ID);
                }

                Structure opt_lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_PTV_ID);
                if (opt_lungs == null)
                {
                    NOT_FOUND_list.Add(LUNGS_PTV_ID);
                }

                Structure spinal_cord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                if (spinal_cord == null)
                {
                    NOT_FOUND_list.Add(SPINAL_CORD_ID);
                }

                Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                if (spinal_cord_prv == null)
                {
                    NOT_FOUND_list.Add(SPINAL_CORD_PRV_ID);
                }

                Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                if (trachea == null)
                {
                    NOT_FOUND_list.Add(TRACHEA_ID);
                    try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                }

                Structure opt_trachea = ss.Structures.FirstOrDefault(x => x.Id == OPT_TRACHEA_ID);
                if (opt_trachea == null)
                {
                    NOT_FOUND_list.Add(OPT_TRACHEA_ID);
                }

                Structure heart = ss.Structures.FirstOrDefault(x => x.Id == HEART_ID);
                if (heart == null)
                {
                    NOT_FOUND_list.Add(HEART_ID);
                }

                Structure opt_heart = ss.Structures.FirstOrDefault(x => x.Id == OPT_HEART_ID);
                if (opt_heart == null)
                {
                    NOT_FOUND_list.Add(OPT_HEART_ID);
                }

                Structure body = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                if (body == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return;
                }


                //put isocenter to the center of PTV
                VVector isocenter = new VVector(Math.Round(ptv.CenterPoint.x / 10.0f) * 10.0f, Math.Round(ptv.CenterPoint.y / 10.0f) * 10.0f, Math.Round(ptv.CenterPoint.z / 10.0f) * 10.0f);

                //show message with list of not found structures
                string[] NOT_FOUND = NOT_FOUND_list.ToArray();
                string res = string.Join("\r\n", NOT_FOUND);
                var array_length = NOT_FOUND.Length;
                if (array_length > 0)
                {
                    MessageBox.Show(string.Format("Structures were not found:\n'{0}'", res), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

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
                new ExternalBeamMachineParameters(linac, "6X", 800, "ARC", "FFF");


                //Check if the beams already exist. If no, create beams

                //Setup by default

                Beam beam1 = plan.Beams.FirstOrDefault(x => x.Id == BEAM_1_ID);
                if (beam1 == null)
                {
                    beam1 = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 60, 300, GantryDirection.CounterClockwise, 0, isocenter);
                    beam1.Id = BEAM_1_ID;
                }

                Beam beam2 = plan.Beams.FirstOrDefault(x => x.Id == BEAM_2_ID);
                if (beam2 == null)
                {
                    beam2 = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 300, 60, GantryDirection.Clockwise, 0, isocenter);
                    beam2.Id = BEAM_2_ID;
                }

                //Fit collimator to structure. For Halcyon machines the next 5 strings are not necessary
                try
                {
                    bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                    beam1.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                    beam2.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                    //beam3.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                    FitToStructureMargins margins = new FitToStructureMargins(0);
                }
                catch { MessageBox.Show(string.Format("Could not place beams for some reason...")); }
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
                plan.OptimizationSetup.AddAutomaticNormalTissueObjective(100);


                //Set optimization adjustments for PTVs
                GreenBar.Value = 30;

                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Defining objectives...";
                GreenBarListBox.Items.Add(text.Text);



                double target_lower = Math.Round((DPF * NOF), 2);
                double target_upper = Math.Round(((DPF * NOF) * 1.05), 2);
                
                if (HN_Lphm_CBI.IsSelected==true) 
                {
                    string DVHestimationModelId = "H&N_Miscellaneous";

                    //Choose target dose level
                    Dictionary<string, DoseValue> targetDoseLevels = new Dictionary<string, DoseValue>();
                    targetDoseLevels.Add("PTV", plan.TotalDose);


                    //Match structures from the structure set to structurel listed in DVHestimationModel 
                    Dictionary<string, string> structureMatches = new Dictionary<string, string>();
                    structureMatches.Add(PTV_ID, "PTV");
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Block")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Block").Id, "Block"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BODY")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BODY").Id, "BODY"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brain")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brain").Id, "Brain"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem").Id, "Brainstem"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem_PRV").Id, "Brainstem_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Brainstem-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Brainstem-PTV").Id, "Brainstem-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm").Id, "Chiasm"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm_PRV").Id, "Chiasm_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Chiasm-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Chiasm-PTV").Id, "Chiasm-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_L").Id, "Cochlea_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Cochlea_R").Id, "Cochlea_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BowelBag")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BowelBag").Id, "BowelBag"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BowelBag-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BowelBag-PTV").Id, "BowelBag-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Eye_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Eye_L").Id, "Eye_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Eye_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Eye_R").Id, "Eye_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Larynx")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Larynx").Id, "Larynx"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Larynx-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Larynx-PTV").Id, "Larynx-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lens_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lens_L").Id, "Lens_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lens_L_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lens_L_PRV").Id, "Lens_L_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lens_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lens_R").Id, "Lens_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lens_R_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lens_R_PRV").Id, "Lens_R_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lips")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lips").Id, "Lips"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lung_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lung_L").Id, "Lung_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lung_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lung_R").Id, "Lung_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lungs")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lungs").Id, "Lungs"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Mandible")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Mandible").Id, "Mandible"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Mandible-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Mandible-PTV").Id, "Mandible-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Nape")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Nape").Id, "Nape"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_L").Id, "OpticNerve_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "OpticNerve_R").Id, "OpticNerve_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "OralCvt-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "OralCvt-PTV").Id, "OralCvt-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Parotid_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Parotid_L").Id, "Parotid_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Parotid_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Parotid_R").Id, "Parotid_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Parotids")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Parotids").Id, "Parotids"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "PharynxConst")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "PharynxConst").Id, "PharynxConst"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "PhConst-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "PhConst-PTV").Id, "PhConst-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Prtd_L-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Prtd_L-PTV").Id, "Prtd_L-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Prtd_L-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Prtd_L-PTV").Id, "Prtd_L-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Skin")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Skin").Id, "Skin"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord").Id, "SpinalCord"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV").Id, "SpnlCrd_PRV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Trachea")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Trachea").Id, "Trachea"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Trachea-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Trachea-PTV").Id, "Trachea-PTV"); } } catch { }


                    plan.CalculateDVHEstimates(DVHestimationModelId, targetDoseLevels, structureMatches);

                    
                }
                if (Plvs_Abdmnl_Lphm_CBI.IsSelected==true) 
                {
                    string DVHestimationModelId = "Lymphoma Pelvis/Abdominal";

                    //Choose target dose level
                    Dictionary<string, DoseValue> targetDoseLevels = new Dictionary<string, DoseValue>();
                    targetDoseLevels.Add("PTV", plan.TotalDose);


                    //Match structures from the structure set to structurel listed in DVHestimationModel 
                    Dictionary<string, string> structureMatches = new Dictionary<string, string>();
                    structureMatches.Add(PTV_ID, "PTV");

                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BODY")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BODY").Id, "BODY"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Bladder")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Bladder").Id, "Bladder"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Bladder-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Bladder-PTV").Id, "Bladder-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BODY")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BODY").Id, "BODY"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BowelBag")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BowelBag").Id, "BowelBag"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BowelBag-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BowelBag-PTV").Id, "BowelBag-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Duodenum")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Duodenum").Id, "Duodenum"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Duodenum-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Duodenum-PTV").Id, "Duodenum-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidney_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidney_L").Id, "Kidney_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidney_L-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidney_L-PTV").Id, "Kidney_L-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidney_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidney_R").Id, "Kidney_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidney_R-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidney_R-PTV").Id, "Kidney_R-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidneys")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidneys").Id, "Kidneys"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidneys-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidneys-PTV").Id, "Kidneys-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Liver")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Liver").Id, "Liver"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Liver-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Liver-PTV").Id, "Liver-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Rectum")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Rectum").Id, "Rectum"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Rectum-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Rectum-PTV").Id, "Rectum-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Stomach")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Stomach").Id, "Stomach"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Stomach-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Stomach-PTV").Id, "Stomach-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord").Id, "SpinalCord"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV").Id, "SpnlCrd_PRV"); } } catch { }


                    plan.CalculateDVHEstimates(DVHestimationModelId, targetDoseLevels, structureMatches);
                }
                if (Thorax_Lphm_CBI.IsSelected==true) 
                {
                    string DVHestimationModelId = "Lymphoma Thorax";

                    //Choose target dose level
                    Dictionary<string, DoseValue> targetDoseLevels = new Dictionary<string, DoseValue>();
                    targetDoseLevels.Add("PTV", plan.TotalDose);


                    //Match structures from the structure set to structurel listed in DVHestimationModel 
                    Dictionary<string, string> structureMatches = new Dictionary<string, string>();
                    structureMatches.Add(PTV_ID, "PTV");

                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BODY")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BODY").Id, "BODY"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BowelBag")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BowelBag").Id, "BowelBag"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "BowelBag-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "BowelBag-PTV").Id, "BowelBag-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Larynx")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Larynx").Id, "Larynx"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Larynx-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Larynx-PTV").Id, "Larynx-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Duodenum")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Duodenum").Id, "Duodenum"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Duodenum-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Duodenum-PTV").Id, "Duodenum-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Heart")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Heart").Id, "Heart"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Heart-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Heart-PTV").Id, "Heart-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidney_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidney_L").Id, "Kidney_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidney_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidney_R").Id, "Kidney_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidney_L-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidney_L-PTV").Id, "Kidney_L-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidney_R-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidney_R-PTV").Id, "Kidney_R-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidneys")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidneys").Id, "Kidneys"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Kidneys-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Kidneys-PTV").Id, "Kidneys-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Liver")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Liver").Id, "Liver"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Liver-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Liver-PTV").Id, "Liver-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lung_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lung_L").Id, "Lung_L"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lung_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lung_R").Id, "Lung_R"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lung_L-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lung_L-PTV").Id, "Lung_L-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lung_R-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lung_R-PTV").Id, "Lung_R-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lungs")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lungs").Id, "Lungs"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Lungs-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Lungs-PTV").Id, "Lungs-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Stomach")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Stomach").Id, "Stomach"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Stomach-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Stomach-PTV").Id, "Stomach-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Trachea")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Trachea").Id, "Trachea"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Trachea-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Trachea-PTV").Id, "Trachea-PTV"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpinalCord").Id, "SpinalCord"); } } catch { }
                    try { if ((ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "SpnlCrd_PRV").Id, "SpnlCrd_PRV"); } } catch { }


                    plan.CalculateDVHEstimates(DVHestimationModelId, targetDoseLevels, structureMatches);
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

                try { plan.CalculateDose(); } catch { try { MessageBox.Show(string.Format("Dear {0},\nUnfortunately, something went wrong during volume dose calculations.\n\nPlease, perform volume dose calculations manually", user), "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }

                GreenBar.Value = 100;
                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Plan calculation finished!";
                GreenBarListBox.Items.Add(text.Text);









                try
                {
                    //Define DVH parameters to report
                    var ptv_DVH = plan.GetDoseAtVolume(ptv, 95.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                    var lungs_V20 = plan.GetDoseAtVolume(lungs, 20.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                    var heart_V10 = plan.GetDoseAtVolume(heart, 10.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);

                    //Show report message
                    var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                    MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! Plan calculation took '{0}' minutes\n 95% '{1}' covered by '{2}' Gy\n 20% '{3}' covered by '{4}' Gy\n 10% '{5}' covered by '{6}' Gy",


                        overall_calculation_time,
                        PTV_ID, ptv_DVH,
                        LUNGS_ID, lungs_V20,
                        HEART_ID, heart_V10
                        ),


                        SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch { MessageBox.Show(string.Format("Dear {0},\n\nUnfortunately, the report message could not be shown...", user), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
            }
            else
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
                const string SCRIPT_NAME = "AutoPlan_Lymphoma";
                //ID of the course
                const string COURSE_ID = "Treatment";
                //ID of the plan
                string PLAN_ID = string.Format("Lmphm_{0}Gy", (Math.Round((NOF * DPF), 0)).ToString());


                //IDs of fields
                const string MVCBCT_ID = "MVCBCT";

                const string BEAM_1_ID = "Thrx_60-300/20";
                const string BEAM_2_ID = "Thrx_300-60/340";
                const string BEAM_3_ID = "Thrx_60-300/0";



                //IDs of PTV structures

                const string PTV_ID = "PTV";
                const string PTV_lungs_ID = "PTV-Lungs";


                //IDs of critical structures
                const string BowelBag_ID = "BowelBag";
                const string OPT_BowelBag_ID = "BowelBag-PTV";
                const string LUNG_L_ID = "Lung_L";
                const string OPT_LUNG_L_ID = "Lung_L-PTV";
                const string LUNG_R_ID = "Lung_R";
                const string OPT_LUNG_R_ID = "Lung_R-PTV";
                const string LUNGS_ID = "Lungs";
                const string LUNGS_PTV_ID = "Lungs-PTV";
                const string SPINAL_CORD_ID = "SpinalCord";
                const string SPINAL_CORD_PRV_ID = "SpnlCrd_PRV";
                const string TRACHEA_ID = "Trachea";
                const string OPT_TRACHEA_ID = "Trachea-PTV";
                const string HEART_ID = "Heart";
                const string OPT_HEART_ID = "Heart-PTV";
                const string PTV_LUNGS_ID = "PTV-Lungs";
                const string BODY_ID = "BODY";



                //Show greetings window
                string Greeting = string.Format("Greetings {0}! Please, ensure that the structure 'PTV' exist in the currest structure set. Script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru').", user);
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




                List<string> NOT_FOUND_list = new List<string>();

                //find PTVs. Show an error notification if no optimization PTV will be found
                Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                if (ptv == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found! Please, ensure that PTV is presented in the structure set", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Window.GetWindow(this).Close(); return; ;
                }

                Structure ptv_lungs = ss.Structures.FirstOrDefault(x => x.Id == PTV_LUNGS_ID);
                if (ptv_lungs == null)
                {
                    NOT_FOUND_list.Add(PTV_LUNGS_ID);
                }


                //Find OARs and optimization structures

                Structure BowelBag = ss.Structures.FirstOrDefault(x => x.Id == BowelBag_ID);
                if (BowelBag == null)
                {
                    NOT_FOUND_list.Add(BowelBag_ID);
                    try { BowelBag = ss.AddStructure("Avoidance", BowelBag_ID); } catch { }
                }

                Structure opt_BowelBag = ss.Structures.FirstOrDefault(x => x.Id == OPT_BowelBag_ID);
                if (opt_BowelBag == null)
                {
                    NOT_FOUND_list.Add(OPT_BowelBag_ID);
                }

                Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_ID);
                if (lung_L == null)
                {
                    NOT_FOUND_list.Add(LUNG_L_ID);
                }

                Structure opt_lung_L = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_L_ID);
                if (opt_lung_L == null)
                {
                    NOT_FOUND_list.Add(OPT_LUNG_L_ID);
                }

                Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                if (lung_R == null)
                {
                    NOT_FOUND_list.Add(LUNG_R_ID);
                }

                Structure opt_lung_R = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_R_ID);
                if (opt_lung_R == null)
                {
                    NOT_FOUND_list.Add(OPT_LUNG_R_ID);
                }

                Structure lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID);
                if (lungs == null)
                {
                    NOT_FOUND_list.Add(LUNGS_ID);
                }

                Structure opt_lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_PTV_ID);
                if (opt_lungs == null)
                {
                    NOT_FOUND_list.Add(LUNGS_PTV_ID);
                }

                Structure spinal_cord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                if (spinal_cord == null)
                {
                    NOT_FOUND_list.Add(SPINAL_CORD_ID);
                }

                Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                if (spinal_cord_prv == null)
                {
                    NOT_FOUND_list.Add(SPINAL_CORD_PRV_ID);
                }

                Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                if (trachea == null)
                {
                    NOT_FOUND_list.Add(TRACHEA_ID);
                    try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                }

                Structure opt_trachea = ss.Structures.FirstOrDefault(x => x.Id == OPT_TRACHEA_ID);
                if (opt_trachea == null)
                {
                    NOT_FOUND_list.Add(OPT_TRACHEA_ID);
                }

                Structure heart = ss.Structures.FirstOrDefault(x => x.Id == HEART_ID);
                if (heart == null)
                {
                    NOT_FOUND_list.Add(HEART_ID);
                }

                Structure opt_heart = ss.Structures.FirstOrDefault(x => x.Id == OPT_HEART_ID);
                if (opt_heart == null)
                {
                    NOT_FOUND_list.Add(OPT_HEART_ID);
                }

                Structure body = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                if (body == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return;
                }


                //put isocenter to the center of PTV
                VVector isocenter = new VVector(Math.Round(ptv.CenterPoint.x / 10.0f) * 10.0f, Math.Round(ptv.CenterPoint.y / 10.0f) * 10.0f, Math.Round(ptv.CenterPoint.z / 10.0f) * 10.0f);

                //show message with list of not found structures
                string[] NOT_FOUND = NOT_FOUND_list.ToArray();
                string res = string.Join("\r\n", NOT_FOUND);
                var array_length = NOT_FOUND.Length;
                if (array_length > 0)
                {
                    MessageBox.Show(string.Format("Structures were not found:\n'{0}'", res), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

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
                new ExternalBeamMachineParameters(linac, "6X", 800, "ARC", "FFF");


                //Check if the beams already exist. If no, create beams

                //Setup by default

                Beam beam1 = plan.Beams.FirstOrDefault(x => x.Id == BEAM_1_ID);
                if (beam1 == null)
                {
                    beam1 = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 60, 300, GantryDirection.CounterClockwise, 0, isocenter);
                    beam1.Id = BEAM_1_ID;
                }

                Beam beam2 = plan.Beams.FirstOrDefault(x => x.Id == BEAM_2_ID);
                if (beam2 == null)
                {
                    beam2 = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 300, 60, GantryDirection.Clockwise, 0, isocenter);
                    beam2.Id = BEAM_2_ID;
                }

                //Fit collimator to structure. For Halcyon machines the next 5 strings are not necessary
                try
                {
                    bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                    beam1.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                    beam2.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                    //beam3.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                    FitToStructureMargins margins = new FitToStructureMargins(0);
                }
                catch { MessageBox.Show(string.Format("Could not place beams for some reason...")); }
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
                plan.OptimizationSetup.AddAutomaticNormalTissueObjective(100);


                //Set optimization adjustments for PTVs
                GreenBar.Value = 30;

                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Defining objectives...";
                GreenBarListBox.Items.Add(text.Text);



                double target_lower = Math.Round((DPF * NOF), 2);
                double target_upper = Math.Round(((DPF * NOF) * 1.05), 2);
                //PTV
                if (ptv != null & (ptv.Volume > 0.1))
                {
                    try
                    {
                        var opt_ptv_lower = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                        var opt_ptv_upper = plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);

                        if (opt_ptv_lower == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                        }
                        if (opt_ptv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);
                        }
                    }
                    catch { MessageBox.Show(string.Format("Could ot define optimization objectives for:'{0}'", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }
                if (ptv_lungs != null & (ptv_lungs.Volume > 0.1))
                {
                    try
                    {
                        var opt_ptv_lower = plan.OptimizationSetup.AddPointObjective(ptv_lungs, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                        var opt_ptv_upper = plan.OptimizationSetup.AddPointObjective(ptv_lungs, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);

                        if (opt_ptv_lower == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                        }
                        if (opt_ptv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);
                        }
                    }
                    catch { MessageBox.Show(string.Format("Could ot define optimization objectives for:'{0}'", PTV_LUNGS_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }

                //Set optimization adjustments for OARs and optimization strucures


                //BowelBag
                if (BowelBag != null & (BowelBag.Volume > 0.1))
                {
                    try
                    {
                        var BowelBag_upper = plan.OptimizationSetup.AddPointObjective(BowelBag, OptimizationObjectiveOperator.Upper, new DoseValue((target_lower - 2), "Gy"), 0, 120);

                        if (BowelBag_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(BowelBag, OptimizationObjectiveOperator.Upper, new DoseValue((target_lower - 2), "Gy"), 0, 120);
                        }
                    }
                    catch { }
                }

                //BowelBag-PTV
                if (opt_BowelBag != null & (opt_BowelBag.Volume > 0.1))
                {
                    try
                    {
                        var opt_BowelBag_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_BowelBag, new DoseValue(28, "Gy"), 80);
                        if (opt_BowelBag_mean == null)
                        {
                            opt_BowelBag_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_BowelBag, new DoseValue(28, "Gy"), 80);
                        }
                    }
                    catch { }
                }




                //Lung_L-PTV
                if (opt_lung_L != null & (opt_lung_L.Volume > 0.1))
                {
                    try
                    {
                        var lung_L_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_lung_L, new DoseValue(8, "Gy"), 80);
                        if (lung_L_ptv_mean == null)
                        {
                            lung_L_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_lung_L, new DoseValue(8, "Gy"), 80);
                        }
                    }
                    catch { }
                }
                //Lung_R-PTV
                if (opt_lung_R != null & (opt_lung_R.Volume > 0.1))
                {
                    try
                    {
                        var lung_R_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_lung_R, new DoseValue(8, "Gy"), 80);
                        if (lung_R_ptv_mean == null)
                        {
                            lung_R_ptv_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_lung_R, new DoseValue(8, "Gy"), 80);
                        }
                    }
                    catch { }
                }


                //Spinal_Cord
                if (spinal_cord != null & (spinal_cord.Volume > 0.1))
                {
                    try
                    {
                        var spinal_cord_upper = plan.OptimizationSetup.AddPointObjective(spinal_cord, OptimizationObjectiveOperator.Upper, new DoseValue(25, "Gy"), 0, 150);

                        if (spinal_cord_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(spinal_cord, OptimizationObjectiveOperator.Upper, new DoseValue(25, "Gy"), 0, 150);
                        }
                    }
                    catch { }
                }


                //Spinal_Cord_PRV
                if (spinal_cord_prv != null & (spinal_cord_prv.Volume > 0.1))
                {
                    try
                    {
                        var spinal_cord_prv_upper = plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(30, "Gy"), 0, 150);

                        if (spinal_cord_prv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(spinal_cord_prv, OptimizationObjectiveOperator.Upper, new DoseValue(30, "Gy"), 0, 150);
                        }
                    }
                    catch { }
                }


                //Trachea-PTV
                if (opt_trachea != null & (opt_trachea.Volume > 0.1))
                {
                    try
                    {
                        var opt_trachea_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_trachea, new DoseValue(20, "Gy"), 80);
                        if (opt_trachea_mean == null)
                        {
                            opt_trachea_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_trachea, new DoseValue(20, "Gy"), 80);
                        }
                    }
                    catch { }
                }


                //Lungs-PTV
                if (opt_lungs != null & (opt_lungs.Volume > 0.1))
                {

                    var opt_lungs_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_lungs, new DoseValue(10, "Gy"), 80);
                    if (opt_lungs_mean == null)
                    {
                        opt_lungs_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_lungs, new DoseValue(10, "Gy"), 80);
                    }
                }


                //Heart-PTV
                if (opt_heart != null & (opt_heart.Volume > 0.1))
                {
                    try
                    {
                        var opt_heart_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_heart, new DoseValue(4, "Gy"), 80);
                        if (opt_heart_mean == null)
                        {
                            opt_heart_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_heart, new DoseValue(4, "Gy"), 80);
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
                        double body_upper_dose = Math.Round(((DPF * NOF) * 1.07), 2);
                        var body_upper = plan.OptimizationSetup.AddPointObjective(body, OptimizationObjectiveOperator.Upper, new DoseValue(body_upper_dose, "Gy"), 0, 400);
                        if (body_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(body, OptimizationObjectiveOperator.Upper, new DoseValue(body_upper_dose, "Gy"), 0, 400);
                            MessageBox.Show(string.Format("Please, define avoidance structures manually, if necessary. To continue with calculations, launch this script again"));
                            Window.GetWindow(this).Close(); return; ;
                        }
                    }
                    catch { }
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

                try { plan.CalculateDose(); } catch { try { MessageBox.Show(string.Format("Dear {0},\nUnfortunately, something went wrong during volume dose calculations.\n\nPlease, perform volume dose calculations manually", user), "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }

                GreenBar.Value = 100;
                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Plan calculation finished!";
                GreenBarListBox.Items.Add(text.Text);









                try
                {
                    //Define DVH parameters to report
                    var ptv_DVH = plan.GetDoseAtVolume(ptv, 95.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                    var lungs_V20 = plan.GetDoseAtVolume(lungs, 20.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                    var heart_V10 = plan.GetDoseAtVolume(heart, 10.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);

                    //Show report message
                    var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                    MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! Plan calculation took '{0}' minutes\n 95% '{1}' covered by '{2}' Gy\n 20% '{3}' covered by '{4}' Gy\n 10% '{5}' covered by '{6}' Gy",


                        overall_calculation_time,
                        PTV_ID, ptv_DVH,
                        LUNGS_ID, lungs_V20,
                        HEART_ID, heart_V10
                        ),


                        SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch { MessageBox.Show(string.Format("Dear {0},\n\nUnfortunately, the report message could not be shown...", user), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
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

        private void CrtAndCalPlanIMRT(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("Dear {0},\n\n Unfortunately, this module is not applicable for Halcyon MV CBCT machine in Eclipse v15.6\n Please, calculate VMAT instead or make necessary calculations manualy", VMS.TPS.Script.context.CurrentUser), "Oh Dear...", MessageBoxButton.OK, MessageBoxImage.Question);
        }

        private void RapidPlanModelIdChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RapidPlanCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RapidPlanTextBox.IsEnabled = true;
            RapidPlanTextBox.Visibility = Visibility.Visible;
            RapidPlanModelId.IsEnabled = true;
            RapidPlanModelId.Visibility = Visibility.Visible;
        }
        private void RapidPlanCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RapidPlanTextBox.IsEnabled = false;
            RapidPlanTextBox.Visibility = Visibility.Hidden;
            RapidPlanModelId.IsEnabled = false;
            RapidPlanModelId.Visibility = Visibility.Hidden;
        }
    }
}
