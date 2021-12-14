﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    //לקוח בחבילה
    /// <summary>
    /// class of customer in delivery.
    /// </summary>
    public class CustomerInDelivery
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("Id of the customer is: {0, -3} \tName of the customer is: {1, -3}\n", Id, Name);
        }
    }
}

