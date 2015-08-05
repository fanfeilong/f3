<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Web.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

void Main()
{
	MultiLine(new double[]{1,2,3,4},new double[]{2,3,4,5},4,2).Dump();
}

// Define other methods and classes here

//二元多次线性方程拟合曲线
public static double[] MultiLine(double[] arrX, double[] arrY, int length, int dimension){
   int n = dimension + 1;                  //dimension次方程需要求 dimension+1个 系数
   double[,] Guass=new double[n,n+1];      //高斯矩阵 例如：y=a0+a1*x+a2*x*x
   for(int i=0;i<n;i++){
       int j;
       for(j=0;j<n;j++){
           Guass[i,j] = SumArr(arrX, j + i, length);
       }
       Guass[i,j] = SumArr(arrX,i,arrY,1,length);          
   }
  return ComputGauss(Guass,n);
}

//求数组的元素的n次方的和
public static double SumArr(double[] arr, int n, int length) {
   double s = 0;
   for (int i = 0; i < length; i++){
       if (arr[i] != 0 || n != 0)         
           s = s + Math.Pow(arr[i], n);
       else
           s = s + 1;
   }
   return s;
}


public static double SumArr(double[] arr1, int n1, double[] arr2, int n2, int length){
   double s=0;
   for (int i = 0; i < length; i++){
       if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
           s = s + Math.Pow(arr1[i], n1) * Math.Pow(arr2[i], n2);
       else
           s = s + 1;
   }
   return s;

}

//返回值是函数的系数
public static double[] ComputGauss(double[,] Guass,int n){
   int i, j;
   int k,m;
   double temp;
   double max;
   double s;
   double[] x = new double[n];
   for (i = 0; i < n; i++)           x[i] = 0.0;//初始化
  
   for (j = 0; j < n; j++){
       max = 0;         
       k = j;    
       for (i = j; i < n; i++){
           if (Math.Abs(Guass[i, j]) > max){
               max = Guass[i, j];
               k = i;
           }
       }

      
       if (k != j){
           for (m = j; m < n + 1; m++){
               temp = Guass[j, m];
               Guass[j, m] = Guass[k, m];
               Guass[k, m] = temp;
           }
       }
       if (0 == max){
           // "此线性方程为奇异线性方程" 
           return x;
       }
      
       for (i = j + 1; i < n; i++) {
           s = Guass[i, j];
           for (m = j; m < n + 1; m++)
           {
               Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);
           }
       }

   }//结束for (j=0;j<n;j++)
  
   for (i = n-1; i >= 0; i--){           
       s = 0;
       for (j = i + 1; j < n; j++){
           s = s + Guass[i,j] * x[j];
       }
       x[i] = (Guass[i,n] - s) / Guass[i,i];
   }
  return x;
}
