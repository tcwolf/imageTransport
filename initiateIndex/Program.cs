using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Media;
using ImageTransport;
using System.Collections;
//using System.Drawing;
//using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDB.DataModel;
using Amazon.DynamoDB.DocumentModel;
using System.Xml.Serialization;
//using System.IO
using System.Xml;
using Amazon.DynamoDB;
using Amazon.DynamoDB.Model;
using Amazon.SecurityToken;
using Amazon.Runtime;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using System.Net;
using System.Drawing;
//using System.Collections.Generic;

namespace initiateIndex
{
    class Program
    {
        //CONSTANTS
        //public const int PART_SIZE = 1000; //If we want to ajdust the part size

        //static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        //static ArrayList masterList = new ArrayList();
        //static String machineName = Environment.MachineName;
        //static String indexID = "1";
        //static String testXML = "<Index><File>C:\test.jpg</File></Index>";

        static void Main(string[] args)
        {
            //Amazon DB
            imgTransport transportObj = new imgTransport(imgTransport.vendorDBtype.AmazonDB);
            //sendImages
            transportObj.sendImages("C:\\glassfish3\\glassfish\\docs\\api\\javax\\faces\\component\\UIComponentHierarchy.jpg");


        }
        //public static void sendImages(String filename)
        //{
        //    Image image = Image.FromFile(filename);
        //    System.Drawing.Imaging.ImageFormat format = image.RawFormat;
        //    string picture = ImageToBase64(image, format);
        //    List<String> partArray = choppedPictures(picture);

        //    int partNo = 0;
        //    foreach (String part in partArray)
        //    {
        //        if (!sendAmazonSimpleDbImage(filename, partNo.ToString(), part))
        //        {
        //            Console.WriteLine("Error sending file.");
        //            //Create method to capture and calculate errors.

        //        }
        //        partNo++;
        //    }

        //}

        //public static bool sendAmazonSimpleDbImage(String filename, String partNo, String part)
        //{
        //    AmazonSimpleDB sdb = AWSClientFactory.CreateAmazonSimpleDBClient(RegionEndpoint.USWest2);
        //    try
        //    {
        //        String domainName = "";

        //        CreateDomainRequest createDomain3 = (new CreateDomainRequest()).WithDomainName("Images");
        //        sdb.CreateDomain(createDomain3);

        //        domainName = "Images";
        //        String itemNameThree = partNo.ToString();
        //        PutAttributesRequest putAttributesActionThree = new PutAttributesRequest().WithDomainName(domainName).WithItemName(itemNameThree);
        //        List<ReplaceableAttribute> attributesThree = putAttributesActionThree.Attribute;
        //        attributesThree.Add(new ReplaceableAttribute().WithName("ImgID").WithValue("TestImage01"));
        //        attributesThree.Add(new ReplaceableAttribute().WithName("indexID").WithValue("1"));
        //        attributesThree.Add(new ReplaceableAttribute().WithName("Extension").WithValue("jpg"));
        //        attributesThree.Add(new ReplaceableAttribute().WithName("location").WithValue(filename));
        //        attributesThree.Add(new ReplaceableAttribute().WithName("imgPart").WithValue(partNo.ToString()));
        //        attributesThree.Add(new ReplaceableAttribute().WithName("raw").WithValue(part));
        //        sdb.PutAttributes(putAttributesActionThree);
        //    }
        //    catch (AmazonSimpleDBException ex)
        //    {
        //        Console.WriteLine("Caught Exception: " + ex.Message);
        //        Console.WriteLine("Response Status Code: " + ex.StatusCode);
        //        Console.WriteLine("Error Code: " + ex.ErrorCode);
        //        Console.WriteLine("Error Type: " + ex.ErrorType);
        //        Console.WriteLine("Request ID: " + ex.RequestId);
        //        Console.WriteLine("XML: " + ex.XML);

        //        return false;
        //    }

        //    return true;
        //}
        ////IMAGE CONVERSION///////

        //public static List<String> choppedPictures(String picture)
        //{
        //    string chopPicture = "";
        //    int partNo = (int)Math.Round(Convert.ToDecimal(picture.Length / PART_SIZE));
        //    int partStart = 0;
        //    int modpicture = picture.Length % PART_SIZE;
        //    List<String> partArray = new List<String>();

        //    //Chop the image 
        //    if (picture.Length > PART_SIZE)
        //    {
        //        for (int i = 0; i < partNo - 1; i++)
        //        {
        //            chopPicture = picture.Substring(partStart, PART_SIZE);
        //            partArray.Add(chopPicture);
        //            partStart += PART_SIZE;
        //        }
        //        chopPicture = picture.Substring(partStart, modpicture);
        //    }
        //    else
        //    {
        //        partArray.Add(picture);
        //    }

        //    return partArray;
        //}

        //public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        // Convert Image to byte[]
        //        image.Save(ms, format);
        //        byte[] imageBytes = ms.ToArray();

        //        // Convert byte[] to Base64 String
        //        string base64String = Convert.ToBase64String(imageBytes);
        //        return base64String;
        //    }
        //}
        //public static Image Base64ToImage(string base64String)
        //{
        //    // Convert Base64 String to byte[]
        //    byte[] imageBytes = Convert.FromBase64String(base64String);
        //    MemoryStream ms = new MemoryStream(imageBytes, 0,
        //      imageBytes.Length);

        //    // Convert byte[] to Image
        //    ms.Write(imageBytes, 0, imageBytes.Length);
        //    Image image = Image.FromStream(ms, true);
        //    return image;
        //}
    }

}

