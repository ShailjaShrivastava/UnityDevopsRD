using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.IO;

public class PostBuildScript
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string dropboxToken = System.Environment.GetEnvironmentVariable("DROPBOX_ACCESS_TOKEN");
        if (string.IsNullOrEmpty(dropboxToken))
        {
            UnityEngine.Debug.LogError("Dropbox access token is not set.");
            return;
        }

        string dropboxPath = "/builds/build.zip";
        string localFilePath = pathToBuiltProject;

        Process process = new Process();
        process.StartInfo.FileName = "curl";
        process.StartInfo.Arguments = $"-X POST https://content.dropboxapi.com/2/files/upload " +
                                      $"--header \"Authorization: Bearer {dropboxToken}\" " +
                                      $"--header \"Dropbox-API-Arg: {{\\\"path\\\": \\\"{dropboxPath}\\\", \\\"mode\\\": \\\"add\\\", \\\"autorename\\\": true, \\\"mute\\\": false, \\\"strict_conflict\\\": false}}\" " +
                                      $"--header \"Content-Type: application/octet-stream\" " +
                                      $"--data-binary @{localFilePath}";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        UnityEngine.Debug.Log("Dropbox Upload Output: " + output);
        UnityEngine.Debug.LogError("Dropbox Upload Error: " + error);
    }
}
