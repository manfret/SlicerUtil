using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MSClipperLib;
using Util.GeometryBasics;
using Util.TreeBuilder;
using SegmentPolygon = System.Collections.Generic.List<Util.GeometryBasics.Segment>;
using Polygon = System.Collections.Generic.List<MSClipperLib.IntPoint>;
using Island = System.Collections.Generic.List<System.Collections.Generic.List<MSClipperLib.IntPoint>>;

namespace Util.PolygonOptimizer
{
    /// <summary>
    /// Статический класс, в котором собраны методы для оптимизации полигонов как по наилучшей точки, так и по порядку между самими полигонами
    /// </summary>
    public static class PolygonOptimizer
    {
        #region GET BEST SEGMENT WAYS

        /// <summary>
        /// Делегат функций, которые возвращают наилучший сегмент по листу сегменту и дополнительной точке
        /// </summary>
        /// <param name="polygon">Полигон, в котором должна быть найдена наилучшая точка</param>
        /// <param name="point">Дополнительный параметр - точка</param>
        public delegate BestSegment GetBestSegmentWay(SegmentPolygon polygon, IntPoint point);

        /// <summary>
        /// Лямбда возвращения наилучшего сегмента с учетом сокрытия шва
        /// </summary>
        public static readonly GetBestSegmentWay GetPointBySeamHiding = (polygon, point) => SeamHider.GetBest(polygon, point);
        /// <summary>
        /// Лямбда возвращения наилучшего сегмента по кратчайшему расстоянию до точки point
        /// </summary>
        public static readonly GetBestSegmentWay GetPointByClosestPoint = (polygon, point) => SeamHider.GetClosest(polygon, point);


        #endregion

        #region OPTIMIZE

        /// <summary>
        /// Возвращает лист оптимизированных полигонов
        /// </summary>
        /// <param name="polygons">Лист полигонов, которые нужно оптимизирвоать</param>
        /// <param name="bestSegmentWay">Способ оптимизации порядка вершин в полигоне</param>
        /// <param name="lastPoint">Последняя точка, ан которой была окончена предыдущая операция</param>
        /// <param name="closeDistance"></param>
        /// <returns></returns>
        public static List<SegmentPolygon> Optimize(List<SegmentPolygon> polygons, GetBestSegmentWay bestSegmentWay, ref IntPoint lastPoint, int closeDistance)
        {
            //если полигон пустой, то возвращаем пустой полигон
            if (polygons == null || !polygons.Any()) return new List<SegmentPolygon>();

            //создает копию последней точки
            var lastPointCopy = new IntPoint(lastPoint);

            //слонируем все полигоны (чтобы не испортить)
            var polygonsClone = polygons.Clone();
            var resPoly = new List<SegmentPolygon>();

            //пока в клонах еще есть элементы
            while (polygonsClone.Count > 0)
            {
                //Находим лучший полигон и пересортировываем его
                var (polygon, bestSegment) = GetBestDistPolygonAndSegment(bestSegmentWay, closeDistance, polygonsClone, lastPointCopy);
                var reorderedPolygon = ReorderSegmentPolygon(polygon, bestSegment);

                //добавляем в результирующий лист
                resPoly.Add(reorderedPolygon);

                //удаляем из листа клонов отработанный полигон
                polygonsClone.Remove(polygon);
                //новая последняя точка - последняя в только что добавленном полигоне
                lastPointCopy = reorderedPolygon.Last().EndPoint;
            }

            lastPoint = lastPointCopy;

            return resPoly;
        }

        private static (SegmentPolygon polygon, BestSegment bestSegment) GetBestDistPolygonAndSegment(GetBestSegmentWay bestSegmentWay, int closeDistance, List<SegmentPolygon> polygons, IntPoint lastPoint)
        {
            SegmentPolygon polygon = null;
            BestSegment bestSegment = null;
            var dist = long.MaxValue;
            foreach (var polygonClone in polygons)
            {
                //если в полигона один сегмент, то способ нахождения наилучшей точки - однозначно по самой близкой
                if (polygonClone.Count == 1)
                {
                    var (bestPoint, distToBest) = SeamHider.GetClosest(polygonClone[0].StartPoint, polygonClone[0].EndPoint, lastPoint);
                    //находим теперь полигон, чей наилучший сегмент ближе всего
                    if (distToBest >= dist) continue;
                    dist = distToBest;
                    if (bestPoint.Equals(polygonClone[0].StartPoint))
                    {
                        polygon = polygonClone;
                        bestSegment = new BestSegment
                        {
                            BestPoint = bestPoint,
                            Segment = polygonClone[0].Clone() as Segment,
                            SegmentIndex = 0
                        };
                    }
                    else
                    {
                        polygon = polygonClone;
                        bestSegment = new BestSegment
                        {
                            BestPoint = bestPoint,
                            Segment = polygonClone[0].Flip(),
                            SegmentIndex = 0
                        };
                    }
                }
                else
                {
                    //если замкнутый полигон
                    BestSegment bestSegment1 = null;
                    if (polygonClone.First().StartPoint.Equals(polygonClone.Last().EndPoint))
                    {
                        bestSegment1 = bestSegmentWay(polygonClone, lastPoint);
                    }
                    else
                    {
                        var toStart = (polygonClone.First().StartPoint - lastPoint).Length();
                        var toEnd = (polygonClone.Last().EndPoint - lastPoint).Length();

                        if (toStart <= toEnd)
                        {
                            bestSegment1 = new BestSegment
                            {
                                BestPoint = polygonClone.First().StartPoint,
                                Segment = polygonClone.First(),
                                SegmentIndex = 0
                            };
                        }
                        else
                        {
                            bestSegment1 = new BestSegment
                            {
                                BestPoint = polygonClone.Last().EndPoint,
                                Segment = polygonClone.Last(),
                                SegmentIndex = polygonClone.Count - 1
                            };
                        }
                    }

                    var lastDist = (lastPoint - bestSegment1.BestPoint).LengthSquared();
                    //находим теперь полигон, чей наилучший сегмент ближе всего
                    if (lastDist >= dist) continue;
                    dist = lastDist;
                    polygon = polygonClone;
                    bestSegment = bestSegment1;
                }

                if (dist <= closeDistance)
                {
                    break;
                }
            }

            return (polygon, bestSegment);
        }

        public static List<SegmentPolygon> OptimizeByStartPoint(List<SegmentPolygon> polygons, ref IntPoint lastPoint)
        {
            //если полигон пустой, то возвращаем пустой полигон
            if (polygons == null || !polygons.Any()) return new List<SegmentPolygon>();

            //создает копию последней точки
            var lastPointCopy = new IntPoint(lastPoint);

            //клонируем все полигоны (чтобы не испортить)
            var polygonsClone = polygons.Clone();
            var resPolys = new List<SegmentPolygon>();

            //пока в клонах еще есть элементы
            while (polygonsClone.Count > 0)
            {
                var bestDist = long.MaxValue;
                SegmentPolygon bestPoly = null;

                foreach (var polygonClone in polygonsClone)
                {
                    var distToFirst = (polygonClone.First().StartPoint - lastPointCopy).LengthSquared();
                    if (distToFirst < bestDist)
                    {
                        bestDist = distToFirst;
                        bestPoly = polygonClone;
                    }
                }

                resPolys.Add(bestPoly);
                lastPointCopy = bestPoly.Last().EndPoint;

                polygonsClone.Remove(bestPoly);
            }

            lastPoint = lastPointCopy;

            return resPolys;
        }

        public static List<(SegmentPolygon polygon, SegmentPolygon tail)> OptimizeByStartPoint(List<(SegmentPolygon polygon, SegmentPolygon tail)> polygons, ref IntPoint lastPoint)
        {
            //если полигон пустой, то возвращаем пустой полигон
            if (polygons == null || !polygons.Any()) return new List<(SegmentPolygon polygon, SegmentPolygon tail)>();

            var optimized = OptimizeByStartPoint(polygons.Select(a => a.polygon).ToList(), ref lastPoint);

            var res = new List<(SegmentPolygon polygon, SegmentPolygon tail)>();
            foreach (var optimizedPolygon in optimized)
            {
                var item = polygons.SingleOrDefault(a => a.polygon.IsTheSamePoly(optimizedPolygon));
                res.Add(item);
            }
            return res;
        }

       public static List<SegmentPolygon> OptimizeWithoutReorder(List<SegmentPolygon> polygons, GetBestSegmentWay bestSegmentWay, ref IntPoint lastPoint)
        {
            //если полигон пустой, то возвращаем пустой полигон
            if (polygons == null || !polygons.Any()) return new List<SegmentPolygon>();

            //создает копию последней точки
            var lastPointCopy = new IntPoint(lastPoint);

            var resPolys = new List<SegmentPolygon>();

            foreach (var polygon in polygons)
            {
                var bestSegment = bestSegmentWay(polygon, lastPointCopy);
                lastPointCopy = bestSegment.BestPoint;
                resPolys.Add(polygon.ReorderList(bestSegment.SegmentIndex));
            }

            lastPoint = lastPointCopy;

            return resPolys;
        }

        public static List<SegmentPolygon> OptimizeWithNesting(List<List<SegmentPolygon>> nestedPolygons,
            ref IntPoint lastPoint,
            int EWUM)
        {
            //если список полигон пустой, то возвращаем пустой список
            if (!nestedPolygons.TrueAny() || nestedPolygons.Count(a => a.TrueAny()) == 0)
                return new List<SegmentPolygon>();

            //создает копию последней точки
            var lastPointCopy = new IntPoint(lastPoint);

            //слонируем все полигоны (чтобы не испортить)
            var nestedPolygonsClone = new List<List<SegmentPolygon>>();
            foreach (var level in nestedPolygons)
            {
                nestedPolygonsClone.Add(level.Clone());
            }

            //Результирующий список полигонов    
            var resPolys = new List<SegmentPolygon>();

            //лист для накопления текущего пути
            var currentPath = new List<SegmentPolygon>();
            var currentLevel = 0;
            bool currentLevelSearch = false;

            var nextLevelThreshold = (int)(EWUM * 3);
            var nextLevelFastSearchDistance = (int)(EWUM * 1.5);
            var curLevelThreshold = (int)(EWUM * 1);
            var curLevelFastSearchDistance = (int)(EWUM * 1);

            //пока в клонах еще есть элементы
            while (nestedPolygonsClone.Count > 0)
            {
                //если текущий путь пустой - создаем новый по лучшей точке из полигонов с верхнего уровня
                if (!currentPath.Any())
                {
                    currentLevel = 0;
                    //находим лучший полигон и его лучший сегмент (по скрытию шва)
                    var (polygon, bestSegment) = GetBestDistPolygonAndSegment(GetPointBySeamHiding, 0, nestedPolygonsClone.First(), lastPointCopy);
                    var reorderedPolygon = ReorderSegmentPolygon(polygon, bestSegment);

                    //добавляем в текущий путь
                    currentPath.Add(reorderedPolygon);
                    //удаляем из листа клонов отработанный полигон
                    nestedPolygonsClone.First().Remove(polygon);
                    //новая последняя точка - последняя в только что добавленном полигоне
                    lastPointCopy = reorderedPolygon.Last().EndPoint;
                    currentLevelSearch = true;
                }
                else
                {
                    if (currentLevel < nestedPolygonsClone.Count)
                    {
                        var thresholdDistance = currentLevelSearch ? curLevelThreshold : nextLevelThreshold;
                        var fastSearchDistance = currentLevelSearch ? curLevelFastSearchDistance : nextLevelFastSearchDistance;
                        //находим лучший полигон и его лучший сегмент (по расстоянию)
                        var (polygon, bestSegment) = GetBestDistPolygonAndSegment(GetPointByClosestPoint, fastSearchDistance, nestedPolygonsClone[currentLevel], lastPointCopy);
                        //если расстояние меньше nestingDistance, то добавляем в путь
                        if (polygon!=null && new Segment(lastPointCopy, bestSegment.BestPoint).LengthUM <= thresholdDistance)
                        {
                            var reorderedPolygon = ReorderSegmentPolygon(polygon, bestSegment);
                            //добавляем в текущий путь
                            currentPath.Add(reorderedPolygon);
                            //удаляем из листа клонов отработанный полигон
                            nestedPolygonsClone[currentLevel].Remove(polygon);
                            //новая последняя точка - последняя в только что добавленном полигоне
                            lastPointCopy = reorderedPolygon.Last().EndPoint;
                            currentLevelSearch = true;
                            continue;
                        }

                        if (currentLevelSearch)
                        {
                            currentLevelSearch = false;
                            currentLevel++;
                            continue;
                        }
                    }

                    //создаем новый путь
                    //var processedPath = inverseNesting ? currentPath.GetFlippedIfOpen() : currentPath;
                    resPolys.AddRange(currentPath);
                    currentPath = new List<SegmentPolygon>();
                    //Очищаем пустые уровни
                    for (int i = nestedPolygonsClone.Count-1; i >=0; i--)
                    {
                        if (!nestedPolygonsClone[i].Any()) nestedPolygonsClone.RemoveAt(i);
                    }
                }
            }

            if (currentPath.Any())
            {
                //var processedPath = inverseNesting ? currentPath.GetFlippedIfOpen() : currentPath;
                resPolys.AddRange(currentPath);
            }

            lastPoint = lastPointCopy;
            return resPolys;
        }

        #endregion

        #region REORDER LIST

        #region NEW ORDER

        /// <summary>
        /// Возвращаем лист индексов для пересортировки листа
        /// </summary>
        /// <param name="lstCount">Общее колиечство элементов</param>
        /// <param name="newStartIndex">Новый стартовый индекс</param>
        /// <returns>Лист с индексами, в обычном порядке, где каждый элемент - новый индекс</returns>
        public static List<int> NewOrder(int lstCount, int newStartIndex)
        {
            if (lstCount <= 0) return null;

            var lst = new List<int>(lstCount);

            for (var i = 0; i < lstCount; i++)
            {
                //вычисляем индекс нового элемента
                var newItem = (i - newStartIndex + lstCount) % lstCount;
                lst.Add(newItem);
            }
            return lst;
        }

        #endregion

        /// <summary>
        /// Пересортровывает лист, позвращая новый
        /// </summary>
        /// <param name="source">Лист, который необходимо пересортировать</param>
        /// <param name="newStartIndex">Индекс элемента, котоорый будет первый</param>
        /// <typeparam name="T">Тип</typeparam>
        /// <returns>Возвращает отсортированный лист, где старый индекс newStartIndex  теперь нулевой</returns>
        public static List<T> ReorderList<T>(this List<T> source, int newStartIndex)
        {
            //если лист пустой или newStartIndex указан неверно, то возвращает null
            if (source == null || !source.Any() || newStartIndex >= source.Count || newStartIndex<0) return null;

            //находим новый порядок
            var newOrderIndexes = NewOrder(source.Count, newStartIndex);
            //заготовка массива
            var newOrderArray = new T[source.Count];
            //для всех элементов исходного листа
            for (var i = 0; i < source.Count; i++)
            {
                //помещает каждый элемент в соответсвующую ячеику массива, номер ячеики берем из newOrderIndexes
                var newIndex = newOrderIndexes[i];
                newOrderArray[newIndex] = source[i];
            }
            return newOrderArray.ToList();
        }

        private static SegmentPolygon ReorderSegmentPolygon(SegmentPolygon polygon, BestSegment bestSegment)
        {
            //если в полигоне один сегмент, то возвращаем этот сегмент, обернув его в полигон
            //иначе - пересортировываем лист, так чтобы наилучший сегмент был первым
            SegmentPolygon res;
            if (polygon.Count == 1)
            {
                res = new SegmentPolygon { bestSegment.Segment };
            }
            else
            {
                if (polygon.First().StartPoint.Equals(polygon.Last().EndPoint))
                {
                    res = polygon.ReorderList(bestSegment.SegmentIndex);
                }
                else
                {
                    if (bestSegment.SegmentIndex == 0) res = polygon;
                    else
                    {
                        var newSegmentsReversed = new List<Segment>();
                        foreach (var segment in polygon)
                        {
                            newSegmentsReversed.Add(segment.Flip());
                        }

                        newSegmentsReversed.Reverse();
                        res = newSegmentsReversed;
                    }
                }
            }

            return res;
        }



        #endregion

        #region IS THE SAME POLY

        /// <summary>
        /// Возвращает true, если сегменты полигонов идентичны по точкам
        /// </summary>
        /// <param name="source">Первый полигон</param>
        /// <param name="check">Второй полигон</param>
        /// <returns></returns>
        public static bool IsTheSamePoly(this SegmentPolygon source, SegmentPolygon check)
        {
            if (source == null || !source.Any() && (check == null || !check.Any())) return false;
            //если количество сегментов - разное, и не отличается на 1 (один замкнутый, другой нет), то можно сразу вернуть false
            var sourceCount = source.Count;
            var checkCount = check.Count;
            if (sourceCount != checkCount && Math.Abs(sourceCount - checkCount)!=1) return false;

            //преобразуем полигон в точки
            var sourcePoints = source.SegmentsToPoints();
            var checkPoints = check.SegmentsToPoints();
            return sourcePoints.IsTheSamePoly(checkPoints);
        }

        public static bool IsTheSamePoly(this Polygon source, Polygon check)
        {
            if (source == null || !source.Any() && (check == null || !check.Any())) return false;
            var sourceCount = source.Count;
            var checkCount = check.Count;
            if (sourceCount != checkCount && Math.Abs(sourceCount - checkCount) != 1) return false;

            var checkClone = check.Clone();
            foreach (var point in source)
            {
                //находим такую же точку в checkPoints
                var samePoint = checkClone.FirstOrDefault(a => a.Equals(point));
                var index = checkClone.IndexOf(samePoint);
                //если такой точки в checkPoints нет, значит полигоны разные
                if (index == -1) return false;
                //если такая точка найдена, то удаляем ее из checkPoints
                checkClone.Remove(samePoint);
            }
            return checkClone.Count == 0;
        }

        #endregion

        #region TRY GET SNAKE

        private static void RecalculateGroupAsSnake(ref List<Segment> group)
        {
            var snake = new List<Segment>();
            var newSegment = new Segment();
            //начальгная точка змейки - это начальная точка в группе коротких последовательных сегментов
            newSegment.StartPoint = new IntPoint(group[0].StartPoint);
            for (var i = 1; i < group.Count; i++)
            {
                //сначала вычисляем конечную точку сегмента змейки, потом начальную
                //вычисляем координаты конечной точки для сегмента змейки
                var newEndPoint = (group[i].StartPoint + group[i - 1].EndPoint) / 2;
                newSegment.EndPoint = newEndPoint;
                //вычисляем поправочный коэффициент экструзии
                newSegment.ExtrusionWidthCorrection = (double)group[i - 1].LengthUM / (double)newSegment.LengthUM;
                //добавляем сегмент в змейку
                snake.Add(newSegment);
                //инициализируем заново, устанавливаем новое значение стартовой точки
                newSegment = new Segment();
                newSegment.StartPoint = newEndPoint;
            }
            //конечная точка последнего элемента устанавливается отдельно
            newSegment.EndPoint = new IntPoint(group.Last().EndPoint);
            //перерасчитываем коэффициент экструзии
            newSegment.ExtrusionWidthCorrection = (double)group.Last().LengthUM / (double)newSegment.LengthUM;
            snake.Add(newSegment);
            group = snake;
        }

        /// <summary>
        /// По листу сегментов segments пытается найти короткие и близкие друг другу и заменить их на змейку
        /// </summary>
        /// <param name="segments">Лист сегментов</param>
        /// <param name="ewum">Толщина экструзии</param>
        /// <returns>Лист "змеек" и неизмененых сегментов</returns>
        public static List<List<Segment>> TryGetSnakes(List<Segment> segments, long ewum)
        {
            if (segments == null || !segments.Any()) return null;

            // максимальная длина для поиска коротких сегментов - это удвоенная толщина экструзии
            var tooShortLength = 2 * ewum;
            // максимальная длина в квадртае (для более быстрых вычислений)
            var tooShortLengthSquared = tooShortLength * tooShortLength;

            //клонируем лист сегментов, чтобы его не испортить
            var segmentsCopy = segments.Clone();

            var resPolys = new List<List<Segment>>();
            var resPoly = new List<Segment>();

            //для всех сешментов кроме последнего
            for (var i = 0; i < segmentsCopy.Count - 1; i++)
            {
                var segment = segmentsCopy[i];
                var nextSegment = segmentsCopy[i + 1];
                //сначала просто добавляем в группу
                resPoly.Add(segment);
                //если группа заканчивается (если сегмент слишком длинный, или его расстояние до следующего слишком больше)
                if (segment.LengthUM > tooShortLength || nextSegment.LengthUM > tooShortLength || (segment.StartPoint - nextSegment.EndPoint).LengthSquared() > tooShortLengthSquared)
                {
                    //добавляем в результирующую коллекцию клон сегментов
                    resPolys.Add(resPoly.Clone());
                    //очищаем группу
                    resPoly.Clear();
                }
            }
            //для последнего элемента
            var segmentLast = segmentsCopy.Last();
            resPoly.Add(segmentLast);
            resPolys.Add(resPoly.Clone());
            resPoly.Clear();

            for (var i = 0; i < resPolys.Count; i++)
            {
                if (resPolys[i].Count > 1)
                {
                    var poly = resPolys[i];
                    RecalculateGroupAsSnake(ref poly);
                    resPolys[i] = poly;
                }
            }

            return resPolys;
        }

        #endregion

        #region TRIM POLYGON

        /// <summary>
        /// Разрывает полигон во избежание наложения пластика друг на друга
        /// </summary>
        /// <param name="polygon">Полигон</param>
        /// <param name="ewum">Толщина экструзии</param>
        /// <param name="ratioGap">Величина зазора (1 - зазор на расстоянии ewum, 0 - нет зазора)</param>
        /// <returns></returns>
        public static void TrimPolygon(ref SegmentPolygon polygon, int ewum, double ratioGap)
        {
            //если полигон нудевой, или он из одного сегмента, или ratioGap имеет отрицательное значение
            if (polygon == null || !polygon.Any() || polygon.Count == 1 || ratioGap <= 0) return;

            //вычисляем расстояние зазора - на столько надо отодвинуть конец полигона
            var dist = (int)(ewum * ratioGap);

            //по всем точкам в полигона
            for (var i = polygon.Count - 1; i >= 0; i--)
            {
                //если длина последнего сегмента слишком мала, то вычтем ее из необходимой длины 
                if (polygon[i].LengthUM <= dist) dist -= polygon[i].LengthUM;
                else
                {
                    //разделим сегмент на расстоянии dist от конца
                    var vectorPair = polygon[i].SplitFromEnd(dist);
                    //конец после точки разреза нам не нужен, а сегмент от начала старого сегмента до точки разреза - берем как новый polygon[i]
                    polygon[i] = vectorPair.fromStartToMiddle;
                    //отбрасываем часть полигона после i
                    polygon = polygon.GetRange(0, i + 1);
                    return;
                }
            }
        }

        /// <summary>
        /// Разрывает полигон во избежание наложения пластика друг на друга, отрезая не конец а начало
        /// </summary>
        /// <param name="polygon">Полигон</param>
        /// <param name="ewum">Толщина экструзии</param>
        /// <param name="ratioGap">Величина зазора (1 - зазор на расстоянии ewum, 0 - нет зазора)</param>
        /// <returns></returns>
        public static void TrimPolygonFromStart(ref SegmentPolygon polygon, int ewum, double ratioGap)
        {
            //если полигон нудевой, или он из одного сегмента, или ratioGap имеет отрицательное значение
            if (polygon == null || !polygon.Any() || polygon.Count == 1 || ratioGap <= 0) return;

            //вычисляем расстояние зазора - на столько надо отодвинуть конец полигона
            var dist = (int)(ewum * ratioGap);

            //по всем точкам в полигона
            for (var i = 0; i < polygon.Count; i++)
            {
                //если длина последнего сегмента слишком мала, то вычтем ее из необходимой длины 
                if (polygon[i].LengthUM <= dist) dist -= polygon[i].LengthUM;
                else
                {
                    //разделим сегмент на расстоянии dist от начала
                    var (_, fromMiddleToEnd) = polygon[i].SplitFromStart(dist);
                    //конец после точки разреза нам не нужен, а сегмент от начала старого сегмента до точки разреза - берем как новый polygon[i]
                    polygon[i] = fromMiddleToEnd;
                    //отбрасываем часть полигона до i включительно
                    polygon = polygon.Skip(i).ToList();
                    return;
                }
            }
        }

        #endregion

        #region PROCESS PATHS

        /// <summary>
        /// Модиифицирует пути таким образом, чтобы в одном пути содержались витки, между которыми можно переходить без обрезки
        /// В случае если нужна обрезка - то такой путь разбивается на несколько в тех местах, где нужна обрезка
        /// </summary>
        /// <param name="paths">Пути для обработки</param>
        /// <param name="layerIndex">Номер слоя</param>
        /// <param name="ewum">Толщина экструзии</param>
        /// <param name="lengthOfCutExtended"></param>
        public static void ProcessPaths(ref List<IPolygonPath> paths, int layerIndex, int ewum, int lengthOfCutExtended)
        {
            //https://yadi.sk/i/gYuNDlaX3af578

            //https://yadi.sk/i/NKg49VAB3af5WN

            if (paths == null || !paths.Any()) return;

            //расстояние оффсета для соседних витков - небольшое увеличение относительно толщины экструзии, достаточное, чтобы офсеты перекрывались
            var offsetDistanceBetweenPair = (int)(ewum * 1.04);

            InsetFArea UpdateIFA(InsetFArea area, int bestSegmentIndex)
            {
                var newArea = area;
                newArea.Middle = area.Middle.ReorderList(bestSegmentIndex);
                return newArea;
            }

            //индекс пути
            var pathIndex = 0;
            //пока есть непройденные пути
            while (pathIndex < paths.Count)
            {
                var currentPath = paths[pathIndex];
                if (currentPath.Nodes == null || !currentPath.Nodes.Any())
                {
                    paths.RemoveAt(pathIndex);
                    continue;
                }
                if (currentPath.Length < lengthOfCutExtended)
                {
                    pathIndex++;
                    continue;
                }
                //если в пути только один узел
                if (currentPath.Nodes.Count == 1)
                {
                    var firstNodeDataMiddle = currentPath.Nodes.First().Data.Middle;
                    //находим лучший сегмент по номеру слоя
                    var bestSegmentOneNode = SeamHider.GetBestSegment(firstNodeDataMiddle, layerIndex);
                    //индекс лучшего сегмента в узле
                    var indexOfBestPointOneNode = firstNodeDataMiddle.IndexOf(bestSegmentOneNode.Segment);
                    //пересортировываем лист так, чтобы лучший сегмент был первым
                    currentPath.Nodes.First().Data = UpdateIFA(currentPath.Nodes.First().Data, indexOfBestPointOneNode);
                    //прибавляем индекс пути - переходим к следующему
                    pathIndex++;
                }
                else
                {
                    //вычисляем дистанцию для оффсета такую, чтобы новый оффсет выяснял плотно ли лежат все оффсеты
                    //это можно узнать сделав проверочный оффсет, котоырй чуть больше, чем предполагается были сделаны оффсеты, если они лежали друг с другом
                    //delta обеспечивает "захождение" оффсета внутрь самого внутреннего предполагаемого оффсета
                    var offsetCount = paths[pathIndex].Nodes.Count - 1;
                    var offsetDistance = offsetCount * offsetDistanceBetweenPair;
                    var mostOuter = currentPath.Nodes.Last().Data.Middle.SegmentsToPoints();
                    var mostOuterOffset = mostOuter.Offset(-offsetDistance, 10);
                    var mostOuterOffsetIslands = mostOuterOffset.ProcessIntoSeparateIsland();
                    var mostInner = currentPath.Nodes.First().Data.Middle;
                    //вычитаем из самого сложенного как контура (не области) область внешнего оффсета (как области)
                    var diff = mostInner.DifferenceForContour(mostOuterOffsetIslands);
                    //если разница не пустая - значит оффсеты генерировались нормально
                    //т.е. между двумя соседними не было промежутка большего, чем толщина экструзии
                    if (diff != null && diff.Any())
                    {
                        //находим наилучшую точку в местах этих пересечений
                        //здесь diff может быть как целым контуром, так и частью контура (см рисунок)
                        var bestPoint = SeamHider.GetBestPoint(diff, layerIndex);

                        var bestOuterPoint = PolygonHelper.GetClosestPoint(new Island(){ mostOuter }, bestPoint);
                        var vectorSegment = new Segment(bestPoint, bestOuterPoint);
                        vectorSegment.StartPoint = vectorSegment.GetPointFromStartByDist(-ewum / 2);
                        vectorSegment.EndPoint = vectorSegment.GetPointFromEndByDist(-ewum / 2);

                        for (var i = 0; i < currentPath.Nodes.Count; i++)
                        {
                            if (currentPath.Nodes[i].Data.Middle.HasIntersect(vectorSegment, out var intersectionPoint, out var bestSegmentMiddle, out _))
                            {
                                var middle = currentPath.Nodes[i].Data.Middle;
                                var bestSegment = SeamHider.SplitByBest(ref middle, intersectionPoint, bestSegmentMiddle);
                                currentPath.Nodes[i].Data = UpdateIFA(currentPath.Nodes[i].Data, bestSegment.SegmentIndex);
                            }
                            else
                            {
                                throw new Exception("ProcessPaths no intersection");
                            }
                        }
                        //переходим к следующему пути
                        pathIndex++;
                    }
                    //если разница пустая - т.е. был значительный промежуток между какими-то инсетами (см рисунок 2)
                    else
                    {
                        //создаем заготовку для новых путей
                        var newPaths = new List<PolygonPath>();
                        //создаем заготовку для новогоо пути
                        var newPath = new PolygonPath();
                        //в новый путь сразу записываем самый вложенный узел
                        newPath.AddNode(currentPath.Nodes.First());
                        //для всех оставшихся узлов
                        for (var i = 1; i < currentPath.Nodes.Count; i++)
                        {
                            var offsetDistance2 = i * offsetDistanceBetweenPair;
                            //оффестив следующий более внешний узел внутрь
                            var offsetNext = currentPath.Nodes[i].Data.Middle.SegmentsToPoints().Offset(-offsetDistance2, 10);
                            var offsetNextIslands = offsetNext.ProcessIntoSeparateIsland();
                            //пересекаем последний узел в новом пути (более вложенный вложенный чем currentPath.Nodes[i]) с заофсеченной областью более внешнего узла
                            var nodesDiff = newPath.Nodes.First().Data.Middle.DifferenceForContour(offsetNextIslands);
                            //если пересечение пустое - между этими инсетами есть значительный промежуток
                            if (nodesDiff == null || !nodesDiff.Any())
                            {
                                //создаем разрыв в путях
                                //добавляем в пути новый путь
                                newPaths.Add(newPath);
                                //создаем новый путь заново
                                newPath = new PolygonPath();
                            }
                            //в новый путь добавляем узел
                            newPath.AddNode(currentPath.Nodes[i]);
                        }
                        //если в новом путе есть узлы, то добавляем этот путь в лист путей
                        if (newPath.Nodes != null && newPath.Nodes.Any()) newPaths.Add(newPath);
                        //удаляем из путей текущий (который разбился на несколько)
                        paths.RemoveAt(pathIndex);
                        //и добавляем в конец новые пути, чтобы обработать их еще раз как уже не имеющие разрыва
                        paths.AddRange(newPaths);
                        //НЕ УВЕЛИЧИВАЕМ pathIndex - нужно пройти еще раз по тому же индексу, т.к. был удален элемент
                    }
                }
            }
        }

        #endregion
    }

    public interface IPolygonOptimizerWrapper
    {
        List<SegmentPolygon> Optimize(List<SegmentPolygon> polygons, PolygonOptimizer.GetBestSegmentWay bestSegmentWay, ref IntPoint lastPoint,
            int closeDistance);

        void ProcessPaths(ref List<IPolygonPath> paths, int layerIndex, int ewum, int lengthOfCut);

        List<SegmentPolygon> OptimizeByStartPoint(List<SegmentPolygon> polygons, ref IntPoint lastPoint);

        List<(SegmentPolygon polygon, SegmentPolygon tail)> OptimizeByStartPoint(List<(SegmentPolygon polygon, SegmentPolygon tail)> polygons,
            ref IntPoint lastPoint);

        List<SegmentPolygon> OptimizeWithNesting(List<List<SegmentPolygon>> nestedPolygons,
            ref IntPoint lastPoint, int EWUM);

        List<List<Segment>> TryGetSnakes(List<Segment> segments, long ewum);

        void TrimPolygon(ref SegmentPolygon polygon, int ewum, double ratioGap);

    }

    [ExcludeFromCodeCoverage]
    public class PolygonOptimizerWrapper : IPolygonOptimizerWrapper
    {
        public List<SegmentPolygon> Optimize(List<SegmentPolygon> polygons, PolygonOptimizer.GetBestSegmentWay bestSegmentWay, ref IntPoint lastPoint, int closeDistance)
        {
            return PolygonOptimizer.Optimize(polygons, bestSegmentWay, ref lastPoint, closeDistance);
        }

        public void ProcessPaths(ref List<IPolygonPath> paths, int layerIndex, int ewum, int lengthOfCut)
        {
            PolygonOptimizer.ProcessPaths(ref paths, layerIndex, ewum, lengthOfCut);
        }

        public List<SegmentPolygon> OptimizeByStartPoint(List<SegmentPolygon> polygons, ref IntPoint lastPoint)
        {
            return PolygonOptimizer.OptimizeByStartPoint(polygons, ref lastPoint);
        }

        public List<(SegmentPolygon polygon, SegmentPolygon tail)> OptimizeByStartPoint(List<(SegmentPolygon polygon, SegmentPolygon tail)> polygons, ref IntPoint lastPoint)
        {
            return PolygonOptimizer.OptimizeByStartPoint(polygons, ref lastPoint);
        }


        public List<SegmentPolygon> OptimizeWithNesting(List<List<SegmentPolygon>> nestedPolygons,
            ref IntPoint lastPoint, int EWUM)
        {
            return PolygonOptimizer.OptimizeWithNesting(nestedPolygons, ref lastPoint, EWUM);
        }

        public List<SegmentPolygon> TryGetSnakes(SegmentPolygon segments, long ewum)
        {
            return PolygonOptimizer.TryGetSnakes(segments, ewum);
        }

        public void TrimPolygon(ref SegmentPolygon polygon, int ewum, double ratioGap)
        { 
            PolygonOptimizer.TrimPolygon(ref polygon, ewum, ratioGap);
        }
    }
}