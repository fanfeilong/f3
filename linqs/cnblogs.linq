<Query Kind="Program">
  <Reference Relative="..\lib\CsQuery.1.3.4\lib\net40\CsQuery.dll">D:\dev\code_git_me\flinq\lib\CsQuery.1.3.4\lib\net40\CsQuery.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Framework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Tasks.v4.0.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Utilities.v4.0.dll</Reference>
  <Reference Relative="..\lib\Newtonsoft.Json.6.0.8\lib\net40\Newtonsoft.Json.dll">D:\dev\code_git_me\flinq\lib\Newtonsoft.Json.6.0.8\lib\net40\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ComponentModel.DataAnnotations.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Design.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.Protocols.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.EnterpriseServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Caching.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceProcess.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.ApplicationServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.RegularExpressions.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.Services.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.XML.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.Linq.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.XDocument.dll</Reference>
  <Namespace>CsQuery</Namespace>
  <Namespace>CsQuery.Engine</Namespace>
  <Namespace>CsQuery.HtmlParser</Namespace>
  <Namespace>CsQuery.Output</Namespace>
  <Namespace>CsQuery.Utility</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Web</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
  <Namespace>System.Xml.Xsl</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

void Main(){
	// 说明
	// 本linq文件请使用LinqPad程序执行
	// 依赖的dll库请在LinqPad的菜单：Query->QueryProperites->Browser 里添加
	// 名称空间请在Query->QueryProperties->Addtional Namespace Import里添加，右侧可以鼠标点选
	// 
	// 本linq依赖的库有:
	// Newtonsoft.Json.Net.dll:http://www.newtonsoft.com/json
	// CsQuery.dll:https://github.com/jamietre/CsQuery
	
	var linqDir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var rootDir = Path.GetDirectoryName(linqDir);
	var dataDir = Path.Combine(rootDir,".\\data");
	
	// Json文件的路径，请修改为合适的路径
	var savePath = Path.Combine(dataDir,".\\cnblog-software-classes.json");
	
	// 创建CNBlog的软工教学班分析类
	var ca = new CNBlogSoftareClassAnalyzer(savePath);
	
	// 下载数据，并保存到savePath
	//var userName = "";
	//var passWord = "";
	//var cnBlog = new CNBlog(userName,passWord).Login();
	//cnBlog.FetchPosts("qluZhao").Select(p=>p.Title).Dump();
	//ca.Connect(cnBlog).List().Fetch().Save();
	
	// 从Json文件加载
	var sc = ca.Load().SoftwareClass;
	
	// get student's feeds example
	//sc.ListStudentComments(20,8);
	
	// get teacher's feeds example
	//sc.ListTeacherComments(20,8);
	//sc.ListTeacherPosts(100,40);
    
    var totalPostCount = 0;
    foreach(var c in sc.Classes){
        foreach(var s in c.Students){
            foreach(var p in s.Posts){
                totalPostCount++;
            }
        }
    }
	
    var query = 
    from c in sc.Classes
    from s in c.Students
    from p in s.Posts
    select p.Text.ExtractParagraphHeaderLines();
    
    var dict  = query.Flat().TrainingFDIDF(totalPostCount);
    var fdidfs = query.Take(2).Select(p=>p.CalculateFDIDF(dict));
}

//***************************************************/
// SearchEnginer
//***************************************************/
public static class SerachAlgorithm{
    static string GetMd5Hash(this string input,MD5 md5Hash){
        // Convert the input string to a byte array and compute the hash. 
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes 
        // and create a string.
        var sb = new StringBuilder();

        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string. 
        for (int i = 0; i < data.Length; i++){
            sb.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string. 
        return sb.ToString();
    }
    public static int Cos(this string left, string right){
        // split
        return 0;
    }
    public static List<string> ExtractParagraphHeaderLines(this string t){
        var c = CQ.Create(t);
        var paragraphis = c.Select("p");
        
        using (MD5 md5Hash = MD5.Create()){
            var query = 
                from p in paragraphis
                let value = p.HtmlDecodeInnerText()
                where !string.IsNullOrWhiteSpace(value)
                let index = value.IndexOf("，")
                where index!=-1
                select value.Substring(0,index).GetMd5Hash(md5Hash);
                
            return query.ToList();
        }
    }
    public static IEnumerable<T> Flat<T>(this IEnumerable<IEnumerable<T>> list){
        foreach(var i in list){
            foreach(var j in i){
                yield return j;
            }
        }
    }
    public static Dictionary<string,double> TrainingFDIDF(this IEnumerable<string> posts,int N){
        // statistic document frequency 
        var count = new Dictionary<string,int>();
        foreach(var post in posts){
            if(count.ContainsKey(post)){
                count[post]++;
            }else{
                count.Add(post,1);
            }  
        };
        
        // calculate FD-IDF Value
        var tfIdf = new Dictionary<string,double>();
        var sum = count.Max(a=>a.Value);
        foreach(var c in count){
            double f = c.Value*1.0/sum;
            double value = Math.Log(1+N/c)*f;
            tfIdf.Add(c.Key,value);
        }
        return tfIdf;
    }
    
    public static IEnumerable<double> CalculateFDIDF(this IEnumerable<string> posts,Dictionary<string,double> tfIdfDict){
        var r = new Dictionary<string,double>();
        foreach(var s in posts){
            if(tfIdfDict.ContainsKey(s)){
                if(r.ContainsKey(s)){
                    r[s]+=tfIdfDict[s];
                }else{
                    r.Add(s,tfIdfDict[s]);
                }   
            }
        }
        
        foreach(var p in tfIdfDict){
            if(r.ContainsKey(p.Key)){
                yield return r[p.Key];
            }else{
                yield return 0;
            }
        }
    }
    
}

//***************************************************/
// CNBlog Sotware Class Analyzer
//***************************************************/
public static class CNBlogSoftareClassAnalyzerExtension{
	public static void ListStudentComments(this CNBlogClassGroup sc,int extraSeedCount,int resultCount){
		foreach(var c in sc.Classes){
			var query =
			from s in c.Students.Take(extraSeedCount)
			from f in s.Feeds.Take(extraSeedCount)
			where f.CommentBlog.Url.AbsoluteUri.Contains(s.Name)==false 
			select f;
			
			query.DistinctBy(f=>f.Author).Take(resultCount).ToMarkDown(c.Name).Dump();
		}
	}
	public static void ListTeacherComments(this CNBlogClassGroup sc,int extraSeedCount,int resultCount){
		foreach(var c in sc.Classes){
			var query = 
			from f in c.Teacher.Feeds.Take(extraSeedCount)
			where f.CommentBlog.Url.AbsoluteUri.Contains(c.Teacher.Name)==false 
			select f;
			
			query.Take(resultCount).ToMarkDown(c.Name).Dump();
		}
	}
	public static void ListTeacherPosts(this CNBlogClassGroup sc,int extraSeedCount,int resultCount){
		foreach(var c in sc.Classes){
			var query = 
			from f in c.Teacher.Posts
			where !f.Title.Contains("置顶")
			select f;
			
			query.Take(resultCount).ToMarkDown(c.Name).Dump();
		}
	}
	public static void ListAssistsPosts(this CNBlogClassGroup sc,int extraSeedCount,int resultCount){
		foreach(var c in sc.Classes){
			var query = 
			from a in c.Assists
			from f in a.Posts
			where !f.Title.Contains("置顶")
			select f;
			
			query.Take(resultCount).ToMarkDown(c.Name).Dump();
		}
	}
	public static void ListGithubs(this CNBlogClassGroup sc,int extraSeedCount,int resultCount){
		foreach(var c in sc.Classes){
			var query = 
			from f in c.Teacher.Posts
			select f;
			
			query.Take(resultCount).ToMarkDown(c.Name).Dump();
		}
	}
	public static string ToMarkDown(this IEnumerable<FeedItem> feeds,string name){
		var sb = new StringBuilder();
		sb.AppendFormat("- {0}\r\n",name).AppendLine();
		sb.AppendLine(FeedItem.GetMarkDownHeader());
		foreach(var feed in feeds){
			sb.AppendLine(feed.ToMarkDownTableLine());
		}
		sb.AppendLine();
		return sb.ToString();
	}
	public static string ToMarkDown(this IEnumerable<Post> posts,string name){
		var sb = new StringBuilder();
		sb.AppendFormat("- {0}\r\n",name).AppendLine();
		sb.AppendLine(Post.GetMarkDownHeader());
		foreach(var post in posts){
			sb.AppendLine(post.ToMarkDownTableLine());
		}
		sb.AppendLine();
		return sb.ToString();
	}
	public static IEnumerable<t> DistinctBy<t>(this IEnumerable<t> list, Func<t, object> propertySelector){
       return list.GroupBy(propertySelector).Select(x => x.First());
    }
}

public class CNBlogSoftareClassAnalyzer{
	public string SavePath{get;set;}
	private CNBlog CNBlog {get;set;}
	
	public CNBlogClassGroup SoftwareClass{get;set;}
	
	public CNBlogSoftareClassAnalyzer(string savePath){
		SavePath = savePath;
		SoftwareClass = new CNBlogClassGroup();
	}
	
	public CNBlogSoftareClassAnalyzer Connect(CNBlog cnblog){
		CNBlog = cnblog;
		var dirName = Path.GetDirectoryName(SavePath);
				
		if(!Directory.Exists(dirName)){
			try{
				Directory.CreateDirectory(dirName);
			}catch(Exception e){
				Console.WriteLine(e.Message);
				throw;
			}
		}
		
		return this;
	}
	
	public CNBlogSoftareClassAnalyzer List(){
	// 齐鲁工业大学
	var qlgyClass = CreateClass(
		"齐鲁工业大学",
		"qluZhao",
		new List<string>{
			"math",
			"xiaozhi_5638",
		},
		"http://www.cnblogs.com/qluZhao/p/4465291.html"
	);
	SoftwareClass.Classes.Add(qlgyClass);
	
	// 广州商学院
	var gzsxyClass = CreateClass(
		"广州商学院",
		"MissDu",
		new List<string>{
			"greyzeng"
		},
		"http://www.cnblogs.com/greyzeng/p/4435780.html"
	);
	SoftwareClass.Classes.Add(gzsxyClass);
	
	return this;
	}
	
	public CNBlogSoftareClassAnalyzer Fetch(){
		foreach(var c in SoftwareClass.Classes){
			c.Fetch(CNBlog);
		}
		return this;
	}
	
	public CNBlogSoftareClassAnalyzer Save(){
		File.WriteAllText(this.SavePath,JsonConvert.SerializeObject(SoftwareClass));
		return this;
	}
	
	public CNBlogSoftareClassAnalyzer Load(){
		SoftwareClass = JsonConvert.DeserializeObject<CNBlogClassGroup>(File.ReadAllText(SavePath));
		return this;
	}
	
	private CNBlogClass CreateClass(string className,string teacherName,List<string> assistNames,string studentListPage){
		var teacher = new CNBlogPerson(CNBlogRole.SoftwareTeacher,teacherName);
	
		var assits = new List<CNBlogPerson>();
		foreach(var a in assistNames){
			assits.Add(new CNBlogPerson(CNBlogRole.SoftwareAssist,a));
		}
		
		var studentNames = CNBlog.FetchStudents(studentListPage);
		var students = new List<CNBlogPerson>();
		foreach(var s in studentNames){
			students.Add(new CNBlogPerson(CNBlogRole.SoftwareStudent,s));		
		}
		
		var cnBlogClass = new CNBlogClass{
			Name = className,
			Teacher = teacher,
			Assists = assits,
			Students = students
		};
		return cnBlogClass;
	}
}

/***************************************************/
// CNBlog Online Class Modle
/***************************************************/
[JsonObject(MemberSerialization.OptIn)]
public class CNBlogClassGroup{

	[JsonProperty]
	public List<CNBlogClass> Classes{get;set;}
	
	public CNBlogClassGroup(){
		Classes = new List<CNBlogClass>();
	}
}

[JsonObject(MemberSerialization.OptIn)]
public class CNBlogClass{
	[JsonProperty]
	public string Name {get;set;}
	
	[JsonProperty]
	public CNBlogPerson Teacher{get;set;}
	
	[JsonProperty]
	public List<CNBlogPerson> Assists{get;set;}
	
	[JsonProperty]
	public List<CNBlogPerson> Students{get;set;}
	
	public CNBlogClass(){
		Assists = new List<CNBlogPerson>();
		Students = new List<CNBlogPerson>();
	}
	
	public void Fetch(CNBlog cnBlog){
		var persons = new List<CNBlogPerson>();
		persons.Add(Teacher);
		persons.AddRange(Assists);
		persons.AddRange(Students);
		persons.AsParallel().ForAll(p=>p.Fetch(cnBlog));
	}
}

public enum CNBlogRole{
	SoftwareDirector,
	SoftwareBookPublicher,
	SoftwareTeacher,
	SoftwareAssist,
	SoftwareStudent
}

[JsonObject(MemberSerialization.OptIn)]
public class CNBlogPerson{
	[JsonProperty]
	public CNBlogRole Role{get;set;}

	[JsonProperty]
	public string Name{get;set;}
	
	[JsonProperty]
	public List<FeedItem> Feeds{get;set;}
	
	[JsonProperty]
	public List<Post> Posts{get;set;}
	
	public CNBlogPerson(){
		this.Feeds = new List<FeedItem>();
		this.Posts = new List<Post>();
	}
	
	public CNBlogPerson(CNBlogRole role,string name){
		this.Role = role;
		this.Name = name;
		this.Feeds = new List<FeedItem>();
		this.Posts = new List<Post>();
	}
	
	public void Fetch(CNBlog cnBlog){
		Feeds = cnBlog.FetchFeeds(this.Name).ToList();
		Posts = cnBlog.FetchPosts(this.Name).ToList();
		Console.WriteLine(Name);
	}
}

[JsonObject(MemberSerialization.OptIn)]
public class FeedBlog{
	[JsonProperty]
	public Uri Url{get;set;}
	
	[JsonProperty]
	public string Title{get;set;}
	
	public override string ToString(){
		return string.Format("[{0}]({1})",Title,Url);
	}
}

[JsonObject(MemberSerialization.OptIn)]
public class FeedItem{
	[JsonProperty]
	public string Author{get;set;}
	
	[JsonProperty]
	public FeedBlog CommentBlog{get;set;}
	
	[JsonProperty]
	public string Date{get;set;}
	
	[JsonProperty]
	public string Comment{get;set;}
	
	public string ToMarkDownTableLine(){
		var sb = new StringBuilder();
		sb.AppendFormat("|{0}|",Date)
		  .AppendFormat("{0}|",Author)
		  .AppendFormat("{0}|",CommentBlog.ToString())
		  .AppendFormat("{0}|",Comment);
		return sb.ToString();
	}
	public static string GetMarkDownHeader(){
		return "|Date|Commenter|RefBlog|Comment|\r\n|:--|:--|:--|:--|";
	}
}

[JsonObject(MemberSerialization.OptIn)]
public class Post{
	[JsonProperty]
	public string Date{get;set;}
	
	[JsonProperty]
	public string Author{get;set;}
	
	[JsonProperty]
	public string Title {get;set;}
	
	[JsonProperty]
	public Uri Url{get;set;}
	
	[JsonProperty]
	public string Text{get;set;}

	public string ToMarkDownTableLine(){
		var sb = new StringBuilder();
		sb.AppendFormat("|{0}|",Date)
		  .AppendFormat("{0}|",Author)
		  .AppendFormat("[{0}]({1})|",Title,Url);
		return sb.ToString();
	}
	public static string GetMarkDownHeader(){
		return "|Date|Author|Blog|\r\n|:--|:--|:--|";
	}
}

/***************************************************/
// CNBlog
/***************************************************/
public static class CNBlogCQExtension{
	public static string HtmlDecodeInnerText(this CsQuery.IDomElement e){
		return HttpUtility.HtmlDecode(e.InnerText);
	}
	public static string HtmlDecodeInnerText(this CsQuery.IDomObject e){
		return HttpUtility.HtmlDecode((e as CsQuery.IDomElement).InnerText);
	}
	public static string HtmlDecodeInnerHtml(this CsQuery.IDomElement e){
		return HttpUtility.HtmlDecode(e.InnerHTML);
	}
	public static string HtmlDecodeInnerHtml(this CsQuery.IDomObject e){
		return HttpUtility.HtmlDecode((e as CsQuery.IDomElement).InnerHTML);
	}
	public static string HtmlDecodeInnerHtml(this string s){
		return HttpUtility.HtmlDecode(s);
	}
	public static string ValueAt(this Match match,int index){
		if(index>=match.Groups.Count){
			return "";
		}
		return match.Groups[index].Value;
	}
}	

public class CNBlog{
	private string UserName{get;set;}
	private string Password{get;set;}
	private CookieContainer Cookie{get;set;}
	private string CookieText{get;set;}
	private HttpHeader Header{get;set;}
	public string HomeHtml{get;private set;}
	
	private static readonly Regex postDescRegex = new Regex(@"posted @ (.+ .+) (.+) 阅读");
	private static readonly Uri cnblogUri = new Uri("http://www.cnblogs.com");
	
	public class HttpHeader{
		public string Accept { get; set; }
		public string ContentType { get; set; }
		public string Method { get; set; }
		public int MaxTry { get; set; }
		public string UserAgent { get; set; }
		public string AcceptEncoding { get; set; }
		public string AcceptLanguage { get; set; }
		public string CacheControl { get; set; }
		public string Connection { get; set; }
	}
		
	public CNBlog(string userName,string password){
		UserName = userName;
		Password = password;
	}
	
	#region Public
	public CNBlog Login(){
		if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password)){
			throw new ArgumentException();
		}
		
		string loginUrl = "http://m.cnblogs.com/mobileLoginPost.aspx";
		string postData = string.Format("tbUserName={0}&tbPassword={1}&txtReturnUrl=", UserName, Password);
		
		Header = new HttpHeader();
		Header.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
		Header.ContentType = "application/x-www-form-urlencoded";
		Header.Method = "POST";
		Header.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
		Header.MaxTry = 300;
		Header.AcceptEncoding = "gzip,deflate,sdch";
		Header.AcceptLanguage = "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4,ja;q=0.2";
		Header.CacheControl = "no-cache";
		Header.Connection = "keep-alive";
		
		string cookieStr = string.Empty;
		var cc = GetCookie(loginUrl, postData, Header, out cookieStr);
		this.Cookie = cc;
		this.CookieText = cookieStr;
		
		this.HomeHtml = GetHtmlByCookie("http://home.cnblogs.com", Header);
		
		return this;
	}
	public string GetHtml(string url){
		var html = GetHtmlByCookie(url, Header);
		return html;
	}
	public IEnumerable<FeedItem> FetchFeeds(string name){
		string homeUrl = string.Format("http://home.cnblogs.com/u/{0}",name);
		CQ dom = CQ.CreateDocument(GetHtml(homeUrl));
		return ParseFeedItems(dom);
	}
	public IEnumerable<Post> FetchPosts(string name){
		string blogUrl = string.Format("http://www.cnblogs.com/{0}",name);
		CQ blogHomeDom = CQ.CreateDocument(GetHtml(blogUrl));
		return ParserPosts(blogHomeDom);
	}
	public IEnumerable<string> FetchStudents(string url){
		CQ page = CQ.CreateDocument(GetHtml(url));
		var query = page.Select("tr > td > a");
		foreach(var e in query){
			var href = e.Attributes["href"];
			if(href.Contains("www.cnblogs.com")){
				yield return Regex.Match(href,"http://www.cnblogs.com/([^/]+)/*.*").ValueAt(1);
			}
		}	
	}
	#endregion
	
	#region Private
	private IEnumerable<FeedItem> ParseFeedItems(CQ dom){
		CQ feedItems = dom.Select(".feed_body");
		foreach(var item in feedItems){
			var feedTitle = item.ChildElements.ElementAt(0);
			var feedDesc  = item.ChildElements.ElementAt(1);
			
			var feedAuthor = feedTitle.ChildElements.ElementAt(0);
			var feedBlog   = feedTitle.ChildElements.ElementAt(1);
			var feedDate   = feedTitle.ChildElements.ElementAt(2);
			
			Uri url = null;
			try{
				Uri baseUri = new Uri("http://www.cnblogs.com");
				url = new Uri(baseUri,feedBlog.Attributes["href"]);
			}catch(Exception e){
				feedBlog.Attributes["href"].Dump();
				e.Message.Dump();
			}
			
			var feedItem = new FeedItem{
				Author = feedAuthor.HtmlDecodeInnerText(),
				CommentBlog = new FeedBlog{
					Url = url,
					Title = feedBlog.HtmlDecodeInnerText()
				},
				Date = feedDate.HtmlDecodeInnerText(),
				Comment = feedDesc.HtmlDecodeInnerText()
			};
			yield return feedItem;
		}
		
		string nextPageUrl = null;
		var q = dom.Select(".block_arrow > a");
		if(q.Any()){
			var node = q.ElementAt(0);	
			if(node.Attributes["href"].Contains("/feed/2.html")){
				nextPageUrl = node.Attributes["href"];
			}
		}
		
		q = dom.Select(".pager");
		if(q.Any()){
			var node = q.ElementAt(0).LastChild;
			if(node.HtmlDecodeInnerText().Contains("Next")){
				nextPageUrl = node.Attributes["href"];
			}
		}
		
		if(nextPageUrl!=null){
			nextPageUrl = string.Format("http://home.cnblogs.com/{0}",nextPageUrl);
			var nextDom = CQ.CreateDocument(GetHtml(nextPageUrl));
			if(nextDom!=null){
				var items = ParseFeedItems(nextDom);
				foreach(var item in items){
					yield return item;
				}
			}
		}
	}
	private IEnumerable<Post> ParserPosts(CQ dom){
		CQ dayItems = dom.Select(".day");
		var postTitles = dom.Select(".day .postTitle > a");
		var postDescs = dom.Select(".day .postDesc");
		
		int count = postTitles.Count();
		for(int i=0;i<count;i++){
			var postTitle = postTitles.ElementAt(i);
			var postDesc = postDescs.ElementAt(i).HtmlDecodeInnerText();
			
			var postDescMatch = postDescRegex.Match(postDesc);
			var postUrl = new Uri(cnblogUri,postTitle.Attributes["href"]);
			
			var postHtml = GetHtml(postUrl.AbsoluteUri);
			var postBodyPageDom = CQ.CreateDocument(postHtml);
			
			string postText= "";
			var postBodyNodes = postBodyPageDom.Select("#cnblogs_post_body");
			if(postBodyNodes.Any()){
				var postBody = postBodyNodes.ElementAt(0);
				postText= postBody.HtmlDecodeInnerHtml();
			}else{
				Console.WriteLine("Can Not Access Blog: {0}",postUrl);
				//throw new Exception("fetch PostBody Failed!");
			}
			
			
			yield return new Post{
				Date = postDescMatch.ValueAt(1),
				Author = postDescMatch.ValueAt(2),
				Title = postTitle.HtmlDecodeInnerText(),
				Url = postUrl,
				Text = postText
				//Text = ""
			};
		};
		
		string nextPageUrl = null;
		var nextPage = dom.Select("#nav_next_page > a");
		if(nextPage!=null&&nextPage.Any()){
			nextPageUrl = nextPage.ElementAt(0).Attributes["href"];
		}
		
		if(nextPageUrl==null){
			var pager = dom.Select(".pager > a");
			if(pager!=null&&pager.Any()){
				var node = pager.Get().Last();
				var pagerContent = node.HtmlDecodeInnerText();
				if(pagerContent.Contains("Next")){
					nextPageUrl = node.Attributes["href"];
				}
			}
		}
		
		if(nextPageUrl!=null){
			var nextDom = CQ.CreateDocument(GetHtml(nextPageUrl));
			if(nextDom!=null){
				var subItems = ParserPosts(nextDom);
				foreach(var subItem in subItems){
					yield return subItem;
				}
			}
		}
	}
	private CookieContainer GetCookie(string loginUrl, string postedData, HttpHeader header, out string cookieStr){
		HttpWebRequest request = null;
		HttpWebResponse response = null;
		Stream requestStream = null;
		CookieContainer cc = new CookieContainer();
		cookieStr = string.Empty;
		
		try
		{
			//准备发起请求
			request = (HttpWebRequest)WebRequest.Create(loginUrl);
			request.Method = header.Method;
			request.ContentType = header.ContentType;
			byte[] postDataByte = Encoding.UTF8.GetBytes(postedData);
			request.ContentLength = postDataByte.Length;
			request.CookieContainer = cc;
			request.KeepAlive = true;
			request.AllowAutoRedirect = false;
		
			//提交请求
			requestStream = request.GetRequestStream();
			requestStream.Write(postDataByte, 0, postDataByte.Length);
		
			//接收响应
			response = (HttpWebResponse)request.GetResponse();
			//response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
			response.Cookies = cc.GetCookies(request.RequestUri);
		
			CookieCollection cookieCollection = response.Cookies;
		
			cc.Add(cookieCollection);
		
			cookieStr = request.CookieContainer.GetCookieHeader(request.RequestUri);
		
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			request = null;
			response.Close();
			requestStream.Close();   
		}
		
		return cc;
	}
	private string GetHtmlByCookie(string url, HttpHeader header){
		string html = string.Empty;
		HttpWebRequest request = null;
		HttpWebResponse response = null;
		StreamReader streamReader = null;
		Stream responseStream = null;
		
		try
		{
			request = (HttpWebRequest)WebRequest.Create(url);
			request.CookieContainer = this.Cookie;
			request.ContentType = header.ContentType;
			request.Referer = url;
			request.Accept = header.Accept;
			request.UserAgent = header.UserAgent;
			request.Method = "GET";
			request.Headers.Add("Cookie:" + this.CookieText); //每次请求时把cookie传给服务器
		
			//发起请求，得到Response
			response = (HttpWebResponse)request.GetResponse();
			responseStream = response.GetResponseStream();
			streamReader = new StreamReader(responseStream, Encoding.UTF8);
			html = streamReader.ReadToEnd();
		
			this.CookieText = request.CookieContainer.GetCookieHeader(request.RequestUri);
		}
		catch (Exception ex)
		{
			if (request != null)
				request.Abort();
			if (response != null)
				response.Close();
			
			Console.WriteLine(url);
			Console.WriteLine(ex.Message);
			Console.WriteLine(ex.StackTrace);
		
			return string.Empty;
			throw ex;
		}
		finally
		{
			if(streamReader!=null) streamReader.Close();
			if(responseStream!=null) responseStream.Close();
			if(request!=null) request.Abort();
			if(response!=null) response.Close();
		}
		return html;
	}
	#endregion
}
