using System;
using System.Collections.Generic;
using MSClipperLib;
using Util;
using Polygon = System.Collections.Generic.List<MSClipperLib.IntPoint>;

namespace Settings.Segment
{
    [Flags]
    public enum Directions
    {
        None = 0,
        OX = 1,
        OY = 1 << 1,
        OZ = 1 << 2,
    }

    public class Segment
    {
        public bool IsBridge { get; set; }

        #region START POINT

        private IntPoint _startPoint;
        public IntPoint StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                RefreshLength();
                RefreshDirection();
                RefreshCoeff();
            }
        }

        #endregion

        #region END POINT

        private IntPoint _endPoint;
        public IntPoint EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint = value;
                RefreshLength();
                RefreshDirection();
                RefreshCoeff();
            }
        }

        #endregion

        #region EXTRUSION WIDTH CORRECTION

        public double ExtrusionWidthCorrection { get; set; }

        #endregion

        #region LENGTH

        public int LengthUM { get; private set; }

        public double Length => LengthUM / 1000;

        #endregion

        #region DIRECTION

        public Directions Direction { get; private set; }

        #endregion

        #region COEFFICIENT

        public PlaneCoefficients Coefficients { get; set; }
        //a1 * x + b1 * y = c1
        //a1 = y2 - y1
        //b1 = x1 - x2
        //c1 = x1(y2-y1) - y1(x2-x1)

        //a2 * x + b2 * z = c2
        //a2 = z2 - z1
        //b2 = x1 - x2
        //c2 = x1(z2-z1) - z1(x2-x1)

        #endregion

        public Segment()
        {
            IsBridge = false;
            ExtrusionWidthCorrection = 1;
        }


        public Segment(IntPoint startPoint, IntPoint endPoint) : this()
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Segment(Segment s) : this()
        {
            StartPoint = new IntPoint(s.StartPoint);
            EndPoint = new IntPoint(s.EndPoint);
        }


        public IntPoint GetPointFromStartByDist(int distanceUM)
        {
            if (distanceUM > LengthUM) return new IntPoint(EndPoint);

            var vect = EndPoint - StartPoint;
            return StartPoint + vect * distanceUM / LengthUM;

        }

        public IntPoint GetPointFromEndByDist(int distanceUM)
        {
            if (distanceUM > LengthUM) return new IntPoint(StartPoint);

            var vect = StartPoint - EndPoint;
            return EndPoint + vect * distanceUM / LengthUM;
        }


        public bool Split(IntPoint pointOnSegment, out Segment segmentFromStartToPoint, out Segment segmentFromPointToEnd)
        {
            segmentFromStartToPoint = new Segment();
            segmentFromPointToEnd = new Segment();

            //проверяем лежит ли точка на сегменте
            var isPointOnSegment = IsPointOnSegment(pointOnSegment);
            if (!isPointOnSegment) return false;

            segmentFromStartToPoint = new Segment(this)
            {
                EndPoint = new IntPoint(pointOnSegment)
            };

            segmentFromPointToEnd = new Segment(this)
            {
                StartPoint = new IntPoint(pointOnSegment)
            };

            return true;
        }

        /// <summary>
        /// Проверяет лежит ли точка на сегменте
        /// </summary>
        /// <param name="point">Точка, которую нужно проверить на принадлежность отрезку</param>
        /// <returns>True, если точка point лежит на отрезке</returns>
        public bool IsPointOnSegment(IntPoint point)
        {
            var boundaryX = (point.X >= StartPoint.X && point.X <= EndPoint.X) || (point.X <= StartPoint.X && point.X >= EndPoint.X);
            var boundaryY = (point.Y >= StartPoint.Y && point.Y <= EndPoint.Y) || (point.Y <= StartPoint.Y && point.Y >= EndPoint.Y);
            var boundaryZ = (point.Z >= StartPoint.Z && point.Z <= EndPoint.Z) || (point.Z <= StartPoint.Z && point.Z >= EndPoint.Z);
            
            //сначала проверяем граничные условия
            if (!boundaryX || !boundaryY || !boundaryZ) return false;

            var v1 = new Vector3(EndPoint - StartPoint);
            var v2 = new Vector3(point - StartPoint);

            var cross = v1.Cross(v2);

            return (int)cross.Length == 0;
        }

        public List<Segment> GetSplitSegments(Polygon splitPolygon, long maxDistance)
        {
            var start2D = new IntPoint(this.StartPoint)
            {
                Z = 0
            };
            var end2D = new IntPoint(this.EndPoint)
            {
                Z = 0
            };

            var requiredSplits2D = new SortedList<long, IntPoint>();

            //вектор от конца сегмента к началу
            var direction = (end2D - start2D);
            //длина вектора
/*            long length = direction.le();*/
            return null;
        }

        public void MergeWithNext(Segment next)
        {
            this.EndPoint = next.EndPoint;
        }


        private void RefreshLength()
        {
            var vectX = EndPoint.X - StartPoint.X;
            var vectY = EndPoint.Y - StartPoint.Y;
            var vectZ = EndPoint.Z - StartPoint.Z;

            LengthUM = (int)Math.Sqrt(vectX * vectX + vectY * vectY + vectZ * vectZ);
        }

        private void RefreshDirection()
        {
            Directions dir = 0;

            if (StartPoint.X != EndPoint.X) dir = dir | Directions.OX;
            if (StartPoint.Y != EndPoint.Y) dir = dir | Directions.OY;
            if (StartPoint.Z != EndPoint.Z) dir = dir | Directions.OZ;

            Direction = dir;
        }

        private void RefreshCoeff()
        {
            Coefficients = new PlaneCoefficients(StartPoint, EndPoint);
        }
    }

    public struct PlaneCoefficients
    {
        public long A1 { get; set; }
        public long B1 { get; set; }
        public long C1 { get; set; }
        public long A2 { get; set; }
        public long B2 { get; set; }
        public long C2 { get; set; }

        public PlaneCoefficients(IntPoint startPoint, IntPoint endPoint) : this()
        {
            var x1 = startPoint.X;
            var y1 = startPoint.Y;
            var z1 = startPoint.Z;

            var x2 = endPoint.X;
            var y2 = endPoint.Y;
            var z2 = endPoint.Z;

            A1 = y2 - y1;
            B1 = x1 - x2;
            C1 = x1 * (y2 - y1) - y1 * (x2 - x1);

            A2 = z2 - z1;
            B2 = x1 - x2;
            C2 = x1 * (z2 - z1) - z1 * (x2 - x1);
        }
    }
}