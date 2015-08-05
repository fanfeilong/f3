<Query Kind="Program">
  <Output>DataGrids</Output>
  <Reference Relative="..\lib\ClosedXML_v0.76.0.0\ClosedXML.dll">D:\dev\code_git_me\flinq\lib\ClosedXML_v0.76.0.0\ClosedXML.dll</Reference>
  <Reference Relative="..\lib\DocumentFormat.OpenXml.2.5\lib\DocumentFormat.OpenXml.dll">D:\dev\code_git_me\flinq\lib\DocumentFormat.OpenXml.2.5\lib\DocumentFormat.OpenXml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Drawing.dll</Reference>
  <Namespace>ClosedXML.Excel</Namespace>
  <Namespace>DocumentFormat.OpenXml</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

void Main() {
    var root = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath),@"..\data\").Dump();
    var output = Path.Combine(root, "statistic.md");

    // readme:
    // 
    // - shool dir can be
    // school1dir
    //   class1dir
    //     1234 xxx.xlsx
    //     455 yyy.xlsx
    //   class2dir
    //     334 aaa.xlsx
    //     987 bbb.xlsx
    //   [students.xlsx]:optional, if exist, it's format is
    //     1234 xxx blogurl1
    //     455  yyy blourl2

    //	var schoolInfos = new Dictionary<string,string>{
    //		{"school1","students.xlsx"},
    //		{"school2","students.xlsx"},
    //		{"school3",""}
    //	};

    var schoolInfos = new Dictionary<string, string>{
        {"齐鲁工业大学","students.xlsx"},
        {"广州商学院","students.xlsx"},
        {"石家庄铁道大学",""}
    };

    var sb = new StringBuilder();
    foreach (var schoolInfo in schoolInfos) {
        var schoolDir = Path.Combine(root, schoolInfo.Key);
        var blogUrlFile = Path.Combine(schoolDir, schoolInfo.Value);
        var school = new SchoolAssessment(schoolDir, blogUrlFile);
        school.Analyze();
        school.DumpMarkDown(sb);
    }
    sb.ToString().Dump();

    File.WriteAllText(output, sb.ToString());
}

public class AssessmentItem {
    public int Id { get; private set; }
    public string Description { get; private set; }
    public int Before { get; private set; }
    public int After { get; private set; }
    public double Air {
        get {
            return After * 1.0 / Before;
        }
    }
    public AssessmentItem(int id, string desc, int before, int after) {
        this.Id = id;
        this.Description = desc;
        this.Before = before;
        this.After = after;
    }
}

public enum State {
    Valid,
    InValid,
    HasMaxScore,
    NoChange,
    CopyedScore,
    Decrese,
}

public class StudentAssessment {
    public FileInfo FileInfo { get; private set; }
    public string FileName { get; private set; }
    public string Name { get; private set; }
    public string No { get; private set; }
    public List<AssessmentItem> Items { get; private set; }
    public State State { get; private set; }
    public bool InValid { get; private set; }
    public bool HasMaxScore { get; private set; }
    public bool NoChange { get; private set; }
    public bool CopyedScore { get; private set; }
    public bool Decrese { get; private set; }

    public ClassAssessment Class { get; private set; }
    public SchoolAssessment School { get; private set; }

    public double Air {
        get {
            return Items.Average(a => a.After * 1.0 / a.Before);
        }
    }
    public double AHPW { get; private set; }
    public double LOC { get; private set; }

    public StudentAssessment(FileInfo file, SchoolAssessment school, ClassAssessment class_) {
        this.FileInfo = file;
        this.FileName = Path.GetFileNameWithoutExtension(this.FileInfo.Name);
        this.Items = new List<AssessmentItem>();
        this.School = school;
        this.Class = class_;
        InValid = false;
        HasMaxScore = false;
        NoChange = false;
        CopyedScore = false;
        State = State.InValid;
    }

    public bool EqualItems(StudentAssessment other) {
        var q = this.Items.Zip(other.Items, (a, b) => a.Before == b.Before && a.After == b.After);
        return q.All(b => b);
    }

    private static readonly int ahpwPos1 = "你在这门课平均每周花".Length;
    private static readonly int locPos1 = "你在这门课中写了大约".Length;
    private static readonly string locRegex1 = @"\D*([0-9,\.]+)\D*";
    private static readonly string locRegex2 = @"\D*([0-9,\.]+)-([0-9,\.]+)\D*";
    private double extracValue(string str) {
        var match = Regex.Match(str, locRegex2);
        if (match.Groups.Count == 3) {
            var begin = match.ValueAt(1).ToDouble();
            var end = match.ValueAt(2).ToDouble();
            if (end < begin) {
                return 0;
            } else {
                var value = begin + (end - begin) / 2;
                return value;
            }
        }

        match = Regex.Match(str, locRegex1);
        if (match.Groups.Count == 2) {
            return match.ValueAt(1).ToDouble();
        }
        return 0;
    }

    public void Analyze() {
        // Parse No
        var regex = @"(\d+)\s*(\w+\s*\w+)";
        var match = Regex.Match(FileName, regex);
        this.No = match.ValueAt(1).Trim();
        this.Name = match.ValueAt(2).Replace(" ", "").Trim();

        // Read Xls
        //FileInfo.FullName.Dump();
        var workBook = new XLWorkbook(FileInfo.FullName);
        var workSheet = workBook.Worksheets.ElementAt(0);
        var rowssUsed = workSheet.RowsUsed();

        // Parse Hours Per Weerk and Lines Of Code
        var headerRow = rowssUsed.First();
        var headerDescs = headerRow.Cell(1).Value.ToString().Split(new char[] { '\n' }).Take(2).ToArray();

        var ahpwStr = headerDescs[0];
        var ahpwPos2 = ahpwStr.LastIndexOf("小时");
        var ahpw = ahpwStr.Substring(ahpwPos1, ahpwPos2 - ahpwPos1).Trim(new char[] { '_', ' ', });
        AHPW = extracValue(ahpw);

        var locStr = headerDescs[1];
        var locPos2 = locStr.LastIndexOf("行代码");
        var loc = locStr.Substring(locPos1, locPos2 - locPos1).Trim(new char[] { '_', ' ', });
        LOC = extracValue(loc);

        // Parse Assessment Items
        var rows = rowssUsed.Skip(3).Take(15);

        int i = 1;
        foreach (var row in rows) {
            // Get Cells
            var cells = row.CellsUsed();
            var cellCount = cells.Count();

            string desc = "";
            int before = 0;
            int after = 0;
            do {
                if (cellCount != 1 && cellCount != 3) {
                    InValid = true;
                    break;
                }

                // Parse Description
                desc = cells.ElementAt(0).Value.ToString();
                if (cellCount == 1) {
                    InValid = true;
                    break;
                }

                // Parse Before
                if (!Int32.TryParse(cells.ElementAt(1).Value.ToString(), out before)) {
                    InValid = true;
                    break;
                }

                // Parse After
                if (!Int32.TryParse(cells.ElementAt(2).Value.ToString(), out after)) {
                    InValid = true;
                    break;
                }

                if (before == 0 || after == 0) {
                    InValid = true;
                    break;
                }

            } while (false);

            var ass = new AssessmentItem(i++, desc, before, after);
            Items.Add(ass);
        }

        // Check Invalid
        if (i != 16) {
            InValid = true;
        }

        if (InValid) {
            State = State.InValid;
            return;
        }

        // Check Has Max Score
        if (Items.Any(a => a.Before == 10 || a.After == 10)) {
            HasMaxScore = true;
            State = State.HasMaxScore;
            return;
        }

        if (Items.Any(a => a.Before > a.After)) {
            Decrese = true;
            State = State.Decrese;
            return;
        }

        // Check All Before and After Score Are No Change
        if (Items.All(a => a.Before == a.After)) {
            NoChange = true;
            State = State.NoChange;
            return;
        }

        // Check All Item's Score Are copy&paste 
        var first = Items.First();
        if (Items.All(a => a.Before == first.Before && a.After == first.After)) {
            CopyedScore = true;
            State = State.CopyedScore;
            return;
        }

        if (AHPW == 0 || LOC == 0) {
            State = State.InValid;
            return;
        }

        State = State.Valid;
    }

    public void DumpMarkDown(StringBuilder sb) {
        sb.Append("|");
        if (School.BlogTable.ContainsKey(Name) && !string.IsNullOrEmpty(School.BlogTable[Name].Url)) {
            sb.Append("[")
              .Append(No)
              .Append("](")
              .Append(School.BlogTable[Name].Url)
              .Append(")");
        } else {
            sb.Append(No);
            //Name.Dump();
        }
        sb.Append("|");

        foreach (var ass in Items) {
            sb.Append(ass.Before)
              .Append("/")
              .Append(ass.After)
              .Append("|");
        }

        sb.AppendFormat("{0}|", AHPW)
          .AppendFormat("{0}|", LOC)
          .AppendFormat("{0:##.00}|", Air);
        sb.AppendLine();
    }

    public string ToItemsString() {
        var sb = new StringBuilder();
        sb.Append("|");
        if (School.BlogTable.ContainsKey(No)) {
            sb.Append(No);
        }
        sb.Append("|");

        foreach (var ass in Items) {
            sb.Append(ass.Before)
              .Append("/")
              .Append(ass.After)
              .Append("|");
        }
        sb.AppendFormat("{0}|", AHPW)
          .AppendFormat("{0}|", LOC)
          .AppendFormat("{0:##.00}|", Air);
        return sb.ToString();
    }
}

public class ClassAssessment {
    public DirectoryInfo DirectoryInfo { get; private set; }
    public string Name { get; private set; }
    public int TotalCount { get; private set; }
    public int ValidCount { get; private set; }
    public int InValidCount { get; private set; }
    public int PerfectScoreCount { get; private set; }
    public int NoChangeCount { get; private set; }
    public int CopyedScoreCount { get; private set; }
    public int DecreaseCount { get; private set; }
    public int CopyedLineCount { get; set; }

    public SchoolAssessment School { get; private set; }
    public List<StudentAssessment> Students { get; private set; }

    private IEnumerable<IEnumerable<AssessmentItem>> table;

    public double Air {
        get {
            return Students.Where(s => s.State == State.Valid).Average(s => s.Air);
        }
    }

    public Tuple<int, double> MinBefore {
        get {
            return table.Min(10.0, items => items.Average(item => item.Before * 1.0), (i, average, items) => Tuple.Create(i, average));
        }
    }
    public Tuple<int, double> MaxBefore {
        get {
            return table.Max(0.0, items => items.Average(item => item.Before * 1.0), (i, average, items) => Tuple.Create(i, average));
        }
    }

    public Tuple<int, double> MinAfter {
        get {
            return table.Min(10.0, items => items.Average(item => item.After * 1.0), (i, average, items) => Tuple.Create(i, average));
        }
    }
    public Tuple<int, double> MaxAfter {
        get {
            return table.Max(0.0, items => items.Average(item => item.After * 1.0), (i, average, items) => Tuple.Create(i, average));
        }
    }

    public Tuple<int, double> MinAir {
        get {
            return table.Min(100.0, items => items.Average(item => item.After * 1.0 / item.Before), (i, average, items) => Tuple.Create(i, average));
        }
    }
    public Tuple<int, double> MaxAir {
        get {
            return table.Max(0.0, items => items.Average(item => item.After * 1.0 / item.Before), (i, average, items) => Tuple.Create(i, average));
        }
    }

    public double AHPW {
        get {
            return Students.Average(s => s.AHPW);
        }
    }
    public double LOC {
        get {
            return Students.Average(s => s.LOC);
        }
    }

    public ClassAssessment(DirectoryInfo dir, SchoolAssessment school) {
        this.DirectoryInfo = dir;
        this.Name = this.DirectoryInfo.Name;
        this.Students = new List<StudentAssessment>();
        this.School = school;
        foreach (var studentFile in dir.GetFiles()) {
            this.Students.Add(new StudentAssessment(studentFile, School, this));
        }
        this.TotalCount = Students.Count;
    }

    public ClassAssessment(List<StudentAssessment> students) {
        this.Students = students.Where(s => s.State == State.Valid).ToList();
        this.table =
        Enumerable.Range(1, 15)
        .Select(i => Students.Select(s => s.Items[i - 1]));
    }

    public void Analyze() {
        foreach (var s in Students) {
            s.Analyze();
            switch (s.State) {
                case State.InValid:
                    this.InValidCount++;
                    break;
                case State.HasMaxScore:
                    this.PerfectScoreCount++;
                    break;
                case State.NoChange:
                    this.NoChangeCount++;
                    break;
                case State.CopyedScore:
                    this.CopyedScoreCount++;
                    break;
                case State.Decrese:
                    this.DecreaseCount++;
                    break;
                case State.Valid:
                    ValidCount++;
                    break;
                default:
                    break;
            }
        }

        Students = Students.Where(s => s.State == State.Valid).OrderBy(s => s.Air).ToList();

        int l = Students.Count - 1;

        var removeIndexs = new List<int>();
        while (l > 1) {
            var s = Students[l];
            int j = l - 1;
            int repeatCount = 0;
            while (j >= 0) {
                var next = Students[j];
                if (s.Air == next.Air) {
                    if (s.EqualItems(next)) {
                        CopyedLineCount++;
                        removeIndexs.Add(j);
                        repeatCount++;
                    }
                } else {
                    break;
                }
                j--;
            }
            l = l - repeatCount - 1;
        }

        foreach (var removeIndex in removeIndexs) {
            ValidCount--;
            Students.RemoveAt(removeIndex);
        }

        table =
        Enumerable.Range(1, 15)
        .Select(i => Students.Where(s => s.State == State.Valid).Select(s => s.Items[i - 1]));
    }

    public void DumpMarkDown(StringBuilder sb) {
        var minBefore = MinBefore;
        var maxBefore = MaxBefore;

        var minAfter = MinAfter;
        var maxAfter = MaxAfter;

        var minAir = MinAir;
        var maxAir = MaxAir;

        var air = Air;

        // Dump Class Statistic 
        sb.AppendLine();
        sb.AppendLine("## " + Name + "\r\n")
          .AppendLine(string.Format("- 提交表格总数：{0}", TotalCount))
          .AppendLine(string.Format("- 剔除的无效、含有满分、拷贝粘贴、Before>After、Before===After表格数：{0}", TotalCount - ValidCount))
          .AppendLine(string.Format("- 剩余可用表格总数：{0}", ValidCount))
          .AppendLine("- **平均提高幅度**:" + Air.ToString("##.00"))
          .AppendLine("- Before:")
          .AppendFormat("    - 最弱: 单项Id:{0}，平均值:{1:##.00}\r\n", minBefore.Item1, minBefore.Item2)
          .AppendFormat("    - 最强: 单项Id:{0}，平均值:{1:##.00}\r\n", maxBefore.Item1, maxBefore.Item2)
          .AppendLine("- After:")
          .AppendFormat("    - 最弱: 单项Id:{0}，平均值:{1:##.00}\r\n", minAfter.Item1, minAfter.Item2)
          .AppendFormat("    - 最强: 单项Id:{0}，平均值:{1:##.00}\r\n", maxAfter.Item1, maxAfter.Item2)
          .AppendFormat("    - 提高最少: 单项Id:{0}，平均值:{1:##.00}\r\n", minAir.Item1, minAir.Item2)
          .AppendFormat("    - 提高最多: 单项Id:{0}，平均值:{1:##.00}\r\n", maxAir.Item1, maxAir.Item2)
          .AppendFormat("- Cost:\r\n")
          .AppendFormat("    - 平均耗时：{0:##.00} 小时/每人每周\r\n", AHPW)
          .AppendFormat("    - 平均代码行数：{0:##} 行/每人\r\n", LOC)
          .AppendLine();

        var students = Students.Where(s => s.State == State.Valid);


        // Plot ASIIC Points
        sb.Append("|单项编号|班级单项分布图|\r\n|:--|:--|\r\n");
        for (int i = 0; i < 15; i++) {
            sb.Append("|").Append(i + 1).Append("|");
            var stu = students.OrderBy(s => s.Items[i].Air);
            var itemAirMax = stu.Max(s => s.Items[i].Air);
            foreach (var s in stu) {
                int iAIR = (int)(s.Items[i].Air / itemAirMax * 8);
                //string.Format("{0}/{1},{2}",s.Items[i].Air,itemAirMax,iAIR).Dump();
                sb.Append(iAIR.ToSpark());
            }
            sb.Append("|\r\n");
        }

        sb.Append("|AHPW|");
        double max = students.Max(s => s.AHPW);
        foreach (var s in students.OrderBy(s => s.AHPW)) {
            int si = (int)(s.AHPW / max * 8);
            sb.Append(si.ToSpark());
        }
        sb.Append("|\r\n");

        sb.Append("|LOC|");
        max = students.Max(s => s.LOC);
        foreach (var s in students.OrderBy(s => s.LOC)) {
            int si = (int)(s.LOC / max * 8);
            sb.Append(si.ToSpark());
        }
        sb.Append("|\r\n");

        sb.Append("|AIR|");
        max = students.Max(s => s.Air);
        foreach (var s in students.OrderBy(s => s.Air)) {
            int si = (int)(s.Air / max * 8);
            sb.Append(si.ToSpark());
        }
        sb.Append("|\r\n");

        sb.AppendLine();
        /**/

        // Plot Curve
        students.DumpPlot(sb, Name);


        // Dump Students Statistic 
        sb.Append("|学号|");
        for (int i = 1; i < 16; i++) {
            sb.Append(string.Format("{0}", i)).Append("|");
        }
        sb.Append("AHPW|LOC|AIR|");
        sb.AppendLine();

        sb.Append("|:--|");
        for (int i = 1; i < 16; i++) {
            sb.Append(":--|");
        }
        sb.Append(":--|");
        sb.AppendLine();

        foreach (var s in students) {
            s.DumpMarkDown(sb);
        }
    }
}

public class BlogItem {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
}

public class SchoolAssessment {
    public string Name { get; private set; }
    public DirectoryInfo DirectoryInfo { get; private set; }
    public string BlogFile { get; private set; }
    public List<ClassAssessment> Classes { get; private set; }
    public Dictionary<string, BlogItem> BlogTable { get; private set; }

    private ClassAssessment UnionClass { get; set; }

    public SchoolAssessment(string dir, string BlogFile) {
        this.DirectoryInfo = new DirectoryInfo(dir);
        this.BlogFile = BlogFile;
        this.Name = this.DirectoryInfo.Name;

        this.Classes = new List<ClassAssessment>();
        foreach (var classDirectoryInfo in this.DirectoryInfo.GetDirectories()) {
            this.Classes.Add(new ClassAssessment(classDirectoryInfo, this));
        }
    }

    private Dictionary<string, BlogItem> FetchBlogItems(string file) {
        var linkTableBook = new XLWorkbook(file);
        var linkSheet = linkTableBook.Worksheets.ElementAt(0);
        var linkRows = linkSheet.RowsUsed();
        var links = new Dictionary<string, BlogItem>();
        foreach (var row in linkRows) {
            var cells = row.CellsUsed();
            try {
                if (cells.Count() >= 3) {
                    var id = cells.ElementAt(0).Value.ToString().Trim();
                    var name = cells.ElementAt(1).Value.ToString().Trim().Replace(" ", "");
                    if (cells.ElementAt(2).HasHyperlink) {
                        var blog = cells.ElementAt(2).Hyperlink.ExternalAddress.AbsoluteUri;
                        links.Add(name, new BlogItem() { Id = id, Name = name, Url = blog });
                    }
                }
            } catch {
                //Ignore
            }
        }
        return links;
    }
    public void Analyze() {
        // Parse Blog Urls File
        if (File.Exists(BlogFile)) {
            BlogTable = FetchBlogItems(BlogFile);
        } else {
            BlogTable = new Dictionary<string, BlogItem>();
        }

        // Analyze Classes
        foreach (var c in Classes) {
            c.Analyze();
        }

        var students = new List<StudentAssessment>();
        students = Classes.Aggregate(students, (a, b) => a.Union(b.Students).ToList());
        UnionClass = new ClassAssessment(students);


    }

    private static bool hasDumpDesc = false;
    public void DumpMarkDown(StringBuilder sb) {

        // Dump AssDescription Table
        if (!hasDumpDesc) {
            hasDumpDesc = true;
            var firstClass = Classes.First();
            var firstValidStudent = firstClass.Students.Where(s => s.State == State.Valid).First();

            sb.AppendLine("# 单项描述表")
            .Append("|单项编号|单项描述|")
            .AppendLine()
            .Append("|:--|:--|")
            .AppendLine();

            int j = 1;
            foreach (var ass in firstValidStudent.Items) {
                sb.Append("|单项")
                .Append(j++)
                .Append("|")
                .Append(ass.Description)
                .Append("|")
                .AppendLine();
            }
            sb.AppendLine("|AHPW|SE: Average Hours Per Week 平均每周所花时间|")
            .AppendLine("|LOC|SE: Lines Of Code 代码行数|")
            .AppendLine("|AIR|SE：Average Improvement Rate 平均提高幅度|")
            .AppendLine();
        } else {
            sb.AppendLine();
        }

        // Dump School Statictis
        var totalCount = Classes.Sum(c => c.TotalCount);
        var validCount = Classes.Sum(c => c.ValidCount);
        var invalidCount = totalCount - validCount;

        var minBefore = UnionClass.MinBefore;
        var maxBefore = UnionClass.MaxBefore;

        var minAfter = UnionClass.MinAfter;
        var maxAfter = UnionClass.MaxAfter;

        var minAir = UnionClass.MinAir;
        var maxAir = UnionClass.MaxAir;

        var air = UnionClass.Air;

        sb.AppendLine()
          .AppendFormat("# {0}总合计:", Name)
          .AppendLine()
          .AppendLine(string.Format("- 提交表格总数：{0}", totalCount))
          .AppendLine(string.Format("- 剔除的无效、含有满分、复制单元格、Before>After、Before===After表格数：{0}", invalidCount))
          .AppendLine(string.Format("- 剩余可用表格总数：{0}", validCount))
          .AppendLine(string.Format("- 平均提高幅度:{0}", air.ToString("##.00")))
          .AppendLine("- Before:")
          .AppendFormat("    - 最弱: 单项Id:{0}，平均值:{1:##.00}\r\n", minBefore.Item1, minBefore.Item2)
          .AppendFormat("    - 最强: 单项Id:{0}，平均值:{1:##.00}\r\n", maxBefore.Item1, maxBefore.Item2)
          .AppendLine("- After:")
          .AppendFormat("    - 最弱: 单项Id:{0}，平均值:{1:##.00}\r\n", minAfter.Item1, minAfter.Item2)
          .AppendFormat("    - 最强: 单项Id:{0}，平均值:{1:##.00}\r\n", maxAfter.Item1, maxAfter.Item2)
          .AppendFormat("    - 提高最少: 单项Id:{0}，平均值:{1:##.00}\r\n", minAir.Item1, minAir.Item2)
          .AppendFormat("    - 提高最多: 单项Id:{0}，平均值:{1:##.00}\r\n", maxAir.Item1, maxAir.Item2)
          .AppendFormat("- Cost:\r\n")
          .AppendFormat("    - 平均耗时：{0:##.00} 小时/每人每周\r\n", UnionClass.AHPW)
          .AppendFormat("    - 平均代码行数：{0:##} 行/每人\r\n", UnionClass.LOC)
          .AppendLine();

        UnionClass.Students.DumpPlot(sb, Name, 20);

        // Dump Class Statistic
        foreach (var c in Classes) {
            c.DumpMarkDown(sb);
        }
    }
}

public class Render : IDisposable {
    private bool disposed = false;

    public Graphics Graphics { get; private set; }
    private Dictionary<string, Pen> Pens { get; set; }
    private Dictionary<string, Brush> Brushes { get; set; }
    private Dictionary<string, Font> Fonts { get; set; }

    public Render(Bitmap bitmap) {
        Graphics = Graphics.FromImage(bitmap);
        Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        Pens = new Dictionary<string, Pen>();
        Brushes = new Dictionary<string, Brush>();
        Fonts = new Dictionary<string, Font>();
    }
    public Pen Pen(string name, Color c) {
        var pen = new Pen(c);
        Pens.Add(name, pen, (n, p) => p.Dispose());
        return pen;
    }
    public Brush Brush(string name, Color c) {
        var brush = new SolidBrush(c);
        Brushes.Add(name, brush, (n, b) => b.Dispose());
        return brush;
    }
    public Font Font(string name, string fontName, int fontSize) {
        var font = new Font(new FontFamily(fontName), fontSize);
        Fonts.Add(name, font, (n, f) => f.Dispose());
        return font;
    }

    #region IDispose
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing) {
        if (disposed) return;
        disposed = true;

        // NOTE:
        // Release unmanaged resources here

        if (!disposing) return;

        // NOTE:
        // Release managed resources here
        Pens.ForEach((n, p) => p.Dispose());
        Brushes.ForEach((n, b) => b.Dispose());
        Fonts.ForEach((n, f) => f.Dispose());
    }
    #endregion
}

public static class Lambda {
    public static Action<T, T, T, T> Action<T>(Action<T, T, T, T> a) {
        return a;
    }
}

public class Axis {
    public int OX { get; private set; }
    public int OY { get; private set; }
    public int XStep { get; private set; }
    public int YStep { get; private set; }
    public Axis(int ox, int oy, int xstep, int ystep) {
        OX = ox;
        OY = oy;
        XStep = xstep;
        YStep = ystep;
    }
}

public class Canvas {
    public Bitmap Bitmap { get; private set; }
    public int Left { get; private set; }
    public int Top { get; private set; }
    public int Bottom { get; private set; }
    public int Right { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int XStep { get; private set; }
    public int YStep { get; private set; }
    public int XSize { get; private set; }
    public int YSize { get; private set; }
    public Axis Axis { get; private set; }
    public int AxisMargin { get; private set; }

    public Canvas(int left, int top, int width, int height, int xstep, int ystep) {
        Bitmap = new Bitmap(width, height);

        Left = left;
        Top = top;

        Width = width - 2 * left;
        Height = height - 2 * top;

        Bottom = top + Height;
        Right = left + Width;

        XStep = xstep;
        YStep = ystep;
        XSize = (int)Math.Floor((Width + XStep) * 1.0 / xstep);
        YSize = (int)Math.Floor((Height + YStep) * 1.0 / ystep);

        Axis = new Axis(Left, Bottom, XStep, YStep);
    }

    public Render AsRender() {
        return new Render(Bitmap);
    }
}

public class Plot {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Canvas Canvas { get; private set; }
    public Bitmap Bitmap {
        get {
            return Canvas.Bitmap;
        }
    }

    public Plot(int marginX, int marginY, int xstep, int ystep, int xsize, int ysize) {
        Width = marginX * 2 + xstep * xsize + xstep / 2;
        Height = marginY * 2 + ystep * ysize + ystep / 2;
        Canvas = new Canvas(marginX, marginY, Width, Height, xstep, ystep);
    }
    public Plot Axis() {
        var c = Canvas;
        using (var r = c.AsRender()) {

            // Prepare
            var g = r.Graphics;
            var p = r.Pen("line", Color.Black);
            var b = r.Brush("text", Color.Black);
            var f = r.Font("default", "宋体", 9);


            // Draw X Axis
            g.DrawLine(p, c.Left, c.Bottom, c.Right, c.Bottom);
            g.DrawArrow(p, c.Right - 3, c.Bottom - 3, c.Right, c.Bottom, c.Right - 3, c.Bottom + 3);
            //g.DrawArrow(p,c.Axis,c.Width*1.0F,0.0F,5,45);
            //g.DrawArrow(p,c.Axis,c.Width*1.0F/2,c.Height*1.0F/2,5,45);

            for (int i = 0; i < c.XSize; i++) {
                // Draw Axis
                var x1 = c.Left + c.XStep * i;
                var y1 = c.Bottom;
                var x2 = x1;
                var y2 = y1 - 5;
                g.DrawLine(p, x1, y1, x2, y2);

                // Draw Label
                int shift = i.CarryNumber();
                g.DrawString(i.ToString(), f, b, x1 - 3, y1 + 5);
            }

            // Draw Y Axis
            g.DrawLine(p, c.Left, c.Bottom, c.Left, c.Top);
            g.DrawArrow(p, c.Left - 3, c.Top + 3, c.Left, c.Top, c.Left + 3, c.Top + 3);
            //g.DrawArrow(p,c.Axis,0.0F,c.Height*1.0F,5,45);

            for (int i = 0; i < c.YSize; i++) {
                // Draw Axis
                var x1 = c.Left;
                var y1 = c.Bottom - c.YStep * i;
                var x2 = x1 + 5;
                var y2 = y1;
                g.DrawLine(p, x1, y1, x2, y2);

                // Draw Lable
                int shift = i.CarryNumber();
                g.DrawString(i.ToString(), f, b, x1 - 10 * shift, y1 - 3);
            }
        }
        return this;
    }

    private int curvesCount = 0;
    public Plot Curve(string title, float[] x, float[] y, Color lineColor, Color dotColor) {
        var v = x.Zip(y, (a, b) => Tuple.Create(a, b));

        var c = Canvas;
        using (var r = c.AsRender()) {

            // Prepare
            var g = r.Graphics;
            var linePen = r.Pen("line", lineColor);
            var dotPen = r.Pen("dot", dotColor);
            var textBrush = r.Brush("text", Color.Black);
            var font = r.Font("default", "宋体", 9);

            // Draw Title
            var titlePos = 60;
            g.DrawEllipse(linePen, c.Right - titlePos, c.Top + (curvesCount + 2) * 12 + 2.5f, 5, 5);
            g.DrawString(title, font, textBrush, c.Right - titlePos + 10, c.Top + (curvesCount + 2) * 12);

            // Draw Curve
            v.ForEach(
                firstPos => g.DrawEllipse(dotPen, c.Axis, firstPos, 2),
                (index, pos1, pos2) =>
                    g.DrawEllipse(dotPen, c.Axis, pos1, 2)
                     .DrawLine(linePen, c.Axis, pos1, pos2));
        }
        curvesCount++;
        return this;
    }

    public Plot Histogram(string title, float[] x, float[] y, Color lineColor, Color fillColor) {
        var v = x.Zip(y, (a, b) => Tuple.Create(a, b));
        var c = Canvas;
        var axis = c.Axis;
        using (var r = c.AsRender()) {
            var g = r.Graphics;
            var linePen = r.Pen("line", lineColor);
            var fillBrush = r.Brush("fill", fillColor);
            var textBrush = r.Brush("text", fillColor);
            var font = r.Font("default", "宋体", 9);

            v.ForEach(pos =>
                g.DrawRectangle(linePen, axis, pos)
                 .FillRectangle(fillBrush, axis, pos));
        }
        return this;
    }
}

public static class Extension {
    #region assessment
    public static U TryGet<T, U>(this Dictionary<T, U> dict, T key) {
        U u = default(U);
        dict.TryGetValue(key, out u);
        return u;
    }
    public static int Count<T, U>(this Dictionary<T, List<U>> dict, T key) {
        if (dict.ContainsKey(key)) {
            return dict[key].Count();
        } else {
            return 0;
        }
    }
    public static R Min<T, C, R>(this IEnumerable<T> list, C seed, Func<T, C> compareSelector, Func<int, C, T, R> valueSelector) where C : IComparable {
        C min = seed;
        T minT = default(T);
        int i = 0;
        int minI = 0;
        foreach (var t in list) {
            var value = compareSelector(t);
            if (value.CompareTo(min) <= 0) {
                min = value;
                minT = t;
                minI = i;
            }
            i++;
        }
        return valueSelector(minI, min, minT);
    }
    public static R Max<T, C, R>(this IEnumerable<T> list, C seed, Func<T, C> compareSelector, Func<int, C, T, R> valueSelector) where C : IComparable {
        C max = seed;
        T maxT = default(T);
        int i = 0;
        int maxI = 0;
        foreach (var t in list) {
            var value = compareSelector(t);
            if (value.CompareTo(max) >= 0) {
                max = value;
                maxT = t;
                maxI = i;
            }
            i++;
        }
        return valueSelector(maxI, max, maxT);
    }
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source) {
            if (seenKeys.Add(keySelector(element))) { yield return element; }
        }
    }
    public static string ValueAt(this Match match, int index) {
        if (index >= match.Groups.Count) {
            return "";
        }
        return match.Groups[index].Value;
    }
    public static double ToDouble(this string s) {
        double v = 0;
        Double.TryParse(s, out v);
        return v;
    }
    public static void DumpPlot(this IEnumerable<StudentAssessment> students, StringBuilder sb, string alt, int range = 10) {
        //if(true) return;
        var marginX = 20;
        var marginY = 20;
        var stepX = 20;
        var stepY = 20;
        var sizeX = range;
        var sizeY = range;

        var plot1 = new Plot(marginX, marginY, stepX, stepY, sizeX, sizeY);
        var plot2 = new Plot(marginX, marginY, stepX, stepY, sizeX, sizeY);
        var plot3 = new Plot(marginX, marginY, stepX, stepY, sizeX, sizeY);

        var maxAHPW = students.Max(s => s.AHPW);
        var maxLOC = students.Max(s => s.LOC);
        var maxsAir = students.Max(s => s.Air);

        var ahpw = students.OrderBy(s => s.AHPW).Select(s => new { AHPW = s.AHPW.Normal(maxAHPW, range), AIR = s.Air.Normal(maxsAir, range), LOC = s.LOC.Normal(maxLOC, range) });
        var loc = students.OrderBy(s => s.LOC).Select(s => new { LOC = s.LOC.Normal(maxLOC, range), AIR = s.Air.Normal(maxsAir, range) });

        var ahpwN = ahpw.Select(i => i.AHPW).ToArray();
        var airN1 = ahpw.Select(i => i.AIR).ToArray();

        var locN = loc.Select(i => i.LOC).ToArray();
        var airN2 = loc.Select(i => i.AIR).ToArray();

        var locN2 = ahpw.Select(i => i.LOC).ToArray();

        var image1 =
        plot1.Axis()
             .Curve("AHPW->AIR", ahpwN, airN1, Color.Green, Color.LightGreen)
             .Bitmap;

        var image2 =
        plot2.Axis()
             .Curve("LOC->AIR", locN, airN2, Color.Red, Color.LightPink)
             .Bitmap;

        var image3 =
        plot3.Axis()
             .Curve("AHPW->LOC", ahpwN, locN2, Color.Blue, Color.LightBlue)
             .Bitmap;

        sb.AppendLine(image1.ToBase64ImageTag(ImageFormat.Png, alt + " AHPW-AIR")).AppendLine();
        sb.AppendLine(image2.ToBase64ImageTag(ImageFormat.Png, alt + " LOC-AIR")).AppendLine();
        sb.AppendLine(image3.ToBase64ImageTag(ImageFormat.Png, alt + " AHPW-LOC")).AppendLine();
        //sb.AppendLine();
    }
    #endregion

    #region plot
    public static Graphics DrawArrow(this Graphics g, Pen p, int x1, int y1, int x2, int y2, int x3, int y3) {
        g.DrawLines(p, new Point[]{
            new Point(x1,y1),
            new Point(x2,y2),
            new Point(x3,y3),
        });
        return g;
    }
    public static Graphics DrawArrow(this Graphics g, Pen p, Axis o, float vecx, float vecy, double width, double degrees) {
        double angle = degrees * Math.PI / 180.0;
        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        double a = width * cos;
        double b = width * sin;

        double d = Math.Sqrt(vecx * vecx + vecy * vecy);


        double r = (d - a) / d;
        double px = vecx * r;
        double py = vecy * r;
        double rotateAngle = Math.Atan(b / (d - a));

        cos = Math.Cos(rotateAngle);
        sin = Math.Sin(rotateAngle);

        float x1 = (float)(px * cos - py * sin);
        float y1 = (float)(px * sin + py * cos);

        cos = Math.Cos(-1.0 * rotateAngle);
        sin = Math.Sin(-1.0 * rotateAngle);
        float x2 = (float)(px * cos - py * sin);
        float y2 = (float)(px * sin + py * cos);

        g.DrawLines(p, new PointF[]{
            new PointF(o.OX+x1,o.OY-y1),
            new PointF(o.OX+vecx,o.OY-vecy),
            new PointF(o.OX+x2,o.OY-y2),
        });
        return g;
    }
    public static Graphics DrawEllipse(this Graphics g, Pen p, Axis a, Tuple<float, float> pos, int r) {
        g.DrawEllipse(p, new RectangleF(a.OX + pos.Item1 * a.XStep - r, a.OY - pos.Item2 * a.YStep - r, r * 2, r * 2));
        return g;
    }
    public static Graphics DrawLine(this Graphics g, Pen p, Axis a, Tuple<float, float> pos1, Tuple<float, float> pos2) {
        g.DrawLine(p, a.OX + pos1.Item1 * a.XStep, a.OY - pos1.Item2 * a.YStep, a.OX + pos2.Item1 * a.XStep, a.OY - pos2.Item2 * a.YStep);
        return g;
    }
    public static Graphics DrawRectangle(this Graphics g, Pen p, Axis a, Tuple<float, float> pos) {
        g.DrawRectangle(p, a.OX + pos.Item1 * a.XStep + 1, a.OY - pos.Item2 * a.YStep, a.XStep - 1, pos.Item2 * a.YStep - 1);
        return g;
    }
    public static Graphics FillRectangle(this Graphics g, Brush b, Axis a, Tuple<float, float> pos) {
        g.FillRectangle(b, a.OX + pos.Item1 * a.XStep + 2, a.OY - pos.Item2 * a.YStep + 1, a.XStep - 3, pos.Item2 * a.YStep - 3);
        return g;
    }

    public static R Aggregate<T, R>(this IEnumerable<T> list, R seed, Func<R, T, R> first, Func<R, int, T, R> con) {
        if (!list.Any()) {
            return default(R);
        }

        var e = list.GetEnumerator();

        T a = default(T);

        R r = seed;
        if (e.MoveNext()) {
            a = (T)e.Current;
            r = first(seed, a);
        }

        int i = 1;
        while (e.MoveNext()) {
            a = (T)e.Current;
            r = con(r, i, a);
            i++;
        }

        return r;
    }
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> a) {
        foreach (var item in list) {
            a(item);
        }
        return list;
    }
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> first, Action<int, T, T> next) {
        var e = list.GetEnumerator();

        T a = default(T);
        if (e.MoveNext()) {
            a = (T)e.Current;
            first(a);
        } else {
            return list;
        }

        int i = 1;
        while (e.MoveNext()) {
            T b = (T)e.Current;
            next(i, a, b);
            i++;
            a = b;
        }

        return list;
    }
    public static Dictionary<K, V> Add<K, V>(this Dictionary<K, V> dict, K key, V value, Action<K, V> delete) {
        if (dict.ContainsKey(key)) {
            var oldValue = dict[key];
            delete(key, oldValue);
            dict.Add(key, value);
        } else {
            dict.Add(key, value);
        }
        return dict;
    }
    public static Dictionary<K, V> ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V> action) {
        foreach (var p in dict) {
            action(p.Key, p.Value);
        }
        return dict;
    }
    public static int CarryNumber(this int v) {
        if (v == 0) return 1;
        int shift = 0;
        while (v > 0) { shift++; v /= 10; }
        return shift;
    }

    public static string ToBase64String(this Bitmap bmp, ImageFormat imageFormat) {
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
    public static string ToBase64ImageTag(this Bitmap bmp, ImageFormat imageFormat, string alt) {
        string imgTag = string.Empty;
        string base64String = string.Empty;
        base64String = bmp.ToBase64String(imageFormat);
        imgTag = "<img alt=\"" + alt + "\" src=\"data:image/" + imageFormat.ToString() + ";base64,";
        imgTag += base64String + "\" ";
        imgTag += "width=\"" + bmp.Width.ToString() + "\" ";
        imgTag += "height=\"" + bmp.Height.ToString() + "\" />";
        return imgTag;
    }
    #endregion

    #region spark
    private static readonly string spark = "▁▁▂▃▄▅▆▇█";
    public static char ToSpark(this int v) {
        try {
            return spark[v];
        } catch {
            v.Dump();
            throw;
        }
    }
    public static float Normal(this double v, double max, int r) {
        return (float)(v * r / max);
    }
    #endregion
}