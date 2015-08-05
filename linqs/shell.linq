<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main()
{
	
}

// Define other methods and classes here
public static class Shell{
	/*执行Python脚本文件*/
	public static string[] PythonFile(string scriptFile){
		return Util.Cmd(string.Format("python {0}",scriptFile));
	}
	
	/*执行MSBuild命令*/
	public static string[] MSBuild(string args){
		var msbuild = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MsBuild.exe";
		return Util.Cmd(string.Format("{0} {1}",msbuild,args));
	}
	
	/*复制目录，不覆盖文件*/
	public static void NonOverWriteXCopy(string SourcePath,string DestinationPath){
		//创建所有子目录
		foreach (string dirPath in Directory.GetDirectories(SourcePath, "*.*", SearchOption.AllDirectories)){
			if(!Directory.Exists(DestinationPath)){
				Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
			}
		}
		
		//拷贝所有文件
		foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories)){
			if(!File.Exists(newPath)){
    			File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath),false);
			}
		}
	}
	
	
	/*复制目录，覆盖文件夹和文件*/
	public static void OverWriteXCopy(string SourcePath,string DestinationPath){
		//创建所有子目录
		foreach (string dirPath in Directory.GetDirectories(SourcePath, "*.*", SearchOption.AllDirectories)){
			if(!Directory.Exists(DestinationPath)){
				Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
			}
		}
		
		//拷贝所有文件
		foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories)){
    		File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath),true);
		}
	}
	
	/*只复制文件，不覆盖文件*/
	public static void NonOverWriteXCopyOnlyFile(string SourcePath,string DestinationPath){
		//拷贝所有文件
		foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories)){
			if(!File.Exists(newPath)){
    			File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath),false);
			}
		}
	}
	
	/*执行svn*/
	public static string[] Svn(string args){
		return Util.Cmd(string.Format(@"C:\Program Files (x86)\VisualSVN Server\bin\svn.exe {0}",args));
	}
	
	/*git导出*/
	public static string[] GitExport(string src,string zipTemp,string dest){
		Environment.CurrentDirectory = src;
		Environment.CurrentDirectory.Dump();
		var gitcmd = "git archive --format zip --output={1} master";
		var gitInfo = Util.Cmd(string.Format(gitcmd,src,zipTemp));
		//ZipFile.ExtractToDirectory(zipTemp,dest);
		return gitInfo;
	}
	
	public static string[] GitClean(string src){
		Environment.CurrentDirectory = src.Dump();
		var gitcmd = "git clean -fx \"\"";
		var gitInfo = Util.Cmd(gitcmd);
		//ZipFile.ExtractToDirectory(zipTemp,dest);
		return gitInfo;
	}
}
