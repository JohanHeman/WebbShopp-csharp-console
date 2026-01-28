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

        private static MongoClient GetClient()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var connString = config["MySettings:MongoConnection"];

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connString));
            var client = new MongoClient(settings);

            return client;
        }


        internal static IMongoCollection<PaymentLog> GetConnectionPayment()
        {
            var client = GetClient();
            var database = client.GetDatabase("WebbshoppLogs");
            return database.GetCollection<PaymentLog>("PaymentLogs");
        }
    }
}
