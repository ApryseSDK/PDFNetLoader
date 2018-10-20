using System;
using System.Reflection;

namespace pdftron
{
    /// <summary>
    /// This class allows for AnyCPU support, and loads the corresponding
    /// x86 or x64 PDFNet.dll at runtime based on the current system. Also
    /// known as side-by-side assembly loading.
    /// </summary>
    /// <example>
    /// To use this class, the method PDFNetLoader.Instance must be added before
    /// any methods containing calls to PDFNet are called. The easiest way to handle
    /// this, is to add the following line of code to the class containing your
    /// entry point, or Main() method.
    /// <code>
    /// private static pdftron.PDFNetLoader pdfnet = pdftron.PDFNetLoader.Instance();
    /// </code>
    /// </example>
    public class PDFNetLoader
    {
        private static readonly PDFNetLoader _singleton = new PDFNetLoader();

        static PDFNetLoader()
        {
            // Just a blank static constructor to force initializers to run first.
        }

        #region Interface

        /// <summary>
        /// Load PDFNetLoader for runtime PDFNet loading.
        /// Loads PDFNet.dll's from 'PDFNet/[x86|x64]'
        /// folders in current working directory.
        /// </summary>
        /// <returns></returns>
        public static PDFNetLoader Instance()
        {
            return _singleton;
        }

        /// <summary>
        /// Change the assembly loading directory. Default is 'PDFNet' sub-folder
        /// in the current working directory. Target folder needs to contain
        /// two sub-folders, x86 and x64, each containing the respective PDFNet.dll assembly.
        /// </summary>
        /// <param name="path">The new path to look for the x86 and x64 folders containing PDFNet.dll</param>
        /// <returns>The PDFNetLoader instance</returns>
        public PDFNetLoader Path(string path)
        {
            this.path = path;
            return this;
        }

        # endregion

        #region Implementation


        private Assembly PDFNetResolveEventHandler(object sender, ResolveEventArgs args)
        {
            if (!args.Name.StartsWith("PDFNet,")) return null;
            
            Assembly assembly = null;
            string module_path = path;
            if (Is64BitProcess())
            {
                module_path = System.IO.Path.Combine(module_path, "x64");
            }
            else
            {
                module_path = System.IO.Path.Combine(module_path, "x86");
            }
            module_path = System.IO.Path.Combine(module_path, "PDFNet.dll");
            assembly = Assembly.LoadFrom(module_path);
            return assembly;
        }

        private PDFNetLoader()
        {
            path = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "PDFNet");
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(PDFNetResolveEventHandler);
        }

        private static bool Is64BitProcess()
        {
            return IntPtr.Size == 8;
        }

        private string path;

        #endregion
    }
}
