﻿using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Annotations;


namespace SmartSql.Test.Entities
{
    public class User
    {
        public User() { }
        public User(long id)
        {
            Id = id;
        }
        public User(long id, string name)
        {
            Id = id;
            UserName = name;
        }
        public User(long id, string name, UserStatus status)
        {
            Id = id;
            UserName = name;
            Status = status;
        }
        public long Id { get; set; }
        public String UserName { get; set; }
        [Column(FieldType = typeof(String))]
        public UserInfo Info { get; set; }
        public UserStatus Status { get; set; }
    }
    public enum UserStatus
    {
        Ok = 1
    }
}
