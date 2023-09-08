﻿#include "pch.h"

//------------------------------------------------------------------------------
// <copyright file="DX11Utils.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "DX11Utils.h"

#include <d3dx11.h>
#include "resource2.h"
#include <winuser.h>

#include "KinectDLLHelp.h"
extern HMODULE DllHandle;

/// <summary>
/// Helper for compiling shaders with D3DX11
/// </summary>
/// <param name="szFileName">full path to shader to compile</param>
/// <param name="szEntryPoint">entry point of shader</param>
/// <param name="szShaderModel">shader model to compile for</param>
/// <param name="ppBlobOut">holds result of compilation</param>
/// <returns>S_OK for success, or failure code</returns>
HRESULT CompileShaderFromFile(LPCSTR szEntryPoint, LPCSTR szShaderModel, ID3D10Blob** ppBlobOut)
{
    HRESULT hr = S_OK;

    ID3D10Blob* pErrorBlob = NULL;

    HRSRC hResource = ::FindResource(DllHandle, MAKEINTRESOURCE(IDR_SHADER1), L"SHADER");
    HGLOBAL hResourceData = ::LoadResource(DllHandle, hResource);
    LPVOID pData = ::LockResource(hResourceData);
    unsigned long resourceSize = ::SizeofResource(DllHandle, hResource);

    char* bytes = new char[resourceSize];
    memcpy(bytes, pData, resourceSize * sizeof(char));

    hr = D3DX11CompileFromMemory(
        (LPCSTR)bytes, resourceSize, NULL,
        NULL, NULL, szEntryPoint, szShaderModel,
        0, 0, NULL, ppBlobOut, &pErrorBlob, NULL );

    if ( FAILED(hr) )
    {
        if (NULL != pErrorBlob)
        {
            OutputDebugStringA( (char*)pErrorBlob->GetBufferPointer() );
        }
    }

    SAFE_RELEASE(pErrorBlob);

    return hr;
}