// ConsoleApplication1.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"
#include<stdio.h>
void printchs(char& ch)
{
	printf("%s",**(char***)&ch);//���ͨ��ch�����"this is a test string"
}

int main(int a,char* b[],char** c)
{
	char** chs = new char*[1];
	chs[0] = "this is a test string";
	char& ch = (char&)chs;
	printchs(ch);
	delete chs;
}



