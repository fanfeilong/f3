<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main()
{
	pattern = string.Format(@"
            (?:
                (?<=\n\n)           # Starting after a blank line
                |                   # or
                \A\n?               # the beginning of the doc
            )
            (                       # save in $1
                [ ]{{0, {0}}}
                <(hr)               # start tag = $2
                \b                  # word break
                ([^<>])*?           #
                /?>                 # the matching end tag
                [ \t]*
                (?=\n{{2,}}|\Z)     # followed by a blank line or end of document
            )", tabWidth - 1);
        text = Regex.Replace(text, pattern, new MatchEvaluator(HtmlEvaluator), RegexOptions.IgnorePatternWhitespace);
}

// Define other methods and classes here
