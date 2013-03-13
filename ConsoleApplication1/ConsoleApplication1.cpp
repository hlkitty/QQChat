/* 
 * combinationForNumber.cpp 
 * 
 *  Created on: 2012-10-19 
 *      Author: happier 
 */  
#include "stdafx.h"
#include <iostream>   
#include <string.h>   
#include <cstdio>   
#include <cstdlib>   
using namespace std;  
  
#define MAX_VALUE   8   
int thenext[MAX_VALUE] = { 0 };    //��������һ������thenext[i]��ʾi��������ŵ�����   
  
/* 
 * �ݹ������� 
 * @nSum Ŀ��� 
 * @pData �����Ѿ����ڵ����� 
 * @nDepth ��¼��ǰ�Ѿ��������ݵĸ��� 
 */  
void SegNum(int nSum, int* pData, int nDepth)  
{  
    if (nSum < 0)  
        return;  
  
    //����Ѿ�����Ҫ�󣬿�ʼ���   
    if (nSum == 0)  
    {  
		cout<<MAX_VALUE<<" = ";
		int j;
        for (j = 0; j < nDepth - 1; j++)  
            cout << pData[j] << " + ";  
		cout<<pData[j];
        cout << endl;  
  
        return;  
    }  
  
    //������һ��Сtrick�����Ҫ����ֵ��������õ�һ�ָ�ֵ��ʽ   
    //����������ظ��ģ����ǵ�����ʽ�����õڶ��ָ�ֵ   
    int i = (nDepth == 0 ? thenext[0] : pData[nDepth - 1]);  
    //int i = thenext[0];   
    for (; i <= nSum;)  
    {  
        pData[nDepth++] = i;  
        SegNum(nSum - i, pData, nDepth);  
        nDepth--;   //�ݹ���ɺ󣬽�ԭ�������ݵ�����������ȥ�����е���һ������   
  
        i = thenext[i];  
    }  
  
    return ;  
}  
  
void ShowResult(int array[], int nLen)  
{  
    thenext[0] = array[0];  
    int i = 0;  
    for (; i < nLen - 1; i++)  
        thenext[array[i]] = array[i + 1];  //��һ����ѡ���ִ�С   
    thenext[array[i]] = array[i] + 1;  //thenext[MAX_VALUE]����MAX_VALUE��һ��Сtrick�������˺ܶ�Ƚ�   
  
    int* pData = new int[MAX_VALUE];  
    SegNum(MAX_VALUE, pData, 0);  
    delete[] pData;  
}  
  
int main()  
{  
    int* array = new int[MAX_VALUE];  
    for (int i = 0; i < MAX_VALUE; i++)  
    {  
        array[i] = i + 1;  
    }  
    //����Ǯ����   
    ShowResult(array, MAX_VALUE);  
  
    //system("pause");   
    return 0;  
} 