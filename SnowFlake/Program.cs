using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SnowFlake
{
    class Program
    {
        private static SnowFlake worker = new SnowFlake(1, 1);

        static void Main(string[] args)
        {
            //var d1 = DateTime.Now;
            //for (var i = 0; i < 4096000; i++)
            //{
            //    worker.NextId();
            //}

            //Console.WriteLine((DateTime.Now - d1).TotalMilliseconds);

            //Working();

            //Thread.Sleep(5000);

            //var count = MsgQueueManager.Ins.Count();

            //var ddd = "";

            ////5Ht1L54N100

            //var i0 = 6547639579355385856;
            //var s0 = IntToi64(i0);
            //Console.WriteLine(i0 + " = " + s0 + " = " + i64ToInt(s0));

            //var i1 = 6547639579355385857;
            //var s1 = IntToi64(i1);
            //Console.WriteLine(i1 + " = " + s1 + " = " + i64ToInt(s1));
            //Console.ReadKey();

            ////var i2 = 6547636351221895169;
            ////var s2 = IntToi64(i2);
            ////Console.WriteLine(i2 + " = " + s2 + " = " + i64ToInt(s2));

            ////var i3 = 6547636351226089472;
            ////var s3 = IntToi64(i3);
            ////Console.WriteLine(i3 + " = " + s3 + " = " + i64ToInt(s3));

            ////var i4 = 6547636351226089473;
            ////var s4 = IntToi64(i4);
            ////Console.WriteLine(i4 + " = " + s4 + " = " + i64ToInt(s4));

            ////var i5 = 6547636351226089474;
            ////var s5 = IntToi64(i5);
            ////Console.WriteLine(i5 + " = " + s5 + " = " + i64ToInt(s5));

            ////Console.ReadKey();

            //DateTime dt = DateTime.Now;

            //var worker = new IdBySnowFlake(1, 1);

            //for (int i = 0; i < 300; i++)
            //{
            //    //worker.NextId();
            //    //IntToi64(worker.NextId());
            //    //Console.WriteLine(worker.NextId());

            //    var i6 = worker.NextId();
            //    var s6 = IntToi64(i6);
            //    Console.WriteLine(i6 + " = " + s6 + " = " + i64ToInt(s6));

            //}

            //Console.WriteLine((DateTime.Now - dt).TotalMilliseconds);

            Working();

            Thread.Sleep(20000);

            Save();

            Console.ReadKey();
        }

        private static void Working()
        {
            for (var i = 0; i < 20; i++)
            {
                new Task(Working, i).Start();
            }
        }
        private static void Working(object thid)
        {
            var table = GetTableSchema();
            for (int i = 0; i < 10000; i++)
            {
                var row = table.NewRow();
                row[0] = worker.NextId();
                row[1] = 1;
                row[2] = string.Format("Title-{0}", i.ToString());
                row[3] = string.Format("线程:{0},信息:{1}", thid.ToString(), i.ToString());
                row[4] = "爱思考的猫";
                row[5] = "127.0.0.1";
                table.Rows.Add(row);
            }

            MsgQueueManager.Ins.En(table);
            Console.WriteLine("线程{0}入列{1}", thid.ToString(), 10000);
        }
        public static DataTable GetTableSchema()
        {
            var table = new DataTable();
            table.Columns.AddRange(new DataColumn[]{
                new DataColumn("Id",typeof(Int64)),
                new DataColumn("Level",typeof(int)),
                new DataColumn("Title",typeof(string)),
                new DataColumn("Content",typeof(string)),
                new DataColumn("User",typeof(string)),
                new DataColumn("Ip",typeof(string))
            });

            return table;
        }

        public static void Save()
        {
            var connStr = "data source=127.0.0.1;initial catalog=Demo;user id=sa;password=1234;MultipleActiveResultSets=True";
            var conn = new SqlConnection(connStr);

            var bulkCopy = new SqlBulkCopy(conn);
            bulkCopy.DestinationTableName = "Logs";

            conn.Open();
            var start = DateTime.Now;
            var total = 0;

            while (true)
            {
                DataTable tb;
                if (MsgQueueManager.Ins.De(out tb))
                {
                    total += tb.Rows.Count;
                    bulkCopy.BatchSize = tb.Rows.Count;
                    bulkCopy.WriteToServer(tb);
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine("Save完毕,总记录数:{0},耗时:{1}", total.ToString(), (DateTime.Now - start).TotalMilliseconds.ToString());
        }

        public static string IntToi64(long value)
        {
            var str64 = "";
            while (value >= 1)
            {
                var tmp = value & 0x3F;
                var index = Convert.ToInt16(tmp);

                str64 = Base64CodeEn[index] + str64;
                value = value >> 6;
            }

            return str64;
        }
        public static long i64ToInt(string str64)
        {
            long value = 0;
            var power = str64.Length - 1;

            for (var i = 0; i <= power; i++)
            {
                value += Base64CodeDe[str64[power - i]] * Convert.ToInt64(Math.Pow(64, i));

                Console.WriteLine(value);
            }

            return value;
        }

        public static Dictionary<int, char> Base64CodeEn = new Dictionary<int, char>() {
            {   0   ,'0'}, {   1   ,'1'}, {   2   ,'2'}, {   3   ,'3'}, {   4   ,'4'}, {   5   ,'5'}, {   6   ,'6'}, {   7   ,'7'}, {   8   ,'8'}, {   9   ,'9'},
            {   10  ,'a'}, {   11  ,'b'}, {   12  ,'c'}, {   13  ,'d'}, {   14  ,'e'}, {   15  ,'f'}, {   16  ,'g'}, {   17  ,'h'}, {   18  ,'i'}, {   19  ,'j'},
            {   20  ,'k'}, {   21  ,'l'}, {   22  ,'m'}, {   23  ,'n'}, {   24  ,'o'}, {   25  ,'p'}, {   26  ,'q'}, {   27  ,'r'}, {   28  ,'s'}, {   29  ,'t'},
            {   30  ,'u'}, {   31  ,'v'}, {   32  ,'w'}, {   33  ,'x'}, {   34  ,'y'}, {   35  ,'z'}, {   36  ,'A'}, {   37  ,'B'}, {   38  ,'C'}, {   39  ,'D'},
            {   40  ,'E'}, {   41  ,'F'}, {   42  ,'G'}, {   43  ,'H'}, {   44  ,'I'}, {   45  ,'J'}, {   46  ,'K'}, {   47  ,'L'}, {   48  ,'M'}, {   49  ,'N'},
            {   50  ,'O'}, {   51  ,'P'}, {   52  ,'Q'}, {   53  ,'R'}, {   54  ,'S'}, {   55  ,'T'}, {   56  ,'U'}, {   57  ,'V'}, {   58  ,'W'}, {   59  ,'X'},
            {   60  ,'Y'}, {   61  ,'Z'}, {   62  ,'-'}, {   63  ,'_'},
        };

        private static Dictionary<char, int> _Base64CodeDe = Enumerable.Range(0, Base64CodeEn.Count()).ToDictionary(i => Base64CodeEn[i], i => i);

        public static Dictionary<char, int> Base64CodeDe = _Base64CodeDe;
    }

    public class SnowFlake
    {
        // https://segmentfault.com/a/1190000011282426?utm_source=tag-newest

        private int workerId;
        private int dataCenterId;
        private int sequence;

        private int workerIdBits = 6;
        private int dataCenterIdBits = 4;
        private int maxWorkerId;
        private int maxDataCenterId;
        private int sequenceBits = 12;

        private int workerIdShift;
        private int dataCenterIdShift;
        private int timestampLeftShift;
        private int sequenceMask;

        private long lastTimestamp = -1L;
        //private static DateTime start = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static object syncRoot = new object();

        public SnowFlake(int workerId, int datacenterId)
        {
            maxWorkerId = -1 ^ (-1 << workerIdBits);
            maxDataCenterId = -1 ^ (-1 << dataCenterIdBits);
            sequenceBits = 12;

            workerIdShift = sequenceBits;
            dataCenterIdShift = sequenceBits + workerIdBits;
            timestampLeftShift = sequenceBits + workerIdBits + dataCenterIdBits;
            sequenceMask = -1 ^ (-1 << sequenceBits);

            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new Exception(string.Format("机器Id不能大于 {0} 或小于 0", maxWorkerId));
            }

            if (datacenterId > maxDataCenterId || datacenterId < 0)
            {
                throw new Exception(string.Format("数据中心不能大于 {0} 或小于 0", maxDataCenterId));
            }

            Console.WriteLine("服务启动. 时间戳左移:{0}b, 数据中心:{1}b, 机器Id:{2}b, 流水号:{3}b, 当前workerId:{4}",
                    timestampLeftShift, dataCenterIdBits, workerIdBits, sequenceBits, workerId);

            this.workerId = workerId;
            this.dataCenterId = datacenterId;
        }

        public long NextId()
        {
            lock (syncRoot)
            {
                var timestamp = timeGen();

                // 如果当前时间小于上一次ID生成的时间戳，说明系统时钟回退过这个时候应当抛出异常
                if (timestamp < lastTimestamp)
                {
                    Console.WriteLine("clock is moving backwards.  Rejecting requests until {0}.", lastTimestamp);
                    throw new Exception(string.Format("Clock moved backwards.  Refusing to generate id for {0} milliseconds",
                        lastTimestamp - timestamp));
                }

                // 如果是同一时间生成的，则进行毫秒内序列
                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;

                    // 毫秒内溢出
                    if (sequence == 0)
                    {
                        // 阻塞到下一个毫秒,获得新的时间戳
                        timestamp = tilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0;
                }

                lastTimestamp = timestamp;

                var val = (timestamp << timestampLeftShift) |
                        (dataCenterId << dataCenterIdShift) |
                        (workerId << workerIdShift) |
                        sequence;

                return val;
            }
        }

        private long tilNextMillis(long lastTimestamp)
        {
            var timestamp = timeGen();
            while (timestamp <= lastTimestamp) timestamp = timeGen();

            return timestamp;
        }

        private long timeGen()
        {
            return (long)(DateTime.UtcNow - start).TotalMilliseconds;
        }
    }
    public class IdManager
    {
        private readonly static SnowFlake idBySnowFlake = new SnowFlake(1, 1);
        private IdManager() { }

        public static SnowFlake Ins
        {
            get
            {
                return idBySnowFlake;
            }
        }
    }

    public class MsgQueue
    {
        private readonly ConcurrentQueue<DataTable> queue = new ConcurrentQueue<DataTable>();
        public MsgQueue() { }

        public void En(DataTable v)
        {
            queue.Enqueue(v);
        }

        public bool De(out DataTable v)
        {
            return queue.TryDequeue(out v);
        }

        public int Count()
        {
            return queue.Count;
        }
    }
    public class MsgQueueManager
    {
        private readonly static MsgQueue msgQueue = new MsgQueue();
        private MsgQueueManager() { }

        public static MsgQueue Ins
        {
            get
            {
                return msgQueue;
            }
        }
    }
}
