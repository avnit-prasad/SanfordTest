using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace SanfordTest.Models
{
    public class ReadyForDispatch
    {
        [Required]
        [JsonProperty("controlNumber")]
        public int ControlNumber { get; set; }

        [Required]
        [JsonProperty("salesOrder")]
        public string SalesOrder { get; set; }

        [Required]
        [JsonProperty("containers")]
        public List<Container> Containers { get; set; }

        [Required]
        [JsonProperty("deliveryAddress")]
        public DeliveryAddress DeliveryAddress { get; set; }
    }

    public class Container
    {
        [Required]
        [JsonProperty("loadId")]
        public string LoadId { get; set; }

        [Required]
        [EnumDataType(typeof(ContainerType))] // Enum validation for container type
        [JsonProperty("containerType")]
        public string ContainerType { get; set; }

        [Required]
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [Required]
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }

        [Required]
        [Range(1, double.MaxValue)] // Quantity should be positive
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.1, double.MaxValue)] // CartonWeight should be greater than 0
        [JsonProperty("cartonWeight")]
        public decimal CartonWeight { get; set; }
    }

    public class DeliveryAddress
    {
        [Required]
        [StringLength(100)] 
        [JsonProperty("street")]
        public string Street { get; set; }

        [Required]
        [StringLength(50)] 
        [JsonProperty("city")]
        public string City { get; set; }

        [Required]
        [StringLength(50)] 
        [JsonProperty("state")]
        public string State { get; set; }

        [Required]
        [StringLength(20)] 
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(50)] 
        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public enum ContainerType
    {
        [EnumMember(Value = "20RF")]
        [JsonProperty("20RF")]
        TwentyRF,

        [EnumMember(Value = "40RF")]
        [JsonProperty("40RF")]
        FortyRF,

        [EnumMember(Value = "20HC")]
        [JsonProperty("20HC")]
        TwentyHC,

        [EnumMember(Value = "40HC")]
        [JsonProperty("40HC")]
        FortyHC
    }
}
