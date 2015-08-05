<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main()
{
	PdfConvert.ConvertHtmlToPdf(new PdfDocument { 
    	Url = "file:///D:/dev/code_git_me/flinq/linq/statistic.html",
    	HeaderLeft = "[title]",
    	HeaderRight = "[date] [time]",
    	FooterCenter = "Page [page] of [topage]"
	}, new PdfOutput {
    	OutputFilePath = "wkhtmltopdf-page.pdf"
	});
}

// Define other methods and classes here
