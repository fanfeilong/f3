<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main() {
    TestPasec();
    //TestTreeList();
}

public void TestPasec() {
    var table = @" t ss = { 
        ni  ,
        hao,zheshi  ,  
        yizhong   ,
        mei you,
        c,
        a = {
            b,
            d
        }
     }";

//    var table2 = @" t ss = { 
//        ni,ni 
//     }";
    var parser =
     Parser
    .Begin().Spaceable(true)
    .Word(Parser.Except('='))
    .Char('=')
    .Char('{')
    .Many(
        Parser
        .Switch(2)
        .Case(
            Parser
            .Word(Parser.Except('=', '{', '}', ','))
            .Try(Parser.Char(','))
            .SkipOne()
        )
        .Case(
            Parser
            .Word(Parser.Except('=', '{', '}', ','))
            .Char('=')
            .Char('{')
            .Many(
                Parser
                .Word(Parser.Except('=', '{', '}', ','))
                .Try(Parser.Char(','))
                .SkipOne()
            )
            .Char('}')
            .Try(Parser.Char(','))
            .SkipOne()
        )
        .Default()
    )
    .Char('}')
    .End();

    var r = parser(table);
    //r.Dump();
    r.ToString().Dump();// Output:^t ss={[ni,hao,zheshi,yizhong,mei you,ca={[b,d]},]}$

    //r.Value.Value.Value.Rest.AsEnumerable().Dump();
    foreach (var v in r.AsEnumerable()) {
        v.Dump();
    }
}

public void TestTreeList() {
    var tt = Tree.CreateList<int>()
        .Add(1)
        .Add(2)
        .Add(3)
        .Add(4)
        .Add(5)
        .Dump();

    tt.ToString().Dump();

    foreach (var v in tt.AsEnumerable()) {
        v.Dump();
    }
}

public class Nil { }

public class Nil<T> : Nil {
    private static Nil<T> instance = new Nil<T>();
    private Nil() {}
    public static Nil<T> Value {
        get {
            return instance;
        }
    }
    public override string ToString() {
        return "Nil";
    }
}

public interface ITree { 
    IEnumerable<object> AsEnumerable();
    void Str(StringBuilder sb);
}

public abstract class Tree : ITree {
    public static Tree<V, R> Create<V, R>(V v, R r) {
        return new Tree<V, R>(v, r);
    }
    
    public static TList<Nil<T>,Nil<T>,T> CreateList<T>() {
        return new TList<Nil<T>,Nil<T>,T>(Nil<T>.Value,Nil<T>.Value);
    }

    public static TList<V, R,T> CreateList<V,R,T>(V v, R r) {
        return new TList<V, R,T>(v, r);
    }

    public abstract IEnumerable<object> AsEnumerable();
    public abstract void Str(StringBuilder sb);
}

public class Tree<V, R> : Tree {
    public V Value { get; private set; }
    public R Rest { get; set; }

    public Tree(V v, R r) {
        Value = v;
        Rest = r;
    }
    public override string ToString() {
        var sb = new StringBuilder();

        Str(sb);
        
        return sb.ToString();
    }

    private bool IsNil<T>(T t) {
        return (t is Nil) || (t==null);
    }

    public override void Str(StringBuilder sb) {
        if (!IsNil(Value) && !IsNil(Rest)) {
            //sb.Append("(");
        }

        StrObject(sb,Value);

        if (!IsNil(Value) && !IsNil(Rest)) {
            //sb.Append(",");
        }

        StrObject(sb,Rest);

        if (!IsNil(Value) && !IsNil(Rest)) {
            //sb.Append(")");
        }

    }

    public void StrList(StringBuilder sb,IList r) {
        var e = r.GetEnumerator();
        int i=0;
        sb.Append("[");
        while (e.MoveNext()) {
            if (i > 0) {
                sb.Append(","); 
            }
            var v = e.Current;
            StrObject(sb,v);
            i++;
        }
        sb.Append("]");
        
    }
    public void StrObject(StringBuilder sb,object v) {
        var rr = v as IList;
        if (rr != null) {
            StrList(sb,rr);
        } else {
            var rrr = v as ITree;
            if (rrr != null) {
                rrr.Str(sb);
            } else if (!IsNil(v)) {
                sb.Append(v);
            }
        }
    }

    public override IEnumerable<object> AsEnumerable() {
        foreach (var vv in Each(Value)) {
            yield return vv;
        }
        foreach (var vv in Each(Rest)) {
            yield return vv;
        }
    }

    public IEnumerable<object> RecursiveEnumerator(IList r) {
        var e = r.GetEnumerator();
        while (e.MoveNext()) {
            var v = e.Current;
            foreach (var vv in Each(v)) {
                yield return vv;
            }
        }
    }
    public IEnumerable<object> Each(object v) {
        var rr = v as IList;
        if (rr != null) {
            foreach (var vv in RecursiveEnumerator(rr)) {
                yield return vv;
            }
        } else {
            var rrr = v as ITree;
            if (rrr != null) {
                    foreach (var vv in rrr.AsEnumerable()) {
                        yield return vv;
                    }
            } else if (v != null ||(v is Nil)) {
                yield return v;
            }
        }
    }
}

interface TreeList<T> { 
    IEnumerable<T> AsEnumerable();
}

public class TList<V,R,T> : Tree<V, R> , TreeList<T> {
    public TList(V t, R v) : base(t, v) {
        //
    }
    
    public new IEnumerable<T> AsEnumerable(){
        var v = Value as TreeList<T>;
        if (v != null) {
            foreach (var vv in v.AsEnumerable()) {
                yield return vv;
            }
        } else {
            Debug.Assert(Value is Nil);
        }
        if (!(Rest is Nil<T>)) {
            yield return (T) Convert.ChangeType(Rest, typeof (T));
        }
    }
}

// Define other methods and classes here
public static class Parser {
    private static bool spaceable = false;
    private static bool lastspace;

    private static string TrimeStartSpace(this string s) {
        if (spaceable) {
            return s.TrimStart();
        } else {
            return s;
        }
    }

    public static Func<string, Tree<F, string>> Spaceable<F>(this Func<string, Tree<F, string>> p, bool v) {
        var parser = new Func<string, Tree<F, string>>(str => {
            spaceable = v;
            return p(str);
        });
        return parser;
    }

    public static int FirstAsc(this string s) {
        if (string.IsNullOrEmpty(s)) {
            throw new Exception("Character is not valid.");
        }
        var asciiEncoding = new ASCIIEncoding();
        var intAsciiCode = (int)asciiEncoding.GetBytes(s.Substring(0, 1))[0];
        return (intAsciiCode);
    }

    public static Tree<string, string> Digit(string str) {
        if (string.IsNullOrEmpty(str)) {
            throw new Exception("Emptry string.");
        }

        str = str.TrimeStartSpace();

        var d = str.FirstAsc();
        if (d <= 57 && d >= 48) {
            return Tree.Create(str.Substring(0, 1), str.Substring(1));
        } else {
            throw new Exception("Not a digit.");
        }
    }
    public static Tree<string, string> Letter(string str) {
        if (string.IsNullOrEmpty(str)) {
            throw new Exception("Emptry string.");
        }

        str = str.TrimeStartSpace();

        var d = str.FirstAsc();
        if ((d <= 90 && d >= 65) || (d <= 122 && d >= 97)) {
            return Tree.Create(str.Substring(0, 1), str.Substring(1));
        } else {
            throw new Exception("Not a Letter.");
        }
    }
    public static Tree<string, string> Space(string str) {
        if (string.IsNullOrEmpty(str)) {
            throw new Exception("Emptry string.");
        }

        var d = str[0];
        if (d == ' ' || d == '\t' || d == '\n') {
            return Tree.Create(str.Substring(0, 1), str.Substring(1));
        } else {
            throw new Exception("Not Space.");
        }
    }
    public static Func<string, Tree<string, string>> Char(char c) {
        var parser = new Func<string, Tree<string, string>>(str => {
            if (string.IsNullOrEmpty(str)) {
                throw new Exception("Emptry string.");
            }

            str = str.TrimeStartSpace();

            var d = str.FirstAsc();
            var cc = c;
            if (d == cc) {
                return Tree.Create(str.Substring(0, 1), str.Substring(1));
            } else {
                throw new Exception("Not a Char.");
            }
        });
        return parser;
    }
    public static Func<string, Tree<string, string>> Begin() {
        var parser = new Func<string, Tree<string, string>>(str => {
            if (string.IsNullOrEmpty(str)) {
                throw new Exception("invalid input.");
            }

            return Tree.Create("^", str);
        });
        return parser;
    }
    public static Tree<string, string> End(string str) {
        if (!string.IsNullOrEmpty(str)) {
            throw new Exception("Not the end of input.");
        }

        return Tree.Create("$", default(string));
    }

    public static Func<string, Tree<Tree<F, string>, string>> Digit<F>(this Func<string, Tree<F, string>> fp) {
        return fp.Next(Digit);
    }
    public static Func<string, Tree<Tree<F, string>, string>> Letter<F>(this Func<string, Tree<F, string>> fp) {
        return fp.Next(Letter);
    }
    public static Func<string, Tree<Tree<F, string>, string>> Space<F>(this Func<string, Tree<F, string>> fp) {
        return fp.Next(Space);
    }
    public static Func<string, Tree<Tree<F, string>, string>> Char<F>(this Func<string, Tree<F, string>> fp, char c) {
        return fp.Next(Char(c));
    }
    public static Func<string, Tree<Tree<F, string>, string>> End<F>(this Func<string, Tree<F, string>> fp) {
        return fp.Next(End);
    }


    public static Func<string, Tree<string, string>> Word<T>(Func<string, Tree<T, string>> p) {
        var parser = new Func<string, Tree<string, string>>(str => {
            bool old = spaceable;
            spaceable = false;
            var r = ManyOne(p).Concat(cs => string.Concat(cs).Trim())(str);
            spaceable = old;
            return r;
        });
        return parser;
    }
    public static Func<string, Tree<Tree<F, string>, string>> Word<F, T>(this Func<string, Tree<F, string>> fp, Func<string, Tree<T, string>> p) {
        return fp.Next(Word(p));
    }

    // see: https://github.com/kklingenberg/parsec/blob/master/parsec/parse.py

    public static Func<string, Tree<List<T>, string>> Many<T>(Func<string, Tree<T, string>> p) {
        var parser = new Func<string, Tree<List<T>, string>>(str => {
            str = str.TrimeStartSpace();

            var result = new List<T>();
            while (true) {
                try {
                    var r = p(str);
                    result.Add(r.Value);
                    str = r.Rest;
                } catch {
                    var rr= Tree.Create(result, str);
                    return rr;
                    
                }
            }
        });
        return parser;
    }
    public static Func<string, Tree<List<T>, string>> ManyOne<T>(Func<string, Tree<T, string>> p) {
        var parser = new Func<string, Tree<List<T>, string>>(str => {
            str = str.TrimeStartSpace();

            var r1 = p(str);
            var r2 = Many(p)(r1.Rest);

            r2.Value.Insert(0, r1.Value);

            return r2;
        });
        return parser;
    }
    private static Func<string, Tree<List<T>, string>> Sequence<T>(params Func<string, Tree<T, string>>[] ps) {
        var parser = new Func<string, Tree<List<T>, string>>(str => {
            var list = new List<T>();
            foreach (var p in ps) {
                str = str.TrimeStartSpace();
                var r = p(str);
                str = r.Rest;
                list.Add(r.Value);
            }

            return Tree.Create(list, str);
        });
        return parser;
    }
    public static Func<string, Tree<R, string>> Map<T, R>(Func<string, Tree<T, string>> p, Func<T, R> map) {
        var parser = new Func<string, Tree<R, string>>(str => {
            str = str.TrimeStartSpace();

            var r = p(str);
            var v = map(r.Value);
            return Tree.Create(v, r.Rest);
        });
        return parser;
    }
    public static Func<string, Tree<T, string>> Return<T>(T value) {
        var parser = new Func<string, Tree<T, string>>(str => {
            return Tree.Create(value, str);
        });
        return parser;
    }
    private static Func<string, Tree<T, string>> Choice<T>(params Func<string, Tree<T, string>>[] ps) {
        var parser = new Func<string, Tree<T, string>>(str => {
            str = str.TrimeStartSpace();

            var message = new StringBuilder();
            int i = 0;
            foreach (var p in ps) {
                try {
                    //i.Dump("x");                    
                    var r = p(str);
                    //i.Dump("xx");

                    i++;
                    return Tree.Create(r.Value, r.Rest);
                } catch (Exception e) {
                    message.AppendLine(e.Message);
                }
            }
            throw new Exception(message.ToString());
        });
        return parser;
    }

    public static Func<string, Tree<T, string>> Try<T>(Func<string, Tree<T, string>> p, int c = -1) {
        var parser = new Func<string, Tree<T, string>>(str => {
            try {
                str = str.TrimeStartSpace();
                var r = p(str);
                return Tree.Create(r.Value, r.Rest);
            } catch {
                return Tree.Create(default(T), str);
            }
        });
        return parser;
    }

    private static int exceptionCount = 0;
    private static int exceptionMax = -1;

    private class exception {
        public int max=-1;
        public int count = 0;
        public bool all {
            get {
                return max>=0 && max==count;
            }
        }
        public void inc() {
            count++;
        }
    }
    private static Stack<exception> except = new Stack<exception>();

    public static Func<string, Tree<string, string>> Switch(int c) {
        var parser = new Func<string, Tree<string, string>>(str => {
            if (string.IsNullOrEmpty(str)) {
                throw new Exception("invalid input.");
            }

            if (c != -1) {
                var e = new exception();
                e.max = c;
                e.count = 0;
                except.Push(e);
            }

            return Tree.Create(default(string), str);
        });
        return parser;
    }
    
    public static Func<string, Tree<Tree<F, string>, string>> Switch<F>(this Func<string, Tree<F, string>> fp,int c) {
        return fp.Next(Switch(c));
    }
    

    public static Func<string, Tree<T, string>> Case<T>(Func<string, Tree<T, string>> p) {
        var parser = new Func<string, Tree<T, string>>(str => {
            try {
                str = str.TrimeStartSpace();
                var r = p(str);
                return Tree.Create(r.Value, r.Rest);
            } catch {
                except.First().inc();
                return Tree.Create(default(T), str);
            }
        });
        return parser;
    }
    
    public static Func<string, Tree<T, string>> Default<T>(this Func<string, Tree<T, string>> p) {
        var parser = new Func<string, Tree<T, string>>(str => {
            str = str.TrimeStartSpace();
            var r = p(str);
            
            var e = except.Pop();
            if (e.all) {
                throw new Exception("xx");
            } else {
                return Tree.Create(r.Value, r.Rest);
            }
        });
        return parser;
    }
    public static Func<string, Tree<string, string>> Except(params char[] letters) {
        var parser = new Func<string, Tree<string, string>>(str => {
            if (string.IsNullOrEmpty(str)) {
                throw new Exception("Empty string.");
            } else {
                str = str.TrimeStartSpace();
                var c = str[0];
                if (letters.Contains(c)) {
                    throw new Exception(string.Format("Input matches one of {0}", c));
                } else {
                    return Tree.Create(str.Substring(0, 1), str.Substring(1));
                }
            }
        });
        return parser;
    }

    public static Func<string, Tree<Tree<F, List<T>>, string>> Many<F, T>(this Func<string, Tree<F, string>> fp, Func<string, Tree<T, string>> p) {
        return fp.Next(Many(p));
    }
    public static Func<string, Tree<Tree<F, List<T>>, string>> ManyOne<F, T>(this Func<string, Tree<F, string>> fp, Func<string, Tree<T, string>> p) {
        return fp.Next(ManyOne(p));
    }
    public static Func<string, Tree<Tree<F, List<T>>, string>> Sequence<F, T>(this Func<string, Tree<F, string>> fp,params Func<string, Tree<T, string>>[] ps) {
        return fp.Next(Sequence(ps));
    }
    public static Func<string, Tree<Tree<F, R>, string>> Map<F, T, R>(this Func<string, Tree<F, string>> fp, Func<string, Tree<T, string>> p, Func<T, R> map) {
        return fp.Next(Map(p, map));
    }
    public static Func<string, Tree<Tree<F, T>, string>> Return<F, T>(this Func<string, Tree<F, string>> fp, T value) {
        return fp.Next(Return(value));
    }
    public static Func<string, Tree<Tree<F, T>, string>> Or<F, T>(this Func<string, Tree<F, string>> fp, params Func<string, Tree<T, string>>[] ps) {
        return fp.Next(Choice(ps));
    }
    public static Func<string, Tree<Tree<F, T>, string>> Try<F, T>(this Func<string, Tree<F, string>> fp, Func<string, Tree<T, string>> p) {
        return fp.Next(Try(p));
    }
    public static Func<string, Tree<Tree<F, T>, string>> Case<F, T>(this Func<string, Tree<F, string>> fp, Func<string, Tree<T, string>> p) {
        return fp.Next(Case(p));
    }

    public static Func<string, Tree<Tree<F, string>, string>> Except<F>(this Func<string, Tree<F, string>> fp, params char[] letters) {
        return fp.Next(Except(letters));
    }
    
    public static Func<string, Tree<Tree<F, Tree<Tree<List<string>,List<T>>,List<string>>>, string>> Spaceable<F, T>(this Func<string, Tree<F, string>> fp, params Func<string, Tree<T, string>>[] ps) {
        var parser = Parser.Many(Parser.Space).Sequence(ps).Many(Parser.Space);
        return fp.Next(parser);
    }
    public static Func<string, Tree<Tree<F, S>, string>> Next<F, S>(this Func<string, Tree<F, string>> fp, Func<string, Tree<S, string>> sp) {
        var parser = new Func<string, Tree<Tree<F, S>, string>>(str => {
            var r1 = fp(str);
            var r2 = sp(r1.Rest);
            return Tree.Create(Tree.Create(r1.Value, r2.Value), r2.Rest);
        });
        return parser;
    }
    public static Func<string, Tree<Tree<FF, Tree<F, S>>, string>> Next<FF, F, S>(this Func<string, Tree<Tree<FF, F>, string>> fp, Func<string, Tree<S, string>> sp) {
        var parser = new Func<string, Tree<Tree<FF, Tree<F, S>>, string>>(str => {
            var r1 = fp(str);
            var r2 = sp(r1.Rest);
            return Tree.Create(Tree.Create(r1.Value.Value, Tree.Create(r1.Value.Rest, r2.Value)), r2.Rest);
        });
        return parser;
    }
    public static Func<string, Tree<R, string>> Concat<T, R>(this Func<string, Tree<List<T>, string>> p, Func<IEnumerable<T>, R> c) {
        var parser = new Func<string, Tree<R, string>>(str => {
            var r = p(str);
            if (r.Value.Equals(default(List<T>))) {
                return Tree.Create(default(R), r.Rest);
            } else {
                var v = r.Value.AsEnumerable();
                return Tree.Create(c(v.Where(i => !i.Equals(default(T)))), r.Rest);
            }
        });
        return parser;
    }
    public static Func<string, Tree<Tree<L, R>, string>> Concat<L, T, R>(this Func<string, Tree<Tree<L, List<T>>, string>> p, Func<IEnumerable<T>, R> c) {
        var parser = new Func<string, Tree<Tree<L, R>, string>>(str => {
            var r = p(str);
            if (r.Value.Value.Equals(default(List<T>))) {
                return Tree.Create(Tree.Create(r.Value.Value, default(R)), r.Rest);
            } else {
                var v = r.Value.Rest.AsEnumerable();
                return Tree.Create(Tree.Create(r.Value.Value, c(v.Where(i => !i.Equals(default(T))))), r.Rest);
            }
        });
        return parser;
    }
    public static Func<string, Tree<F, string>> SkipOne<F, S>(this Func<string, Tree<Tree<F, S>, string>> p) {
        var parser = new Func<string, Tree<F, string>>(str => {
            var r = p(str);
            return Tree.Create(r.Value.Value, r.Rest);
        });
        return parser;
    }
}

public static class TreeListExtension {
    public static TList<F,Nil<F>,F> Add<F>(this TList<Nil, Nil,F> list, F t) {
        return Tree.CreateList<F,Nil<F>,F>(t,Nil<F>.Value);
    }
    
    public static TList<TList<FF,F,F>,Nil<F>,F> Add<FF, F>(this TList<FF, Nil<F>,F> list, F t) {
        return Tree.CreateList<TList<FF,F,F>,Nil<F>,F>(Tree.CreateList<FF,F,F>(list.Value, t), Nil<F>.Value);
    }
    
    public static TList<TList<TList<FF,F,F>,T,F>, Nil<F>,F> Add<FF, F, T>(this TList<TList<FF,F,F>, Nil<F>,F > list, T t) {
        return Tree.CreateList<TList<TList<FF,F,F>,T,F>, Nil<F>,F>(Tree.CreateList<TList<FF,F,F>,T,F>(list.Value,t), Nil<F>.Value);
    }
}















