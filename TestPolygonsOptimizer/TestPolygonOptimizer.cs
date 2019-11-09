using System.Collections.Generic;
using System.Linq;
using MSClipperLib;
using NUnit.Framework;
using Util.GeometryBasics;
using Util.PolygonOptimizer;
using Util.TreeBuilder;

namespace TestUtils.PolygonsOptimizer
{
    [TestFixture]
    public partial class TestPolygonOptimizer
    {
        [Test]
        public void PolygonOptimizer_NewOrder_LstCountNull()
        {
            var lst = PolygonOptimizer.NewOrder(0, 4);
            Assert.IsNull(lst);
        }

        [Test]
        public void PolygonOptimizer_NewOrder_Case1()
        {
            var lst = PolygonOptimizer.NewOrder(7, 4);

            Assert.AreEqual(7, lst.Count);
            Assert.AreEqual(3, lst[0]);
            Assert.AreEqual(4, lst[1]);
            Assert.AreEqual(5, lst[2]);
            Assert.AreEqual(6, lst[3]);
            Assert.AreEqual(0, lst[4]);
            Assert.AreEqual(1, lst[5]);
            Assert.AreEqual(2, lst[6]);
        }

        [Test]
        public void PolygonOptimizer_NewOrder_Case2()
        {
            var lst = PolygonOptimizer.NewOrder(7, 0);

            Assert.AreEqual(7, lst.Count);
            Assert.AreEqual(0, lst[0]);
            Assert.AreEqual(1, lst[1]);
            Assert.AreEqual(2, lst[2]);
            Assert.AreEqual(3, lst[3]);
            Assert.AreEqual(4, lst[4]);
            Assert.AreEqual(5, lst[5]);
            Assert.AreEqual(6, lst[6]);
        }

        [Test]
        public void PolygonOptimizer_NewOrder_Case3()
        {
            var lst = PolygonOptimizer.NewOrder(7, 6);

            Assert.AreEqual(7, lst.Count);
            Assert.AreEqual(1, lst[0]);
            Assert.AreEqual(2, lst[1]);
            Assert.AreEqual(3, lst[2]);
            Assert.AreEqual(4, lst[3]);
            Assert.AreEqual(5, lst[4]);
            Assert.AreEqual(6, lst[5]);
            Assert.AreEqual(0, lst[6]);
        }

        [Test]
        public void PolygonOptimizer_PolygonNull_Case1()
        {
            var newOrder = new List<IntPoint>().ReorderList(4);
            Assert.IsNull(newOrder);
        }

        [Test]
        public void PolygonOptimizer_ReorderList_Case1()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(1000, 1000);
            var p2 = new IntPoint(2000, 2000);
            var p3 = new IntPoint(3000, 3000);
            var p4 = new IntPoint(4000, 4000);
            var p5 = new IntPoint(5000, 5000);
            var p6 = new IntPoint(6000, 6000);

            var p = new List<IntPoint> { p0, p1, p2, p3, p4, p5, p6 };

            var newOrder = p.ReorderList(4);
            Assert.AreEqual(p4, newOrder[0]);
            Assert.AreEqual(p5, newOrder[1]);
            Assert.AreEqual(p6, newOrder[2]);
            Assert.AreEqual(p0, newOrder[3]);
            Assert.AreEqual(p1, newOrder[4]);
            Assert.AreEqual(p2, newOrder[5]);
            Assert.AreEqual(p3, newOrder[6]);
        }

        [Test]
        public void PolygonOptimizer_ReorderList_Case2()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(1000, 1000);

            var p = new List<IntPoint> { p0, p1 };

            var newOrder = p.ReorderList(1);
            Assert.AreEqual(p0, newOrder[1]);
            Assert.AreEqual(p1, newOrder[0]);
        }

        [Test]
        public void PolygonOptimizer_IsTheSamePoly_PolygonIsEmpty()
        {
            var p = new List<IntPoint> { };
            var s = p.PointsToSegments(true);
            var s1 = p.PointsToSegments(true);

            var isTheSame = s.IsTheSamePoly(s1);

            Assert.IsFalse(isTheSame);
        }

        [Test]
        public void PolygonOptimizer_IsTheSamePoly_SamePoly()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(1000, 0);
            var p2 = new IntPoint(1000, 1000);
            var p3 = new IntPoint(0, 1000);

            var p = new List<IntPoint> { p0, p1, p2, p3 };
            var s = p.PointsToSegments(true);
            var s1 = p.PointsToSegments(true);

            var isTheSame = s.IsTheSamePoly(s1);

            Assert.IsTrue(isTheSame);
        }

        [Test]
        public void PolygonOptimizer_IsTheSamePoly_NotSamePoly()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(1000, 0);
            var p2 = new IntPoint(1000, 1000);
            var p3 = new IntPoint(0, 1000);
            var p4 = new IntPoint(100, 1000);

            var p = new List<IntPoint> { p0, p1, p2, p3 };
            var s = p.PointsToSegments(true);

            var pp = new List<IntPoint> { p0, p1, p2, p4 };
            var s1 = pp.PointsToSegments(true);

            var isTheSame = s.IsTheSamePoly(s1);

            Assert.IsFalse(isTheSame);
        }

        [Test]
        public void PolygonOptimizer_IsTheSamePoly_NotSamePoly_DifferentCounts()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(1000, 0);
            var p2 = new IntPoint(1000, 1000);
            var p3 = new IntPoint(0, 1000);
            var p4 = new IntPoint(100, 1000);

            var p = new List<IntPoint> { p0, p1, p2 };
            var s = p.PointsToSegments(true);

            var pp = new List<IntPoint> { p0, p1, p2, p3, p4 };
            var s1 = pp.PointsToSegments(true);

            var isTheSame = s.IsTheSamePoly(s1);

            Assert.IsFalse(isTheSame);
        }

        [Test]
        public void PolygonOptimizer_TryGetSnakes_NullPolygon()
        {
            var polygons = PolygonOptimizer.TryGetSnakes(new List<Segment>(), 1000);
            Assert.IsNull(polygons);
        }

        [Test]
        public void PolygonOptimizer_TryGetSnakes_HasLongGroups()
        {
            var pl00 = new IntPoint(-1000, 0);
            var pl01 = new IntPoint(-1000, 5000);
            var sl = new Segment(pl01, pl00);

            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(0, 1000);
            var s0 = new Segment(p00, p01);

            var p10 = new IntPoint(1000, 0);
            var p11 = new IntPoint(1000, 2000);
            var s1 = new Segment(p11, p10);

            var p20 = new IntPoint(2000, 0);
            var p21 = new IntPoint(2000, 2000);
            var s2 = new Segment(p20, p21);

            var p30 = new IntPoint(3000, 0);
            var p31 = new IntPoint(3000, 2000);
            var s3 = new Segment(p31, p30);

            var p40 = new IntPoint(4000, 0);
            var p41 = new IntPoint(4000, 1000);
            var s4 = new Segment(p40, p41);

            var p50 = new IntPoint(5000, 0);
            var p51 = new IntPoint(5000, 2000);
            var s5 = new Segment(p51, p50);

            var p60 = new IntPoint(6000, 0);
            var p61 = new IntPoint(6000, 1000);
            var s6 = new Segment(p60, p61);

            var p70 = new IntPoint(7000, 0);
            var p71 = new IntPoint(7000, 2000);
            var s7 = new Segment(p71, p70);

            var p80 = new IntPoint(10000, 0);
            var p81 = new IntPoint(10000, 2000);
            var s8 = new Segment(p80, p81);

            var p90 = new IntPoint(11000, 0);
            var p91 = new IntPoint(11000, 1000);
            var s9 = new Segment(p91, p90);

            var p100 = new IntPoint(12000, 0);
            var p101 = new IntPoint(12000, 1000);
            var s10 = new Segment(p100, p101);
            
            var p110 = new IntPoint(13000, 0);
            var p111 = new IntPoint(13000, 1000);
            var s11 = new Segment(p111, p110);

            var p120 = new IntPoint(14000, 0);
            var p121 = new IntPoint(14000, 3000);
            var s12 = new Segment(p120, p121);

            var p130 = new IntPoint(15000, 0);
            var p131 = new IntPoint(15000, 4000);
            var s13 = new Segment(p131, p130);

            var p140 = new IntPoint(16000, 0);
            var p141 = new IntPoint(16000, 5000);
            var s14 = new Segment(p140, p141);
            
            var hugeMess = new List<Segment>{  sl, s0, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14 };

            var polygons = PolygonOptimizer.TryGetSnakes(hugeMess, 1000);

            Assert.AreEqual(6, polygons.Count);

            Assert.IsTrue(polygons[0][0].ExtrusionWidthCorrection == 1);

            Assert.AreEqual(new IntPoint(0, 0), polygons[1][0].StartPoint);
            Assert.AreEqual(new IntPoint(500, 1500), polygons[1][0].EndPoint);
            Assert.IsTrue(polygons[1][0].ExtrusionWidthCorrection != 1);

            Assert.AreEqual(new IntPoint(500, 1500), polygons[1][1].StartPoint);
            Assert.AreEqual(new IntPoint(1500, 0), polygons[1][1].EndPoint);
            Assert.IsTrue(polygons[1][1].ExtrusionWidthCorrection != 1);

            Assert.AreEqual(new IntPoint(1500, 0), polygons[1][2].StartPoint);
            Assert.AreEqual(new IntPoint(2500, 2000), polygons[1][2].EndPoint);
            Assert.IsTrue(polygons[1][2].ExtrusionWidthCorrection != 1);

            Assert.AreEqual(new IntPoint(2500, 2000), polygons[1][3].StartPoint);
            Assert.AreEqual(new IntPoint(3500, 0), polygons[1][3].EndPoint);
            Assert.IsTrue(polygons[1][3].ExtrusionWidthCorrection != 1);
            
            Assert.AreEqual(new IntPoint(3500, 0), polygons[1][4].StartPoint);
            Assert.AreEqual(new IntPoint(4500, 1500), polygons[1][4].EndPoint);
            Assert.IsTrue(polygons[1][4].ExtrusionWidthCorrection != 1);

            Assert.AreEqual(new IntPoint(4500, 1500), polygons[1][5].StartPoint);
            Assert.AreEqual(new IntPoint(5500, 0), polygons[1][5].EndPoint);
            Assert.IsTrue(polygons[1][5].ExtrusionWidthCorrection != 1);
            
            Assert.AreEqual(new IntPoint(5500, 0), polygons[1][6].StartPoint);
            Assert.AreEqual(new IntPoint(6500, 1500), polygons[1][6].EndPoint);
            Assert.IsTrue(polygons[1][6].ExtrusionWidthCorrection != 1);
           
            Assert.AreEqual(new IntPoint(6500, 1500), polygons[1][7].StartPoint);
            Assert.AreEqual(new IntPoint(7000, 0), polygons[1][7].EndPoint);
            Assert.IsTrue(polygons[1][7].ExtrusionWidthCorrection != 1);
            
            Assert.AreEqual(new IntPoint(10000, 0), polygons[2][0].StartPoint);
            Assert.AreEqual(new IntPoint(10500, 1500), polygons[2][0].EndPoint);
            Assert.IsTrue(polygons[2][0].ExtrusionWidthCorrection != 1);

            Assert.AreEqual(new IntPoint(10500, 1500), polygons[2][1].StartPoint);
            Assert.AreEqual(new IntPoint(11500, 0), polygons[2][1].EndPoint);
            Assert.IsTrue(polygons[2][1].ExtrusionWidthCorrection != 1);

            Assert.AreEqual(new IntPoint(11500, 0), polygons[2][2].StartPoint);
            Assert.AreEqual(new IntPoint(12500, 1000), polygons[2][2].EndPoint);
            Assert.IsTrue(polygons[2][2].ExtrusionWidthCorrection != 1);

            Assert.AreEqual(new IntPoint(12500, 1000), polygons[2][3].StartPoint);
            Assert.AreEqual(new IntPoint(13000, 0), polygons[2][3].EndPoint);
            Assert.IsTrue(polygons[2][3].ExtrusionWidthCorrection != 1);

            Assert.IsTrue(polygons[3].Count == 1);
            Assert.IsTrue(polygons[4].Count == 1);
            Assert.IsTrue(polygons[5].Count == 1);
        }

        [Test]
        public void PolygonOptimizer_TryGetSnakes_OnlyLongSegments()
        {
            var pl00 = new IntPoint(-1000, 0);
            var pl01 = new IntPoint(-1000, 5000);
            var sl = new Segment(pl01, pl00);

            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(0, 10000);
            var s0 = new Segment(p00, p01);

            var hugeMess = new List<Segment> { sl, s0};

            var polygons = PolygonOptimizer.TryGetSnakes(hugeMess, 1000);

            Assert.IsTrue(polygons[0].Count == 1);
            Assert.IsTrue(polygons[1].Count == 1);
        }

        [Test]
        public void PolygonOptimizer_TryGetSnakes_OneShortSegment()
        {
            var pl00 = new IntPoint(-1000, 0);
            var pl01 = new IntPoint(-1000, 5000);
            var sl = new Segment(pl01, pl00);

            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(0, 1000);
            var s0 = new Segment(p00, p01);


            var hugeMess = new List<Segment> { sl, s0 };

            var polygons = PolygonOptimizer.TryGetSnakes(hugeMess, 1000);

            Assert.IsTrue(polygons[0].Count == 1);
            Assert.IsTrue(polygons[1].Count == 1);
        }

        [Test]
        public void PolygonOptimizer_TryGetSnakes_TwoShortFarSegments()
        {
            var pl00 = new IntPoint(-1000, 0);
            var pl01 = new IntPoint(-1000, 1000);
            var sl = new Segment(pl01, pl00);

            var p00 = new IntPoint(5000, 0);
            var p01 = new IntPoint(5000, 1000);
            var s0 = new Segment(p00, p01);


            var hugeMess = new List<Segment> { sl, s0 };

            var polygons = PolygonOptimizer.TryGetSnakes(hugeMess, 1000);

            Assert.IsTrue(polygons[0].Count == 1);
            Assert.IsTrue(polygons[1].Count == 1);
        }

        [Test]
        public void PolygonOptimizer_TryGetSnakes_TwoShortFarSegments2()
        {
            var pl00 = new IntPoint(-1000, 0);
            var pl01 = new IntPoint(-1000, 5000);
            var sl = new Segment(pl01, pl00);

            var p00 = new IntPoint(5000, 0);
            var p01 = new IntPoint(5000, 5000);
            var s0 = new Segment(p00, p01);


            var hugeMess = new List<Segment> { sl, s0 };

            var polygons = PolygonOptimizer.TryGetSnakes(hugeMess, 1000);

            Assert.IsTrue(polygons[0].Count == 1);
            Assert.IsTrue(polygons[1].Count == 1);
        }

        [Test]
        public void PolygonOptimizer_TrimPolygon_DistEnougthInLastSegment()
        {
            var p00 = new IntPoint(10000, 10000);
            var p01 = new IntPoint(0, 0);
            var s0 = new Segment(p00, p01);

            var p10 = new IntPoint(0, 0);
            var p11 = new IntPoint(10000, 0);
            var s1 = new Segment(p10, p11);

            var p20 = new IntPoint(10000, 0);
            var p21 = new IntPoint(10000, 10000);
            var s2 = new Segment(p20, p21);


            var poly = new List<Segment> { s0, s1, s2 };

            PolygonOptimizer.TrimPolygon(ref poly, 1000, 0.6);

            Assert.AreEqual(3, poly.Count);
            Assert.AreEqual(9400, poly.Last().LengthUM);
        }

        [Test]
        public void PolygonOptimizer_TrimPolygon_DistEnougthNotInLastSegment()
        {
            var p00 = new IntPoint(10000, 10000);
            var p01 = new IntPoint(0, 0);
            var s0 = new Segment(p00, p01);

            var p10 = new IntPoint(0, 0);
            var p11 = new IntPoint(10000, 0);
            var s1 = new Segment(p10, p11);

            var p20 = new IntPoint(10000, 0);
            var p21 = new IntPoint(10000, 9000);
            var s2 = new Segment(p20, p21);

            var p30 = new IntPoint(10000, 9000);
            var p31 = new IntPoint(10000, 9250);
            var s3 = new Segment(p30, p31);

            var p40 = new IntPoint(10000, 9250);
            var p41 = new IntPoint(10000, 9500);
            var s4 = new Segment(p40, p41);

            var p50 = new IntPoint(10000, 9500);
            var p51 = new IntPoint(10000, 9750);
            var s5 = new Segment(p50, p51);

            var p60 = new IntPoint(10000, 9750);
            var p61 = new IntPoint(10000, 10000);
            var s6 = new Segment(p60, p61);


            var poly = new List<Segment> { s0, s1, s2, s3, s4, s5, s6 };

           PolygonOptimizer.TrimPolygon(ref poly, 1000, 0.6);

            Assert.AreEqual(5, poly.Count);
            Assert.AreEqual(150, poly.Last().LengthUM);
        }

        [Test]
        public void PolygonOptimizer_TrimPolygon_OneSegment()
        {
            var p00 = new IntPoint(10000, 10000);
            var p01 = new IntPoint(0, 0);
            var s0 = new Segment(p00, p01);

            var poly = new List<Segment> { s0 };

            PolygonOptimizer.TrimPolygon(ref poly, 1000, 0.6);

            Assert.AreEqual(1, poly.Count);
        }

        [Test]
        public void PolygonOptimizer_TrimPolygon_RatioIsNull()
        {
            var p00 = new IntPoint(10000, 10000);
            var p01 = new IntPoint(0, 0);
            var s0 = new Segment(p00, p01);

            var p10 = new IntPoint(0, 0);
            var p11 = new IntPoint(10000, 0);
            var s1 = new Segment(p10, p11);

            var p20 = new IntPoint(10000, 0);
            var p21 = new IntPoint(10000, 9000);
            var s2 = new Segment(p20, p21);

            var p30 = new IntPoint(10000, 9000);
            var p31 = new IntPoint(10000, 9250);
            var s3 = new Segment(p30, p31);

            var p40 = new IntPoint(10000, 9250);
            var p41 = new IntPoint(10000, 9500);
            var s4 = new Segment(p40, p41);

            var p50 = new IntPoint(10000, 9500);
            var p51 = new IntPoint(10000, 9750);
            var s5 = new Segment(p50, p51);

            var p60 = new IntPoint(10000, 9750);
            var p61 = new IntPoint(10000, 10000);
            var s6 = new Segment(p60, p61);


            var poly = new List<Segment> { s0, s1, s2, s3, s4, s5, s6 };

            PolygonOptimizer.TrimPolygon(ref poly, 1000, 0);

            Assert.AreEqual(7, poly.Count);
        }

        [Test]
        public void PolygonOptimizer_TrimPolygonFromStart_DistEnougthInLastSegment()
        {
            var p00 = new IntPoint(10000, 0);
            var p01 = new IntPoint(0, 0);
            var s0 = new Segment(p00, p01);

            var p10 = new IntPoint(0, 0);
            var p11 = new IntPoint(0, 10000);
            var s1 = new Segment(p10, p11);

            var p20 = new IntPoint(0, 10000);
            var p21 = new IntPoint(10000, 10000);
            var s2 = new Segment(p20, p21);


            var poly = new List<Segment> { s0, s1, s2 };

            PolygonOptimizer.TrimPolygonFromStart(ref poly, 1000, 0.6);

            Assert.AreEqual(3, poly.Count);
            Assert.AreEqual(9400, poly.First().LengthUM);
        }

        [Test]
        public void PolygonOptimizer_TrimPolygonFromStart_DistEnougthNotInLastSegment()
        {
            var p00 = new IntPoint(10000, 10000);
            var p01 = new IntPoint(0, 0);
            var s0 = new Segment(p00, p01);

            var p10 = new IntPoint(0, 0);
            var p11 = new IntPoint(10000, 0);
            var s1 = new Segment(p10, p11);

            var p20 = new IntPoint(10000, 0);
            var p21 = new IntPoint(10000, 9000);
            var s2 = new Segment(p20, p21);

            var p30 = new IntPoint(10000, 9000);
            var p31 = new IntPoint(10000, 9250);
            var s3 = new Segment(p30, p31);

            var p40 = new IntPoint(10000, 9250);
            var p41 = new IntPoint(10000, 9500);
            var s4 = new Segment(p40, p41);

            var p50 = new IntPoint(10000, 9500);
            var p51 = new IntPoint(10000, 9750);
            var s5 = new Segment(p50, p51);

            var p60 = new IntPoint(10000, 9750);
            var p61 = new IntPoint(10000, 10000);
            var s6 = new Segment(p60, p61);


            var poly = new List<Segment> { s0, s1, s2, s3, s4, s5, s6 };
            poly.Reverse();
            for (var i = 0; i < poly.Count; i++)
            {
                poly[i] = poly[i].Flip();
            }

            PolygonOptimizer.TrimPolygonFromStart(ref poly, 1000, 0.6);

            Assert.AreEqual(5, poly.Count);
            Assert.AreEqual(150, poly.First().LengthUM);
        }

        [Test]
        public void PolygonOptimizer_TrimPolygonFromStart_OneSegment()
        {
            var p00 = new IntPoint(10000, 10000);
            var p01 = new IntPoint(0, 0);
            var s0 = new Segment(p00, p01);

            var poly = new List<Segment> { s0 };
            poly.Reverse();
            for (var i = 0; i < poly.Count; i++)
            {
                poly[i] = poly[i].Flip();
            }

            PolygonOptimizer.TrimPolygonFromStart(ref poly, 1000, 0.6);

            Assert.AreEqual(1, poly.Count);
        }

        [Test]
        public void PolygonOptimizer_TrimPolygonFromStart_RatioIsNull()
        {
            var p00 = new IntPoint(10000, 10000);
            var p01 = new IntPoint(0, 0);
            var s0 = new Segment(p00, p01);

            var p10 = new IntPoint(0, 0);
            var p11 = new IntPoint(10000, 0);
            var s1 = new Segment(p10, p11);

            var p20 = new IntPoint(10000, 0);
            var p21 = new IntPoint(10000, 9000);
            var s2 = new Segment(p20, p21);

            var p30 = new IntPoint(10000, 9000);
            var p31 = new IntPoint(10000, 9250);
            var s3 = new Segment(p30, p31);

            var p40 = new IntPoint(10000, 9250);
            var p41 = new IntPoint(10000, 9500);
            var s4 = new Segment(p40, p41);

            var p50 = new IntPoint(10000, 9500);
            var p51 = new IntPoint(10000, 9750);
            var s5 = new Segment(p50, p51);

            var p60 = new IntPoint(10000, 9750);
            var p61 = new IntPoint(10000, 10000);
            var s6 = new Segment(p60, p61);

            var poly = new List<Segment> { s0, s1, s2, s3, s4, s5, s6 };
            poly.Reverse();
            for (var i = 0; i < poly.Count; i++)
            {
                poly[i] = poly[i].Flip();
            }

            PolygonOptimizer.TrimPolygonFromStart(ref poly, 1000, 0);

            Assert.AreEqual(7, poly.Count);
        }

        [Test]
        public void PolygonOptimizer_ProcessPaths_PathHasOneNode_NodeIsEmpty()
        {
            var pp = new PolygonPath();

            var paths = new List<IPolygonPath> { pp };

            PolygonOptimizer.ProcessPaths(ref paths, 0, 500, 1);
            Assert.AreEqual(0, paths.Count);
        }

        [Test]
        public void PolygonOptimizer_ProcessPaths_ShortNode()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);
            var ifa = new InsetFArea();
            ifa.Middle = p;

            var treeNode = new TreeNode(ifa);
            var pp = new PolygonPath();
            pp.AddNode(treeNode);

            var paths = new List<IPolygonPath> { pp };

            PolygonOptimizer.ProcessPaths(ref paths, 0, 500, 50000);
            Assert.AreEqual(1, paths.Count);
            Assert.AreEqual(1, paths[0].Nodes.Count);
            Assert.AreEqual(4, paths[0].Nodes[0].Data.Middle.Count);
            Assert.AreEqual(p0, paths[0].Nodes[0].Data.Middle[0].StartPoint);
        }

        [Test]
        public void PolygonOptimizer_ProcessPaths_PathHasOneNode()
        {
            var p0 = new IntPoint(5000, 0);
            var p1 = new IntPoint(10000, 5000);
            var p2 = new IntPoint(5000, 10000);
            var p3 = new IntPoint(0, 5000);
            var p = new List<IntPoint> { p0, p1, p2, p3 }.PointsToSegments(true);
            var ifa = new InsetFArea();
            ifa.Middle = p;

            var treeNode = new TreeNode(ifa);
            var pp = new PolygonPath();
            pp.AddNode(treeNode);

            var paths = new List<IPolygonPath> { pp };

            PolygonOptimizer.ProcessPaths(ref paths, 0, 500, 1);
            Assert.AreEqual(1, paths.Count);
            Assert.AreEqual(1, paths[0].Nodes.Count);
            Assert.AreEqual(4, paths[0].Nodes[0].Data.Middle.Count);
            Assert.AreEqual(p3, paths[0].Nodes[0].Data.Middle[0].StartPoint);
        }

        [Test]
        public void PolygonOptimizer_ProcessPaths_PathHasTwoNodesDiffIsNotNull()
        {
            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(18000, 0);
            var p02 = new IntPoint(18000, 14000);
            var p03 = new IntPoint(0, 14000);
            var p0 = new List<IntPoint> { p00, p01, p02, p03 }.PointsToSegments(true);
            var ifa0 = new InsetFArea();
            ifa0.Middle = p0;

            var p10 = new IntPoint(5000, 1000);
            var p11 = new IntPoint(13000, 1000);
            var p12 = new IntPoint(13000, 4000);
            var p13 = new IntPoint(17000, 4000);
            var p14 = new IntPoint(17000, 10000);
            var p15 = new IntPoint(13000, 10000);
            var p16 = new IntPoint(13000, 13000);
            var p17 = new IntPoint(5000, 13000);
            var p18 = new IntPoint(5000, 10000);
            var p19 = new IntPoint(1000, 10000);
            var p110 = new IntPoint(1000, 4000);
            var p111 = new IntPoint(5000, 5000);
            var p1 = new List<IntPoint> { p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p110, p111 }.PointsToSegments(true);
            var ifa1 = new InsetFArea();
            ifa1.Middle = p1;

            var treeNode0 = new TreeNode(ifa0);
            var treeNode1 = new TreeNode(ifa1);
            var pp = new PolygonPath();
            pp.AddNode(treeNode1);
            pp.AddNode(treeNode0);

            var paths = new List<IPolygonPath> { pp };

            PolygonOptimizer.ProcessPaths(ref paths, 0, 1000, 1);
            var bestLeftPoint = SeamHider.GetBestPoint(p1.SegmentsToPoints(), 0);
            Assert.AreEqual(bestLeftPoint, paths[0].Nodes[0].Data.Middle[0].StartPoint);

            var bestCloseSegment = SeamHider.GetClosest(p0, bestLeftPoint);
            Assert.AreEqual(bestCloseSegment.Segment, paths[0].Nodes[1].Data.Middle[0]);
        }

        [Test]
        public void PolygonOptimizer_ProcessPaths_PathHasTwoNodesDiffIsNotNullCreateNewPoint()
        {
            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(10000, 0);
            var p02 = new IntPoint(10000, 10000);
            var p03 = new IntPoint(0, 10000);
            var p0 = new List<IntPoint> { p00, p01, p02, p03 }.PointsToSegments(true);
            var ifa0 = new InsetFArea();
            ifa0.Middle = p0;

            var p10 = new IntPoint(1000, 1000);
            var p11 = new IntPoint(9000, 1000);
            var p12 = new IntPoint(9000, 9000);
            var p13 = new IntPoint(1000, 9000);
            var p1 = new List<IntPoint> { p10, p11, p12, p13 }.PointsToSegments(true);
            var ifa1 = new InsetFArea();
            ifa1.Middle = p1;

            var treeNode0 = new TreeNode(ifa0);
            var treeNode1 = new TreeNode(ifa1);
            var pp = new PolygonPath();
            pp.AddNode(treeNode1);
            pp.AddNode(treeNode0);

            var paths = new List<IPolygonPath> { pp };

            PolygonOptimizer.ProcessPaths(ref paths, 1, 1000, 1);
            var bestTopPoint = SeamHider.GetBestPoint(p1.SegmentsToPoints(), 1);
            Assert.AreEqual(bestTopPoint, paths[0].Nodes[0].Data.Middle[0].StartPoint);
            Assert.AreEqual(4, paths[0].Nodes[0].Data.Middle.Count);

            Assert.AreEqual(5, paths[0].Nodes[1].Data.Middle.Count);
            Assert.AreEqual(new IntPoint(10000, 9000), paths[0].Nodes[1].Data.Middle[0].StartPoint);
        }

        [Test]
        public void PolygonOptimizer_ProcessPaths_PathHasThreeNodesDiffIsNullSpaceAfterFirstOuter()
        {
            var p00 = new IntPoint(5000, 0);
            var p01 = new IntPoint(10000, 5000);
            var p02 = new IntPoint(5000, 10000);
            var p03 = new IntPoint(0, 5000);
            var p0 = new List<IntPoint> { p00, p01, p02, p03 }.PointsToSegments(true);
            var ifa0 = new InsetFArea();
            ifa0.Middle = p0;

            var p10 = new IntPoint(5000, 2000);
            var p11 = new IntPoint(8000, 5000);
            var p12 = new IntPoint(5000, 8000);
            var p13 = new IntPoint(2000, 5000);
            var p1 = new List<IntPoint> { p10, p11, p12, p13 }.PointsToSegments(true);
            var ifa1 = new InsetFArea();
            ifa1.Middle = p1;

            var p20 = new IntPoint(5000, 3000);
            var p21 = new IntPoint(7000, 5000);
            var p22 = new IntPoint(5000, 7000);
            var p23 = new IntPoint(3000, 5000);
            var p2 = new List<IntPoint> { p20, p21, p22, p23 }.PointsToSegments(true);
            var ifa2 = new InsetFArea();
            ifa2.Middle = p2;

            var treeNode0 = new TreeNode(ifa0);
            var treeNode1 = new TreeNode(ifa1);
            var treeNode2 = new TreeNode(ifa2);
            var pp = new PolygonPath();
            pp.AddNode(treeNode2);
            pp.AddNode(treeNode1);
            pp.AddNode(treeNode0);

            var paths = new List<IPolygonPath> { pp };

            PolygonOptimizer.ProcessPaths(ref paths, 0, 707, 1);

            var bestLeftPoint = SeamHider.GetBestPoint(p2.SegmentsToPoints(), 0);
            Assert.AreEqual(bestLeftPoint, paths[0].Nodes[0].Data.Middle[0].StartPoint);

            var bestCloseSegment0 = SeamHider.GetClosest(p1, bestLeftPoint);
            Assert.AreEqual(bestCloseSegment0.Segment, paths[0].Nodes[1].Data.Middle[0]);

            var bestCloseSegment1 = SeamHider.GetClosest(p0, bestCloseSegment0.BestPoint);
            Assert.AreEqual(bestCloseSegment1.Segment, paths[1].Nodes[0].Data.Middle[0]);
        }

        [Test]
        public void PolygonOptimizer_ProcessPaths_PathHasThreeNodesDiffIsNullSpaceAfterFirstInner()
        {
            var p00 = new IntPoint(5000, 0);
            var p01 = new IntPoint(10000, 5000);
            var p02 = new IntPoint(5000, 10000);
            var p03 = new IntPoint(0, 5000);
            var p0 = new List<IntPoint> { p00, p01, p02, p03 }.PointsToSegments(true);
            var ifa0 = new InsetFArea();
            ifa0.Middle = p0;

            var p10 = new IntPoint(5000, 1000);
            var p11 = new IntPoint(9000, 5000);
            var p12 = new IntPoint(5000, 9000);
            var p13 = new IntPoint(1000, 5000);
            var p1 = new List<IntPoint> { p10, p11, p12, p13 }.PointsToSegments(true);
            var ifa1 = new InsetFArea();
            ifa1.Middle = p1;

            var p20 = new IntPoint(5000, 3000);
            var p21 = new IntPoint(7000, 5000);
            var p22 = new IntPoint(5000, 7000);
            var p23 = new IntPoint(3000, 5000);
            var p2 = new List<IntPoint> { p20, p21, p22, p23 }.PointsToSegments(true);
            var ifa2 = new InsetFArea();
            ifa2.Middle = p2;

            var treeNode0 = new TreeNode(ifa0);
            var treeNode1 = new TreeNode(ifa1);
            var treeNode2 = new TreeNode(ifa2);
            var pp = new PolygonPath();
            pp.AddNode(treeNode2);
            pp.AddNode(treeNode1);
            pp.AddNode(treeNode0);

            var paths = new List<IPolygonPath> { pp };

            PolygonOptimizer.ProcessPaths(ref paths, 0, 707, 1);

            var bestLeftPoint = SeamHider.GetBestPoint(p2.SegmentsToPoints(), 0);
            Assert.AreEqual(bestLeftPoint, paths[0].Nodes[0].Data.Middle[0].StartPoint);

            var bestCloseSegment0 = SeamHider.GetClosest(p1, bestLeftPoint);
            Assert.AreEqual(bestCloseSegment0.Segment, paths[1].Nodes[0].Data.Middle[0]);

            var bestCloseSegment1 = SeamHider.GetClosest(p0, bestCloseSegment0.BestPoint);
            Assert.AreEqual(bestCloseSegment1.Segment, paths[1].Nodes[1].Data.Middle[0]);
        }
    }
}