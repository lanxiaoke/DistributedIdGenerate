// ConsoleApplication1.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"

#include <iostream>
using namespace std;

int Fun()
{
	char *p = new char[16];
	memset(p, 0x00, 16 * sizeof(char));

	p[0] = 1; //�Ϸ���д
	p[15] = 2; //�Ϸ���д
	p[16] = 3; //Խ�磬delete�ɼ�����
	p[17] = 4; //Խ�磬delete�ɼ�����
	p[18] = 5; //Խ�磬delete�ɼ�����
	p[19] = 6; //Խ�磬delete�ɼ�����
	p[20] = 7; //Խ�磬Gflags�ɼ�����

	delete p;
	p = NULL;

	return 0;
}

extern "C" __declspec(dllexport) int Fun1()
{
	return Fun();
}

// SMDB
#pragma comment(lib,"Infrastructure.DistributedIdGenerate.lib")
extern "C" int Worker(int workerId, int dataCenterId, int workerIdLen = 6, int dataCenterIdLen = 4);
extern "C" __int64 NextId();
extern "C" int IntCompress(__int64 value, char *str64);
extern "C" __int64 IntDecompression(char *str64, int len);

int _tmain(int argc, _TCHAR* argv[])
{
	/*Fun();*/

	////__try
	//try
	//{
	//	char *p = new char[16];
	//	memset(p, 0x00, 16 * sizeof(char));

	//	p[0] = 1; //�Ϸ���д
	//	p[15] = 2; //�Ϸ���д
	//	p[16] = 3; //Խ�磬delete�ɼ�����

	//	delete p;
	//	p = NULL;
	//}
	////__except (0)
	//catch (...)
	//{ 
	//	printf("11");
	//}



	Worker(1, 1);

	for (int i = 0; i < 100; i++)
	{

		__int64 v = NextId();
		//__int64 v = 6547639579355385856;
		printf("%I64d = ", v);

		char str64[20] = { 0 };
		IntCompress(v, str64);

		printf(" %s = ", str64);

		__int64 vv = IntDecompression(str64, strlen(str64));
		printf(" %I64d \r\n", vv);


		//__int64 v2 = 6547639579355385857;
		//printf("%I64d = ", v2);

		//char str642[20] = { 0 };
		//IntToI64(v2, str642);

		//printf(" %s = \r\n", str642);

		//__int64 vv2 = I64ToInt(str642, strlen(str642));
		////printf(" %I64d \r\n", vv2);
		//printf("%I64d", vv2);


	}

	getchar();
}

