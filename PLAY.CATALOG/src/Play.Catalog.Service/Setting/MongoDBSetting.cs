using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Setting;
    public class MongoDBSetting
    {
        public string Host {get; set;}

        public int Port { get; init; }

        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }