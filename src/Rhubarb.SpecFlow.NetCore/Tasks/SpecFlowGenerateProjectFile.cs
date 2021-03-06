﻿using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
 
namespace Rhubarb.SpecFlow.NetCore.Tasks
{
    public class SpecFlowGenerateProjectFile : Task
    {
        [Required]
        public ITaskItem OutputProjectFile { get; set; }

        [Required]
        public ITaskItem[] FeatureFiles { get; set; }

        public SpecFlowGenerateProjectFile()
            : base(AssemblyResources.PrimaryResources)
        {

        }

        private bool ValidateInputs()
        {
            if (OutputProjectFile == null)
            {
                Log.LogErrorWithCodeFromResources(nameof(SpecFlowGenerateProjectFile) + ".NeedsOutputProjectFile", nameof(OutputProjectFile));
                return false;
            }

            return true;
        }

        public override bool Execute()
        {
            if (!ValidateInputs())
            {
                return false;
            }

            const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";

            var settings = new XmlWriterSettings
            {
                Indent = true
            };

            using (var w = XmlWriter.Create(OutputProjectFile.ItemSpec, settings))
            {
                w.WriteStartElement("Project", ns);
                w.WriteAttributeString("ToolsVersion", "14.0");
                w.WriteAttributeString("DefaultTargets", "Build");

                w.WriteStartElement("PropertyGroup", ns);
                w.WriteStartElement("RootNamespace", ns);
                w.WriteValue("SpecFlow.GeneratedTests");
                w.WriteEndElement();
                w.WriteStartElement("AssemblyName", ns);
                w.WriteValue("SpecFlow.GeneratedTests");
                w.WriteEndElement();
                w.WriteEndElement();

                w.WriteStartElement("ItemGroup", ns);
                w.WriteStartElement("None", ns);
                w.WriteAttributeString("Include", "app.config");
                w.WriteEndElement();

                if (FeatureFiles != null)
                {
                    foreach (var featureFile in FeatureFiles)
                    {
                        w.WriteStartElement("None", ns);
                        w.WriteAttributeString("Include", featureFile.ItemSpec);

                        w.WriteStartElement("Generator", ns);
                        w.WriteValue("SpecFlowSingleFileGenerator");
                        w.WriteEndElement();

                        w.WriteStartElement("LastGenOutput", ns);
                        w.WriteValue(featureFile.ItemSpec + ".cs");
                        w.WriteEndElement();

                        w.WriteEndElement();
                    }
                }

                w.WriteEndElement();

                w.WriteEndElement();
            }

            return true;
        }
    }
}
