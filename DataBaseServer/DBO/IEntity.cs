using System;

namespace DataBaseServer.DBO
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime changed { get; set; }
    }
}