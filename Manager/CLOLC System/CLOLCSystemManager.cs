using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

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
        private string tempLocalPath = $"D:\\CLOC";
        private CLOLCSystemManager()
        {
            isRunning = false;
        }

        private void GetRepositoryProject(string repository, Action completd)
        {
            string gitPath = "C:/Program Files/Git/git-bash.exe";
            string filename = Directory.Exists(gitPath) ? gitPath : "git.exe";
            string command = $"clone {repository} {tempLocalPath}";

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

            completd?.Invoke();
        }

        public void StartRepositoryCount(string path)
        {
            if (!isRunning)
            {
                isRunning = true;
                GetRepositoryProject(path, () =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.WorkingDirectory = "d:\\";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "D:\\Projects\\CLOC_EmailSender\\CLOC\\_External\\cloc-1.88.exe";
                    process.StartInfo.Arguments = $"cloc-1.88.exe {tempLocalPath}";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.Start();

                    this.output = process.StandardOutput.ReadToEnd();

                    process.Close();

                    OnOutputRecieved?.Invoke();
                });

                isRunning = false;
            }
        }

        public void SendReportToEmail(string email, string body)
        {
            string sender = "tayna-ct12@hotmail.com";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sender, "password"),
                EnableSsl = true
            };
            try
            {
                client.Send(sender, email, "test", "testbody");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
            }
        }
    }
}
