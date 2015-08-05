<Query Kind="Program">
  <Output>DataGrids</Output>
  <Reference Relative="..\lib\ClosedXML_v0.76.0.0\ClosedXML.dll">D:\dev\code_git_me\flinq\lib\ClosedXML_v0.76.0.0\ClosedXML.dll</Reference>
  <Reference Relative="..\lib\DocumentFormat.OpenXml.2.5\lib\DocumentFormat.OpenXml.dll">D:\dev\code_git_me\flinq\lib\DocumentFormat.OpenXml.2.5\lib\DocumentFormat.OpenXml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>ClosedXML.Excel</Namespace>
  <Namespace>DocumentFormat.OpenXml</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main()
{
	var dir = @"D:\dev\code_git_me\flinq\linq\广州商学院";
	var blog1 = @"D:\dev\code_git_me\flinq\linq\广州商学院班级1.xlsx";
	var blog2 = @"D:\dev\code_git_me\flinq\linq\广州商学院班级2.xlsx";
	
	var classDir1 = @"D:\dev\code_git_me\flinq\linq\广州商学院\班级1";
	var classDir2 = @"D:\dev\code_git_me\flinq\linq\广州商学院\班级2";
	if(!Directory.Exists(classDir1)){
		Directory.CreateDirectory(classDir1);
	}

	if(!Directory.Exists(classDir2)){
		Directory.CreateDirectory(classDir2);
	}
	
	var blogs1 = FetchBlogItems(blog1);
	var blogs2 = FetchBlogItems(blog2).Dump();
	
	var dirInfo = new DirectoryInfo(dir);
	foreach(var file in dirInfo.GetFiles()){
		var name = file.Name.Replace("SE_skill_survey_","").Replace(".xlsx","").Trim();
		if(blogs1.ContainsKey(name)){
			var item = blogs1[name];
			var newFile = Path.Combine(classDir1,string.Format("{0} {1}.xlsx",item.Id,item.Name));
			file.MoveTo(newFile);
			//newFile.Dump();
		}else if(blogs2.ContainsKey(name)){
			var item = blogs2[name];
			var newFile = Path.Combine(classDir2,string.Format("{0} {1}.xlsx",item.Id,item.Name));
			file.MoveTo(newFile);
			//newFile.Dump();
		}else{
			("error"+name).Dump();
		}
	}
}

// Define other methods and classes here
public class BlogItem{
	public string Id{get;set;}
	public string Name{get;set;}
	public string Url{get;set;}
}

public Dictionary<string,BlogItem> FetchBlogItems(string file){
	var linkTableBook = new XLWorkbook(file);
	var linkSheet = linkTableBook.Worksheets.ElementAt(0);
	var linkRows= linkSheet.RowsUsed();
	var links = new Dictionary<string,BlogItem>();
	foreach(var row in linkRows){
		var cells = row.CellsUsed();
		try{
			var id = cells.ElementAt(0).Value.ToString().Trim();
			var name = cells.ElementAt(1).Value.ToString().Trim();
			var blog = cells.ElementAt(2).Hyperlink.ExternalAddress.AbsoluteUri;
			links.Add(name,new BlogItem(){Id=id,Name=name,Url=blog});
		}catch{
			//Ignore
		}
	}
	return links;
}

