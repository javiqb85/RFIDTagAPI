using System;
using System.Collections.Generic;
using System.Text;
using Azure.Data.Tables;
namespace RFIDTagAPI
{
    static class Shared
    {
        public static string AzureTablename = "RFIDtbl";
        public static string PartitionKey = "partition1";
        public static string AzureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=javclestorage1;AccountKey=FUkwZZcuiftc0vucMnu7BTbftlhZJ4A/NPReMxw51DPC8jRgf7vDGiOK24qJptnU6S7cyABdNQSu+AStwsk4aQ==;EndpointSuffix=core.windows.net";
    }
    public class RFIDEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; } // used as RFID
        public DateTimeOffset? Timestamp { get; set; }
        public Azure.ETag ETag { get; set; }

    }
}
