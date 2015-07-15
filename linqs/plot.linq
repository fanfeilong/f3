<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
</Query>

void Main(){
	var plot = new Plot(20,20,30,30,10,10);

	var x = new float[]{
		0,2,4,5,7,3
	};
	var y = new float[]{
		1,2,5,6,8,9
	};
	
	plot.Axis()
	    .Curve("AHPW-AIR",x,y,Color.Green,Color.Blue)
		.Curve("LOF-AIR",y,x,Color.Red,Color.Blue)
		.Bitmap
		.Dump();
}

// Define other methods and classes here
public class Render : IDisposable{
	private bool disposed = false;
	
	public Graphics Graphics{get;private set;}
	private Dictionary<string,Pen> Pens{get;set;}
	private Dictionary<string,Brush> Brushes{get;set;}
	private Dictionary<string,Font> Fonts{get;set;}
	
	public Render(Bitmap bitmap){
		Graphics = Graphics.FromImage(bitmap);
		Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
		Pens = new  Dictionary<string,Pen>();
		Brushes = new Dictionary<string,Brush>();
		Fonts = new Dictionary<string,Font>();
	}
	public Pen Pen(string name,Color c){
		var pen = new Pen(c);
		Pens.Add(name,pen,(n,p)=>p.Dispose());
		return pen;
	}
	public Brush Brush(string name,Color c){
		var brush = new SolidBrush(c);
		Brushes.Add(name,brush,(n,b)=>b.Dispose());
		return brush;
	}
	public Font Font(string name,string fontName,int fontSize){
		var font = new Font(new FontFamily(fontName),fontSize);
		Fonts.Add(name,font,(n,f)=>f.Dispose());
		return font;
	}
				
	#region IDispose
	public void Dispose(){
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	protected virtual void Dispose(bool disposing){
		if(disposed) return;
		disposed = true;
		
		// NOTE:
		// Release unmanaged resources here
		
		if(!disposing) return;
		
		// NOTE:
		// Release managed resources here
		Pens.ForEach((n,p)=>p.Dispose());
		Brushes.ForEach((n,b)=>b.Dispose());
		Fonts.ForEach((n,f)=>f.Dispose());
	}
	#endregion
}

public static class Lambda{
	public static Action<T,T,T,T> Action<T>(Action<T,T,T,T> a){
		return a;
	}
}

public class Axis{
	public int OX{get;private set;}
	public int OY{get;private set;}
	public int XStep{get;private set;}
	public int YStep{get;private set;}
	public Axis(int ox,int oy,int xstep,int ystep){
		OX = ox;
		OY = oy;
		XStep = xstep;
		YStep = ystep;
	}
}

public class Canvas{
	public Bitmap Bitmap{get;private set;}
	public int Left{get;private set;}
	public int Top{get;private set;}
	public int Bottom{get;private set;}
	public int Right{get;private set;}
	public int Width{get;private set;}
	public int Height{get;private set;}
	public int XStep{get;private set;}
	public int YStep{get;private set;}
	public int XSize{get;private set;}
	public int YSize{get;private set;}
	public Axis Axis{get;private set;}
	
	public Canvas(int left,int top,int width,int height,int xstep,int ystep){
		Left = left;
		Top = top;
		Bottom = top+height-2*left;
		Right = left+width-2*top;
		
		Width = width;
		Height = height;
		
		XStep = xstep;
		YStep = ystep;
		XSize = (int)Math.Floor(width*1.0/xstep);
		YSize = (int)Math.Floor(height*1.0/ystep);
		
		Bitmap = new Bitmap(Width,Height);
		Axis = new Axis(Left,Bottom,XStep,YStep);
	}
	
	public Render AsRender(){
		return new Render(Bitmap);
	}
	
	
}

public class Plot{
	public int Width{get;private set;}
	public int Height{get;private set;}
	public Canvas Canvas{get;private set;}
	public Bitmap Bitmap{
		get{
			return Canvas.Bitmap;
		}
	}

	public Plot(int marginX,int marginY,int xstep,int ystep,int xsize,int ysize){
		Width  = marginX*2+xstep*xsize+10;
		Height = marginY*2+ystep*ysize+10;
		Canvas = new Canvas(marginX,marginY,Width,Height,xstep,ystep);
	}
	public Plot Axis(){
		var c = Canvas;
		using(var r = c.AsRender()){

			// Prepare
			var g = r.Graphics;
			var p = r.Pen("line",Color.Black);
			var b = r.Brush("text",Color.Black);
			var	f = r.Font("default","宋体",9);
			
			
			// Draw X Axis
			g.DrawLine(p,c.Left,c.Bottom,c.Right,c.Bottom);
			g.DrawArrow(p,c.Right-3,c.Bottom-3,c.Right,c.Bottom,c.Right-3,c.Bottom+3);
			for(int i=0;i<c.XSize;i++){
				var x1 = c.Left+c.XStep*i;
				var y1 = c.Bottom;
				var x2 = x1;
				var y2 = y1-5;
				g.DrawLine(p,x1,y1,x2,y2);
				
				int shift=1;
				int v=i/10;
				while(v>0) {shift++;v/=10;}
				g.DrawString(i.ToString(),f,b,x1-3,y1+5);
			}
			
			// Draw Y Axis
			g.DrawLine(p,c.Left,c.Bottom,c.Left,c.Top);
			g.DrawArrow(p,c.Left-3,c.Top+3,c.Left,c.Top,c.Left+3,c.Top+3);
			for(int i=0;i<c.YSize;i++){
				var x1 = c.Left;
				var y1 = c.Bottom-c.YStep*i;
				var x2 = x1+5;
				var y2 = y1;
				g.DrawLine(p,x1,y1,x2,y2);
				
				int shift=1;
				int v=i/10;
				while(v>0) {shift++;v/=10;}
				g.DrawString(i.ToString(),f,b,x1-10*shift,y1-3);
			}
		}
		return this;
	}
	
	private int curvesCount=0;
	public Plot Curve(string title,float[] x,float[] y,Color lineColor,Color dotColor){
		var v = x.Zip(y,(a,b)=>Tuple.Create(a,b));
		
		var c = Canvas;
		using(var r = c.AsRender()){
			
			// Prepare
			var g = r.Graphics;
			var linePen = r.Pen("line",lineColor);
			var dotPen = r.Pen("dot",dotColor);
			var textBrush = r.Brush("text",Color.Black);
			var font = r.Font("default","宋体",9);
			
			// Draw Title
			var titlePos = 60;
			g.DrawEllipse(linePen,c.Right-titlePos,c.Top+(curvesCount+2)*12+2.5f,5,5);
			g.DrawString(title,font,textBrush,c.Right-titlePos+10,c.Top+(curvesCount+2)*12);
			
			// Draw Curve
			v.ForEach(
				firstPos=>g.DrawEllipse(dotPen,c.Axis,firstPos,2),
				(index,pos1,pos2)=>
					g.DrawEllipse(dotPen,c.Axis,pos1,2)
				     .DrawLine(linePen,c.Axis,pos1,pos2));
		}
		curvesCount++;
		return this;
	}
}

public static class Extension{
	public static void DrawArrow(this Graphics g,Pen p,int x1,int y1,int x2,int y2,int x3,int y3){
		g.DrawLines(p,new Point[]{
			new Point(x1,y1),
			new Point(x2,y2),
			new Point(x3,y3),
		});
	}
	
	public static Graphics DrawEllipse(this Graphics g,Pen p,Axis a,Tuple<float,float> pos,int r) {
		g.DrawEllipse(p,new RectangleF(a.OX+pos.Item1*a.XStep-r,a.OY-pos.Item2*a.YStep-r,r*2,r*2));
		return g;
	}
	public static Graphics DrawLine(this Graphics g,Pen p,Axis a,Tuple<float,float> pos1,Tuple<float,float> pos2){
		g.DrawLine(p,a.OX+pos1.Item1*a.XStep,a.OY-pos1.Item2*a.YStep,a.OX+pos2.Item1*a.XStep,a.OY-pos2.Item2*a.YStep);
		return g;
	}
	
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
	public static void ForEach<T>(this IEnumerable<T> list,Action<T> first,Action<int,T,T> next){
		var e = list.GetEnumerator();
		
		T a = default(T);
		if(e.MoveNext()){
			a = (T)e.Current;	
			first(a);
		}else{
			return;
		}
		
		int i = 1;
		while(e.MoveNext()){
			T b = (T)e.Current;
			next(i,a,b);
			i++;
			a = b;
		}
		
		return ;
	}
	public static Dictionary<K,V> Add<K,V>(this Dictionary<K,V> dict,K key,V value,Action<K,V> delete){
		if(dict.ContainsKey(key)){
			var oldValue = dict[key];
			delete(key,oldValue);
			dict.Add(key,value);
		}else{
			dict.Add(key,value);
		}
		return dict;
	}
	public static V TryGet<K,V>(this Dictionary<K,V> dict,K key){
		if(dict.ContainsKey(key)) return dict[key];
		return default(V);
	}
	
	public static Dictionary<K,V> ForEach<K,V>(this Dictionary<K,V> dict,Action<K,V> action){
		foreach(var p in dict){
			action(p.Key,p.Value);	
		}
		return dict;
	}
}

// Make a Generic Number?
interface INumber<T>
{
    T Zero();
    T Add(T a, T b);
	T Sub(T a, T b);
	T Div(T a, T b);
	T Multiply(T a, T b);
}

class Number:
    INumber<int>,
    INumber<float>,
	INumber<double>
{
    int INumber<int>.Zero(){ return 0; }
	int INumber<int>.Sub(int a,int b) { return a - b; }
    int INumber<int>.Add(int a,int b) { return a + b; }
	int INumber<int>.Div(int a,int b){ return a/b;}
	int INumber<int>.Multiply(int a,int b){return a*b;}
	
    float INumber<float>.Zero(){ return 0; }
    float INumber<float>.Add(float a,float b) { return a + b; }
	float INumber<float>.Sub(float a,float b) { return a - b; }
	float INumber<float>.Div(float a,float b){ return a/b;}
	float INumber<float>.Multiply(float a,float b){return a*b;}
	
	double INumber<double>.Zero(){ return 0; }
    double INumber<double>.Add(double a,double b) { return a + b; }
	double INumber<double>.Sub(double a,double b) { return a - b; }
	double INumber<double>.Div(double a,double b){ return a/b;}
	double INumber<double>.Multiply(double a,double b){return a*b;}

    public static Number N = new Number();
}