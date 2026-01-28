using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webbshop.Connections;
using Webbshop.ModelsMDB;

namespace Webbshop
{
    internal class MongoQueries
    {
        public static void InsertPaymentLog(PaymentLog log)
        {
            var collection = MDBConnection.GetConnectionPayment();
            collection.InsertOne(log);
        }

        public static void InsertUserLog(UserLog log)
        {
            var collection = MDBConnection.GetConnectionUser();
            collection.InsertOne(log);
        }


        public static void InsertActivityLog(ActivityLog log)
        {
            var collection = MDBConnection.GetConnectionActivity();
            collection.InsertOne(log);
        }


        public static void InsertNewProductLog(AddProduct log)
        {
            var collection = MDBConnection.GetCOnnectionAddProduct();
            collection.InsertOne(log);
        }
    }
}
