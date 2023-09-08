#include "pch.h"

#include "KinectDLLHelp.h"
#include <stdio.h>

#define MAX_MSG_LENGTH 200
void showErrorBox(LPCWSTR text)
{
    WCHAR buffer[MAX_MSG_LENGTH];
    wsprintf(buffer, text);
    MessageBox(NULL, buffer, L"INFO", MB_ICONHAND | MB_OK);
}

void WriteLog(void* data, size_t size, size_t count)
{
    FILE* fp = NULL;
    fopen_s(&fp, "log.txt", "w");
    fwrite(data, size, count, fp);
    fclose(fp);
}