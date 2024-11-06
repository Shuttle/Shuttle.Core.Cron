using System.Reflection;
using System.Runtime.InteropServices;

#if NETFRAMEWORK
[assembly: AssemblyTitle(".NET Framework")]
#endif

#if NETCOREAPP
[assembly: AssemblyTitle(".NET Core")]
#endif

#if NETSTANDARD
[assembly: AssemblyTitle(".NET Standard")]
#endif

[assembly: AssemblyVersion("10.1.4.0")]
[assembly: AssemblyCopyright("Copyright (c) #{Year}#, Eben Roux")]
[assembly: AssemblyProduct("Shuttle.Core.Cron")]
[assembly: AssemblyCompany("Eben Roux")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyInformationalVersion("#{SemanticVersion}#")]
[assembly: ComVisible(false)]