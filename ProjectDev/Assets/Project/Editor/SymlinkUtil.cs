using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

public class Symlink
{
	public string Name { get; set; }
	public string Target { get; set; }
}

public class SymlinkUtil {

	public static IEnumerable<Symlink> GetAllSymLinks(string workingDir)
	{
		Process converter = new Process();
		converter.StartInfo = new ProcessStartInfo("cmd", "/c dir /Al") { 
			RedirectStandardOutput = true, 
			UseShellExecute = false, 
			CreateNoWindow = true, 
			WorkingDirectory = workingDir
		};

		string output = "";
		converter.OutputDataReceived += (sender, e) =>
		{
			output += e.Data + "\r\n";
		};
		converter.Start();
		converter.BeginOutputReadLine();
		converter.WaitForExit();

		Regex regex = new Regex(@"\n.*\<SYMLINKD\>\s(.*)\s\[(.*)\]\r");

		var matches = regex.Matches(output);
		foreach (Match match in matches)
		{
			var name = match.Groups[1].Value.Trim();
			var target = match.Groups[2].Value.Trim();
			Console.WriteLine("Symlink: " + name + " --> " + target);

			yield return new Symlink() { Name = name, Target = target };
		}
	}

	//mklink /d link target
	//ln -n exsisting_thing new_thing (in man ln:  ln -s source_file target_file, has inverse 'target' definition with windows)
	//In the following codes, we choose the windows definitions
	public static bool ContainSymlink(string workingDir, string symlink, string symlinkTarget) {
		foreach(Symlink link in GetAllSymLinks (workingDir)) {
			if (link.Name.Equals (symlink) && link.Target.Equals (symlinkTarget)) {
				return true;
			}
		}
		return false;
	}

	private static bool RunProcess(string workingDir, string fileName, string arguments) {
		try {
			Process myProcess = new Process();
			myProcess.StartInfo = new ProcessStartInfo(fileName, arguments) {
				RedirectStandardOutput = true, 
				UseShellExecute = false, 
				CreateNoWindow = true, 
				WorkingDirectory = workingDir
			};
			//myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			//myProcess.EnableRaisingEvents = true;

			myProcess.Start();
			myProcess.WaitForExit();
			int exitCode = myProcess.ExitCode;
			UnityEngine.Debug.LogFormat("{0} {1} result: {2}", fileName, arguments, exitCode == 0 ? "OK" : "Failed");
			return exitCode == 0;
		} catch (Exception e){
			UnityEngine.Debug.LogException (e);
		}
		return false;
	}

	public static bool CreateSymlink(string workingDir, string symlink, string symlinkTarget) {
		#if UNITY_EDITOR_WIN
		string fileName = "cmd.exe";
		string arguments = "/c mklink /d " + symlink + " " + symlinkTarget;
		#endif

		#if UNITY_EDITOR_OSX
		string fileName = "bash";
		string arguments = "-c ln -s " + symlinkTarget + " " + symlink;
		#endif

		return RunProcess(workingDir, fileName, arguments);
	}


	public static bool RemoveSymlink(string workingDir, string symlink) {
		#if UNITY_EDITOR_WIN
		string fileName = "cmd.exe";
		string arguments = "/c rmdir " + symlink;
		#endif

		#if UNITY_EDITOR_OSX
		string fileName = "bash";
		string arguments = "-c rm " + symlink;
		#endif

		return RunProcess(workingDir, fileName, arguments);
	}
}
