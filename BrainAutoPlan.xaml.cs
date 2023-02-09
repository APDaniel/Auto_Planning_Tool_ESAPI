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
    /*public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            using (_timeoutTimer)
                MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
    */

    /// <summary>
    /// Interaction logic for BrainAutoPlan.xaml
    /// </summary>
    public partial class BrainAutoPlan : UserControl
    {
        

        Tuple<int, DoseValue> prescription = null;
        string linac = "";
        string calculation_model = "PO_15.6.06";
        public Patient patient = VMS.TPS.Script.context.Patient;
        User user = VMS.TPS.Script.context.CurrentUser;
        StructureSet ss = VMS.TPS.Script.context.StructureSet;
        double DPF = 2.0;
        int NOF =1;

        private void LinacIDChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
               
        public BrainAutoPlan(ScriptContext context)
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
            if (double.TryParse(dosePerFx.Text, out double dose_perFx) && int.TryParse(numberOfFractions.Text, out int numFractions) && (dose_perFx>0) && (numFractions>0))
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
                    double bED = (TD*(1+(DPF/aplha_beta_double)));
                    double eQD2 = (TD*((DPF+aplha_beta_double)/(2+aplha_beta_double)));
                    EQD2.Text = (Math.Round(eQD2,2)).ToString();
                    BED.Text = (Math.Round(bED, 2)).ToString();
                }
                else { EQD2.Text = ""; BED.Text = ""; }
            }
            else 
            {
                return;
            }
        }
        private void CrtAndCalPlan(object sender, RoutedEventArgs e)
        {

            if (RapidPlan.IsChecked == true) 
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
                const string SCRIPT_NAME = "AutoPlan_Brain";
                //ID of the course
                const string COURSE_ID = "Treatment";
                //ID of the plan
                string PLAN_ID = string.Format("Brain_{0}Gy", (Math.Round((NOF * DPF), 0)).ToString());

                //IDs of fields
                const string MVCBCT_ID = "MVCBCT";
                const string BEAM_1_ID = "Brn179-181/20";
                const string BEAM_2_ID = "Brn181-179/340";

                //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                const string PTV_ID = "PTV";
                const string OPT_PTV_ID = "PTV_OPT";

                //IDs of critical structures
                const string BRAINSTEM_ID = "Brainstem";
                const string OPT_BRAINSTEM_ID = "Brainstem-PTV";
                const string BRAINSTEM_PRV = "Brainstem_PRV";
                const string CHIASM_ID = "Chiasm";
                const string OPT_CHIASM_ID = "Chiasm-PTV";
                const string CHIASM_PRV = "Chiasm_PRV";
                const string EYE_L_ID = "Eye_L";
                const string EYE_R_ID = "Eye_R";
                const string LENS_L_ID = "Lens_L";
                const string LENS_L_PRV_ID = "Lens_L_PRV";
                const string LENS_R_ID = "Lens_R";
                const string LENS_R_PRV_ID = "Lens_R_PRV";
                const string OPTIC_NERVE_L_ID = "OpticNerve_L";
                const string OPTIC_NERVE_L_PRV_ID = "OptNrv_L_PRV";
                const string OPTIC_NERVE_R_ID = "OpticNerve_R";
                const string OPTIC_NERVE_R_PRV_ID = "OptNrv_R_PRV";
                const string BODY_ID = "BODY";



                //Show greetings window
                string Greeting = string.Format("Greetings {0}! Please, ensure that at least one of the structures 'PTV', 'BODY' exist in the currest structure set.\nThis script is made by 'PET_Tehnology'\n Before implementing this script in clinical practise, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
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


                //Create a list to write 'not found structures'
                List<string> NOT_FOUND_list = new List<string>();

                //find PTVs
                Structure opt_ptv = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);
                if (opt_ptv == null)
                {
                    NOT_FOUND_list.Add(OPT_PTV_ID);
                }

                Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                if (ptv == null)
                {
                    NOT_FOUND_list.Add(PTV_ID);
                }


                //Show an error notification if no optimization PTVs will be found
                if (opt_ptv == null || opt_ptv.Volume < 0.01)
                {
                    MessageBox.Show(string.Format("{0} not found or null volume! Please, ensure that it is at least one of mentioned PTVs are presented in the structure set", OPT_PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return;
                }


                //Find OARs and optimization structures
                Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                if (brainstem == null)
                {
                    NOT_FOUND_list.Add(BRAINSTEM_ID);
                }

                Structure opt_brainstem = ss.Structures.FirstOrDefault(x => x.Id == OPT_BRAINSTEM_ID);
                if (opt_brainstem == null)
                {
                    NOT_FOUND_list.Add(OPT_BRAINSTEM_ID);
                }

                Structure opt_brainstem_prv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV);
                if (opt_brainstem_prv == null)
                {
                    NOT_FOUND_list.Add(BRAINSTEM_PRV);
                }

                Structure chiasm = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_ID);
                if (chiasm == null)
                {
                    NOT_FOUND_list.Add(CHIASM_ID);
                }

                Structure opt_chiasm = ss.Structures.FirstOrDefault(x => x.Id == OPT_CHIASM_ID);
                if (opt_chiasm == null)
                {
                    NOT_FOUND_list.Add(OPT_CHIASM_ID);
                }

                Structure chiasm_prv = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_PRV);
                if (chiasm_prv == null)
                {
                    NOT_FOUND_list.Add(CHIASM_PRV);
                }

                Structure eye_L = ss.Structures.FirstOrDefault(x => x.Id == EYE_L_ID);
                if (eye_L == null)
                {
                    NOT_FOUND_list.Add(EYE_L_ID);
                }

                Structure eye_R = ss.Structures.FirstOrDefault(x => x.Id == EYE_R_ID);
                if (eye_R == null)
                {
                    NOT_FOUND_list.Add(EYE_R_ID);
                }

                Structure lens_L = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_ID);
                if (lens_L == null)
                {
                    NOT_FOUND_list.Add(LENS_L_ID);
                }

                Structure lens_L_prv = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_PRV_ID);
                if (lens_L == null)
                {
                    NOT_FOUND_list.Add(LENS_L_PRV_ID);
                }

                Structure lens_R = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_ID);
                if (lens_R == null)
                {
                    NOT_FOUND_list.Add(LENS_R_ID);
                }

                Structure lens_R_prv = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_PRV_ID);
                if (lens_R == null)
                {
                    NOT_FOUND_list.Add(LENS_R_PRV_ID);
                }

                Structure optic_nrv_L = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_ID);
                if (optic_nrv_L == null)
                {
                    NOT_FOUND_list.Add(OPTIC_NERVE_L_ID);
                }

                Structure optic_nrv_L_prv = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_PRV_ID);
                if (optic_nrv_L_prv == null)
                {
                    NOT_FOUND_list.Add(OPTIC_NERVE_L_PRV_ID);
                }

                Structure optic_nrv_R = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_ID);
                if (optic_nrv_R == null)
                {
                    NOT_FOUND_list.Add(OPTIC_NERVE_R_ID);
                }

                Structure optic_nrv_R_prv = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_PRV_ID);
                if (optic_nrv_R_prv == null)
                {
                    NOT_FOUND_list.Add(OPTIC_NERVE_R_PRV_ID);
                }

                Structure body = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                if (body == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return;
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
                new ExternalBeamMachineParameters(linac, "6X", 800, "ARC", "FFF");


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

                GreenBar.Value = 25;

                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Placing beams in plan...";
                GreenBarListBox.Items.Add(text.Text);




                //Fit collimator to structure. For Halcyon machines the next 4 strings are not necessary
                bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                beam1.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                beam2.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                FitToStructureMargins margins = new FitToStructureMargins(0);

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
                plan.OptimizationSetup.AddNormalTissueObjective(120, 1, 103, 45, 0.12);


                //Set optimization adjustments for PTVs
                GreenBar.Value = 30;

                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Defining objectives...";
                GreenBarListBox.Items.Add(text.Text);



                //Choose estimation model ID
                string DVHestimationModelId = "Brain";


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
                try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Esophagus")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Esophagus").Id, "Esophagus"); } } catch { }
                try { if ((ss.Structures.FirstOrDefault(x => x.Id == "Esophagus-PTV")).IsEmpty != true) { structureMatches.Add(ss.Structures.FirstOrDefault(x => x.Id == "Esophagus-PTV").Id, "Esophagus-PTV"); } } catch { }
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
                    var PTV_V95 = plan.GetDoseAtVolume(opt_ptv, 95.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                    //Show report message
                    var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                    MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! Plan calculation took '{0}' minutes\n 95% '{1}' covered by '{2}' Gy",


                        overall_calculation_time,
                        OPT_PTV_ID, PTV_V95

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
                const string SCRIPT_NAME = "AutoPlan_Brain";
                //ID of the course
                const string COURSE_ID = "Treatment";
                //ID of the plan
                string PLAN_ID = string.Format("Brain_{0}Gy", (Math.Round((NOF * DPF), 0)).ToString());

                //IDs of fields
                const string MVCBCT_ID = "MVCBCT";
                const string BEAM_1_ID = "Brn179-181/20";
                const string BEAM_2_ID = "Brn181-179/340";

                //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                const string PTV_ID = "PTV";
                const string OPT_PTV_ID = "PTV_OPT";

                //IDs of critical structures
                const string BRAINSTEM_ID = "Brainstem";
                const string OPT_BRAINSTEM_ID = "Brainstem-PTV";
                const string BRAINSTEM_PRV = "Brainstem_PRV";
                const string CHIASM_ID = "Chiasm";
                const string OPT_CHIASM_ID = "Chiasm-PTV";
                const string CHIASM_PRV = "Chiasm_PRV";
                const string EYE_L_ID = "Eye_L";
                const string EYE_R_ID = "Eye_R";
                const string LENS_L_ID = "Lens_L";
                const string LENS_L_PRV_ID = "Lens_L_PRV";
                const string LENS_R_ID = "Lens_R";
                const string LENS_R_PRV_ID = "Lens_R_PRV";
                const string OPTIC_NERVE_L_ID = "OpticNerve_L";
                const string OPTIC_NERVE_L_PRV_ID = "OptNrv_L_PRV";
                const string OPTIC_NERVE_R_ID = "OpticNerve_R";
                const string OPTIC_NERVE_R_PRV_ID = "OptNrv_R_PRV";
                const string BODY_ID = "BODY";



                //Show greetings window
                string Greeting = string.Format("Greetings {0}! Please, ensure that at least one of the structures 'PTV', 'BODY' exist in the currest structure set.\nThis script is made by 'PET_Tehnology'\n Before implementing this script in clinical practise, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
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


                //Create a list to write 'not found structures'
                List<string> NOT_FOUND_list = new List<string>();

                //find PTVs
                Structure opt_ptv = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);
                if (opt_ptv == null)
                {
                    NOT_FOUND_list.Add(OPT_PTV_ID);
                }

                Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                if (ptv == null)
                {
                    NOT_FOUND_list.Add(PTV_ID);
                }


                //Show an error notification if no optimization PTVs will be found
                if (opt_ptv == null || opt_ptv.Volume < 0.01)
                {
                    MessageBox.Show(string.Format("{0} not found or null volume! Please, ensure that it is at least one of mentioned PTVs are presented in the structure set", OPT_PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return;
                }


                //Find OARs and optimization structures
                Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                if (brainstem == null)
                {
                    NOT_FOUND_list.Add(BRAINSTEM_ID);
                }

                Structure opt_brainstem = ss.Structures.FirstOrDefault(x => x.Id == OPT_BRAINSTEM_ID);
                if (opt_brainstem == null)
                {
                    NOT_FOUND_list.Add(OPT_BRAINSTEM_ID);
                }

                Structure opt_brainstem_prv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV);
                if (opt_brainstem_prv == null)
                {
                    NOT_FOUND_list.Add(BRAINSTEM_PRV);
                }

                Structure chiasm = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_ID);
                if (chiasm == null)
                {
                    NOT_FOUND_list.Add(CHIASM_ID);
                }

                Structure opt_chiasm = ss.Structures.FirstOrDefault(x => x.Id == OPT_CHIASM_ID);
                if (opt_chiasm == null)
                {
                    NOT_FOUND_list.Add(OPT_CHIASM_ID);
                }

                Structure chiasm_prv = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_PRV);
                if (chiasm_prv == null)
                {
                    NOT_FOUND_list.Add(CHIASM_PRV);
                }

                Structure eye_L = ss.Structures.FirstOrDefault(x => x.Id == EYE_L_ID);
                if (eye_L == null)
                {
                    NOT_FOUND_list.Add(EYE_L_ID);
                }

                Structure eye_R = ss.Structures.FirstOrDefault(x => x.Id == EYE_R_ID);
                if (eye_R == null)
                {
                    NOT_FOUND_list.Add(EYE_R_ID);
                }

                Structure lens_L = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_ID);
                if (lens_L == null)
                {
                    NOT_FOUND_list.Add(LENS_L_ID);
                }

                Structure lens_L_prv = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_PRV_ID);
                if (lens_L == null)
                {
                    NOT_FOUND_list.Add(LENS_L_PRV_ID);
                }

                Structure lens_R = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_ID);
                if (lens_R == null)
                {
                    NOT_FOUND_list.Add(LENS_R_ID);
                }

                Structure lens_R_prv = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_PRV_ID);
                if (lens_R == null)
                {
                    NOT_FOUND_list.Add(LENS_R_PRV_ID);
                }

                Structure optic_nrv_L = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_ID);
                if (optic_nrv_L == null)
                {
                    NOT_FOUND_list.Add(OPTIC_NERVE_L_ID);
                }

                Structure optic_nrv_L_prv = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_PRV_ID);
                if (optic_nrv_L_prv == null)
                {
                    NOT_FOUND_list.Add(OPTIC_NERVE_L_PRV_ID);
                }

                Structure optic_nrv_R = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_ID);
                if (optic_nrv_R == null)
                {
                    NOT_FOUND_list.Add(OPTIC_NERVE_R_ID);
                }

                Structure optic_nrv_R_prv = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_PRV_ID);
                if (optic_nrv_R_prv == null)
                {
                    NOT_FOUND_list.Add(OPTIC_NERVE_R_PRV_ID);
                }

                Structure body = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                if (body == null)
                {
                    MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                    Window.GetWindow(this).Close(); return;
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
                new ExternalBeamMachineParameters(linac, "6X", 800, "ARC", "FFF");


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

                GreenBar.Value = 25;

                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Placing beams in plan...";
                GreenBarListBox.Items.Add(text.Text);




                //Fit collimator to structure. For Halcyon machines the next 4 strings are not necessary
                bool useAsymmetricXJaw = false, useAsymmetricYJaws = false, optimizeCollimatorRotation = false;
                beam1.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                beam2.FitCollimatorToStructure(new FitToStructureMargins(0), ptv, useAsymmetricXJaw, useAsymmetricYJaws, optimizeCollimatorRotation);
                FitToStructureMargins margins = new FitToStructureMargins(0);

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
                plan.OptimizationSetup.AddNormalTissueObjective(120, 1, 103, 45, 0.12);


                //Set optimization adjustments for PTVs
                GreenBar.Value = 30;

                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Defining objectives...";
                GreenBarListBox.Items.Add(text.Text);




                //PTV
                if (opt_ptv != null & (opt_ptv.Volume > 0.1))
                {
                    double target_lower = Math.Round((DPF * NOF), 2);
                    double target_upper = Math.Round(((DPF * NOF) * 1.05), 2);
                    try
                    {
                        var opt_ptv_lower = plan.OptimizationSetup.AddPointObjective(opt_ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                        var opt_ptv_upper = plan.OptimizationSetup.AddPointObjective(opt_ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);

                        if (opt_ptv_lower == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(opt_ptv, OptimizationObjectiveOperator.Lower, new DoseValue(target_lower, "Gy"), 99.9, 100);
                        }
                        if (opt_ptv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(opt_ptv, OptimizationObjectiveOperator.Upper, new DoseValue(target_upper, "Gy"), 0, 100);
                        }
                    }
                    catch { MessageBox.Show(string.Format("Optimization objectives for:'{0}' could not be set", OPT_PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }

                //Set optimization adjustments for OARs and optimization strucures


                //Brainstem
                if (brainstem != null & (brainstem.Volume > 0.1))
                {
                    try
                    {
                        var brainstem_upper = plan.OptimizationSetup.AddPointObjective(brainstem, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 150);

                        if (brainstem_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(brainstem, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 150);
                        }
                    }
                    catch { MessageBox.Show(string.Format("Optimization objectives for:'{0}' could not be set", BRAINSTEM_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }


                //Brainstem PRV
                if (opt_brainstem_prv != null & (opt_brainstem_prv.Volume > 0.1))
                {
                    try
                    {
                        var opt_brainstem_prv_upper = plan.OptimizationSetup.AddPointObjective(opt_brainstem_prv, OptimizationObjectiveOperator.Upper, new DoseValue(51, "Gy"), 0, 150);

                        if (opt_brainstem_prv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(opt_brainstem_prv, OptimizationObjectiveOperator.Upper, new DoseValue(51, "Gy"), 0, 150);
                        }
                    }
                    catch { MessageBox.Show(string.Format("Optimization objectives for:'{0}' could not be set", BRAINSTEM_PRV), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }


                //Brainstem-PTV
                if (opt_brainstem != null & (opt_brainstem.Volume > 0.1))
                {
                    try
                    {
                        var opt_brainstem_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_brainstem, new DoseValue(30, "Gy"), 80);
                        if (opt_brainstem_mean == null)
                        {
                            opt_brainstem_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_brainstem, new DoseValue(30, "Gy"), 80);
                        }
                    }
                    catch { }
                }


                //Chiasm
                if (chiasm != null & (chiasm.Volume > 0.1))
                {
                    try
                    {
                        var chiasm_upper = plan.OptimizationSetup.AddPointObjective(chiasm, OptimizationObjectiveOperator.Upper, new DoseValue(50, "Gy"), 0, 150);

                        if (chiasm_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(chiasm, OptimizationObjectiveOperator.Upper, new DoseValue(50, "Gy"), 0, 150);
                        }
                    }
                    catch { }
                }


                //Chiasm-PTV
                if (opt_chiasm != null & (opt_chiasm.Volume > 0.1))
                {
                    try
                    {
                        var opt_chiasm_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_chiasm, new DoseValue(30, "Gy"), 80);
                        if (opt_chiasm_mean == null)
                        {
                            opt_chiasm_mean = plan.OptimizationSetup.AddMeanDoseObjective(opt_chiasm, new DoseValue(30, "Gy"), 80);
                        }
                    }
                    catch { }
                }

                //Eye_L
                if (eye_L != null & (eye_L.Volume > 0.1))
                {
                    try
                    {
                        var eye_L_upper = plan.OptimizationSetup.AddPointObjective(eye_L, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 120);

                        if (eye_L_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(eye_L, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 120);
                        }
                    }
                    catch { }
                }


                //Eye_R
                if (eye_R != null & (eye_R.Volume > 0.1))
                {
                    try
                    {
                        var eye_R_upper = plan.OptimizationSetup.AddPointObjective(eye_R, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 120);

                        if (eye_R_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(eye_R, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 120);
                        }
                    }
                    catch { }
                }

                //Lens_L_PRV
                if (lens_L_prv != null & (lens_L_prv.Volume > 0.1))
                {
                    try
                    {
                        var lens_L_prv_upper = plan.OptimizationSetup.AddPointObjective(lens_L_prv, OptimizationObjectiveOperator.Upper, new DoseValue(10, "Gy"), 0, 120);

                        if (lens_L_prv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(lens_L_prv, OptimizationObjectiveOperator.Upper, new DoseValue(10, "Gy"), 0, 120);
                        }
                    }
                    catch { }
                }


                //Lens_R_PRV
                if (lens_R_prv != null & (lens_R_prv.Volume > 0.1))
                {
                    try
                    {
                        var lens_R_prv_upper = plan.OptimizationSetup.AddPointObjective(lens_R_prv, OptimizationObjectiveOperator.Upper, new DoseValue(10, "Gy"), 0, 120);

                        if (lens_R_prv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(lens_R_prv, OptimizationObjectiveOperator.Upper, new DoseValue(10, "Gy"), 0, 120);
                        }
                    }
                    catch { }
                }

                //Optic_nerve_L
                if (optic_nrv_L != null & (optic_nrv_L.Volume > 0.1))
                {
                    try
                    {
                        var optic_nrv_L_upper = plan.OptimizationSetup.AddPointObjective(optic_nrv_L, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 150);

                        if (optic_nrv_L_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(optic_nrv_L, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 150);
                        }
                    }
                    catch { }
                }


                //Optic_nerve_L_PRV
                if (optic_nrv_L_prv != null & (optic_nrv_L_prv.Volume > 0.1))
                {
                    try
                    {
                        var optic_nrv_L_prv_upper = plan.OptimizationSetup.AddPointObjective(optic_nrv_L_prv, OptimizationObjectiveOperator.Upper, new DoseValue(50, "Gy"), 0, 150);

                        if (optic_nrv_L_prv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(optic_nrv_L_prv, OptimizationObjectiveOperator.Upper, new DoseValue(50, "Gy"), 0, 150);
                        }
                    }
                    catch { }
                }


                //Optic_nerve_R
                if (optic_nrv_R != null & (optic_nrv_R.Volume > 0.1))
                {
                    try
                    {
                        var optic_nrv_R_upper = plan.OptimizationSetup.AddPointObjective(optic_nrv_R, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 150);

                        if (optic_nrv_R_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(optic_nrv_R, OptimizationObjectiveOperator.Upper, new DoseValue(48, "Gy"), 0, 150);
                        }
                    }
                    catch { }
                }


                //Optic_nerve_L_PRV
                if (optic_nrv_R_prv != null & (optic_nrv_R_prv.Volume > 0.1))
                {
                    try
                    {
                        var optic_nrv_R_prv_upper = plan.OptimizationSetup.AddPointObjective(optic_nrv_R_prv, OptimizationObjectiveOperator.Upper, new DoseValue(50, "Gy"), 0, 150);

                        if (optic_nrv_R_prv_upper == null)
                        {
                            plan.OptimizationSetup.AddPointObjective(optic_nrv_L_prv, OptimizationObjectiveOperator.Upper, new DoseValue(50, "Gy"), 0, 150);
                        }
                    }
                    catch { }
                }


                //Global maximum limit
                //PharConst-PTV
                if (body != null & (body.Volume > 0.1))
                {
                    double body_upper_dose = Math.Round(((DPF * NOF) * 1.07), 2);
                    var body_upper = plan.OptimizationSetup.AddPointObjective(body, OptimizationObjectiveOperator.Upper, new DoseValue(body_upper_dose, "Gy"), 0, 400);
                    if (body_upper == null)
                    {
                        plan.OptimizationSetup.AddPointObjective(body, OptimizationObjectiveOperator.Upper, new DoseValue(body_upper_dose, "Gy"), 0, 400);
                        MessageBox.Show(string.Format("Please, define avoidance structures manually, if necessary. To continue with calculations, launch this script again"));
                        Window.GetWindow(this).Close(); return;
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

                try { plan.CalculateDose(); } catch { try { MessageBox.Show(string.Format("Dear {0},\nUnfortunately, something went wrong during volume dose calculations.\n\nPlease, perform volume dose calculations manually", user), "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }

                GreenBar.Value = 100;
                text.Text = "Completed...";
                GreenBarListBox.Items.Add(text.Text);

                text.Text = "Plan calculation finished!";
                GreenBarListBox.Items.Add(text.Text);




                try
                {
                    //Define DVH parameters to report
                    var PTV_V95 = plan.GetDoseAtVolume(opt_ptv, 95.0f, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                    //Show report message
                    var overall_calculation_time = Math.Round((((DateTime.Now - stopwatch).TotalSeconds) / 60));
                    MessageBox.Show(string.Format("Optimization and dose calculation completed successfuly! Plan calculation took '{0}' minutes\n 95% '{1}' covered by '{2}' Gy",


                        overall_calculation_time,
                        OPT_PTV_ID, PTV_V95

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
