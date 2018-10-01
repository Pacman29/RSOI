using System;

namespace DataBaseServer.DBO
{
    public interface IEntity
    {
        int Id { get; set; }
        public DateTime changed { get; set; }
    }
}