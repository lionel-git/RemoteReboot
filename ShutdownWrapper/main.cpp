#include <windows.h>

#include <iostream>

// cf https://docs.microsoft.com/en-us/windows/win32/shutdown/how-to-shut-down-the-system
// Modified
extern "C" __declspec(dllexport) 
bool MySystemReboot(bool force_apps_closed, bool dry_run)
{
    HANDLE hToken;
    TOKEN_PRIVILEGES tkp;

    // Get a token for this process. 
    if (!OpenProcessToken(GetCurrentProcess(),
        TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hToken))
        return(FALSE);

    // Get the LUID for the shutdown privilege. 
    LookupPrivilegeValue(NULL, SE_SHUTDOWN_NAME, &tkp.Privileges[0].Luid);
    tkp.PrivilegeCount = 1;  // one privilege to set    
    tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

    // Get the shutdown privilege for this process. 
    AdjustTokenPrivileges(hToken, FALSE, &tkp, 0, (PTOKEN_PRIVILEGES)NULL, 0);

    if (GetLastError() != ERROR_SUCCESS)
        return FALSE;

    std::cout << std::hex << "Will call InitiateSystemShutdown(reboot) with force_apps_closed = " << force_apps_closed << std::endl;
    if (!dry_run)
    {
        // https://docs.microsoft.com/en-us/windows/win32/api/winreg/nf-winreg-initiatesystemshutdownexa
        const char* message = "Test Reboot";
        DWORD reason = SHTDN_REASON_MAJOR_OPERATINGSYSTEM | SHTDN_REASON_MINOR_UPGRADE | SHTDN_REASON_FLAG_PLANNED;
        return InitiateSystemShutdownEx(nullptr, (char*)message, 10, force_apps_closed, true, reason);
    }
    else
    {
        std::cout << "Dry run ..." << std::endl;
        return true;
    }
}
