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

    }
}
