// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <cstdio>
#include <cstdint>
#include <string>
#include "MinHook.h"

const int TARGET_WIDTH = 1080;
const int TARGET_HEIGHT = 1920;

typedef int64_t(__fastcall* tWndProc)(HWND, UINT, WPARAM, WPARAM);
tWndProc WndProc;
tWndProc fpWndProc;

typedef HWND(__stdcall* tCreateWindowExW)(DWORD dwExStyle, LPCWSTR lpClassName, LPCWSTR lpWindowName, DWORD dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam);
tCreateWindowExW fpCreateWindowExW;
HWND __stdcall DetourCreateWindowExW(DWORD dwExStyle, LPCWSTR lpClassName, LPCWSTR lpWindowName, DWORD dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam)
{
    nWidth = TARGET_WIDTH;
    nHeight = TARGET_HEIGHT;
    return fpCreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
}

int64_t __fastcall DetourWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, WPARAM lParam)
{
    if (uMsg == WM_GETMINMAXINFO)
    {
        DefWindowProc(hWnd, uMsg, wParam, lParam);
        MINMAXINFO* pmmi = (MINMAXINFO*)lParam;
        pmmi->ptMaxTrackSize.x = TARGET_WIDTH;
        pmmi->ptMaxTrackSize.y = TARGET_HEIGHT;

        // is this needed? don't care because it works
        SetWindowPos(hWnd, HWND_TOP, 0, 0, TARGET_WIDTH, TARGET_HEIGHT, SWP_NOMOVE | SWP_FRAMECHANGED);
        return 0;
    }
    return fpWndProc(hWnd, uMsg, wParam, lParam);
}

int CreateHooks()
{
    OutputDebugStringW(L"[galliumhook] CreateHooks()\r\n");
    auto base = (uint64_t)GetModuleHandleW(NULL);
    WndProc = (tWndProc)(base + 0x72AE80ull);

    if (MH_Initialize() != MH_OK)
    {
        MessageBox(NULL, L"Couldn't initialize MinHook", L"galliumhook", MB_OK);
        return 0;
    }

    auto createResult = MH_CreateHook(WndProc, &DetourWndProc, reinterpret_cast<LPVOID*>(&fpWndProc));
    if (createResult != MH_OK)
    {
        MessageBox(NULL, L"Couldn't create WndProc hook", L"galliumhook", MB_OK);
        return 0;
    }

    if (MH_EnableHook(WndProc) != MH_OK)
    {
        MessageBox(NULL, L"Couldn't enable WndProc hook", L"galliumhook", MB_OK);
        return 0;
    }

    if (MH_CreateHook(&CreateWindowExW, &DetourCreateWindowExW, reinterpret_cast<LPVOID*>(&fpCreateWindowExW)) != MH_OK)
    {
        MessageBox(NULL, L"Couldn't create CreateWindowExW hook", L"galliumhook", MB_OK);
        return 0;
    }

    if (MH_EnableHook(&CreateWindowExW) != MH_OK)
    {
        MessageBox(NULL, L"Couldn't enable CreateWindowExW hook", L"galliumhook", MB_OK);
        return 0;
    }

    OutputDebugStringW(L"[galliumhook] CreateHooks() completed");
    return 0;
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        LoadLibrary(L"mercuryhook.dll");
        CreateHooks();
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

