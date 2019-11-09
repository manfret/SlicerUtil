using System.Collections.Generic;
using MSClipperLib;
using NUnit.Framework;
using Util.GeometryBasics;
using Util.PolygonOptimizer;
using Polygon = System.Collections.Generic.List<MSClipperLib.IntPoint>;
using SegmentPolygon = System.Collections.Generic.List<Util.GeometryBasics.Segment>;
using Island = System.Collections.Generic.List<System.Collections.Generic.List<MSClipperLib.IntPoint>>;

namespace TestUtils.PolygonsOptimizer
{
    [TestFixture]
    public class TestSeamHider
    {
        [Test]
        public void SeamHider_GetBest_EmptyPolygon()
        {
            //https://yadi.sk/i/KMzSz1A93aeyyz

            var p0 = new IntPoint(8000, 0);

            var bestSegment = SeamHider.GetBest(new SegmentPolygon(), p0);

            Assert.IsNull(bestSegment);
        }

        [Test]
        public void SeamHider_GetBest_CCW_Case1()
        {
            //https://yadi.sk/i/KMzSz1A93aeyyz

            var p0 = new IntPoint(8000, 0);
            var p1 = new IntPoint(16000, 3000);
            var p2 = new IntPoint(11000, 6000);
            var p3 = new IntPoint(15000, 12000);
            var p4 = new IntPoint(8000, 6000);
            var p5 = new IntPoint(11000, 12000);
            var p6 = new IntPoint(0, 9000);

            var p = new Polygon { p0, p1, p2, p3, p4, p5, p6 };

            var bestSegment = SeamHider.GetBest(p.PointsToSegments(true), p0);

            Assert.AreEqual(p4, bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetBest_CW_Case1()
        {
            //https://yadi.sk/i/c8ilrqRc3aezFJ

            var p0 = new IntPoint(8000, 0);
            var p1 = new IntPoint(0, 9000);
            var p2 = new IntPoint(11000, 12000);
            var p3 = new IntPoint(8000, 6000);
            var p4 = new IntPoint(15000, 12000);
            var p5 = new IntPoint(11000, 6000);
            var p6 = new IntPoint(16000, 3000);

            var p = new List<IntPoint> { p0, p1, p2, p3, p4, p5, p6 };

            var bestSegment = SeamHider.GetBest(p.PointsToSegments(true), p0);

            Assert.AreEqual(p4, bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetBest_CW_OneSegmentPolygon()
        {
            var p0 = new IntPoint(8000, 0);
            var p1 = new IntPoint(0, 9000);

            var p = new List<IntPoint> { p0, p1 };

            var bestSegment = SeamHider.GetBest(p.PointsToSegments(true), p0);

            Assert.IsTrue(p0.Equals(bestSegment.BestPoint));
            Assert.AreEqual(0, bestSegment.SegmentIndex);
        }

        [Test]
        public void SeamHider_GetBest_SmoothContour()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(1000, 0);
            var p2 = new IntPoint(1000, 1000);
            var p3 = new IntPoint(0, 1000);

            var p = new List<IntPoint>{ p0, p1, p2, p3 };

            var bestSegment = SeamHider.GetBest(p.PointsToSegments(true), p0);

            Assert.IsTrue(p1.Equals(bestSegment.BestPoint));
            Assert.AreEqual(1, bestSegment.SegmentIndex);

        }

        [Test]
        public void SeamHider_GetClosest_PolygonIsNull()
        {
            var pLast = new IntPoint(0, 0);
            var bestPoint = SeamHider.GetClosest(new SegmentPolygon(), pLast);

            Assert.IsNull(bestPoint);
        }

        [Test]
        public void SeamHider_GetClosest_Case1()
        {
            var p0 = new IntPoint(8000, 0);
            var p1 = new IntPoint(16000, 3000);
            var p2 = new IntPoint(11000, 6000);
            var p3 = new IntPoint(15000, 12000);
            var p4 = new IntPoint(8000, 6000);
            var p5 = new IntPoint(11000, 12000);
            var p6 = new IntPoint(0, 9000);

            var p = new List<IntPoint> { p0, p1, p2, p3, p4, p5, p6 };

            var pLast = new IntPoint(0, 0);
            var bestPoint = SeamHider.GetClosest(p.PointsToSegments(true), pLast);

            Assert.AreEqual(p0, bestPoint.BestPoint);
        }

        [Test]
        public void SeamHider_SplitByBest_PolygonIsNull()
        {
            var p1 = new IntPoint(1000, 0);
            var p2 = new IntPoint(1000, 1000);

            var s1 = new Segment(p1, p2);

            var p = new List<Segment>();
            var best = SeamHider.SplitByBest(ref p, p1, s1);

            Assert.IsNull(best);
        }

        [Test]
        public void SeamHider_SplitByBest_BestInMiddle_Exist()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(1000, 0);
            var p2 = new IntPoint(1000, 1000);
            var p3 = new IntPoint(0, 1000);

            var s0 = new Segment(p0, p1);
            var s1 = new Segment(p1, p2);
            var s2 = new Segment(p2, p3);
            var s3 = new Segment(p3, p0);

            var poly = new List<Segment> { s0, s1, s2, s3 };

            var best = SeamHider.SplitByBest(ref poly, p1, s1);

            Assert.AreEqual(4, poly.Count);
            Assert.AreEqual(p1, best.BestPoint);
            Assert.AreEqual(p1, best.Segment.StartPoint);
            Assert.AreEqual(1, best.SegmentIndex);
        }


        [Test]
        public void SeamHider_SplitByBest_BestInMiddle_NpExist()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(1000, 0);
            var p2 = new IntPoint(1000, 1000);
            var p3 = new IntPoint(0, 1000);

            var s0 = new Segment(p0, p1);
            var s1 = new Segment(p1, p2);
            var s2 = new Segment(p2, p3);
            var s3 = new Segment(p3, p0);

            var poly = new List<Segment> { s0, s1, s2, s3 };

            var best = SeamHider.SplitByBest(ref poly, new IntPoint(500, 0), s1);

            Assert.AreEqual(5, poly.Count);
            Assert.AreEqual(new IntPoint(500, 0), best.BestPoint);
            Assert.AreEqual(new IntPoint(500, 0), best.Segment.StartPoint);
            Assert.AreEqual(2, best.SegmentIndex);
        }


        [Test]
        public void SeamHider_GetClosest_Case2()
        {
            var p0 = new IntPoint(8000, 0);
            var p1 = new IntPoint(16000, 3000);
            var p2 = new IntPoint(11000, 6000);
            var p3 = new IntPoint(15000, 12000);
            var p4 = new IntPoint(8000, 6000);
            var p5 = new IntPoint(11000, 12000);
            var p6 = new IntPoint(0, 8000);

            var p = new List<IntPoint> { p0, p1, p2, p3, p4, p5, p6 };

            var pLast = new IntPoint(0, 0);
            var bestPoint = SeamHider.GetClosest(p.PointsToSegments(true), pLast);

            Assert.IsTrue(p0.Equals(bestPoint.BestPoint));
            Assert.AreEqual(0, bestPoint.SegmentIndex);
        }

        [Test]
        public void SeamHider_GetClosest_OneSegmentPolygon_Case1()
        {
            var p0 = new IntPoint(8000, 0);
            var p1 = new IntPoint(16000, 3000);

            var p = new List<IntPoint> { p0, p1 };

            var pLast = new IntPoint(0, 0);
            var bestPoint = SeamHider.GetClosest(p.PointsToSegments(true), pLast);

            Assert.AreEqual(p0, bestPoint.BestPoint);
            Assert.AreEqual(0, bestPoint.SegmentIndex);
            Assert.AreEqual(p0, bestPoint.Segment.StartPoint);
            Assert.AreEqual(p1, bestPoint.Segment.EndPoint);
        }

        [Test]
        public void SeamHider_GetMostLeft_PolygonIsNull()
        {
            var bestSegment = SeamHider.GetMostLeft(new SegmentPolygon());

            Assert.IsNull(bestSegment);
        }

        [Test]
        public void SeamHider_GetMostLeft_Case1()
        {
            var p0 = new IntPoint(10000, 0);
            var p1 = new IntPoint(10000, 10000);
            var p2 = new IntPoint(0, 10000);
            var p3 = new IntPoint(0, 0);
            var p = new List<IntPoint>{ p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetMostLeft(p);

            Assert.IsTrue(p2 == bestSegment.BestPoint || p3 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetMostLeft_Case2()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetMostLeft(p);
            Assert.IsTrue(p3 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetMostLeftIntPoint_PolygonIsNull()
        {
            var mostLeft = SeamHider.GetMostLeft(new List<IntPoint>());

            Assert.AreEqual(mostLeft, new IntPoint());
        }

        [Test]
        public void SeamHider_GetMostTop_EmptyPolygon()
        {
            var mostBottom = SeamHider.GetMostTop(new Polygon());

            Assert.AreEqual(new IntPoint(), mostBottom);
        }

        [Test]
        public void SeamHider_GetMostTop_Case1()
        {
            var p0 = new IntPoint(10000, 0);
            var p1 = new IntPoint(10000, 10000);
            var p2 = new IntPoint(0, 10000);
            var p3 = new IntPoint(0, 0);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetMostTop(p);

            Assert.IsTrue(p1 == bestSegment.BestPoint || p2 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetMostTop_Case2()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetMostTop(p);
            Assert.IsTrue(p2 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetMostTop_PolygonIsNull()
        {
            var bestSegment = SeamHider.GetMostTop(new SegmentPolygon());

            Assert.IsNull(bestSegment);
        }

        [Test]
        public void SeamHider_GetMostRight_EmptyPolygon()
        {
            var mostBottom = SeamHider.GetMostRight(new Polygon());

            Assert.AreEqual(new IntPoint(), mostBottom);
        }

        [Test]
        public void SeamHider_GetMostRight_Case1()
        {
            var p0 = new IntPoint(10000, 0);
            var p1 = new IntPoint(10000, 10000);
            var p2 = new IntPoint(0, 10000);
            var p3 = new IntPoint(0, 0);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetMostRight(p);

            Assert.IsTrue(p0 == bestSegment.BestPoint || p1 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetMostRight_Case1_Points()
        {
            var p0 = new IntPoint(10000, 0);
            var p1 = new IntPoint(10000, 10000);
            var p2 = new IntPoint(0, 10000);
            var p3 = new IntPoint(0, 0);
            var p = new List<IntPoint> { p0, p1, p2, p3 };

            var bestSegment = SeamHider.GetMostRight(p);

            Assert.IsTrue(p0 == bestSegment || p1 == bestSegment);
        }

        [Test]
        public void SeamHider_GetMostRight_Case2()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetMostRight(p);
            Assert.IsTrue(p1 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetMostRight_PolygonIsNull()
        {
            var bestSegment = SeamHider.GetMostRight(new SegmentPolygon());

            Assert.IsNull(bestSegment);
        }

        [Test]
        public void SeamHider_GetMostBottom_Case1()
        {
            var p0 = new IntPoint(10000, 0);
            var p1 = new IntPoint(10000, 10000);
            var p2 = new IntPoint(0, 10000);
            var p3 = new IntPoint(0, 0);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetMostBottom(p);

            Assert.IsTrue(p0 == bestSegment.BestPoint || p3 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetMostBottom_EmptyPolygon()
        {
            var mostBottom = SeamHider.GetMostBottom(new Polygon());

            Assert.AreEqual(new IntPoint(), mostBottom);
        }

        [Test]
        public void SeamHider_GetMostBottom_Case1_Points()
        {
            var p0 = new IntPoint(10000, 0);
            var p1 = new IntPoint(10000, 10000);
            var p2 = new IntPoint(0, 10000);
            var p3 = new IntPoint(0, 0);
            var p = new List<IntPoint> { p0, p1, p2, p3 };

            var bestSegment = SeamHider.GetMostBottom(p);

            Assert.IsTrue(p0 == bestSegment || p3 == bestSegment);
        }

        [Test]
        public void SeamHider_GetMostBottom_Case2()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetMostBottom(p);
            Assert.IsTrue(p0 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetMostBottom_PolygonIsNull()
        {
            var bestSegment = SeamHider.GetMostBottom(new SegmentPolygon());

            Assert.IsNull(bestSegment);
        }

        [Test]
        public void SeamHider_GetBestSegment_Layer0()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetBestSegment(p, 0);
            Assert.IsTrue(p3 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetBestSegment_Layer1()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetBestSegment(p, 1);
            Assert.IsTrue(p2 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetBestSegment_Layer2()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetBestSegment(p, 2);
            Assert.IsTrue(p1 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetBestSegment_Layer3()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);

            var bestSegment = SeamHider.GetBestSegment(p, 3);
            Assert.IsTrue(p0 == bestSegment.BestPoint);
        }

        [Test]
        public void SeamHider_GetBestPoint_Layer0()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 };

            var bestSegment = SeamHider.GetBestPoint(p, 0);
            Assert.IsTrue(p3 == bestSegment);
        }

        [Test]
        public void SeamHider_GetBestPoint_Layer1()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 };

            var bestSegment = SeamHider.GetBestPoint(p, 1);
            Assert.IsTrue(p2 == bestSegment);
        }

        [Test]
        public void SeamHider_GetBestPoint_Layer2()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 };

            var bestSegment = SeamHider.GetBestPoint(p, 2);
            Assert.IsTrue(p1 == bestSegment);
        }

        [Test]
        public void SeamHider_GetBestPoint_Layer3()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 };

            var bestSegment = SeamHider.GetBestPoint(p, 3);
            Assert.IsTrue(p0 == bestSegment);
        }
    }
}