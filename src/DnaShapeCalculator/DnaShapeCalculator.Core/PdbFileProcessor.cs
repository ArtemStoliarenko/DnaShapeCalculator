using System;
using System.Collections.Generic;
using System.Text;

namespace DnaShapeCalculator.Core
{
	using System;
	using System.Diagnostics;
	using System.IO;

	public sealed class PdbFileProcessor
	{
		private const string findPairProcessName = "find_pair";
		private const string findPairCurArgumentName = "-c+";
		private const string curvesProcessName = "Cur+";

		private const string tempResultFilename = "temp.fp";
		private const string helRegionsFilename = "hel_regions.pdb";
		private const string helRegionRenamed = "helices.pdb";
		private const string curvesInputFilename = "in.cur";
		private const string curvesResultFilename = "helices.lis";
		private const string curvesResultExtension = ".lis";

		private const int processWaitTimeInMilliseconds = 10000;

		private static readonly string[] standardLibFilenames = { "standard_b.lib", "standard_i.lib", "standard_s.lib" };

		private readonly FileInfo pdbFile;
		private readonly string resultDirectoryPath;

		private PdbFileProcessor(string resultDirectoryPath)
		{
			if (string.IsNullOrEmpty(resultDirectoryPath) || !Directory.Exists(resultDirectoryPath))
			{
				throw new ArgumentException(nameof(resultDirectoryPath));
			}

			this.resultDirectoryPath = resultDirectoryPath;
		}

		public PdbFileProcessor(string pdbFilename, string resultDirectoryPath)
			: this(resultDirectoryPath)
		{
			if (string.IsNullOrEmpty(pdbFilename))
			{
				throw new ArgumentException(nameof(pdbFilename));
			}

			this.pdbFile = new FileInfo(pdbFilename);
		}

		public PdbFileProcessor(FileInfo pdbFile, string resultDirectoryPath)
			: this(resultDirectoryPath)
		{
			this.pdbFile = pdbFile ?? throw new ArgumentNullException(nameof(pdbFile));
		}

		public bool ProcessFile()
		{
			bool runCleanup = true;
			try
			{
				var tempFile = GetTempFileInfo();
				FindDnaShape(tempFile);
				GetResult(tempFile.Directory, Path.GetFileNameWithoutExtension(tempFile.Name));

				return true;
			}
			catch (Exception)
			{
				runCleanup = false;
				return false;
			}
			finally
			{
				if (runCleanup)
				{
					Cleanup();
				}
			}
		}

		private FileInfo GetTempFileInfo()
		{
			var tempDirectory = GetTempDirectoryName();

			PrepareDirectory(tempDirectory);
			var newFilename = Path.Combine(tempDirectory, pdbFile.Name);
			return pdbFile.CopyTo(newFilename);
		}

		private void PrepareDirectory(string tempDirectory)
		{
			if (Directory.Exists(tempDirectory))
			{
				Directory.Delete(tempDirectory, true);
			}
			Directory.CreateDirectory(tempDirectory);

			var currentDirectory = Environment.CurrentDirectory;
			foreach (var filename in standardLibFilenames)
			{
				File.Copy(Path.Combine(currentDirectory, filename), Path.Combine(tempDirectory, filename));
			}
		}

		private bool FindDnaShape(FileInfo file)
		{
			bool result = true;
			string output = string.Empty;

			(result, output) = RunProcess(findPairProcessName, $"{file.Name} {tempResultFilename}", file.DirectoryName);
			if (!ValidateResult(result, output))
			{
				return false;
			}
			File.Move(Path.Combine(file.DirectoryName, helRegionsFilename), Path.Combine(file.DirectoryName, helRegionRenamed));

			(result, output) = RunProcess(findPairProcessName, $"{findPairCurArgumentName} {helRegionRenamed} {curvesInputFilename}", file.DirectoryName);
			if (!ValidateResult(result, output))
			{
				return false;
			}

			var curvesInputFile = new FileInfo(Path.Combine(file.DirectoryName, curvesInputFilename));
			(result, output) = RunProcess(curvesProcessName, string.Empty, file.DirectoryName, inputFile: curvesInputFile);
			if (!ValidateResult(result, output))
			{
				return false;
			}

			return result;
		}

		private static bool ValidateResult(bool result, string output) => result && (output == null || !output.Contains("no base-pair"));

		private void GetResult(DirectoryInfo directory, string resultFilename)
		{
			var resultFile = new FileInfo(Path.Combine(directory.FullName, curvesResultFilename));
			resultFile.CopyTo(Path.Combine(resultDirectoryPath, Path.ChangeExtension(resultFilename, curvesResultExtension)));
		}

		private void Cleanup()
		{
			Directory.Delete(GetTempDirectoryName(), true);
		}

		private string GetTempDirectoryName() => Path.GetFileNameWithoutExtension(pdbFile.Name);

		private static (bool success, string output) RunProcess(string executableName, string arguments, string workingDirectory, FileInfo inputFile = null)
		{
			var processStartInfo = new ProcessStartInfo(executableName, arguments)
			{
				RedirectStandardInput = !(inputFile == null),
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = workingDirectory,
			};

			using (var process = Process.Start(processStartInfo))
			{
				if (inputFile != null)
				{
					process.StandardInput.Write(File.ReadAllText(inputFile.FullName));
				}
				process.WaitForExit(processWaitTimeInMilliseconds);

				return (process.HasExited, $"{process.StandardOutput.ReadToEnd()} {process.StandardError.ReadToEnd()}");
			}
		}

		private static bool IsMessageContainsNoBasePairsError(string output)
		{
			const string noBasePairErrorMessage = "no base-pair";

			return !string.IsNullOrEmpty(output) && output.Contains(noBasePairErrorMessage);
		}
	}
}
