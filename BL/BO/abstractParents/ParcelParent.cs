﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    /// <summary>
    /// abstract class of parcel.
    /// </summary>
    public abstract class ParcelParent
    {
        public int Id { get; set; }

        public WeightCategories Weight { get; set; }

        public Priorities Prior { get; set; }

        public override string ToString()
        {
            return string.Format("ID is:{0}\nWeight Categorie:{1}\nPrioritie:{2}\n", Id, Weight, Prior);
        }
    }
}
