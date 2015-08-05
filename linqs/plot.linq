<Query Kind="Program">
  <Namespace>System</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

void Main(){
	var plot = new Plot(20,20,20,20,10,10);

	var x = new float[]{
		1,2,4,5,7,3
	};
	var y = new float[]{
		1,2,5,6,8,9
    };
    var names = new string[]{
        "nice","to","meet","you","and","then"
    };
    var colors = new Color[]{
        Color.Red,
        Color.LightBlue,
        Color.Green,
        Color.Orange,
        Color.Yellow,
        Color.DarkGray
    };
	
    // Plot Curvers
	plot.Reset()
        .Axis()
	    .Curve("AHPW-AIR",x,y,Color.Green,Color.Blue)
		.Curve("LOF-AIR",y,x,Color.Red,Color.Blue)
        .Dump()
    
    // Plot Histogram
        .Reset()
		.Axis()
        .Histogram("test",x,y,Color.LightGray,Color.LightBlue)
        .Dump()
        
    // Plot Pie
        .Reset()
        .Pie("test",x,names,colors)
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
	public int AxisMargin{get;private set;}
	
	public Canvas(int left,int top,int width,int height,int xstep,int ystep){
		Bitmap = new Bitmap(width,height);
		
		Left = left;
		Top = top;
		
		Width = width-2*left;
		Height = height-2*top;
		
		Bottom = top+Height;
		Right = left+Width;
		
		XStep = xstep;
		YStep = ystep;
		XSize = (int)Math.Floor((Width+XStep)*1.0/xstep);
		YSize = (int)Math.Floor((Height+YStep)*1.0/ystep);
		
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
		Width  = marginX*2+xstep*xsize+xstep/2;
		Height = marginY*2+ystep*ysize+ystep/2;
		Canvas = new Canvas(marginX,marginY,Width,Height,xstep,ystep);
    }
    public Plot Reset() {
        var c = Canvas;
        using (var r = c.AsRender()) {
            var g = r.Graphics;
            g.Clear(Color.White);
        }
        return this;
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
			//g.DrawArrow(p,c.Axis,c.Width*1.0F,0.0F,5,45);
			//g.DrawArrow(p,c.Axis,c.Width*1.0F/2,c.Height*1.0F/2,5,45);
			
			for(int i=0;i<c.XSize;i++){
				// Draw Axis
				var x1 = c.Left+c.XStep*i;
				var y1 = c.Bottom;
				var x2 = x1;
				var y2 = y1-5;
				g.DrawLine(p,x1,y1,x2,y2);
				
				// Draw Label
				int shift=i.CarryNumber();
				g.DrawString(i.ToString(),f,b,x1-3,y1+5);
			}
			
			// Draw Y Axis
			g.DrawLine(p,c.Left,c.Bottom,c.Left,c.Top);
			g.DrawArrow(p,c.Left-3,c.Top+3,c.Left,c.Top,c.Left+3,c.Top+3);
			//g.DrawArrow(p,c.Axis,0.0F,c.Height*1.0F,5,45);
			
			for(int i=0;i<c.YSize;i++){
				// Draw Axis
				var x1 = c.Left;
				var y1 = c.Bottom-c.YStep*i;
				var x2 = x1+5;
				var y2 = y1;
				g.DrawLine(p,x1,y1,x2,y2);
				
				// Draw Lable
				int shift=i.CarryNumber();
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
	
	public Plot Histogram(string title,float[] x,float[] y,Color lineColor,Color fillColor){
		var v = x.Zip(y,(a,b)=>Tuple.Create(a,b));
		var c = Canvas;
		var axis = c.Axis;
		using(var r = c.AsRender()){
			var g = r.Graphics;
			var linePen = r.Pen("line",lineColor);
			var fillBrush = r.Brush("fill",fillColor);
			var textBrush = r.Brush("text",fillColor);
			var font = r.Font("default","宋体",9);
			
			v.ForEach(pos=>
				g.DrawRectangle(linePen,axis,pos)
				 .FillRectangle(fillBrush,axis,pos));
		}
		return this;
	}

    public Plot Pie(string title, float[] values, string[] names, Color[] colors) {
        var list = 
        values.Zip(names,(a,b)=>Tuple.Create(a,b))
              .Zip(colors,(a,b)=>Tuple.Create(a.Item1,a.Item2,b));
        var maxValue = values.Sum();
              
        var c = Canvas;
        var axis = c.Axis;
        using (var r = c.AsRender()) {
            var g = r.Graphics;
            var font = r.Font("default", "宋体", 9);

            float degreeOffset=0;
            float angleOffset = 0;
            float ra = c.Width/2;
            float cx = axis.OX+c.Width/2;
            float cy = axis.OY-c.Height/2;
            
            int i=0;
            list.ForEach(t => {
                var value = t.Item1;
                var name  = t.Item2;
                var color = t.Item3;
                var pen = r.Pen(name,color);
                var brush = r.Brush(name,color);
                
                var p = value / maxValue;
                var degrees = (float)( p* 360.0);
                double angle = (degreeOffset+degrees) * Math.PI / 180.0;
                double cos = Math.Cos(angle)*ra;
                double sin = Math.Sin(angle)*ra;
                var x = (float)(cx+cos);
                var y = (float)(cy-sin);
                g.DrawLine(pen,cx,cy,x,y);


                angle = degreeOffset * Math.PI / 180.0;
                cos = Math.Cos(angle) * ra;
                sin = Math.Sin(angle) * ra;
                x = (float)(cx + cos);
                y = (float)(cy - sin);
                g.DrawLine(pen,cx,cy,x,y);

                //if(i==3)
                var start = angleOffset<0?angleOffset+1:0;
                var sweep =  0-(degrees>0?degrees-2:0);
                g.DrawArc(pen, c.Left, c.Top, c.Width,c.Height, start,sweep);
                g.FillPie(brush, c.Left+2, c.Top+2, c.Width-4, c.Height-4, start,sweep);
                i++;
                degreeOffset+=degrees;
                angleOffset-=degrees;
            });
        }
        return this;

    }

    public Plot Dump() {
        Bitmap.Dump();
        return this;
    }
}

public static class Extension {
    public static Graphics DrawArrow(this Graphics g, Pen p, int x1, int y1, int x2, int y2, int x3, int y3) {
        g.DrawLines(p, new Point[]{
            new Point(x1,y1),
            new Point(x2,y2),
            new Point(x3,y3),
        });
        return g;
    }
    public static Graphics DrawArrow(this Graphics g, Pen p, Axis o, float vecx,float vecy,double width,double degrees) {
		double angle = degrees*Math.PI/180.0;
		double cos = Math.Cos(angle);
		double sin = Math.Sin(angle);
		
		double a = width*cos;
		double b = width*sin;
		
		double d = Math.Sqrt(vecx*vecx+vecy*vecy);
		
		
		double r = (d-a)/d;
		double px = vecx*r;
		double py = vecy*r;
		double rotateAngle = Math.Atan(b/(d-a));
		
		cos = Math.Cos(rotateAngle);
		sin = Math.Sin(rotateAngle);
		
		float x1 = (float)(px * cos - py * sin);
		float y1 = (float)(px * sin + py * cos);
		
		cos = Math.Cos(-1.0*rotateAngle);
		sin = Math.Sin(-1.0*rotateAngle);
		float x2 = (float)(px * cos - py * sin);
		float y2 = (float)(px * sin + py * cos);
		
		g.DrawLines(p,new PointF[]{
			new PointF(o.OX+x1,o.OY-y1),
			new PointF(o.OX+vecx,o.OY-vecy),
			new PointF(o.OX+x2,o.OY-y2),
		});
		return g;
	}
	public static Graphics DrawEllipse(this Graphics g,Pen p,Axis a,Tuple<float,float> pos,int r) {
		g.DrawEllipse(p,new RectangleF(a.OX+pos.Item1*a.XStep-r,a.OY-pos.Item2*a.YStep-r,r*2,r*2));
		return g;
	}
	public static Graphics DrawLine(this Graphics g,Pen p,Axis a,Tuple<float,float> pos1,Tuple<float,float> pos2){
		g.DrawLine(p,a.OX+pos1.Item1*a.XStep,a.OY-pos1.Item2*a.YStep,a.OX+pos2.Item1*a.XStep,a.OY-pos2.Item2*a.YStep);
		return g;
	}
	public static Graphics DrawRectangle(this Graphics g,Pen p,Axis a,Tuple<float,float> pos){
		g.DrawRectangle(p,a.OX+pos.Item1*a.XStep+1,a.OY-pos.Item2*a.YStep,a.XStep-1,pos.Item2*a.YStep-1);
		return g;
	}
	public static Graphics FillRectangle(this Graphics g,Brush b,Axis a,Tuple<float,float> pos){
		g.FillRectangle(b,a.OX+pos.Item1*a.XStep+2,a.OY-pos.Item2*a.YStep+1,a.XStep-3,pos.Item2*a.YStep-3);
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
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list,Action<T> a){
		foreach(var item in list){
			a(item);
		}
		return list;
	}
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list,Action<T> first,Action<int,T,T> next){
		var e = list.GetEnumerator();
		
		T a = default(T);
		if(e.MoveNext()){
			a = (T)e.Current;	
			first(a);
		}else{
			return list;
		}
		
		int i = 1;
		while(e.MoveNext()){
			T b = (T)e.Current;
			next(i,a,b);
			i++;
			a = b;
		}
		
		return list;
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
	public static int CarryNumber(this int v){
		if(v==0) return 1;
		int shift=0;
		while(v>0) {shift++;v/=10;}
		return shift;
	}
	public static string ToBase64String(this Bitmap bmp, ImageFormat imageFormat){
		string base64String = string.Empty;
		MemoryStream memoryStream = new MemoryStream();
		bmp.Save(memoryStream, imageFormat);
		memoryStream.Position = 0;
		byte[] byteBuffer = memoryStream.ToArray();
		memoryStream.Close();
		base64String = Convert.ToBase64String(byteBuffer);
		byteBuffer = null;
		return base64String;
	}
	public static string ToBase64ImageTag(this Bitmap bmp, ImageFormat imageFormat,string alt){
		string imgTag = string.Empty;
		string base64String = string.Empty;
		base64String = bmp.ToBase64String(imageFormat);
		imgTag = "<img alt=\""+alt+"\" src=\"data:image/" + imageFormat.ToString() + ";base64,";
		imgTag += base64String + "\" ";
		imgTag += "width=\"" + bmp.Width.ToString() + "\" ";
		imgTag += "height=\"" + bmp.Height.ToString() + "\" />";
		return imgTag;
	}
}