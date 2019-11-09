using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Aura.ViewModels;
using PostProcessor.Blocks;
using PostProcessor.Blocks.Comment;
using PostProcessor.Blocks.Extrude;
using PostProcessor.Blocks.Move;
using PostProcessor.Blocks.Retract;
using PostProcessor.Blocks.Travel;

namespace Aura.Controls
{
    public class BlocksConverter : IValueConverter
    {
        private static readonly Brush _brushCommand = Brushes.Purple;
        private static readonly Brush _brushCoordinate = Brushes.CornflowerBlue;
        private static readonly Brush _brushCommon = Brushes.Gray;
        private static readonly Brush _brushExtrusion = Brushes.ForestGreen;
        private static readonly Brush _brushComment = Brushes.LightGray;

        private const int POS_0 = 0;
        private const int POS_1 = 5;
        private const int POS_2 = 15;
        private const int POS_3 = 25;
        private const int POS_4 = 35;
        private const int POS_5 = 45;
        private const int POS_6 = 55;

        private readonly int[] POSES = new int[7]{ POS_0 , POS_1, POS_2, POS_3, POS_4, POS_5, POS_6 };

        private string GetAdditionalSpaces(int count)
        {
            var str = String.Empty;
            for (int i = 0; i < count; i++)
            {
                str += " ";
            }
            return str;
        }

        private List<Run> GetRuns(ITravel travel)
        {
            var pos = 0;
            var textOutLength = 0;

            var runs = new List<Run>();

            var code = travel.GetCode();
            var regexG0 = new Regex(@"G0 ");
            var g0 = regexG0.Match(code).Value;
            textOutLength += g0.Length;
            var travelRun0 = new Run(g0) { Foreground = _brushCommand };
            runs.Add(travelRun0);

            var regexX = new Regex(@"X\d+[.]?\d*");
            var x = regexX.Matches(code);
            if (x.Count > 0)
            {
                pos++;
                var strX = GetAdditionalSpaces(POSES[pos] - textOutLength) + x[0].Value;
                var travelRun1 = new Run(strX) { Foreground = _brushCoordinate };
                runs.Add(travelRun1);
                textOutLength += strX.Length;
            }

            var regexY = new Regex(@"Y\d+[.]?\d*");
            var y = regexY.Matches(code);
            if (y.Count > 0)
            {
                pos++;
                var strY = GetAdditionalSpaces(POSES[pos] - textOutLength) + y[0].Value;
                var travelRun2 = new Run(strY) { Foreground = _brushCoordinate };
                runs.Add(travelRun2);
                textOutLength += strY.Length;
            }

            var regexZ = new Regex(@"Z\d+[.]?\d*");
            var z = regexZ.Matches(code);
            if (z.Count > 0)
            {
                pos++;
                var strZ = GetAdditionalSpaces(POSES[pos] - textOutLength) + z[0].Value;
                var travelRun3 = new Run(strZ) { Foreground = _brushCoordinate };
                runs.Add(travelRun3);
                textOutLength += strZ.Length;
            }

            var regexF = new Regex(@"F\d+[.]?\d*");
            var f = regexF.Matches(code);
            if (f.Count > 0)
            {
                pos++;
                var strF = GetAdditionalSpaces(POSES[pos] - textOutLength) + f[0].Value;
                var travelRun4 = new Run(strF) { Foreground = _brushCommon };
                runs.Add(travelRun4);
            }

            return runs;
        }

        private List<Run> GetRuns(IMoveP moveP)
        {
            var pos = 0;
            var textOutLength = 0;

            var runs = new List<Run>();

            var code = moveP.GetCode();
            var regexG0 = new Regex(@"G1 ");
            var g0 = regexG0.Match(code).Value;
            textOutLength += g0.Length;
            var travelRun0 = new Run(g0) { Foreground = _brushCommand };
            runs.Add(travelRun0);

            var regexX = new Regex(@"X\d+[.]?\d*");
            var x = regexX.Matches(code);
            if (x.Count > 0)
            {
                pos++;
                var strX = GetAdditionalSpaces(POSES[pos] - textOutLength) + x[0].Value;
                var travelRun1 = new Run(strX) { Foreground = _brushCoordinate };
                runs.Add(travelRun1);
                textOutLength += strX.Length;
            }

            var regexY = new Regex(@"Y\d+[.]?\d*");
            var y = regexY.Matches(code);
            if (y.Count > 0)
            {
                pos++;
                var strY = GetAdditionalSpaces(POSES[pos] - textOutLength) + y[0].Value;
                var travelRun2 = new Run(strY) { Foreground = _brushCoordinate };
                runs.Add(travelRun2);
                textOutLength += strY.Length;
            }

            var regexZ = new Regex(@"Z\d+[.]?\d*");
            var z = regexZ.Matches(code);
            if (z.Count > 0)
            {
                pos++;
                var strZ = GetAdditionalSpaces(POSES[pos] - textOutLength) + z[0].Value;
                var travelRun3 = new Run(strZ) { Foreground = _brushCoordinate };
                runs.Add(travelRun3);
                textOutLength += strZ.Length;
            }

            var regexE = new Regex(@"E\d+[.]?\d*");
            var e = regexE.Matches(code);
            if (e.Count > 0)
            {
                pos++;
                var strE = GetAdditionalSpaces(POSES[pos] - textOutLength) + e[0].Value;
                var travelRun4 = new Run(strE) { Foreground = _brushExtrusion };
                runs.Add(travelRun4);
                textOutLength += strE.Length;
            }

            var regexF = new Regex(@"F\d+[.]?\d*");
            var f = regexF.Matches(code);
            if (f.Count > 0)
            {
                pos++;
                var strF = GetAdditionalSpaces(POSES[pos] - textOutLength) + f[0].Value;
                var travelRun5 = new Run(strF) { Foreground = _brushCommon };
                runs.Add(travelRun5);
            }

            return runs;
        }

        private List<Run> GetRuns(IMovePF movePF)
        {
            var pos = 0;
            var textOutLength = 0;

            var runs = new List<Run>();

            var code = movePF.GetCode();
            var regexG0 = new Regex(@"G1 ");
            var g0 = regexG0.Match(code).Value;
            textOutLength += g0.Length;
            var travelRun0 = new Run(g0) { Foreground = _brushCommand };
            runs.Add(travelRun0);

            var regexX = new Regex(@"X\d+[.]?\d*");
            var x = regexX.Matches(code);
            if (x.Count > 0)
            {
                pos++;
                var strX = GetAdditionalSpaces(POSES[pos] - textOutLength) + x[0].Value;
                var travelRun1 = new Run(strX) { Foreground = _brushCoordinate };
                runs.Add(travelRun1);
                textOutLength += strX.Length;
            }

            var regexY = new Regex(@"Y\d+[.]?\d*");
            var y = regexY.Matches(code);
            if (y.Count > 0)
            {
                pos++;
                var strY = GetAdditionalSpaces(POSES[pos] - textOutLength) + y[0].Value;
                var travelRun2 = new Run(strY) { Foreground = _brushCoordinate };
                runs.Add(travelRun2);
                textOutLength += strY.Length;
            }

            var regexZ = new Regex(@"Z\d+[.]?\d*");
            var z = regexZ.Matches(code);
            if (z.Count > 0)
            {
                pos++;
                var strZ = GetAdditionalSpaces(POSES[pos] - textOutLength) + z[0].Value;
                var travelRun3 = new Run(strZ) { Foreground = _brushCoordinate };
                runs.Add(travelRun3);
                textOutLength += strZ.Length;
            }

            var regexV = new Regex(@"V\d+[.]?\d*");
            var v = regexV.Matches(code);
            if (v.Count > 0)
            {
                pos++;
                var strV = GetAdditionalSpaces(POSES[pos] - textOutLength) + v[0].Value;
                var travelRun4 = new Run(strV) { Foreground = _brushExtrusion };
                runs.Add(travelRun4);
                textOutLength += strV.Length;
            }

            var regexU = new Regex(@"U\d+[.]?\d*");
            var u = regexU.Matches(code);
            if (u.Count > 0)
            {
                pos++;
                var strU = GetAdditionalSpaces(POSES[pos] - textOutLength) + u[0].Value;
                var travelRun5 = new Run(strU) { Foreground = _brushExtrusion };
                runs.Add(travelRun5);
                textOutLength += strU.Length;
            }

            var regexF = new Regex(@"F\d+[.]?\d*");
            var f = regexF.Matches(code);
            if (f.Count > 0)
            {
                pos++;
                var strF = GetAdditionalSpaces(POSES[pos] - textOutLength) + f[0].Value;
                var travelRun6 = new Run(strF) { Foreground = _brushCommon };
                runs.Add(travelRun6);
            }

            return runs;
        }

        private List<Run> GetRuns(IRetract retract)
        {
            var pos = 0;
            var textOutLength = 0;

            var runs = new List<Run>();

            var code = retract.GetCode();
            var regexG0 = new Regex(@"G1 ");
            var g0 = regexG0.Match(code).Value;
            textOutLength += g0.Length;
            var travelRun0 = new Run(g0) { Foreground = _brushCommand };
            runs.Add(travelRun0);

            var regexF = new Regex(@"F\d+[.]?\d*");
            var f = regexF.Matches(code);
            if (f.Count > 0)
            {
                pos++;
                var strF = GetAdditionalSpaces(POSES[pos] - textOutLength) + f[0].Value;
                var travelRun1 = new Run(strF) { Foreground = _brushCommon };
                runs.Add(travelRun1);
                textOutLength += strF.Length;
            }

            var regexE = new Regex(@"E\d+[.]?\d*");
            var e = regexE.Matches(code);
            if (e.Count > 0)
            {
                pos++;
                var strE = GetAdditionalSpaces(POSES[pos] - textOutLength) + e[0].Value;
                var travelRun2 = new Run(strE) { Foreground = _brushExtrusion };
                runs.Add(travelRun2);
                textOutLength += strE.Length;
            }

            var regexV = new Regex(@"V\d+[.]?\d*");
            var v = regexV.Matches(code);
            if (v.Count > 0)
            {
                pos++;
                var strV = GetAdditionalSpaces(POSES[pos] - textOutLength) + v[0].Value;
                var travelRun3 = new Run(strV) { Foreground = _brushExtrusion };
                runs.Add(travelRun3);
                textOutLength += strV.Length;
            }

            return runs;
        }

        private List<Run> GetRuns(IExtrudeP extrude)
        {
            var pos = 0;
            var textOutLength = 0;

            var runs = new List<Run>();

            var code = extrude.GetCode();
            var regexG0 = new Regex(@"G1 ");
            var g0 = regexG0.Match(code).Value;
            textOutLength += g0.Length;
            var travelRun0 = new Run(g0) { Foreground = _brushCommand };
            runs.Add(travelRun0);

            var regexF = new Regex(@"F\d+[.]?\d*");
            var f = regexF.Matches(code);
            if (f.Count > 0)
            {
                pos++;
                var strF = GetAdditionalSpaces(POSES[pos] - textOutLength) + f[0].Value;
                var travelRun1 = new Run(strF) { Foreground = _brushCommon };
                runs.Add(travelRun1);
                textOutLength += strF.Length;
            }

            var regexE = new Regex(@"E\d+[.]?\d*");
            var e = regexE.Matches(code);
            if (e.Count > 0)
            {
                pos++;
                var strE = GetAdditionalSpaces(POSES[pos] - textOutLength) + e[0].Value;
                var travelRun2 = new Run(strE) { Foreground = _brushExtrusion };
                runs.Add(travelRun2);
                textOutLength += strE.Length;
            }

            return runs;
        }

        private List<Run> GetRuns(IExtrudeF extrude)
        {
            var pos = 0;
            var textOutLength = 0;

            var runs = new List<Run>();

            var code = extrude.GetCode();
            var regexG0 = new Regex(@"G1 ");
            var g0 = regexG0.Match(code).Value;
            textOutLength += g0.Length;
            var travelRun0 = new Run(g0) { Foreground = _brushCommand };
            runs.Add(travelRun0);

            var regexF = new Regex(@"F\d+[.]?\d*");
            var f = regexF.Matches(code);
            if (f.Count > 0)
            {
                pos++;
                var strF = GetAdditionalSpaces(POSES[pos] - textOutLength) + f[0].Value;
                var travelRun1 = new Run(strF) { Foreground = _brushCommon };
                runs.Add(travelRun1);
                textOutLength += strF.Length;
            }

            var regexU = new Regex(@"U\d+[.]?\d*");
            var u = regexU.Matches(code);
            if (u.Count > 0)
            {
                pos++;
                var strU = GetAdditionalSpaces(POSES[pos] - textOutLength) + u[0].Value;
                var travelRun2 = new Run(strU) { Foreground = _brushExtrusion };
                runs.Add(travelRun2);
                textOutLength += strU.Length;
            }

            return runs;
        }

        private List<Run> GetRuns(IExtrudePF extrude)
        {
            var pos = 0;
            var textOutLength = 0;

            var runs = new List<Run>();

            var code = extrude.GetCode();
            var regexG0 = new Regex(@"G1 ");
            var g0 = regexG0.Match(code).Value;
            textOutLength += g0.Length;
            var travelRun0 = new Run(g0) { Foreground = _brushCommand };
            runs.Add(travelRun0);

            var regexF = new Regex(@"F\d+[.]?\d*");
            var f = regexF.Matches(code);
            if (f.Count > 0)
            {
                pos++;
                var strF = GetAdditionalSpaces(POSES[pos] - textOutLength) + f[0].Value;
                var travelRun1 = new Run(strF) { Foreground = _brushCommon };
                runs.Add(travelRun1);
                textOutLength += strF.Length;
            }

            var regexV = new Regex(@"V\d+[.]?\d*");
            var v = regexV.Matches(code);
            if (v.Count > 0)
            {
                pos++;
                var strV = GetAdditionalSpaces(POSES[pos] - textOutLength) + v[0].Value;
                var travelRun2 = new Run(strV) { Foreground = _brushExtrusion };
                runs.Add(travelRun2);
                textOutLength += strV.Length;
            }

            var regexU = new Regex(@"E\d+[.]?\d*");
            var u = regexU.Matches(code);
            if (u.Count > 0)
            {
                pos++;
                var strU = GetAdditionalSpaces(POSES[pos] - textOutLength) + u[0].Value;
                var travelRun3 = new Run(strU) { Foreground = _brushExtrusion };
                runs.Add(travelRun3);
                textOutLength += strU.Length;
            }

            return runs;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var outString = "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"  xml:space=\"preserve\">";

            var runs = new List<Run>();

            var blockPart = (BlockCode) value;
            var block = blockPart.Block;

            if (block is ITravel travel) runs = GetRuns(travel);
            else
            {
                if (block is IMoveP moveP) runs = GetRuns(moveP);
                else
                {
                    if (block is IMovePF movePF) runs = GetRuns(movePF);
                    else
                    {
                        if (block is IRetract retract) runs = GetRuns(retract);
                        else
                        {
                            if (block is IExtrudeP extrudeP) runs = GetRuns(extrudeP);
                            else
                            {
                                if (block is IExtrudeF extrudeF) runs = GetRuns(extrudeF);
                                else
                                {
                                    if (block is IExtrudePF extrudePF) runs = GetRuns(extrudePF);
                                    else
                                    {
                                        if (block is IComment comment)
                                        {
                                            outString = "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"  xml:space=\"preserve\" TextWrapping=\"Wrap\">";
                                            var run = new Run(blockPart.PartCode) { Foreground = _brushComment };
                                            runs.Add(run);
                                        }
                                        else
                                        {
                                            var run = new Run(blockPart.PartCode) { Foreground = _brushCommon };
                                            runs.Add(run);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (runs.Any())
            {
                foreach (var run in runs)
                {
                    outString += System.Windows.Markup.XamlWriter.Save(run);
                }
            }
            outString += "</TextBlock>";
            return System.Windows.Markup.XamlReader.Parse(outString);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}