﻿namespace CUITe.IntegrationTests.NuGet
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class AssemblyResourceLoader
    {
        public static void ExtractEmbeddedResource(Assembly assembly, string resourceName, string outputFilePath)
        {
            using (Stream input = assembly.GetManifestResourceStream(resourceName))
            {
                if (input == null)
                {
                    throw new Exception(string.Format("Failed to get manifest resource stream '{0}' in assembly '{1}'", resourceName, assembly.Location));
                }

                using (Stream output = File.Open(outputFilePath, FileMode.Create))
                {
                    byte[] buffer = new byte[8 * 1024];
                    int len;
                    while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, len);
                    }
                }
            }
        }
    }
}
