<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.Formatters.Soap.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Activities.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xaml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceModel.Internals.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.DurableInstancing.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.VisualBasic.Activities.Compiler.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.VisualBasic.dll</Reference>
  <Namespace>System.Windows.Forms</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Activities</Namespace>
</Query>

void Main() {
	var bitmap = Image.FromFile(@"C:\Users\ffl\Pictures\carly-rae-jepsen.jpg");
	var bitmaps = new List<Image> {
		bitmap,bitmap,bitmap	
	};
	bitmaps.Display();
}

// Define other methods and classes here
public static class Extensions {
	public static void DisplayInGrid(this DataTable dt) {
		var grid = new DataGridView { DataSource = dt };
		PanelManager.DisplayControl(grid);
	}
	public static void Display(this IEnumerable<Image> bitmaps) {
		var panel = new Panel();
		panel.Paint += (sender, args) => {
			using (var g = panel.CreateGraphics()) {
				int x=0;
				int r=300;
				int marginx = 5;
				foreach (var bitmap in bitmaps) {
					g.ImageAt(bitmap, x, 0, r);
					x+=r+marginx;
				}
			}
		};
		PanelManager.DisplayControl(panel);
	}
	private static void ImageAt(this Graphics g, Image bitmap, int x, int y, int r) {
		var ow = bitmap.Width;
		var oh = bitmap.Height;
		var max = Math.Max(ow,oh);
		var neww = ow*r/max;
		var newh = oh*r/max;
		var newx = x+(r-neww)/2;
		var newy = y+(r-newh)/2;
		g.DrawImage(bitmap,newx,newy,neww,newh);
	}
}

public static class TimerExtension {
	public static System.Timers.Timer SetTimer(double interval, Action callback) {
		var timer = new System.Timers.Timer(interval);
		timer.Elapsed += (sender, args) => callback();
		timer.Enabled = true;
		return timer;
	}
	public static void KillTimer(System.Timers.Timer timer) {
		timer.Stop();
		return;
	}
	public static System.Timers.Timer SetOnceTimer(double interval, Action callback) {
		var timer = new System.Timers.Timer(interval);
		timer.Elapsed += (sender, args) => {
			timer.Stop();
			callback();
		};
        timer.Enabled = true;
		return timer;
	}
}