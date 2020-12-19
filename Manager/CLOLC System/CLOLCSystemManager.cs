using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CLOC.Manager.CLOLC_System
{
    class CLOLCSystemManager
    {
        #region Singleton
        private static CLOLCSystemManager instance;
        public static CLOLCSystemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CLOLCSystemManager();                
                }

                return instance;
            }
        }

        #endregion

        public Action OnOutputRecieved;

        public string output { get; private set; }
        private bool isRunning;

        private CLOLCSystemManager()
        {
            isRunning = false;
        }

        public void StartRepositoryCount(string path)
        {
            if (!isRunning)
            {
                isRunning = true;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.WorkingDirectory = "d:\\";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "D:\\Projects\\CLOC_EmailSender\\CLOC\\_External\\cloc-1.88.exe";
                process.StartInfo.Arguments = $"cloc-1.88.exe {path}";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                
                process.Start();
                
                this.output = process.StandardOutput.ReadToEnd();                

                process.Close();

                OnOutputRecieved?.Invoke();
                isRunning = false;
            }
        }
    }
}
