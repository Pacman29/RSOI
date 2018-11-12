using System;

namespace DataBaseServer.DBO
{
    public interface IEntity
    {
        DateTime changed { get; set; }
    }
}