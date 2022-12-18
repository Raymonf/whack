// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "toml.hpp"
#include <cstdio>
#include <cstdint>
#include <string>
// i hate C++20
#include <locale>
#include <codecvt>
#include "MinHook.h"

int targetWidth = 1080;
int targetHeight = 1920;
int windowX = 0;
int windowY = 0;
uint64_t wndProcAddr = 0x72AE80ull;

typedef int64_t(__fastcall* tWndProc)(HWND, UINT, WPARAM, WPARAM);
tWndProc WndProc;
tWndProc fpWndProc;

typedef HWND(__stdcall* tCreateWindowExW)(DWORD dwExStyle, LPCWSTR lpClassName, LPCWSTR lpWindowName, DWORD dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam);
tCreateWindowExW fpCreateWindowExW;
HWND __stdcall DetourCreateWindowExW(DWORD dwExStyle, LPCWSTR lpClassName, LPCWSTR lpWindowName, DWORD dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam)
{
    nWidth = targetWidth;
    nHeight = targetHeight;
    return fpCreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
}

int64_t __fastcall DetourWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, WPARAM lParam)
{
    if (uMsg == WM_GETMINMAXINFO)
    {
        DefWindowProc(hWnd, uMsg, wParam, lParam);
        MINMAXINFO* pmmi = (MINMAXINFO*)lParam;
        pmmi->ptMaxTrackSize.x = targetWidth;
        pmmi->ptMaxTrackSize.y = targetHeight;

        // is this needed? don't care because it works
        SetWindowPos(hWnd, HWND_TOP, windowX, windowY, targetWidth, targetHeight, SWP_NOMOVE | SWP_FRAMECHANGED);
        return 0;
    }
    return fpWndProc(hWnd, uMsg, wParam, lParam);
}

int CreateHooks()
{
    OutputDebugStringW(L"[galliumhook] CreateHooks()\r\n");
    auto base = (uint64_t)GetModuleHandleW(NULL);

    if (MH_Initialize() != MH_OK)
    {
        MessageBox(NULL, L"Couldn't initialize MinHook", L"galliumhook", MB_OK);
        return 0;
    }

    if (wndProcAddr != 0)
    {
        OutputDebugStringW(L"[galliumhook] Creating WndProc hook\r\n");
        WndProc = (tWndProc)(base + wndProcAddr);

        if (MH_CreateHook(WndProc, &DetourWndProc, reinterpret_cast<LPVOID*>(&fpWndProc)) != MH_OK)
        {
            MessageBox(NULL, L"Couldn't create WndProc hook", L"galliumhook", MB_OK);
            return 0;
        }

        if (MH_EnableHook(WndProc) != MH_OK)
        {
            MessageBox(NULL, L"Couldn't enable WndProc hook", L"galliumhook", MB_OK);
            return 0;
        }
    }
    else
    {
        OutputDebugStringW(L"[galliumhook] Not creating WndProc hook\r\n");
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

void LoadSettings()
{
    OutputDebugStringW(L"[galliumhook] LoadSettings()\r\n");

    std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
    try
    {
        auto config = toml::parse_file("galliumhook.toml");
        targetWidth = config["window"]["width"].value_or(targetWidth);
        targetHeight = config["window"]["height"].value_or(targetHeight);
        wndProcAddr = config["window"]["wndproc"].value_or(wndProcAddr);

        // don't know if these do anything
        windowX = config["position"]["x"].value_or(windowX);
        windowY = config["position"]["y"].value_or(windowY);
    }
    catch (std::runtime_error& e)
    {
        OutputDebugStringW(L"error while loading settings occurred");

        auto message = converter.from_bytes(e.what());
        OutputDebugStringW(message.c_str());
    }
}

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        LoadLibrary(L"mercuryhook.dll");
        LoadSettings();
        CreateHooks();
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
