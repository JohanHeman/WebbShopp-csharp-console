using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Webbshop.ModelsMDB;

namespace Webbshop.Connections
{
    internal class MDBConnection
    {
        // this first function is to get the connection to the client
        private static MongoClient GetClient()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var connString = config["MySettings:MongoConnection"];

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connString));
            var client = new MongoClient(settings);

            return client;
        }

        // these functions bellow is different connections to different collections

        public static IMongoCollection<PaymentLog> GetConnectionPayment()
        {
            var client = GetClient();
            var database = client.GetDatabase("WebbshoppLogs");
            return database.GetCollection<PaymentLog>("PaymentLogs");
        }

        public static IMongoCollection<UserLog> GetConnectionUser()
        {
            var client = GetClient();
            var database = client.GetDatabase("WebbshoppLogs");
            return database.GetCollection<UserLog>("UserLogs");
        }
        

        public static IMongoCollection<ActivityLog> GetConnectionActivity()
        {
            var client = GetClient();
            var database = client.GetDatabase("WebbshoppLogs");
            return database.GetCollection<ActivityLog>("CustomerActivity");
        }

        public static IMongoCollection<AddProduct> GetCOnnectionAddProduct()
        {
            var client = GetClient();
            var database = client.GetDatabase("WebbshoppLogs");
            return database.GetCollection<AddProduct>("NewProducts");
        }


    }
}
