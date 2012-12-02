using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Media;
using ImageTransport;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDB.DataModel;
using Amazon.DynamoDB.DocumentModel;
using System.Xml.Serialization;
using System.Xml;
using Amazon.DynamoDB;
using Amazon.DynamoDB.Model;
using Amazon.SecurityToken;
using Amazon.Runtime;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using System.Net;
using System.Drawing;

namespace ImageTransport
{
    public class imgTransport
    {
        //CONSTANTS
        public const int PART_SIZE = 1000; //If we want to ajdust the part size

        //Starts on creation
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        static ArrayList masterList = new ArrayList();

        //What is the computer's name?
        public String machineName = Environment.MachineName;

        //Percentage done; for progress bar
        public int percentDone = 0;
        public int failCount = 0;
        public int successCount = 0;

        public String compID = "1";
        public String indexID = "1";

        //What database are we using?
        static vendorDBtype vendorDB = vendorDBtype.AmazonDB;

        //Debug
        public const bool DEBUG_ON = true;

        public imgTransport(vendorDBtype _vendorDB)
        {
            //Set the vendor
            vendorDB = _vendorDB;

            //Does the computer already exist in the database? (queue database)
            if (computer_exists())
            {
                //Populate masterList with index
                masterList = populateMasterList(); //Not finished
                //Check database for each entry, starting at the beginning - remove entries already in database?
                removeRedundancy(masterList); //Not finished
                //Send Images
                foreach (String filename in masterList)
                {
                    sendImages(filename);
                }
            }
            else
            {
                //If it does not, first enter machineName into database
                sendMachineName();
                //Create an index to pass over.
                generateIndex(); //writes to masterList
                //Send Index
                sendIndex(generateXMLIndex());
                //Send Images
                foreach (String filename in masterList)
                {
                    sendImages(filename);
                }
            }
        }

        private void removeRedundancy(ArrayList masterList)
        {
            throw new NotImplementedException();
        }

        private ArrayList populateMasterList()
        {
            //Pull XML string from database
            String XML = "";
            switch (vendorDB)
            {
                case vendorDBtype.AmazonDB:
                    XML = populateAmazonDB_ML();
                    break;
                case vendorDBtype.Dynamo:
                    break;
                case vendorDBtype.Firebase:
                    break;
                case vendorDBtype.Google:
                    break;
                case vendorDBtype.Heroku:
                    break;
                case vendorDBtype.Mongo:
                    break;
            }


            throw new NotImplementedException();
        }

        private string populateAmazonDB_ML()
        {
            throw new NotImplementedException();
        }

        private void sendImages()
        {
            foreach (String filename in masterList)
            {
                sendImages(filename);
            }
            throw new NotImplementedException();
        }

        private void sendMachineName()
        {
            //Create new record of machineName
            switch (vendorDB)
            {
                case vendorDBtype.AmazonDB:
                    sendAmazonSimpleMachineName();
                    break;
                case vendorDBtype.Dynamo:
                    break;
                case vendorDBtype.Firebase:
                    break;
                case vendorDBtype.Google:
                    break;
                case vendorDBtype.Heroku:
                    break;
                case vendorDBtype.Mongo:
                    break;
            }
            throw new NotImplementedException();
        }


        void generateIndex()
        {
            // Start with drives if you have to search the entire computer. 
            string[] drives = System.Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);

                // Here we skip the drive if it is not ready to be read. This 
                // is not necessarily the appropriate action in all scenarios. 
                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTree(rootDir);
            }

            // Write out all the files that could not be processed.
            StringBuilder sb = new StringBuilder();
            // Never hurts to write to console.
            Console.WriteLine("Files with restricted access:");
            foreach (string s in log)
            {
                Console.WriteLine(s);
                sb.AppendLine(s);
            }

            using (StreamWriter outfile = new StreamWriter(@"\UserInputFile.txt", true))
            {
                outfile.Write(sb.ToString());
            }

            // Keep the console window open in debug mode.
            if (DEBUG_ON)
            {
                Console.WriteLine("Press any key");
                Console.ReadKey();
            }

        }

        //private static void AddFiles(string path, IList<string> files)
        //{
        //    try
        //    {
        //        Directory.GetFiles(path)
        //            .ToList()
        //            .ForEach(s => files.Add(s));

        //        Directory.GetDirectories(path)
        //            .ToList()
        //            .ForEach(s => AddFiles(s, files));
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        // ok, so we are not allowed to dig into that directory. Move on.
        //    }
        //}

        ///File Searching / Picture indexing
        ///
        static void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            //MasterList contains all JPGs found

            // First, process all the files directly under this folder 
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse. 
                // You may decide to do something different here. For example, you 
                // can try to elevate your privileges and access the file again.
                log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {

                foreach (System.IO.FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().
                    Console.WriteLine(fi.FullName);
                    //Dim List() as string = Directory.GetFiles(Path, "*.jpg")
                    try
                    {
                        //string[] List = Directory.GetFiles(fi.FullName, GetImageFiles(fi.FullName).ToString());
                        //masterList.AddRange(List);
                        //DirectoryInfo dir = new DirectoryInfo(fi.FullName);
                        //FileInfo[] imgFiles = dir.GetFiles("*.jpg");
                        if (fi.FullName.ToString().Contains("jpg"))
                        {
                            masterList.Add(fi.FullName.ToString());
                        }
                    }
                    catch (IOException)
                    {
                        //Do nothing
                    }
                    catch
                    {
                        Console.WriteLine("Non-IO error");
                    }

                    //Just for debugging purposes (as it takes a while to index all the files)
                    if ((DEBUG_ON) && (masterList.Count > 10))
                    {
                        break;
                    }
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }


        }

        //private static IEnumerable<string> GetImageFiles(string sourceFolder)
        //{
        //    return from file in System.IO.Directory.EnumerateFiles(sourceFolder)
        //           let extension = Path.GetExtension(file)
        //           where extension == ".jpg" || extension == ".gif" || extension == ".png"
        //           select file;
        //}

        //void consoleRun()
        //{
        //    imgTransport transportObj = new imgTransport();
        //    Console.Write("Machine name: " + transportObj.machineName);


        //}

        //void directorySearchTest(String startPath)
        //{
        //    //string startPath = @"C:\";
        //    string[] oDirectories = Directory.GetDirectories(startPath, "xml", SearchOption.AllDirectories);
        //    Console.WriteLine(oDirectories.Length.ToString());
        //    foreach (string oCurrent in oDirectories)
        //        Console.WriteLine(oCurrent);
        //    Console.ReadLine();
        //}

        public String generateXMLIndex()
        {
            //Does this XML need to be formatted using a specific standard?
            StringBuilder xml = new StringBuilder();

            xml.AppendLine("<Index>");
            if (machineName != null)
            {
                xml.AppendLine("<MachineName>");
                xml.AppendLine(machineName);
                xml.AppendLine("</MachineName>");
            }
            foreach (String filename in masterList)
            {
                //Other information could be added to this section (Filesize, etc)
                xml.AppendLine("<File>" + filename + "</File>");
            }

            xml.AppendLine("</Index>");
            return xml.ToString();
        }

        /// <summary>
        /// SENDING; Transport Layer
        /// </summary>
        /// 
        
        //Generic Sending Mechanisms
        public void sendImages(String filename)
        {
            Image image = Image.FromFile(filename);
            System.Drawing.Imaging.ImageFormat format = image.RawFormat;
            string picture = ImageToBase64(image, format);
            List<String> partArray = choppedPictures(picture);

            int partNo = 0;
            foreach (String part in partArray)
            {
                if (!sendImage(filename, partNo.ToString(), part))
                {
                    Console.WriteLine("Error sending file.");
                    //Create method to capture and calculate errors.

                }
                partNo++;
            }

        }



        //Amazon
        public void sendAmazonSimpleMachineName()
        {
            AmazonSimpleDB sdb = AWSClientFactory.CreateAmazonSimpleDBClient(RegionEndpoint.USWest2);

            try
            {
                String domainName = "";

                CreateDomainRequest createDomain = (new CreateDomainRequest()).WithDomainName("Computer");
                sdb.CreateDomain(createDomain);

                // Putting data into a domain
                domainName = "Computer";

                String itemNameOne = "1";
                PutAttributesRequest putAttributesActionOne = new PutAttributesRequest().WithDomainName(domainName).WithItemName(itemNameOne);
                List<ReplaceableAttribute> attributesOne = putAttributesActionOne.Attribute;
                attributesOne.Add(new ReplaceableAttribute().WithName("compID").WithValue(machineName));
                attributesOne.Add(new ReplaceableAttribute().WithName("compName").WithValue(machineName));
                sdb.PutAttributes(putAttributesActionOne);

            }
            catch (AmazonSimpleDBException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
            }

            Console.WriteLine("Press Enter to continue...");
            Console.Read();

        }
        public void sendIndex(String XML)
        {
            //sendAmazonSimpleDbIndex(generateXMLIndex());
            switch (vendorDB)
            {
                case vendorDBtype.AmazonDB:
                    sendAmazonSimpleDbIndex(XML);
                    break;
                case vendorDBtype.Dynamo:
                    break;
                case vendorDBtype.Firebase:
                    break;
                case vendorDBtype.Google:
                    break;
                case vendorDBtype.Heroku:
                    break;
                case vendorDBtype.Mongo:
                    break;
            }
        }
        public void sendAmazonSimpleDbIndex(String XML)
        {
            //Image image = Image.FromFile(raw.Text);
            //System.Drawing.Imaging.ImageFormat format = image.RawFormat;

            AmazonSimpleDB sdb = AWSClientFactory.CreateAmazonSimpleDBClient(RegionEndpoint.USWest2);

            try
            {

                String domainName = "";

                CreateDomainRequest createDomain2 = (new CreateDomainRequest()).WithDomainName("index");
                sdb.CreateDomain(createDomain2);

                domainName = "index";
                String itemNameTwo = "1";
                PutAttributesRequest putAttributesActionTwo = new PutAttributesRequest().WithDomainName(domainName).WithItemName(itemNameTwo);
                List<ReplaceableAttribute> attributesTwo = putAttributesActionTwo.Attribute;
                attributesTwo.Add(new ReplaceableAttribute().WithName("indexID").WithValue(indexID));
                attributesTwo.Add(new ReplaceableAttribute().WithName("compID").WithValue(machineName));
                attributesTwo.Add(new ReplaceableAttribute().WithName("XML_Profile").WithValue(XML));

                sdb.PutAttributes(putAttributesActionTwo);

            }
            catch (AmazonSimpleDBException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
            }

            //Console.WriteLine("Press Enter to continue...");
            //Console.Read();
        }
        public static bool sendImage(String filename, String partNo, String part)
        {
            try
            {
                switch (vendorDB)
                {
                    case vendorDBtype.AmazonDB:
                        sendAmazonSimpleDbImage(filename, partNo, part);
                        break;
                    case vendorDBtype.Dynamo:
                        break;
                    case vendorDBtype.Firebase:
                        break;
                    case vendorDBtype.Google:
                        break;
                    case vendorDBtype.Heroku:
                        break;
                    case vendorDBtype.Mongo:
                        break;
                }
                return true;
            }catch(Exception ex){
                //Error message indicating ex
                Console.WriteLine("Exception: " + ex);
                return false;
            }
        }
        public static bool sendAmazonSimpleDbImage(String filename, String partNo, String part)
        {
            AmazonSimpleDB sdb = AWSClientFactory.CreateAmazonSimpleDBClient(RegionEndpoint.USWest2);
            try
            {
                String domainName = "";

                CreateDomainRequest createDomain3 = (new CreateDomainRequest()).WithDomainName("Images");
                sdb.CreateDomain(createDomain3);

                domainName = "Images";
                String itemNameThree = partNo.ToString();
                PutAttributesRequest putAttributesActionThree = new PutAttributesRequest().WithDomainName(domainName).WithItemName(itemNameThree);
                List<ReplaceableAttribute> attributesThree = putAttributesActionThree.Attribute;
                attributesThree.Add(new ReplaceableAttribute().WithName("ImgID").WithValue("TestImage01"));
                attributesThree.Add(new ReplaceableAttribute().WithName("indexID").WithValue("1"));
                attributesThree.Add(new ReplaceableAttribute().WithName("Extension").WithValue("jpg"));
                attributesThree.Add(new ReplaceableAttribute().WithName("location").WithValue(filename));
                attributesThree.Add(new ReplaceableAttribute().WithName("imgPart").WithValue(partNo.ToString()));
                attributesThree.Add(new ReplaceableAttribute().WithName("raw").WithValue(part));
                sdb.PutAttributes(putAttributesActionThree);
            }
            catch (AmazonSimpleDBException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);

                return false;
            }

            return true;
        }

        //IMAGE CONVERSION///////

        private string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        private Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        private static List<String> choppedPictures(String picture)
        {
            string chopPicture = "";
            int partNo = (int)Math.Round(Convert.ToDecimal(picture.Length / PART_SIZE));
            int partStart = 0;
            int modpicture = picture.Length % PART_SIZE;
            List<String> partArray = new List<String>();

            //Chop the image 
            if (picture.Length > PART_SIZE)
            {
                for (int i = 0; i < partNo - 1; i++)
                {
                    chopPicture = picture.Substring(partStart, PART_SIZE);
                    partArray.Add(chopPicture);
                    partStart += PART_SIZE;
                }
                chopPicture = picture.Substring(partStart, modpicture);
            }
            else
            {
                partArray.Add(picture);
            }

            return partArray;
        }

        public enum vendorDBtype : int
        {
            AmazonDB, Dynamo, Google, Mongo, Heroku, Firebase
        }

        //Success / Fail

        public int failRate()
        {   //Calculate failrate
            return ((failCount / masterList.Count) * 100);
        }

        private Boolean computer_exists()
        {
            //Needs to query database to see if machineName exists there
            //// Should also check if index exists

            switch (vendorDB)
            {
                case vendorDBtype.AmazonDB:
                    break;
                case vendorDBtype.Dynamo:
                    break;
                case vendorDBtype.Firebase:
                    break;
                case vendorDBtype.Google:
                    break;
                case vendorDBtype.Heroku:
                    break;
                case vendorDBtype.Mongo:
                    break;
            }
            return false;
        }
    }
}
