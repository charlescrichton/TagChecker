using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Dicom.Data;
using Dicom;
using Dicom.IO;

namespace TagChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!(args.Length == 1 || args.Length == 2))
            {
                
                Console.WriteLine("usage: TagChecker <DCM-Directory> [txt-output]");
                return;
            }

            String RootDirectoryPath = args[0];

            String TextOutputPath = null;
            if (args.Length == 2)
            {
                TextOutputPath = args[1];
            }

            //Check to see that it exists
            DirectoryInfo RootDirectoryInfo = new DirectoryInfo(RootDirectoryPath);
            if (!RootDirectoryInfo.Exists)
            {
                Console.WriteLine("The directory '" + RootDirectoryPath + "' does not exist.");
                return;
            }

            //Load internal dictionary
            DcmDictionary.LoadInternalDictionary();


            TextWriter output;
            try
            {
                if (args.Length == 2)
                {
                    output = new StreamWriter(TextOutputPath);
                }
                else
                {
                    output = Console.Out;
                }

                var DicomTagsToCheck = CalculateDicomTagsToCheck();
                var TagTovalues = new Dictionary<DicomTag, List<String>>();
                CheckDirectory(RootDirectoryInfo, output, DicomTagsToCheck, TagTovalues);

                output.WriteLine("All values");
                foreach (var tag in TagTovalues.Keys)
                {
                    List<String> values = TagTovalues[tag];
                    output.WriteLine(tag.Entry);
                    foreach (var value in values)
                    {
                        output.WriteLine(" = " + value);
                    }
                    output.WriteLine();
                }
                 
                output.Flush();
                output.Close();



            }
            catch (Exception e)
            {
                Console.WriteLine("CheckTag: Failed, reason: {0}.", e.Message);
            }

            Console.In.Read();

        }

        private static void CheckDirectory(DirectoryInfo DIR, TextWriter output, List<DicomTag> DicomTagsToCheck, Dictionary<DicomTag, List<String>> tagTovalues)
        {

            //Go through the directory and find all of the .dcm files

            foreach (FileInfo FI in DIR.EnumerateFiles("*.dcm"))
            {
                CheckDICOMFile(FI, output, DicomTagsToCheck, tagTovalues);
            }

            //Go through the directory and check the sub directories
            foreach (DirectoryInfo SD in DIR.EnumerateDirectories())
            {
                CheckDirectory(SD, output,DicomTagsToCheck, tagTovalues);
            }

        }

        private static void addValue(Dictionary<DicomTag, List<String>> tagTovalues, DicomTag tag, String value)
        {
            if (value == null || value.Length == 0)
            {
                return;
            }

            List<String> values;
            if (!tagTovalues.TryGetValue(tag, out values))
            {
                values = new List<String>();
                tagTovalues.Add(tag, values);
            }

            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }

        private static void CheckDICOMFile(FileInfo FI, TextWriter output, List<DicomTag> DicomTagsToCheck, Dictionary<DicomTag, List<String>> tagTovalues)
        {
 

            //output.WriteLine("Checking: " + FI.FullName);

            var dcmFile = new DicomFileFormat();
            var dcmFileName = FI.FullName;
            if (dcmFile.Load(dcmFileName, DicomReadOptions.Default) == DicomReadStatus.Success)
            {
                var sb = new StringBuilder();

                if (dcmFile.Dataset != null)
                {
                    foreach(var DT in DicomTagsToCheck) {
                        var Element = dcmFile.Dataset.GetElement(DT);
                       
                        if (Element != null)
                        {
                            var ElementValue = Element.GetValueString();
                            if (ElementValue != null && ElementValue.Length != 0)
                            {
                                //output.WriteLine(Element + " = " + ElementValue);
                                addValue(tagTovalues, DT, ElementValue);
                            }
                        }
                    }
                     
                }
                else
                {
                    Console.WriteLine("dcm2txt: Missing dataset in DICOM file '{0}'.", dcmFileName);

                    return;
                }

                //output.WriteLine(sb.ToString());
            }
            else
            {
                Console.WriteLine("dcm2txt: '{0}' does not appear to be a DICOM file.", dcmFileName);
                return;
            }
        }

        private static List<DicomTag> CalculateDicomTagsToCheck()
        {
            var TagsToCheck = new String[] { 
                
               
"(0002,0013)",
"(0008,0080)",
"(0008,0090)",
"(0008,0115)",
"(0008,1010)",
"(0008,1040)",
"(0008,1050)",
"(0008,1060)",
"(0008,1070)",
"(0008,1090)",
"(0008,1120)",
"(0008,2120)",
"(0008,2127)",
"(0008,2132)",
"(0010,0010)",
"(0010,0020)",
"(0010,0021)",
"(0010,0022)",
"(0010,0030)",
"(0010,0032)",
"(0010,0032)",
"(0010,0040)",
"(0010,0050)",
"(0010,0050)",
"(0010,0101)",
"(0010,0102)",
"(0010,1000)",
"(0010,1001)",
"(0010,1002)",
"(0010,1005)",
"(0010,1010)",
"(0010,1020)",
"(0010,1020)",
"(0010,1030)",
"(0010,1040)",
"(0010,1050)",
"(0010,1060)",
"(0010,1080)",
"(0010,1081)",
"(0010,1090)",
"(0010,2000)",
"(0010,2110)",
"(0010,2150)",
"(0010,2152)",
"(0010,2154)",
"(0010,2160)",
"(0010,2180)",
"(0010,21A0)",
"(0010,21B0)",
"(0010,21C0)",
"(0010,21D0)",
"(0010,21F0)",
"(0010,21F0)",
"(0010,2201)",
"(0010,2202)",
"(0010,2203)",
"(0010,2292)",
"(0010,2293)",
"(0010,2294)",
"(0010,2295)",
"(0010,2296)",
"(0010,2297)",
"(0010,2298)",
"(0010,2299)",
"(0010,4000)",
"(0010,9431)",
"(0012,0010)",
"(0012,0021)",
"(0012,0031)",
"(0012,0040)",
"(0012,0042)",
"(0012,0060)",
"(0012,0062)",
"(0018,0024)",
"(0018,0034)",
"(0018,1018)",
"(0018,101B)",
"(0018,1030)",
//
"(0018,1111)",
"(0018,1180)",
"(0018,1250)",
"(0018,1251)",
"(0018,5100)",
"(0018,5104)",
"(0018,702A)",
"(0018,702B)",
"(0018,9005)",
"(0018,9041)",
"(0018,9047)",
"(0018,9050)",
"(0018,9313)",
"(0018,9318)",
"(0018,9423)",
"(0018,9447)",
"(0020,0020)",
//"(0020,0032)",
//"(0020,0037)",
"(0020,1200)",
"(0020,1202)",
"(0020,1204)",
"(0020,9450)",
"(0022,0005)", 
"(0022,0006)",
"(0038,0004)",
"(0038,001E)",
"(0038,0300)",
"(0038,0400)",
"(0038,0500)",
"(0038,0502)",
"(0040,0006)",
"(0040,0010)",
"(0040,0242)",
"(0040,1004)",
"(0040,1010)",
"(0040,3001)",
"(0040,4025)",
"(0040,4028)",
"(0040,4037)",
"(0040,A043)",
"(0040,A075)",
"(0040,A123)",
"(0054,0018)",
"(0054,0410)",
"(0054,0412)",
"(0054,0414)",
"(0062,0009)",
"(0070,0084)",
"(0072,0002)",
"(0072,0010)",
"(0072,0700)",
"(0072,0714)",
"(0088,0906)",
"(2110,0030)",
"(3002,0003)",
"(3002,0020)",
"(3006,0004)",
"(3006,0026)",
"(300A,0003)",
"(300A,004C)",
"(300A,00B2)",
"(300A,00C2)",
"(300A,00FE)",
"(300A,0122)",
"(300A,0123)",
"(300A,0180)",
"(300A,0182)",
"(300A,0183)",
"(300A,0184)",
"(300A,0226)",
"(300A,0236)",
"(300A,0244)",
"(300A,0266)",
"(300A,0294)",
"(300A,02B4)",
"(300A,0350)",
"(300A,0352)",
"(300A,0354)",
"(300C,006A)",
"(300E,0008)",
"(4008,0119)"


 };



            var DicomTagsToCheck = new List<DicomTag>();
            foreach (var Tag in TagsToCheck)
            {
                ushort group = Convert.ToUInt16(Tag.Substring(1, 4), 16);
                ushort element = Convert.ToUInt16(Tag.Substring(6, 4), 16);
                DicomTag DT = new DicomTag(group, element);
                DicomTagsToCheck.Add(DT);
            }
            return DicomTagsToCheck;
        }

    }
}



        
             
   

        
