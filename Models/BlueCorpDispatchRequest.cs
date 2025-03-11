using System;
using System.Collections.Generic;
using System.Text;

namespace SanfordTest.Models
{
    public class BlueCorpDispatchRequest
    {
        public string SalesOrder { get; set; }
        public string CustomerReference { get; set; }
        public string LoadId { get; set; }
        public string ContainerType { get; set; }
        public string ItemCode { get; set; }
        public string ItemQuantity { get; set; }
        public string ItemWeight { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public static string FileName => "bluecorp-3pl.csv";

        public static string ConvertContainerType(string containerType)
        {
            return containerType switch
            {
                "20RF" => "REF20",
                "40RF" => "REF40",
                "20HC" => "HC20",
                "40HC" => "HC40",
                _ => containerType
            };
        }
    }
}
