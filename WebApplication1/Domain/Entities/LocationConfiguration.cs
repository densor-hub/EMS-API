using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class LocationConfiguration
    {
        public Guid Id {get; private set;}
        public Guid LocationId {get; private set;}
        public bool DirectSaleOperations {get; private set;} = false;
        //public bool 

        public static bool GetDirectSaleOperations ()
        {
            return false;
        }
    }
}