﻿using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.Models;

namespace Demo.PL.MappingProfiles
{
    public class EmployeeProfile:Profile
    {
        public EmployeeProfile()
        {
            CreateMap<EmployeeViewModel, Employee>().ReverseMap();
        }
    }
}
