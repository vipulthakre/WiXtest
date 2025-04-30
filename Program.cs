using System;
using test.wix.Dialogs;
using WixSharp;

namespace test.wix
{
    public class Program
    {
        static void Main()
        {
            Feature doc = new Feature("Documentation");
            var project = new ManagedProject("test.app",
                             new InstallDir(@"%ProgramFiles64Folder%\VipulWix\test.app",
                                 new Files(@"C:\Wix\test.wix\bin\Debug\net472\*.exe"),
                                 new Files(@"C:\Wix\test.wix\bin\Debug\net472\*.dll"),
                                 new Files(@"C:\Wix\test.wix\bin\Debug\net472\*.pdb"),
                                 new ExeFileShortcut("Uninstall","[System64Folder]msiexec.exe", "/x [ProductCode]"),
                                 new Dir(@"Eventlog", new DirPermission("Everyone", GenericPermission.All)),
                                 new File("readme.txt")
                                 { 
                                    Features = new[] { doc }
                                 }
                                 ),
                             new Dir("%Startup%",
                                new ExeFileShortcut("test.app", "[INSTALLDIR]test.app.exe", "")),
                             new Dir(@"%ProgramMenu%\VipulWix\test.app",
                                new ExeFileShortcut("Uninstall test.app", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

            project.GUID = new Guid("6059577b-ab56-4c21-9de5-2c267578d301");

            //custom set of standard UI dialogs
            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                            .Add<LicenceDialog>()
                                            .Add<SetupTypeDialog>()
                                            .Add<FeaturesDialog>()
                                            .Add<InstallDirDialog>()
                                            .Add<ProgressDialog>()
                                            .Add<ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
                                           .Add<FeaturesDialog>()
                                           .Add<ProgressDialog>()
                                           .Add<ExitDialog>();

            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            ValidateAssemblyCompatibility();

            project.BuildMsi();
        }

        static void ValidateAssemblyCompatibility()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            if (!assembly.ImageRuntimeVersion.StartsWith("v2."))
            {
                Console.WriteLine("Warning: assembly '{0}' is compiled for {1} runtime, which may not be compatible with the CLR version hosted by MSI. " +
                                  "The incompatibility is particularly possible for the EmbeddedUI scenarios. " +
                                   "The safest way to solve the problem is to compile the assembly for v3.5 Target Framework.",
                                   assembly.GetName().Name, assembly.ImageRuntimeVersion);
            }
        }
    }
}