using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

        private delegate void OutputRecieved(string output);
        private event OutputRecieved OnOutputRecived;

        public Action ShowOutput;

        public string output { get; private set; }
        private bool isRunning;
        private string tempLocalPath = $"D:\\CLOC";
        
        private CLOLCSystemManager()
        {
            isRunning = false;
            OnOutputRecived += CLOLCSystemManager_OnOutputRecived;
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

            string command = $"clone {repository} {tempLocalPath}\\{timestamp}";

            RunGit(command, completed);
        }
                

        private void RunGit(string command, Action completed)
        {
            string gitPath = "C:/Program Files/Git/git-bash.exe";
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

            completed?.Invoke();
        }

        public async Task StartRepositoryCount(string path)
        {
            string clocEXEPath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\_External\\cloc-1.88.exe";

            if (!isRunning)
            {
                isRunning = true;
                GetRepositoryProject(path, () =>
                {
                    Debug.WriteLine($"Tring to read lines report!");

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.WorkingDirectory = "d:\\";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = clocEXEPath;
                    process.StartInfo.Arguments = $"cloc-1.88.exe {tempLocalPath}";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.Start();
                    
                    string error = process.StandardError.ReadToEnd();
                    var exitCode = process.ExitCode;
                    string result = process.StandardOutput.ReadToEnd();

                    Debug.WriteLine($" ERROR: {error}");
                    Debug.WriteLine($" EXIT CODE    : {exitCode}");

                    process.Close();

                    OnOutputRecived?.Invoke(result);
                    //ClearTempRepository(() => { ClearDirectory(tempLocalPath); });                       
                    ClearDirectory(tempLocalPath);
                });

                isRunning = false;
            }
        }

        private void ClearTempRepository(Action completed)
        {
            string comand = $" rm -rf .git*";
            RunGit(comand, completed);
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
