using Microsoft.AspNetCore.Mvc;
using Shamia.API.Dtos.Request;
using Shamia.DataAccessLayer.Entities;
using System.Text.Json.Serialization;

namespace Shamia.API.Dtos.Response
{
    public class GetProductOptionsResponse
    {
        public int Id { get; set; }
        public string Name_Ar { get; set; }
        public string Name_En { get; set; }
        public decimal Price { get; set; }
        public decimal? Offer { get; set; }
        public float Quantity_In_Unit { get; set; }
        public bool Default { get; set; }
    }


}
