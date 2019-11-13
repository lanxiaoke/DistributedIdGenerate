using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleApplication2
{
    class Program
    {
        private static ConcurrentDictionary<Int64, int> dic = new ConcurrentDictionary<Int64, int>();
        private static double tt = 0;

        static void Main(string[] args)
        {
            var d1 = DateTime.Now;
            for (var i = 0; i < 4096000; i++)
            {
                IdManager.Sington.GetNextId();
            }

            Console.WriteLine((DateTime.Now - d1).TotalMilliseconds);

            //Working();

            //Thread.Sleep(10000);

            //Console.WriteLine("总耗时:{0}", tt);

            var dd = dic;

            //DateTime dt = DateTime.Now;
            //var sb = new StringBuilder(20);

            //for (int i = 0; i < 300; i++)
            //{
            //    var id = IdManager.Sington.GetNextId();
            //    Console.Write(id + " = ");

            //    sb.Clear();
            //    IdManager.Sington.ShortId(id, sb);
            //    Console.Write(sb + " = ");

            //    var id2 = IdManager.Sington.LongId(sb.ToString(), sb.Length);
            //    Console.Write(id2 + Environment.NewLine);
            //}

            //Console.WriteLine((DateTime.Now - dt).TotalMilliseconds);
            Console.ReadKey();
        }
        private static void Working()
        {
            for (var i = 0; i < 50; i++)
            {
                new Task(Working, i).Start();
            }
        }
        private static void Working(object thid)
        {
            //IdBySnowFlake worker = new IdBySnowFlake((int)thid, 1);

            DateTime d1 = DateTime.Now;
            for (var i = 0; i < 100000; i++)
            {
                IdManager.Sington.GetNextId();
                //dic.TryAdd(IdManager.Sington.GetNextId(), 1);
            }

            tt += (DateTime.Now - d1).TotalMilliseconds;

            Console.WriteLine("{0}", thid);
        }
    }


    public class IdBySnowFlake
    {
        [DllImport("Infrastructure.DistributedIdGenerate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Worker(int workerId, int dataCenterId, int workerIdLen = 6, int dataCenterIdLen = 4);

        [DllImport("Infrastructure.DistributedIdGenerate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 NextId();

        [DllImport("Infrastructure.DistributedIdGenerate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int IntCompress(Int64 value, StringBuilder str64);

        [DllImport("Infrastructure.DistributedIdGenerate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 IntDecompression(string str64, int len);

        public IdBySnowFlake(int workerId, int dataCenterId)
        {
            Worker(1, 1);
        }

        public Int64 GetNextId()
        {
            return NextId();
        }

        public int ShortId(Int64 value, StringBuilder str64)
        {
            return IntCompress(value, str64);
        }
        public Int64 LongId(string str64, int len)
        {
            return IntDecompression(str64, len);
        }
    }

    public class IdManager
    {
        private const int workerId = 1;
        private const int dataCenterId = 1;

        private readonly static IdBySnowFlake idBySnowFlake = new IdBySnowFlake(workerId, dataCenterId);

        private IdManager() { }

        public static IdBySnowFlake Sington { get { return idBySnowFlake; } }
    }
}
