using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

namespace CLOC.Manager.CLOLC_System
{
    class CLOCSystemManager
    {
        #region Singleton
        private static CLOCSystemManager instance;
        public static CLOCSystemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CLOCSystemManager();                
                }

                return instance;
            }
        }

        #endregion

        private delegate void OutputRecieved(string output);
        private delegate void OnRepositoryCloningComplete();

        //private event OutputRecieved OnOutputRecived;

        public Action ShowOutput;

        public string output { get; private set; }
        private bool isRunning;
        private string tempLocalPath;
        
        private CLOCSystemManager()
        {
            isRunning = false;
            tempLocalPath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\_Repositories";
           // OnOutputRecived += CLOLCSystemManager_OnOutputRecived;
        }

        private void CLOLCSystemManager_OnOutputRecived(string output)
        {
            this.output = output;
            ShowOutput?.Invoke();
        }

        private void GetRepositoryProject(string repository, Action completed)
        {
            TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            string timestamp = span.TotalSeconds.ToString();
            timestamp.Replace(',', '-'); 

            string command = $"clone {repository} {tempLocalPath}\\{timestamp}";

            RunGit(command, completed);
        }

        private void RunGit(string command, Action completed)
        {
            try
            {
                string gitPath = "C:\\Program Files\\Git\\cmd\\git.exe";
                string filename = Directory.Exists(gitPath) ? gitPath : "git.exe";

                ProcessStartInfo processStartInfo = new ProcessStartInfo(filename, command)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(processStartInfo);
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                var exitCode = process.ExitCode;

                Debug.WriteLine($" OUTPUT: {output}");
                Debug.WriteLine($" ERROR: {error}");
                Debug.WriteLine($" EXIT CODE    : {exitCode}");

                process.Close();
            }
            catch (Exception e)
            {

                Debug.WriteLine($"{GetType().Name} -RunGit: {e.ToString()} ");
            }

            completed?.Invoke();
        }

        public void StartRepositoryCount(string path)
        {           
            if (!isRunning)
            {
                isRunning = true;
                //Thread th = new Thread(() =>
                //{
                    GetRepositoryProject(path, () =>
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke(new OnRepositoryCloningComplete(AnalyzeRepository), DispatcherPriority.Render, new object[] {  });

                        // AnalyzeRepository();
                    });

                    isRunning = false;
                //});

                //th.IsBackground = true;
                //th.Start();
            }
        }

        private void AnalyzeRepository()
        {
            //Thread th = new Thread(() =>
            //{
                string clocEXEPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "_External", "cloc-1.88.exe");

                Debug.WriteLine($"Cloc path: {clocEXEPath}");

                Debug.WriteLine($"Trying to read lines report!");

                ProcessStartInfo processStartInfo = new ProcessStartInfo(clocEXEPath, $"cloc-1.88.exe {tempLocalPath}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(processStartInfo);
                process.WaitForExit();

                string error = process.StandardError.ReadToEnd();
                var exitCode = process.ExitCode;
                string result = process.StandardOutput.ReadToEnd();

                Debug.WriteLine($" ERROR: {error}");
                Debug.WriteLine($" EXIT CODE    : {exitCode}");

                process.Close();

                Dispatcher.CurrentDispatcher.BeginInvoke(new OutputRecieved(CLOLCSystemManager_OnOutputRecived), DispatcherPriority.Render, new object[] { result });

                ClearDirectory(tempLocalPath);
            //});

            //th.IsBackground = true;
            //th.Start();
        }
            
        private void ClearDirectory(string directorypath)
        {
            if (Directory.Exists(directorypath))
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(directorypath);

                    foreach (FileInfo fi in dir.GetFiles())
                    {
                        fi.Attributes = FileAttributes.Normal;
                        fi.Delete();
                    }

                    foreach (DirectoryInfo di in dir.GetDirectories())
                    {
                        ClearDirectory(di.FullName);
                        di.Delete();
                    }
                }
                catch (Exception e) {

                    Debug.WriteLine($"{e.ToString()}");
                }
            }
        }        
    }
}
