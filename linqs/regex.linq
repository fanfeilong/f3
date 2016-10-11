<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main() {
	u.TestReplace();
}
public static class u {
	public static void TestReplace() {
		var input = "sss 888 = sadf 444";
		var regex = @"(\d+)";
		var matches = Regex.Matches(input,regex);
		var output = matches.Replace(input,"fanfeilong",1);
		output.Dump();
	}
}

public static class RegexExtensions {
    public static string Replace(this MatchCollection matches, string source, string replacement, int groupIndex) {
        var inversMatches = new Stack<Match>();
        foreach (var match in matches.Cast<Match>()) {
            inversMatches.Push(match);
        }
        while (inversMatches.Any()) {
            var match = inversMatches.Pop();
            source = match.Replace(source, replacement, groupIndex);
        }
        return source;
    }
    public static string Replace(this MatchCollection matches, string source, string replacement, int matchIndex, int groupIndex) {
        Match match = null;
        int i = 0;
        foreach (var m in matches.Cast<Match>()) {
            if (i == matchIndex) {
                match = m;
            }
            i++;
        }
        if (match != null) {
            source = match.Replace(source, replacement, groupIndex);
            return source;
        } else {
            return null;
        }
    }
    public static string Replace(this Match match, string source, string replacement, int index) {
        var g = match.Groups[index];
        return source.Substring(0, g.Index) + replacement + source.Substring(g.Index + g.Length);
    }
    public static string ValueAt(this Match match, int index) {
        if (match.Captures.Count == 0) {
            return null;
        } else {
            if (match.Groups.Count > index) {
                return match.Groups[index].Value;
            } else {
                return null;
            }
        }
    }
}