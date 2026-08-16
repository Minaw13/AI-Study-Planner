using System.Diagnostics;

namespace AIStudyPlanner.Services
{
    public class PythonAIProcessManager : IDisposable
    {
        private Process? _pythonProcess;

        private readonly string _pythonScript;

        public PythonAIProcessManager(
            IWebHostEnvironment environment)
        {
            _pythonScript = Path.Combine(
                environment.ContentRootPath,
                "PythonAI",
                "study_planner.py"
            );
        }

        public void Start()
        {
            if (_pythonProcess != null &&
                !_pythonProcess.HasExited)
            {
                return;
            }

            if (!File.Exists(_pythonScript))
            {
                throw new FileNotFoundException(
                    "Python AI Engine script was not found.",
                    _pythonScript
                );
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{_pythonScript}\"",

                WorkingDirectory =
                    Path.GetDirectoryName(_pythonScript)!,

                UseShellExecute = false,
                CreateNoWindow = true,

                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _pythonProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            _pythonProcess.OutputDataReceived +=
                (_, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        Console.WriteLine(
                            $"[Python AI] {e.Data}"
                        );
                    }
                };

            _pythonProcess.ErrorDataReceived +=
                (_, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        Console.WriteLine(
                            $"[Python AI] {e.Data}"
                        );
                    }
                };

            _pythonProcess.Start();

            _pythonProcess.BeginOutputReadLine();
            _pythonProcess.BeginErrorReadLine();
        }

        public void Dispose()
        {
            try
            {
                if (_pythonProcess != null &&
                    !_pythonProcess.HasExited)
                {
                    _pythonProcess.Kill(
                        entireProcessTree: true
                    );

                    _pythonProcess.WaitForExit(3000);
                }
            }
            catch
            {
                // Ignore shutdown errors.
            }

            _pythonProcess?.Dispose();
            _pythonProcess = null;
        }
    }
}