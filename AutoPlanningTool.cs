using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("0.0.2.74")]
[assembly: AssemblyFileVersion("0.0.2.74")]
[assembly: AssemblyInformationalVersion("2.74")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    
    public class Script
    {
        
        const string SCRIPT_NAME = "AutoPlanning_Tool";
        public static ScriptContext context = null;
        public Script() 
        { 

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext c/*, System.Windows.Window w, ScriptEnvironment environment*/)
        {
            //===========================================================================================================================================================================
            //===========================================================================================================================================================================
            context = c;

            //for licensing purpose
            var current_date = DateTime.Now;
            var user = context.CurrentUser;
            DateTime expiration_date = new DateTime(2022, 09, 30);
            DateTime warning_date = new DateTime(2022, 09, 26);
            if (current_date > expiration_date)
            {
                MessageBox.Show(string.Format("Dear {0},\n Thank you for your interest in using this script.\n Unfortunately, this version is expired.\n\n If you would like to proceed, contact to Daniel Peidus ('daniel.peudys@gmail.com')", user, MessageBoxButton.OK, MessageBoxImage.Error));
                return;
            }
            if (current_date > warning_date)
            {
                MessageBox.Show(string.Format("Dear {0},\n This message is to warn you that this version of software will expire soon. The expiration date is 30/09/2022.\n\n To purchase the next version, contact to Daniel Peidus ('daniel.peudys@gmail.com')", user, MessageBoxButton.OK, MessageBoxImage.Exclamation));
            }
            //Check if the patient, structure set are loaded. If yes, then download structure set. If no, show the error message to the user and stop execution
            if (context.Patient == null || context.StructureSet == null)
            {
                MessageBox.Show("Please load a patient data before running this script.", SCRIPT_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
                return;;
            }
            //===========================================================================================================================================================================
            //===========================================================================================================================================================================


            AutoPlanningTool.GreetingsWindow usercontrol = new AutoPlanningTool.GreetingsWindow(context);
            Window windows = new Window();
            windows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windows.Height = 676;
            windows.Width = 800;
            windows.Content = usercontrol;
            windows.ShowDialog();
            windows.SizeToContent = SizeToContent.WidthAndHeight;
            windows.Title = "Auto planning tool";
            windows.Style = default;
        }
        
        public static ScriptContext GetScriptContext()
        { 
            return(context);
        }
        
    }
}