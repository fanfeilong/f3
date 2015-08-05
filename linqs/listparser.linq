<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main() {
    var input1 = "[a,[c,[d,e,f]],b]";
    var input2 = "[a,]";
    var input3 = "[a,[c,[d,e,f]],bc=ss]";
    var lexer = new ListLexer(input3);
    var token = lexer.NextToken();
    while (token.Type != ListLexer.EOF_TYPE) {
        token.ToString().Dump();
        token = lexer.NextToken();
    }
    //var parser = new ListParser(lexer);
    //parser.List();
   
}

// Define other methods and classes here
public class Token {
    public int Type { get; private set; }
    public string Text { get; private set; }
    public override string ToString() {
        return string.Format("<{0},{1}>",Text,ListLexer.GetTokenNameByType(Type));
    }
    public Token(int type, string text) {
        Type = type;
        Text = text;
    }
}

public abstract class Lexer {
    public static char EOF = Convert.ToChar(0x1A);
    public static int EOF_TYPE = 1;
    protected string input;
    protected int p;
    protected char c;
    
    public Lexer(string str) {
        this.input = str;
        this.c = input[0];
        this.p = 0;
    }
    public abstract string GetTokenName(int type);
    public abstract Token NextToken();

    protected void Consume() {
        p++;
        if (p >= input.Length) c = EOF;
        else c = input[p];
    }
    protected void Match(char x) {
        if (c == x) {
            Consume();
        } else {
            throw new Exception("expecting " +x+ "; found " +c);
        }
    }
}

public class ListLexer : Lexer {
    public static int NAME = 2;
    public static int COMMA = 3;
    public static int LBREAK = 4;
    public static int RBREAK = 5;
    public static int EQUALS = 6;
    
    public static readonly List<string> TokenTypes = new List<string> {
        "n/a",
        "<EOF>",
        "NAME",
        "COMMA",
        "LBREAK",
        "RBREAK",
        "EQUALS"
    };
    public static string GetTokenNameByType(int type) {
        return TokenTypes[type];
    }

    public ListLexer(string input) : base(input) {
        //Do Nothing
    }
    
    public override string GetTokenName(int type) {
        return GetTokenNameByType(type);
    }
    public override Token NextToken() {
        while (c != EOF) {
            switch (c) {
                case ' ':
                case '\t':
                case '\n':
                case '\r':{
                    WhiteSpace();
                    continue;
                }
                case ',':{
                    Consume();
                    return new Token(COMMA,",");
                }
                case '[':{
                    Consume();
                    return new Token(LBREAK, "[");
                }
                case ']':{
                    Consume();
                    return new Token(RBREAK, "]");
                    }
                case '=':{
                    Consume();
                    return new Token(EQUALS,"=");
                }
                default:{
                    if (IsLetter()) return Name();
                    throw new Exception("invalid character: " + c);
                }
            }
        }
        return new Token(EOF_TYPE, "<EOF>");
    }

    private bool IsLetter() {
        return (c>='a'&&c<='z')||(c>='A'&&c<='Z');
    }
    private void WhiteSpace() {
        while(c==' '||c=='\t'||c=='\r'||c=='\n') Consume();
    }
    private Token Name() {
        var sb = new StringBuilder();
        do {
            sb.Append(c);Consume();
        }while(IsLetter());
        return new Token(NAME,sb.ToString());
    }
}

public abstract class Parser {
    protected Token[] lookahead;
    protected Lexer lexer;
    protected int p=0;
    protected int K=0;
    
    public Parser(Lexer lexer,int K) {
        this.K = K;
        this.lexer = lexer;
        this.lookahead = new Token[K];
        for(int i=0;i<K;i++) Consume();
    }
    
    protected void Consume() {
        lookahead[p] = lexer.NextToken();
        p = (p+1)%K;
    }
    protected Token LT(int i) {
        return this.lookahead[(p+i-1)%K];
    }
    protected int LA(int i) {
        return LT(i).Type;
    }
    protected void Match(int x) {
        if(LA(1)==x) Consume();
        else throw new Exception("exception:"+lexer.GetTokenName(x)+"; found "+LT(1));
    }
}

public class ListParser : Parser {
    public ListParser(ListLexer lexer) : base(lexer,2) {
        // Do Nothing
    }
    public void List() {
        Match(ListLexer.LBREAK);
        Elements();
        Match(ListLexer.RBREAK);
    }
    private void Elements() {
        Element();
        while (LA(1) == ListLexer.COMMA) {
            Match(ListLexer.COMMA);
            Element();
        }
    }
    private void Element() {
        if (LA(1) == ListLexer.NAME && LA(2) == ListLexer.EQUALS) {
            Match(ListLexer.NAME);
            Match(ListLexer.EQUALS);
            Match(ListLexer.NAME);
        } else if(LA(1)==ListLexer.NAME){
            Match(ListLexer.NAME);
        } else if (LA(1) == ListLexer.LBREAK) {
            List();
        } else {
            throw new Exception("Expection List Or Name Or Name=Name; Found:"+LT(1));
        }
    }
}