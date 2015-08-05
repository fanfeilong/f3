<Query Kind="Program">
  <Reference Relative="..\lib\ClosedXML_v0.76.0.0\ClosedXML.dll">D:\dev\code_git_me\flinq\lib\ClosedXML_v0.76.0.0\ClosedXML.dll</Reference>
  <Reference Relative="..\lib\DocumentFormat.OpenXml.2.5\lib\DocumentFormat.OpenXml.dll">D:\dev\code_git_me\flinq\lib\DocumentFormat.OpenXml.2.5\lib\DocumentFormat.OpenXml.dll</Reference>
  <Namespace>ClosedXML.Excel</Namespace>
  <Namespace>DocumentFormat.OpenXml</Namespace>
</Query>

void Main()
{
	var xls = @"C:\Users\ffl\Google 云端硬盘\lab\构建之法-齐鲁工业大学\团队项目评分.xlsx";
	var sheet = "Sheet1";
	xls.ToMd(sheet).Dump();
}

// Define other methods and classes here
public static class ExcelExtension{
	public static string ToMd(this string xls,string sheet){
		var workBook = new XLWorkbook(xls);
		var worksheet = workBook.Worksheet(sheet);
		var excelRows = worksheet.RowsUsed().Select((row,index)=>new {Row=row,Index=index});
		
		var mdRows =
		from xrow in excelRows
		let xr = xrow.Row
		let i  = xrow.Index
		let mr = 
			from xc in xr.Cells()
			let v = xc.HasHyperlink 
				? string.Format("[{0}]({1})",xc.Value,xc.Hyperlink.ExternalAddress.AbsoluteUri)
				: xc.Value
			select string.Format("{0}|",v)
		let mrr = i==0
			? string.Format("|{0}\r\n|{1}\r\n",mr.Aggregate((a,b)=>a+b),mr.Aggregate("",(s,a)=>s+":--:|"))
			: string.Format("|{0}\r\n",mr.Aggregate((a,b)=>a+b))
		select mrr;
		
		var mt = mdRows.Aggregate((a,b)=>a+b);
		return mt.ToString();
	}
}