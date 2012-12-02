using ImageTransport;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Media;
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
        static void Main(string[] args)
        {
            //Amazon DB
            imgTransport transportObj = new imgTransport(imgTransport.vendorDBtype.AmazonDB);
            //sendImages
            transportObj.sendImages("C:\\glassfish3\\glassfish\\docs\\api\\javax\\faces\\component\\UIComponentHierarchy.jpg");
        }
    }
}

