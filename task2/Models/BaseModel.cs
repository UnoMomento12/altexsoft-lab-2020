﻿
namespace task2.Models
{
    abstract class BaseModel
    {
        public string ID { get; set; } = default;
        public string Name { get; set; } = default;

        public BaseModel() { }
    }
}
