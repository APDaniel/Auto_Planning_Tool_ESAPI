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
    /// Interaction logic for OPT_STR.xaml
    /// </summary>
    public partial class OPT_STR : UserControl
    {
        public Patient patient = null;
        User user = null;
        StructureSet ss = null;

        public OPT_STR(ScriptContext context)
        {
            InitializeComponent();
                        
            //===========================================================================================================================================================================
            //===========================================================================================================================================================================
            //Check if the patient, structure set are loaded. If yes, then download structure set. If no, show the error message to the user and stop execution

            if (VMS.TPS.Script.context.Patient == null || VMS.TPS.Script.context.StructureSet == null)
            {
                MessageBox.Show("Please load a patient, 3D image, and structure set before running this script.", "No patient and StructureSet loaded!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Window.GetWindow(this).Close(); return;;
            }
            StructureSet ss = VMS.TPS.Script.context.StructureSet;
            //===========================================================================================================================================================================
            //===========================================================================================================================================================================
            patient = VMS.TPS.Script.context.Patient;
            user = VMS.TPS.Script.context.CurrentUser;
            ss = VMS.TPS.Script.context.StructureSet;
            CrtStrsBYtmplt.IsEnabled = false;
            //download available structuresets for the opened patient
            try
            {
                foreach (StructureSet structureSet in context.Patient.StructureSets)
                {
                    if (structureSet.Id.Contains("CBCT") == false)
                    {
                        StructureSetID.Items.Add(structureSet.Id);
                    }
                }
            }
            catch { }
            
        }
        private void HeadAndNeck_Checked(object sender, RoutedEventArgs e)
        {
            HeadAndNeckRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }
        private void Brain_Checked(object sender, RoutedEventArgs e)
        {
            BrainRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }
        private void Abdominal_Checked(object sender, RoutedEventArgs e)
        {
            AbdominalRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void Breast_Checked(object sender, RoutedEventArgs e)
        {
            BreastRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void Esophagus_Checked(object sender, RoutedEventArgs e)
        {
            EsophagusRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void Pelvis_Checked(object sender, RoutedEventArgs e)
        {
            PelvisRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void Lungs_Checked(object sender, RoutedEventArgs e)
        {
            LungsRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void SBRT_Thorax_Checked(object sender, RoutedEventArgs e)
        {
            SBRT_ThoraxRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void SBRT_Abdominal_Checked(object sender, RoutedEventArgs e)
        {
            SBRT_AbdominalRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void SBRT_Pelvis_Checked(object sender, RoutedEventArgs e)
        {
            SBRT_PelvisRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void SRS_Brain1target_Checked(object sender, RoutedEventArgs e)
        {
            SRS_Brain1targetRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void SRS_BrainMultipleTargets_Checked(object sender, RoutedEventArgs e)
        {
            SRS_BrainMultipleTargetsRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }

        private void CrtStrBYtmplt_Click(object sender, RoutedEventArgs e)
        {

            //Apply selected in combobox StructureSet for script execution
            try { ss = VMS.TPS.Script.context.Patient.StructureSets.FirstOrDefault(x => x.Id == StructureSetID.Text); }
            catch { }
            try
            {
                if (BrainRB.IsChecked == true)
                {
                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_Brain";

                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_ID = "PTV";


                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_PTV_ID = "PTV_ALL_3mm";

                    //IDs of critical structures
                    const string OPT_PTV_ID = "PTV_OPT";
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
                    const string NAPE_ID = "Nape";
                    const string BLOCK_ID = "Block";
                    const string BODY_ID = "BODY";
                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structures: PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution
                    List<string> NOT_FOUND_list = new List<string>();
                    //find PharynxConst


                    //find BrainStem
                    Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                    if (brainstem == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_ID);
                        try { brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { }
                    }

                    //find Chiasm
                    Structure chiasm = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_ID);
                    if (chiasm == null)
                    {
                        NOT_FOUND_list.Add(CHIASM_ID);
                        try { chiasm = ss.AddStructure("Avoidance", CHIASM_ID); } catch { }
                    }


                    //find Eye_L
                    Structure eye_L = ss.Structures.FirstOrDefault(x => x.Id == EYE_L_ID);
                    if (eye_L == null)
                    {
                        NOT_FOUND_list.Add(EYE_L_ID);
                        try { eye_L = ss.AddStructure("Avoidance", EYE_L_ID); } catch { }
                    }

                    //find Eye_R
                    Structure eye_R = ss.Structures.FirstOrDefault(x => x.Id == EYE_R_ID);
                    if (eye_R == null)
                    {
                        NOT_FOUND_list.Add(EYE_R_ID);
                        try { eye_R = ss.AddStructure("Avoidance", EYE_R_ID); } catch { }
                    }


                    //find Lens_L
                    Structure lens_L = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_ID);
                    if (lens_L == null)
                    {
                        NOT_FOUND_list.Add(LENS_L_ID);
                        try { lens_L = ss.AddStructure("Avoidance", LENS_L_ID); } catch { }
                    }

                    //find Lens_R
                    Structure lens_R = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_ID);
                    if (lens_R == null)
                    {
                        NOT_FOUND_list.Add(LENS_R_ID);
                        try { lens_R = ss.AddStructure("Avoidance", LENS_R_ID); } catch { }
                    }

                    //find OpticNerve_L
                    Structure opticnerve_L = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_ID);
                    if (opticnerve_L == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_NERVE_L_ID);
                        try { opticnerve_L = ss.AddStructure("Avoidance", OPTIC_NERVE_L_ID); } catch { }
                    }

                    //find OpticNerve_R
                    Structure opticnerve_R = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_ID);
                    if (opticnerve_R == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_NERVE_R_ID);
                        try { opticnerve_R = ss.AddStructure("Avoidance", OPTIC_NERVE_R_ID); } catch { }
                    }
                    //find Nape
                    Structure nape = ss.Structures.FirstOrDefault(x => x.Id == NAPE_ID);
                    if (nape == null)
                    {
                        NOT_FOUND_list.Add(NAPE_ID);
                        try { nape = ss.AddStructure("Avoidance", NAPE_ID); } catch { }
                    }
                    //find Block
                    Structure block = ss.Structures.FirstOrDefault(x => x.Id == BLOCK_ID);
                    if (block == null)
                    {
                        NOT_FOUND_list.Add(BLOCK_ID);
                        try { block = ss.AddStructure("Avoidance", BLOCK_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV 54/60/66 structures in the structure set. If some PTVs will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Window.GetWindow(this).Close(); return;
                    }


                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("No PTV has been found in the Structure Set. Please, ensure that you have at PTVin the StructureSet"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Window.GetWindow(this).Close(); return;
                    }





                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Check if the optimization structures already exist. If so, delete it from the structure set


                    Structure brainstem_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_BRAINSTEM_ID);
                    if (brainstem_PTV != null)
                    {
                        try { ss.RemoveStructure(brainstem_PTV); } catch { }
                    }

                    Structure brainstem_PRV = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV);
                    if (brainstem_PRV != null)
                    {
                        try { ss.RemoveStructure(brainstem_PRV); } catch { }
                    }

                    Structure chiasm_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_CHIASM_ID);
                    if (chiasm_PTV != null)
                    {
                        try { ss.RemoveStructure(chiasm_PTV); } catch { }
                    }

                    Structure chiasm_PRV = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_PRV);
                    if (chiasm_PRV != null)
                    {
                        try { ss.RemoveStructure(chiasm_PRV); } catch { }
                    }


                    Structure lens_L_PRV = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_PRV_ID);
                    if (lens_L_PRV != null)
                    {
                        try { ss.RemoveStructure(lens_L_PRV); } catch { }
                    }

                    Structure lens_R_PRV = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_PRV_ID);
                    if (lens_R_PRV != null)
                    {
                        try { ss.RemoveStructure(lens_R_PRV); } catch { }
                    }


                    Structure opt_nrv_L_PRV = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_PRV_ID);
                    if (opt_nrv_L_PRV != null)
                    {
                        try { ss.RemoveStructure(opt_nrv_L_PRV); } catch { }
                    }

                    Structure opt_nrv_R_PRV = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_PRV_ID);
                    if (opt_nrv_R_PRV != null)
                    {
                        try { ss.RemoveStructure(opt_nrv_R_PRV); } catch { }
                    }

                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);
                    if (ptv_opt != null)
                    {
                        try { ss.RemoveStructure(ptv_opt); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script


                    try { ptv_opt = ss.AddStructure("PTV", OPT_PTV_ID); } catch { }
                    try { brainstem_PTV = ss.AddStructure("Avoidance", OPT_BRAINSTEM_ID); } catch { }
                    try { brainstem_PRV = ss.AddStructure("Avoidance", BRAINSTEM_PRV); } catch { }
                    try
                    {
                        chiasm_PTV = ss.AddStructure("Avoidance", OPT_CHIASM_ID);
                    }
                    catch { }
                    try
                    {
                        chiasm_PRV = ss.AddStructure("Avoidance", CHIASM_PRV);
                    }
                    catch { }
                    try
                    {
                        lens_L_PRV = ss.AddStructure("Avoidance", LENS_L_PRV_ID);
                    }
                    catch { }
                    try
                    {
                        lens_R_PRV = ss.AddStructure("Avoidance", LENS_R_PRV_ID);
                    }
                    catch { }
                    try
                    {
                        opt_nrv_L_PRV = ss.AddStructure("Avoidance", OPTIC_NERVE_L_PRV_ID);
                    }
                    catch { }
                    try
                    {
                        opt_nrv_R_PRV = ss.AddStructure("Avoidance", OPTIC_NERVE_R_PRV_ID);
                    }
                    catch { }


                    //Create trash structures
                    Structure ptv_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_PTV_ID);
                    try
                    {
                        ptv_3mm = ss.AddStructure("PTV", EXPANDED_PTV_ID);
                        ptv_opt.SegmentVolume = ptv.Margin(0);
                    }
                    catch { }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Define PRVs with 5mm margin. If a structure is empty, the empty PRV will be created

                    if (brainstem != null)
                    {
                        try { brainstem_PRV.SegmentVolume = brainstem.Margin(5.0); } catch { }
                    }
                    if (brainstem == null)
                    {
                        try { brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { }
                    }

                    if (chiasm != null)
                    {
                        try { chiasm_PRV.SegmentVolume = chiasm.Margin(5.0); } catch { }
                    }
                    if (chiasm == null)
                    {
                        try { chiasm = ss.AddStructure("Avoidance", CHIASM_ID); } catch { }
                    }

                    if (lens_L != null)
                    {
                        try { lens_L_PRV.SegmentVolume = lens_L.Margin(5.0); } catch { }
                    }
                    if (lens_L == null)
                    {
                        try { lens_L = ss.AddStructure("Avoidance", LENS_L_ID); } catch { }
                    }

                    if (lens_R != null)
                    {
                        lens_R_PRV.SegmentVolume = lens_R.Margin(5.0);
                    }
                    if (lens_R == null)
                    {
                        try { lens_R = ss.AddStructure("Avoidance", LENS_R_ID); } catch { }
                    }

                    if (opticnerve_L != null)
                    {
                        try { opt_nrv_L_PRV.SegmentVolume = opticnerve_L.Margin(5.0); } catch { }
                    }
                    if (opticnerve_L == null)
                    {
                        try { opticnerve_L = ss.AddStructure("Avoidance", OPTIC_NERVE_L_ID); } catch { }
                    }

                    if (opticnerve_R != null)
                    {
                        try { opt_nrv_R_PRV.SegmentVolume = opticnerve_R.Margin(5.0); } catch { }
                    }
                    if (opticnerve_R == null)
                    {
                        try { opticnerve_R = ss.AddStructure("Avoidance", OPTIC_NERVE_R_ID); } catch { }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //
                    if (ptv.IsHighResolution)
                    {
                        foreach (Structure s in ss.Structures)
                        {
                            if (s.IsHighResolution == false && s.Id != "BODY" && s.Id != "CouchSurface" && s.Id != "CouchInterior")
                            {
                                try
                                {
                                    s.ConvertToHighResolution();
                                }
                                catch
                                {
                                    MessageBox.Show(string.Format("It is not possible to convert {0} to high resolution", s.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }

                            }
                        }

                    }


                    //Substract critical structures from expanded PTV_ALL to create 3mm buffer

                    if (brainstem != null)
                    {
                        try { brainstem_PTV.SegmentVolume = brainstem.Sub(ptv_3mm); } catch { }
                        try { ptv_opt.SegmentVolume = ptv_opt.Sub(brainstem_PTV); }
                        catch { }
                    }
                    if (brainstem == null)
                    {
                        try { brainstem_PTV = ss.AddStructure("AVOIDANCE", OPT_BRAINSTEM_ID); } catch { }
                    }

                    if (chiasm != null)
                    {
                        try { chiasm_PTV.SegmentVolume = chiasm.Sub(ptv_3mm); } catch { }
                        try { ptv_opt.SegmentVolume = ptv_opt.Sub(chiasm_PTV); }
                        catch { }
                    }
                    if (chiasm == null)
                    {
                        try { chiasm_PTV = ss.AddStructure("AVOIDANCE", OPT_CHIASM_ID); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}",

                            ptv.Id, ptv.Volume,

                            brainstem.Id, brainstem.Volume,

                            brainstem_PTV.Id, brainstem_PTV.Volume,

                            brainstem_PRV.Id, brainstem_PRV.Volume,

                            opticnerve_L.Id, opticnerve_L.Volume,

                            opt_nrv_L_PRV.Id, opt_nrv_L_PRV.Volume,

                            opticnerve_R.Id, opticnerve_R.Volume,

                            opt_nrv_R_PRV.Id, opt_nrv_R_PRV.Volume,

                            chiasm.Id, chiasm.Volume,

                            chiasm_PRV.Id, chiasm_PRV.Volume,

                            chiasm_PTV.Id, chiasm_PTV.Volume
                            );



                        //Show the messages

                        MessageBox.Show(message_PTV);

                    }
                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown...")); }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations


                    try { ss.RemoveStructure(ptv_3mm); } catch { }

                }
                if (HeadAndNeckRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_HN_54_60_66";

                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_54_ID = "PTV_54";
                    const string OPT_PTV_54_ID = "PTV_54_OPT";
                    const string PTV_60_ID = "PTV_60";
                    const string OPT_PTV_60_ID = "PTV_60_OPT";
                    const string PTV_66_ID = "PTV_66";
                    const string OPT_PTV_66_ID = "PTV_66_OPT";
                    const string PTV_ALL_ID = "PTV_ALL";
                    const string PTV_ID = "PTV";
                    const string PTV_OPT_ID = "PTV_OPT";



                    //Due to the inability to add structures one to another (boolean "And" seems to be having malfunction), it is mandatory to create some "trash" structures for the purpose of creating PTV summ, Lungs and Parotids. These structures will be removed at the end of this script
                    const string BODY_PTV_54_ID = "BODY-PTV_54";
                    const string BODY_PTV_54_60_ID = "BODY_54_60";
                    const string BODY_PTV_54_60_66_ID = "BODY_54_60_66";
                    const string EXPANDED_PTV_54_ID = "PTV_54_3mm";
                    const string EXPANDED_PTV_60_ID = "PTV_60_3mm";
                    const string EXPANDED_PTV_66_ID = "PTV_66_3mm";
                    const string BODY_LUNG_L_ID = "BODY_LUNG_L";
                    const string BODY_LUNG_L_R_ID = "BODY_LUNG_L_R";
                    const string BODY_PAROTID_L_ID = "BODY_PAROTID_L";
                    const string BODY_PAROTID_L_R_ID = "BODY_PAROTID_L_R";
                    const string SUB_PTV_54_60 = "sub_54_60";

                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_3MM_PTV_ALL_ID = "PTV_ALL_3mm";

                    //IDs of critical structures

                    const string BRAINSTEM_ID = "Brainstem";
                    const string OPT_BRAINSTEM_ID = "Brainstem-PTV";
                    const string BRAINSTEM_PRV = "Brainstem_PRV";
                    const string CHIASM_ID = "Chiasm";
                    const string OPT_CHIASM_ID = "Chiasm-PTV";
                    const string CHIASM_PRV = "Chiasm_PRV";
                    const string ESOPHAGUS_ID = "Esophagus";
                    const string OPT_ESOPHAGUS_ID = "Esophagus-PTV";
                    const string EYE_L_ID = "Eye_L";
                    const string EYE_R_ID = "Eye_R";
                    const string LARYNX_ID = "Larynx";
                    const string OPT_LARYNX_ID = "Larynx-PTV";
                    const string LENS_L_ID = "Lens_L";
                    const string LENS_L_PRV_ID = "Lens_L_PRV";
                    const string LENS_R_ID = "Lens_R";
                    const string LENS_R_PRV_ID = "Lens_R_PRV";
                    const string LUNG_L_ID = "Lung_L";
                    const string LUNG_R_ID = "Lung_R";
                    const string LUNGS_ID = "Lungs";
                    const string MANDIBLE_ID = "Mandible";
                    const string OPT_MANDIBLE_ID = "Mandible-PTV";
                    const string OPTIC_NERVE_L_ID = "OpticNerve_L";
                    const string OPTIC_NERVE_L_PRV_ID = "OptNrv_L_PRV";
                    const string OPTIC_NERVE_R_ID = "OpticNerve_R";
                    const string OPTIC_NERVE_R_PRV_ID = "OptNrv_R_PRV";
                    const string ORAL_CAVITY_ID = "OralCavity";
                    const string OPT_ORAL_CAVITY_ID = "OralCvt-PTV";
                    const string PAROTID_L_ID = "Parotid_L";
                    const string OPT_PAROTID_L_ID = "Prtd_L-PTV";
                    const string PAROTID_R_ID = "Parotid_R";
                    const string OPT_PAROTID_R_ID = "Prts_R-PTV";
                    const string PAROTIDS_ID = "Parotids";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpnlCrd_PRV";
                    const string THYROID_ID = "Thyroid";
                    const string OPT_THYROID_ID = "Thyroid-PTV";
                    const string TRACHEA_ID = "Trachea";
                    const string OPT_TRACHEA_ID = "Trachea-PTV";
                    const string LIPS_ID = "Lips";
                    const string SUBMANDIBULAR_GLAND_L_ID = "SubmGl_L";
                    const string OPT_SUBMANDIBULAR_GLAND_L_ID = "SubmGl_L-PTV";
                    const string SUBMANDIBULAR_GLAND_R_ID = "SubmGl_R";
                    const string OPT_SUBMANDIBULAR_GLAND_R_ID = "SubmGl_R-PTV";
                    const string PHAR_CONST_ID = "PharynxConst";
                    const string OPT_PHAR_CONST_ID = "PhConst-PTV";
                    const string NAPE_ID = "Nape";
                    const string BLOCK_ID = "Block";
                    const string BODY_ID = "BODY";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structures: PTV_54, PTV_60, PTV_66, BODY exist in the current structure set. This script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);

                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution

                    List<string> NOT_FOUND_list = new List<string>();
                    //find PharynxConst
                    Structure pharconst = ss.Structures.FirstOrDefault(x => x.Id == PHAR_CONST_ID);
                    if (pharconst == null)
                    {
                        NOT_FOUND_list.Add(PHAR_CONST_ID);
                        try { pharconst = ss.AddStructure("Avoidance", PHAR_CONST_ID); } catch { }
                    }

                    //find BrainStem
                    Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                    if (brainstem == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_ID);
                        try { brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { }
                    }

                    //find Chiasm
                    Structure chiasm = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_ID);
                    if (chiasm == null)
                    {
                        NOT_FOUND_list.Add(CHIASM_ID);
                        try { chiasm = ss.AddStructure("Avoidance", CHIASM_ID); } catch { }
                    }


                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }

                    //find Eye_L
                    Structure eye_L = ss.Structures.FirstOrDefault(x => x.Id == EYE_L_ID);
                    if (eye_L == null)
                    {
                        NOT_FOUND_list.Add(EYE_L_ID);
                        try { eye_L = ss.AddStructure("Avoidance", EYE_L_ID); } catch { }
                    }

                    //find Eye_R
                    Structure eye_R = ss.Structures.FirstOrDefault(x => x.Id == EYE_R_ID);
                    if (eye_R == null)
                    {
                        NOT_FOUND_list.Add(EYE_R_ID);
                        try { eye_R = ss.AddStructure("Avoidance", EYE_R_ID); } catch { }
                    }

                    //find Larynx
                    Structure larynx = ss.Structures.FirstOrDefault(x => x.Id == LARYNX_ID);
                    if (larynx == null)
                    {
                        NOT_FOUND_list.Add(LARYNX_ID);
                        try { larynx = ss.AddStructure("Avoidance", LARYNX_ID); } catch { }
                    }

                    //find Lens_L
                    Structure lens_L = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_ID);
                    if (lens_L == null)
                    {
                        NOT_FOUND_list.Add(LENS_L_ID);
                        try { lens_L = ss.AddStructure("Avoidance", LENS_L_ID); } catch { }
                    }

                    //find Lens_R
                    Structure lens_R = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_ID);
                    if (lens_R == null)
                    {
                        NOT_FOUND_list.Add(LENS_R_ID);
                        try { lens_R = ss.AddStructure("Avoidance", LENS_R_ID); } catch { }
                    }

                    //find Lung_L
                    Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_ID);
                    if (lung_L == null)
                    {
                        NOT_FOUND_list.Add(LUNG_L_ID);
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { }
                    }

                    //find Lung_R
                    Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                    if (lung_R == null)
                    {
                        NOT_FOUND_list.Add(LUNG_R_ID);
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }

                    //find Mandible
                    Structure mandible = ss.Structures.FirstOrDefault(x => x.Id == MANDIBLE_ID);
                    if (mandible == null)
                    {
                        NOT_FOUND_list.Add(MANDIBLE_ID);
                        try { mandible = ss.AddStructure("Avoidance", MANDIBLE_ID); } catch { }
                    }

                    //find OpticNerve_L
                    Structure opticnerve_L = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_ID);
                    if (opticnerve_L == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_NERVE_L_ID);
                        try { opticnerve_L = ss.AddStructure("Avoidance", OPTIC_NERVE_L_ID); } catch { }
                    }

                    //find OpticNerve_R
                    Structure opticnerve_R = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_ID);
                    if (opticnerve_R == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_NERVE_R_ID);
                        try { opticnerve_R = ss.AddStructure("Avoidance", OPTIC_NERVE_R_ID); } catch { }
                    }

                    //find OralCavity
                    Structure oralcavity = ss.Structures.FirstOrDefault(x => x.Id == ORAL_CAVITY_ID);
                    if (oralcavity == null)
                    {
                        NOT_FOUND_list.Add(ORAL_CAVITY_ID);
                        try { oralcavity = ss.AddStructure("Avoidance", ORAL_CAVITY_ID); } catch { }
                    }

                    //find Parotid_L
                    Structure parotid_L = ss.Structures.FirstOrDefault(x => x.Id == PAROTID_L_ID);
                    if (parotid_L == null)
                    {
                        NOT_FOUND_list.Add(PAROTID_L_ID);
                        try { parotid_L = ss.AddStructure("Avoidance", PAROTID_L_ID); } catch { }
                    }

                    //find Parotid_R
                    Structure parotid_R = ss.Structures.FirstOrDefault(x => x.Id == PAROTID_R_ID);
                    if (parotid_R == null)
                    {
                        NOT_FOUND_list.Add(PAROTID_R_ID);
                        try { parotid_R = ss.AddStructure("Avoidance", PAROTID_R_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Thyroid
                    Structure thyroid = ss.Structures.FirstOrDefault(x => x.Id == THYROID_ID);
                    if (thyroid == null)
                    {
                        NOT_FOUND_list.Add(THYROID_ID);
                        try { thyroid = ss.AddStructure("Avoidance", THYROID_ID); } catch { }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find Lips
                    Structure lips = ss.Structures.FirstOrDefault(x => x.Id == LIPS_ID);
                    if (lips == null)
                    {
                        NOT_FOUND_list.Add(LIPS_ID);
                        try { lips = ss.AddStructure("Avoidance", LIPS_ID); } catch { }
                    }

                    //find SubmGl_L
                    Structure submgl_L = ss.Structures.FirstOrDefault(x => x.Id == SUBMANDIBULAR_GLAND_L_ID);
                    if (submgl_L == null)
                    {
                        NOT_FOUND_list.Add(SUBMANDIBULAR_GLAND_L_ID);
                        try { submgl_L = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_L_ID); } catch { }
                    }

                    //find SubmGl_R
                    Structure submgl_R = ss.Structures.FirstOrDefault(x => x.Id == SUBMANDIBULAR_GLAND_R_ID);
                    if (submgl_R == null)
                    {
                        NOT_FOUND_list.Add(SUBMANDIBULAR_GLAND_R_ID);
                        try { submgl_R = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_R_ID); } catch { }
                    }
                    //find Nape
                    Structure nape = ss.Structures.FirstOrDefault(x => x.Id == NAPE_ID);
                    if (nape == null)
                    {
                        NOT_FOUND_list.Add(NAPE_ID);
                        try { nape = ss.AddStructure("Avoidance", NAPE_ID); } catch { }
                    }
                    //find Block
                    Structure block = ss.Structures.FirstOrDefault(x => x.Id == BLOCK_ID);
                    if (block == null)
                    {
                        NOT_FOUND_list.Add(BLOCK_ID);
                        try { block = ss.AddStructure("Avoidance", BLOCK_ID); } catch { }
                    }
                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV 54/60/66 structures in the structure set. If some PTVs will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv_54 = ss.Structures.FirstOrDefault(x => x.Id == PTV_54_ID);
                    if (ptv_54 == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_54_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                    Structure ptv_60 = ss.Structures.FirstOrDefault(x => x.Id == PTV_60_ID);
                    if (ptv_60 == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_60_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    }

                    Structure ptv_66 = ss.Structures.FirstOrDefault(x => x.Id == PTV_66_ID);
                    if (ptv_66 == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_66_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    }

                    if (ptv_54 == null && ptv_60 == null && ptv_66 == null)
                    {
                        MessageBox.Show(string.Format("No one of PTVs has been found in the Structure Set. Please, ensure that you have at least PTV_54 or PTV 60 or PTV_66 in the StructureSet"));
                        Window.GetWindow(this).Close(); return;
                    }





                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Check if PTVs exist. If no, then create empty structures

                    if (ptv_54 == null)
                    {
                        try { ptv_54 = ss.AddStructure("Avoidance", PTV_54_ID); } catch { }
                    }
                    if (ptv_60 == null)
                    {
                        try { ptv_60 = ss.AddStructure("Avoidance", PTV_60_ID); } catch { }
                    }
                    if (ptv_66 == null)
                    {
                        try { ptv_66 = ss.AddStructure("Avoidance", PTV_66_ID); } catch { }
                    }


                    //Check if the optimization structures already exist. If so, delete it from the structure set

                    Structure pharconst_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_PHAR_CONST_ID);
                    if (pharconst_PTV != null)
                    {
                        try { ss.RemoveStructure(pharconst_PTV); } catch { }
                    }

                    Structure brainstem_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_BRAINSTEM_ID);
                    if (brainstem_PTV != null)
                    {
                        try { ss.RemoveStructure(brainstem_PTV); } catch { }
                    }

                    Structure brainstem_PRV = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV);
                    if (brainstem_PRV != null)
                    {
                        try { ss.RemoveStructure(brainstem_PRV); } catch { }
                    }

                    Structure chiasm_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_CHIASM_ID);
                    if (chiasm_PTV != null)
                    {
                        try { ss.RemoveStructure(chiasm_PTV); } catch { }
                    }

                    Structure chiasm_PRV = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_PRV);
                    if (chiasm_PRV != null)
                    {
                        try { ss.RemoveStructure(chiasm_PRV); } catch { }
                    }

                    Structure esophagus_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_ESOPHAGUS_ID);
                    if (esophagus_PTV != null)
                    {
                        try { ss.RemoveStructure(esophagus_PTV); } catch { }
                    }

                    Structure larynx_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LARYNX_ID);
                    if (larynx_PTV != null)
                    {
                        try { ss.RemoveStructure(larynx_PTV); } catch { }
                    }

                    Structure mandible_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_MANDIBLE_ID);
                    if (mandible_PTV != null)
                    {
                        try { ss.RemoveStructure(mandible_PTV); } catch { }
                    }

                    Structure lens_L_PRV = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_PRV_ID);
                    if (lens_L_PRV != null)
                    {
                        try { ss.RemoveStructure(lens_L_PRV); } catch { }
                    }

                    Structure lens_R_PRV = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_PRV_ID);
                    if (lens_R_PRV != null)
                    {
                        try { ss.RemoveStructure(lens_R_PRV); } catch { }
                    }

                    Structure lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID);
                    if (lungs != null)
                    {
                        try { ss.RemoveStructure(lungs); } catch { }
                    }

                    Structure opt_nrv_L_PRV = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_PRV_ID);
                    if (opt_nrv_L_PRV != null)
                    {
                        try { ss.RemoveStructure(opt_nrv_L_PRV); } catch { }
                    }

                    Structure opt_nrv_R_PRV = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_PRV_ID);
                    if (opt_nrv_R_PRV != null)
                    {
                        try { ss.RemoveStructure(opt_nrv_R_PRV); } catch { }
                    }

                    Structure oralcavity_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_ORAL_CAVITY_ID);
                    if (oralcavity_PTV != null)
                    {
                        try { ss.RemoveStructure(oralcavity_PTV); } catch { }
                    }

                    Structure parotid_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_PAROTID_L_ID);
                    if (parotid_L_PTV != null)
                    {
                        try { ss.RemoveStructure(parotid_L_PTV); } catch { }
                    }

                    Structure parotid_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_PAROTID_R_ID);
                    if (parotid_R_PTV != null)
                    {
                        try { ss.RemoveStructure(parotid_R_PTV); } catch { }
                    }

                    Structure parotids = ss.Structures.FirstOrDefault(x => x.Id == PAROTIDS_ID);
                    if (parotids != null)
                    {
                        try { ss.RemoveStructure(parotids); } catch { }
                    }

                    Structure spnlcrd_PRV = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spnlcrd_PRV != null)
                    {
                        try { ss.RemoveStructure(spnlcrd_PRV); } catch { }
                    }

                    Structure thyroid_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_THYROID_ID);
                    if (thyroid_PTV != null)
                    {
                        try { ss.RemoveStructure(thyroid_PTV); } catch { }
                    }

                    Structure trachea_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_TRACHEA_ID);
                    if (trachea_PTV != null)
                    {
                        try { ss.RemoveStructure(trachea_PTV); } catch { }
                    }

                    Structure smblr_gl_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_SUBMANDIBULAR_GLAND_L_ID);
                    if (smblr_gl_L_PTV != null)
                    {
                        try { ss.RemoveStructure(smblr_gl_L_PTV); } catch { }
                    }

                    Structure smblr_gl_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_SUBMANDIBULAR_GLAND_R_ID);
                    if (smblr_gl_R_PTV != null)
                    {
                        try { ss.RemoveStructure(smblr_gl_R_PTV); } catch { }
                    }

                    Structure ptv_54_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_54_ID);
                    if (ptv_54_opt != null)
                    {
                        try { ss.RemoveStructure(ptv_54_opt); } catch { }
                    }

                    Structure ptv_60_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_60_ID);
                    if (ptv_60_opt != null)
                    {
                        try { ss.RemoveStructure(ptv_60_opt); } catch { }
                    }

                    Structure ptv_66_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_66_ID);
                    if (ptv_66_opt != null)
                    {
                        try { ss.RemoveStructure(ptv_66_opt); } catch { }
                    }

                    Structure ptv_all = ss.Structures.FirstOrDefault(x => x.Id == PTV_ALL_ID);
                    if (ptv_all != null)
                    {
                        try { ss.RemoveStructure(ptv_all); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script


                    try { ptv_all = ss.AddStructure("PTV", PTV_ALL_ID); } catch { }
                    try { ptv_54_opt = ss.AddStructure("PTV", OPT_PTV_54_ID); } catch { }
                    try { ptv_60_opt = ss.AddStructure("PTV", OPT_PTV_60_ID); } catch { }
                    try { ptv_66_opt = ss.AddStructure("PTV", OPT_PTV_66_ID); } catch { }
                    try { pharconst_PTV = ss.AddStructure("Avoidance", OPT_PHAR_CONST_ID); } catch { }
                    try { brainstem_PTV = ss.AddStructure("Avoidance", OPT_BRAINSTEM_ID); } catch { }
                    try { brainstem_PRV = ss.AddStructure("Avoidance", BRAINSTEM_PRV); } catch { }
                    try { chiasm_PTV = ss.AddStructure("Avoidance", OPT_CHIASM_ID); } catch { }
                    try { chiasm_PRV = ss.AddStructure("Avoidance", CHIASM_PRV); } catch { }
                    try { esophagus_PTV = ss.AddStructure("Avoidance", OPT_ESOPHAGUS_ID); } catch { }
                    try { larynx_PTV = ss.AddStructure("Avoidance", OPT_LARYNX_ID); } catch { }
                    try
                    { lens_L_PRV = ss.AddStructure("Avoidance", LENS_L_PRV_ID); }
                    catch { }
                    try
                    { lens_R_PRV = ss.AddStructure("Avoidance", LENS_R_PRV_ID); }
                    catch { }
                    try
                    { lungs = ss.AddStructure("Avoidance", LUNGS_ID); }
                    catch { }
                    try
                    { mandible_PTV = ss.AddStructure("Avoidance", OPT_MANDIBLE_ID); }
                    catch { }
                    try
                    { opt_nrv_L_PRV = ss.AddStructure("Avoidance", OPTIC_NERVE_L_PRV_ID); }
                    catch { }
                    try
                    { opt_nrv_R_PRV = ss.AddStructure("Avoidance", OPTIC_NERVE_R_PRV_ID); }
                    catch { }
                    try
                    { oralcavity_PTV = ss.AddStructure("Avoidance", OPT_ORAL_CAVITY_ID); }
                    catch { }
                    try
                    { parotid_L_PTV = ss.AddStructure("Avoidance", OPT_PAROTID_L_ID); }
                    catch { }
                    try
                    { parotid_R_PTV = ss.AddStructure("Avoidance", OPT_PAROTID_R_ID); }
                    catch { }
                    try
                    { parotids = ss.AddStructure("Avoidance", PAROTIDS_ID); }
                    catch { }
                    try
                    { spnlcrd_PRV = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); }
                    catch { }
                    try
                    { thyroid_PTV = ss.AddStructure("Avoidance", OPT_THYROID_ID); }
                    catch { }
                    try
                    { trachea_PTV = ss.AddStructure("Avoidance", OPT_TRACHEA_ID); }
                    catch { }
                    try
                    { smblr_gl_L_PTV = ss.AddStructure("Avoidance", OPT_SUBMANDIBULAR_GLAND_L_ID); }
                    catch { }
                    try
                    { smblr_gl_R_PTV = ss.AddStructure("Avoidance", OPT_SUBMANDIBULAR_GLAND_R_ID); }
                    catch { }

                    //Create trash structures
                    Structure body_ptv_54 = ss.Structures.FirstOrDefault(x => x.Id == BODY_PTV_54_ID);
                    try { body_ptv_54 = ss.AddStructure("Avoidance", BODY_PTV_54_ID); } catch { }
                    Structure body_ptv_54_60 = ss.Structures.FirstOrDefault(x => x.Id == BODY_PTV_54_60_ID);
                    try { body_ptv_54_60 = ss.AddStructure("Avoidance", BODY_PTV_54_60_ID); } catch { }
                    Structure body_ptv_54_60_66 = ss.Structures.FirstOrDefault(x => x.Id == BODY_PTV_54_60_66_ID);
                    try { body_ptv_54_60_66 = ss.AddStructure("Avoidance", BODY_PTV_54_60_66_ID); } catch { }
                    Structure body_lung_L = ss.Structures.FirstOrDefault(x => x.Id == BODY_LUNG_L_ID);
                    try { body_lung_L = ss.AddStructure("Avoidance", BODY_LUNG_L_ID); } catch { }
                    Structure body_lung_L_R = ss.Structures.FirstOrDefault(x => x.Id == BODY_LUNG_L_R_ID);
                    try { body_lung_L_R = ss.AddStructure("Avoidance", BODY_LUNG_L_R_ID); } catch { }
                    Structure body_parotid_L = ss.Structures.FirstOrDefault(x => x.Id == BODY_PAROTID_L_ID);
                    try { body_parotid_L = ss.AddStructure("Avoidance", BODY_PAROTID_L_ID); } catch { }
                    Structure body_parotid_L_R = ss.Structures.FirstOrDefault(x => x.Id == BODY_PAROTID_L_R_ID);
                    try { body_parotid_L_R = ss.AddStructure("Avoidance", BODY_PAROTID_L_R_ID); } catch { }
                    Structure sub_ptv_54_60 = ss.Structures.FirstOrDefault(x => x.Id == SUB_PTV_54_60);
                    try { sub_ptv_54_60 = ss.AddStructure("Avoidance", SUB_PTV_54_60); } catch { }
                    Structure ptv_all_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_3MM_PTV_ALL_ID);
                    try { ptv_all_3mm = ss.AddStructure("PTV", EXPANDED_3MM_PTV_ALL_ID); } catch { }
                    Structure ptv_54_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_PTV_54_ID);
                    try { ptv_54_3mm = ss.AddStructure("PTV", EXPANDED_PTV_54_ID); } catch { }
                    Structure ptv_60_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_PTV_60_ID); ;
                    try { ptv_60_3mm = ss.AddStructure("PTV", EXPANDED_PTV_60_ID); } catch { }
                    Structure ptv_66_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_PTV_66_ID); ;
                    try { ptv_66_3mm = ss.AddStructure("PTV", EXPANDED_PTV_66_ID); } catch { }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PTV_ALL and generate expanded ptv_all with 3mm margin
                    Structure BODY_HR = ss.AddStructure("Avoidance", "BODY_HR"); //This structure is mandatory if one of the PTVs if in high resolution

                    if (ptv_54 != null && ptv_60 != null && ptv_66 != null)
                    {

                        if (ptv_54.IsHighResolution || ptv_60.IsHighResolution || ptv_66.IsHighResolution)
                        {

                            BODY_HR.SegmentVolume = BODY.SegmentVolume;
                            BODY_HR.ConvertToHighResolution();
                            body_ptv_54.ConvertToHighResolution();
                            body_ptv_54_60.ConvertToHighResolution();
                            body_ptv_54_60_66.ConvertToHighResolution();
                            ptv_all.ConvertToHighResolution();
                            ptv_all_3mm.ConvertToHighResolution();
                            body_ptv_54.SegmentVolume = BODY_HR.Sub(ptv_54);
                            body_ptv_54_60.SegmentVolume = body_ptv_54.Sub(ptv_60);
                            body_ptv_54_60_66.SegmentVolume = body_ptv_54_60.Sub(ptv_66);
                            ptv_all.SegmentVolume = BODY_HR.Sub(body_ptv_54_60_66);
                            ptv_all_3mm.SegmentVolume = ptv_all.Margin(3.0);
                        }
                        else
                        {
                            body_ptv_54.SegmentVolume = BODY.Sub(ptv_54);
                            body_ptv_54_60.SegmentVolume = body_ptv_54.Sub(ptv_60);
                            body_ptv_54_60_66.SegmentVolume = body_ptv_54_60.Sub(ptv_66);
                            ptv_all.SegmentVolume = BODY.Sub(body_ptv_54_60_66);
                            ptv_all_3mm.SegmentVolume = ptv_all.Margin(3.0);
                        }

                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PTV 54/60/66 OPT structures

                    ptv_66_opt.SegmentVolume = ptv_66.Margin(0.0);
                    ptv_66_3mm.SegmentVolume = ptv_66.Margin(3.0);

                    ptv_60_opt.SegmentVolume = ptv_60.Sub(ptv_66_3mm);
                    ptv_60_3mm.SegmentVolume = ptv_60.Margin(3.0);

                    sub_ptv_54_60.SegmentVolume = ptv_54.Sub(ptv_60_3mm);
                    ptv_54_opt.SegmentVolume = sub_ptv_54_60.Sub(ptv_66_3mm);
                    ptv_54_3mm.SegmentVolume = ptv_54.Margin(3.0);


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define Lungs structure
                    if (BODY != null && lungs != null && lung_R != null)
                    {

                        body_lung_L.SegmentVolume = BODY.Sub(lung_L);
                        body_lung_L_R.SegmentVolume = body_lung_L.Sub(lung_R);
                        lungs.SegmentVolume = BODY.Sub(body_lung_L_R);

                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define Parotids structure
                    if (BODY != null && parotid_L != null && parotid_R != null)
                    {
                        if (ptv_54.IsHighResolution || ptv_60.IsHighResolution || ptv_66.IsHighResolution)
                        {
                            parotid_L.ConvertToHighResolution();
                            parotid_R.ConvertToHighResolution();
                            body_parotid_L.ConvertToHighResolution();
                            body_parotid_L_R.ConvertToHighResolution();
                            parotids.ConvertToHighResolution();
                            body_parotid_L.SegmentVolume = BODY_HR.Sub(parotid_L);
                            body_parotid_L_R.SegmentVolume = body_parotid_L.Sub(parotid_R);
                            parotids.SegmentVolume = BODY_HR.Sub(body_parotid_L_R);
                        }
                        else
                        {
                            body_parotid_L.SegmentVolume = BODY.Sub(parotid_L);
                            body_parotid_L_R.SegmentVolume = body_parotid_L.Sub(parotid_R);
                            parotids.SegmentVolume = BODY.Sub(body_parotid_L_R);
                        }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 5mm margin. If a structure is empty, the empty PRV will be created

                    if (brainstem != null)
                    {
                        brainstem_PRV.SegmentVolume = brainstem.Margin(5.0);
                    }
                    if (brainstem == null)
                    {
                        try { brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { }
                    }

                    if (chiasm != null)
                    {
                        chiasm_PRV.SegmentVolume = chiasm.Margin(5.0);
                    }
                    if (chiasm == null)
                    {
                        try { chiasm = ss.AddStructure("Avoidance", CHIASM_ID); } catch { }
                    }

                    if (lens_L != null)
                    {
                        try { lens_L_PRV.SegmentVolume = lens_L.Margin(5.0); } catch { }
                    }
                    if (lens_L == null)
                    {
                        try { lens_L = ss.AddStructure("Avoidance", LENS_L_ID); } catch { }
                    }

                    if (lens_R != null)
                    {
                        lens_R_PRV.SegmentVolume = lens_R.Margin(5.0);
                    }
                    if (lens_R == null)
                    {
                        try { lens_R = ss.AddStructure("Avoidance", LENS_R_ID); } catch { }
                    }

                    if (opticnerve_L != null)
                    {
                        opt_nrv_L_PRV.SegmentVolume = opticnerve_L.Margin(5.0);
                    }
                    if (opticnerve_L == null)
                    {
                        try { opticnerve_L = ss.AddStructure("Avoidance", OPTIC_NERVE_L_ID); } catch { }
                    }

                    if (opticnerve_R != null)
                    {
                        opt_nrv_R_PRV.SegmentVolume = opticnerve_R.Margin(5.0);
                    }
                    if (opticnerve_R == null)
                    {
                        try { opticnerve_R = ss.AddStructure("Avoidance", OPTIC_NERVE_R_ID); } catch { }
                    }

                    if (spinalcord != null)
                    {
                        spnlcrd_PRV.SegmentVolume = spinalcord.Margin(5.0);
                    }
                    if (spinalcord == null)
                    {
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //
                    if (ptv_54.IsHighResolution || ptv_60.IsHighResolution || ptv_66.IsHighResolution)
                    {
                        foreach (Structure s in ss.Structures)
                        {
                            if (s.IsHighResolution == false && s.Id != "BODY" && s.Id != "CouchSurface" && s.Id != "CouchInterior")
                            {
                                try
                                {
                                    s.ConvertToHighResolution();
                                }
                                catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", s.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                            }
                        }

                    }


                    //Substract critical structures from expanded PTV_ALL to create 3mm buffer

                    if (pharconst != null)
                    {

                        pharconst_PTV.SegmentVolume = pharconst.Sub(ptv_all_3mm);
                    }
                    if (pharconst == null)
                    {
                        try { pharconst_PTV = ss.AddStructure("Avoidance", OPT_PHAR_CONST_ID); } catch { }
                    }

                    if (brainstem != null)
                    {
                        brainstem_PTV.SegmentVolume = brainstem.Sub(ptv_all_3mm);
                    }
                    if (brainstem == null)
                    {
                        try { brainstem_PTV = ss.AddStructure("AVOIDANCE", OPT_BRAINSTEM_ID); } catch { }
                    }

                    if (chiasm != null)
                    {
                        chiasm_PTV.SegmentVolume = chiasm.Sub(ptv_all_3mm);
                    }
                    if (chiasm == null)
                    {
                        try { chiasm_PTV = ss.AddStructure("AVOIDANCE", OPT_CHIASM_ID); } catch { }
                    }

                    if (esophagus != null)
                    {
                        esophagus_PTV.SegmentVolume = esophagus.Sub(ptv_all_3mm);
                    }
                    if (esophagus == null)
                    {
                        try { esophagus_PTV = ss.AddStructure("AVOIDANCE", OPT_ESOPHAGUS_ID); } catch { }
                    }

                    if (larynx != null)
                    {
                        larynx_PTV.SegmentVolume = larynx.Sub(ptv_all_3mm);
                    }
                    if (larynx == null)
                    {
                        try { larynx_PTV = ss.AddStructure("AVOIDANCE", OPT_LARYNX_ID); } catch { }
                    }

                    if (mandible != null)
                    {
                        mandible_PTV.SegmentVolume = mandible.Sub(ptv_all_3mm);
                    }
                    if (mandible == null)
                    {
                        try { mandible_PTV = ss.AddStructure("AVOIDANCE", OPT_MANDIBLE_ID); } catch { }
                    }

                    if (oralcavity != null)
                    {
                        oralcavity_PTV.SegmentVolume = oralcavity.Sub(ptv_all_3mm);
                    }
                    if (oralcavity == null)
                    {
                        try { oralcavity_PTV = ss.AddStructure("AVOIDANCE", OPT_ORAL_CAVITY_ID); } catch { }
                    }

                    if (parotid_L != null)
                    {
                        parotid_L_PTV.SegmentVolume = parotid_L.Sub(ptv_all_3mm);
                    }
                    if (parotid_L == null)
                    {
                        try { parotid_L_PTV = ss.AddStructure("AVOIDANCE", OPT_PAROTID_L_ID); } catch { }
                    }

                    if (parotid_R != null)
                    {
                        parotid_R_PTV.SegmentVolume = parotid_R.Sub(ptv_all_3mm);
                    }
                    if (parotid_R == null)
                    {
                        try { parotid_L_PTV = ss.AddStructure("AVOIDANCE", OPT_PAROTID_R_ID); } catch { }
                    }

                    if (thyroid != null)
                    {
                        thyroid_PTV.SegmentVolume = thyroid.Sub(ptv_all_3mm);
                    }
                    if (thyroid == null)
                    {
                        try { thyroid_PTV = ss.AddStructure("AVOIDANCE", OPT_THYROID_ID); } catch { }
                    }

                    if (trachea != null)
                    {
                        trachea_PTV.SegmentVolume = trachea.Sub(ptv_all_3mm);
                    }
                    if (trachea == null)
                    {
                        try { trachea_PTV = ss.AddStructure("AVOIDANCE", OPT_TRACHEA_ID); } catch { }
                    }

                    if (submgl_L != null)
                    {
                        try { smblr_gl_L_PTV.SegmentVolume = submgl_L.Sub(ptv_all_3mm); } catch { }
                    }

                    if (submgl_R != null)
                    {
                        try { smblr_gl_R_PTV.SegmentVolume = submgl_R.Sub(ptv_all_3mm); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Check if the summ structures was created. If no, than create empty structures for report generation

                    if (lung_R == null)
                    {
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }
                    if (lung_L == null)
                    {
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { }
                    }
                    if (lungs == null)
                    {
                        try { lungs = ss.AddStructure("Avoidance", LUNGS_ID); } catch { }
                    }
                    if (submgl_L == null)
                    {
                        try { submgl_L = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_L_ID); } catch { }
                    }
                    if (submgl_R == null)
                    {
                        try { submgl_R = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_R_ID); } catch { }
                    }
                    if (submgl_L == null)
                    {
                        try { submgl_L = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_L_ID); } catch { }
                    }
                    if (parotid_L == null)
                    {
                        try { parotid_L = ss.AddStructure("Avoidance", PAROTID_L_ID); } catch { }
                    }
                    if (parotid_R == null)
                    {
                        try { parotid_R = ss.AddStructure("Avoidance", PAROTID_R_ID); } catch { }
                    }
                    if (parotids == null)
                    {
                        try { parotids = ss.AddStructure("Avoidance", PAROTIDS_ID); } catch { }
                    }
                    if (lips == null)
                    {
                        try { lips = ss.AddStructure("Avoidance", LIPS_ID); } catch { }
                    }

                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}",

                            ptv_54.Id, ptv_54.Volume,

                            ptv_54_opt.Id, ptv_54_opt.Volume,

                            ptv_54_3mm.Id, ptv_54_3mm.Volume,

                            ptv_60.Id, ptv_60.Volume,

                            ptv_60_opt.Id, ptv_60_opt.Volume,

                            ptv_60_3mm.Id, ptv_60_3mm.Volume,

                            ptv_66.Id, ptv_66.Volume,

                            ptv_66_opt.Id, ptv_66_opt.Volume,

                            ptv_66_3mm.Id, ptv_66_3mm.Volume,

                            ptv_all.Id, ptv_all.Volume,

                            ptv_all_3mm.Id, ptv_all_3mm.Volume
                            );

                        string message_OAR = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}",

                            brainstem.Id, brainstem.Volume,

                            brainstem_PTV.Id, brainstem_PTV.Volume,

                            brainstem_PRV.Id, brainstem_PRV.Volume,

                            chiasm.Id, chiasm.Volume,

                            chiasm_PTV.Id, chiasm_PTV.Volume,

                            chiasm_PRV.Id, chiasm_PRV.Volume,

                            esophagus.Id, esophagus.Volume,

                            esophagus_PTV.Id, esophagus_PTV.Volume,

                            eye_L.Id, eye_L.Volume,

                            eye_R.Id, eye_R.Volume,

                            larynx.Id, larynx.Volume,

                            larynx_PTV.Id, larynx_PTV.Volume,

                            lens_L.Id, lens_L.Volume,

                            lens_L_PRV.Id, lens_L_PRV.Volume,

                            lung_R.Id, lung_R.Volume,

                            lung_L.Id, lung_L.Volume
                            );




                        string message_OAR_2 = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}",

                                oralcavity.Id, oralcavity.Volume,

                                parotids.Id, parotids.Volume,

                                parotid_L.Id, parotid_L.Volume,

                                parotid_L_PTV.Id, parotid_L_PTV.Volume,

                                parotid_R.Id, parotid_R.Volume,

                                parotid_R_PTV.Id, parotid_R_PTV.Volume,

                                spinalcord.Id, spinalcord.Volume,

                                spnlcrd_PRV.Id, spnlcrd_PRV.Volume,

                                thyroid.Id, thyroid.Volume,

                                thyroid_PTV.Id, thyroid_PTV.Volume,

                                trachea.Id, trachea.Volume,

                                lungs.Id, lungs.Volume,

                                mandible.Id, mandible.Volume,

                                mandible_PTV.Id, mandible_PTV.Volume,

                                opticnerve_L.Id, opticnerve_L.Volume,

                                opt_nrv_L_PRV.Id, opt_nrv_L_PRV.Volume
                                );

                        string message_OAR_3 = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}",

                                trachea_PTV.Id, trachea_PTV.Volume,

                                lips.Id, lips.Volume,

                                submgl_L.Id, submgl_L.Volume,

                                smblr_gl_L_PTV.Id, smblr_gl_L_PTV.Volume,

                                submgl_R.Id, submgl_R.Volume,

                                smblr_gl_R_PTV.Id, smblr_gl_R_PTV.Volume,

                                opticnerve_R.Id, opticnerve_R.Volume,

                                opt_nrv_R_PRV.Id, opt_nrv_R_PRV.Volume,

                                pharconst.Id, pharconst.Volume,

                                pharconst_PTV.Id, pharconst_PTV.Volume

                                );

                        //Show the messages

                        MessageBox.Show(message_PTV);
                        MessageBox.Show(message_OAR);
                        MessageBox.Show(message_OAR_2);
                        MessageBox.Show(message_OAR_3);
                    }
                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown...")); }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations

                    try { ss.RemoveStructure(ptv_66_3mm); } catch { }
                    try { ss.RemoveStructure(ptv_60_3mm); } catch { }
                    try { ss.RemoveStructure(ptv_54_3mm); } catch { }
                    try { ss.RemoveStructure(ptv_all_3mm); } catch { }
                    try { ss.RemoveStructure(sub_ptv_54_60); } catch { }
                    try { ss.RemoveStructure(body_parotid_L_R); } catch { }
                    try { ss.RemoveStructure(body_parotid_L); } catch { }
                    try { ss.RemoveStructure(body_lung_L_R); } catch { }
                    try { ss.RemoveStructure(body_lung_L); } catch { }
                    try { ss.RemoveStructure(body_ptv_54_60_66); } catch { }
                    try { ss.RemoveStructure(body_ptv_54_60); } catch { }
                    try { ss.RemoveStructure(body_ptv_54); } catch { }
                    try { ss.RemoveStructure(BODY_HR); } catch { }
                }
                if (HeadAndNeck1trgtRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_HN_1target";

                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66

                    const string PTV_ID = "PTV";
                    const string PTV_OPT_ID = "PTV_OPT";
                    



                    //Due to the inability to add structures one to another (boolean "And" seems to be having malfunction), it is mandatory to create some "trash" structures for the purpose of creating PTV summ, Lungs and Parotids. These structures will be removed at the end of this script
                    const string BODY_PTV_54_ID = "BODY-PTV_54";
                    const string BODY_PTV_54_60_ID = "BODY_54_60";
                    const string BODY_PTV_54_60_66_ID = "BODY_54_60_66";
                    const string EXPANDED_PTV_54_ID = "PTV_54_3mm";
                    const string EXPANDED_PTV_60_ID = "PTV_60_3mm";
                    const string EXPANDED_PTV_66_ID = "PTV_66_3mm";
                    const string BODY_LUNG_L_ID = "BODY_LUNG_L";
                    const string BODY_LUNG_L_R_ID = "BODY_LUNG_L_R";
                    const string BODY_PAROTID_L_ID = "BODY_PAROTID_L";
                    const string BODY_PAROTID_L_R_ID = "BODY_PAROTID_L_R";
                    const string SUB_PTV_54_60 = "sub_54_60";

                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_3MM_PTV_ALL_ID = "PTV_ALL_3mm";

                    //IDs of critical structures

                    const string BRAINSTEM_ID = "Brainstem";
                    const string OPT_BRAINSTEM_ID = "Brainstem-PTV";
                    const string BRAINSTEM_PRV = "Brainstem_PRV";
                    const string CHIASM_ID = "Chiasm";
                    const string OPT_CHIASM_ID = "Chiasm-PTV";
                    const string CHIASM_PRV = "Chiasm_PRV";
                    const string ESOPHAGUS_ID = "Esophagus";
                    const string OPT_ESOPHAGUS_ID = "Esophagus-PTV";
                    const string EYE_L_ID = "Eye_L";
                    const string EYE_R_ID = "Eye_R";
                    const string LARYNX_ID = "Larynx";
                    const string OPT_LARYNX_ID = "Larynx-PTV";
                    const string LENS_L_ID = "Lens_L";
                    const string LENS_L_PRV_ID = "Lens_L_PRV";
                    const string LENS_R_ID = "Lens_R";
                    const string LENS_R_PRV_ID = "Lens_R_PRV";
                    const string LUNG_L_ID = "Lung_L";
                    const string LUNG_R_ID = "Lung_R";
                    const string LUNGS_ID = "Lungs";
                    const string MANDIBLE_ID = "Mandible";
                    const string OPT_MANDIBLE_ID = "Mandible-PTV";
                    const string OPTIC_NERVE_L_ID = "OpticNerve_L";
                    const string OPTIC_NERVE_L_PRV_ID = "OptNrv_L_PRV";
                    const string OPTIC_NERVE_R_ID = "OpticNerve_R";
                    const string OPTIC_NERVE_R_PRV_ID = "OptNrv_R_PRV";
                    const string ORAL_CAVITY_ID = "OralCavity";
                    const string OPT_ORAL_CAVITY_ID = "OralCvt-PTV";
                    const string PAROTID_L_ID = "Parotid_L";
                    const string OPT_PAROTID_L_ID = "Prtd_L-PTV";
                    const string PAROTID_R_ID = "Parotid_R";
                    const string OPT_PAROTID_R_ID = "Prts_R-PTV";
                    const string PAROTIDS_ID = "Parotids";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpnlCrd_PRV";
                    const string THYROID_ID = "Thyroid";
                    const string OPT_THYROID_ID = "Thyroid-PTV";
                    const string TRACHEA_ID = "Trachea";
                    const string OPT_TRACHEA_ID = "Trachea-PTV";
                    const string LIPS_ID = "Lips";
                    const string SUBMANDIBULAR_GLAND_L_ID = "SubmGl_L";
                    const string OPT_SUBMANDIBULAR_GLAND_L_ID = "SubmGl_L-PTV";
                    const string SUBMANDIBULAR_GLAND_R_ID = "SubmGl_R";
                    const string OPT_SUBMANDIBULAR_GLAND_R_ID = "SubmGl_R-PTV";
                    const string PHAR_CONST_ID = "PharynxConst";
                    const string OPT_PHAR_CONST_ID = "PhConst-PTV";
                    const string NAPE_ID = "Nape";
                    const string BLOCK_ID = "Block";
                    const string BODY_ID = "BODY";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structures: PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);

                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution

                    List<string> NOT_FOUND_list = new List<string>();
                    //find PharynxConst
                    Structure pharconst = ss.Structures.FirstOrDefault(x => x.Id == PHAR_CONST_ID);
                    if (pharconst == null)
                    {
                        NOT_FOUND_list.Add(PHAR_CONST_ID);
                        try { pharconst = ss.AddStructure("Avoidance", PHAR_CONST_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", PHAR_CONST_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find BrainStem
                    Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                    if (brainstem == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_ID);
                        try { brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", BRAINSTEM_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Chiasm
                    Structure chiasm = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_ID);
                    if (chiasm == null)
                    {
                        NOT_FOUND_list.Add(CHIASM_ID);
                        try { chiasm = ss.AddStructure("Avoidance", CHIASM_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", CHIASM_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }


                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", ESOPHAGUS_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Eye_L
                    Structure eye_L = ss.Structures.FirstOrDefault(x => x.Id == EYE_L_ID);
                    if (eye_L == null)
                    {
                        NOT_FOUND_list.Add(EYE_L_ID);
                        try { eye_L = ss.AddStructure("Avoidance", EYE_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", EYE_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Eye_R
                    Structure eye_R = ss.Structures.FirstOrDefault(x => x.Id == EYE_R_ID);
                    if (eye_R == null)
                    {
                        NOT_FOUND_list.Add(EYE_R_ID);
                        try { eye_R = ss.AddStructure("Avoidance", EYE_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", EYE_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Larynx
                    Structure larynx = ss.Structures.FirstOrDefault(x => x.Id == LARYNX_ID);
                    if (larynx == null)
                    {
                        NOT_FOUND_list.Add(LARYNX_ID);
                        try { larynx = ss.AddStructure("Avoidance", LARYNX_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", LARYNX_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Lens_L
                    Structure lens_L = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_ID);
                    if (lens_L == null)
                    {
                        NOT_FOUND_list.Add(LENS_L_ID);
                        try { lens_L = ss.AddStructure("Avoidance", LENS_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", LENS_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Lens_R
                    Structure lens_R = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_ID);
                    if (lens_R == null)
                    {
                        NOT_FOUND_list.Add(LENS_R_ID);
                        try { lens_R = ss.AddStructure("Avoidance", LENS_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", LENS_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Lung_L
                    Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_ID);
                    if (lung_L == null)
                    {
                        NOT_FOUND_list.Add(LUNG_L_ID);
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", LUNG_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Lung_R
                    Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                    if (lung_R == null)
                    {
                        NOT_FOUND_list.Add(LUNG_R_ID);
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", LUNG_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Mandible
                    Structure mandible = ss.Structures.FirstOrDefault(x => x.Id == MANDIBLE_ID);
                    if (mandible == null)
                    {
                        NOT_FOUND_list.Add(MANDIBLE_ID);
                        try { mandible = ss.AddStructure("Avoidance", MANDIBLE_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", MANDIBLE_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find OpticNerve_L
                    Structure opticnerve_L = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_ID);
                    if (opticnerve_L == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_NERVE_L_ID);
                        try { opticnerve_L = ss.AddStructure("Avoidance", OPTIC_NERVE_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", OPTIC_NERVE_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find OpticNerve_R
                    Structure opticnerve_R = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_ID);
                    if (opticnerve_R == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_NERVE_R_ID);
                        try { opticnerve_R = ss.AddStructure("Avoidance", OPTIC_NERVE_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", OPTIC_NERVE_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find OralCavity
                    Structure oralcavity = ss.Structures.FirstOrDefault(x => x.Id == ORAL_CAVITY_ID);
                    if (oralcavity == null)
                    {
                        NOT_FOUND_list.Add(ORAL_CAVITY_ID);
                        try { oralcavity = ss.AddStructure("Avoidance", ORAL_CAVITY_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", ORAL_CAVITY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Parotid_L
                    Structure parotid_L = ss.Structures.FirstOrDefault(x => x.Id == PAROTID_L_ID);
                    if (parotid_L == null)
                    {
                        NOT_FOUND_list.Add(PAROTID_L_ID);
                        try { parotid_L = ss.AddStructure("Avoidance", PAROTID_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", PAROTID_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Parotid_R
                    Structure parotid_R = ss.Structures.FirstOrDefault(x => x.Id == PAROTID_R_ID);
                    if (parotid_R == null)
                    {
                        NOT_FOUND_list.Add(PAROTID_R_ID);
                        try { parotid_R = ss.AddStructure("Avoidance", PAROTID_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", PAROTID_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", SPINAL_CORD_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Thyroid
                    Structure thyroid = ss.Structures.FirstOrDefault(x => x.Id == THYROID_ID);
                    if (thyroid == null)
                    {
                        NOT_FOUND_list.Add(THYROID_ID);
                        try { thyroid = ss.AddStructure("Avoidance", THYROID_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", THYROID_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", TRACHEA_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Lips
                    Structure lips = ss.Structures.FirstOrDefault(x => x.Id == LIPS_ID);
                    if (lips == null)
                    {
                        NOT_FOUND_list.Add(LIPS_ID);
                        try { lips = ss.AddStructure("Avoidance", LIPS_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", LIPS_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find SubmGl_L
                    Structure submgl_L = ss.Structures.FirstOrDefault(x => x.Id == SUBMANDIBULAR_GLAND_L_ID);
                    if (submgl_L == null)
                    {
                        NOT_FOUND_list.Add(SUBMANDIBULAR_GLAND_L_ID);
                        try { submgl_L = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", SUBMANDIBULAR_GLAND_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find SubmGl_R
                    Structure submgl_R = ss.Structures.FirstOrDefault(x => x.Id == SUBMANDIBULAR_GLAND_R_ID);
                    if (submgl_R == null)
                    {
                        NOT_FOUND_list.Add(SUBMANDIBULAR_GLAND_R_ID);
                        try { submgl_R = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", SUBMANDIBULAR_GLAND_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //find Nape
                    Structure nape = ss.Structures.FirstOrDefault(x => x.Id == NAPE_ID);
                    if (nape == null)
                    {
                        NOT_FOUND_list.Add(NAPE_ID);
                        try { nape = ss.AddStructure("Avoidance", NAPE_ID); } catch { }
                    }

                    //find Block
                    Structure block = ss.Structures.FirstOrDefault(x => x.Id == BLOCK_ID);
                    if (block == null)
                    {
                        NOT_FOUND_list.Add(BLOCK_ID);
                        try { block = ss.AddStructure("Avoidance", BLOCK_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV 54/60/66 structures in the structure set. If some PTVs will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == PTV_OPT_ID);
                    if (ptv_opt == null)
                    {
                        try { ptv_opt = ss.AddStructure("Avoidance", PTV_OPT_ID); } catch { MessageBox.Show(string.Format("PTV_OPT could not be created"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Check if PTVs exist. If no, then create empty structures

                    if (ptv_opt == null)
                    {
                        try
                        {
                            ptv_opt = ss.AddStructure("Avoidance", PTV_OPT_ID);
                        }
                        catch { MessageBox.Show(string.Format("{0} could not be created!", PTV_OPT_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }


                    //Check if the optimization structures already exist. If so, delete it from the structure set

                    Structure pharconst_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_PHAR_CONST_ID);
                    if (pharconst_PTV != null)
                    {
                        try { ss.RemoveStructure(pharconst_PTV); } catch { }
                    }

                    Structure brainstem_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_BRAINSTEM_ID);
                    if (brainstem_PTV != null)
                    {
                        try { ss.RemoveStructure(brainstem_PTV); } catch { }
                    }

                    Structure brainstem_PRV = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV);
                    if (brainstem_PRV != null)
                    {
                        try { ss.RemoveStructure(brainstem_PRV); } catch { }
                    }

                    Structure chiasm_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_CHIASM_ID);
                    if (chiasm_PTV != null)
                    {
                        try { ss.RemoveStructure(chiasm_PTV); } catch { }
                    }

                    Structure chiasm_PRV = ss.Structures.FirstOrDefault(x => x.Id == CHIASM_PRV);
                    if (chiasm_PRV != null)
                    {
                        try { ss.RemoveStructure(chiasm_PRV); } catch { }
                    }

                    Structure esophagus_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_ESOPHAGUS_ID);
                    if (esophagus_PTV != null)
                    {
                        try { ss.RemoveStructure(esophagus_PTV); } catch { }
                    }

                    Structure larynx_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LARYNX_ID);
                    if (larynx_PTV != null)
                    {
                        try { ss.RemoveStructure(larynx_PTV); } catch { }
                    }

                    Structure mandible_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_MANDIBLE_ID);
                    if (mandible_PTV != null)
                    {
                        try { ss.RemoveStructure(mandible_PTV); } catch { }
                    }

                    Structure lens_L_PRV = ss.Structures.FirstOrDefault(x => x.Id == LENS_L_PRV_ID);
                    if (lens_L_PRV != null)
                    {
                        try { ss.RemoveStructure(lens_L_PRV); } catch { }
                    }

                    Structure lens_R_PRV = ss.Structures.FirstOrDefault(x => x.Id == LENS_R_PRV_ID);
                    if (lens_R_PRV != null)
                    {
                        try { ss.RemoveStructure(lens_R_PRV); } catch { }
                    }

                    Structure lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID);
                    if (lungs != null)
                    {
                        try { ss.RemoveStructure(lungs); } catch { }
                    }

                    Structure opt_nrv_L_PRV = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_L_PRV_ID);
                    if (opt_nrv_L_PRV != null)
                    {
                        try { ss.RemoveStructure(opt_nrv_L_PRV); } catch { }
                    }

                    Structure opt_nrv_R_PRV = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_NERVE_R_PRV_ID);
                    if (opt_nrv_R_PRV != null)
                    {
                        try { ss.RemoveStructure(opt_nrv_R_PRV); } catch { }
                    }

                    Structure oralcavity_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_ORAL_CAVITY_ID);
                    if (oralcavity_PTV != null)
                    {
                        try { ss.RemoveStructure(oralcavity_PTV); } catch { }
                    }

                    Structure parotid_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_PAROTID_L_ID);
                    if (parotid_L_PTV != null)
                    {
                        try { ss.RemoveStructure(parotid_L_PTV); } catch { }
                    }

                    Structure parotid_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_PAROTID_R_ID);
                    if (parotid_R_PTV != null)
                    {
                        try { ss.RemoveStructure(parotid_R_PTV); } catch { }
                    }

                    Structure parotids = ss.Structures.FirstOrDefault(x => x.Id == PAROTIDS_ID);
                    if (parotids != null)
                    {
                        try { ss.RemoveStructure(parotids); } catch { }
                    }

                    Structure spnlcrd_PRV = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spnlcrd_PRV != null)
                    {
                        try { ss.RemoveStructure(spnlcrd_PRV); } catch { }
                    }

                    Structure thyroid_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_THYROID_ID);
                    if (thyroid_PTV != null)
                    {
                        try { ss.RemoveStructure(thyroid_PTV); } catch { }
                    }

                    Structure trachea_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_TRACHEA_ID);
                    if (trachea_PTV != null)
                    {
                        try { ss.RemoveStructure(trachea_PTV); } catch { }
                    }

                    Structure smblr_gl_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_SUBMANDIBULAR_GLAND_L_ID);
                    if (smblr_gl_L_PTV != null)
                    {
                        try { ss.RemoveStructure(smblr_gl_L_PTV); } catch { }
                    }

                    Structure smblr_gl_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_SUBMANDIBULAR_GLAND_R_ID);
                    if (smblr_gl_R_PTV != null)
                    {
                        try { ss.RemoveStructure(smblr_gl_R_PTV); } catch { }
                    }

                    /*Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == PTV_OPT_ID);
                    if (ptv_opt != null)
                    {
                        try{ss.RemoveStructure(ptv_opt); } catch { }
                    }
                    */

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script


                    //try { ptv_opt = ss.AddStructure("PTV", PTV_OPT_ID); } catch { MessageBox.Show(string.Format("{0} could not be created!", PTV_OPT_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { pharconst_PTV = ss.AddStructure("Avoidance", OPT_PHAR_CONST_ID); } catch { }
                    try { brainstem_PTV = ss.AddStructure("Avoidance", OPT_BRAINSTEM_ID); } catch { }
                    try { brainstem_PRV = ss.AddStructure("Avoidance", BRAINSTEM_PRV); } catch { }
                    try { chiasm_PTV = ss.AddStructure("Avoidance", OPT_CHIASM_ID); } catch { }
                    try { chiasm_PRV = ss.AddStructure("Avoidance", CHIASM_PRV); } catch { }
                    try { esophagus_PTV = ss.AddStructure("Avoidance", OPT_ESOPHAGUS_ID); } catch { }
                    try { larynx_PTV = ss.AddStructure("Avoidance", OPT_LARYNX_ID); } catch { }
                    try { lens_L_PRV = ss.AddStructure("Avoidance", LENS_L_PRV_ID); } catch { }
                    try { lens_R_PRV = ss.AddStructure("Avoidance", LENS_R_PRV_ID); } catch { }
                    try { lungs = ss.AddStructure("Avoidance", LUNGS_ID); } catch { }
                    try { mandible_PTV = ss.AddStructure("Avoidance", OPT_MANDIBLE_ID); } catch { }
                    try { opt_nrv_L_PRV = ss.AddStructure("Avoidance", OPTIC_NERVE_L_PRV_ID); } catch { }
                    try { opt_nrv_R_PRV = ss.AddStructure("Avoidance", OPTIC_NERVE_R_PRV_ID); } catch { }
                    try { oralcavity_PTV = ss.AddStructure("Avoidance", OPT_ORAL_CAVITY_ID); } catch { }
                    try { parotid_L_PTV = ss.AddStructure("Avoidance", OPT_PAROTID_L_ID); } catch { }
                    try { parotid_R_PTV = ss.AddStructure("Avoidance", OPT_PAROTID_R_ID); } catch { }
                    try { parotids = ss.AddStructure("Avoidance", PAROTIDS_ID); } catch { }
                    try { spnlcrd_PRV = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); } catch { }
                    try { thyroid_PTV = ss.AddStructure("Avoidance", OPT_THYROID_ID); } catch { }
                    try { trachea_PTV = ss.AddStructure("Avoidance", OPT_TRACHEA_ID); } catch { }
                    try { smblr_gl_L_PTV = ss.AddStructure("Avoidance", OPT_SUBMANDIBULAR_GLAND_L_ID); } catch { }
                    try { smblr_gl_R_PTV = ss.AddStructure("Avoidance", OPT_SUBMANDIBULAR_GLAND_R_ID); } catch { }

                    try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    try { larynx = ss.AddStructure("Avoidance", LARYNX_ID); } catch { }
                    try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    //Create trash structures
                    Structure body_lung_L = ss.Structures.FirstOrDefault(x => x.Id == BODY_LUNG_L_ID);
                    Structure body_lung_L_R = ss.Structures.FirstOrDefault(x => x.Id == BODY_LUNG_L_R_ID);
                    Structure body_parotid_L = ss.Structures.FirstOrDefault(x => x.Id == BODY_PAROTID_L_ID);
                    Structure body_parotid_L_R = ss.Structures.FirstOrDefault(x => x.Id == BODY_PAROTID_L_R_ID);
                    Structure ptv_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_3MM_PTV_ALL_ID);


                    try { body_lung_L = ss.AddStructure("Avoidance", BODY_LUNG_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created!", BODY_LUNG_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { body_lung_L_R = ss.AddStructure("Avoidance", BODY_LUNG_L_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created!", BODY_LUNG_L_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { body_parotid_L = ss.AddStructure("Avoidance", BODY_PAROTID_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created!", BODY_PAROTID_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { body_parotid_L_R = ss.AddStructure("Avoidance", BODY_PAROTID_L_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created!", BODY_PAROTID_L_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { ptv_3mm = ss.AddStructure("PTV", EXPANDED_3MM_PTV_ALL_ID); } catch { MessageBox.Show(string.Format("{0} could not be created!", EXPANDED_3MM_PTV_ALL_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }



                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    Structure BODY_HR = ss.Structures.FirstOrDefault(x => x.Id == "BODY_HR");
                    //Define PTV_ALL and generate expanded ptv_all with 3mm margin
                    try { BODY_HR = ss.AddStructure("Avoidance", "BODY_HR"); } catch { MessageBox.Show(string.Format("BODY_HR could not be created!"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                    if (ptv != null)
                    {

                        if (ptv.IsHighResolution)
                        {
                            try
                            {
                                BODY_HR.SegmentVolume = BODY.SegmentVolume;
                                BODY_HR.ConvertToHighResolution();
                                ptv_3mm.ConvertToHighResolution();
                            }
                            catch { }
                        }
                        else
                        {

                        }

                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PTV 54/60/66 OPT structures

                    ptv_3mm.SegmentVolume = ptv.Margin(3.0);


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define Lungs structure

                    if (BODY != null && lungs != null && lung_R != null)
                    {

                        body_lung_L.SegmentVolume = BODY.Sub(lung_L);
                        body_lung_L_R.SegmentVolume = body_lung_L.Sub(lung_R);
                        lungs.SegmentVolume = BODY.Sub(body_lung_L_R);

                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define Parotids structure
                    if (BODY != null && parotid_L != null && parotid_R != null)
                    {
                        if (ptv.IsHighResolution)
                        {

                            try { parotid_L.ConvertToHighResolution(); } catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", PAROTID_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                            try { parotid_R.ConvertToHighResolution(); } catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", PAROTID_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                            try { body_parotid_L.ConvertToHighResolution(); } catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", BODY_PAROTID_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                            try { body_parotid_L_R.ConvertToHighResolution(); } catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", BODY_PAROTID_L_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                            try { parotids.ConvertToHighResolution(); } catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", PAROTIDS_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                            try { body_parotid_L.SegmentVolume = BODY_HR.Sub(parotid_L); } catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", PAROTID_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                            try { body_parotid_L_R.SegmentVolume = body_parotid_L.Sub(parotid_R); } catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", PAROTID_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                            try { parotids.SegmentVolume = BODY_HR.Sub(body_parotid_L_R); } catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", BODY_PAROTID_L_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                        }
                        else
                        {
                            body_parotid_L.SegmentVolume = BODY.Sub(parotid_L);
                            body_parotid_L_R.SegmentVolume = body_parotid_L.Sub(parotid_R);
                            parotids.SegmentVolume = BODY.Sub(body_parotid_L_R);
                        }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 5mm margin. If a structure is empty, the empty PRV will be created

                    if (brainstem != null)
                    {
                        brainstem_PRV.SegmentVolume = brainstem.Margin(5.0);
                    }
                    if (brainstem == null)
                    {
                        try { brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", BRAINSTEM_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (chiasm != null)
                    {
                        try { chiasm_PRV.SegmentVolume = chiasm.Margin(5.0); } catch { MessageBox.Show(string.Format("{0} could not be created", CHIASM_PRV), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (chiasm == null)
                    {
                        try { chiasm = ss.AddStructure("Avoidance", CHIASM_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", CHIASM_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (lens_L != null)
                    {
                        try { lens_L_PRV.SegmentVolume = lens_L.Margin(5.0); } catch { MessageBox.Show(string.Format("{0} could not be created", LENS_L_PRV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (lens_L == null)
                    {
                        try { lens_L = ss.AddStructure("Avoidance", LENS_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", LENS_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (lens_R != null)
                    {
                        try { lens_R_PRV.SegmentVolume = lens_R.Margin(5.0); } catch { MessageBox.Show(string.Format("{0} could not be created", LENS_R_PRV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (lens_R == null)
                    {
                        try { lens_R = ss.AddStructure("Avoidance", LENS_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", LENS_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (opticnerve_L != null)
                    {
                        try { opt_nrv_L_PRV.SegmentVolume = opticnerve_L.Margin(5.0); } catch { MessageBox.Show(string.Format("{0} could not be created", OPTIC_NERVE_L_PRV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (opticnerve_L == null)
                    {
                        try { opticnerve_L = ss.AddStructure("Avoidance", OPTIC_NERVE_L_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", OPTIC_NERVE_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (opticnerve_R != null)
                    {
                        try { opt_nrv_R_PRV.SegmentVolume = opticnerve_R.Margin(5.0); } catch { MessageBox.Show(string.Format("{0} could not be created", OPTIC_NERVE_R_PRV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (opticnerve_R == null)
                    {
                        try { opticnerve_R = ss.AddStructure("Avoidance", OPTIC_NERVE_R_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", OPTIC_NERVE_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (spinalcord != null)
                    {
                        try { spnlcrd_PRV.SegmentVolume = spinalcord.Margin(5.0); } catch { MessageBox.Show(string.Format("{0} could not be created", SPINAL_CORD_PRV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (spinalcord == null)
                    {
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { MessageBox.Show(string.Format("{0} could not be created", SPINAL_CORD_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //
                    if (ptv.IsHighResolution)
                    {
                        foreach (Structure s in ss.Structures)
                        {
                            if (s.IsHighResolution == false && s.Id != "BODY" && s.Id != "CouchSurface" && s.Id != "CouchInterior")
                            {
                                try { s.ConvertToHighResolution(); }
                                catch { MessageBox.Show(string.Format("{0} could not be converted to high resolution", s.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                            }
                        }
                    }


                    //Substract critical structures from expanded PTV_ALL to create 3mm buffer


                    if (pharconst != null)
                    {
                        try { pharconst_PTV.SegmentVolume = pharconst.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_PHAR_CONST_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (pharconst == null)
                    {
                        try { pharconst_PTV = ss.AddStructure("Avoidance", OPT_PHAR_CONST_ID); } catch { }
                    }

                    if (brainstem != null)
                    {
                        try { brainstem_PTV.SegmentVolume = brainstem.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_BRAINSTEM_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (brainstem == null)
                    {
                        try { brainstem_PTV = ss.AddStructure("AVOIDANCE", OPT_BRAINSTEM_ID); } catch { }
                    }

                    if (chiasm != null)
                    {
                        try { chiasm_PTV.SegmentVolume = chiasm.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_CHIASM_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (chiasm == null)
                    {
                        try { chiasm_PTV = ss.AddStructure("AVOIDANCE", OPT_CHIASM_ID); } catch { }
                    }

                    if (esophagus != null)
                    {
                        try { esophagus_PTV.SegmentVolume = esophagus.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_ESOPHAGUS_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (esophagus == null)
                    {
                        try { esophagus_PTV = ss.AddStructure("AVOIDANCE", OPT_ESOPHAGUS_ID); } catch { }
                    }

                    if (larynx != null)
                    {
                        try { larynx_PTV.SegmentVolume = larynx.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not be could not substracted from PTV", OPT_LARYNX_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (larynx == null)
                    {
                        try { larynx_PTV = ss.AddStructure("AVOIDANCE", OPT_LARYNX_ID); } catch { }
                    }

                    if (mandible != null)
                    {
                        try { mandible_PTV.SegmentVolume = mandible.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_MANDIBLE_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (mandible == null)
                    {
                        try { mandible_PTV = ss.AddStructure("AVOIDANCE", OPT_MANDIBLE_ID); } catch { }
                    }

                    if (oralcavity != null)
                    {
                        try { oralcavity_PTV.SegmentVolume = oralcavity.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_ORAL_CAVITY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (oralcavity == null)
                    {
                        try { oralcavity_PTV = ss.AddStructure("AVOIDANCE", OPT_ORAL_CAVITY_ID); } catch { }
                    }

                    if (parotid_L != null)
                    {
                        try { parotid_L_PTV.SegmentVolume = parotid_L.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_PAROTID_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (parotid_L == null)
                    {
                        try { parotid_L_PTV = ss.AddStructure("AVOIDANCE", OPT_PAROTID_L_ID); } catch { }
                    }

                    if (parotid_R != null)
                    {
                        try { parotid_R_PTV.SegmentVolume = parotid_R.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_PAROTID_R_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (parotid_R == null)
                    {
                        try { parotid_L_PTV = ss.AddStructure("AVOIDANCE", OPT_PAROTID_R_ID); } catch { }
                    }

                    if (thyroid != null)
                    {
                        try { thyroid_PTV.SegmentVolume = thyroid.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_THYROID_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (thyroid == null)
                    {
                        try { thyroid_PTV = ss.AddStructure("AVOIDANCE", OPT_THYROID_ID); } catch { }
                    }

                    if (trachea != null)
                    {
                        try { trachea_PTV.SegmentVolume = trachea.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_TRACHEA_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                    if (trachea == null)
                    {
                        try { trachea_PTV = ss.AddStructure("AVOIDANCE", OPT_TRACHEA_ID); } catch { }
                    }

                    if (submgl_L != null)
                    {
                        try { smblr_gl_L_PTV.SegmentVolume = submgl_L.Sub(ptv_3mm); } catch { MessageBox.Show(string.Format("{0} could not substracted from PTV", OPT_SUBMANDIBULAR_GLAND_L_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    if (submgl_R != null)
                    {
                        try { smblr_gl_R_PTV.SegmentVolume = submgl_R.Sub(ptv_3mm); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Check if the summ structures was created. If no, than create empty structures for report generation

                    if (lung_R == null) { try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { } }
                    if (lung_L == null) { try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { } }
                    if (lungs == null) { try { lungs = ss.AddStructure("Avoidance", LUNGS_ID); } catch { } }
                    if (submgl_L == null) { try { submgl_L = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_L_ID); } catch { } }
                    if (submgl_R == null) { try { submgl_R = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_R_ID); } catch { } }
                    if (submgl_L == null) { try { submgl_L = ss.AddStructure("Avoidance", SUBMANDIBULAR_GLAND_L_ID); } catch { } }
                    if (parotid_L == null) { try { parotid_L = ss.AddStructure("Avoidance", PAROTID_L_ID); } catch { } }
                    if (parotid_R == null) { try { parotid_R = ss.AddStructure("Avoidance", PAROTID_R_ID); } catch { } }
                    if (parotids == null) { try { parotids = ss.AddStructure("Avoidance", PAROTIDS_ID); } catch { } }
                    if (lips == null) { try { lips = ss.AddStructure("Avoidance", LIPS_ID); } catch { } }


                    ptv_opt.SegmentVolume = ptv.Margin(0);
                    try { ptv_opt.SegmentVolume = ptv_opt.Sub(brainstem); } catch { }
                    try { ptv_opt.SegmentVolume = ptv_opt.Sub(chiasm); } catch { }
                    try
                    {ptv_opt.SegmentVolume = ptv_opt.Sub(opticnerve_L); }
                    catch { }
                    try
                    {ptv_opt.SegmentVolume = ptv_opt.Sub(opticnerve_R); }
                    catch { }


                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}",

                            ptv.Id, ptv.Volume,

                            ptv_opt.Id, ptv_opt.Volume

                            );

                        string message_OAR = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}",

                            brainstem.Id, brainstem.Volume,

                            brainstem_PTV.Id, brainstem_PTV.Volume,

                            brainstem_PRV.Id, brainstem_PRV.Volume,

                            chiasm.Id, chiasm.Volume,

                            chiasm_PTV.Id, chiasm_PTV.Volume,

                            chiasm_PRV.Id, chiasm_PRV.Volume,

                            esophagus.Id, esophagus.Volume,

                            esophagus_PTV.Id, esophagus_PTV.Volume,

                            eye_L.Id, eye_L.Volume,

                            eye_R.Id, eye_R.Volume,

                            larynx.Id, larynx.Volume,

                            larynx_PTV.Id, larynx_PTV.Volume,

                            lens_L.Id, lens_L.Volume,

                            lens_L_PRV.Id, lens_L_PRV.Volume,

                            lung_R.Id, lung_R.Volume,

                            lung_L.Id, lung_L.Volume
                            );




                        string message_OAR_2 = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}",

                                oralcavity.Id, oralcavity.Volume,

                                parotids.Id, parotids.Volume,

                                parotid_L.Id, parotid_L.Volume,

                                parotid_L_PTV.Id, parotid_L_PTV.Volume,

                                parotid_R.Id, parotid_R.Volume,

                                parotid_R_PTV.Id, parotid_R_PTV.Volume,

                                spinalcord.Id, spinalcord.Volume,

                                spnlcrd_PRV.Id, spnlcrd_PRV.Volume,

                                thyroid.Id, thyroid.Volume,

                                thyroid_PTV.Id, thyroid_PTV.Volume,

                                trachea.Id, trachea.Volume,

                                lungs.Id, lungs.Volume,

                                mandible.Id, mandible.Volume,

                                mandible_PTV.Id, mandible_PTV.Volume,

                                opticnerve_L.Id, opticnerve_L.Volume,

                                opt_nrv_L_PRV.Id, opt_nrv_L_PRV.Volume
                                );

                        string message_OAR_3 = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}",

                                trachea_PTV.Id, trachea_PTV.Volume,

                                lips.Id, lips.Volume,

                                submgl_L.Id, submgl_L.Volume,

                                smblr_gl_L_PTV.Id, smblr_gl_L_PTV.Volume,

                                submgl_R.Id, submgl_R.Volume,

                                smblr_gl_R_PTV.Id, smblr_gl_R_PTV.Volume,

                                opticnerve_R.Id, opticnerve_R.Volume,

                                opt_nrv_R_PRV.Id, opt_nrv_R_PRV.Volume,

                                pharconst.Id, pharconst.Volume,

                                pharconst_PTV.Id, pharconst_PTV.Volume

                                );

                        //Show the messages

                        MessageBox.Show(message_PTV);
                        MessageBox.Show(message_OAR);
                        MessageBox.Show(message_OAR_2);
                        MessageBox.Show(message_OAR_3);
                    }
                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown...")); }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations


                    try { ss.RemoveStructure(ptv_3mm); } catch { }
                    try { ss.RemoveStructure(body_parotid_L_R); } catch { }
                    try { ss.RemoveStructure(body_parotid_L); } catch { }
                    try { ss.RemoveStructure(body_lung_L_R); } catch { }
                    try { ss.RemoveStructure(body_lung_L); } catch { }
                    try { ss.RemoveStructure(BODY_HR); } catch { }
                }
                if (AbdominalRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script

                    const string SCRIPT_NAME = "OPT_str_Abdominal";

                    //IDs of PTV structures

                    const string PTV_ID = "PTV";
                    const string EXPANDED_PTV_ID = "PTV+3mm";

                    //Due to the inability to add structures one to another (boolean "And" seems to be having malfunction), it is mandatory to create some "trash" structures for the purpose of creating PTV summ, Lungs and Parotids. These structures will be removed at the end of this script

                    const string BODY_KIDN_L_ID = "BODY-KDN_L";
                    const string BODY_KDN_L_D_ID = "BODY-KDN_L_R";
                    const string BODY_FEM_L_ID = "BODY-FEM_L";
                    const string BODY_FEM_L_R_ID = "BODY-FEM_L_R";


                    //IDs of critical structures
                    const string BOWELBAG_ID = "BowelBag";
                    const string OPT_BOWELBAG_ID = "BwlBag-PTV";

                    const string RECTUM_ID = "Rectum";
                    const string OPT_RECTUM_ID = "Rectum-PTV";

                    const string FEM_L_ID = "FemoralHead_L";
                    const string OPT_FEM_L_ID = "FemHead_L-PTV";

                    const string FEM_R_ID = "FemoralHead_R";
                    const string OPT_FEM_R_ID = "FemHead_R-PTV";

                    const string FEMORALS_ID = "FmrlHeads";

                    const string BLADDER_ID = "Bladder";
                    const string OPT_BALDDER_ID = "Bladder-PTV";

                    const string KIDNEY_L_ID = "Kidney_L";
                    const string OPT_KIDNEY_L_ID = "Kidney_L-PTV";

                    const string KIDNEY_R_ID = "Kidney_R";
                    const string OPT_KIDNEY_R_ID = "Kidney_R-PTV";

                    const string KIDNEYS_ID = "Kidneys";

                    const string STOMACH_ID = "Stomach";
                    const string OPT_STOMACH_ID = "Stomach-PTV";

                    const string DUODENUM_ID = "Duodenum";
                    const string OPT_DUODENUM_ID = "Duodenum-PTV";

                    const string LIVER_ID = "Liver";
                    const string OPT_LIVER_ID = "Liver-PTV";

                    const string SPINALCRD_ID = "SpinalCord";
                    const string PRV_SPINALCRD_ID = "SpnlCord_PRV";

                    const string BODY_ID = "BODY";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structures: PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);


                    List<string> NOT_FOUND_list = new List<string>();
                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution

                    //find BowelBag
                    Structure bowelbag = ss.Structures.FirstOrDefault(x => x.Id == BOWELBAG_ID);
                    if (bowelbag == null)
                    {
                        NOT_FOUND_list.Add(BOWELBAG_ID);
                        try { bowelbag = ss.AddStructure("Avoidance", BOWELBAG_ID); } catch { }
                    }

                    //find Rectum
                    Structure rectum = ss.Structures.FirstOrDefault(x => x.Id == RECTUM_ID);
                    if (rectum == null)
                    {
                        NOT_FOUND_list.Add(RECTUM_ID);
                        try { rectum = ss.AddStructure("Avoidance", RECTUM_ID); } catch { }
                    }

                    //find FemoralHead_L
                    Structure femoralhead_L = ss.Structures.FirstOrDefault(x => x.Id == FEM_L_ID);
                    if (femoralhead_L == null)
                    {
                        NOT_FOUND_list.Add(FEM_L_ID);
                        try { femoralhead_L = ss.AddStructure("Avoidance", FEM_L_ID); } catch { }
                    }

                    //find FemoralHead_R
                    Structure femoralhead_R = ss.Structures.FirstOrDefault(x => x.Id == FEM_R_ID);
                    if (femoralhead_R == null)
                    {
                        NOT_FOUND_list.Add(FEM_R_ID);
                        try { femoralhead_R = ss.AddStructure("Avoidance", FEM_R_ID); } catch { }
                    }


                    //find Bladder
                    Structure bladder = ss.Structures.FirstOrDefault(x => x.Id == BLADDER_ID);
                    if (bladder == null)
                    {
                        NOT_FOUND_list.Add(BLADDER_ID);
                        try { bladder = ss.AddStructure("Avoidance", BLADDER_ID); } catch { }
                    }

                    //find Kidney_L
                    Structure kidney_L = ss.Structures.FirstOrDefault(x => x.Id == KIDNEY_L_ID);
                    if (kidney_L == null)
                    {
                        NOT_FOUND_list.Add(KIDNEY_L_ID);
                        try { kidney_L = ss.AddStructure("Avoidance", KIDNEY_L_ID); } catch { }
                    }

                    //find Kidney_R
                    Structure kidney_R = ss.Structures.FirstOrDefault(x => x.Id == KIDNEY_R_ID);
                    if (kidney_R == null)
                    {
                        NOT_FOUND_list.Add(KIDNEY_R_ID);
                        try { kidney_R = ss.AddStructure("Avoidance", KIDNEY_R_ID); } catch { }
                    }

                    //find Stomach
                    Structure stomach = ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID);
                    if (stomach == null)
                    {
                        NOT_FOUND_list.Add(STOMACH_ID);
                        try { stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                    }

                    //find Duodenum
                    Structure duodenum = ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID);
                    if (duodenum == null)
                    {
                        NOT_FOUND_list.Add(DUODENUM_ID);
                        try { duodenum = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }

                    //find Liver
                    Structure liver = ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID);
                    if (liver == null)
                    {
                        NOT_FOUND_list.Add(LIVER_ID);
                        try { liver = ss.AddStructure("Avoidance", LIVER_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINALCRD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINALCRD_ID);
                        try { spinalcord = ss.AddStructure("Avoidance", SPINALCRD_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
                    {
                        NOT_FOUND_list.Add(BODY_ID);
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




                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV structure in the structure set. If PTVs will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Window.GetWindow(this).Close(); return;
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Check if the optimization structures already exist. If so, delete it from the structure set

                    Structure bowelbag_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_BOWELBAG_ID);
                    if (bowelbag_PTV != null)
                    {
                        try { ss.RemoveStructure(bowelbag_PTV); } catch { }
                    }

                    Structure rectum_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_RECTUM_ID);
                    if (rectum_PTV != null)
                    {
                        try { ss.RemoveStructure(rectum_PTV); } catch { }
                    }

                    Structure femoral_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_FEM_L_ID);
                    if (femoral_L_PTV != null)
                    {
                        try { ss.RemoveStructure(femoral_L_PTV); } catch { }
                    }

                    Structure femoral_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_FEM_R_ID);
                    if (femoral_R_PTV != null)
                    {
                        try { ss.RemoveStructure(femoral_R_PTV); } catch { }
                    }

                    Structure femorals = ss.Structures.FirstOrDefault(x => x.Id == FEMORALS_ID);
                    if (femorals != null)
                    {
                        try { ss.RemoveStructure(femorals); } catch { }
                    }

                    Structure bladder_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_BALDDER_ID);
                    if (bladder_PTV != null)
                    {
                        try { ss.RemoveStructure(bladder_PTV); } catch { }
                    }

                    Structure kidney_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_KIDNEY_L_ID);
                    if (kidney_L_PTV != null)
                    {
                        try { ss.RemoveStructure(kidney_L_PTV); } catch { }
                    }

                    Structure kidney_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_KIDNEY_R_ID);
                    if (kidney_R_PTV != null)
                    {
                        try { ss.RemoveStructure(kidney_R_PTV); } catch { }
                    }

                    Structure kidneys = ss.Structures.FirstOrDefault(x => x.Id == KIDNEYS_ID);
                    if (kidneys != null)
                    {
                        try { ss.RemoveStructure(kidneys); } catch { }
                    }

                    Structure stomach_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_STOMACH_ID);
                    if (stomach_PTV != null)
                    {
                        try { ss.RemoveStructure(stomach_PTV); } catch { }
                    }

                    Structure duodenum_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_DUODENUM_ID);
                    if (duodenum_PTV != null)
                    {
                        try { ss.RemoveStructure(duodenum_PTV); } catch { }
                    }

                    Structure liver_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LIVER_ID);
                    if (liver_PTV != null)
                    {
                        try { ss.RemoveStructure(liver_PTV); } catch { }
                    }

                    Structure spinalcrd_PRV = ss.Structures.FirstOrDefault(x => x.Id == PRV_SPINALCRD_ID);
                    if (spinalcrd_PRV != null)
                    {
                        try { ss.RemoveStructure(spinalcrd_PRV); } catch { }
                    }

                    Structure ptv_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_PTV_ID);
                    if (ptv_3mm != null)
                    {
                        try { ss.RemoveStructure(ptv_3mm); } catch { }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script


                    try { ptv_3mm = ss.AddStructure("Avoidance", EXPANDED_PTV_ID); } catch { }
                    try { bowelbag_PTV = ss.AddStructure("Avoidance", OPT_BOWELBAG_ID); } catch { }
                    try { rectum_PTV = ss.AddStructure("Avoidance", OPT_RECTUM_ID); } catch { }
                    try { femoral_L_PTV = ss.AddStructure("Avoidance", OPT_FEM_L_ID); } catch { }
                    try { femoral_R_PTV = ss.AddStructure("Avoidance", OPT_FEM_R_ID); } catch { }
                    try { femorals = ss.AddStructure("Avoidance", FEMORALS_ID); } catch { }
                    try { bladder_PTV = ss.AddStructure("Avoidance", OPT_BALDDER_ID); } catch { }
                    try { kidney_L_PTV = ss.AddStructure("Avoidance", OPT_KIDNEY_L_ID); } catch { }
                    try { kidney_R_PTV = ss.AddStructure("Avoidance", OPT_KIDNEY_R_ID); } catch { }
                    try { kidneys = ss.AddStructure("Avoidance", KIDNEYS_ID); } catch { }
                    try { stomach_PTV = ss.AddStructure("Avoidance", OPT_STOMACH_ID); } catch { }
                    try { duodenum_PTV = ss.AddStructure("Avoidance", OPT_DUODENUM_ID); } catch { }
                    try { liver_PTV = ss.AddStructure("Avoidance", OPT_LIVER_ID); } catch { }



                    //Create "trash" structures
                    Structure body_kid_L = ss.Structures.FirstOrDefault(x => x.Id == BODY_KIDN_L_ID);
                    try { body_kid_L = ss.AddStructure("Avoidance", BODY_KIDN_L_ID); } catch { }

                    Structure body_kid_L_R = ss.Structures.FirstOrDefault(x => x.Id == BODY_KDN_L_D_ID);
                    try { body_kid_L_R = ss.AddStructure("Avoidance", BODY_KDN_L_D_ID); } catch { }

                    Structure body_fem_L = ss.Structures.FirstOrDefault(x => x.Id == BODY_FEM_L_ID);
                    try { body_fem_L = ss.AddStructure("Avoidance", BODY_FEM_L_ID); } catch { }

                    Structure body_fem_L_R = ss.Structures.FirstOrDefault(x => x.Id == BODY_FEM_L_R_ID);
                    try { body_fem_L_R = ss.AddStructure("Avoidance", BODY_FEM_L_R_ID); } catch { }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Generate 3mm margin from ptv

                    if (ptv != null)
                    {
                        ptv_3mm.SegmentVolume = ptv.Margin(3.0);
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define femorals structure
                    if (BODY != null && femoralhead_L != null && femoralhead_R != null)
                    {

                        body_fem_L.SegmentVolume = BODY.Sub(femoralhead_L);
                        body_fem_L_R.SegmentVolume = body_fem_L.Sub(femoralhead_R);
                        femorals.SegmentVolume = BODY.Sub(body_fem_L_R);

                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define kidneys structure
                    if (BODY != null && kidney_L != null && kidney_R != null)
                    {

                        body_kid_L.SegmentVolume = BODY.Sub(kidney_L);
                        body_kid_L_R.SegmentVolume = body_kid_L.Sub(kidney_R);
                        kidneys.SegmentVolume = BODY.Sub(body_kid_L_R);

                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 5mm margin. If a structure is empty, the empty PRV will be created

                    if (spinalcord != null)
                    {
                        try { spinalcrd_PRV = ss.AddStructure("Avoidance", PRV_SPINALCRD_ID); } catch { }
                        try { spinalcrd_PRV.SegmentVolume = spinalcord.Margin(5.0); } catch { }
                    }
                    if (spinalcord == null)
                    {
                        try { spinalcrd_PRV = ss.AddStructure("Avoidance", PRV_SPINALCRD_ID); } catch { }
                    }



                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Substract critical structures from expanded PTV_ALL to create 3mm buffer

                    if (bowelbag != null)
                    {
                        try { bowelbag_PTV.SegmentVolume = bowelbag.Sub(ptv_3mm); } catch { }
                    }
                    if (bowelbag == null)
                    {
                        try { bowelbag = ss.AddStructure("Avoidance", BOWELBAG_ID); } catch { }
                    }

                    if (rectum != null)
                    {
                        try { rectum_PTV.SegmentVolume = rectum.Sub(ptv_3mm); } catch { }
                    }
                    if (rectum == null)
                    {
                        try { rectum = ss.AddStructure("Avoidance", RECTUM_ID); } catch { }
                    }

                    if (femoralhead_L != null)
                    {
                        try { femoral_L_PTV.SegmentVolume = femoralhead_L.Sub(ptv_3mm); } catch { }
                    }
                    if (femoralhead_L == null)
                    {
                        try { femoralhead_L = ss.AddStructure("Avoidance", FEM_L_ID); } catch { }
                    }

                    if (femoralhead_R != null)
                    {
                        try { femoral_R_PTV.SegmentVolume = femoralhead_R.Sub(ptv_3mm); } catch { }
                    }
                    if (femoralhead_R == null)
                    {
                        try { femoralhead_R = ss.AddStructure("Avoidance", FEM_R_ID); } catch { }
                    }

                    if (bladder != null)
                    {
                        try { bladder_PTV.SegmentVolume = bladder.Sub(ptv_3mm); } catch { }
                    }
                    if (bladder == null)
                    {
                        try { bladder = ss.AddStructure("Avoidance", BLADDER_ID); } catch { }
                    }

                    if (kidney_L != null)
                    {
                        try { kidney_L_PTV.SegmentVolume = kidney_L.Sub(ptv_3mm); } catch { }
                    }
                    if (kidney_L == null)
                    {
                        try { kidney_L = ss.AddStructure("Avoidance", KIDNEY_L_ID); } catch { }
                    }

                    if (kidney_R != null)
                    {
                        try { kidney_R_PTV.SegmentVolume = kidney_R.Sub(ptv_3mm); } catch { }
                    }
                    if (kidney_R == null)
                    {
                        try { kidney_R = ss.AddStructure("Avoidance", KIDNEY_R_ID); } catch { }
                    }

                    if (stomach != null)
                    {
                        try { stomach_PTV.SegmentVolume = stomach.Sub(ptv_3mm); } catch { }
                    }
                    if (stomach == null)
                    {
                        try { stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                    }

                    if (duodenum != null)
                    {
                        try { duodenum_PTV.SegmentVolume = duodenum.Sub(ptv_3mm); } catch { }
                    }
                    if (duodenum == null)
                    {
                        try { duodenum = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }

                    if (liver != null)
                    {
                        try { liver_PTV.SegmentVolume = liver.Sub(ptv_3mm); } catch { }
                    }
                    if (liver == null)
                    {
                        try { liver = ss.AddStructure("Avoidance", LIVER_ID); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Check if the summ structures was created. If no, than create empty structures for report generation

                    if (femoralhead_L == null)
                    {
                        try { femoralhead_L = ss.AddStructure("Avoidance", FEM_L_ID); } catch { }
                    }
                    if (femoralhead_L == null)
                    {
                        try { femoralhead_R = ss.AddStructure("Avoidance", FEM_R_ID); } catch { }
                    }
                    if (femoralhead_R == null)
                    {
                        try { femorals = ss.AddStructure("Avoidance", FEMORALS_ID); } catch { }
                    }


                    if (kidney_L == null)
                    {
                        try { kidney_L = ss.AddStructure("Avoidance", KIDNEY_L_ID); } catch { }
                    }
                    if (kidney_R == null)
                    {
                        try { kidney_R = ss.AddStructure("Avoidance", KIDNEY_R_ID); } catch { }
                    }
                    if (kidneys == null)
                    {
                        try { kidneys = ss.AddStructure("Avoidance", KIDNEYS_ID); } catch { }
                    }

                    if (bowelbag == null)
                    {
                        try { bowelbag = ss.AddStructure("Avoidance", BOWELBAG_ID); } catch { }
                    }
                    if (rectum == null)
                    {
                        try { rectum = ss.AddStructure("Avoidance", RECTUM_ID); } catch { }
                    }
                    if (bladder == null)
                    {
                        try { bladder = ss.AddStructure("Avoidance", BLADDER_ID); } catch { }
                    }
                    if (stomach == null)
                    {
                        try { stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                    }
                    if (duodenum == null)
                    {
                        try { duodenum = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }
                    if (liver == null)
                    {
                        try { liver = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }
                    if (spinalcord == null)
                    {
                        try { spinalcord = ss.AddStructure("Avoidance", SPINALCRD_ID); } catch { }
                    }




                    //Create a message after the script is executed
                    try
                    {
                        string message = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}\n{32} volume ={33}\n{34} volume ={35}\n{36} volume ={37}\n{38} volume ={39}\n{40} volume ={41}\n{42} volume ={43}\n{44} volume ={45}\n{46} volume ={47}\n{48} volume ={49}",

                            ptv.Id, ptv.Volume,
                            ptv_3mm.Id, ptv_3mm.Volume,
                            bowelbag.Id, bowelbag.Volume,
                            bowelbag_PTV.Id, bowelbag_PTV.Volume,
                            rectum.Id, rectum.Volume,
                            rectum_PTV.Id, rectum_PTV.Volume,
                            femoralhead_L.Id, femoralhead_L.Volume,
                            femoral_L_PTV.Id, femoral_L_PTV.Volume,
                            femoralhead_R.Id, femoralhead_R.Volume,
                            femoral_R_PTV.Id, femoral_R_PTV.Volume,
                            bladder.Id, bladder.Volume,
                            bladder_PTV.Id, bladder_PTV.Volume,
                            kidney_L.Id, kidney_L.Volume,
                            kidney_L_PTV.Id, kidney_L_PTV.Volume,
                            kidney_R.Id, kidney_R.Volume,
                            kidney_R_PTV.Id, kidney_R_PTV.Volume,
                            kidneys.Id, kidneys.Volume,
                            stomach.Id, stomach.Volume,
                            stomach_PTV.Id, stomach_PTV.Volume,
                            duodenum.Id, duodenum.Volume,
                            duodenum_PTV.Id, duodenum_PTV.Volume,
                            liver.Id, liver.Volume,
                            liver_PTV.Id, liver_PTV.Volume,
                            spinalcord.Id, spinalcord.Volume,
                            spinalcrd_PRV.Id, spinalcrd_PRV.Volume
                            );

                        MessageBox.Show(message);
                    }
                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); };


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations

                    try { ss.RemoveStructure(ptv_3mm); } catch { }
                    try { ss.RemoveStructure(body_fem_L); } catch { }
                    try { ss.RemoveStructure(body_fem_L_R); } catch { }
                    try { ss.RemoveStructure(body_kid_L); } catch { }
                    try { ss.RemoveStructure(body_kid_L_R); } catch { }
                }
                if (BreastRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_Breast";

                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_EVAL_ID = "PTV_eval";



                    //Due to the inability to add structures one to another (boolean "And" seems to be having malfunction), it is mandatory to create some "trash" structures for the purpose of creating PTV summ, Lungs and Parotids. These structures will be removed at the end of this script
                    const string BODY_LUNG_L_ID = "BODY_LUNG_L";
                    const string BODY_LUNG_L_R_ID = "BODY_LUNG_L_R";


                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_5MM_PTV_EVAL_ID = "PTV_eval_5mm";

                    //IDs of critical structures


                    const string ESOPHAGUS_ID = "Esophagus";
                    const string OPT_ESOPHAGUS_ID = "Esophagus-PTV";
                    const string ESOPHAGUS_PRV = "Esophagus_PRV";
                    const string LUNG_L_ID = "Lung_L";
                    const string OPT_LUNG_L_ID = "Lung_L-PTV";
                    const string LUNG_R_ID = "Lung_R";
                    const string OPT_LUNG_R_ID = "Lung_R-PTV";
                    const string LUNGS_ID = "Lungs";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpnCrd_PRV";
                    const string THYROID_ID = "Thyroid";
                    const string OPT_THYROID_ID = "Thyroid-PTV";
                    const string TRACHEA_ID = "Trachea";
                    const string OPT_TRACHEA_ID = "Trachea-PTV";
                    const string TRACHEA_PRV = "Trachea_PRV";
                    const string HEART_ID = "Heart";
                    const string OPT_HEART_ID = "Heart-PTV";
                    const string BODY_ID = "BODY";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structures: PTV_eval, BODY exist in the current structure set. This script is made by 'PET_Tehnology'.\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);
                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution


                    List<string> NOT_FOUND_list = new List<string>();

                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }

                    //find Lung_L
                    Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_ID);
                    if (lung_L == null)
                    {
                        NOT_FOUND_list.Add(LUNG_L_ID);
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { }
                    }

                    //find Lung_R
                    Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                    if (lung_R == null)
                    {
                        NOT_FOUND_list.Add(LUNG_R_ID);
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Thyroid
                    Structure thyroid = ss.Structures.FirstOrDefault(x => x.Id == THYROID_ID);
                    if (thyroid == null)
                    {
                        NOT_FOUND_list.Add(THYROID_ID);
                        try { thyroid = ss.AddStructure("Avoidance", THYROID_ID); } catch { }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find Heart
                    Structure heart = ss.Structures.FirstOrDefault(x => x.Id == HEART_ID);
                    if (heart == null)
                    {
                        NOT_FOUND_list.Add(HEART_ID);
                        try { heart = ss.AddStructure("Avoidance", HEART_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
                    {
                        NOT_FOUND_list.Add(BODY_ID);
                        try { MessageBox.Show(string.Format("{0} not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
                        Window.GetWindow(this).Close(); return;
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV eval structure in the structure set. If PTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv_eval = ss.Structures.FirstOrDefault(x => x.Id == PTV_EVAL_ID);
                    if (ptv_eval == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_EVAL_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);

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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Check if the optimization structures already exist. If so, delete it from the structure set


                    Structure esophagus_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_ESOPHAGUS_ID);
                    if (esophagus_PTV != null)
                    {
                        try { ss.RemoveStructure(esophagus_PTV); } catch { }
                    }

                    Structure esophagus_PRV = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_PRV);
                    if (esophagus_PRV != null)
                    {
                        try { ss.RemoveStructure(esophagus_PRV); } catch { }
                    }

                    Structure lung_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_L_ID);
                    if (lung_L_PTV != null)
                    {
                        try { ss.RemoveStructure(lung_L_PTV); } catch { }
                    }

                    Structure lung_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_R_ID);
                    if (lung_R_PTV != null)
                    {
                        try { ss.RemoveStructure(lung_R_PTV); } catch { }
                    }

                    Structure lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID);
                    if (lungs != null)
                    {
                        try { ss.RemoveStructure(lungs); } catch { }
                    }


                    Structure spnlcrd_PRV = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spnlcrd_PRV != null)
                    {
                        try { ss.RemoveStructure(spnlcrd_PRV); } catch { }
                    }

                    Structure thyroid_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_THYROID_ID);
                    if (thyroid_PTV != null)
                    {
                        try { ss.RemoveStructure(thyroid_PTV); } catch { }
                    }

                    Structure trachea_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_TRACHEA_ID);
                    if (trachea_PTV != null)
                    {
                        try { ss.RemoveStructure(trachea_PTV); } catch { }
                    }

                    Structure trachea_PRV = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_PRV);
                    if (trachea_PRV != null)
                    {
                        try { ss.RemoveStructure(trachea_PRV); } catch { }
                    }

                    Structure heart_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_HEART_ID);
                    if (heart_PTV != null)
                    {
                        try { ss.RemoveStructure(heart_PTV); } catch { }
                    }

                    if (spnlcrd_PRV != null)
                    {
                        try { ss.RemoveStructure(spnlcrd_PRV); }
                        catch { MessageBox.Show(string.Format("Something went wrong with deleting structure 'SpnCrd_PRV.\n Please, ensure that this PRV has been represented correctly'"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script

                    try { esophagus_PTV = ss.AddStructure("Avoidance", OPT_ESOPHAGUS_ID); } catch { }
                    try { esophagus_PRV = ss.AddStructure("Avoidance", ESOPHAGUS_PRV); } catch { }
                    try { thyroid_PTV = ss.AddStructure("Avoidance", OPT_THYROID_ID); } catch { }
                    try { trachea_PTV = ss.AddStructure("Avoidance", OPT_TRACHEA_ID); } catch { }
                    try { trachea_PRV = ss.AddStructure("Avoidance", TRACHEA_PRV); } catch { }
                    try { lung_L_PTV = ss.AddStructure("Avoidance", OPT_LUNG_L_ID); } catch { }
                    try { lung_R_PTV = ss.AddStructure("Avoidance", OPT_LUNG_R_ID); } catch { }
                    try { heart_PTV = ss.AddStructure("Avoidance", OPT_HEART_ID); } catch { }
                    try { lungs = ss.AddStructure("Avoidance", LUNGS_ID); } catch { }
                    try { spnlcrd_PRV = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); }
                    catch { MessageBox.Show(string.Format("Please, ensure that there is no issues with SpinalCord_PRV"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }





                    //Check is the OAR exist. If no, then create empty structures

                    if (lung_L == null)
                    {
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { }
                    }
                    if (lung_R == null)
                    {
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }

                    //Create trash structures

                    Structure body_lung_L = ss.AddStructure("Avoidance", BODY_LUNG_L_ID);
                    Structure body_lung_L_R = ss.AddStructure("Avoidance", BODY_LUNG_L_R_ID);
                    Structure ptv_eval_5mm = ss.AddStructure("PTV", EXPANDED_5MM_PTV_EVAL_ID);



                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Generate expanded ptv_eval with 5mm margin

                    if (ptv_eval != null)
                    {
                        ptv_eval_5mm.SegmentVolume = ptv_eval.Margin(5.0);
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Define Lungs structure

                    if (BODY != null && lung_L != null && lung_R != null)
                    {
                        body_lung_L.SegmentVolume = BODY.Sub(lung_L);
                        body_lung_L_R.SegmentVolume = body_lung_L.Sub(lung_R);
                        lungs.SegmentVolume = BODY.Sub(body_lung_L_R);
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 5mm margin. If a structure is empty, the empty PRV will be created


                    if (spinalcord != null)
                    {
                        try { spnlcrd_PRV.SegmentVolume = spinalcord.Margin(5.0); } catch { }
                    }
                    if (spinalcord == null)
                    {
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }
                    if (trachea_PRV!=null) { try { trachea_PRV.SegmentVolume = trachea.Margin(5); } catch { } }
                    if (esophagus_PRV!=null) { try { esophagus_PRV.SegmentVolume = esophagus.Margin(5); } catch { } }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Substract critical structures from expanded PTV_ALL to create 3mm buffer



                    if (esophagus != null)
                    {
                        try { esophagus_PTV.SegmentVolume = esophagus.Sub(ptv_eval_5mm); } catch { }
                    }
                    if (esophagus == null)
                    {
                        try { esophagus = ss.AddStructure("AVOIDANCE", ESOPHAGUS_ID); } catch { }
                    }


                    if (thyroid != null)
                    {
                        try { thyroid_PTV.SegmentVolume = thyroid.Sub(ptv_eval_5mm); } catch { }
                    }
                    if (thyroid == null)
                    {
                        try { thyroid = ss.AddStructure("AVOIDANCE", THYROID_ID); } catch { }
                    }

                    if (trachea != null)
                    {
                        try { trachea_PTV.SegmentVolume = trachea.Sub(ptv_eval_5mm); } catch { }
                    }
                    if (trachea == null)
                    {
                        try { trachea = ss.AddStructure("AVOIDANCE", TRACHEA_ID); } catch { }
                    }


                    if (lung_L != null)
                    {
                        try { lung_L_PTV.SegmentVolume = lung_L.Sub(ptv_eval_5mm); } catch { }
                    }
                    if (lung_L == null)
                    {
                        try { lung_L = ss.AddStructure("AVOIDANCE", LUNG_L_ID); } catch { }
                    }

                    if (lung_R != null)
                    {
                        try { lung_R_PTV.SegmentVolume = lung_R.Sub(ptv_eval_5mm); } catch { }
                    }
                    if (lung_R == null)
                    {
                        try { lung_R = ss.AddStructure("AVOIDANCE", LUNG_R_ID); } catch { }
                    }

                    if (heart != null)
                    {
                        try { heart_PTV.SegmentVolume = heart.Sub(ptv_eval_5mm); } catch { }
                    }
                    if (heart == null)
                    {
                        try { heart = ss.AddStructure("AVOIDANCE", HEART_ID); } catch { }
                    }




                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}",

                                ptv_eval.Id, ptv_eval.Volume,
                                ptv_eval_5mm.Id, ptv_eval_5mm.Volume
                                );

                        string message_OAR = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume = {25}\n{26} volume = {27}\n{28} volume = {29}\n{30} volume = {31}",

                            esophagus.Id, esophagus.Volume,

                            esophagus_PTV.Id, esophagus_PTV.Volume,

                            spinalcord.Id, spinalcord.Volume,

                            spnlcrd_PRV.Id, spnlcrd_PRV.Volume,

                            thyroid.Id, thyroid.Volume,

                            thyroid_PTV.Id, thyroid_PTV.Volume,

                            trachea.Id, trachea.Volume,

                            trachea_PTV.Id, trachea_PTV.Volume,

                            heart.Id, heart.Volume,

                            lung_L.Id, lung_L.Volume,

                            lung_L_PTV.Id, lung_L_PTV.Volume,

                            lung_R.Id, lung_R.Volume,

                            lung_R_PTV.Id, lung_R_PTV.Volume,

                            lungs.Id, lungs.Volume,

                            esophagus_PRV.Id,esophagus_PRV.Volume,

                            trachea_PRV.Id,trachea_PRV.Volume

                            );

                        //Show the messages

                        MessageBox.Show(message_PTV);
                        MessageBox.Show(message_OAR);
                    }
                    catch
                    {
                        MessageBox.Show(string.Format("Unfortunately, a report message could not be shown"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations


                    try { ss.RemoveStructure(ptv_eval_5mm); } catch { MessageBox.Show(string.Format("Please, ensure that all trash structures were deleted.\nIf no, delete it manually"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { ss.RemoveStructure(body_lung_L_R); } catch { MessageBox.Show(string.Format("Please, ensure that all trash structures were deleted.\nIf no, delete it manually"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { ss.RemoveStructure(body_lung_L); } catch { MessageBox.Show(string.Format("Please, ensure that all trash structures were deleted.\nIf no, delete it manually"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }
                if (EsophagusRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_Esophagus";

                    //IDs of PTV structure
                    const string PTV_ID = "PTV";


                    //Due to the inability to add structures one to another (boolean "And" seems to be having malfunction), it is mandatory to create some "trash" structures for the purpose of creating PTV summ, Lungs and Parotids. These structures will be removed at the end of this script

                    const string EXPANDED_PTV_ID = "PTV_3mm";

                    const string BODY_LUNG_L_ID = "BODY_LUNG_L";
                    const string BODY_LUNG_L_R_ID = "BODY_LUNG_L_R";
                    const string BODY_KIDNEY_L_ID = "BODY_Kidney_L";
                    const string BODY_KIDNEY_L_R_ID = "BODY_Kidney_L_R";


                    //IDs of critical structures


                    const string ESOPHAGUS_ID = "Esophagus";
                    const string OPT_ESOPHAGUS_ID = "Esophagus-PTV";
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
                    const string KIDNEY_L_ID = "Kidney_L";
                    const string OPT_KIDNEY_L_ID = "Kidney_L-PTV";
                    const string KIDNEY_R_ID = "Kidney_R";
                    const string OPT_KIDNEY_R_ID = "Kidney_R-PTV";
                    const string KIDNEYS_ID = "Kidneys";
                    const string OPT_KIDNEYS_ID = "Kidneys-PTV";
                    const string DUODENUM_ID = "Duodenum";
                    const string OPT_DUODENUM_ID = "Duodenum-PTV";
                    const string STOMACH_ID = "Stomach";
                    const string OPT_STOMACH_ID = "Stomach-PTV";
                    const string LIVER_ID = "Liver";
                    const string OPT_LIVER_ID = "Liver-PTV";
                    const string BOTH_LUNGS_ID = "Both_Lungs";
                    const string PTV_LUNGS_ID = "PTV-Lungs";
                    const string BODY_ID = "BODY";
                    string Greeting = string.Format("Greetings {0}! Please, ensure that structures: PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);
                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution
                    List<string> NOT_FOUND_list = new List<string>();

                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }


                    //find Lung_L
                    Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_ID);
                    if (lung_L == null)
                    {
                        NOT_FOUND_list.Add(LUNG_L_ID);
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { }
                    }

                    //find Lung_R
                    Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                    if (lung_R == null)
                    {
                        NOT_FOUND_list.Add(LUNG_R_ID);
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }


                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }

                    }


                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find Heart
                    Structure heart = ss.Structures.FirstOrDefault(x => x.Id == HEART_ID);
                    if (heart == null)
                    {
                        NOT_FOUND_list.Add(HEART_ID);
                        try { heart = ss.AddStructure("Avoidance", HEART_ID); } catch { }
                    }

                    Structure kidney_L = ss.Structures.FirstOrDefault(x => x.Id == KIDNEY_L_ID);
                    if (kidney_L == null)
                    {
                        NOT_FOUND_list.Add(KIDNEY_L_ID);
                        try { kidney_L = ss.AddStructure("Avoidance", KIDNEY_L_ID); } catch { }
                    }

                    Structure kidney_R = ss.Structures.FirstOrDefault(x => x.Id == KIDNEY_R_ID);
                    if (kidney_R == null)
                    {
                        NOT_FOUND_list.Add(KIDNEY_R_ID);
                        try { kidney_R = ss.AddStructure("Avoidance", KIDNEY_R_ID); } catch { }
                    }

                    Structure duodenum = ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID);
                    if (duodenum == null)
                    {
                        NOT_FOUND_list.Add(DUODENUM_ID);
                        try { duodenum = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }

                    Structure stomach = ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID);
                    if (stomach == null)
                    {
                        NOT_FOUND_list.Add(STOMACH_ID);
                        try { stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                    }

                    Structure liver = ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID);
                    if (liver == null)
                    {
                        NOT_FOUND_list.Add(LIVER_ID);
                        try { liver = ss.AddStructure("Avoidance", LIVER_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return; ;
                    }

                    //show message with list of not found structures
                    string[] NOT_FOUND = NOT_FOUND_list.ToArray();
                    string res = string.Join("\r\n", NOT_FOUND);
                    var array_length = NOT_FOUND.Length;
                    if (array_length > 0)
                    {
                        MessageBox.Show(string.Format("Structures were not found:\n'{0}'", res), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV structure in the structure set. If PTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return; ;
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================




                    //Check if the optimization structures already exist. If so, delete it from the structure set

                    Structure lung_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_R_ID);
                    if (lung_R_PTV != null)
                    {
                        try { ss.RemoveStructure(lung_R_PTV); } catch { }
                    }

                    Structure lung_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_L_ID);
                    if (lung_L_PTV != null)
                    {
                        try { ss.RemoveStructure(lung_L_PTV); } catch { }
                    }

                    Structure lungs_PTV = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_PTV_ID);
                    if (lungs_PTV != null)
                    {
                        try { ss.RemoveStructure(lungs_PTV); } catch { }
                    }

                    Structure esophagus_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_ESOPHAGUS_ID);
                    if (esophagus_PTV != null)
                    {
                        try { ss.RemoveStructure(esophagus_PTV); } catch { }
                    }

                    Structure both_lungs = ss.Structures.FirstOrDefault(x => x.Id == BOTH_LUNGS_ID);
                    if (both_lungs == null)
                    {
                        try { both_lungs = ss.AddStructure("Avoidance", BOTH_LUNGS_ID); } catch { }
                    }

                    Structure lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID);
                    if (lungs != null)
                    {

                        try { both_lungs.SegmentVolume = lungs.Margin(0); } catch { }
                        try { ss.RemoveStructure(lungs); } catch { }
                    }

                    Structure spnlcrd_PRV = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spnlcrd_PRV != null)
                    {
                        try { ss.RemoveStructure(spnlcrd_PRV); } catch { }
                    }

                    Structure trachea_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_TRACHEA_ID);
                    if (trachea_PTV != null)
                    {
                        try { ss.RemoveStructure(trachea_PTV); } catch { }
                    }

                    Structure heart_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_HEART_ID);
                    if (heart_PTV != null)
                    {
                        try { ss.RemoveStructure(heart_PTV); } catch { }
                    }

                    Structure ptv_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_PTV_ID);
                    if (ptv_3mm != null)
                    {
                        try { ss.RemoveStructure(ptv_3mm); } catch { }
                    }

                    Structure kidney_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_KIDNEY_L_ID);
                    if (kidney_L_PTV != null)
                    {
                        try { ss.RemoveStructure(kidney_L_PTV); } catch { }
                    }

                    Structure kidney_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_KIDNEY_R_ID);
                    if (kidney_R_PTV != null)
                    {
                        try { ss.RemoveStructure(kidney_R_PTV); } catch { }
                    }

                    Structure kidneys_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_KIDNEYS_ID);
                    if (kidneys_PTV != null)
                    {
                        try { ss.RemoveStructure(kidneys_PTV); } catch { }
                    }

                    Structure kidneys = ss.Structures.FirstOrDefault(x => x.Id == KIDNEYS_ID);
                    if (kidneys != null)
                    {
                        try { ss.RemoveStructure(kidneys); } catch { }
                    }

                    Structure duodenum_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_DUODENUM_ID);
                    if (duodenum_PTV != null)
                    {
                        try { ss.RemoveStructure(duodenum_PTV); } catch { }
                    }

                    Structure stomach_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_STOMACH_ID);
                    if (stomach_PTV != null)
                    {
                        try { ss.RemoveStructure(stomach_PTV); } catch { }
                    }

                    Structure liver_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LIVER_ID);
                    if (liver_PTV != null)
                    {
                        try { ss.RemoveStructure(liver_PTV); } catch { }
                    }

                    Structure ptv_lungs = ss.Structures.FirstOrDefault(x => x.Id == PTV_LUNGS_ID);
                    if (ptv_lungs != null)
                    {
                        try { ss.RemoveStructure(ptv_lungs); } catch { }
                    }
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script

                    try { ptv_lungs = ss.AddStructure("Avoidance", PTV_LUNGS_ID); } catch { }
                    try { ptv_3mm = ss.AddStructure("Avoidance", EXPANDED_PTV_ID); } catch { }
                    try { esophagus_PTV = ss.AddStructure("Avoidance", OPT_ESOPHAGUS_ID); } catch { }
                    try { lung_L_PTV = ss.AddStructure("Avoidance", OPT_LUNG_L_ID); } catch { }
                    try { lung_R_PTV = ss.AddStructure("Avoidance", OPT_LUNG_R_ID); } catch { }
                    try { lungs = ss.AddStructure("Avoidance", LUNGS_ID); } catch { }
                    try { lungs_PTV = ss.AddStructure("Avoidance", LUNGS_PTV_ID); } catch { }
                    try { spnlcrd_PRV = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); } catch { }
                    try { trachea_PTV = ss.AddStructure("Avoidance", OPT_TRACHEA_ID); } catch { }
                    try { heart_PTV = ss.AddStructure("Avoidance", OPT_HEART_ID); } catch { }
                    try { kidney_L_PTV = ss.AddStructure("Avoidance", OPT_KIDNEY_L_ID); } catch { }
                    try { kidney_R_PTV = ss.AddStructure("Avoidance", OPT_KIDNEY_R_ID); } catch { }
                    try { kidneys = ss.AddStructure("Avoidance", KIDNEYS_ID); } catch { }
                    try { kidneys_PTV = ss.AddStructure("Avoidance", OPT_KIDNEYS_ID); } catch { }
                    try { duodenum_PTV = ss.AddStructure("Avoidance", OPT_DUODENUM_ID); } catch { }
                    try { stomach_PTV = ss.AddStructure("Avoidance", OPT_STOMACH_ID); } catch { }
                    try { liver_PTV = ss.AddStructure("Avoidance", OPT_LIVER_ID); } catch { }



                    //Create trash structures

                    Structure body_lung_L = ss.AddStructure("Avoidance", BODY_LUNG_L_ID);
                    Structure body_lung_L_R = ss.AddStructure("Avoidance", BODY_LUNG_L_R_ID);
                    Structure body_kidney_L = ss.AddStructure("Avoidance", BODY_KIDNEY_L_ID);
                    Structure body_kidney_L_R = ss.AddStructure("Avoidance", BODY_KIDNEY_L_R_ID);



                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define lungs, PTV_Lungs and generate expanded ptv with 3 mm margin
                    if (lung_L != null && lung_R != null)
                    {
                        body_lung_L.SegmentVolume = BODY.Sub(lung_L);
                        body_lung_L_R.SegmentVolume = body_lung_L.Sub(lung_R);
                        lungs.SegmentVolume = BODY.Sub(body_lung_L_R);
                        if (both_lungs.Volume > 0)
                        {
                            lungs.SegmentVolume = both_lungs.Margin(0);
                        }
                    }

                    if (kidney_L != null && kidney_R != null)
                    {
                        body_kidney_L.SegmentVolume = BODY.Sub(kidney_L);
                        body_kidney_L_R.SegmentVolume = body_kidney_L.Sub(kidney_R);
                        kidneys.SegmentVolume = BODY.Sub(body_kidney_L_R);
                    }

                    if (ptv != null && lungs != null)
                    {
                        ptv_lungs.SegmentVolume = ptv.Sub(lungs);
                        ptv_3mm.SegmentVolume = ptv.Margin(3.0);
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================




                    //Define PRVs with 5mm margin. If a structure is empty, the empty PRV will be created



                    if (spinalcord != null)
                    {
                        try { spnlcrd_PRV.SegmentVolume = spinalcord.Margin(5.0); } catch { }
                    }
                    if (spinalcord == null)
                    {
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Substract critical structures from expanded PTV_ALL to create 3mm buffer


                    if (esophagus != null)
                    {
                        try { esophagus_PTV.SegmentVolume = esophagus.Sub(ptv_3mm); } catch { }
                    }
                    if (esophagus == null)
                    {
                        try { esophagus = ss.AddStructure("AVOIDANCE", ESOPHAGUS_ID); } catch { }
                    }


                    if (trachea != null)
                    {
                        try { trachea_PTV.SegmentVolume = trachea.Sub(ptv_3mm); } catch { }
                    }
                    if (trachea == null)
                    {
                        try { trachea = ss.AddStructure("AVOIDANCE", TRACHEA_ID); } catch { }
                    }

                    if (lung_L != null)
                    {
                        try { lung_L_PTV.SegmentVolume = lung_L.Sub(ptv_3mm); } catch { }
                    }
                    if (lung_L == null)
                    {
                        try { lung_L = ss.AddStructure("AVOIDANCE", LUNG_L_ID); } catch { }
                    }

                    if (lung_R != null)
                    {
                        try { lung_R_PTV.SegmentVolume = lung_R.Sub(ptv_3mm); } catch { }
                    }
                    if (lung_R == null)
                    {
                        try { lung_R = ss.AddStructure("AVOIDANCE", LUNG_R_ID); } catch { }
                    }

                    if (lungs != null)
                    {
                        try { lungs_PTV.SegmentVolume = lungs.Sub(ptv_3mm); } catch { }
                    }
                    if (lungs == null)
                    {
                        try { lungs = ss.AddStructure("AVOIDANCE", LUNGS_ID); } catch { }
                    }

                    if (heart != null)
                    {
                        try { heart_PTV.SegmentVolume = heart.Sub(ptv_3mm); } catch { }
                    }
                    if (heart == null)
                    {
                        try { heart = ss.AddStructure("AVOIDANCE", HEART_ID); } catch { }
                    }

                    if (heart != null)
                    {
                        try { heart_PTV.SegmentVolume = heart.Sub(ptv_3mm); } catch { }
                    }
                    if (heart == null)
                    {
                        try { heart = ss.AddStructure("AVOIDANCE", HEART_ID); } catch { }
                    }

                    if (kidney_L != null)
                    {
                        try { kidney_L_PTV.SegmentVolume = kidney_L.Sub(ptv_3mm); } catch { }
                    }
                    if (kidney_L == null)
                    {
                        try { kidney_L = ss.AddStructure("AVOIDANCE", KIDNEY_L_ID); } catch { }
                    }

                    if (kidney_R != null)
                    {
                        try { kidney_R_PTV.SegmentVolume = kidney_R.Sub(ptv_3mm); } catch { }
                    }
                    if (kidney_R == null)
                    {
                        try { kidney_R = ss.AddStructure("AVOIDANCE", KIDNEY_R_ID); } catch { }
                    }

                    if (kidneys != null)
                    {
                        try { kidneys_PTV.SegmentVolume = kidneys.Sub(ptv_3mm); } catch { }
                    }
                    if (kidneys == null)
                    {
                        try { kidneys = ss.AddStructure("AVOIDANCE", KIDNEYS_ID); } catch { }
                    }

                    if (duodenum != null)
                    {
                        try { duodenum_PTV.SegmentVolume = duodenum.Sub(ptv_3mm); } catch { }
                    }
                    if (duodenum == null)
                    {
                        try { duodenum = ss.AddStructure("AVOIDANCE", DUODENUM_ID); } catch { }
                    }

                    if (stomach != null)
                    {
                        try { stomach_PTV.SegmentVolume = stomach.Sub(ptv_3mm); } catch { }
                    }
                    if (stomach == null)
                    {
                        try { stomach = ss.AddStructure("AVOIDANCE", STOMACH_ID); } catch { }
                    }

                    if (liver != null)
                    {
                        try { liver_PTV.SegmentVolume = liver.Sub(ptv_3mm); } catch { }
                    }
                    if (liver == null)
                    {
                        try { liver = ss.AddStructure("AVOIDANCE", LIVER_ID); } catch { }
                    }











                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Check if the summ structures was created. If no, than create empty structures for report generation

                    if (lung_R == null)
                    {
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }
                    if (lung_L == null)
                    {
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { }
                    }
                    if (lungs == null)
                    {
                        try { lungs = ss.AddStructure("Avoidance", LUNGS_ID); } catch { }
                    }

                    if (kidney_L == null)
                    {
                        try { kidney_L = ss.AddStructure("Avoidance", KIDNEY_L_ID); } catch { }
                    }
                    if (kidney_R == null)
                    {
                        try { kidney_R = ss.AddStructure("Avoidance", KIDNEY_R_ID); } catch { }
                    }
                    if (kidneys == null)
                    {
                        try { kidneys = ss.AddStructure("Avoidance", KIDNEYS_ID); } catch { }
                    }

                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}\n{32} volume ={33},\n{34} volume ={35},\n{36} volume ={37},\n{38} volume ={39},\n{40} volume ={41},\n{42} volume ={43},\n{44} volume ={45},\n{46} volume ={47},\n{48} volume ={49},\n{50} volume ={51},\n{52} volume ={53},\n{54} volume ={55}",

                            ptv.Id, ptv.Volume,

                            ptv_3mm.Id, ptv_3mm.Volume,

                            lung_R.Id, lung_R.Volume,

                            lung_R_PTV.Id, lung_R_PTV.Volume,

                            lung_L.Id, lung_L.Volume,

                            lung_L_PTV.Id, lung_L_PTV.Volume,

                            lungs.Id, lungs.Volume,

                            lungs_PTV.Id, lungs_PTV.Volume,

                            esophagus.Id, esophagus.Volume,

                            esophagus_PTV.Id, esophagus_PTV.Volume,

                            trachea.Id, trachea.Volume,

                            trachea_PTV.Id, trachea_PTV.Volume,

                            heart.Id, heart.Volume,

                            heart_PTV.Id, heart_PTV.Volume,

                            spinalcord.Id, spinalcord.Volume,

                            spnlcrd_PRV.Id, spnlcrd_PRV.Volume,

                            kidney_L.Id, kidney_L.Volume,

                            kidney_L_PTV.Id, kidney_L_PTV.Volume,

                            kidney_R.Id, kidney_R.Volume,

                            kidney_R_PTV.Id, kidney_R_PTV.Volume,


                            kidneys.Id, kidneys.Volume,

                            kidneys_PTV.Id, kidneys_PTV.Volume,

                            duodenum.Id, duodenum.Volume,

                            duodenum_PTV.Id, duodenum_PTV.Volume,

                            stomach.Id, stomach.Volume,

                            stomach_PTV.Id, stomach_PTV.Volume,

                            liver.Id, liver.Volume,

                            liver_PTV.Id, liver_PTV.Volume

                            );


                        //Show the messages

                        MessageBox.Show(message_PTV);
                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }



                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations

                    ss.RemoveStructure(ptv_3mm);
                    ss.RemoveStructure(body_lung_L_R);
                    ss.RemoveStructure(body_lung_L);
                    ss.RemoveStructure(body_kidney_L);
                    ss.RemoveStructure(body_kidney_L_R);
                }
                if (PelvisRB.IsChecked == true)
                {
                    // Change these IDs to match your clinical protocol

                    const string PTV_ID = "PTV";
                    const string RECTUM_ID = "Rectum";
                    const string BOWEL_BAG_ID = "BowelBag";
                    const string BLADDER_ID = "Bladder";
                    const string EXPANDED_PTV_ID = "PTV+3mm";
                    const string RECTUM_OPT_ID = "Rectum-PTV";
                    const string BLADDER_OPT_ID = "Bladder-PTV";
                    const string BOWEL_BAG_OPT_ID = "BowelBag-PTV";
                    const string SCRIPT_NAME = "OPT_str_Prostate_No_LN";
                    string Greeting = string.Format("Greetings {0}! Please, ensure that structures: Bowel_bag, Bladder, Rectum, PTV exist in the currest structure set. Script is made by 'PET_Tehnology'.\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru').", user);
                    MessageBox.Show(Greeting);

                    if (VMS.TPS.Script.context.Patient == null || VMS.TPS.Script.context.StructureSet == null)
                    {
                        MessageBox.Show("Please load a patient, 3D image, and structure set before running this script.", SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Window.GetWindow(this).Close(); return;
                    }


                    List<string> NOT_FOUND_list = new List<string>();

                    // find Rectum
                    Structure rectum = ss.Structures.FirstOrDefault(x => x.Id == RECTUM_ID);
                    if (rectum == null)
                    {
                        NOT_FOUND_list.Add(RECTUM_ID);
                        try { rectum = ss.AddStructure("Avoidance", RECTUM_ID); } catch { }
                    }

                    //find Bladder
                    Structure bladder = ss.Structures.FirstOrDefault(x => x.Id == BLADDER_ID);
                    if (bladder == null)
                    {
                        NOT_FOUND_list.Add(BLADDER_ID);
                        try { bladder = ss.AddStructure("Avoidance", BLADDER_ID); } catch { }
                    }

                    //find Bowel bag
                    Structure bowel_bag = ss.Structures.FirstOrDefault(x => x.Id == BOWEL_BAG_ID);
                    if (bowel_bag == null)
                    {
                        NOT_FOUND_list.Add(BOWEL_BAG_ID);
                        try { bowel_bag = ss.AddStructure("Avoidance", BOWEL_BAG_ID); } catch { }
                    }

                    // find PTV
                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
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

                    VMS.TPS.Script.context.Patient.BeginModifications();   // enable writing with this script.

                    //Check is the optimization structures already exist. If so, delete it from the structure set

                    Structure Bowel_bag_OPT = ss.Structures.FirstOrDefault(x => x.Id == BOWEL_BAG_OPT_ID);
                    if (Bowel_bag_OPT != null)
                    {
                        try { ss.RemoveStructure(Bowel_bag_OPT); } catch { }
                    }

                    Structure Rectum_OPT = ss.Structures.FirstOrDefault(x => x.Id == RECTUM_OPT_ID);
                    if (Rectum_OPT != null)
                    {
                        try { ss.RemoveStructure(Rectum_OPT); } catch { }
                    }

                    Structure Bladder_OPT = ss.Structures.FirstOrDefault(x => x.Id == BLADDER_OPT_ID);
                    if (Bladder_OPT != null)
                    {
                        try { ss.RemoveStructure(Bladder_OPT); } catch { }
                    }

                    //============================
                    // GENERATE 3mm expansion of PTV
                    //============================

                    // create the empty "ptv+3mm" structure
                    Structure ptv_3mm = ss.AddStructure("PTV", EXPANDED_PTV_ID);

                    // expand PTV
                    ptv_3mm.SegmentVolume = ptv.Margin(3.0);

                    //============================
                    // subtract critical structures from expansion to create 3mm buffer
                    //============================
                    Structure buffered_rectum = ss.AddStructure("AVOIDANCE", RECTUM_OPT_ID);
                    Structure buffered_bladder = ss.AddStructure("AVOIDANCE", BLADDER_OPT_ID);
                    Structure buffered_bowel_bag = ss.AddStructure("AVOIDANCE", BOWEL_BAG_OPT_ID);

                    // calculate overlap structures using Boolean operators.

                    if (rectum != null)
                    {
                        try { buffered_rectum.SegmentVolume = rectum.Sub(ptv_3mm); } catch { } //'Sub' subtracts overlapping volume of expanded PTV from rectum 
                    }
                    if (rectum == null)
                    {
                        try { rectum = ss.AddStructure("AVOIDANCE", RECTUM_ID); } catch { }
                    }



                    if (bladder != null)
                    {
                        try { buffered_bladder.SegmentVolume = bladder.Sub(ptv_3mm); } catch { }
                    }
                    if (bladder == null)
                    {
                        try { bladder = ss.AddStructure("AVOIDANCE", BLADDER_ID); } catch { }
                    }

                    if (bowel_bag != null)
                    {
                        try { buffered_bowel_bag.SegmentVolume = bowel_bag.Sub(ptv_3mm); } catch { }
                    }
                    if (bowel_bag == null)
                    {
                        try { bowel_bag = ss.AddStructure("AVOIDANCE", BOWEL_BAG_ID); } catch { }
                    }



                    try
                    {
                        string message = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}",
                                ptv.Id, ptv.Volume, ptv_3mm.Id, ptv_3mm.Volume,
                                bladder.Id, bladder.Volume, buffered_bladder.Id, buffered_bladder.Volume,
                                rectum.Id, rectum.Volume, buffered_rectum.Id, buffered_rectum.Volume,
                                bowel_bag.Id, bowel_bag.Volume, buffered_bowel_bag.Id, buffered_bowel_bag.Volume);
                        MessageBox.Show(message);
                    }
                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error); }
                    ss.RemoveStructure(ptv_3mm);

                }
                if (LungsRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_Lungs";

                    //IDs of PTV structure
                    const string PTV_ID = "PTV";


                    //Due to the inability to add structures one to another (boolean "And" seems to be having malfunction), it is mandatory to create some "trash" structures for the purpose of creating PTV summ, Lungs and Parotids. These structures will be removed at the end of this script

                    const string EXPANDED_PTV_ID = "PTV_3mm";

                    const string BODY_LUNG_L_ID = "BODY_LUNG_L";
                    const string BODY_LUNG_L_R_ID = "BODY_LUNG_L_R";


                    //IDs of critical structures


                    const string ESOPHAGUS_ID = "Esophagus";
                    const string OPT_ESOPHAGUS_ID = "Esophagus-PTV";
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
                    const string LARYNX_ID = "Larynx";
                    const string OPT_LARYNX_ID = "Larynx-PTV";
                    const string BOTH_LUNGS_ID = "Both_Lungs";
                    const string BODY_ID = "BODY";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structures: PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);

                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution
                    List<string> NOT_FOUND_list = new List<string>();

                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try { esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }

                    //find Larynx
                    Structure larynx = ss.Structures.FirstOrDefault(x => x.Id == LARYNX_ID);
                    if (larynx == null)
                    {
                        NOT_FOUND_list.Add(LARYNX_ID);
                        try { larynx = ss.AddStructure("Avoidance", LARYNX_ID); } catch { }
                    }


                    //find Lung_L
                    Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_ID);
                    if (lung_L == null)
                    {
                        NOT_FOUND_list.Add(LUNG_L_ID);
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { }
                    }

                    //find Lung_R
                    Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                    if (lung_R == null)
                    {
                        NOT_FOUND_list.Add(LUNG_R_ID);
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }


                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }


                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try { trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find Heart
                    Structure heart = ss.Structures.FirstOrDefault(x => x.Id == HEART_ID);
                    if (heart == null)
                    {
                        NOT_FOUND_list.Add(HEART_ID);
                        try { heart = ss.AddStructure("Avoidance", HEART_ID); } catch { }
                    }


                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV structure in the structure set. If PTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================




                    //Check if the optimization structures already exist. If so, delete it from the structure set

                    Structure lung_R_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_R_ID);
                    if (lung_R_PTV != null)
                    {
                        try { ss.RemoveStructure(lung_R_PTV); } catch { }
                    }

                    Structure larynx_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LARYNX_ID);
                    if (larynx_PTV != null)
                    {
                        try { ss.RemoveStructure(larynx_PTV); } catch { }
                    }

                    Structure lung_L_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_LUNG_L_ID);
                    if (lung_L_PTV != null)
                    {
                        try { ss.RemoveStructure(lung_L_PTV); } catch { }
                    }

                    Structure lungs_PTV = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_PTV_ID);
                    if (lungs_PTV != null)
                    {
                        try { ss.RemoveStructure(lungs_PTV); } catch { }
                    }

                    Structure esophagus_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_ESOPHAGUS_ID);
                    if (esophagus_PTV != null)
                    {
                        try { ss.RemoveStructure(esophagus_PTV); } catch { }
                    }


                    Structure both_lungs = ss.Structures.FirstOrDefault(x => x.Id == BOTH_LUNGS_ID);
                    if (both_lungs == null)
                    {
                        try { both_lungs = ss.AddStructure("Avoidance", BOTH_LUNGS_ID); } catch { }
                    }

                    Structure lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID);
                    if (lungs != null)
                    {

                        both_lungs.SegmentVolume = lungs.Margin(0);
                        try { ss.RemoveStructure(lungs); } catch { }
                    }

                    Structure spnlcrd_PRV = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spnlcrd_PRV != null)
                    {
                        try { ss.RemoveStructure(spnlcrd_PRV); } catch { }
                    }

                    Structure trachea_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_TRACHEA_ID);
                    if (trachea_PTV != null)
                    {
                        try { ss.RemoveStructure(trachea_PTV); } catch { }
                    }

                    Structure ptv_lungs = ss.Structures.FirstOrDefault(x => x.Id == PTV_LUNGS_ID);
                    if (ptv_lungs != null)
                    {
                        try { ss.RemoveStructure(ptv_lungs); } catch { }
                    }

                    Structure heart_PTV = ss.Structures.FirstOrDefault(x => x.Id == OPT_HEART_ID);
                    if (heart_PTV != null)
                    {
                        try { ss.RemoveStructure(heart_PTV); } catch { }
                    }

                    Structure ptv_3mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_PTV_ID);
                    if (ptv_3mm != null)
                    {
                        try { ss.RemoveStructure(ptv_3mm); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script


                    try { ptv_lungs = ss.AddStructure("Avoidance", PTV_LUNGS_ID); } catch { }
                    try { esophagus_PTV = ss.AddStructure("Avoidance", OPT_ESOPHAGUS_ID); } catch { }
                    try { lung_L_PTV = ss.AddStructure("Avoidance", OPT_LUNG_L_ID); } catch { }
                    try { lung_R_PTV = ss.AddStructure("Avoidance", OPT_LUNG_R_ID); } catch { }
                    try { lungs = ss.AddStructure("Avoidance", LUNGS_ID); } catch { }
                    try { lungs_PTV = ss.AddStructure("Avoidance", LUNGS_PTV_ID); } catch { }
                    try { spnlcrd_PRV = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); } catch { }
                    try { trachea_PTV = ss.AddStructure("Avoidance", OPT_TRACHEA_ID); } catch { }
                    try { heart_PTV = ss.AddStructure("Avoidance", OPT_HEART_ID); } catch { }
                    try { ptv_3mm = ss.AddStructure("Avoidance", EXPANDED_PTV_ID); } catch { }


                    //Create trash structures

                    Structure body_lung_L = ss.AddStructure("Avoidance", BODY_LUNG_L_ID);
                    Structure body_lung_L_R = ss.AddStructure("Avoidance", BODY_LUNG_L_R_ID);



                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define lungs, PTV_Lungs and generate expanded ptv with 3 mm margin
                    if (lung_L != null && lung_R != null)
                    {
                        body_lung_L.SegmentVolume = BODY.Sub(lung_L);
                        body_lung_L_R.SegmentVolume = body_lung_L.Sub(lung_R);
                        lungs.SegmentVolume = BODY.Sub(body_lung_L_R);
                        if (both_lungs.Volume > 0)
                        {
                            lungs.SegmentVolume = both_lungs.Margin(0);
                        }
                    }

                    if (ptv != null && lungs != null)
                    {
                        ptv_lungs.SegmentVolume = ptv.Sub(lungs);
                        ptv_3mm.SegmentVolume = ptv.Margin(3.0);
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================




                    //Define PRVs with 3mm margin. If a structure is empty, the empty PRV will be created



                    if (spinalcord != null)
                    {
                        try { spnlcrd_PRV.SegmentVolume = spinalcord.Margin(5.0); } catch { }
                    }
                    if (spinalcord == null)
                    {
                        try { spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Substract critical structures from expanded PTV_ALL to create 3mm buffer


                    if (esophagus != null)
                    {
                        try { esophagus_PTV.SegmentVolume = esophagus.Sub(ptv_3mm); } catch { }
                    }
                    if (esophagus == null)
                    {
                        try { esophagus_PTV = ss.AddStructure("AVOIDANCE", OPT_ESOPHAGUS_ID); } catch { }
                    }

                    if (larynx != null)
                    {
                        try { larynx_PTV.SegmentVolume = larynx.Sub(ptv_3mm); } catch { }
                    }
                    if (larynx == null)
                    {
                        try { larynx_PTV = ss.AddStructure("AVOIDANCE", OPT_LARYNX_ID); } catch { }
                    }


                    if (trachea != null)
                    {
                        try { trachea_PTV.SegmentVolume = trachea.Sub(ptv_3mm); } catch { }
                    }
                    if (trachea == null)
                    {
                        try { trachea_PTV = ss.AddStructure("AVOIDANCE", OPT_TRACHEA_ID); } catch { }
                    }

                    if (lung_L != null)
                    {
                        try { lung_L_PTV.SegmentVolume = lung_L.Sub(ptv_3mm); } catch { }
                    }
                    if (lung_L == null)
                    {
                        try { lung_L = ss.AddStructure("AVOIDANCE", LUNG_L_ID); } catch { }
                    }

                    if (lung_R != null)
                    {
                        try { lung_R_PTV.SegmentVolume = lung_R.Sub(ptv_3mm); } catch { }
                    }
                    if (lung_R == null)
                    {
                        try { lung_R = ss.AddStructure("AVOIDANCE", LUNG_R_ID); } catch { }
                    }

                    if (lungs != null)
                    {
                        try { lungs_PTV.SegmentVolume = lungs.Sub(ptv_3mm); } catch { }
                    }
                    if (lungs == null)
                    {
                        try { lungs = ss.AddStructure("AVOIDANCE", LUNGS_ID); } catch { }
                    }

                    if (heart != null)
                    {
                        try { heart_PTV.SegmentVolume = heart.Sub(ptv_3mm); } catch { }
                    }
                    if (heart == null)
                    {
                        try { heart = ss.AddStructure("AVOIDANCE", HEART_ID); } catch { }
                    }






                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Check if the summ structures was created. If no, than create empty structures for report generation

                    if (lung_R == null)
                    {
                        try { lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }
                    if (lung_L == null)
                    {
                        try { lung_L = ss.AddStructure("Avoidance", LUNG_L_ID); } catch { }
                    }
                    if (lungs == null)
                    {
                        try { lungs = ss.AddStructure("Avoidance", LUNGS_ID); } catch { }
                    }


                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}\n{32} volume ={33}",

                            ptv.Id, ptv.Volume,

                            ptv_3mm.Id, ptv_3mm.Volume,

                            ptv_lungs.Id, ptv_lungs.Volume,

                            lung_R.Id, lung_R.Volume,

                            lung_R_PTV.Id, lung_R_PTV.Volume,

                            lung_L.Id, lung_L.Volume,

                            lung_L_PTV.Id, lung_L_PTV.Volume,

                            lungs.Id, lungs.Volume,

                            lungs_PTV.Id, lungs_PTV.Volume,

                            esophagus.Id, esophagus.Volume,

                            esophagus_PTV.Id, esophagus_PTV.Volume,

                            trachea.Id, trachea.Volume,

                            trachea_PTV.Id, trachea_PTV.Volume,

                            heart.Id, heart.Volume,

                            heart_PTV.Id, heart_PTV.Volume,

                            spinalcord.Id, spinalcord.Volume,

                            spnlcrd_PRV.Id, spnlcrd_PRV.Volume

                            );


                        //Show the messages

                        MessageBox.Show(message_PTV);
                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }



                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations

                    try { ss.RemoveStructure(ptv_3mm); } catch { MessageBox.Show(string.Format("Unfortunately, some trash structures were not deleted for some reason.\nPlease, delete it manualy"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { ss.RemoveStructure(body_lung_L_R); } catch { MessageBox.Show(string.Format("Unfortunately, some trash structures were not deleted for some reason.\nPlease, delete it manualy"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    try { ss.RemoveStructure(body_lung_L); } catch { MessageBox.Show(string.Format("Unfortunately, some trash structures were not deleted for some reason.\nPlease, delete it manualy"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                
                    
                }
                if (SBRT_ThoraxRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_SBRT_Thorax";

                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_ID = "PTV";
                    const string OPT_PTV_ID = "PTV_OPT";
                    const string GTVp_ID = "GTVp";
                    const string OPT_GTVp_ID = "GTVp_OPT";
                    const string PTV_GTV_ID = "PTV-GTV";
                    const string PTV_SHELL_ID = "PTV_shell";

                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_2MM_PTV_ID = "PTV_2mm";


                    //IDs of critical structures
                    const string BODY_ID = "BODY";
                    const string SKIN_ID = "Skin_bySCRPT";
                    const string CAUDA_EQUINA_ID = "Cauda equina";
                    const string CAUDA_EQUINA_PRV_ID = "Cauda equ_PRV";
                    const string SACRAL_PLEXUS_ID = "Sacral plexus";
                    const string SACRAL_PLEXUS_PRV_ID = "Sacral pl_PRV";
                    const string STOMACH_ID = "Stomach";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpnlCrd_PRV";
                    const string MEDULLA_ID = "Medulla";
                    const string SPINALCORD_SUB_VOLUME_ID = "SpinalCrdSubvol";
                    const string RIB_ID = "Rib";
                    const string DUODENUM_ID = "Duodenum";
                    const string LUNG_R_ID = "Lung_R";
                    const string LUNG_L_Id = "Lung_L";
                    const string LUNGS_ID = "Lungs";
                    const string LUNGS_PTV_ID = "Lungs_PTV";
                    const string LIVER_ID = "Liver";
                    const string BOTH_LUNGS_ID = "Both Lungs";
                    const string HEART_ID = "Heart/prcrdm";
                    const string GREAT_VESSELS_ID = "Great Vessels";
                    const string TRACHEA_ID = "Trachea";
                    const string LARGE_BRONCHUS_ID = "Large Bronchus";
                    const string BRNHS_SMALL_AIR_WAYS_ID = "BrnchsSmrAirWays";
                    const string GTV_EXP_ID = "GTV_exp";
                    const string PTV_exp_ID = "PTV_exp";
                    const string TRACHEA_PTV_ID = "Trachea_PTV";
                    const string TRACHEA_PRV_ID = "Trachea_PRV";
                    const string ESOPHAGUS_ID = "Esophagus";
                    const string ESOPHAGUS_PTV_ID = "Esophagus_PTV";
                    const string ESOPHAGUS_PRV_ID = "Esophagus_PRV";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structure PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology' For implementing this script, please contact Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution                    

                    List<string> NOT_FOUND_list = new List<string>();
                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
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
                        try{sacral_plexus = ss.AddStructure("Avoidance", SACRAL_PLEXUS_ID); } catch { }
                    }

                    //find Stomach
                    Structure stomach = ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID);
                    if (stomach == null)
                    {
                        NOT_FOUND_list.Add(STOMACH_ID);
                        try{stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinal_cord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinal_cord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try{spinal_cord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Medulla
                    Structure medulla = ss.Structures.FirstOrDefault(x => x.Id == MEDULLA_ID);
                    if (medulla == null)
                    {
                        NOT_FOUND_list.Add(MEDULLA_ID);
                        try{medulla = ss.AddStructure("Avoidance", MEDULLA_ID); } catch { }
                    }

                    //find SpinalCrd Subvol
                    Structure spinalcrd_subvol = ss.Structures.FirstOrDefault(x => x.Id == SPINALCORD_SUB_VOLUME_ID);
                    if (spinalcrd_subvol == null)
                    {
                        NOT_FOUND_list.Add(SPINALCORD_SUB_VOLUME_ID);
                        try{spinalcrd_subvol = ss.AddStructure("Avoidance", SPINALCORD_SUB_VOLUME_ID); } catch { }
                    }

                    //find Rib
                    Structure rib = ss.Structures.FirstOrDefault(x => x.Id == RIB_ID);
                    if (rib == null)
                    {
                        NOT_FOUND_list.Add(RIB_ID);
                        try{rib = ss.AddStructure("Avoidance", RIB_ID); } catch { }
                    }

                    //find Duodenum
                    Structure duodenum = ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID);
                    if (duodenum == null)
                    {
                        NOT_FOUND_list.Add(DUODENUM_ID);
                        try{duodenum = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }

                    //find Lung_L
                    Structure lung_L = ss.Structures.FirstOrDefault(x => x.Id == LUNG_L_Id);
                    if (lung_L == null)
                    {
                        NOT_FOUND_list.Add(LUNG_L_Id);
                        try{lung_L = ss.AddStructure("Avoidance", LUNG_L_Id); } catch { }
                    }

                    //find Lung_R
                    Structure lung_R = ss.Structures.FirstOrDefault(x => x.Id == LUNG_R_ID);
                    if (lung_R == null)
                    {
                        NOT_FOUND_list.Add(LUNG_R_ID);
                        try{lung_R = ss.AddStructure("Avoidance", LUNG_R_ID); } catch { }
                    }

                    //find Liver
                    Structure liver = ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID);
                    if (liver == null)
                    {
                        NOT_FOUND_list.Add(LIVER_ID);
                        try{liver = ss.AddStructure("Avoidance", LIVER_ID); } catch { }
                    }

                    //find BothLungs
                    Structure both_lungs = ss.Structures.FirstOrDefault(x => x.Id == BOTH_LUNGS_ID);
                    if (both_lungs == null)
                    {
                        NOT_FOUND_list.Add(BOTH_LUNGS_ID);
                        try{both_lungs = ss.AddStructure("Avoidance", BOTH_LUNGS_ID); } catch { }
                    }

                    //find Heart
                    Structure heart = ss.Structures.FirstOrDefault(x => x.Id == HEART_ID);
                    if (heart == null)
                    {
                        NOT_FOUND_list.Add(HEART_ID);
                        try{heart = ss.AddStructure("Avoidance", HEART_ID); } catch { }
                    }

                    //find GreatVessels
                    Structure great_vessels = ss.Structures.FirstOrDefault(x => x.Id == GREAT_VESSELS_ID);
                    if (great_vessels == null)
                    {
                        NOT_FOUND_list.Add(GREAT_VESSELS_ID);
                        try{great_vessels = ss.AddStructure("Avoidance", GREAT_VESSELS_ID); } catch { }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try{trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find LargeBronchus
                    Structure large_brochus = ss.Structures.FirstOrDefault(x => x.Id == LARGE_BRONCHUS_ID);
                    if (large_brochus == null)
                    {
                        NOT_FOUND_list.Add(LARGE_BRONCHUS_ID);
                        try{large_brochus = ss.AddStructure("Avoidance", LARGE_BRONCHUS_ID); } catch { }
                    }

                    //find BrnchsSmrAirWays
                    Structure brnchssmrairways = ss.Structures.FirstOrDefault(x => x.Id == BRNHS_SMALL_AIR_WAYS_ID);
                    if (brnchssmrairways == null)
                    {
                        NOT_FOUND_list.Add(BRNHS_SMALL_AIR_WAYS_ID);
                        try{brnchssmrairways = ss.AddStructure("Avoidance", BRNHS_SMALL_AIR_WAYS_ID); } catch { }
                    }


                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try{esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }

                    //show message with list of not found structures
                    string[] NOT_FOUND = NOT_FOUND_list.ToArray();
                    string res = string.Join("\r\n", NOT_FOUND);
                    var array_length = NOT_FOUND.Length;
                    if (array_length > 0)
                    {
                        MessageBox.Show(string.Format("Structures were not found:\n'{0}'", res), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                        MessageBox.Show(string.Format("'{0}' not found or empty!", GTVp_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        gtv = ss.AddStructure("Avoidance", GTVp_ID);
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Check if the optimization structures already exist. If so, delete it from the structure set

                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);
                    Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTVp_ID);
                    Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);
                    Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                    Structure expanded_PTV_2mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_2MM_PTV_ID);
                    Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                    Structure cauda_equina_prv = ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_PRV_ID);
                    Structure sacral_plexus_prv = ss.Structures.FirstOrDefault(x => x.Id == SACRAL_PLEXUS_PRV_ID);
                    Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    Structure lungs_ptv = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_PTV_ID);
                    Structure lungs = ss.Structures.FirstOrDefault(x => x.Id == LUNGS_ID);
                    Structure trachea_PTV = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_PTV_ID);
                    Structure trachea_PRV = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_PRV_ID);
                    Structure esophagus_PTV = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_PTV_ID);
                    Structure esophagus_PRV = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_PRV_ID);
                    Structure gtv_expanded = ss.Structures.FirstOrDefault(x => x.Id == GTV_EXP_ID);
                    Structure ptv_1mm = ss.Structures.FirstOrDefault(x => x.Id == PTV_exp_ID);



                    Structure[] TEC_STR = {ptv_opt, gtv_opt, ptv_shell, ptv_gtv, expanded_PTV_2mm, skin, cauda_equina_prv, sacral_plexus_prv, spinal_cord_prv,
                                    lungs_ptv, lungs, gtv_expanded, ptv_1mm, trachea_PTV, esophagus_PTV};

                    foreach (Structure s in TEC_STR)
                    {
                        if (s != null)
                        {
                            try{ss.RemoveStructure(s); } catch { }
                        }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create the empty structures requered for the script
                    try{expanded_PTV_2mm = ss.AddStructure("Avoidance", EXPANDED_2MM_PTV_ID); } catch { }
                    try
                    {ptv_gtv = ss.AddStructure("Avoidance", PTV_GTV_ID); }
                    catch { }
                    try
                    {ptv_opt = ss.AddStructure("Avoidance", OPT_PTV_ID); }
                    catch { }
                    try
                    {gtv_opt = ss.AddStructure("Avoidance", OPT_GTVp_ID); }
                    catch { }
                    try
                    {skin = ss.AddStructure("Avoidance", SKIN_ID); }
                    catch { }
                    try
                    {ptv_shell = ss.AddStructure("Avoidance", PTV_SHELL_ID); }
                    catch { }
                    try
                    {lungs_ptv = ss.AddStructure("Avoidance", LUNGS_PTV_ID); }
                    catch { }
                    try
                    {lungs = ss.AddStructure("Avoidance", LUNGS_ID); }
                    catch { }
                    try
                    {gtv_expanded = ss.AddStructure("Avoidance", GTV_EXP_ID); }
                    catch { }
                    try
                    {ptv_1mm = ss.AddStructure("Avoidance", "PTV_1mm"); }
                    catch { }


                    //Check for high resolution

                    if (ptv != null && gtv != null)
                    {

                        if (ptv.IsHighResolution || gtv.IsHighResolution)
                        {
                            MessageBox.Show(string.Format("Detected target structure is in high resolution! Critical structures will be converted to high resolution to prevent malfunction. Please, review contours and launch this script again!"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            foreach (Structure s in ss.Structures)
                            {
                                if (s.IsHighResolution == false && s.Id != "BODY" && s.DicomType != "Support" && s.DicomType != "FIXATION" && s.DicomType != "PTV" && s.DicomType != "GTV" && s.Id != PTV_ID && s.Id != GTVp_ID && s.DicomType != "EXTERNAL" && s.Id != "CouchInterior" && s.Id != "CouchSurface")
                                {
                                    try
                                    {
                                        try{s.ConvertToHighResolution(); } catch { }
                                    }
                                    catch { MessageBox.Show(string.Format("'{0}' could not be converted to high resolution", s.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                                }

                            }



                        }

                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 2mm margin. If a structure is empty, the empty PRV will be created
                    Structure[] PRVs = { spinal_cord, sacral_plexus, cauda_equina, trachea, esophagus };

                    foreach (Structure s in PRVs)
                    {
                        if (s != null)
                        {
                            string short_id = s.Id;
                            if (s.Id.Length > 9)

                            {
                                string long_id = s.Id;
                                short_id = long_id.Remove(9);
                            }

                            string PRV_ID = string.Format("{0}_PRV", short_id);
                            Structure prv = ss.Structures.FirstOrDefault(x => x.Id == PRV_ID);
                            if (prv != null)
                            {
                                try{ss.RemoveStructure(prv); } catch { }
                            }
                            prv = ss.AddStructure("Avoidance", PRV_ID);
                            if (ptv.IsHighResolution || gtv.IsHighResolution)
                            {
                                try{prv.ConvertToHighResolution(); } catch { }
                            }
                            prv.SegmentVolume = s.Margin(3);
                        }


                    }

                    //Define PTV-GTV, PTV_OPT and GTV_OPT structures

                    gtv_expanded.SegmentVolume = gtv.Margin(1);
                    if (gtv.Volume > 0.01)
                    {
                        ptv_gtv.SegmentVolume = ptv.Sub(gtv_expanded);
                    }
                    if (gtv.Volume < 0.01)
                    {
                        ptv_gtv.SegmentVolume = ptv.Margin(0);
                    }

                    Structure[] OPT = { spinal_cord_prv, cauda_equina_prv, sacral_plexus_prv, trachea_PRV, esophagus_PRV };
                    ptv_opt.SegmentVolume = ptv.Margin(0);
                    foreach (Structure s in OPT)
                    {
                        try
                        {
                            if (s != null)
                            {
                                if (s.Volume > 0.01)
                                {
                                    try{ptv_opt.SegmentVolume = ptv_opt.Sub(s); } catch { }
                                }
                            }
                        }
                        catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", s.Id), SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }


                    }
                    gtv_opt.SegmentVolume = gtv.Margin(0);

                    foreach (Structure s in OPT)
                    {
                        try
                        {
                            if (s != null)
                            {
                                if (s.Volume > 0.01)
                                {
                                    try{gtv_opt.SegmentVolume = gtv_opt.Sub(s); } catch { }
                                }
                            }

                        }

                        catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }

                    }


                    //Define PTV_shell structure


                    ptv_1mm.SegmentVolume = ptv.Margin(-1);
                    expanded_PTV_2mm.SegmentVolume = ptv.Margin(2);
                    ptv_shell.SegmentVolume = expanded_PTV_2mm.Sub(ptv_1mm);


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create Lungs structure and Lungs-PTV structure
                    if (both_lungs == null & both_lungs.Volume <= 0.01)
                    {
                        Structure body_lung_L = ss.AddStructure("Avoidance", "B_lung_L");
                        Structure body_lung_L_R = ss.AddStructure("Avoidance", "B_lung_L_R");
                        body_lung_L.SegmentVolume = BODY.Sub(lung_L);
                        body_lung_L_R.SegmentVolume = body_lung_L.Sub(lung_R);
                        if (ptv.IsHighResolution || gtv.IsHighResolution)
                        {
                            body_lung_L.ConvertToHighResolution();
                            body_lung_L_R.ConvertToHighResolution();
                        }
                        lungs_ptv.SegmentVolume = BODY.Sub(lungs);
                        ss.RemoveStructure(body_lung_L);
                        ss.RemoveStructure(body_lung_L_R);
                    }
                    if (both_lungs != null & both_lungs.Volume > 0.01)
                    {
                        lungs.SegmentVolume = both_lungs.Margin(0);
                        lungs_ptv.SegmentVolume = lungs.Sub(expanded_PTV_2mm);

                    }
                    //Create Skin structure
                    Structure body_4mm = ss.AddStructure("Avoidance", "BODY-4MM");
                    body_4mm.SegmentVolume = BODY.Margin(-4);
                    Structure body_HR = ss.Structures.FirstOrDefault(x => x.Id == "BODY_HR");
                    if (body_HR != null)
                    {
                        ss.RemoveStructure(body_HR);
                    }
                    body_HR = ss.AddStructure("Avoidance", "BODY_HR");
                    body_HR.SegmentVolume = BODY.Margin(0);
                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        body_4mm.ConvertToHighResolution();
                        body_HR.ConvertToHighResolution();
                    }
                    skin.SegmentVolume = body_HR.Sub(body_4mm);
                    ss.RemoveStructure(body_HR);



                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}",

                            ptv.Id, ptv.Volume,

                            ptv_opt.Id, ptv_opt.Volume,

                            gtv.Id, gtv.Volume,

                            gtv_opt.Id, gtv_opt.Volume,

                            ptv_gtv.Id, ptv_gtv.Volume


                            );








                        //Show the messages

                        MessageBox.Show(message_PTV);
                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); }



                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations
                    ss.RemoveStructure(gtv_expanded);
                    ss.RemoveStructure(ptv_1mm);
                    ss.RemoveStructure(expanded_PTV_2mm);
                    ss.RemoveStructure(body_4mm);
                }
                if (SBRT_PelvisRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_SBRT_Pelvis";

                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_ID = "PTV";
                    const string OPT_PTV_ID = "PTV_OPT";
                    const string GTVp_ID = "GTVp";
                    const string OPT_GTVp_ID = "GTVp_OPT";
                    const string PTV_GTV_ID = "PTV-GTV";
                    const string PTV_SHELL_ID = "PTV_shell";

                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_2MM_PTV_ID = "PTV_2mm";


                    //IDs of critical structures
                    const string BODY_ID = "BODY";
                    const string SKIN_ID = "Skin_bySCRPT";
                    const string COLON_ID = "Colon";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpnlCrd_PRV";
                    const string CAUDA_EQUINA_ID = "Cauda equina";
                    const string CAUDA_EQUINA_PRV_ID = "Cd_qn_PRV";
                    const string SACRAL_PLEXUS_ID = "Sacral plexus";
                    const string SACRAL_PLEXUS_PRV_ID = "Slplxs_PRV";
                    const string STOMACH_ID = "Stomach";
                    const string BLADDER_WALL_ID = "Bladder Wall";
                    const string RECTUM_ID = "Rectum";
                    const string RIB_ID = "Rib";
                    const string DUODENUM_ID = "Duodenum";
                    const string JEJUNUM_ID = "Jejunum";
                    const string RENAL_HILUM_ID = "Renal Hilum";
                    const string VASCULAR_TRUNK_ID = "Vascular Trunk";
                    const string BOTH_LUNGS_ID = "Both Lungs";
                    const string LIVER_ID = "Liver";
                    const string RENAL_CORTEX_ID = "Renal Cortex";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structure PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'.\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);

                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution                    
                    List<string> NOT_FOUND_list = new List<string>();

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try{spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Colon
                    Structure colon = ss.Structures.FirstOrDefault(x => x.Id == COLON_ID);
                    if (colon == null)
                    {
                        NOT_FOUND_list.Add(COLON_ID);
                        try{colon = ss.AddStructure("Avoidance", COLON_ID); } catch { }
                    }

                    //find Cauda Equina
                    Structure cauda_equina = ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_ID);
                    if (cauda_equina == null)
                    {
                        NOT_FOUND_list.Add(CAUDA_EQUINA_ID);
                        try{cauda_equina = ss.AddStructure("Avoidance", CAUDA_EQUINA_ID); } catch { }
                    }
                    //find SacralPlexus
                    Structure sacral_plexus = ss.Structures.FirstOrDefault(x => x.Id == SACRAL_PLEXUS_ID);
                    if (sacral_plexus == null)
                    {
                        NOT_FOUND_list.Add(SACRAL_PLEXUS_ID);
                        try{sacral_plexus = ss.AddStructure("Avoidance", SACRAL_PLEXUS_ID); } catch { }
                    }

                    //find Stomach
                    Structure stomach = ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID);
                    if (stomach == null)
                    {
                        NOT_FOUND_list.Add(STOMACH_ID);
                        try{stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                    }

                    //find BladderWall
                    Structure bladder_wall = ss.Structures.FirstOrDefault(x => x.Id == BLADDER_WALL_ID);
                    if (bladder_wall == null)
                    {
                        NOT_FOUND_list.Add(BLADDER_WALL_ID);
                        try{bladder_wall = ss.AddStructure("Avoidance", BLADDER_WALL_ID); } catch { }
                    }

                    //find Rectum
                    Structure rectum = ss.Structures.FirstOrDefault(x => x.Id == RECTUM_ID);
                    if (rectum == null)
                    {
                        NOT_FOUND_list.Add(RECTUM_ID);
                        try{rectum = ss.AddStructure("Avoidance", RECTUM_ID); } catch { }
                    }

                    //find Rib
                    Structure rib = ss.Structures.FirstOrDefault(x => x.Id == RIB_ID);
                    if (rib == null)
                    {
                        NOT_FOUND_list.Add(RIB_ID);
                        try{rib = ss.AddStructure("Avoidance", RIB_ID); } catch { }
                    }

                    //find Duodenum
                    Structure duodenum = ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID);
                    if (duodenum == null)
                    {
                        NOT_FOUND_list.Add(DUODENUM_ID);
                        try{duodenum = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }

                    //find Jejunum
                    Structure jejunum = ss.Structures.FirstOrDefault(x => x.Id == JEJUNUM_ID);
                    if (jejunum == null)
                    {
                        NOT_FOUND_list.Add(JEJUNUM_ID);
                        try{jejunum = ss.AddStructure("Avoidance", JEJUNUM_ID); } catch { }
                    }

                    //find Renal Hilum
                    Structure renal_hilum = ss.Structures.FirstOrDefault(x => x.Id == RENAL_HILUM_ID);
                    if (renal_hilum == null)
                    {
                        NOT_FOUND_list.Add(RENAL_HILUM_ID);
                        try
                        {
                            renal_hilum = ss.AddStructure("Avoidance", RENAL_HILUM_ID);
                        }
                        catch { }
                    }

                    //find Vascular Trunk
                    Structure vascular_trunk = ss.Structures.FirstOrDefault(x => x.Id == VASCULAR_TRUNK_ID);
                    if (vascular_trunk == null)
                    {
                        NOT_FOUND_list.Add(VASCULAR_TRUNK_ID);
                            try
                            {
                                vascular_trunk = ss.AddStructure("Avoidance", VASCULAR_TRUNK_ID);
                        }
                        catch { }
                    }

                    //find Both_lungs
                    Structure both_lungs = ss.Structures.FirstOrDefault(x => x.Id == BOTH_LUNGS_ID);
                    if (both_lungs == null)
                    {
                        NOT_FOUND_list.Add(BOTH_LUNGS_ID);
                        try{both_lungs = ss.AddStructure("Avoidance", BOTH_LUNGS_ID); } catch { }
                    }

                    //find Liver
                    Structure liver = ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID);
                    if (liver == null)
                    {
                        NOT_FOUND_list.Add(LIVER_ID);
                        try{liver = ss.AddStructure("Avoidance", LIVER_ID); } catch { }
                    }

                    //find Renal Cortex
                    Structure renal_cortex = ss.Structures.FirstOrDefault(x => x.Id == RENAL_CORTEX_ID);
                    if (renal_cortex == null)
                    {
                        NOT_FOUND_list.Add(RENAL_CORTEX_ID);
                        try{renal_cortex = ss.AddStructure("Avoidance", RENAL_CORTEX_ID); } catch { }
                    }
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV and GTV structures in the structure set. If PTV and GTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    Structure gtv = ss.Structures.FirstOrDefault(x => x.Id == GTVp_ID);
                    if (gtv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", GTVp_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Check if the optimization structures already exist. If so, delete it from the structure set

                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);

                    if (ptv_opt != null)
                    {
                        try{ss.RemoveStructure(ptv_opt); } catch { }
                    }

                    Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTVp_ID);

                    if (gtv_opt != null)
                    {
                        try{ss.RemoveStructure(gtv_opt); } catch { }
                    }

                    Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);

                    if (ptv_shell != null)
                    {
                        try{ss.RemoveStructure(ptv_shell); } catch { }
                    }

                    Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                    if (ptv_gtv != null)
                    {
                        try{ss.RemoveStructure(ptv_gtv); } catch { }
                    }

                    Structure expanded_PTV_2mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_2MM_PTV_ID);
                    if (expanded_PTV_2mm != null)
                    {
                        try{ss.RemoveStructure(expanded_PTV_2mm); } catch { }
                    }

                    Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                    if (skin != null)
                    {
                        try{ss.RemoveStructure(skin); } catch { }
                    }

                    Structure cauda_equina_prv = ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_PRV_ID);
                    if (cauda_equina_prv != null)
                    {
                        try{ss.RemoveStructure(cauda_equina_prv); } catch { }
                    }

                    Structure sacral_plexus_prv = ss.Structures.FirstOrDefault(x => x.Id == SACRAL_PLEXUS_PRV_ID);
                    if (sacral_plexus_prv != null)
                    {
                        try{ss.RemoveStructure(sacral_plexus_prv); } catch { }
                    }

                    Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spinal_cord_prv != null)
                    {
                        try { ss.RemoveStructure(spinal_cord_prv); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create the empty structures requered for the script
                    try { expanded_PTV_2mm = ss.AddStructure("Avoidance", EXPANDED_2MM_PTV_ID); } catch { }
                    try{ptv_gtv = ss.AddStructure("Avoidance", PTV_GTV_ID); } catch { }
                    try{ptv_opt = ss.AddStructure("Avoidance", OPT_PTV_ID); } catch { }
                    try{gtv_opt = ss.AddStructure("Avoidance", OPT_GTVp_ID); } catch { }
                    try
                    {skin = ss.AddStructure("Avoidance", SKIN_ID); }
                    catch { }
                    try
                    {ptv_shell = ss.AddStructure("Avoidance", PTV_SHELL_ID); }
                    catch { }
                    try
                    {cauda_equina_prv = ss.AddStructure("Avoidance", CAUDA_EQUINA_PRV_ID); }
                    catch { }
                    try
                    {sacral_plexus_prv = ss.AddStructure("Avoidance", SACRAL_PLEXUS_PRV_ID); }
                    catch { }
                    try
                    {spinal_cord_prv = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); }
                    catch { }


                    //Check for high resolution

                    if (ptv != null && gtv != null)
                    {

                        if (ptv.IsHighResolution || gtv.IsHighResolution)
                        {
                            MessageBox.Show(string.Format("Detected target structure is in high resolution! Critical structures will be converted to high resolution to prevent malfunction. Please, review contours and launch this script again!"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            foreach (Structure s in ss.Structures)
                            {
                                if (s.IsHighResolution == false && s.DicomType != "BODY" && s.DicomType != "Support" && s.DicomType != "FIXATION" && s.Id != PTV_ID && s.Id != GTVp_ID && s.DicomType != "EXTERNAL" && s.Id != "CouchInterior" && s.Id != "CouchSurface")
                                {

                                try { s.ConvertToHighResolution(); } catch { }
                            }

                            }



                        }

                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 2mm margin. If a structure is empty, the empty PRV will be created

                    if (spinalcord != null)
                    {
                        spinal_cord_prv.SegmentVolume = spinalcord.Margin(2.0);
                    }
                    if (spinalcord == null)
                    {
                        try{spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    if (sacral_plexus != null)
                    {
                        try{sacral_plexus_prv.SegmentVolume = sacral_plexus.Margin(2.0); } catch { }
                    }
                    if (sacral_plexus == null)
                    {
                        try{sacral_plexus = ss.AddStructure("Avoidance", SACRAL_PLEXUS_ID); } catch { }
                    }

                    if (cauda_equina != null)
                    {
                        try{cauda_equina_prv.SegmentVolume = cauda_equina.Margin(2.0); } catch { }
                    }
                    if (cauda_equina == null)
                    {
                        try{cauda_equina = ss.AddStructure("Avoidance", CAUDA_EQUINA_ID); } catch { }
                    }


                    //Define PTV-GTV, PTV_OPT and GTV_OPT structures
                    Structure gtv_expanded = ss.AddStructure("Avoidance", "GTV_exp");
                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        try{gtv_expanded.ConvertToHighResolution(); } catch { }
                    }
                    gtv_expanded.SegmentVolume = gtv.Margin(1);

                    ptv_gtv.SegmentVolume = ptv.Sub(gtv_expanded);
                    try { ptv_opt.SegmentVolume = ptv.Sub(spinal_cord_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", spinal_cord_prv.Id)); } catch { } }
                    try { ptv_opt.SegmentVolume = ptv_opt.Sub(cauda_equina_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", cauda_equina_prv.Id)); } catch { } }
                    try { ptv_opt.SegmentVolume = ptv_opt.Sub(sacral_plexus_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", sacral_plexus_prv.Id)); } catch { } }
                    try { ptv_opt.SegmentVolume = ptv_opt.Sub(gtv_expanded); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", gtv_expanded.Id)); } catch { } }
                    try { gtv_opt.SegmentVolume = gtv.Sub(spinal_cord_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", spinal_cord_prv.Id)); } catch { } }
                    try { gtv_opt.SegmentVolume = gtv_opt.Sub(cauda_equina_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", cauda_equina_prv.Id)); } catch { } }
                    try { gtv_opt.SegmentVolume = gtv_opt.Sub(sacral_plexus_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", sacral_plexus_prv.Id)); } catch { } }








                    //Define PTV_shell structure
                    Structure ptv_1mm = ss.AddStructure("Avoidance", "PTV_1mm");
                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        ptv_1mm.ConvertToHighResolution();

                    }
                    ptv_1mm.SegmentVolume = ptv.Margin(-1);
                    expanded_PTV_2mm.SegmentVolume = ptv.Margin(3);
                    ptv_shell.SegmentVolume = expanded_PTV_2mm.Sub(ptv);


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create Skin structure
                    Structure body_4mm = ss.AddStructure("Avoidance", "BODY-4MM");
                    body_4mm.SegmentVolume = BODY.Margin(-4);
                    skin.SegmentVolume = BODY.Sub(body_4mm);

                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        skin.ConvertToHighResolution();
                    }


                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}",

                            ptv.Id, ptv.Volume,

                            ptv_opt.Id, ptv_opt.Volume,

                            gtv.Id, gtv.Volume,

                            gtv_opt.Id, gtv_opt.Volume,

                            ptv_gtv.Id, ptv_gtv.Volume


                            );

                        string message_OAR = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}",

                            colon.Id, colon.Volume,

                            cauda_equina.Id, cauda_equina.Volume,

                            cauda_equina_prv.Id, cauda_equina_prv.Volume,

                            sacral_plexus.Id, sacral_plexus.Volume,

                            sacral_plexus_prv.Id, sacral_plexus_prv.Volume,

                            stomach.Id, stomach.Volume,

                            bladder_wall.Id, bladder_wall.Volume,

                            spinalcord.Id, spinalcord.Volume,

                            rib.Id, rib.Volume,

                            spinal_cord_prv.Id, spinal_cord_prv.Volume,

                            duodenum.Id, duodenum.Volume,

                            jejunum.Id, jejunum.Volume,

                            renal_cortex.Id, renal_cortex.Volume,

                            renal_hilum.Id, renal_hilum.Volume,

                            liver.Id, liver.Volume,

                            both_lungs.Id, both_lungs.Volume

                            );




                        //Show the messages

                        MessageBox.Show(message_PTV);
                        MessageBox.Show(message_OAR);
                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations

                    ss.RemoveStructure(expanded_PTV_2mm);
                    ss.RemoveStructure(body_4mm);
                    ss.RemoveStructure(ptv_1mm);
                    ss.RemoveStructure(gtv_expanded);
                }
                if (SBRT_AbdominalRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_SBRT_Abdominal";

                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_ID = "PTV";
                    const string OPT_PTV_ID = "PTV_OPT";
                    const string GTVp_ID = "GTVp";
                    const string OPT_GTVp_ID = "GTVp_OPT";
                    const string PTV_GTV_ID = "PTV-GTV";
                    const string PTV_SHELL_ID = "PTV_shell";

                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_2MM_PTV_ID = "PTV_2mm";


                    //IDs of critical structures
                    const string BODY_ID = "BODY";
                    const string SKIN_ID = "Skin_bySCRPT";
                    const string COLON_ID = "Colon";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpnlCrd_PRV";
                    const string CAUDA_EQUINA_ID = "Cauda equina";
                    const string CAUDA_EQUINA_PRV_ID = "Cd_qn_PRV";
                    const string SACRAL_PLEXUS_ID = "Sacral plexus";
                    const string SACRAL_PLEXUS_PRV_ID = "Slplxs_PRV";
                    const string STOMACH_ID = "Stomach";
                    const string BLADDER_WALL_ID = "Bladder Wall";
                    const string RECTUM_ID = "Rectum";
                    const string RIB_ID = "Rib";
                    const string DUODENUM_ID = "Duodenum";
                    const string ILEUM_ID = "Ileum";
                    const string JEJUNUM_ID = "Jejunum";
                    const string RENAL_HILUM_ID = "Renal Hilum";
                    const string VASCULAR_TRUNK_ID = "Vascular Trunk";
                    const string BOTH_LUNGS_ID = "Both Lungs";
                    const string LIVER_ID = "Liver";
                    const string RENAL_CORTEX_ID = "Renal Cortex";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structure PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'.\n Before implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);

                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution                    
                    List<string> NOT_FOUND_list = new List<string>();

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", BODY_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    //find Ileum
                    Structure ileum = ss.Structures.FirstOrDefault(x => x.Id == ILEUM_ID);
                    if (ileum == null) 
                    {
                        NOT_FOUND_list.Add(ILEUM_ID);
                        try { ileum = ss.AddStructure("Avoidance", ILEUM_ID); } catch { }
                    }
                       
                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try{spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Colon
                    Structure colon = ss.Structures.FirstOrDefault(x => x.Id == COLON_ID);
                    if (colon == null)
                    {
                        NOT_FOUND_list.Add(COLON_ID);
                        try{colon = ss.AddStructure("Avoidance", COLON_ID); } catch { }
                    }

                    //find Cauda Equina
                    Structure cauda_equina = ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_ID);
                    if (cauda_equina == null)
                    {
                        NOT_FOUND_list.Add(CAUDA_EQUINA_ID);
                        try{cauda_equina = ss.AddStructure("Avoidance", CAUDA_EQUINA_ID); } catch { }
                    }
                    //find SacralPlexus
                    Structure sacral_plexus = ss.Structures.FirstOrDefault(x => x.Id == SACRAL_PLEXUS_ID);
                    if (sacral_plexus == null)
                    {
                        NOT_FOUND_list.Add(SACRAL_PLEXUS_ID);
                        try{sacral_plexus = ss.AddStructure("Avoidance", SACRAL_PLEXUS_ID); } catch { }
                    }

                    //find Stomach
                    Structure stomach = ss.Structures.FirstOrDefault(x => x.Id == STOMACH_ID);
                    if (stomach == null)
                    {
                        NOT_FOUND_list.Add(STOMACH_ID);
                        try{stomach = ss.AddStructure("Avoidance", STOMACH_ID); } catch { }
                    }

                    //find BladderWall
                    Structure bladder_wall = ss.Structures.FirstOrDefault(x => x.Id == BLADDER_WALL_ID);
                    if (bladder_wall == null)
                    {
                        NOT_FOUND_list.Add(BLADDER_WALL_ID);
                        try{bladder_wall = ss.AddStructure("Avoidance", BLADDER_WALL_ID); } catch { }
                    }

                    //find Rectum
                    Structure rectum = ss.Structures.FirstOrDefault(x => x.Id == RECTUM_ID);
                    if (rectum == null)
                    {
                        NOT_FOUND_list.Add(RECTUM_ID);
                        try{rectum = ss.AddStructure("Avoidance", RECTUM_ID); } catch { }
                    }

                    //find Rib
                    Structure rib = ss.Structures.FirstOrDefault(x => x.Id == RIB_ID);
                    if (rib == null)
                    {
                        NOT_FOUND_list.Add(RIB_ID);
                        try{rib = ss.AddStructure("Avoidance", RIB_ID); } catch { }
                    }

                    //find Duodenum
                    Structure duodenum = ss.Structures.FirstOrDefault(x => x.Id == DUODENUM_ID);
                    if (duodenum == null)
                    {
                        NOT_FOUND_list.Add(DUODENUM_ID);
                        try{duodenum = ss.AddStructure("Avoidance", DUODENUM_ID); } catch { }
                    }

                    //find Jejunum
                    Structure jejunum = ss.Structures.FirstOrDefault(x => x.Id == JEJUNUM_ID);
                    if (jejunum == null)
                    {
                        NOT_FOUND_list.Add(JEJUNUM_ID);
                        try{jejunum = ss.AddStructure("Avoidance", JEJUNUM_ID); } catch { }
                    }

                    //find Renal Hilum
                    Structure renal_hilum = ss.Structures.FirstOrDefault(x => x.Id == RENAL_HILUM_ID);
                    if (renal_hilum == null)
                    {
                        NOT_FOUND_list.Add(RENAL_HILUM_ID);
                        try{renal_hilum = ss.AddStructure("Avoidance", RENAL_HILUM_ID); } catch { }
                    }

                    //find Vascular Trunk
                    Structure vascular_trunk = ss.Structures.FirstOrDefault(x => x.Id == VASCULAR_TRUNK_ID);
                    if (vascular_trunk == null)
                    {
                        NOT_FOUND_list.Add(VASCULAR_TRUNK_ID);
                        try{vascular_trunk = ss.AddStructure("Avoidance", VASCULAR_TRUNK_ID); } catch { }
                    }

                    //find Both_lungs
                    Structure both_lungs = ss.Structures.FirstOrDefault(x => x.Id == BOTH_LUNGS_ID);
                    if (both_lungs == null)
                    {
                        NOT_FOUND_list.Add(BOTH_LUNGS_ID);
                        try{both_lungs = ss.AddStructure("Avoidance", BOTH_LUNGS_ID); } catch { }
                    }

                    //find Liver
                    Structure liver = ss.Structures.FirstOrDefault(x => x.Id == LIVER_ID);
                    if (liver == null)
                    {
                        NOT_FOUND_list.Add(LIVER_ID);
                        try{liver = ss.AddStructure("Avoidance", LIVER_ID); } catch { }
                    }

                    //find Renal Cortex
                    Structure renal_cortex = ss.Structures.FirstOrDefault(x => x.Id == RENAL_CORTEX_ID);
                    if (renal_cortex == null)
                    {
                        NOT_FOUND_list.Add(RENAL_CORTEX_ID);
                        try{renal_cortex = ss.AddStructure("Avoidance", RENAL_CORTEX_ID); } catch { }
                    }
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV and GTV structures in the structure set. If PTV and GTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    Structure gtv = ss.Structures.FirstOrDefault(x => x.Id == GTVp_ID);
                    if (gtv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", GTVp_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Check if the optimization structures already exist. If so, delete it from the structure set

                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);

                    if (ptv_opt != null)
                    {
                        try{ss.RemoveStructure(ptv_opt); } catch { }
                    }

                    Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTVp_ID);

                    if (gtv_opt != null)
                    {
                        try{ss.RemoveStructure(gtv_opt); } catch { }
                    }

                    Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);

                    if (ptv_shell != null)
                    {
                        try{ss.RemoveStructure(ptv_shell); } catch { }
                    }

                    Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                    if (ptv_gtv != null)
                    {
                        try{ss.RemoveStructure(ptv_gtv); } catch { }
                    }

                    Structure expanded_PTV_2mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_2MM_PTV_ID);
                    if (expanded_PTV_2mm != null)
                    {
                        try{ss.RemoveStructure(expanded_PTV_2mm); } catch { }
                    }

                    Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                    if (skin != null)
                    {
                        try{ss.RemoveStructure(skin); } catch { }
                    }

                    Structure cauda_equina_prv = ss.Structures.FirstOrDefault(x => x.Id == CAUDA_EQUINA_PRV_ID);
                    if (cauda_equina_prv != null)
                    {
                        try{ss.RemoveStructure(cauda_equina_prv); } catch { }
                    }

                    Structure sacral_plexus_prv = ss.Structures.FirstOrDefault(x => x.Id == SACRAL_PLEXUS_PRV_ID);
                    if (sacral_plexus_prv != null)
                    {
                        try{ss.RemoveStructure(sacral_plexus_prv); } catch { }
                    }

                    Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    if (spinal_cord_prv != null)
                    {
                        try{ss.RemoveStructure(spinal_cord_prv); } catch { }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create the empty structures requered for the script
                    try{expanded_PTV_2mm = ss.AddStructure("Avoidance", EXPANDED_2MM_PTV_ID); } catch { }
                    try
                    {ptv_gtv = ss.AddStructure("Avoidance", PTV_GTV_ID); }
                    catch { }
                    try
                    {ptv_opt = ss.AddStructure("Avoidance", OPT_PTV_ID); }
                    catch { }
                    try
                    {gtv_opt = ss.AddStructure("Avoidance", OPT_GTVp_ID); }
                    catch { }
                    try
                    {skin = ss.AddStructure("Avoidance", SKIN_ID); }
                    catch { }
                    try
                    {ptv_shell = ss.AddStructure("Avoidance", PTV_SHELL_ID); }
                    catch { }
                    try
                    {cauda_equina_prv = ss.AddStructure("Avoidance", CAUDA_EQUINA_PRV_ID); }
                    catch { }
                    try
                    {sacral_plexus_prv = ss.AddStructure("Avoidance", SACRAL_PLEXUS_PRV_ID); }
                    catch { }
                    try
                    {spinal_cord_prv = ss.AddStructure("Avoidance", SPINAL_CORD_PRV_ID); }
                    catch { }


                    //Check for high resolution

                    if (ptv != null && gtv != null)
                    {

                        if (ptv.IsHighResolution || gtv.IsHighResolution)
                        {
                            MessageBox.Show(string.Format("Detected target structure is in high resolution! Critical structures will be converted to high resolution to prevent malfunction. Please, review contours and launch this script again!"), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            foreach (Structure s in ss.Structures)
                            {
                                if (s.IsHighResolution == false && s.DicomType != "BODY" && s.DicomType != "Support" && s.DicomType != "FIXATION" && s.Id != PTV_ID && s.Id != GTVp_ID && s.DicomType != "EXTERNAL" && s.Id != "CouchInterior" && s.Id != "CouchSurface")
                                {

                                    try{s.ConvertToHighResolution(); } catch { }
                                }

                            }



                        }

                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 2mm margin. If a structure is empty, the empty PRV will be created

                    if (spinalcord != null)
                    {
                        spinal_cord_prv.SegmentVolume = spinalcord.Margin(2.0);
                    }
                    if (spinalcord == null)
                    {
                        try{spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    if (sacral_plexus != null)
                    {
                        try{sacral_plexus_prv.SegmentVolume = sacral_plexus.Margin(2.0); } catch { }
                    }
                    if (sacral_plexus == null)
                    {
                        try{sacral_plexus = ss.AddStructure("Avoidance", SACRAL_PLEXUS_ID); } catch { }
                    }

                    if (cauda_equina != null)
                    {
                        try{cauda_equina_prv.SegmentVolume = cauda_equina.Margin(2.0); } catch { }
                    }
                    if (cauda_equina == null)
                    {
                        try{cauda_equina = ss.AddStructure("Avoidance", CAUDA_EQUINA_ID); } catch { }
                    }


                    //Define PTV-GTV, PTV_OPT and GTV_OPT structures
                    Structure gtv_expanded = ss.AddStructure("Avoidance", "GTV_exp");
                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        gtv_expanded.ConvertToHighResolution();
                    }
                    gtv_expanded.SegmentVolume = gtv.Margin(1);


                    try
                    { ptv_gtv.SegmentVolume = ptv.Sub(gtv_expanded); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", gtv_expanded.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }
                    try { ptv_opt.SegmentVolume = ptv.Sub(spinal_cord_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", spinal_cord_prv.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }
                    try { ptv_opt.SegmentVolume = ptv_opt.Sub(cauda_equina_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", cauda_equina_prv.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }
                    try { ptv_opt.SegmentVolume = ptv_opt.Sub(sacral_plexus_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", sacral_plexus_prv.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }

                    try { ptv_opt.SegmentVolume = ptv_opt.Sub(gtv_expanded); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", ptv_opt.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }


                    try { gtv_opt.SegmentVolume = gtv.Sub(spinal_cord_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", spinal_cord_prv.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }
                    try { gtv_opt.SegmentVolume = gtv_opt.Sub(cauda_equina_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", cauda_equina_prv.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }
                    try { gtv_opt.SegmentVolume = gtv_opt.Sub(sacral_plexus_prv); }
                    catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", sacral_plexus_prv.Id), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }


                    //Define PTV_shell structure
                    Structure ptv_1mm = ss.AddStructure("Avoidance", "PTV_1mm");
                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        ptv_1mm.ConvertToHighResolution();

                    }
                    ptv_1mm.SegmentVolume = ptv.Margin(1);
                    expanded_PTV_2mm.SegmentVolume = ptv.Margin(3);
                    ptv_shell.SegmentVolume = expanded_PTV_2mm.Sub(ptv);


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create Skin structure
                    Structure body_4mm = ss.AddStructure("Avoidance", "BODY-4MM");
                    body_4mm.SegmentVolume = BODY.Margin(-4);
                    skin.SegmentVolume = BODY.Sub(body_4mm);

                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        skin.ConvertToHighResolution();
                    }


                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}",

                            ptv.Id, ptv.Volume,

                            ptv_opt.Id, ptv_opt.Volume,

                            gtv.Id, gtv.Volume,

                            gtv_opt.Id, gtv_opt.Volume,

                            ptv_gtv.Id, ptv_gtv.Volume


                            );

                        string message_OAR = string.Format("{0} volume = {1}\n{2} volume = {3}\n{4} volume = {5}\n{6} volume = {7}\n{8} volume = {9}\n{10} volume = {11}\n{12} volume = {13}\n{14} volume = {15}\n{16} volume = {17}\n{18} volume = {19}\n{20} volume = {21}\n{22} volume = {23}\n{24} volume ={25}\n{26} volume ={27}\n{28} volume ={29}\n{30} volume ={31}",

                            colon.Id, colon.Volume,

                            cauda_equina.Id, cauda_equina.Volume,

                            cauda_equina_prv.Id, cauda_equina_prv.Volume,

                            sacral_plexus.Id, sacral_plexus.Volume,

                            sacral_plexus_prv.Id, sacral_plexus_prv.Volume,

                            stomach.Id, stomach.Volume,

                            bladder_wall.Id, bladder_wall.Volume,

                            spinalcord.Id, spinalcord.Volume,

                            rib.Id, rib.Volume,

                            spinal_cord_prv.Id, spinal_cord_prv.Volume,

                            duodenum.Id, duodenum.Volume,

                            jejunum.Id, jejunum.Volume,

                            renal_cortex.Id, renal_cortex.Volume,

                            renal_hilum.Id, renal_hilum.Volume,

                            liver.Id, liver.Volume,

                            both_lungs.Id, both_lungs.Volume

                            );




                        //Show the messages

                        MessageBox.Show(message_PTV);
                        MessageBox.Show(message_OAR);
                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers than was required during the script operations

                    ss.RemoveStructure(expanded_PTV_2mm);
                    ss.RemoveStructure(body_4mm);
                    ss.RemoveStructure(ptv_1mm);
                    ss.RemoveStructure(gtv_expanded);
                }
                if (SRS_Brain1targetRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_SRS_brain_HR";

                    //IDs of PTV structures. May be convenient to have low/intermediate/high instead of 54/60/66
                    const string PTV_ID = "PTV";
                    const string OPT_PTV_ID = "PTV_OPT";
                    const string GTV_ID = "GTV";
                    const string OPT_GTV_ID = "GTV_OPT";
                    const string PTV_GTV_ID = "PTV-GTV";
                    const string PTV_SHELL_ID = "PTV_shell";


                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_2MM_PTV_ID = "PTV_2mm";


                    //IDs of critical structures
                    const string BODY_ID = "BODY";
                    const string SKIN_ID = "Skin_bySCRPT";
                    const string OPTIC_PATHWAY_ID = "Optic pathway";
                    const string OPTIC_PATHWAY_PRV_ID = "OptcPth_PRV";
                    const string COCHLEA_ID = "Cochlea";
                    const string COCHLEA_PRV_ID = "Cchl_PRV";
                    const string BRAINSTEM_ID = "Brainstem";
                    const string BRAINSTEM_PRV_ID = "Brnst_PRV";
                    const string BRAINSTEM_OPT_ID = "Brnst-PTV";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpinalCo_PRV";
                    const string SPINAL_CORD_OPT = "SpinalCo-PTV";
                    const string SPINAL_CORD_SUBVOL_ID = "SpinalCrd Subvol";
                    const string MEDULLA_ID = "Medulla";
                    const string ESOPHAGUS_ID = "Esophagus";
                    const string ESOPHAGUS_OPT_ID = "Esphgs-PTV";
                    const string BRACHIAL_PLEXUS_ID = "Brachial plexus";
                    const string BRACHIAL_PLEXUS_PRV_ID = "BrchlPs_PRV";
                    const string BRACHIAL_PLEXUS_PTV_ID = "BrchlPs-PTV";
                    const string TRACHEA_ID = "Trachea";
                    const string TRACHEA_OPT_ID = "Trachea-PTV";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structure PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'.\n For implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution

                    List<string> NOT_FOUND_list = new List<string>();

                    //find Skin
                    Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);


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
                        try{cochlea = ss.AddStructure("Avoidance", COCHLEA_ID); } catch { }
                    }

                    //find BrainStem
                    Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                    if (brainstem == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_ID);
                        try{brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { }
                    }

                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try{esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try{spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Medulla
                    Structure medulla = ss.Structures.FirstOrDefault(x => x.Id == MEDULLA_ID);
                    if (medulla == null)
                    {
                        NOT_FOUND_list.Add(MEDULLA_ID);
                        try{medulla = ss.AddStructure("Avoidance", MEDULLA_ID); } catch { }
                    }

                    //find SpinalCord sub volume
                    Structure spinalcord_subvol = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_SUBVOL_ID);
                    if (spinalcord_subvol == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_SUBVOL_ID);
                        try{spinalcord_subvol = ss.AddStructure("Avoidance", SPINAL_CORD_SUBVOL_ID); } catch { }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try{trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find Brachial plexus
                    Structure brachial_plexus = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_ID);
                    if (brachial_plexus == null)
                    {
                        NOT_FOUND_list.Add(BRACHIAL_PLEXUS_ID);
                        try{brachial_plexus = ss.AddStructure("Avoidance", BRACHIAL_PLEXUS_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV and GTV structures in the structure set. If PTV and GTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    Structure gtv = ss.Structures.FirstOrDefault(x => x.Id == GTV_ID);
                    if (gtv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Check if the optimization structures already exist. If so, delete it from the structure set


                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);
                    Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_ID);
                    Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);
                    Structure optic_pathway_prv = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_PATHWAY_PRV_ID);
                    Structure cochlea_prv = ss.Structures.FirstOrDefault(x => x.Id == COCHLEA_PRV_ID);
                    Structure brainstem_prv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV_ID);
                    Structure brainstem_ptv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_OPT_ID);
                    Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    Structure spinal_cord_ptv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_OPT);
                    Structure brachial_plexus_prv = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_PRV_ID);
                    Structure brachial_plexus_ptv = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_PTV_ID);
                    Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                    Structure esophagus_ptv = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_OPT_ID);
                    Structure trachea_ptv = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_OPT_ID);
                    Structure expanded_PTV_2mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_2MM_PTV_ID);
                    Structure body_ptv = ss.Structures.FirstOrDefault(x => x.Id == "body_ptv");
                    Structure body_ptv_1 = ss.Structures.FirstOrDefault(x => x.Id == "body_ptv1");
                    Structure body_ptv_2 = ss.Structures.FirstOrDefault(x => x.Id == "body_ptv12");
                    Structure body_ptv_3 = ss.Structures.FirstOrDefault(x => x.Id == "body_ptv123");
                    Structure body_gtv = ss.Structures.FirstOrDefault(x => x.Id == "body_gtv");
                    Structure body_gtv_1 = ss.Structures.FirstOrDefault(x => x.Id == "body_gtv1");
                    Structure body_gtv_2 = ss.Structures.FirstOrDefault(x => x.Id == "body_gtv12");
                    Structure body_gtv_3 = ss.Structures.FirstOrDefault(x => x.Id == "body_gtv123");


                    Structure[] OPT_STR =  {ptv_opt, gtv_opt, ptv_shell, optic_pathway_prv,
                                     cochlea_prv, brainstem_prv, brainstem_ptv, spinal_cord_prv, spinal_cord_ptv, brachial_plexus_prv, brachial_plexus_ptv,
                                     ptv_gtv, esophagus_ptv, trachea_ptv, expanded_PTV_2mm,body_ptv, body_ptv_1, body_ptv_2,
                                     body_ptv_3, body_gtv, body_gtv_1, body_gtv_2, body_gtv_3};


                    foreach (Structure s in OPT_STR)
                    {
                        if (s != null)
                        {
                            try{ss.RemoveStructure(s);} catch { }
                        }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script

                    try{ptv_opt = ss.AddStructure("Avoidance", OPT_PTV_ID); } catch { }
                    try{gtv_opt = ss.AddStructure("Avoidance", OPT_GTV_ID); } catch { }
                    try{ptv_shell = ss.AddStructure("Avoidance", PTV_SHELL_ID); } catch { }
                    try{ptv_gtv = ss.AddStructure("Avoidance", PTV_GTV_ID); } catch { }
                    try{esophagus_ptv = ss.AddStructure("Avoidance", ESOPHAGUS_OPT_ID); } catch { }
                    try{expanded_PTV_2mm = ss.AddStructure("Avoidance", EXPANDED_2MM_PTV_ID); } catch { }
                    try{body_ptv = ss.AddStructure("Avoidance", "body_ptv"); } catch { }
                    try{body_gtv = ss.AddStructure("Avoidance", "body_gtv"); } catch { }

                    //Check for high resolution

                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        foreach (Structure s in ss.Structures)
                        {
                            if (s.IsHighResolution == false && s.Id != "CouchSurface" && s.Id != "CouchInterior" && s.DicomType != "EXTERNAL" && s.Id != "BODY")
                            {
                                try
                                {
                                    s.ConvertToHighResolution();
                                }
                                catch { MessageBox.Show(string.Format("'{0}' could not be converted to high resolution",s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); }
                            }
                        }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create PTV-GTV structures

                    ptv_gtv.SegmentVolume = ptv.Sub(gtv);

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create PTV_OPT structures
                    Structure[] OARs = { skin, optic_pathway, cochlea, brainstem, esophagus, spinalcord, medulla, spinalcord_subvol, trachea, brachial_plexus };

                    //for PTV
                    if (ptv.Volume > 0.01 && gtv.Volume > 0.01)
                    {
                        ptv_opt.SegmentVolume = ptv.Sub(gtv);
                        foreach (Structure s in OARs)
                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        ptv_opt.SegmentVolume = ptv_opt.Sub(s);
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }

                        }
                    }

                    //Create GTV_OPT structures

                    //for GTV
                    if (gtv.Volume > 0.01)
                    {
                        gtv_opt.SegmentVolume = gtv.Margin(0);
                        foreach (Structure s in OARs)
                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        gtv_opt.SegmentVolume = gtv_opt.Sub(s);
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }

                        }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 2mm margin. If a structure is empty, the empty structure will be created

                    Structure[] FOR_PRV = { brainstem, optic_pathway, cochlea, spinalcord, brachial_plexus };

                    foreach (Structure s in FOR_PRV)
                    {
                        if (s != null)
                        {
                            string short_id = s.Id;
                            if (s.Id.Length > 9)

                            {
                                string long_id = s.Id;
                                short_id = long_id.Remove(8);
                            }

                            string PRV_ID = string.Format("{0}_PRV", short_id);
                            Structure prv = ss.Structures.FirstOrDefault(x => x.Id == PRV_ID);
                            if (prv != null)
                            {
                                try{ss.RemoveStructure(prv); } catch { }
                            }
                            prv = ss.AddStructure("Avoidance", PRV_ID);
                            if (ptv.IsHighResolution || gtv.IsHighResolution)
                            {
                                try{prv.ConvertToHighResolution(); } catch { }
                            }
                            try{prv.SegmentVolume = s.Margin(3); } catch { }
                        }


                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Substract critical structures from expanded PTV_ALL to create 2mm buffer
                    Structure[] OPTstrs = { esophagus, trachea, brainstem, spinalcord, brachial_plexus };
                    expanded_PTV_2mm.SegmentVolume = ptv.Margin(2);

                    foreach (Structure s in OPTstrs)
                    {
                        if (s != null)
                        {
                            string short_id = s.Id;
                            if (s.Id.Length > 9)

                            {
                                string long_id = s.Id;
                                short_id = long_id.Remove(8);
                            }

                            string _PTV_ID = string.Format("{0}_PTV", short_id);
                            Structure _ptv = ss.Structures.FirstOrDefault(x => x.Id == _PTV_ID);
                            if (_ptv != null)
                            {
                                try{ss.RemoveStructure(_ptv); } catch { }
                            }
                            _ptv = ss.AddStructure("Avoidance", _PTV_ID);
                            if (ptv.IsHighResolution || gtv.IsHighResolution)
                            {
                                try { _ptv.ConvertToHighResolution(); } catch { }
        }
                            _ptv.SegmentVolume = s.Margin(0);
                            _ptv.SegmentVolume = _ptv.Sub(expanded_PTV_2mm);
                        }

                    }



                    //Define PTV_shell structure
                    Structure ptv_1mm = ss.AddStructure("Avoidance", "PTV_1mm");
                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        ptv_1mm.ConvertToHighResolution();
                    }
                    ptv_1mm.SegmentVolume = ptv.Margin(-1);
                    ptv_shell.SegmentVolume = expanded_PTV_2mm.Sub(ptv);


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create Skin structure
                    Structure body_4mm = ss.AddStructure("Avoidance", "BODY-4MM");
                    skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                    if (skin != null)
                    {
                        try { ss.RemoveStructure(skin); } catch { }
                    }
                    skin = ss.AddStructure("Avoidance", SKIN_ID);
                    body_4mm.SegmentVolume = BODY.Margin(-4);
                    skin.SegmentVolume = BODY.Sub(body_4mm);

                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        try{skin.ConvertToHighResolution(); } catch { }
                    }


                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}",

                            ptv.Id, ptv.Volume,

                            gtv.Id, gtv.Volume

                            );


                        //===========================================================================================================================================================================
                        //===========================================================================================================================================================================


                        //Show the messages

                        MessageBox.Show(message_PTV);
                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers that were required during the script operations

                    try { ss.RemoveStructure(expanded_PTV_2mm); } catch { }
                    try{ss.RemoveStructure(body_4mm); } catch { }
                    try
                    {ss.RemoveStructure(ptv_1mm); }
                    catch { }
                    try
                    {ss.RemoveStructure(body_gtv); }
                    catch { }
                    try
                    {ss.RemoveStructure(body_ptv); }
                    catch { }
                }
                if (SRS_BrainMultipleTargetsRB.IsChecked == true)
                {
                    //Change these IDs to match your clinical protocol

                    //Name of the script
                    const string SCRIPT_NAME = "OPT_str_SRS_brain_HR";

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
                    const string PTV_ALL_ID = "PTV_ALL";
                    const string GTV_ALL_ID = "GTV_ALL";

                    //We will use a structure expanded from PTV_ALL to create the margin for critical structure's dose optimization. It will be removed at the end of the script's execution
                    const string EXPANDED_2MM_PTV_ID = "PTV_2mm";


                    //IDs of critical structures
                    const string BODY_ID = "BODY";
                    const string SKIN_ID = "Skin_bySCRPT";
                    const string OPTIC_PATHWAY_ID = "Optic pathway";
                    const string OPTIC_PATHWAY_PRV_ID = "OptcPth_PRV";
                    const string COCHLEA_ID = "Cochlea";
                    const string COCHLEA_PRV_ID = "Cchl_PRV";
                    const string BRAINSTEM_ID = "Brainstem";
                    const string BRAINSTEM_PRV_ID = "Brnst_PRV";
                    const string BRAINSTEM_OPT_ID = "Brnst-PTV";
                    const string SPINAL_CORD_ID = "SpinalCord";
                    const string SPINAL_CORD_PRV_ID = "SpnlCrd_PRV";
                    const string SPINAL_CORD_OPT = "SpnlCrd-PTV";
                    const string SPINAL_CORD_SUBVOL_ID = "SpinalCrd Subvol";
                    const string MEDULLA_ID = "Medulla";
                    const string ESOPHAGUS_ID = "Esophagus";
                    const string ESOPHAGUS_OPT_ID = "Esphgs-PTV";
                    const string BRACHIAL_PLEXUS_ID = "Brachial plexus";
                    const string BRACHIAL_PLEXUS_PRV_ID = "BrchlPs_PRV";
                    const string BRACHIAL_PLEXUS_PTV_ID = "BrchlPs-PTV";
                    const string TRACHEA_ID = "Trachea";
                    const string TRACHEA_OPT_ID = "Trachea-PTV";

                    //Show greetings window

                    string Greeting = string.Format("Greetings {0}! Please, ensure that structure PTV, BODY exist in the current structure set. This script is made by 'PET_Tehnology'.\n For implementing this script, please contact to Daniel Peidus ('daniel.peudys@gmail.com' or 'd.peidus@pet-net.ru')", user);
                    MessageBox.Show(Greeting);

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Make this script enabled for writing

                    VMS.TPS.Script.context.Patient.BeginModifications();


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // Find critical structures in the structure set. If some structures will not be found, the user will recieve a notification. This notification will not prevent script from execution

                    List<string> NOT_FOUND_list = new List<string>();

                    //find Skin
                    Structure skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);


                    //find Optic pathway
                    Structure optic_pathway = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_PATHWAY_ID);
                    if (optic_pathway == null)
                    {
                        NOT_FOUND_list.Add(OPTIC_PATHWAY_ID);
                        try{optic_pathway = ss.AddStructure("Avoidance", OPTIC_PATHWAY_ID); } catch { }
                    }

                    //find Cochlea
                    Structure cochlea = ss.Structures.FirstOrDefault(x => x.Id == COCHLEA_ID);
                    if (cochlea == null)
                    {
                        NOT_FOUND_list.Add(COCHLEA_ID);
                        try{cochlea = ss.AddStructure("Avoidance", COCHLEA_ID); } catch { }
                    }

                    //find BrainStem
                    Structure brainstem = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_ID);
                    if (brainstem == null)
                    {
                        NOT_FOUND_list.Add(BRAINSTEM_ID);
                        try{brainstem = ss.AddStructure("Avoidance", BRAINSTEM_ID); } catch { }
                    }

                    //find Esophagus
                    Structure esophagus = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_ID);
                    if (esophagus == null)
                    {
                        NOT_FOUND_list.Add(ESOPHAGUS_ID);
                        try{esophagus = ss.AddStructure("Avoidance", ESOPHAGUS_ID); } catch { }
                    }

                    //find SpinalCord
                    Structure spinalcord = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_ID);
                    if (spinalcord == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_ID);
                        try{ spinalcord = ss.AddStructure("Avoidance", SPINAL_CORD_ID); } catch { }
                    }

                    //find Medulla
                    Structure medulla = ss.Structures.FirstOrDefault(x => x.Id == MEDULLA_ID);
                    if (medulla == null)
                    {
                        NOT_FOUND_list.Add(MEDULLA_ID);
                        try{medulla = ss.AddStructure("Avoidance", MEDULLA_ID); } catch { }
                    }

                    //find SpinalCord sub volume
                    Structure spinalcord_subvol = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_SUBVOL_ID);
                    if (spinalcord_subvol == null)
                    {
                        NOT_FOUND_list.Add(SPINAL_CORD_SUBVOL_ID);
                        try{spinalcord_subvol = ss.AddStructure("Avoidance", SPINAL_CORD_SUBVOL_ID); } catch { }
                    }

                    //find Trachea
                    Structure trachea = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_ID);
                    if (trachea == null)
                    {
                        NOT_FOUND_list.Add(TRACHEA_ID);
                        try{trachea = ss.AddStructure("Avoidance", TRACHEA_ID); } catch { }
                    }

                    //find Brachial plexus
                    Structure brachial_plexus = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_ID);
                    if (brachial_plexus == null)
                    {
                        NOT_FOUND_list.Add(BRACHIAL_PLEXUS_ID);
                        try{brachial_plexus = ss.AddStructure("Avoidance", BRACHIAL_PLEXUS_ID); } catch { }
                    }

                    //find BODY
                    Structure BODY = ss.Structures.FirstOrDefault(x => x.Id == BODY_ID);
                    if (BODY == null)
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

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    // find PTV and GTV structures in the structure set. If PTV and GTV will not be found, the user will recive an error notification. This notification will prevent script from execution

                    Structure ptv = ss.Structures.FirstOrDefault(x => x.Id == PTV_ID);
                    if (ptv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", PTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    Structure ptv_1 = ss.Structures.FirstOrDefault(x => x.Id == PTV_1_ID);
                    if (ptv_1 == null)
                    {
                        try{ptv_1 = ss.AddStructure("Avoidance", PTV_1_ID); } catch { }
                    }
                    Structure ptv_2 = ss.Structures.FirstOrDefault(x => x.Id == PTV_2_ID);
                    if (ptv_2 == null)
                    {
                        try{ptv_2 = ss.AddStructure("Avoidance", PTV_2_ID); } catch { }
                    }
                    Structure ptv_3 = ss.Structures.FirstOrDefault(x => x.Id == PTV_3_ID);
                    if (ptv_3 == null)
                    {
                        try{ptv_3 = ss.AddStructure("Avoidance", PTV_3_ID); } catch { }
                    }

                    Structure gtv = ss.Structures.FirstOrDefault(x => x.Id == GTV_ID);
                    if (gtv == null)
                    {
                        MessageBox.Show(string.Format("'{0}' not found!", GTV_ID), SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                        Window.GetWindow(this).Close(); return;
                    }

                    Structure gtv_1 = ss.Structures.FirstOrDefault(x => x.Id == GTV_1_ID);
                    if (gtv_1 == null)
                    {
                        try{gtv_1 = ss.AddStructure("Avoidance", GTV_1_ID); } catch { }
                    }
                    Structure gtv_2 = ss.Structures.FirstOrDefault(x => x.Id == GTV_2_ID);
                    if (gtv_2 == null)
                    {
                        try{gtv_2 = ss.AddStructure("Avoidance", GTV_2_ID); } catch { }
                    }
                    Structure gtv_3 = ss.Structures.FirstOrDefault(x => x.Id == GTV_3_ID);
                    if (gtv_3 == null)
                    {
                        try{gtv_3 = ss.AddStructure("Avoidance", GTV_3_ID); } catch { }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Check if the optimization structures already exist. If so, delete it from the structure set


                    Structure ptv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_ID);
                    Structure ptv_opt_1 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_1_ID);
                    Structure ptv_opt_2 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_2_ID);
                    Structure ptv_opt_3 = ss.Structures.FirstOrDefault(x => x.Id == OPT_PTV_3_ID);
                    Structure gtv_opt = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_ID);
                    Structure gtv_opt_1 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_1_ID);
                    Structure gtv_opt_2 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_2_ID);
                    Structure gtv_opt_3 = ss.Structures.FirstOrDefault(x => x.Id == OPT_GTV_3_ID);
                    Structure ptv_shell = ss.Structures.FirstOrDefault(x => x.Id == PTV_SHELL_ID);
                    Structure ptv_all = ss.Structures.FirstOrDefault(x => x.Id == PTV_ALL_ID);
                    Structure gtv_all = ss.Structures.FirstOrDefault(x => x.Id == GTV_ALL_ID);
                    Structure optic_pathway_prv = ss.Structures.FirstOrDefault(x => x.Id == OPTIC_PATHWAY_PRV_ID);
                    Structure cochlea_prv = ss.Structures.FirstOrDefault(x => x.Id == COCHLEA_PRV_ID);
                    Structure brainstem_prv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_PRV_ID);
                    Structure brainstem_ptv = ss.Structures.FirstOrDefault(x => x.Id == BRAINSTEM_OPT_ID);
                    Structure spinal_cord_prv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_PRV_ID);
                    Structure spinal_cord_ptv = ss.Structures.FirstOrDefault(x => x.Id == SPINAL_CORD_OPT);
                    Structure brachial_plexus_prv = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_PRV_ID);
                    Structure brachial_plexus_ptv = ss.Structures.FirstOrDefault(x => x.Id == BRACHIAL_PLEXUS_PTV_ID);
                    Structure ptv_gtv = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_ID);
                    Structure ptv_gtv_1 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_1_ID);
                    Structure ptv_gtv_2 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_2_ID);
                    Structure ptv_gtv_3 = ss.Structures.FirstOrDefault(x => x.Id == PTV_GTV_3_ID);
                    Structure esophagus_ptv = ss.Structures.FirstOrDefault(x => x.Id == ESOPHAGUS_OPT_ID);
                    Structure trachea_ptv = ss.Structures.FirstOrDefault(x => x.Id == TRACHEA_OPT_ID);
                    Structure expanded_PTV_2mm = ss.Structures.FirstOrDefault(x => x.Id == EXPANDED_2MM_PTV_ID);
                    Structure body_ptv = ss.Structures.FirstOrDefault(x => x.Id == "body_ptv");
                    Structure body_ptv_1 = ss.Structures.FirstOrDefault(x => x.Id == "body_ptv1");
                    Structure body_ptv_2 = ss.Structures.FirstOrDefault(x => x.Id == "body_ptv12");
                    Structure body_ptv_3 = ss.Structures.FirstOrDefault(x => x.Id == "body_ptv123");
                    Structure body_gtv = ss.Structures.FirstOrDefault(x => x.Id == "body_gtv");
                    Structure body_gtv_1 = ss.Structures.FirstOrDefault(x => x.Id == "body_gtv1");
                    Structure body_gtv_2 = ss.Structures.FirstOrDefault(x => x.Id == "body_gtv12");
                    Structure body_gtv_3 = ss.Structures.FirstOrDefault(x => x.Id == "body_gtv123");


                    Structure[] OPT_STR =  {ptv_opt, ptv_opt_1, ptv_opt_2, ptv_opt_3, gtv_opt, gtv_opt_1, gtv_opt_2, gtv_opt_3, ptv_shell, ptv_all, gtv_all, optic_pathway_prv,
                                     cochlea_prv, brainstem_prv, brainstem_ptv, spinal_cord_prv, spinal_cord_ptv, brachial_plexus_prv, brachial_plexus_ptv,
                                     ptv_gtv, ptv_gtv_1, ptv_gtv_2, ptv_gtv_3, esophagus_ptv, trachea_ptv, expanded_PTV_2mm,body_ptv, body_ptv_1, body_ptv_2,
                                     body_ptv_3, body_gtv, body_gtv_1, body_gtv_2, body_gtv_3};


                    foreach (Structure s in OPT_STR)
                    {
                        if (s != null)
                        {
                            try{ss.RemoveStructure(s); } catch { }
                        }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create the empty structures requered for the script
                    try{ptv_opt = ss.AddStructure("Avoidance", OPT_PTV_ID); } catch { }
                    try{ptv_opt_1 = ss.AddStructure("Avoidance", OPT_PTV_1_ID); } catch { }
                    try{ptv_opt_2 = ss.AddStructure("Avoidance", OPT_PTV_2_ID);} catch { }
                    try{ptv_opt_3 = ss.AddStructure("Avoidance", OPT_PTV_3_ID); } catch { }
                    try
                    {gtv_opt = ss.AddStructure("Avoidance", OPT_GTV_ID); }
                    catch { }
                    try
                    {gtv_opt_1 = ss.AddStructure("Avoidance", OPT_GTV_1_ID); }
                    catch { }
                    try
                    {gtv_opt_2 = ss.AddStructure("Avoidance", OPT_GTV_2_ID); }
                    catch { }
                    try
                    {gtv_opt_3 = ss.AddStructure("Avoidance", OPT_GTV_3_ID); }
                    catch { }
                    try
                    {ptv_shell = ss.AddStructure("Avoidance", PTV_SHELL_ID); }
                    catch { }
                    try
                    {ptv_all = ss.AddStructure("Avoidance", PTV_ALL_ID); }
                    catch { }
                    try
                    {gtv_all = ss.AddStructure("Avoidance", GTV_ALL_ID); }
                    catch { }
                    try
                    {ptv_gtv = ss.AddStructure("Avoidance", PTV_GTV_ID); }
                    catch { }
                    try
                    {ptv_gtv_1 = ss.AddStructure("Avoidance", PTV_GTV_1_ID); }
                    catch { }
                    try
                    {ptv_gtv_2 = ss.AddStructure("Avoidance", PTV_GTV_2_ID); }
                    catch { }
                    try
                    {ptv_gtv_3 = ss.AddStructure("Avoidance", PTV_GTV_3_ID); }
                    catch { }
                    try
                    {esophagus_ptv = ss.AddStructure("Avoidance", ESOPHAGUS_OPT_ID); }
                    catch { }
                    try
                    {expanded_PTV_2mm = ss.AddStructure("Avoidance", EXPANDED_2MM_PTV_ID); }
                    catch { }
                    try
                    {body_ptv = ss.AddStructure("Avoidance", "body_ptv"); }
                    catch { }
                    try
                    {body_ptv_1 = ss.AddStructure("Avoidance", "body_ptv1"); }
                    catch { }
                    try
                    {body_ptv_2 = ss.AddStructure("Avoidance", "body_ptv12"); }
                    catch { }
                    try
                    {body_ptv_3 = ss.AddStructure("Avoidance", "body_ptv123"); }
                    catch { }
                    try
                    {body_gtv = ss.AddStructure("Avoidance", "body_gtv"); }
                    catch { }
                    try
                    {body_gtv_1 = ss.AddStructure("Avoidance", "body_gtv1"); }
                    catch { }
                    try
                    {body_gtv_2 = ss.AddStructure("Avoidance", "body_gtv12"); }
                    catch { }
                    try
                    {body_gtv_3 = ss.AddStructure("Avoidance", "body_gtv123"); }
                    catch { }


                    //Check for high resolution

                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        foreach (Structure s in ss.Structures)
                        {
                            if (s.IsHighResolution == false && s.Id != "CouchSurface" && s.Id != "CouchInterior" && s.DicomType != "EXTERNAL" && s.Id != "BODY")
                            {
                                try
                                {
                                    s.ConvertToHighResolution();
                                }
                                catch { try { MessageBox.Show(string.Format("'{0}' could not be converted to high resolution",s.Id),SCRIPT_NAME,MessageBoxButton.OK, MessageBoxImage.Exclamation); } catch { } }
                            }
                        }
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create PTV_ALL structure

                    if (ptv_1 != null || ptv_2 != null || ptv_3 != null)
                    {
                        if (ptv_1.Volume > 0.01 || ptv_2.Volume > 0.01 || ptv_3.Volume > 0.01)
                        {

                            Structure BODY_HD = ss.AddStructure("Avoidance", "BODY_HD");
                            BODY_HD.SegmentVolume = BODY.Margin(0);
                            if (ptv.IsHighResolution || gtv.IsHighResolution)
                            {
                                BODY_HD.ConvertToHighResolution();
                            }

                            body_ptv.SegmentVolume = BODY_HD.Sub(ptv);
                            body_ptv_1.SegmentVolume = body_ptv.Sub(ptv_1);
                            body_ptv_2.SegmentVolume = body_ptv_1.Sub(ptv_2);
                            body_ptv_3.SegmentVolume = body_ptv_2.Sub(ptv_3);
                            ptv_all.SegmentVolume = BODY_HD.Sub(body_ptv_3);
                            try{ss.RemoveStructure(body_ptv); } catch { }
                            try{ss.RemoveStructure(body_ptv_1); } catch { }
                            try{ss.RemoveStructure(body_ptv_2); } catch { }
                            try{ss.RemoveStructure(body_ptv_3); } catch { }
                            try{ss.RemoveStructure(BODY_HD); } catch { }
                        }
                        if (ptv_1.Volume < 0.01 && ptv_2.Volume < 0.01 && ptv_3.Volume < 0.01)
                        {
                            ptv_all.SegmentVolume = ptv.Margin(0);
                            try{ss.RemoveStructure(body_ptv); } catch { }
                            try{ss.RemoveStructure(body_ptv_1); } catch { }
                            try{ss.RemoveStructure(body_ptv_2); } catch { }
                            try{ss.RemoveStructure(body_ptv_3); } catch { }
                        }
                    }
                    if (ptv_1 == null && ptv_2 == null && ptv_3 == null)
                    {
                        ptv_all.SegmentVolume = ptv.Margin(0);
                        try{ss.RemoveStructure(body_ptv); } catch { }
                        try{ss.RemoveStructure(body_ptv_1); } catch { }
                        try{ss.RemoveStructure(body_ptv_2); } catch { }
                        try{ss.RemoveStructure(body_ptv_3); } catch { }
                    }


                    //Create GTV_ALL structure


                    if (gtv_1 != null || gtv_2 != null || gtv_3 != null)
                    {
                        if (gtv_1.Volume > 0.01 || gtv_2.Volume > 0.01 || gtv_3.Volume > 0.01)
                        {
                            Structure BODY_HD = ss.AddStructure("Avoidance", "BODY_HD");
                            BODY_HD.SegmentVolume = BODY.Margin(0);
                            if (ptv.IsHighResolution || gtv.IsHighResolution)
                            {
                                BODY_HD.ConvertToHighResolution();
                            }
                            body_gtv.SegmentVolume = BODY_HD.Sub(gtv);
                            body_gtv_1.SegmentVolume = body_gtv.Sub(gtv_1);
                            body_gtv_2.SegmentVolume = body_gtv_1.Sub(gtv_2);
                            body_gtv_3.SegmentVolume = body_gtv_2.Sub(gtv_3);
                            gtv_all.SegmentVolume = BODY_HD.Sub(body_gtv_3);
                            try{ss.RemoveStructure(body_gtv); } catch { }
                            try{ss.RemoveStructure(body_gtv_1); } catch { }
                            try{ss.RemoveStructure(body_gtv_2); } catch { }
                            try{ss.RemoveStructure(body_gtv_3); } catch { }
                            try{ss.RemoveStructure(BODY_HD); } catch { }
                        }
                        if (gtv_1.Volume < 0.01 && gtv_2.Volume < 0.01 && gtv_3.Volume < 0.01)
                        {
                            gtv_all.SegmentVolume = gtv.Margin(0);
                            try{ss.RemoveStructure(body_gtv); } catch { }
                            try{ss.RemoveStructure(body_gtv_1); } catch { }
                            try{ss.RemoveStructure(body_gtv_2); } catch { }
                            try{ss.RemoveStructure(body_gtv_3); } catch { }
                        }
                    }
                    if (gtv_1 == null && gtv_2 == null && gtv_3 == null)
                    {
                        gtv_all.SegmentVolume = gtv.Margin(0);
                        try{ss.RemoveStructure(body_gtv); } catch { }
                        try{ss.RemoveStructure(body_gtv_1); } catch { }
                        try{ss.RemoveStructure(body_gtv_2); } catch { }
                        try{ss.RemoveStructure(body_gtv_3); } catch { }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create PTV-GTV structures

                    ptv_gtv.SegmentVolume = ptv.Sub(gtv);

                    if (ptv_1.Volume > 0.01)
                    {
                        ptv_gtv_1.SegmentVolume = ptv_1.Sub(gtv_1);
                    }
                    if (ptv_2.Volume > 0.01)
                    {
                        ptv_gtv_2.SegmentVolume = ptv_2.Sub(gtv_2);
                    }
                    if (ptv_3.Volume > 0.01)
                    {
                        ptv_gtv_3.SegmentVolume = ptv_3.Sub(gtv_3);
                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================

                    //Create PTV_OPT structures
                    Structure[] OARs = { skin, optic_pathway, cochlea, brainstem, esophagus, spinalcord, medulla, spinalcord_subvol, trachea, brachial_plexus };

                    //for PTV
                    if (ptv.Volume > 0.01 && gtv.Volume > 0.01)
                    {
                        ptv_opt.SegmentVolume = ptv.Sub(gtv);

                        foreach (Structure s in OARs)

                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        try{ptv_opt.SegmentVolume = ptv_opt.Sub(s); } catch { }
                                    }
                                }

                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }

                        }

                    }


                    //for PTV_1
                    if (ptv_1.Volume > 0.01 && gtv_1.Volume > 0.01)
                    {
                        ptv_opt_1.SegmentVolume = ptv_1.Sub(gtv_1);
                        foreach (Structure s in OARs)
                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        try{ptv_opt_1.SegmentVolume = ptv_opt_1.Sub(s); } catch { }
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV_1", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }

                        }
                    }

                    //for PTV_2
                    if (ptv_2.Volume > 0.01 && gtv_2.Volume > 0.01)
                    {
                        ptv_opt_2.SegmentVolume = ptv_2.Sub(gtv_2);
                        foreach (Structure s in OARs)
                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        try{ptv_opt_2.SegmentVolume = ptv_opt_2.Sub(s); } catch { }
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV_2", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }
                        }
                    }

                    //for PTV_3
                    if (ptv_3.Volume > 0.01 && gtv_3.Volume > 0.01)
                    {
                        ptv_opt_3.SegmentVolume = ptv_3.Sub(gtv_3);
                        foreach (Structure s in OARs)
                        {
                            try
                            {


                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        try{ptv_opt_3.SegmentVolume = ptv_opt_3.Sub(s); } catch { }
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from PTV_3", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }
                        }
                    }


                    //Create GTV_OPT structures

                    //for GTV
                    if (gtv.Volume > 0.01)
                    {
                        gtv_opt.SegmentVolume = gtv.Margin(0);
                        foreach (Structure s in OARs)
                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        try{gtv_opt.SegmentVolume = gtv_opt.Sub(s); } catch { }
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }

                        }
                    }

                    //for GTV_1
                    if (gtv_1.Volume > 0.01)
                    {
                        gtv_opt_1.SegmentVolume = gtv_1.Margin(0);
                        foreach (Structure s in OARs)
                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        try{gtv_opt_1.SegmentVolume = gtv_opt_1.Sub(s); } catch { }
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV_1", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }
                        }
                    }

                    //for GTV_2
                    if (gtv_2.Volume > 0.01)
                    {
                        gtv_opt_2.SegmentVolume = gtv_2.Margin(0);
                        foreach (Structure s in OARs)
                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        try{gtv_opt_2.SegmentVolume = gtv_opt_2.Sub(s); } catch { }
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV_2", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }
                        }
                    }

                    //for GTV_3
                    if (gtv_3.Volume > 0.01)
                    {
                        gtv_opt_3.SegmentVolume = gtv_3.Margin(0);
                        foreach (Structure s in OARs)
                        {
                            try
                            {
                                if (s != null)
                                {
                                    if (s.Volume > 0.01)
                                    {
                                        try{gtv_opt_3.SegmentVolume = gtv_opt_3.Sub(s); } catch { }
                                    }
                                }
                            }
                            catch { try { MessageBox.Show(string.Format("Unfortunately, {0} could not be substracted from GTV_3", s.Id),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); } catch { } }
                        }
                    }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Define PRVs with 2mm margin. If a structure is empty, the empty structure will be created

                    Structure[] FOR_PRV = { brainstem, optic_pathway, cochlea, spinalcord, brachial_plexus };

                    foreach (Structure s in FOR_PRV)
                    {
                        if (s != null)
                        {
                            string short_id = s.Id;
                            if (s.Id.Length > 9)

                            {
                                string long_id = s.Id;
                                short_id = long_id.Remove(8);
                            }

                            string PRV_ID = string.Format("{0}_PRV", short_id);
                            Structure prv = ss.Structures.FirstOrDefault(x => x.Id == PRV_ID);
                            if (prv != null)
                            {
                                try{ss.RemoveStructure(prv); } catch { }
                            }
                            prv = ss.AddStructure("Avoidance", PRV_ID);
                            if (ptv.IsHighResolution || gtv.IsHighResolution)
                            {
                                try{prv.ConvertToHighResolution(); } catch { }
                            }
                            prv.SegmentVolume = s.Margin(3);
                        }


                    }


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Substract critical structures from expanded PTV_ALL to create 2mm buffer
                    Structure[] OPTstrs = { esophagus, trachea, brainstem, spinalcord, brachial_plexus };
                    expanded_PTV_2mm.SegmentVolume = ptv_all.Margin(2);

                    foreach (Structure s in OPTstrs)
                    {
                        if (s != null)
                        {
                            string short_id = s.Id;
                            if (s.Id.Length > 9)

                            {
                                string long_id = s.Id;
                                short_id = long_id.Remove(8);
                            }

                            string _PTV_ID = string.Format("{0}_PTV", short_id);
                            Structure _ptv = ss.Structures.FirstOrDefault(x => x.Id == _PTV_ID);
                            if (_ptv != null)
                            {
                                try{ss.RemoveStructure(_ptv); } catch { }
                            }
                            _ptv = ss.AddStructure("Avoidance", _PTV_ID);
                            if (ptv.IsHighResolution || gtv.IsHighResolution)
                            {
                                try { _ptv.ConvertToHighResolution(); } catch { }
                            }
                            _ptv.SegmentVolume = s.Margin(0);
                            _ptv.SegmentVolume = _ptv.Sub(expanded_PTV_2mm);
                        }

                    }



                    //Define PTV_shell structure
                    Structure ptv_1mm = ss.AddStructure("Avoidance", "PTV_1mm");
                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        ptv_1mm.ConvertToHighResolution();
                    }
                    ptv_1mm.SegmentVolume = ptv_all.Margin(-1);
                    ptv_shell.SegmentVolume = expanded_PTV_2mm.Sub(ptv_all);


                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Create Skin structure
                    Structure body_4mm = ss.AddStructure("Avoidance", "BODY-4MM");
                    skin = ss.Structures.FirstOrDefault(x => x.Id == SKIN_ID);
                    if (skin != null)
                    {
                        ss.RemoveStructure(skin);
                    }
                    skin = ss.AddStructure("Avoidance", SKIN_ID);
                    body_4mm.SegmentVolume = BODY.Margin(-4);
                    skin.SegmentVolume = BODY.Sub(body_4mm);

                    if (ptv.IsHighResolution || gtv.IsHighResolution)
                    {
                        skin.ConvertToHighResolution();
                    }


                    //Create a message after the script is executed
                    try
                    {
                        string message_PTV = string.Format("{0} volume = {1}\n{2} volume = {3}",

                            ptv.Id, ptv.Volume,

                            gtv.Id, gtv.Volume

                            );


                        //===========================================================================================================================================================================
                        //===========================================================================================================================================================================


                        //Show the messages

                        MessageBox.Show(message_PTV);
                    }

                    catch { MessageBox.Show(string.Format("Unfortunately, a report message could not be shown..."),SCRIPT_NAME,MessageBoxButton.OK,MessageBoxImage.Exclamation); }

                    //===========================================================================================================================================================================
                    //===========================================================================================================================================================================


                    //Remove trash structers that were required during the script operations

                    try{ss.RemoveStructure(expanded_PTV_2mm); } catch { }
                    try{ss.RemoveStructure(body_4mm); } catch { }
                    try{ss.RemoveStructure(ptv_1mm); } catch { }
                }
            }
            catch (Exception exception) { MessageBox.Show(string.Format("{0}\n\n\n at the line number {1}", exception.Message, exception.StackTrace), "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error); Window.GetWindow(this).Close(); }
            Window.GetWindow(this).Close();
        }

        private void UniversalTmplt(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("This module is still under development.\nPlease, let's keep things going..."),"Achtung",MessageBoxButton.OK,MessageBoxImage.Error);
            /*Window.GetWindow(this).Hide();
            UnivarsalStructres univarsalStructres = new UnivarsalStructres(VMS.TPS.Script.context);
            univarsalStructres.Visibility = Visibility.Visible;
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = univarsalStructres;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Universal structures";*/
        }

        private void Cancel(object sender, RoutedEventArgs e)
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

        private void StructureSetID_Changed(object sender, SelectionChangedEventArgs e)
        {

        }

        private void HeadAndNeck1trgt_Checked(object sender, RoutedEventArgs e)
        {
            HeadAndNeck1trgtRB.IsChecked = true;
            CrtStrsBYtmplt.IsEnabled = true;
        }
    }
}
