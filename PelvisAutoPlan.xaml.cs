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
    /// Interaction logic for PelvisAutoPlan.xaml
    /// </summary>
    public partial class PelvisAutoPlan : UserControl
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

        public PelvisAutoPlan(ScriptContext context)
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
                return;
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
                const string SCRIPT_NAME = "AutoPlan_Pelvis";
                //ID of the course
                const string COURSE_ID = "Treatment";
                //ID of the plan
                string PLAN_ID = string.Format("Plvs_{0}Gy", (Math.Round((NOF * DPF), 0)).ToString());


                // Change these IDs to match your clinical protocol
                const string PTV_ID = "PTV";
                const string MVCBCT_ID = "MVCBCT";
                const string BEAM_1_ID = "Plvs179-181/20";
                const string BEAM_2_ID = "Plvs181-179/340";
                const string RECTUM_ID = "Rectum";
                const string OPT_RECTUM_ID = "Rectum-PTV";
                const string BLADDER_ID = "Bladder";
                const string OPT_BLADDER_ID = "Bladder-PTV";
                const string BLOCK_ID = "Block";
                const string BOWELBAG_ID = "BowelBag";
                const string OPT_BOWELBAG_ID = "BowelBag-PTV";
                const string BODY_ID = "BODY";



                //Show greetings window
                string Greeting = string.Format("Greetings {0}! Please, ensure that structure 'PTV' exist in the currest structure set. Script is made by 'PET_Tehnology'\n  Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru').", user);
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

                //find PTV
                Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                if (ptv == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return;
                }


                //Find OARs and optimization structures
                Structure rectum = ss.Structures.FirstOrDefault(x => x.Id == RECTUM_ID);
                if (rectum == null)
                {
                    NOT_FOUND_list.Add(RECTUM_ID);
                }

                Structure opt_rectum = ss.Structures.FirstOrDefault(x => x.Id == OPT_RECTUM_ID);
                if (opt_rectum == null)
                {
                    NOT_FOUND_list.Add(OPT_RECTUM_ID);
                }

                Structure bladder = ss.Structures.FirstOrDefault(x => x.Id == BLADDER_ID);
                if (bladder == null)
                {
                    NOT_FOUND_list.Add(BLADDER_ID);
                }

                Structure opt_bladder = ss.Structures.FirstOrDefault(x => x.Id == OPT_BLADDER_ID);
                if (opt_bladder == null)
                {
                    NOT_FOUND_list.Add(OPT_BLADDER_ID);
                }

                Structure block = ss.Structures.FirstOrDefault(x => x.Id == BLOCK_ID);
                if (block == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found! It is an avoidance structure for beams entry point", BLOCK_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                Structure body = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                if (body == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return; ;
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

                //Check if the beams already exist. If no, create beams
                Beam beam1 = plan.Beams.FirstOrDefault(x => x.Id == BEAM_1_ID);
                if (beam1 == null)
                {
                    beam1 = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 179, 181, GantryDirection.CounterClockwise, 0, isocenter);
                    beam1.Id = BEAM_1_ID;
                }
                Beam beam2 = plan.Beams.FirstOrDefault(x => x.Id == BEAM_2_ID);
                if (beam2 == null)
                {
                    beam2 = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 181, 179, GantryDirection.Clockwise, 0, isocenter);
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
                catch { MessageBox.Show(string.Format("Could not place beams for some reason..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

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

                string DVHestimationModelId = "Pelvis/Abdominal";

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
                try { if ((ss.Structures.FirstOrDefault(x => x.Id == "FemoralHead_L")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "FemoralHead_L").Id, "FemoralHead_L"); } } catch { }
                try { if ((ss.Structures.FirstOrDefault(x => x.Id == "FemoralHead_R")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "FemoralHead_R").Id, "FemoralHead_R"); } } catch { }


                plan.CalculateDVHEstimates(DVHestimationModelId, targetDoseLevels, structureMatches);

                GreenBar.Value = 35;

                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);
                text.Text = "Analyze volume dose...";
                GreenBarListBox.Items.Add(text.Text);



                //Begin VMAT optimization
                //Check if the dose already has been calculated. If so, use it is intermediate dose
                if (plan.Dose != null)
                {
                    GreenBar.Value = 70;

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
                plan.CalculateDose();

                GreenBar.Value = 100;
                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);
                text.Text = "Plan calculation finished!";
                GreenBarListBox.Items.Add(text.Text);








                try
                {

                    //Show report message
                    var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                    //Define DVH parameters to report
                    var PTV_V95 = plan.GetDoseAtVolume(ptv, 95.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                    MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! PTV: D95%[Gy] = {0}. Overall calculation time is {1}", PTV_V95, overall_calculation_time), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);

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
                const string SCRIPT_NAME = "AutoPlan_Pelvis";
                //ID of the course
                const string COURSE_ID = "Treatment";
                //ID of the plan
                string PLAN_ID = string.Format("Plvs_{0}Gy", (Math.Round((NOF * DPF), 0)).ToString());


                // Change these IDs to match your clinical protocol
                const string PTV_ID = "PTV";
                const string MVCBCT_ID = "MVCBCT";
                const string BEAM_1_ID = "Plvs179-181/20";
                const string BEAM_2_ID = "Plvs181-179/340";
                const string RECTUM_ID = "Rectum";
                const string OPT_RECTUM_ID = "Rectum-PTV";
                const string BLADDER_ID = "Bladder";
                const string OPT_BLADDER_ID = "Bladder-PTV";
                const string BLOCK_ID = "Block";
                const string BOWELBAG_ID = "BowelBag";
                const string OPT_BOWELBAG_ID = "BowelBag-PTV";
                const string BODY_ID = "BODY";



                //Show greetings window
                string Greeting = string.Format("Greetings {0}! Please, ensure that structure 'PTV' exist in the currest structure set. Script is made by 'PET_Tehnology'\n  Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru').", user);
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

                //find PTV
                Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                if (ptv == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return;
                }


                //Find OARs and optimization structures
                Structure rectum = ss.Structures.FirstOrDefault(x => x.Id == RECTUM_ID);
                if (rectum == null)
                {
                    NOT_FOUND_list.Add(RECTUM_ID);
                }

                Structure opt_rectum = ss.Structures.FirstOrDefault(x => x.Id == OPT_RECTUM_ID);
                if (opt_rectum == null)
                {
                    NOT_FOUND_list.Add(OPT_RECTUM_ID);
                }

                Structure bladder = ss.Structures.FirstOrDefault(x => x.Id == BLADDER_ID);
                if (bladder == null)
                {
                    NOT_FOUND_list.Add(BLADDER_ID);
                }

                Structure opt_bladder = ss.Structures.FirstOrDefault(x => x.Id == OPT_BLADDER_ID);
                if (opt_bladder == null)
                {
                    NOT_FOUND_list.Add(OPT_BLADDER_ID);
                }

                Structure block = ss.Structures.FirstOrDefault(x => x.Id == BLOCK_ID);
                if (block == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found! It is an avoidance structure for beams entry point", BLOCK_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                Structure body = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                if (body == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return; ;
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

                //Check if the beams already exist. If no, create beams
                Beam beam1 = plan.Beams.FirstOrDefault(x => x.Id == BEAM_1_ID);
                if (beam1 == null)
                {
                    beam1 = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 20, 179, 181, GantryDirection.CounterClockwise, 0, isocenter);
                    beam1.Id = BEAM_1_ID;
                }
                Beam beam2 = plan.Beams.FirstOrDefault(x => x.Id == BEAM_2_ID);
                if (beam2 == null)
                {
                    beam2 = plan.AddArcBeam(MachineParameters, new VRect<double>(280, 280, 280, 280), 340, 181, 179, GantryDirection.Clockwise, 0, isocenter);
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
                catch { MessageBox.Show(string.Format("Could not place beams for some reason..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

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
                    catch { MessageBox.Show(string.Format("Could not set optimization objectives for:'{0}'", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }



                //Set optimization adjustments for OARs and optimization strucures
                try
                {
                    var rectum_upper = plan.OptimizationSetup.AddPointObjective(rectum, OptimizationObjectiveOperator.Upper, new DoseValue(68.5, "Gy"), 0, 120);
                    if (rectum_upper == null)
                    {
                        plan.OptimizationSetup.AddPointObjective(rectum, OptimizationObjectiveOperator.Upper, new DoseValue(68.5, "Gy"), 0, 120);
                    }
                }
                catch { }
                try
                {
                    var bladder_upper = plan.OptimizationSetup.AddPointObjective(bladder, OptimizationObjectiveOperator.Upper, new DoseValue(68.5, "Gy"), 0, 120);
                    if (bladder_upper == null)
                    {
                        plan.OptimizationSetup.AddPointObjective(bladder, OptimizationObjectiveOperator.Upper, new DoseValue(68.5, "Gy"), 0, 120);
                    }
                }
                catch { }
                try
                {
                    var opt_rectum_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_rectum, new DoseValue(20, "Gy"), 80);
                    if (opt_rectum_mean == null)
                    {
                        plan.OptimizationSetup.AddMeanDoseObjective(opt_rectum, new DoseValue(20, "Gy"), 80);
                    }
                }
                catch { }
                try
                {
                    var opt_bladder_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_bladder, new DoseValue(25, "Gy"), 80);
                    if (opt_bladder_mean == null)
                    {
                        opt_bladder_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_bladder, new DoseValue(25, "Gy"), 80);
                    }
                }
                catch { }


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
                    GreenBar.Value = 70;

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
                plan.CalculateDose();

                GreenBar.Value = 100;
                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);
                text.Text = "Plan calculation finished!";
                GreenBarListBox.Items.Add(text.Text);








                try
                {

                    //Show report message
                    var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                    //Define DVH parameters to report
                    var PTV_V95 = plan.GetDoseAtVolume(ptv, 95.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                    MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! PTV: D95%[Gy] = {0}. Overall calculation time is {1}", PTV_V95, overall_calculation_time), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);

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

        private void RapidPlanCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void RapidPlanCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RapidPlan.IsChecked = false;
        }
    }
}
