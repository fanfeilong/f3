<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main()
{
	var a = new int[]{
		1,2,3,4,5,6,8,8
	};
	
	var sb = new StringBuilder();
	a.Aggregate(sb,
		(b,first) =>b.AppendFormat("|{0}|",first),
		(b,i,next)=>b.AppendFormat("{0}|",next))
	 .ToString()
	 .Dump();
}

// Define other methods and classes here
public static class Extension{
	public static R Aggregate<T,R>(this IEnumerable<T> list,R seed,Func<R,T,R> first,Func<R,int,T,R> con){
		if(!list.Any()){
			return default(R);
		}
		
		var e = list.GetEnumerator();
		
		T a = default(T);
		
		R r = seed;
		if(e.MoveNext()){
			a = (T)e.Current;	
			r = first(seed,a);
		}
		
		int i = 1;
		while(e.MoveNext()){
			a = (T)e.Current;
			r = con(r,i,a);
			i++;
		}
		
		return r;
	}
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> list){
		return new HashSet<T>(list);
	}
	public static IEnumerable<T> Concat<T>(this T first,T second){
		yield return first;
		yield return second;
	}
	public static IEnumerable<T> Concat<T>(this IEnumerable<T> first,T second){
		foreach(var item in first) yield return item;
		yield return second;
	}
	public static void ForEach<T>(this IEnumerable<T> list,Action<T> task){
		foreach(var item in list) task(item);
	}
	public static SortedDictionary<string,string> ToDictionary(this IDictionary dict){
		var keys = dict.Keys;
		var values = dict.Values;
		var result = new SortedDictionary<string,string>();
		var keyEnumerator = keys.GetEnumerator();
		var valueEnumerator = values.GetEnumerator();
		while(keyEnumerator.MoveNext() && valueEnumerator.MoveNext()){
			result.Add(keyEnumerator.Current.ToString(),valueEnumerator.Current.ToString());
		}
		
		return result;
	}
	public static IEnumerable<Tuple<T,T>> ToColumes2<T>(this IEnumerable<T> list){
		var count=0;
		var values = new T[2];
		foreach(var item in list){
			values[count]=item;
			
			if(count==1){
				yield return Tuple.Create(values[0],values[1]);
				count = 0;
				values = new T[2];
			}else{
				count++;
			}
		}
		if(count>0){
			yield return Tuple.Create(values[0],values[1]);
		}
	}
	public static IEnumerable<Tuple<T,T,T>> ToColumes3<T>(this IEnumerable<T> list){
		var count=0;
		var values = new T[3];
		foreach(var item in list){
			values[count]=item;
			
			if(count==2){
				var tuple = Tuple.Create(values[0],values[1],values[2]);
				count = 0;
				values = new T[3];
				yield return tuple;
			}else{
				count++;
			}
		}
		
		if(count>0){
			yield return Tuple.Create(values[0],values[1],values[2]);
		}
	}
	public static IEnumerable<Tuple<T,T,T,T>> ToColumes4<T>(this IEnumerable<T> list){
		var count=0;
		var values = new T[4];
		foreach(var item in list){
			values[count]=item;
			
			if(count==3){
				var tuple = Tuple.Create(values[0],values[1],values[2],values[3]);
				count = 0;
				values = new T[4];
				yield return tuple;
			}else{
				count++;
			}
		}
		
		if(count>0){
			yield return Tuple.Create(values[0],values[1],values[2],values[3]);
		}
	}	
}