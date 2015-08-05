<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main() {
    dynamic table=null;
    table = new {
        Name = "fanfeilong",
        Id = 2,
        Infos =  new {
            Color = ConsoleColor.Black,
            logs = new List<string> {
                "nihao",
                "nice to meet you"
            }
        },
        ToString =new Func<string>(() => {
            var self = table;
            var sb = new StringBuilder();
            sb.AppendFormat("Name:{0}\r\n",self.Name)
              .AppendFormat("Id:{0}\r\n", self.Id);
            foreach (var log in self.Infos.logs) {
                sb.AppendLine(log);
            }
            return sb.ToString();
        })
    };
    
    Console.WriteLine(table.ToString());
}