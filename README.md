# ElectricFence
## IT 2910, University of Missouri, Fall 2017

#### Abstract
_**Electric Fence**_ is a WPF-based desktop GUI running on .NET Framework 4.6 that aims to help administrators build Windows Firewall rulesets via PowerShell scripting. Currently, simultaneous asynchronous script execution is supported, and all script execution is able to be previewed before sending to PowerShell.

#### Planned Features
- Import and export _.ps1_ script files
- Live preview integration in PowerShell IDE
- Network shell remote execution

#### Notes
- Administrator privileges should be required to run the application, but this can be changed in the _app.manifest_ file
- Additional packages are not required to run **Electric Fence** in it's current state, although any additional dependencies will be listed as NuGet packages (not included here)