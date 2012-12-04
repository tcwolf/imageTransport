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
using System.Reflection;

namespace ImageTransport
{
    public class imgTransport
    {
        //Initialize the log object
        //public string log = "";

        //CONSTANTS
        public const int PART_SIZE = 1000; //If we want to ajdust the part size

        //Starts on creation
        //static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        static ArrayList masterList = new ArrayList();

        //What is the computer's name?
        public String machineName = Environment.MachineName;

        //Percentage done; for progress bar
        public int percentDone = 0;
        public int failCount = 0;
        public int successCount = 0;

        //These need to be loaded from the database
        public String compID = "1";
        public String indexID = "1";
        public int itemNo = 0;

        //What database are we using?
        static vendorDBtype vendorDB = vendorDBtype.AmazonDB;

        //Vendor related variables
        public static AmazonDynamoDB client;

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
                    sendDynamoMachineName();
                    break;
                case vendorDBtype.Firebase:
                    throw new NotImplementedException();
                //break;
                case vendorDBtype.Google:
                    throw new NotImplementedException();
                //break;
                case vendorDBtype.Heroku:
                    throw new NotImplementedException();
                //break;
                case vendorDBtype.Mongo:
                    throw new NotImplementedException();
                //break;
            }
        }

        private void sendDynamoMachineName()
        {

            //Image image = Image.FromFile(raw.Text);

            //System.Drawing.Imaging.ImageFormat format = image.RawFormat;
            //string picture = ImageToBase64(image, format);
            string[] SampleTables = new string[] { "Computer", "Index", "Images" };

            Console.WriteLine("Getting list of tables");
            List<string> currentTables = client.ListTables().ListTablesResult.TableNames;
            Console.WriteLine("Number of tables: " + currentTables.Count);
            client = new AmazonDynamoDBClient(RegionEndpoint.USWest2);
            bool tablesAdded = false;
            if (!currentTables.Contains("Computer"))
            {
                Console.WriteLine("Computer table does not exist, creating");
                client.CreateTable(new CreateTableRequest
                {
                    TableName = "Computer",
                    ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10, WriteCapacityUnits = 10 },
                    KeySchema = new KeySchema
                    {
                        HashKeyElement = new KeySchemaElement { AttributeName = "compID", AttributeType = "S" },

                    }
                });
                tablesAdded = true;
            }
            //if (!currentTables.Contains("Index"))
            //{
            //    Console.WriteLine("Index table does not exist, creating");
            //    client.CreateTable(new CreateTableRequest
            //    {
            //        TableName = "Index",
            //        ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10, WriteCapacityUnits = 10 },
            //        KeySchema = new KeySchema
            //        {

            //            HashKeyElement = new KeySchemaElement { AttributeName = "indexID", AttributeType = "S" }
            //        }
            //    });
            //    tablesAdded = true;
            //}
            //if (!currentTables.Contains("Images"))
            //{
            //    Console.WriteLine("Images table does not exist, creating");
            //    client.CreateTable(new CreateTableRequest
            //    {
            //        TableName = "Images",
            //        ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10, WriteCapacityUnits = 10 },
            //        KeySchema = new KeySchema
            //        {
            //            HashKeyElement = new KeySchemaElement { AttributeName = "imgeID", AttributeType = "S" },

            //        }
            //    });
            //    tablesAdded = true;
            //}

            if (tablesAdded)
            {
                while (true)
                {
                    bool allActive = true;
                    foreach (var table in SampleTables)
                    {
                        string tableStatus = GetTableStatus(table);
                        bool isTableActive = string.Equals(tableStatus, "ACTIVE", StringComparison.OrdinalIgnoreCase);
                        if (!isTableActive)
                            allActive = false;
                    }
                    if (allActive)
                    {
                        Console.WriteLine("All tables are ACTIVE");
                        break;
                    }


                }
            }

            //Console.WriteLine("All sample tables created");


            //Console.WriteLine("Creating the context object");
            DynamoDBContext context = new DynamoDBContext(client);

            //Console.WriteLine("Creating actors");
            Computer newcomputer = new Computer
            {
                compID1 = compID,
                compName1 = machineName

            };
            //Index newIndex = new Index
            //{
            //    indexID1 = indexID.Text,
            //    compID2 = CompID.Text,
            //    XMLProfile1 = XML.Text
            //};
            //Images newImage = new Images
            //{
            //    imgID1 = ImgID.Text,
            //    indexID1 = indexID.Text,
            //    extension1 = Extension.Text,
            //    location1 = Location.Text,
            //    raw1 = picture
            //};


        }
        [DynamoDBTable("Computer")]
        public class Computer
        {
            [DynamoDBHashKey]
            public string compID1 { get; set; }



            [DynamoDBProperty(AttributeName = "compName")]
            public string compName1 { get; set; }




        }
        [DynamoDBTable("Index")]
        public class Index
        {
            [DynamoDBHashKey]
            public string indexID1 { get; set; }



            [DynamoDBProperty(AttributeName = "compID")]
            public string compID2 { get; set; }
            [DynamoDBProperty(AttributeName = "XMLProfile")]
            public string XMLProfile1 { get; set; }



        }
        [DynamoDBTable("Images")]
        public class Images
        {
            [DynamoDBHashKey]
            public string imgID1 { get; set; }



            [DynamoDBProperty(AttributeName = "indexID")]
            public string indexID1 { get; set; }
            [DynamoDBProperty(AttributeName = "extension")]
            public string extension1 { get; set; }
            [DynamoDBProperty(AttributeName = "location")]
            public string location1 { get; set; }
            [DynamoDBProperty(AttributeName = "raw")]
            public string raw1 { get; set; }


        }
        private static string GetTableStatus(string tableName)
        {
            try
            {
                var table = client.DescribeTable(new DescribeTableRequest { TableName = tableName }).DescribeTableResult.Table;
                return (table == null) ? string.Empty : table.TableStatus;
            }
            catch (AmazonDynamoDBException db)
            {
                if (db.ErrorCode == "ResourceNotFoundException")
                    return string.Empty;
                throw;
            }
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
                    //Console.WriteLine("The drive {0} could not be read", di.Name);
                    log("The drive " + di.Name + " could not be read.");
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTree(rootDir);
            }

            // Write out all the files that could not be processed.
            StringBuilder sb = new StringBuilder();
            using (StreamWriter outfile = new StreamWriter(@"\UserInputFile.txt", true))
            {
                outfile.Write(sb.ToString());
            }
        }

        ///File Searching / Picture indexing
        ///
        void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            //MasterList contains all JPGs found

            // First, process all the files directly under this folder 
            String test = "";
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                //log("WalkDirectoryTree: " + e.Message);
                //failCount++;

            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                //Console.WriteLine(e.Message);
                //log(e.Message);
            }

            if (files != null)
            {

                foreach (System.IO.FileInfo fi in files)
                {
                    //Console.WriteLine(fi.FullName);

                    try
                    {
                        //Specified search pattern here; used jpg
                        if (fi.FullName.ToString().Contains("jpg"))
                        {
                            masterList.Add(fi.FullName.ToString());
                        }
                    }
                    catch (IOException ex)
                    {
                        //IOException
                        log(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //Non-IO error
                        log(ex.Message);
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
                    //Create method to capture and calculate errors.
                    log("Error sending file.");
                }
                partNo++;
            }

            image.Dispose();
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
                log(".........AmazonSimpleDBException.........");
                log("Caught Exception: " + ex.Message);
                log("Response Status Code: " + ex.StatusCode);
                log("Error Code: " + ex.ErrorCode);
                log("Error Type: " + ex.ErrorType);
                log("Request ID: " + ex.RequestId);
                log("XML: " + ex.XML);
            }
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
                failCount++;
                log("Caught Exception: " + ex.Message);
                log("Response Status Code: " + ex.StatusCode);
                log("Error Code: " + ex.ErrorCode);
                log("Error Type: " + ex.ErrorType);
                log("Request ID: " + ex.RequestId);
                log("XML: " + ex.XML);

            }

            //Console.WriteLine("Press Enter to continue...");
            //Console.Read();
        }
        public bool sendImage(String filename, String partNo, String part)
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
            }
            catch (Exception ex)
            {
                //Error message indicating ex
                log("Exception: " + ex);
                return false;
            }
        }
        public bool sendAmazonSimpleDbImage(String filename, String partNo, String part)
        {
            AmazonSimpleDB sdb = AWSClientFactory.CreateAmazonSimpleDBClient(RegionEndpoint.USWest2);
            try
            {
                String domainName = "";

                CreateDomainRequest createDomain3 = (new CreateDomainRequest()).WithDomainName("Images");
                sdb.CreateDomain(createDomain3);

                domainName = "Images";
                String itemNameThree = itemNo.ToString();
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
                failCount++;
                log("Caught Exception: " + ex.Message);
                log("Response Status Code: " + ex.StatusCode);
                log("Error Code: " + ex.ErrorCode);
                log("Error Type: " + ex.ErrorType);
                log("Request ID: " + ex.RequestId);
                log("XML: " + ex.XML);

                return false;
            }
            itemNo++;
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

        public String failRate()
        {   //Calculate failrate
            return ("((Fail Count: " + failCount + "))((Index Count: " + masterList.Count + "))");
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

        public void log(String entry)
        {
            // Write the log to a file.
            //System.IO.StreamWriter file = new System.IO.StreamWriter("C://Users//user//Documents//Visual Studio 2010//Projects//ImageTransport//ImageTransport//trunk//ImageTransport//logs//log.txt");
            System.IO.StreamWriter file = File.AppendText("C://Users//user//Documents//Visual Studio 2010//Projects//ImageTransport//ImageTransport//trunk//ImageTransport//logs//log.txt");
            file.Write("---" + DateTime.Now.ToString() + "---");
            file.Write(failRate().ToString());
            file.WriteLine(entry);
            file.Close();
        }
    }
}
