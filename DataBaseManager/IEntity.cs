using System;

namespace DataBaseManager
{
    public interface IEntity
    {
        DateTime changed { get; set; }
    }
}