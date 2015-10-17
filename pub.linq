<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main() {
    var notePath = @"D:\dev\code_git_me\f3\notes\";
    var pubPath  = @"D:\dev\code_git_me\f3\pub\";
    var pubPage  = @"D:\dev\code_git_me\f3\pub\index.html";
    var footPage     = @"D:\dev\code_git_me\f3\foot.md";
    
    "Begin".Dump();
    
    var foot = File.ReadAllLines(footPage);
    var noteDir = new DirectoryInfo(notePath);
    foreach (var note in noteDir.GetFiles()) {
        var fileName = Path.Combine(pubPath,note.Name+".html").Dump();
        var content = File.ReadAllLines(note.FullName);
        var output = content.Union(foot);
        File.WriteAllLines(fileName,output);
    }
    
    "End".Dump();
}

// Define other methods and classes here
