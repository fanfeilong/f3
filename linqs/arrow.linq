<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	
}

// Define other methods and classes here
public class ArrowRenderer{
   #region Properties

   private float width = 15;
   public float Width{
       get { return width; }
       set { width = value; }
   }

   private float theta = 0.5f;
   public float Theta{
       get { return theta; }
       set { theta = value; }
   }

   public void SetThetaInDegrees(float degrees){
       if (degrees <= 0) degrees = 1;
       if (degrees >= 180) degrees = 179;
       theta = (float)Math.PI / 180 * degrees;
   }

   private bool fillArrowHead = true;
   public bool FillArrowHead{
       get { return fillArrowHead; }
       set { fillArrowHead = value; }
   }

   private int numberOfBezierCurveNodes = 100;
   public int NumberOfBezierCurveNodes{
       get { return numberOfBezierCurveNodes; }
       set{
           if (value > 4) numberOfBezierCurveNodes = value;
       }
   }

   #endregion

   #region Constructors

   public ArrowRenderer() { }

   public ArrowRenderer(float width){
       this.width = width;
   }
   public ArrowRenderer(float width, float theta){
       this.width = width;
       this.theta = theta;
   }

   public ArrowRenderer(float width, float theta, bool fillArrowHead){
       this.width = width;
       this.theta = theta;
       this.fillArrowHead = fillArrowHead;
   }

   public ArrowRenderer(float width, float theta, bool fillArrowHead, int numberOfBezierCurveNodes){
       this.width = width;
       this.theta = theta;
       this.fillArrowHead = fillArrowHead;
       this.numberOfBezierCurveNodes = numberOfBezierCurveNodes;
   }

   #endregion

   #region DrawArrow

   public void DrawArrow(Graphics g, Pen pen, Brush brush, float x1, float y1, float x2, float y2){
       DrawArrow(g, pen, brush, new PointF(x1, y1), new PointF(x2, y2));
   }
   
   public void DrawArrow(Graphics g, Pen pen, Brush brush, PointF p1, PointF p2){
       PointF[] aptArrowHead = new PointF[3];

       // set first node to terminal point            
       aptArrowHead[0] = p2;

       Vector vecLine = new Vector(p2.X - p1.X, p2.Y - p1.Y);// build the line vector
       Vector vecLeft = new Vector(-vecLine[1], vecLine[0]);// build the arrow base vector - normal to the line

       // setup remaining arrow head points
       float lineLength = vecLine.Length;
       float th = Width / (2.0f * lineLength);
       float ta = Width / (2.0f * ((float)Math.Tan(Theta / 2.0f)) * lineLength);

       // find the base of the arrow
       PointF pBase = new PointF(aptArrowHead[0].X + -ta * vecLine[0], aptArrowHead[0].Y + -ta * vecLine[1]); //base of the arrow

       // build the points on the sides of the arrow
       aptArrowHead[1] = new PointF(pBase.X + th * vecLeft[0], pBase.Y + th * vecLeft[1]);
       aptArrowHead[2] = new PointF(pBase.X + -th * vecLeft[0], pBase.Y + -th * vecLeft[1]);

       g.DrawLine(pen, p1, pBase); //master line

       if (FillArrowHead) g.FillPolygon(brush, aptArrowHead); //fill arrow head if desired

       g.DrawPolygon(pen, aptArrowHead); //draw outline
   }

   #endregion

   #region DrawArrowOnCurve

   public void DrawArrowOnCurve(Graphics g, Pen pen, Brush brush, float[] controlPoints){
       if (controlPoints.Length != 8)
           throw new ArgumentException("controlPoints has to be valid float array 8 elements in length");
       DrawArrowOnCurve(g, pen, brush, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3],
                                       controlPoints[4], controlPoints[5], controlPoints[6], controlPoints[7]);
   }

   public void DrawArrowOnCurve(Graphics g, Pen pen, Brush brush, PointF p1, PointF p2, PointF p1c, PointF p2c){
       DrawArrowOnCurve(g, pen, brush, p1.X, p1.Y, p2.X, p2.Y, p1c.X, p1c.Y, p2c.X, p2c.Y);
   }

   public void DrawArrowOnCurve(Graphics g, Pen pen, Brush brush, float p1X, float p1Y, float p2X, float p2Y, float p1cX, float p1cY, float p2cX, float p2cY){
       if (numberOfBezierCurveNodes < 4) numberOfBezierCurveNodes = 4;
       PointF[] bezierLine = GetBezierCurveNodes(p1X, p1Y, p2X, p2Y, p1cX, p1cY, p2cX, p2cY, numberOfBezierCurveNodes);

       float arrowHeadHeight = width / (2.0f * ((float)Math.Tan(theta / 2.0f)));
       float distDelta = arrowHeadHeight;
       int lineTerminalNode = bezierLine.Length - 2;
       for (int i = bezierLine.Length - 2; i >= 1; i--){
           float currDist = GetDistance(bezierLine[i].X, bezierLine[i].Y, p2X, p2Y);
           float tempDelta = Math.Abs(arrowHeadHeight - currDist);

           if (tempDelta > distDelta) break;
           distDelta = tempDelta;
           lineTerminalNode = i;
       }

       PointF pBase = bezierLine[lineTerminalNode]; //set arrow base node
       arrowHeadHeight = GetDistance(pBase.X, pBase.Y, p2X, p2Y);

       PointF[] aptArrowHead = new PointF[3];
       aptArrowHead[0] = new PointF(p2X, p2Y); // set first node to terminal point

       float th = width / (2.0f * arrowHeadHeight);//coeficient used for remaining arrow head points setup

	   // build the points on the remaining sides of the arrow
       aptArrowHead[1] = new PointF(pBase.X + th * (pBase.Y - p2Y), pBase.Y + th * (p2X - pBase.X));
       aptArrowHead[2] = new PointF(pBase.X + th * (p2Y - pBase.Y), pBase.Y + th * (pBase.X - p2X));

       g.DrawCurve(pen, bezierLine, 0, lineTerminalNode); //master line
       if (FillArrowHead) g.FillPolygon(brush, aptArrowHead); //fill arrow head if desired
       g.DrawPolygon(pen, aptArrowHead); //draw outline
   }

   private PointF[] GetBezierCurveNodes(PointF p1, PointF p2, PointF p1c, PointF p2c, int numberOfNodes){
       return GetBezierCurveNodes(p1.X, p1.Y, p2.X, p2.Y, p1c.X, p1c.Y, p2c.X, p2c.Y, numberOfNodes);
   }

   protected PointF[] GetBezierCurveNodes(float p1X, float p1Y, float p2X, float p2Y, float p1cX, float p1cY, float p2cX, float p2cY, int numberOfNodes){
       PointF[] apt = new PointF[numberOfNodes];

       float dt = 1f / (apt.Length - 1);
       float t = -dt;
       for (int i = 0; i < apt.Length; i++){
           t += dt;
           float tt = t * t;
           float ttt = tt * t;
           float tt1 = (1 - t) * (1 - t);
           float ttt1 = tt1 * (1 - t);

           float x = ttt1 * p1X +
                     3 * t * tt1 * p1cX +
                     3 * tt * (1 - t) * p2cX +
                     ttt * p2X;

           float y = ttt1 * p1Y +
                     3 * t * tt1 * p1cY +
                     3 * tt * (1 - t) * p2cY +
                     ttt * p2Y;

           apt[i] = new PointF(x, y);
       }
       return apt;
   }

   protected float GetDistance(float x1, float y1, float x2, float y2){
       return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
   }

   #endregion

   #region DrawArrows

   public void DrawArrows(Graphics g, Pen pen, Brush brush, PointF[] apf){
       DrawArrows(g, pen, brush, apf, 0, 0);
   }

   public void DrawArrows(Graphics g, Pen pen, Brush brush, PointF[] apf, float startOffset, float endOffset){
       if (apf.Length < 2) throw new ArgumentException("PointF[] apf has to be at least 2 in length");
       PointF[] newApf = new PointF[apf.Length + 1];
       Array.Copy(apf, newApf, apf.Length);
       newApf[newApf.Length - 1] = newApf[0];

       PointF fromP = newApf[0], toP;
       for (int i = 0; i < newApf.Length - 1; i++){
           toP = newApf[i + 1];
           Vector vecLine = new Vector(toP.X - fromP.X, toP.Y - fromP.Y);
           Vector vecStart = vecLine - (vecLine.Length - startOffset);
           PointF nFromP = new PointF(fromP.X + vecStart.X, fromP.Y + vecStart.Y);
           Vector vecEnd = vecLine - endOffset;
           PointF nToP = new PointF(fromP.X + vecEnd.X, fromP.Y + vecEnd.Y);
           DrawArrow(g, pen, brush, nFromP, nToP);
           fromP = toP;
       }
   }

   public void DrawArrows(Graphics g, Pen pen, Brush brush, PointF[] apf, float[] startOffsets, float[] endOffsets){
       if (apf.Length < 2) throw new ArgumentException("PointF[] apf has to be at least 2 in length");
       if (apf.Length != startOffsets.Length || apf.Length != endOffsets.Length ||
           endOffsets.Length != startOffsets.Length)
           throw new ArgumentException("apf, startOffsets and endOffsets have to be arrays of equal length");

       PointF[] newApf = new PointF[apf.Length + 1];
       Array.Copy(apf, newApf, apf.Length);
       newApf[newApf.Length - 1] = newApf[0];

       PointF fromP = newApf[0], toP;
       for (int i = 0; i < newApf.Length - 1; i++){
           toP = newApf[i + 1];
           Vector vecLine = new Vector(toP.X - fromP.X, toP.Y - fromP.Y);
           Vector vecStart = vecLine - (vecLine.Length - startOffsets[i]);
           PointF nFromP = new PointF(fromP.X + vecStart.X, fromP.Y + vecStart.Y);
           Vector vecEnd = vecLine - endOffsets[i];
           PointF nToP = new PointF(fromP.X + vecEnd.X, fromP.Y + vecEnd.Y);
           DrawArrow(g, pen, brush, nFromP, nToP);
           fromP = toP;
       }
   }

   #endregion
}

