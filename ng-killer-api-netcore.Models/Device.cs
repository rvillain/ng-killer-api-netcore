using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NgKillerApiCore.Models
{
    public class Device : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PushEndpoint { get; set; }
        public string PushP256DH { get; set; }
        public string PushAuth { get; set; }
    }
}