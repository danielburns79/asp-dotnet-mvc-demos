using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

namespace SqlDemo.Models
{
    public class Enrolement
    {
        // the key & column attributes are not needed for entity framework composite keys
        // entity framework composite keys require fluent api, see enrolemententityframeworkrespository
        //Key]
        //[Column(Order = 1)]
        public Guid ClassId { get; set; }
        //[Key]
        //[Column(Order = 2)]
        public Guid UserId { get; set; }
        public int Role { get; set; }

        override public bool Equals(object obj)
        {
            Enrolement other = obj as Enrolement;
            if (other == null)
            {
                return false;
            }
            return other.ClassId == this.ClassId && other.UserId == this.UserId;
        }
        override public int GetHashCode()
        {
            return this.ClassId.GetHashCode() + this.UserId.GetHashCode();
        }
    }
}