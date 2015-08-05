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
public static class IOExtension{
	/*获取文件下指定模式的文件名，并排除某个文件*/
	public static IEnumerable<string> GetFileNames(this DirectoryInfo info,string pattern,string eclude=""){
		var fileNames = info.GetFiles(pattern).Select(f=>f.FullName);
		if(eclude!=""){
			fileNames = fileNames.Where(f=>Path.GetFileName(f)!=eclude);
		}
		return fileNames;
	}
	
	public static IEnumerable<string> GetFileNames(this string dir,string pattern,string eclude=""){
		return GetFileNames(new DirectoryInfo(dir),pattern,eclude);
	}
	
	public static IEnumerable<string> GetAllFileNames(this string dir,string pattern,string eclude=""){
		var info = new DirectoryInfo(dir);
		var fileNames = info.GetFiles(pattern, SearchOption.AllDirectories).Select(f=>f.FullName);
		if(eclude!=""){
			fileNames = fileNames.Where(f=>Path.GetFileName(f)!=eclude);
		}
		return fileNames;
	}
	
	public static IEnumerable<string> GetTopFileNames(this string dir,string pattern,string eclude=""){
		var info = new DirectoryInfo(dir);
		var fileNames = info.GetFiles(pattern, SearchOption.TopDirectoryOnly).Select(f=>f.FullName);
		if(eclude!=""){
			fileNames = fileNames.Where(f=>Path.GetFileName(f)!=eclude);
		}
		return fileNames;
	}
	
	public static IEnumerable<string> ShortNames(this IEnumerable<string> fileNames){
		foreach(var f in fileNames)
			yield return Path.GetFileName(f);
	}
	
	public static string ShortName(this string f){
		return Path.GetFileName(f);
	}
	
	/*批量获取文件夹列表里指定模式的文件名，并排除某个文件*/
	public static IEnumerable<string> GetFileNames(this IEnumerable<DirectoryInfo> infos,string pattern,string eclude=""){
		var fileNames=infos.SelectMany(info=>info.GetFileNames(pattern,eclude));
		return fileNames;
	}
	
	/*将文件列表分别拷贝到指定目录列表*/
	public static void CopyTo(this IEnumerable<string> files,IEnumerable<string> dirs){
		foreach(var dir in dirs){
			if(Directory.Exists(dir)){
				foreach(var file in new DirectoryInfo(dir).GetFiles()){
					file.Delete();
				}
			}else{
				Directory.CreateDirectory(dir);
			}
		}
		files.AsParallel().ForAll(file=>{
			var fileName = Path.GetFileName(file);
			dirs.AsParallel().ForAll(dir=>{
				var destFile = Path.Combine(dir,fileName).Dump();
				File.Copy(file,destFile,true);
			});
		});
	}
	
	/*递归删除空文件夹*/
	public static bool DeleteEmptySubDirectory(this string target){
		var dirInfo = new DirectoryInfo(target);
		var subDirs = dirInfo.GetDirectories();
		var subFiles = dirInfo.GetFiles();
		if(subDirs.Count()==0&&subFiles.Count()==0){
			dirInfo.Name.Dump("删除空文件夹：");
			dirInfo.Delete();
			return false;
		}
		
		if(subDirs.Count()>0){
			bool is_empty = true;
			foreach(var subDir in subDirs){
				if(DeleteEmptySubDirectory(subDir.FullName)){
					is_empty = false;
				}
			}
			if(is_empty&&subFiles.Count()==0){
				dirInfo.FullName.Dump("在删除空子文件夹后，父文件夹成为空文件夹，删除之：");
				dirInfo.Delete();
				return false;
			}
			return true;
		}
		return true;
	}
	
	public static void CopyDirTo(this string sourceDirName, string destDirName, bool copySubDirs,bool overwrite){
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists){
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        // If the destination directory doesn't exist, create it. 
        if (!Directory.Exists(destDirName)){
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files){
            string temppath = Path.Combine(destDirName, file.Name);
			if(overwrite){
				var tempFile = new FileInfo(temppath);
				tempFile.Attributes = FileAttributes.Normal;
			}
			try{
            	file.CopyTo(temppath, overwrite);
			}catch(Exception e){
				e.Message.Dump("exception");
			}
        }

        // If copying subdirectories, copy them and their contents to new location. 
        if (copySubDirs){
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                CopyDirTo(subdir.FullName, temppath, copySubDirs,overwrite);
            }
        }
    }
	
	public static void DeleteDirectory(this string target_dir){
        Util.Cmd(string.Format("rd /s /q {0}",target_dir));
    }
	
	public static long GetDirectoryLength(this string dirPath){
		//判断给定的路径是否存在,如果不存在则退出
		if (!Directory.Exists(dirPath))
			return 0;
		long len = 0;
		
		//定义一个DirectoryInfo对象
		DirectoryInfo di = new DirectoryInfo(dirPath);
		
		//通过GetFiles方法,获取di目录中的所有文件的大小
		foreach (FileInfo fi in di.GetFiles()){
			len += fi.Length;
		}
		
		//获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
		DirectoryInfo[] dis = di.GetDirectories();
		if (dis.Length > 0){
			for (int i = 0; i < dis.Length; i++)
			{
				len += GetDirectoryLength(dis[i].FullName);
			}
		}
		return len;
	}
}
