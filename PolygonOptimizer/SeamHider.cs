using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MSClipperLib;
using Util.GeometryBasics;
using Polygon = System.Collections.Generic.List<MSClipperLib.IntPoint>;
using SegmentPolygon = System.Collections.Generic.List<Util.GeometryBasics.Segment>;

namespace Util.PolygonOptimizer
{

    #region BEST SEGMENT

    /// <summary>
    /// Класс-хранитель наилучшего сегмента в полигоне
    /// </summary>
    public sealed class BestSegment
    {
        /// <summary>
        /// Наилучший сегмент
        /// </summary>
        public Segment Segment { get; set; }
        /// <summary>
        /// Индекс наилучшего сегмента
        /// </summary>
        public int SegmentIndex { get; set; }
        /// <summary>
        /// Наилучшая точка в наилучшем сегменте
        /// </summary>
        public IntPoint BestPoint { get; set; }
    }

    #endregion

    /// <summary>
    /// Класс, содержащий методы поиска шва
    /// </summary>
    public static class SeamHider
    {
        private const double THRESHOLD_ANGLE = 10.0;

        #region GET BEST SEGMENT

        private static BestSegment GetBestSegment(SegmentPolygon polygon, IntPoint bestPoint)
        {
            var bestSegment = new BestSegment
            {
                BestPoint = bestPoint
            };

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].StartPoint.Equals(bestPoint))
                {
                    bestSegment.Segment = polygon[i];
                    bestSegment.SegmentIndex = i;
                    break;
                }
            }

            return bestSegment;
        }

        #endregion

        #region GET BEST

        /// <summary>
        /// Возвращает наилучший сегмент в полигоне, относительно точки point
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static BestSegment GetBest(SegmentPolygon polygon, IntPoint point)
        {
            if (polygon == null || !polygon.Any()) return null;

            if (polygon.Count == 1) return new BestSegment
            {
                BestPoint = polygon.First().StartPoint,
                Segment = polygon.First(),
                SegmentIndex = 0
            };

            var polyPoint = polygon.SegmentsToPoints();

            //собираем лист углов и точек для этих углов
            var lstAngles = new SortedDictionary<double, List<IntPoint>>(Comparer<double>.Default);
            for (var i = 0; i < polyPoint.Count; i++)
            {
                var prevPoint = polyPoint[(i + polyPoint.Count - 1) % polyPoint.Count];
                var currentPoint = polyPoint[i];
                var nextPoint = polyPoint[(i + 1) % polyPoint.Count];

                //находим угол обхода от предыдущей точки к следующей
                var turnAmount = IntPointHelper.GetTurnAmount(prevPoint, currentPoint, nextPoint);
                //если в списке уже есть такой угол, то добавляем такую точку
                if (lstAngles.ContainsKey(turnAmount)) lstAngles[turnAmount].Add(currentPoint);
                else
                {
                    var found = false;
                    //если нет, то проходим по всем углами находим есть ли близкий угол (где разница в любую сторону меньше 10 градусов)
                    foreach (var angle in lstAngles)
                    {
                        if (Math.Abs(angle.Key - turnAmount) < Math.PI * THRESHOLD_ANGLE / 180)
                        {
                            //если есть близкий угол, то расширяем найденный итем
                            lstAngles[angle.Key].Add(currentPoint);
                            found = true;
                            break;
                        }
                    }
                    //если близкого угла нет, то добавляем в словарь новый итем
                    if (!found) lstAngles.Add(turnAmount, new Polygon { currentPoint });
                }
            }

            //если самый первый (самый маленький угол меньше -30 градусов)
            if (lstAngles.FirstOrDefault().Key < - Math.PI * 30 / 180)
            {
                //находим ближайшую по расстоянию точку в самых впуклых углаз
                var closestInFirst = lstAngles.FirstOrDefault().Value.First();
                var closestDist = double.MaxValue;
                foreach (var bestAnglePoint in lstAngles.FirstOrDefault().Value)
                {
                    var length = (bestAnglePoint - point).LengthSquared();
                    if (length < closestDist)
                    {
                        closestDist = length;
                        closestInFirst = bestAnglePoint;
                    }
                }
                return GetBestSegment(polygon, closestInFirst);
            }
            else //если нет достаточно впуклых углов - то просто возвращаем самую правую точку
            {
                return GetMostRight(polygon);
            }
        }

        #endregion

        #region GET CLOSEST

        public static (IntPoint point, long dist) GetClosest(IntPoint p0, IntPoint p1, IntPoint lastBestPoint)
        {
            var distToStart = PolygonHelper.LengthSquaredWithoutZ(p0, lastBestPoint);
            var distToEnd = PolygonHelper.LengthSquaredWithoutZ(p1, lastBestPoint);

            return distToStart < distToEnd ? (p0, distToStart) : (p1, distToEnd);
        }

        public static BestSegment GetClosest(SegmentPolygon polygon, IntPoint lastBestPoint)
        {
            if (polygon == null || polygon.Count == 0) return null;

            var bestPoint = new IntPoint();
            var closestDist = double.MaxValue;
            //для каждой точки в полигоне
            for (var i = 0; i < polygon.Count; i++)
            {
                //длина в квадрате между переданной точкой и текущей точкой в полигоне 
                var dist = PolygonHelper.LengthSquaredWithoutZ(polygon[i].StartPoint, lastBestPoint);
                //если найденное расстояние меньше последнего запомненного
                if (dist < closestDist)
                {
                    //запоминаем индекс, изменяем наименьшее расстояние
                    bestPoint = polygon[i].StartPoint;
                    closestDist = dist;
                }
            }

            return GetBestSegment(polygon, bestPoint);           
        }

        #endregion

        #region SPLIT BY BEST

        /// <summary>
        /// Возвращает наилучшую точку, при этом разбивая исходный полигон при необходмости (если в нем самом такой точки нет)
        /// </summary>
        /// <param name="polygon">Исходный полигон</param>
        /// <param name="bestPoint">Наилучшая точка в полигоне</param>
        /// <param name="bestSegment">Наилучший сегмент в полигоне</param>
        /// <returns></returns>
        public static BestSegment SplitByBest(ref SegmentPolygon polygon, IntPoint bestPoint, Segment bestSegment)
        {
            if (polygon == null || !polygon.Any() || bestSegment == null) return null;

            //иначе - находим индекс сегмента, где располагается наилучшая точка-проекция
            var indexOfSegment = polygon.IndexOf(bestSegment);
            //разбиваем этот сегмент на два точкой-проекцией
            var (fromStartToPoint, fromPointToEnd) = bestSegment.Split(bestPoint);
            //удаляем старый сегмент
            polygon.Remove(bestSegment);
            //вставляем на его место новые
            var lstSegments = new List<Segment>();
            if (fromStartToPoint.LengthUM != 0) lstSegments.Add(fromStartToPoint);
            if (fromPointToEnd.LengthUM != 0) lstSegments.Add(fromPointToEnd);
            polygon.InsertRange(indexOfSegment, lstSegments);
            //возвращаем наилучший сегмент
            var offset = fromStartToPoint.LengthUM == 0 ? 0 : 1;
            return new BestSegment
            {
                //т.к. наилучший сегмент указывает на сегмент, где наилучшая точка - начальная, то берем fromPointToEnd
                Segment = lstSegments.Last(),
                //к индексу сегмента добавляем единицу
                SegmentIndex = (indexOfSegment + offset) % polygon.Count,
                BestPoint = bestPoint
            };
        }

        #endregion

        #region GET MOST LEFT

        public static BestSegment GetMostLeft(SegmentPolygon polygon)
        {
            if (polygon == null || !polygon.Any()) return null;

            var mostLeftPoint = new IntPoint(int.MaxValue, int.MinValue);
            var segmentIndex = 0;

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].StartPoint.X <= mostLeftPoint.X)
                {
                    if (polygon[i].StartPoint.X < mostLeftPoint.X || polygon[i].StartPoint.Y < mostLeftPoint.Y)
                    {
                        mostLeftPoint = polygon[i].StartPoint;
                        segmentIndex = i;
                    }
                }
            }

            var newSegment = new BestSegment
            {
                Segment = polygon[segmentIndex],
                SegmentIndex = segmentIndex
            };
            newSegment.BestPoint = newSegment.Segment.StartPoint;
            return newSegment;
        }

        public static IntPoint GetMostLeft(Polygon polygon)
        {
            if (polygon == null || !polygon.Any()) return default;

            var mostLeft = new IntPoint(int.MaxValue, int.MinValue);

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].X <= mostLeft.X)
                {
                    if (polygon[i].X < mostLeft.X || polygon[i].Y < mostLeft.Y)
                    {
                        mostLeft = polygon[i];
                    }
                }
            }

            return mostLeft;
        }
        

        #endregion

        #region GET MOST TOP

        public static BestSegment GetMostTop(SegmentPolygon polygon)
        {
            if (polygon == null || !polygon.Any()) return null;

            var mostTopPoint = new IntPoint(int.MinValue, int.MinValue);
            var segmentIndex = 0;

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].StartPoint.Y >= mostTopPoint.Y)
                {
                    if (polygon[i].StartPoint.Y > mostTopPoint.Y || polygon[i].StartPoint.X > mostTopPoint.X)
                    {
                        mostTopPoint = polygon[i].StartPoint;
                        segmentIndex = i;
                    }
                }
            }

            var newSegment = new BestSegment
            {
                Segment = polygon[segmentIndex],
                SegmentIndex = segmentIndex
            };
            newSegment.BestPoint = newSegment.Segment.StartPoint;
            return newSegment;
        }

        public static IntPoint GetMostTop(Polygon polygon)
        {
            if (polygon == null || !polygon.Any()) return default(IntPoint);

            var mostTop = new IntPoint(int.MinValue, int.MinValue);

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y >= mostTop.Y)
                {
                    if (polygon[i].Y > mostTop.Y || polygon[i].X > mostTop.X)
                    {
                        mostTop = polygon[i];
                    }
                }
            }

            return mostTop;
        }

        #endregion

        #region GET MOST RIGHT

        public static BestSegment GetMostRight(SegmentPolygon polygon)
        {
            if (polygon == null || !polygon.Any()) return null;

            var mostRightPoint = new IntPoint(int.MinValue, int.MaxValue);
            var segmentIndex = 0;

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].StartPoint.X >= mostRightPoint.X)
                {
                    if (polygon[i].StartPoint.X > mostRightPoint.X || polygon[i].StartPoint.Y < mostRightPoint.Y)
                    {
                        mostRightPoint = polygon[i].StartPoint;
                        segmentIndex = i;
                    }
                }
            }

            var newSegment = new BestSegment
            {
                Segment = polygon[segmentIndex],
                SegmentIndex = segmentIndex
            };
            newSegment.BestPoint = newSegment.Segment.StartPoint;
            return newSegment;
        }

        public static IntPoint GetMostRight(Polygon polygon)
        {
            if (polygon == null || !polygon.Any()) return default(IntPoint);

            var mostRight = new IntPoint(int.MinValue, int.MaxValue);

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].X >= mostRight.X)
                {
                    if (polygon[i].X > mostRight.X || polygon[i].Y < mostRight.Y)
                    {
                        mostRight = polygon[i];
                    }
                }
            }

            return mostRight;
        }

        #endregion

        #region GET MOST BOTTOM

        public static BestSegment GetMostBottom(SegmentPolygon polygon)
        {
            if (polygon == null || !polygon.Any()) return null;

            var mostBottomPoint = new IntPoint(int.MaxValue, int.MaxValue);
            var segmentIndex = 0;

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].StartPoint.Y <= mostBottomPoint.Y)
                {
                    if (polygon[i].StartPoint.Y < mostBottomPoint.Y || polygon[i].StartPoint.X < mostBottomPoint.X)
                    {
                        mostBottomPoint = polygon[i].StartPoint;
                        segmentIndex = i;
                    }
                }
            }

            var newSegment = new BestSegment
            {
                Segment = polygon[segmentIndex],
                SegmentIndex = segmentIndex
            };
            newSegment.BestPoint = newSegment.Segment.StartPoint;
            return newSegment;
        }

        public static IntPoint GetMostBottom(Polygon polygon)
        {
            if (polygon == null || !polygon.Any()) return default;

            var mostBottom = new IntPoint(int.MaxValue, int.MaxValue);

            for (var i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y <= mostBottom.Y)
                {
                    if (polygon[i].Y < mostBottom.Y || polygon[i].X < mostBottom.X)
                    {
                        mostBottom = polygon[i];
                    }
                }
            }

            return mostBottom;
        }

        #endregion

        #region GET BEST SEGMENT

        public static BestSegment GetBestSegment(SegmentPolygon polygon, int layerIndex)
        {
            var remainder = layerIndex % 4;
            switch (remainder)
            {
                case 0:
                    return GetMostLeft(polygon);
                case 1:
                    return GetMostTop(polygon);
                case 2:
                    return GetMostRight(polygon);
                case 3:
                    return GetMostBottom(polygon);
            }
            return null;
        }

        #endregion

        #region GET BEST POINT

        public static IntPoint GetBestPoint(Polygon polygon, int layerIndex)
        {
            var remainder = layerIndex % 4;
            switch (remainder)
            {
                case 0:
                    return GetMostLeft(polygon);
                case 1:
                    return GetMostTop(polygon);
                case 2:
                    return GetMostRight(polygon);
                case 3:
                    return GetMostBottom(polygon);
            }
            return default;
        }

        [ExcludeFromCodeCoverage]
        public static IntPoint GetBestPoint(List<Polygon> polygons, int layerIndex)
        {
            if (polygons == null || !polygons.Any()) return default;

            var hugeMess = polygons.MakeAHugeMess();
            return GetBestPoint(hugeMess, layerIndex);
        }

        #endregion
    }
}