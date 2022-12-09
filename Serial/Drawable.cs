using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Maui.Graphics.Converters;

namespace GraphingClass
{
    public class LineDrawable: BaseGraphData, IDrawable
    {
        private const int numberOfGraphs = 3;
        private string[] colorName = new string[numberOfGraphs] { "Red", "Blue", "DarkGreen" };
        private int[] lineWidth = new int[numberOfGraphs] {1, 2, 3};
        public BaseGraphData[] lineGraphs = new BaseGraphData[numberOfGraphs];

      

        //default contructor
        public LineDrawable() : base()
        {


            lineGraphs[0] = new BaseGraphData
              (
          Yaxis: 0,
          Xaxis: 0,
          lineColor: Colors.White,
          lineSize: lineWidth[1],
          newGraph: true
          );




            for (int i = 1; i < numberOfGraphs; i++)
            {
                
                lineGraphs[i] = new BaseGraphData
                    (
                Yaxis : 0,
                Xaxis : 0,
                lineColor: Colors.Yellow,
                lineSize: lineWidth[i],
                newGraph: true
                );

            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            for(int graphIndex = 0; graphIndex<lineGraphs.Length; graphIndex++)
            {
                Rect lineGraphsRect = new(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
                DrawLineGraph(canvas, lineGraphsRect, lineGraphs[graphIndex]);
            }
        }

        private void DrawLineGraph(ICanvas canvas, Rect lineGraphRect, BaseGraphData lineGraph) 
        {
                if(lineGraph.Xaxis < 2)
            {
                lineGraph.pointArray[lineGraph.Xaxis] = lineGraph.Yaxis;
                lineGraph.Xaxis++;
                return;
            }
                else if(lineGraph.Xaxis < 1000)
            {
                lineGraph.pointArray[lineGraph.Xaxis] = lineGraph.Yaxis;
                lineGraph.Xaxis++;
            }
            else
            {
                lineGraph.pointArray[999] = lineGraph.Yaxis;
                for(int i = 0; i < 999;i++)
                {
                   lineGraph.pointArray[i] = lineGraph.pointArray[i + 1];
                }
     
            }
            for (int i = 0; i < lineGraph.Xaxis - 1; i++)
            {
                canvas.StrokeColor = lineGraph.lineColor;
                canvas.StrokeSize = lineGraph.lineSize;
                canvas.DrawLine(i, lineGraph.pointArray[i], i + 1, lineGraph.pointArray[i + 1]); 
            }
        }

    }
}
