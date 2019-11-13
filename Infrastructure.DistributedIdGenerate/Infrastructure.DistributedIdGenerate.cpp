// Infrastructure.DistributedIdGenerate.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include <math.h>
#include <iostream>
#include <string>
#include <chrono>
#include <map>
#include <mutex>

using namespace std;

std::mutex mt;

class API
{
private:
	int workerId;
	int dataCenterId;
	int sequence;
	int workerIdBits;
	int dataCenterIdBits;
	int maxWorkerId;
	int maxDataCenterId;
	int sequenceBits;
	int workerIdShift;
	int dataCenterIdShift;
	int timestampLeftShift;
	int sequenceMask;

	map<int, char> Base64CodeEn;
	map<char, int> Base64CodeDe;
	__int64 lastTimestamp;

	__int64 tilNextMillis(__int64 lastTimestamp)
	{
		__int64 timestamp = timeGen();
		while (timestamp <= lastTimestamp) timestamp = timeGen();

		return timestamp;
	}
	__int64 timeGen()
	{
		std::chrono::time_point<std::chrono::system_clock, std::chrono::milliseconds> tp
			= std::chrono::time_point_cast<std::chrono::milliseconds>(std::chrono::system_clock::now());
		return tp.time_since_epoch().count();

		//std::chrono::time_point<std::chrono::system_clock, std::chrono::milliseconds> tp = std::chrono::time_point_cast<std::chrono::milliseconds>(std::chrono::system_clock::now());
		//auto tmp = std::chrono::duration_cast<std::chrono::milliseconds>(tp.time_since_epoch());
		//return tmp.count();
	}

	void InitBase64CodeEn()
	{
		Base64CodeEn.insert(pair<int, char>(0, '0'));
		Base64CodeEn.insert(pair<int, char>(1, '1'));
		Base64CodeEn.insert(pair<int, char>(2, '2'));
		Base64CodeEn.insert(pair<int, char>(3, '3'));
		Base64CodeEn.insert(pair<int, char>(4, '4'));
		Base64CodeEn.insert(pair<int, char>(5, '5'));
		Base64CodeEn.insert(pair<int, char>(6, '6'));
		Base64CodeEn.insert(pair<int, char>(7, '7'));
		Base64CodeEn.insert(pair<int, char>(8, '8'));
		Base64CodeEn.insert(pair<int, char>(9, '9'));

		Base64CodeEn.insert(pair<int, char>(10, 'a'));
		Base64CodeEn.insert(pair<int, char>(11, 'b'));
		Base64CodeEn.insert(pair<int, char>(12, 'c'));
		Base64CodeEn.insert(pair<int, char>(13, 'd'));
		Base64CodeEn.insert(pair<int, char>(14, 'e'));
		Base64CodeEn.insert(pair<int, char>(15, 'f'));
		Base64CodeEn.insert(pair<int, char>(16, 'g'));
		Base64CodeEn.insert(pair<int, char>(17, 'h'));
		Base64CodeEn.insert(pair<int, char>(18, 'i'));
		Base64CodeEn.insert(pair<int, char>(19, 'j'));

		Base64CodeEn.insert(pair<int, char>(20, 'k'));
		Base64CodeEn.insert(pair<int, char>(21, 'l'));
		Base64CodeEn.insert(pair<int, char>(22, 'm'));
		Base64CodeEn.insert(pair<int, char>(23, 'n'));
		Base64CodeEn.insert(pair<int, char>(24, 'o'));
		Base64CodeEn.insert(pair<int, char>(25, 'p'));
		Base64CodeEn.insert(pair<int, char>(26, 'q'));
		Base64CodeEn.insert(pair<int, char>(27, 'r'));
		Base64CodeEn.insert(pair<int, char>(28, 's'));
		Base64CodeEn.insert(pair<int, char>(29, 't'));

		Base64CodeEn.insert(pair<int, char>(30, 'u'));
		Base64CodeEn.insert(pair<int, char>(31, 'v'));
		Base64CodeEn.insert(pair<int, char>(32, 'w'));
		Base64CodeEn.insert(pair<int, char>(33, 'x'));
		Base64CodeEn.insert(pair<int, char>(34, 'y'));
		Base64CodeEn.insert(pair<int, char>(35, 'z'));
		Base64CodeEn.insert(pair<int, char>(36, 'A'));
		Base64CodeEn.insert(pair<int, char>(37, 'B'));
		Base64CodeEn.insert(pair<int, char>(38, 'C'));
		Base64CodeEn.insert(pair<int, char>(39, 'D'));

		Base64CodeEn.insert(pair<int, char>(40, 'E'));
		Base64CodeEn.insert(pair<int, char>(41, 'F'));
		Base64CodeEn.insert(pair<int, char>(42, 'G'));
		Base64CodeEn.insert(pair<int, char>(43, 'H'));
		Base64CodeEn.insert(pair<int, char>(44, 'I'));
		Base64CodeEn.insert(pair<int, char>(45, 'J'));
		Base64CodeEn.insert(pair<int, char>(46, 'K'));
		Base64CodeEn.insert(pair<int, char>(47, 'L'));
		Base64CodeEn.insert(pair<int, char>(48, 'M'));
		Base64CodeEn.insert(pair<int, char>(49, 'N'));

		Base64CodeEn.insert(pair<int, char>(50, 'O'));
		Base64CodeEn.insert(pair<int, char>(51, 'P'));
		Base64CodeEn.insert(pair<int, char>(52, 'Q'));
		Base64CodeEn.insert(pair<int, char>(53, 'R'));
		Base64CodeEn.insert(pair<int, char>(54, 'S'));
		Base64CodeEn.insert(pair<int, char>(55, 'T'));
		Base64CodeEn.insert(pair<int, char>(56, 'U'));
		Base64CodeEn.insert(pair<int, char>(57, 'V'));
		Base64CodeEn.insert(pair<int, char>(58, 'W'));
		Base64CodeEn.insert(pair<int, char>(59, 'X'));

		Base64CodeEn.insert(pair<int, char>(60, 'Y'));
		Base64CodeEn.insert(pair<int, char>(61, 'Z'));
		Base64CodeEn.insert(pair<int, char>(62, '-'));
		Base64CodeEn.insert(pair<int, char>(63, '_'));
	}
	void InitBase64CodeDe()
	{
		map<int, char>::iterator it = Base64CodeEn.begin();

		while (it != Base64CodeEn.end())
		{
			Base64CodeDe.insert(pair<char, int>(it->second, it->first));
			it++;
		}
	}
	void InitPrivate()
	{
		workerIdBits = 6;
		dataCenterIdBits = 4;
		maxWorkerId = -1 ^ (-1 << workerIdBits);
		maxDataCenterId = -1 ^ (-1 << dataCenterIdBits);
		sequenceBits = 12;

		workerIdShift = sequenceBits;
		dataCenterIdShift = sequenceBits + workerIdBits;
		timestampLeftShift = sequenceBits + workerIdBits + dataCenterIdBits;
		sequenceMask = -1 ^ (-1 << sequenceBits);
		lastTimestamp = -1;
	}
	void Seting(int _workerId, int _dataCenterId, int workerIdLen, int dataCenterIdLen)
	{
		if (_workerId > maxWorkerId || _workerId < 0)
		{
			//throw new Exception(string.Format("机器Id不能大于 {0} 或小于 0", maxWorkerId));
			throw -1;
		}

		if (_dataCenterId > maxDataCenterId || _dataCenterId < 0)
		{
			//throw new Exception(string.Format("数据中心不能大于 {0} 或小于 0", maxDataCenterId));
			throw -2;
		}

		workerId = _workerId;
		dataCenterId = _dataCenterId;

		//Console.WriteLine("服务启动. 时间戳左移:{0}b, 数据中心:{1}b, 机器Id:{2}b, 流水号:{3}b, 当前workerId:{4}",
		//	timestampLeftShift, dataCenterIdBits, workerIdBits, sequenceBits, workerId);

		// set length
		if (workerIdLen <= 0 || dataCenterIdLen <= 0)
		{
			throw -3;
		}

		if (workerIdLen + dataCenterIdLen > 10)
		{
			throw -4;
		}

		workerIdBits = workerIdLen;
		dataCenterIdBits = dataCenterIdLen;
	}
public:
	API(int workerId, int dataCenterId, int workerIdLen, int dataCenterIdLen)
	{
		// init
		InitPrivate();

		// set 
		Seting(workerId, dataCenterId, workerIdLen, dataCenterIdLen);

		// map
		InitBase64CodeEn();
		InitBase64CodeDe();
	}
	~API();

	__int64 NextId()
	{
		std::lock_guard<std::mutex> lck(mt);

		__int64 timestamp = timeGen();

		// 如果当前时间小于上一次ID生成的时间戳，说明系统时钟回退过这个时候应当抛出异常
		if (timestamp < lastTimestamp)
		{
			//Console.WriteLine("clock is moving backwards.  Rejecting requests until {0}.", lastTimestamp);
			//throw new Exception(string.Format("Clock moved backwards.  Refusing to generate id for {0} milliseconds",
			//	lastTimestamp - timestamp));
			throw -11;
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

		__int64 val = (timestamp << timestampLeftShift) |
			(dataCenterId << dataCenterIdShift) |
			(workerId << workerIdShift) |
			sequence;

		return val;
	}
	int GetWorkerId()
	{
		return workerId;
	}

	int IntCompress(__int64 value, char *str64)
	{
		int i = 1;

		const int len = 20;
		char arr[len] = { 0 };

		while (value >= 1)
		{
			__int64 index = value & 0x3F;
			arr[len - i] = Base64CodeEn[index];

			value = value >> 6;
			i++;
		}

		int start = len - i;
		memset(str64, 0, 20);
		memcpy(str64, arr + start + 1, i - 1);

		//printf("%s \r\n", str64);

		return 1;
	}
	__int64 IntDecompression(char *str64, int len)
	{
		__int64 value = 0;
		int power = len - 1;

		for (int i = 0; i <= power; i++)
		{
			double v = pow(64, i);
			value += Base64CodeDe[str64[power - i]] * (__int64)v;

			//printf("%I64d \r\n", value);
		}

		return value;
	}
};

static API * api = nullptr;

extern "C" __declspec(dllexport) int Worker(int workerId, int dataCenterId, int workerIdLen = 6, int dataCenterIdLen = 4)
{
	api = new API(workerId, dataCenterId, workerIdLen, dataCenterIdLen);
	return api->GetWorkerId() > 0;
}

extern "C" __declspec(dllexport) __int64 NextId()
{
	if (api == nullptr) return -1;

	return api->NextId();
}

extern "C" __declspec(dllexport) int IntCompress(__int64 value, char *str64)
{
	if (api == nullptr) return -1;

	return api->IntCompress(value, str64);
}

extern "C" __declspec(dllexport) __int64 IntDecompression(char *str64, int len)
{
	if (api == nullptr) return -1;

	return api->IntDecompression(str64, len);
}