﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Research.WeightedPINQ;
using Microsoft.Research.WeightedPINQ.Operators;

namespace VLDB2014_Experiments
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //Please change below path to absolue path of "InputData" directory
            string prefix_for_inputdata = "/../../../../../InputData/";
            DoExperiments(prefix_for_inputdata);
        }

        public static void DoExperiments(string path)
        {
            Console.WriteLine("Begin to load input data.");
            var customer_file_path = path + @"/customer.txt";
            var order_file_path = path + @"/order.txt";
            var supplier_file_path = path + @"/supplier.txt";
            var lineitem_file_path = path + @"/lineitem.txt";
            var ps_file_path = path + @"/ps.txt";
            var r1_file_path = path + @"/R1.txt";
            var r2_file_path = path + @"/R2.txt";
            var r3_file_path = path + @"/R3.txt";
            var r4_file_path = path + @"/R4.txt";
            var r5_file_path = path + @"/R5.txt";
            var r6_file_path = path + @"/R6.txt";
            var customer_file = System.IO.File.ReadAllLines(customer_file_path);
            var customer = customer_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var supplier_file = System.IO.File.ReadAllLines(supplier_file_path);
            var supplier = supplier_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var order_file = System.IO.File.ReadAllLines(order_file_path);
            var order = order_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var lineitem_file = System.IO.File.ReadAllLines(lineitem_file_path);
            var lineitem = lineitem_file.Select(x => x.Split(' ')).Select(x => new Triple(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]), Convert.ToInt32(x[2]))).ToArray();
            var ps_file = System.IO.File.ReadAllLines(ps_file_path);
            var ps = ps_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var r1_file = System.IO.File.ReadAllLines(r1_file_path);
            var r1 = r1_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var r2_file = System.IO.File.ReadAllLines(r2_file_path);
            var r2 = r2_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var r3_file = System.IO.File.ReadAllLines(r3_file_path);
            var r3 = r3_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var r4_file = System.IO.File.ReadAllLines(r4_file_path);
            var r4 = r4_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var r5_file = System.IO.File.ReadAllLines(r5_file_path);
            var r5 = r5_file.Select(x => x.Split(' ')).Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToArray();
            var r6_file = System.IO.File.ReadAllLines(r6_file_path);
            var r6 = r6_file.Select(x => x.Split(' ')).Select(x => new Triple(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]), Convert.ToInt32(x[2]))).ToArray();
            Console.WriteLine("Finish loading input data.");
            Console.WriteLine("Begin to add weights for data.");
            var w_c = PINQCollection<Pair<int>>.Input(4);
            w_c.OnNext(customer.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_s = PINQCollection<Pair<int>>.Input(4);
            w_s.OnNext(supplier.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_o = PINQCollection<Pair<int>>.Input(4);
            w_o.OnNext(order.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_ps = PINQCollection<Pair<int>>.Input(4);
            w_ps.OnNext(ps.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_l = PINQCollection<Triple>.Input(4);
            w_l.OnNext(lineitem.Select(x => new Weighted<Triple>(x, +1.0)));
            var w_r1 = PINQCollection<Pair<int>>.Input(4);
            w_r1.OnNext(r1.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_r2 = PINQCollection<Pair<int>>.Input(4);
            w_r2.OnNext(r2.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_r3 = PINQCollection<Pair<int>>.Input(4);
            w_r3.OnNext(r3.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_r4 = PINQCollection<Pair<int>>.Input(4);
            w_r4.OnNext(r4.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_r5 = PINQCollection<Pair<int>>.Input(4);
            w_r5.OnNext(r5.Select(x => new Weighted<Pair<int>>(x, +1.0)));
            var w_r6 = PINQCollection<Triple>.Input(4);
            w_r6.OnNext(r6.Select(x => new Weighted<Triple>(x, +1.0)));
            Console.WriteLine("Finish adding weights to data.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double total = 0;
            int repeat_time = 10;
            Console.WriteLine("For query 1: ");
            for (int i = 0; i< repeat_time; i++)
            {
                var w_c_o = w_c.Join(w_o, x => x.a, y => y.b, (x, y) => new Triple(x.b, x.a, y.a));
                var w_s_l = w_l.Join(w_s, x => x.c, y => y.a, (x, y) => new Triple(x.a, x.b, x.c));
                var w_c_o_s_l = w_c_o.Join(w_s_l, x => x.c, y => y.a, (x, y) => true);
                var q1_result = w_c_o_s_l.Count(x => true, 100000)[true];
                total += q1_result;
            }
            sw.Stop();
            Console.WriteLine("Result is " + total/ repeat_time);
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("Time: "+ ts2.TotalMilliseconds/repeat_time/ 1000+"s");

            sw.Start();
            total = 0;
            Console.WriteLine("For query 2: ");
            for (int i = 0; i < repeat_time; i++)
            {
                
                var w_s_ps = w_s.Join(w_ps, x => x.a, y => y.b, (x, y) => new Triple(x.b, x.a, y.a));
                var w_o_l = w_o.Join(w_l, x => x.a, y => y.a, (x, y) => new Triple(y.c, y.b, y.a));
                var w_s_ps_o_l = w_s_ps.Join(w_o_l, x => new Pair<int>(x.b, x.c), y => new Pair<int>(y.a, y.b), (x, y) => true);
                var q2_result = w_s_ps_o_l.Count(x => true, 100000)[true];
                total += q2_result;
            }
            sw.Stop();
            Console.WriteLine("Result is " + total / repeat_time);
            ts2 = sw.Elapsed;
            Console.WriteLine("Time: "+ ts2.TotalMilliseconds / repeat_time / 1000 + "s");

            sw.Start();
            total = 0;
            Console.WriteLine("For query 3: ");
            for (int i = 0; i < repeat_time; i++)
            {
                var w_c_o = w_c.Join(w_o, x => x.a, y => y.b, (x, y) => new Triple(x.b, x.a, y.a));
                var w_s_l = w_s.Join(w_l, x => x.a, y => y.c, (x, y) => new Triple(x.b, x.a, y.a));
                var w_c_o_s_l = w_c_o.Join(w_s_l, x => new Pair<int>(x.a, x.c), y => new Pair<int>(y.a, y.c), (x, y) => true);
                var q3_result = w_c_o_s_l.Count(x => true, 100000)[true];
                total += q3_result;
            }
            sw.Stop();
            Console.WriteLine("Result is " + total / repeat_time);
            ts2 = sw.Elapsed;
            Console.WriteLine("Time: "+ts2.TotalMilliseconds/ repeat_time / 1000 + "s");

            sw.Start();
            total = 0;
            Console.WriteLine("For query 4: ");
            for (int i = 0; i < repeat_time; i++)
            {
                var w_r12 = w_r1.Join(w_r2, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r123 = w_r12.Join(w_r3, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r1234 = w_r123.Join(w_r4, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r12345 = w_r1234.Join(w_r5, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var q4_result = w_r12345.Count(x => true, 100000)[true];
                total += q4_result;
            }
            sw.Stop();
            Console.WriteLine("Result is " + total / repeat_time);
            ts2 = sw.Elapsed;
            Console.WriteLine("Time: "+ts2.TotalMilliseconds / repeat_time / 1000 + "s");

            sw.Start();
            total = 0;
            Console.WriteLine("For query 5: ");
            for (int i = 0; i < repeat_time; i++)
            {
                var w_r12 = w_r1.Join(w_r2, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r123 = w_r12.Join(w_r3, x => new Pair<int>(x.a, x.b), y => new Pair<int>(y.b, y.a), (x, y) => true);
                var q5_result = w_r123.Count(x => true, 100000)[true];
                total += q5_result;
            }
            sw.Stop();
            Console.WriteLine("Result is " + total / repeat_time);
            ts2 = sw.Elapsed;
            Console.WriteLine("Time: "+ ts2.TotalMilliseconds / repeat_time / 1000 + "s");

            sw.Start();
            total = 0;
            Console.WriteLine("For query 6: ");
            for (int i = 0; i < repeat_time; i++)
            {
                var w_r12 = w_r1.Join(w_r2, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r123 = w_r12.Join(w_r3, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r1234 = w_r123.Join(w_r4, x => new Pair<int>(x.a, x.b), y => new Pair<int>(y.b, y.a), (x, y) => true);
                var q6_result = w_r1234.Count(x => true, 100000)[true];
                total += q6_result;
            }
            sw.Stop();
            Console.WriteLine("Result is " + total / repeat_time);
            ts2 = sw.Elapsed;
            Console.WriteLine("Time: "+ ts2.TotalMilliseconds / repeat_time / 1000 + "s");

            sw.Start();
            total = 0;
            Console.WriteLine("For query 7: ");
            for (int i = 0; i < repeat_time; i++)
            {
                var w_r12 = w_r1.Join(w_r2, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r123 = w_r12.Join(w_r3, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r1234 = w_r123.Join(w_r4, x => x.b, y => y.a, (x, y) => new Pair<int>(x.a, y.b));
                var w_r12345 = w_r1234.Join(w_r5, x => new Pair<int>(x.a, x.b), y => new Pair<int>(y.b, y.a), (x, y) => true);
                var q7_result = w_r12345.Count(x => true, 100000)[true];
                total += q7_result;
            }
            sw.Stop();
            Console.WriteLine("Result is " + total / repeat_time);
            ts2 = sw.Elapsed;
            Console.WriteLine("Time: "+ ts2.TotalMilliseconds / repeat_time / 1000 + "s");

            sw.Start();
            total = 0;
            Console.WriteLine("For query 8: ");
            for (int i = 0; i < repeat_time; i++)
            {
                var w_r34 = w_r3.Join(w_r4, x => x.b, y => y.a, (x, y) => new Triple(x.a,x.b,y.b));
                var w_r345 = w_r34.Join(w_r5, x => new Pair(x.a,x.c), y => new Pair(y.b,y.a), (x, y) => new Triple(x.a, x.b, x.c));
                var w_r3456 = w_r345.Join(w_r6, x => x, y => y, (x,y)=>true);
                var q8_result = w_r3456.Count(x => true, 100000)[true];
                total += q8_result;
            }
            sw.Stop();
            Console.WriteLine("Result is " + total / repeat_time);
            ts2 = sw.Elapsed;
            Console.WriteLine("Time: "+ ts2.TotalMilliseconds / repeat_time / 1000 + "s");
        }

        public static void TestMCMC(Pair<int>[] edges, int parallelism, string filename)
        {
            double epsilon = 0.1;
            var graph = PINQCollection<Pair<int>>.Input(parallelism);

            // introduce the sensitive graph data
            graph.OnNext(edges.Select(x => new Weighted<Pair<int>>(x, +1.0)));

            #region take preliminary node and edge multiplicity measurements

            // count number of nodes
            var doubleNodes = 0.5 + graph.GroupBy(e => e.a, i => i, (i, l) => l.Count)
                                         .Concat(graph.Select(e => e.b))
                                         .Shave(1.0, (i, x) => i)
                                         .Where(x => x == 0)
                                         .Count(y => y, epsilon / 2)[0] * 2;
            var numNodes = (int)doubleNodes;
            Console.WriteLine("num of nodes: " + numNodes + " " + doubleNodes);
            //count multiedges and selfloops
            var multiplicity = graph.Shave(1.0, (i, x) => new Pair<int> { a = i, b = x.a == x.b ? 1 : 0 })
                                    .Count(y => y, epsilon);

            #endregion 

            #region warm start measurements (in and out degree distributions and ccdf)

            var synth = graph.WarmStart(numNodes, numNodes, epsilon).ToArray();

            //foreach (var edge in synth) Console.WriteLine("Synth: {0}", edge);
            //synth.PrintDegreeDistribution("synth");

            //print the graph after warm start
            //System.IO.File.WriteAllLines(filename + "-synthGraph.txt", synth.Select(x => x.a + "\t" + x.b));

            #endregion

            #region define a graph analysis

            graph.TrianglesByDegree(0.1);

            //Clustering(graph, 0.1);
            //Console.WriteLine("measuring tris");
            //graph.jddTriangles(Enumerable.Range(10, 10).Select(x => new Pair<int>(10 * x, 10 * x)).ToArray(), 0.1);
            //Console.WriteLine("measured tris");

            #endregion

            // revealing this directly violates differential privacy; for measurement only.
            Console.WriteLine("Error on real data: {0}", Observation.TotalError);

            // remove sensitive graph data and introduce synthetic graph data.
            graph.OnNext(edges.Select(x => new Weighted<Pair<int>>(x, -1.0)));
            graph.OnNext(synth.Select(x => new Weighted<Pair<int>>(x, +1.0)));

            // perform mcmc steps.
            var mcmcIterations = 1000000;
            var mcmcd = graph.ComputeWithSwaps(synth, numNodes, mcmcIterations).ToArray();

            //write the synthetic graph 
            //System.IO.File.WriteAllLines(filename + "-newresultingGraph.txt", mcmcd.Select(x => x.a + "\t" + x.b));

            // remove the mcmc graph data and re-introduce sensitive graph data.
            graph.OnNext(mcmcd.Select(x => new Weighted<Pair<int>>(x, -1.0)));
            graph.OnNext(edges.Select(x => new Weighted<Pair<int>>(x, +1.0)));

            // revealing this directly violates differential privace; for measurement only.
            Console.WriteLine("Final error {0}", Observation.TotalError);
        }


        public static void PrintDegreeDistribution(this IEnumerable<Pair<int>> edges, string prefix)
        {
            foreach (var entry in edges.GroupBy(x => x.a, (k, l) => l.Count()).OrderByDescending(x => x))
                Console.WriteLine(prefix + " o " + entry);
            foreach (var entry in edges.GroupBy(x => x.b, (k, l) => l.Count()).OrderByDescending(x => x))
                Console.WriteLine(prefix + " i " + entry);
        }

    }


    #region degree sequence fitter (based on shortest paths)

    /// <summary>
    /// Implements a priority queue
    /// </summary>
    public class PriorityQueue<T>
    {
        public struct PriorityElement
        {
            public T element;
            public double priority;

            public override string ToString() { return "" + element + "\t" + priority; }

            public PriorityElement(T e, double p) { element = e; priority = p; }
        }

        public PriorityElement[] heap;
        public int count;

        public int Parent(int i) { return (i - 1) / 2; }
        public int LChild(int i) { return (2 * i) + 2; }
        public int RChild(int i) { return (2 * i) + 1; }

        public PriorityElement this[int index]
        {
            get
            {
                PriorityElement deflt = new PriorityElement();
                deflt.priority = double.MaxValue;

                if (index < count)
                    return heap[index];
                else
                    return deflt;
            }
        }

        public void Add(T element, double priority)
        {
            if (heap.Length == count)
            {
                var newHeap = new PriorityElement[count * 2];
                heap.CopyTo(newHeap, 0);
                heap = newHeap;
            }

            heap[count++] = new PriorityElement(element, priority);

            var index = count - 1;
            while (heap[index].priority < heap[Parent(index)].priority)
            {
                var temp = heap[index];
                heap[index] = heap[Parent(index)];
                heap[Parent(index)] = temp;

                index = Parent(index);
            }
        }

        public PriorityElement Min()
        {
            var result = heap[0];
            heap[0] = heap[count - 1];

            var index = 0;
            while ((this[LChild(index)].priority < heap[index].priority) ||
                    (this[RChild(index)].priority < heap[index].priority))
            {
                var temp = heap[index];
                var next = (this[LChild(index)].priority < this[RChild(index)].priority) ? LChild(index) : RChild(index);

                heap[index] = heap[next];
                heap[next] = temp;

                index = next;
            }

            count--;

            return result;
        }

        public PriorityQueue()
        {
            heap = new PriorityElement[1];
            count = 0;
        }

    }

    public struct Position : IEquatable<Position>
    {
        public int x;
        public int y;

        public Position(int xx, int yy) { x = xx; y = yy; }

        public override int GetHashCode()
        {
            return x + (17 * y);
        }

        public bool Equals(Position other)
        {
            return other.x == x && other.y == y;
        }

        public override string ToString()
        {
            return String.Format("[{0},{1}]", x, y);
        }
    }

    public static class DegreeSequenceFitter
    {
        public static double HCost(Position position, double[] h)
        {
            var index = position.y - 1;
            if (0 <= index && index < h.Length)
                return Math.Abs(h[index] - position.x);
            else
                return 0.0;
        }

        public static double VCost(Position position, double[] v)
        {
            return v.Length > position.x ? Math.Abs(v[position.x] - position.y) : 0.0;
        }

        public static IEnumerable<int> FitDegSeq(double[] h, double[] v)
        {
            // Console.WriteLine("\nInside FitDegSeq\n");

            var start = new System.Diagnostics.Stopwatch();
            start.Start();

            var queue = new PriorityQueue<Position>();		// dijkstra's priority queue
            var costs = new Dictionary<Position, double>();	// records the paths to take

            #region Establishing bounds

            var hmax = h.ToArray();
            var hmin = h.ToArray();

            var vmax = v.ToArray();
            var vmin = v.ToArray();

            for (int i = 1; i < hmin.Length; i++) hmin[i] = Math.Min(hmin[i - 1], h[i]);
            for (int i = 1; i < vmin.Length; i++) vmin[i] = Math.Min(vmin[i - 1], v[i]);

            for (int i = hmin.Length - 2; i >= 0; i--) hmax[i] = Math.Max(hmax[i + 1], h[i]);
            for (int i = vmin.Length - 2; i >= 0; i--) vmax[i] = Math.Max(vmax[i + 1], v[i]);


            var vlim = vmax.Length > 0 ? Convert.ToInt32(Math.Max(Math.Ceiling(vmax[0]), h.Length)) : h.Length;
            var hlim = hmax.Length > 0 ? Convert.ToInt32(Math.Max(Math.Ceiling(hmax[0]), v.Length)) : v.Length;

            hmax = new double[] { hlim }.Concat(hmax).Select(x => Math.Ceiling(x)).ToArray();
            vmax = new double[] { vlim }.Concat(vmax).Select(x => Math.Ceiling(x)).ToArray();

            hmin = hmin.Concat(new double[] { 0.0 }).Select(x => Math.Floor(x)).ToArray();
            vmin = vmin.Concat(new double[] { 0.0 }).Select(x => Math.Floor(x)).ToArray();

            #endregion

            queue.Add(new Position(0, vlim), 0.0);					// we start here at zero cost

            for (var element = queue.Min(); !(element.element.x == hlim && element.element.y == 0); element = queue.Min())
            {
                // if we have not seen this element yet
                if (!costs.ContainsKey(element.element))
                {
                    var distance = element.priority;
                    var position = element.element;

                    costs.Add(position, distance);

                    // only process elements in the plausible region
                    if ((0 <= position.y && position.y < hmin.Length && hmin[position.y] <= position.x && position.x <= hmax[position.y]) ||
                        (0 <= position.x && position.x < vmin.Length && vmin[position.x] <= position.y && position.y <= vmax[position.x]))
                    {
                        queue.Add(new Position(position.x, position.y - 1), distance + HCost(position, h));
                        queue.Add(new Position(position.x + 1, position.y), distance + VCost(position, v));
                    }
                }
            }

            // now to walk back from (n, 0) finding the right path
            var result = new int[hlim];
            var current = new Position(hlim, 0);
            while (current.x > 0)
            {
                var hstep = new Position(current.x - 1, current.y);
                var vstep = new Position(current.x, current.y + 1);

                var hcost = costs.ContainsKey(hstep) ? costs[hstep] : double.MaxValue;
                var vcost = costs.ContainsKey(vstep) ? costs[vstep] : double.MaxValue;

                if (hcost + VCost(hstep, v) < vcost + HCost(vstep, h))
                {
                    result[hstep.x] = hstep.y;
                    current.x = hstep.x;
                }
                else
                    current.y = vstep.y;
            }

            Console.WriteLine("fitted degree sequence in " + start.Elapsed);
            return result;
        }

        public static T[] Permute<T>(this IEnumerable<T> input, Random random)
        {
            var result = input.ToArray();

            var orders = new int[result.Length];
            for (int i = 0; i < orders.Length; i++)
                orders[i] = random.Next();

            Array.Sort(orders, result);

            return result;

            //return input.Select(x => new { dst = x, r = random.Next() })
            //            .OrderBy(x => x.r)  //permute
            //            .Select(x => x.dst)
            //            .ToArray();
        }

        public static IEnumerable<Pair<int>> GenerateGraph(IEnumerable<int> odegrees, IEnumerable<int> idegrees, int nodes)
        {
            var odegs = odegrees.OrderByDescending(x => x).Take(nodes).ToArray();
            var idegs = idegrees.OrderByDescending(x => x).Take(nodes).ToArray();
            var numedges = Math.Min(odegs.Sum(), idegs.Sum());

            Console.WriteLine("Generating graph with {0} edges.", numedges);

            var onodes = odegs.Where(x => x > 0).Count();
            var inodes = idegs.Where(x => x > 0).Count();
            //nodes = Math.Max(odegs.Length, idegs.Length);



            var random = new Random(0);

            var oedges = odegs.SelectMany((x, i) => Enumerable.Repeat(i, x))
                              .Permute(random);

            var doFirst = Enumerable.Range(onodes, nodes - onodes)
                                    .Permute(random);

            var doSecond = Enumerable.Range(0, onodes)
                                     .Permute(random);

            var iNames = doFirst.Concat(doSecond).ToArray();

            var iedges = idegs.Take(inodes)
                              .Permute(random)
                              .SelectMany((x, i) => Enumerable.Repeat(iNames[i], x))
                              .Permute(random);

            for (int i = 0; i < numedges; i++)
                yield return new Pair<int>(oedges[i], iedges[i]);
        }
    }

    #endregion

    public static class Measurements
    {
        public static Count<int> OutDegCCDF<T>(this PINQCollection<Pair<T>> edges, double epsilon) where T : IEquatable<T>
        {
            return edges.Select(x => x.a)
                        .Transpose()
                        .Count(i => i, epsilon);
        }
        public static Count<int> InDegCCDF<T>(this PINQCollection<Pair<T>> edges, double epsilon) where T : IEquatable<T>
        {
            return edges.Select(x => x.b)
                        .Transpose()
                        .Count(i => i, epsilon);
        }
        public static Count<int> OutDegSeq<T>(this PINQCollection<Pair<T>> edges, double epsilon) where T : IEquatable<T>
        {
            return edges.Select(x => x.a)
                        .Transpose()
                        .Transpose()
                        .Count(i => i, epsilon);
        }
        public static Count<int> InDegSeq<T>(this PINQCollection<Pair<T>> edges, double epsilon) where T : IEquatable<T>
        {
            return edges.Select(x => x.b)
                        .Transpose()
                        .Transpose()
                        .Count(i => i, epsilon);
        }

        public static PINQCollection<int> Transpose<T>(this PINQCollection<T> input) where T : IEquatable<T>
        {
            return input.Shave(1.0, (i, t) => i);
        }
    }

    #region MCMC and warmstart

    public static class MCMC
    {
        public static bool KeepGoing = true;

        public static IEnumerable<Pair<int>> WarmStart(this PINQCollection<Pair<int>> graph, int maxdeg, int numnodes, double eps)
        {
            maxdeg = Math.Max(1, maxdeg);
            numnodes = Math.Max(1, numnodes);

            Console.WriteLine("Doing WarmStart with maxdeg {0} and numnodes {1}", maxdeg, numnodes);

            var odegrees = graph.OutDegCCDF(eps);   // measures ccdf of outgoing degrees
            var idegrees = graph.InDegCCDF(eps);    // measures ccdf of incoming degrees
            var idegrseq = graph.InDegSeq(eps);     // measures  cdf of outgoing degrees
            var odegrseq = graph.OutDegSeq(eps);    // measures  cdf of incoming degrees

            var ods = Enumerable.Range(0, maxdeg).Select(i => odegrees[i]).ToArray();
            var ids = Enumerable.Range(0, maxdeg).Select(i => idegrees[i]).ToArray();
            var odq = Enumerable.Range(0, numnodes).Select(i => odegrseq[i]).ToArray();
            var idq = Enumerable.Range(0, numnodes).Select(i => idegrseq[i]).ToArray();

            var idfitted = DegreeSequenceFitter.FitDegSeq(ids, idq);
            var odfitted = DegreeSequenceFitter.FitDegSeq(ods, odq);

            //System.IO.File.WriteAllLines("odegrees.txt", ods.Select((x,i) => i + "\t" + x));
            //System.IO.File.WriteAllLines("odegrseq.txt", odq.Select((x, i) => i + "\t" + x));
            //System.IO.File.WriteAllLines("odfitted.txt", odfitted.Select((x, i) => i + "\t" + x));

            var synthGraph = DegreeSequenceFitter.GenerateGraph(odfitted, idfitted, numnodes);

            return synthGraph;
        }

        public static IEnumerable<Pair<int>> ComputeWithSwaps(this IObserver<IEnumerable<Weighted<Pair<int>>>> graph, Pair<int>[] edges, int nodes, int steps)
        {
            if (edges.Length == 0)
                return edges;

            var random = new Random(0);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var starttime = stopwatch.ElapsedMilliseconds;

            var delta = new Weighted<Pair<int>>[8]; // stores change we want to make, and possible undoing of last change we made.

            // tracks the current error levels, compared with newError.
            var curError = Observation.TotalError;

            Console.WriteLine("Starting MCMC with error {0:0.00}", curError);

            for (int step = 0; step < steps; step++)
            {
                // introduce a new edge, chosen totally randomly.
                var index1 = random.Next(edges.Length);
                var index2 = random.Next(edges.Length);

                delta[0] = new Weighted<Pair<int>>(new Pair<int>(edges[index1].a, edges[index2].b), 1.0);
                delta[1] = new Weighted<Pair<int>>(new Pair<int>(edges[index2].a, edges[index1].b), 1.0);
                delta[2] = new Weighted<Pair<int>>(edges[index1], -1.0);
                delta[3] = new Weighted<Pair<int>>(edges[index2], -1.0);

                graph.OnNext(delta);

                var newError = Observation.TotalError;

                if (stopwatch.ElapsedMilliseconds - starttime > 1000)
                {
                    starttime = stopwatch.ElapsedMilliseconds;
                    System.GC.Collect(2);
                    Console.WriteLine("error @ {0}:\t{1:0.00} v {2:0.00}", step, newError, curError);
                }

                // high = greedy, low = random.
                var focus = 10000.0;

                // randomly revert if not an improvement in probability
                if (random.NextDouble() > Math.Exp(focus * (curError - newError)))
                {
                    // prep the next change to include a reversion.
                    delta[4] = new Weighted<Pair<int>>(delta[0].record, -1.0);
                    delta[5] = new Weighted<Pair<int>>(delta[1].record, -1.0);
                    delta[6] = new Weighted<Pair<int>>(delta[2].record, 1.0);
                    delta[7] = new Weighted<Pair<int>>(delta[3].record, 1.0);
                }
                else
                {
                    // update our view of error.
                    curError = newError;

                    // update our graph appropriately.
                    edges[index1] = delta[0].record;
                    edges[index2] = delta[1].record;

                    // don't push any new deltas.
                    delta[4].weight = 0;
                    delta[5].weight = 0;
                    delta[6].weight = 0;
                    delta[7].weight = 0;
                }
            }

            delta[0] = new Weighted<Pair<int>>();
            delta[1] = new Weighted<Pair<int>>();
            delta[2] = new Weighted<Pair<int>>();
            delta[3] = new Weighted<Pair<int>>();

            graph.OnNext(delta);

            Console.WriteLine("We are done!");

            return edges;
        }
    }

    #endregion

    #region equatable data types for use in wpinq queries

    public struct Pair : IEquatable<Pair>
    {
        public int a, b;

        public bool Equals(Pair that) { return (a == that.a && b == that.b); }

        public override int GetHashCode()
        {
            return a.GetHashCode() + 24 * b.GetHashCode();
        }

        public Pair(int aa, int bb) { a = aa; b = bb; }
    }
    public struct Pair<T> : IEquatable<Pair<T>> where T : IEquatable<T>
    {
        public T a;
        public T b;

        public bool Equals(Pair<T> that) { return this.a.Equals(that.a) && this.b.Equals(that.b); }
        public override int GetHashCode()
        {
            return 12341211 * a.GetHashCode() + 24 * b.GetHashCode();
        }

        public Pair(T aa, T bb) { a = aa; b = bb; }
        public override string ToString()
        {
            return string.Format("Pair({0},{1})", a, b);
        }
    }
    public struct Pair<S, T> : IEquatable<Pair<S, T>>
        where S : IEquatable<S>
        where T : IEquatable<T>
    {
        public S a;
        public T b;

        public bool Equals(Pair<S, T> that) { return this.a.Equals(that.a) && this.b.Equals(that.b); }
        public override int GetHashCode()
        {
            return 12341211 * a.GetHashCode() + 132412341 * b.GetHashCode();
        }

        public Pair(S aa, T bb) { a = aa; b = bb; }
        public override string ToString()
        {
            return string.Format("Pair({0},{1})", a, b);
        }
    }

    public struct Triple : IEquatable<Triple>
    {
        public int a, b, c;

        public bool Equals(Triple that) { return (a == that.a && b == that.b && c == that.c); }

        public override int GetHashCode()
        {
            return 134987 * a.GetHashCode() + 24 * b.GetHashCode() + 13413 * c.GetHashCode();
        }

        public Triple(int aa, int bb, int cc) { a = aa; b = bb; c = cc; }

        public override string ToString()
        {
            return string.Format("Triple({0},{1},{2})", a, b, c);
        }
    }
    public struct Triple<R, S, T> : IEquatable<Triple<R, S, T>>
        where R : IEquatable<R>
        where S : IEquatable<S>
        where T : IEquatable<T>
    {
        public R a;
        public S b;
        public T c;

        public bool Equals(Triple<R, S, T> that) { return (a.Equals(that.a) && b.Equals(that.b) && c.Equals(that.c)); }

        public override int GetHashCode()
        {
            return 134987 * a.GetHashCode() + 24 * b.GetHashCode() + 13413 * c.GetHashCode();
        }

        public Triple(R aa, S bb, T cc) { a = aa; b = bb; c = cc; }

        public override string ToString()
        {
            return string.Format("Triple({0},{1},{2})", a, b, c);
        }
    }

    public struct VertexData : IEquatable<VertexData>
    {
        public int name;
        public int[] edges;

        public bool Equals(VertexData that)
        {
            if (this.name != that.name)
                return false;

            if (this.edges.Length != that.edges.Length)
                return false;

            for (int i = 0; i < this.edges.Length; i++)
                if (this.edges[i] != that.edges[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            var result = name.GetHashCode();
            for (int i = 0; i < edges.Length; i++)
                result += edges[i].GetHashCode();

            return result;
        }

        public VertexData(int name, ResizeableSubArray<int> edges)
        {
            this.name = name;
            this.edges = new int[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                this.edges[i] = edges.Array[i];

            Array.Sort(this.edges);
        }
    }

    #endregion

    public static class Statics
    {
        public static void PrintGraph(IEnumerable<Weighted<Pair<int>>> graph, string filename)
        {
            var output = new System.IO.StreamWriter(filename);
            foreach (var record in graph)
                output.WriteLine(record);

            output.Close();
        }

        public static IEnumerable<Pair<int>> ReadGraph(string filename)
        {
            var file = System.IO.File.ReadAllLines(filename);

            return file.Select(x => x.Split(' '))
                        .Select(x => new Pair<int>(Convert.ToInt32(x[0]), Convert.ToInt32(x[1])));
        }

        public static Pair<int>[] GenerateGraph(this Random random, int nodes, int edges)
        {
            var results = new Pair<int>[edges];
            for (int i = 0; i < results.Length; i++)
                results[i] = new Pair<int>(random.Next(nodes), random.Next(nodes));

            return results;
        }
    }

    public static class GraphAnalyses
    {
        public static PINQCollection<Triple> jddTriangles(this PINQCollection<Pair<int>> graph, Pair<int>[] buckets, double epsilon)
        {
            bool symmetry = false;
            if (symmetry)
            {
                var symmGraph = graph.Select(edge => new Pair<int>(edge.b, edge.a));
                var undirectedGraph = graph.Concat(symmGraph);
                graph = undirectedGraph;
                //I need to double the buckets (maybe is not correct to double)
                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i].a = buckets[i].a * 2;
                    buckets[i].b = buckets[i].b * 2;
                }
            }
            // does a DPCount, but then immediately replaces the degree with the bucket.
            //var aBuckets = graph.DPCount(x => x.a, (k, i) => new Pair<int>(k, buckets.Where(bucket => i < bucket.a).First().a));
            //var bBuckets = graph.DPCount(x => x.b, (k, i) => new Pair<int>(k, buckets.Where(bucket => i < bucket.b).First().b));

            //var aDegEdge = graph.Join(aBuckets, edge => edge.a, pair => pair.a, (edge, pair) => new Pair<Pair<int>, int>(edge, pair.b));
            //var bDegEdge = graph.Join(bBuckets, edge => edge.b, pair => pair.a, (edge, pair) => new Pair<Pair<int>, int>(edge, pair.b));


            var aDegrees = graph.DPCount(x => x.a, (k, i) => new Pair<int>(k, i));
            var bDegrees = graph.DPCount(x => x.b, (k, i) => new Pair<int>(k, i));

            var aDegEdge = graph.Join(aDegrees, edge => edge.a, pair => pair.a, (edge, pair) => new Pair<Pair<int>, int>(edge, pair.b));
            var bDegEdge = graph.Join(bDegrees, edge => edge.b, pair => pair.a, (edge, pair) => new Pair<Pair<int>, int>(edge, pair.b));

            var edgeDegree = aDegEdge.Join(bDegEdge, aTriple => aTriple.a, bTriple => bTriple.a, (aTriple, bTriple)
                              => new Triple<Pair<int>, int, int>(aTriple.a, aTriple.b, bTriple.b));


            //create a a path of length 3 with the degrees of each node involved
            var abc = edgeDegree.Join(edgeDegree, x => x.a.b, y => y.a.a, (x, y) => new Triple(x.b, x.c, y.c));
            return abc;
        }
        
        public static void Triangles(this PINQCollection<Pair<int>> graph, Pair<int>[] buckets, double epsilon)
        {
            var symmGraph = graph.Select(edge => new Pair<int>(edge.b, edge.a));
            var undirectedGraph = graph.Concat(symmGraph);

            var abc = undirectedGraph.Join(undirectedGraph, x => x.b, y => y.a, (x, y) => new Triple(x.a, x.b, y.b));
            var bca = abc.Select(x => new Triple(x.b, x.c, x.a));
            var cab = abc.Select(x => new Triple(x.c, x.a, x.b));

            var result = abc.Intersect(bca).Intersect(cab);

            Console.WriteLine("triangles: {0}", result.Count(x => true, epsilon)[true]);

        }

        public static PINQCollection<Pair<int>> JointDegrees(this PINQCollection<Pair<int>> graph)
        {
            var aDegrees = graph.DPCount(x => x.a, (k, i) => new Pair<int>(k, i - 1));
            var bDegrees = graph.DPCount(x => x.b, (k, i) => new Pair<int>(k, i - 1));

            var aDegEdge = graph.Join(aDegrees, edge => edge.a, pair => pair.a, (edge, pair) => new Pair<Pair<int>, int>(edge, pair.b));
            var bDegEdge = graph.Join(bDegrees, edge => edge.b, pair => pair.a, (edge, pair) => new Pair<Pair<int>, int>(edge, pair.b));

            return aDegEdge.Join(bDegEdge, aTriple => aTriple.a, bTriple => bTriple.a, (aTriple, bTriple) => new Pair<int>(aTriple.b, bTriple.b));
        }

        public static Count<Triple> Triangles(this PINQCollection<Pair<int>> graph, double epsilon)
        {
            var degrees = graph.DPCount(edge => edge.a, (k, i) => new Pair<int>(k, i - 1));

            var len2paths = graph.Join(graph, edge1 => edge1.b, edge2 => edge2.a, (edge1, edge2) => new Triple(edge1.a, edge1.b, edge2.b));

            var abcDb = len2paths.Join(degrees, path => path.b, degr => degr.a, (path, degr) => new Pair<Triple, int>(path, degr.b));
            var bcaDc = abcDb.Select(pair => new Pair<Triple, int>(new Triple(pair.a.b, pair.a.c, pair.a.a), pair.b));
            var cabDa = abcDb.Select(pair => new Pair<Triple, int>(new Triple(pair.a.c, pair.a.a, pair.a.b), pair.b));

            var results = abcDb.Join(bcaDc, abc => abc.a, bca => bca.a, (abc, bca) => new Pair<Triple, Pair<int>>(abc.a, new Pair<int>(abc.b, bca.b)))
                               .Join(cabDa, abc => abc.a, cab => cab.a, (abc, cab) => new Triple(cab.b, abc.b.a, abc.b.b));

            return results.Count(x => x, epsilon);
        }

        public static void Clustering(this PINQCollection<Pair<int>> graph, double epsilon)
        {
            //order the graph before, it improves both number of triangles and clustering coefficient
            graph = graph.Select(edge => new Pair<int>(Math.Min(edge.a, edge.b), Math.Max(edge.a, edge.b)));

            var length2Path = graph.Join(graph, x => x.b, y => y.a, x => x.a, y => y.b,
                                            (k, x, y) => new Triple(x, k, y)).Where(node => node.a != node.c);

            var triangles = graph.Intersect(length2Path.Select(x => new Pair<int>(x.a, x.c)));

            Console.WriteLine("the number of triangles is: {0}", triangles.Count(x => true, epsilon)[true]);
        }

        public static int median(int x, int y, int z)
        {
            if ((x <= y && y <= z) || (z <= y && y <= x))
                return y;
            if ((y <= x && x <= z) || (z <= x && x <= y))
                return x;
            return z;
        }

        public static void TrianglesByDegree(this PINQCollection<Pair<int>> graph, double epsilon)
        {
            Console.WriteLine("in TrianglesByDegree...");

            // form (b, db) pairs each with weight 1/2
            var bDegs = graph.GroupBy(e => e.a, e => e.b, (k, i) => new VertexData(k, i));

            // form length 2 paths (a,b,c)  weight 1/2db.
            var path = graph.Join(graph, x => x.b, y => y.a, x => x.a, y => y.b,
                                 (key, x, y) => new Triple(x, key, y));//.Where(x => x.a != x.c);


            // form ((a,b,c), db) tuples, with weights 1/2db(1 + db).
            var abc = path.Join(bDegs, x => x.b, y => y.name, x => x, y => y.edges.Length,
                                 (key, x, y) => new Pair<Triple, int>(x, y));

            // rotate to get ((c,a,b),db) or equivalently ((a,b,c),dc)
            var cab = abc.Select(x => new Pair<Triple, int>(new Triple(x.a.c, x.a.a, x.a.b), x.b));


            // rotate to get ((b,c,a),db) or equivalently ((a,b,c),da)
            var bca = abc.Select(x => new Pair<Triple, int>(new Triple(x.a.b, x.a.c, x.a.a), x.b));


            // form length ((a,b,c),da,db) tuples with weight 1/2(da(1+da) + db(1 +db))
            var tuple = abc.Join(bca, x => x.a, y => y.a, x => x.b, y => y.b,
                                 (key, x, y) => new Pair<Triple, Pair>(key, new Pair(y, x)));

            // form length ((a,b,c),da,db,db) tuples with weight 1/2(da(1+da) + db(1+db) + dc(1+dc))
            var tuple2 = tuple.Join(cab, x => x.a, y => y.a, x => x.b, y => y.b,
                                 (key, x, y) => new Pair<Triple, Triple>(key, new Triple(x.a, x.b, y)));


            // transform to (da,db,dc) tuples where da < db < dc
            var tris = tuple2.Select(x => x.b);
            tris = tris.Select(x => new Triple(Math.Min(Math.Min(x.a, x.b), x.c), median(x.a, x.b, x.c), Math.Max(x.a, Math.Max(x.b, x.c))));

            // return the noisy histogram
            var result = tris.Count(x => x, epsilon);
        }


    }

}