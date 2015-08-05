<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main(){
//    var list = Enumerable.Range(1,5);
//    var query = list
//    .Select(i=>Enumerable.Range(1,4))
//    .Blend()
//    .Dump();
    
    var Input = "Hello Laser Participants!";
}

// Define other methods and classes here
public static class Extension{
    public static IEnumerable<T> Single<T>(this T t){
        yield return t;
    }
    public static IEnumerable<T> Append<T>(this IEnumerable<T> list,T t){
        foreach(var i in list)
            yield return i;
        yield return t;
    }
    public static IEnumerable<T> Repeat<T>(this T t,int n){
        for(int i=0;i<n;i++)
            yield return t;
    }
    public static IEnumerable<T> Blend<T>(this IEnumerable<IEnumerable<T>> list){
        // Once In, Nerver Out
        foreach(var l in list)
            foreach(var i in l)
                yield return i;
    }
}