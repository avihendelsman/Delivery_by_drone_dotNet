﻿using System;

namespace DO
{
    /// <summary>
    /// Data structure to know the package data
    /// Package ID number, sender and recipient ID,  
    /// package weight category, priority, skimmer ID,
    /// package times(times are arranged in chronological order)
    /// </summary>
    public struct Parcel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int TargetId { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public int DroneId { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? Assigned { get; set; }
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }
        public override string ToString()
        {
            return string.Format("ID is: {0,-9}\t sender ID is: {1,-9}\t target ID: {2,-9}\t" +
                "parcel Weight: {3,-5}\t  Priority: {4,-5}\t  drone id: {5,-9}\n" +
                "Request started in: {6,-12}\t  Assigned: {7,-12}\t" +
                "pick up time at: {8,-12}\t  arivel time at: {9,-12}\t"
                , Id, SenderId, TargetId, Weight, Priority, DroneId, Requested, Assigned, PickedUp, Delivered);
        }
    }
}







